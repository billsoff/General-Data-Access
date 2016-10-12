#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityPropertyDefinition.cs
// �ļ�������������ʾһ��ʵ�����ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110413
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
    /// ��ʾһ��ʵ�����ԡ�
    /// </summary>
    internal abstract class EntityPropertyDefinition : IEquatable<EntityPropertyDefinition>
    {
        #region ˽���ֶ�

        private readonly EntityDefinition m_entity;
        private readonly PropertyInfo m_propertyInfo;

        private readonly Boolean m_lazyLoad;
        private readonly Boolean m_suppressExpand;
        private readonly Boolean m_candidateKey;
        private readonly Boolean m_autoGenerateOnNew;
        private readonly UseEnumTextAttribute m_useEnumText;
        private readonly OddValueConverterAttribute m_oddValueConverter;

        private readonly List<String> m_aliases;

        private ColumnDefinition m_column;

        #endregion

        #region ���캯��

        /// <summary>
        /// ���캯������������������ʵ�嶨���������Ϣ��
        /// </summary>
        /// <param name="entity">����������ʵ�嶨�塣</param>
        /// <param name="propertyInfo">������Ϣ��</param>
        protected EntityPropertyDefinition(EntityDefinition entity, PropertyInfo propertyInfo)
        {
            m_entity = entity;
            m_propertyInfo = propertyInfo;

            DbEntityPropertyInfo info = DbEntityPropertyInfoCache.GetProperty(entity.Type);
            m_lazyLoad = info.IsBusinessObject && Attribute.IsDefined(propertyInfo, typeof(LazyLoadAttribute));
            m_suppressExpand = Attribute.IsDefined(propertyInfo, typeof(SuppressExpandAttribute));
            m_candidateKey = Attribute.IsDefined(propertyInfo, typeof(CandidateKeyAttribute));
            m_autoGenerateOnNew = Attribute.IsDefined(propertyInfo, typeof(AutoGenerateOnNewAttribute));

            m_useEnumText = (UseEnumTextAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(UseEnumTextAttribute));
            m_oddValueConverter = (OddValueConverterAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(OddValueConverterAttribute));

            // ���ñ�������
            PropertyAliasAttribute[] allAliasAttrs = (PropertyAliasAttribute[])Attribute.GetCustomAttributes(m_propertyInfo, typeof(PropertyAliasAttribute));

            if (allAliasAttrs.Length != 0)
            {
                m_aliases = new List<String>(
                        Array.ConvertAll<PropertyAliasAttribute, String>(
                                allAliasAttrs,
                                delegate(PropertyAliasAttribute aliasAttr) { return aliasAttr.Alias; }
                            )
                    );
            }
        }

        #endregion

        #region ��������

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ���Ե�ֵ�����ݿ�Ự�������ɡ�
        /// </summary>
        public Boolean AutoGenerateOnNew
        {
            get { return !IsChildren && m_autoGenerateOnNew && IsPrimitive; }
        }

        /// <summary>
        /// ��ȡ����������Ӧ���ж��壨�����������ԣ����ӳ����Ϊ�����У�ȡ��һ���ж��壩��
        /// </summary>
        public ColumnDefinition Column
        {
            get { return m_column; }
            internal set { m_column = value; }
        }

        /// <summary>
        /// ��ȡ���԰������ж�������
        /// </summary>
        public Int32 ColumnCount
        {
            get
            {
                if (HasComproundColumns)
                {
                    return Columns.Length;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ����Ϊ�޲�����
        /// </summary>
        public Boolean IsCandidateKey
        {
            get { return !IsChildren && m_candidateKey; }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�������Ƿ���ԭ���������������ݿ����ɵ���������
        /// </summary>
        public Boolean IsPrimaryKeyNative
        {
            get { return !IsChildren && IsPrimaryKey && Entity.NativePrimaryKeyInfo.IsNative; }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�������Ƿ�Ϊ����������ʶ�����ԡ�
        /// </summary>
        public Boolean IsPrimaryKeyAutoIncrement
        {
            get
            {
                return IsPrimaryKeyNative && Entity.NativePrimaryKeyInfo.AutoIncrement;
            }
        }

        /// <summary>
        /// ��ȡ���ڻ�ȡ����������ʶ���ֶε�ֵ�� SQL ָ�
        /// </summary>
        public String RetrieveIdentifierStatement
        {
            get
            {
                return IsPrimaryKeyAutoIncrement ? Entity.NativePrimaryKeyInfo.RetrieveIdentifierStatement : null;
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�����Ƿ��ӳټ��ء�
        /// </summary>
        public Boolean LazyLoad
        {
            get
            {
                if (!IsPrimitive)
                {
                    return false;
                }

                return m_lazyLoad;
            }
        }

        /// <summary>
        /// ��ȡ�������ơ�
        /// </summary>
        public String Name
        {
            get { return m_propertyInfo.Name; }
        }

        /// <summary>
        /// ��ȡӵ�д����Ե�ʵ�嶨�塣
        /// </summary>
        public EntityDefinition Entity
        {
            get { return m_entity; }
        }

        /// <summary>
        /// ��ȡ����ֵת�������������û�б�� OddValueConverterAttribute �������࣬�򷵻� null��
        /// </summary>
        public OddValueConverterAttribute OddValueConverter
        {
            get { return m_oddValueConverter; }
        }

        /// <summary>
        /// ��ȡ������Ϣ��
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get { return m_propertyInfo; }
        }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�Ƿ��һ�������ⲿ���á�
        /// </summary>
        public Boolean SuppressExpand
        {
            get { return m_suppressExpand; }
        }

        /// <summary>
        /// ��ȡ�������͡�
        /// </summary>
        public Type Type
        {
            get { return m_propertyInfo.PropertyType; }
        }

        /// <summary>
        /// ��ȡ�����ϵ� UseEnumTextAttribute ��ǣ�ֻ�е����Ե�����Ϊö���Ҹ������ϱ���� UseEnumTextAttribute���ŷ��ظñ��ʵ�����������Ƿ��� null��
        /// </summary>
        public UseEnumTextAttribute UseEnumText
        {
            get
            {
                Debug.Assert(
                        (m_useEnumText == null) || Type.IsEnum,
                        String.Format("���� {0}.{1} ����ö�����ͣ���Ӧ��� UseEnumTextAttribute��", Entity.Type.FullName, Name)
                    );

                return (Type.IsEnum ? m_useEnumText : null);
            }
        }

        #region ��������

        /// <summary>
        /// ��ȡ����ӵ�е��ж����б�
        /// </summary>
        public abstract ColumnDefinition[] Columns { get; }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�����Ƿ�Ϊ�����С�
        /// </summary>
        public abstract Boolean HasComproundColumns { get; }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ������
        /// </summary>
        public abstract Boolean IsPrimaryKey { get; }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ�������ԣ����Ϊ false������Ϊ�ⲿ�������ԡ�
        /// </summary>
        public abstract Boolean IsPrimitive { get; }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��������ӳ������Ƿ�����Ϊ�ա�
        /// </summary>
        public abstract Boolean PermitNull { get; }

        /// <summary>
        /// ��ȡ�������ù�ϵ��
        /// </summary>
        public abstract EntityPropertyDefinitionRelation Relation { get; internal set; }

        #region ��ʵ���б��������

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ�������Ƿ�Ϊ��ʵ���б�
        /// </summary>
        public abstract Boolean IsChildren { get; }

        /// <summary>
        /// ��ȡ��ʵ���б������б�Ԫ�ص����Զ��塣
        /// </summary>
        public abstract EntityPropertyDefinition ChildrenProperty { get; }

        /// <summary>
        /// ��ȡ��ʵ���б����Ե������������ڼ�����ʵ���б�
        /// </summary>
        public abstract Sorter ChildrenSorter { get; }

        #endregion

        #endregion

        #endregion

        #region ��������

        /// <summary>
        /// �ж������Ƿ�ƥ��������������ƣ��������Ա�����
        /// </summary>
        /// <param name="propertyName">�������ơ�</param>
        /// <returns>�������������ƥ�䣬�򷵻� true�����򷵻� false��</returns>
        public Boolean Matches(String propertyName)
        {
            if (Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
            {
                return true;
            }

            if ((m_aliases != null) && m_aliases.Contains(propertyName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// �������Ե�ֵ����������ǵݹ�ġ�
        /// </summary>
        /// <param name="entity">��������ʵ�塣</param>
        /// <param name="values">������ֵ�б�</param>
        public void SetValue(Object entity, Object[] values)
        {
            if (IsPrimitive || (values[0] == null))
            {
                Object val = values[0];

                if (Type.IsEnum)
                {
                    val = DbConverter.ConvertEnumProperty(val, this);
                }

                PropertyInfo.SetValue(entity, val, null);
            }
            else
            {
                Object parentEntity = EtyBusinessObject.CreatePartially(Type);

                if (Relation.ParentProperties.Length == 1)
                {
                    Relation.ParentProperties[0].SetValue(parentEntity, values);
                }
                else
                {
                    foreach (EntityPropertyDefinition parentProperty in Relation.ParentProperties)
                    {
                        Object[] parentValues = new Object[parentProperty.Columns.Length];

                        for (Int32 i = 0; i < parentValues.Length; i++)
                        {
                            ColumnDefinition columnDef = parentProperty.Columns[i];
                            Int32 index = Array.IndexOf<ColumnDefinition>(Relation.ParentColumns, columnDef);
                            parentValues[i] = values[index];
                        }

                        parentProperty.SetValue(parentEntity, parentValues);
                    }
                }

                PropertyInfo.SetValue(entity, parentEntity, null);
            }
        }

        /// <summary>
        /// ��ʾ�������ơ�
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return Entity.Type.Name + "." + Name;
        }

        #region ���󷽷�

        /// <summary>
        /// �������ڼ�����ʵ��Ĺ�������
        /// </summary>
        /// <param name="parentEntity">��ʵ�����</param>
        /// <returns>Filter ʵ�������ڵõ��ø�ʵ��ӵ�е�������ʵ�塣</returns>
        public abstract Filter ComposeChildrenFilter(Object parentEntity);

        /// <summary>
        /// ��ȡ���е���ʵ�塣
        /// </summary>
        /// <param name="parentEntity">��ʵ�塣</param>
        /// <param name="dbSession">���ݿ�Ự���档</param>
        /// <returns>��ʵ���б�</returns>
        public abstract Object[] GetAllChildren(Object parentEntity, IDatabaseSession dbSession);

        #endregion

        #endregion

        #region �����

        /// <summary>
        /// ��д����ԣ��������Ϊͬһʵ���ͬһ���ԣ�����Ϊ���ߵ�ͬ��
        /// </summary>
        /// <param name="obj">��һ����Ѯ��</param>
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

            EntityPropertyDefinition other = (EntityPropertyDefinition)obj;

            return obj.Equals(other);
        }

        /// <summary>
        /// ��д��
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return Entity.GetHashCode() ^ CommonPolicies.NormalizePropertyName(Name).GetHashCode();
        }

        /// <summary>
        /// ��д��
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator ==(EntityPropertyDefinition left, EntityPropertyDefinition right)
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
        public static Boolean operator !=(EntityPropertyDefinition left, EntityPropertyDefinition right)
        {
            return !(left == right);
        }

        #region IEquatable<EntityProperty> ��Ա

        /// <summary>
        /// ָʾ��ǰʵ�����Զ����Ƿ��ͬ����һ��ʵ�����ԡ�
        /// </summary>
        /// <param name="other">��һ��ʵ�����ԡ�</param>
        /// <returns>�����ǰʵ������Ϊͬһʵ���ͬһ���ԣ��򷵻� true�����򷵻� false��</returns>
        public Boolean Equals(EntityPropertyDefinition other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return Entity.Equals(other.Entity) && Name.Equals(other.Name, CommonPolicies.PropertyNameComparison);
        }

        #endregion

        #endregion
    }
}