#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityDefinition.cs
// �ļ�����������ͨ���������ݼܹ����ԣ���֯ȫ���ܹ���Ϣ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110412
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// ͨ���������ݼܹ����ԣ���֯ȫ���ܹ���Ϣ��
    /// </summary>
    internal sealed partial class EntityDefinition : IEquatable<EntityDefinition>
    {
        #region ˽���ֶ�

        private readonly Type m_type;
        private readonly String m_tableName;
        private readonly Boolean m_isStoredProcedure;
        private readonly LoadStrategyAttribute m_loadStrategy;


        private readonly EntityPropertyDefinitionCollection m_properties;
        private readonly EntityPropertyDefinition[] m_foreignReferenceProperties;
        private readonly ColumnDefinitionCollection m_columns;
        private readonly ColumnDefinitionCollection m_primaryKeys;

        private NativePrimaryKeyInfo m_nativePrimaryKeyInfo;

        private IEntityDefinitionProvider m_provider;

        // ���������ⲿ�������ԣ����ڵڶ��׶ι���
        private List<EntityForeignReferencePropertyDefinition> m_allForeignReferencePropertiesInternal;
        private Dictionary<EntityForeignReferencePropertyDefinition, String[]> m_childColumnNamesSet;
        private Dictionary<EntityForeignReferencePropertyDefinition, String[]> m_parentColumnNamesSet;

        #endregion

        #region ���캯��

        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="entityType">ʵ�����͡�</param>
        public EntityDefinition(Type entityType)
        {
            Debug.Assert((entityType != null), "ʵ�����Ͳ�������Ϊ�ա�");
            Debug.Assert(
                    Attribute.IsDefined(entityType, typeof(TableAttribute)),
                    String.Format("ʵ������ {0} Ӧ��� TableAttribute ���ԡ�", entityType.FullName)
                );

            m_type = entityType;

            TableAttribute tableAttr = (TableAttribute)Attribute.GetCustomAttribute(entityType, typeof(TableAttribute));

            m_tableName = tableAttr.Name;
            m_isStoredProcedure = tableAttr.IsStoredProcedure;

            m_loadStrategy = (LoadStrategyAttribute)Attribute.GetCustomAttribute(entityType, typeof(LoadStrategyAttribute));

            if (m_loadStrategy == null)
            {
                m_loadStrategy = new LoadStrategyAttribute(LoadStrategyOption.Auto);
            }

            // �������Ժ���
            List<EntityPropertyDefinition> allEntityProperties = new List<EntityPropertyDefinition>();
            List<ColumnDefinition> allColumnDefinitions = new List<ColumnDefinition>();

            PropertyInfo[] allPropertyInfos = entityType.GetProperties(CommonPolicies.PropertyBindingFlags);

            foreach (PropertyInfo propertyInfo in allPropertyInfos)
            {
                if (Attribute.IsDefined(propertyInfo, typeof(ColumnAttribute)))
                {
                    ExtractPrimitiveProperty(propertyInfo, allEntityProperties, allColumnDefinitions);
                }
                else if (Attribute.IsDefined(propertyInfo, typeof(ForeignReferenceAttribute)))
                {
                    ExtractForeignRefrenceProperty(propertyInfo, allEntityProperties, allColumnDefinitions);
                }
                else if (Attribute.IsDefined(propertyInfo, typeof(ChildrenAttribute)))
                {
                    allEntityProperties.Add(new EntityChildrenPropertyDefinition(this, propertyInfo));
                }
            }

            // ����ʵ�����Զ��弯�Ϻ��ж��弯��
            m_properties = new EntityPropertyDefinitionCollection(this, allEntityProperties);

            if (m_allForeignReferencePropertiesInternal != null)
            {
                m_foreignReferenceProperties = m_allForeignReferencePropertiesInternal.ToArray();
            }

            m_columns = new ColumnDefinitionCollection(allColumnDefinitions);

            List<ColumnDefinition> primaryKeys = allColumnDefinitions.FindAll(
                    delegate(ColumnDefinition columnDef) { return columnDef.IsPrimaryKey; }
                );

            m_primaryKeys = new ColumnDefinitionCollection(primaryKeys);
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ�м��ϡ�
        /// </summary>
        public ColumnDefinitionCollection Columns
        {
            get { return m_columns; }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ʵ�������Դ�Ƿ�Ϊ�洢���̣�����Ϊ��
        /// </summary>
        public Boolean IsStoredProcedure
        {
            get { return m_isStoredProcedure; }
        }

        /// <summary>
        /// ��ȡ���ز��ԡ�
        /// </summary>
        public LoadStrategyAttribute LoadStrategy
        {
            get { return m_loadStrategy; }
        }

        /// <summary>
        /// ��ȡԭ��������Ϣ��
        /// </summary>
        internal NativePrimaryKeyInfo NativePrimaryKeyInfo
        {
            get
            {
                if (m_nativePrimaryKeyInfo == null)
                {
                    EntityPropertyDefinition nativePrimaryKey = Properties.GetNativePrimaryKey();
                    EntityPropertyDefinition[] candidateKeys = Properties.GetCandidateKeys();

                    NativeAttribute nativeAttr = null;

                    if (nativePrimaryKey != null)
                    {
                        nativeAttr = (NativeAttribute)Attribute.GetCustomAttribute(nativePrimaryKey.PropertyInfo, typeof(NativeAttribute));
                    }

                    Debug.Assert(
                            (nativePrimaryKey == null) || nativeAttr.AutoIncrement || (candidateKeys.Length != 0),
                            "����������Ϊԭ���ģ���Ҫô������Ϊ����������ǣ��ֶΣ�Ҫô�����Ǻ�ѡ����"
                        );

                    if (nativePrimaryKey != null)
                    {
                        m_nativePrimaryKeyInfo = new NativeActualPrimaryKeyInfo(this, nativePrimaryKey, candidateKeys);
                    }
                    else
                    {
                        m_nativePrimaryKeyInfo = NativeEmptyPrimaryKeyInfo.Value;
                    }
                }

                return m_nativePrimaryKeyInfo;
            }
        }

        /// <summary>
        /// ��ȡ�����м��ϡ�
        /// </summary>
        public ColumnDefinitionCollection PrimaryKeys
        {
            get { return m_primaryKeys; }
        }

        /// <summary>
        /// ��ȡʵ�����Լ��ϡ�
        /// </summary>
        public EntityPropertyDefinitionCollection Properties
        {
            get { return m_properties; }
        }

        /// <summary>
        /// ��ȡʵ�嶨���ṩ�ߡ�
        /// </summary>
        public IEntityDefinitionProvider Provider
        {
            get { return m_provider; }
        }

        /// <summary>
        /// ��ȡʵ�����͡�
        /// </summary>
        public Type Type
        {
            get { return m_type; }
        }

        /// <summary>
        /// ��ȡ�����ơ�
        /// </summary>
        public String TableName
        {
            get { return m_tableName; }
        }

        #endregion

        #region ��������

        /// <summary>
        /// Ϊʵ�崴����������
        /// </summary>
        /// <param name="entity">ʵ�塣</param>
        /// <returns>�����õĹ�������</returns>
        public Filter ComposeLoadFilter(Object entity)
        {
            #region ǰ������

            Debug.Assert((entity != null), "ʵ�岻��Ϊ�ա�");
            Debug.Assert((entity.GetType() == Type), "ʵ������ͱ�����ʵ�嶨���������ͬ");

            #endregion

            FilterExpression expression = new FilterExpression();

            foreach (ColumnDefinition columnDef in this.PrimaryKeys)
            {
                expression.And.Property(columnDef.PropertyChain).Is.EqualTo(columnDef.GetDbValue(entity));
            }

            return expression.Resolve();
        }

        /// <summary>
        /// ��ȡ�ⲿ�������Լ��ϡ�
        /// </summary>
        /// <returns>�ⲿ���������б�</returns>
        public EntityPropertyDefinition[] GetForeignReferenceProperties()
        {
            if (m_foreignReferenceProperties != null)
            {
                return m_foreignReferenceProperties;
            }
            else
            {
                return new EntityPropertyDefinition[0];
            }
        }

        /// <summary>
        /// ��ʾʵ�嶨����������ơ�
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return Type.Name;
        }

        #endregion

        #region �ڲ�����

        /// <summary>
        /// ��ȡ�����ⲿ���������б�
        /// </summary>
        internal List<EntityForeignReferencePropertyDefinition> AllForeignReferenceProperties
        {
            get
            {
                if (m_allForeignReferencePropertiesInternal == null)
                {
                    m_allForeignReferencePropertiesInternal = new List<EntityForeignReferencePropertyDefinition>();
                }

                return m_allForeignReferencePropertiesInternal;
            }
        }

        /// <summary>
        /// ��ȡ�ⲿ�������Ե��������б��ϣ���Ϊ�ⲿ���ԡ���
        /// </summary>
        internal Dictionary<EntityForeignReferencePropertyDefinition, String[]> ChildColumnNamesSet
        {
            get
            {
                if (m_childColumnNamesSet == null)
                {
                    m_childColumnNamesSet = new Dictionary<EntityForeignReferencePropertyDefinition, String[]>();
                }

                return m_childColumnNamesSet;
            }
        }

        /// <summary>
        /// ��ȡ�ⲿ�������Եĸ������б��ϣ���Ϊ�ⲿ���ԡ�
        /// </summary>
        internal Dictionary<EntityForeignReferencePropertyDefinition, String[]> ParentColumnNamesSet
        {
            get
            {
                if (m_parentColumnNamesSet == null)
                {
                    m_parentColumnNamesSet = new Dictionary<EntityForeignReferencePropertyDefinition, String[]>();
                }

                return m_parentColumnNamesSet;
            }
        }

        #endregion

        #region �ڲ�����

        /// <summary>
        /// �ڶ��׶ι��죬�����ⲿ�������Լ����е����ԡ�
        /// </summary>
        /// <param name="provider">ʵ�嶨���ṩ�ߡ�</param>
        internal void Initialize(IEntityDefinitionProvider provider)
        {
            m_provider = provider;

            // ʵ�岻�����ⲿ��������
            if (m_allForeignReferencePropertiesInternal == null)
            {
                return;
            }

            foreach (EntityForeignReferencePropertyDefinition entityProperty in m_allForeignReferencePropertiesInternal)
            {
                EntityDefinition parentEntityDef = provider.GetDefinition(entityProperty.Type);
                String[] childColumnNames = ChildColumnNamesSet[entityProperty];
                String[] parentColumnNames = ParentColumnNamesSet[entityProperty];

                Int32 length = childColumnNames.Length;

                ColumnDefinition[] childColumns = new ColumnDefinition[length];
                ColumnDefinition[] parentColumns = new ColumnDefinition[length];

                for (Int32 i = 0; i < length; i++)
                {
                    String childName = childColumnNames[i];
                    String parentName = parentColumnNames[i];

                    childColumns[i] = Columns[childName];
                    parentColumns[i] = parentEntityDef.Columns[parentName];
                }

                // �����ⲿ�������ԵĹ�ϵ��
                entityProperty.Relation = new EntityPropertyDefinitionRelation(childColumns, parentColumns);
            }

            // �ͷ���ʱ����
            m_allForeignReferencePropertiesInternal = null;
            m_childColumnNamesSet = null;
            m_parentColumnNamesSet = null;
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡ�ⲿ�������ԡ�
        /// </summary>
        /// <param name="propertyInfo">������Ϣ��</param>
        /// <param name="propertyList">Ҫ�����ʵ�������б�</param>
        /// <param name="columnList">Ҫ������ж����б�</param>
        private void ExtractForeignRefrenceProperty(PropertyInfo propertyInfo, List<EntityPropertyDefinition> propertyList, List<ColumnDefinition> columnList)
        {
            Boolean permitNull = Attribute.IsDefined(propertyInfo, typeof(PermitNullAttribute));
            Boolean isPrimaryKey = Attribute.IsDefined(propertyInfo, typeof(PrimaryKeyAttribute));

            if (isPrimaryKey)
            {
                permitNull = false;
            }

            EntityForeignReferencePropertyDefinition entityProperty = new EntityForeignReferencePropertyDefinition(this, propertyInfo, permitNull);

            ColumnMappingAttribute[] allColumnMappings = (ColumnMappingAttribute[])Attribute.GetCustomAttributes(
                    propertyInfo,
                    typeof(ColumnMappingAttribute)
                );

            Int32 length = allColumnMappings.Length;

            ColumnDefinition[] allColumnDefinitions = new ColumnDefinition[length];
            String[] childColumnNames = new String[length];
            String[] parentColumnNames = new String[length];

            for (Int32 i = 0; i < length; i++)
            {
                ColumnMappingAttribute mapping = allColumnMappings[i];

                ColumnDefinition columnDef = new ColumnDefinition(mapping.ChildColumnName, isPrimaryKey);
                columnDef.Property = entityProperty;

                allColumnDefinitions[i] = columnDef;
                childColumnNames[i] = mapping.ChildColumnName;
                parentColumnNames[i] = mapping.ParentColumnName;
            }

            entityProperty.Column = allColumnDefinitions[0];

            propertyList.Add(entityProperty);
            columnList.AddRange(allColumnDefinitions);

            // ������������Ϣ���ڶ��׼�����
            AllForeignReferenceProperties.Add(entityProperty);
            ChildColumnNamesSet.Add(entityProperty, childColumnNames);
            ParentColumnNamesSet.Add(entityProperty, parentColumnNames); // TODO���������������Ϊͬһ�ֶΣ���Ӧ�׳��쳣��������������޵ݹ��������ջ���������
        }

        /// <summary>
        /// ��ȡ������ֵ�����ԡ�
        /// </summary>
        /// <param name="propertyInfo">������Ϣ��</param>
        /// <param name="propertyList">Ҫ�����ʵ�������б�</param>
        /// <param name="columnList">Ҫ������ж����б�</param>
        private void ExtractPrimitiveProperty(PropertyInfo propertyInfo, List<EntityPropertyDefinition> propertyList, List<ColumnDefinition> columnList)
        {
            ColumnAttribute columnAttr = (ColumnAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ColumnAttribute));

            EntityPrimitivePropertyDefinition entityProperty = new EntityPrimitivePropertyDefinition(this, propertyInfo);
            ColumnDefinition columnDef = new ColumnDefinition(columnAttr.Name, columnAttr.IsPrimaryKey);

            columnDef.Type = propertyInfo.PropertyType;
            columnDef.DbType = columnAttr.DbType;

            columnDef.Property = entityProperty;
            entityProperty.Column = columnDef;

            propertyList.Add(entityProperty);
            columnList.Add(columnDef);
        }

        #endregion

        #region �����

        /// <summary>
        /// ��д����Զ��壬ֻҪʵ�嶨��Ļ���������ͬ������Ϊ���ߵ�ͬ��
        /// </summary>
        /// <param name="obj">��һ������</param>
        /// <returns></returns>
        public override Boolean Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            EntityDefinition other = (EntityDefinition)obj;

            return Equals(other);
        }

        /// <summary>
        /// ��д�����Ƿ��ػ������͵Ĺ�ϣֵ��
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return Type.GetHashCode();
        }

        /// <summary>
        /// ��д��
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(EntityDefinition left, EntityDefinition right)
        {
            if (Object.ReferenceEquals(left, null))
            {
                return Object.ReferenceEquals(null, right);
            }

            return left.Equals(right);
        }

        /// <summary>
        /// ��д��
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(EntityDefinition left, EntityDefinition right)
        {
            return !(left == right);
        }

        #region IEquatable<EntityDefinition> ��Ա

        /// <summary>
        /// ָʾ��ǰʵ������Ƿ��ͬ����һ��ʵ�嶨�����
        /// </summary>
        /// <param name="other">��һ��ʵ�����</param>
        /// <returns>�����ǰʵ�嶨���ͬ����һ��ʵ�嶨������򷵻� true�����򷵻� false��</returns>
        public Boolean Equals(EntityDefinition other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return Type.Equals(other.Type);
        }

        #endregion

        #endregion
    }
}