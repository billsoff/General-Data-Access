#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityDefinition.cs
// 文件功能描述：通过解析数据架构特性，组织全部架构信息。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110412
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 通过解析数据架构特性，组织全部架构信息。
    /// </summary>
    internal sealed partial class EntityDefinition : IEquatable<EntityDefinition>
    {
        #region 私有字段

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

        // 缓存所有外部引用属性，用于第二阶段构造
        private List<EntityForeignReferencePropertyDefinition> m_allForeignReferencePropertiesInternal;
        private Dictionary<EntityForeignReferencePropertyDefinition, String[]> m_childColumnNamesSet;
        private Dictionary<EntityForeignReferencePropertyDefinition, String[]> m_parentColumnNamesSet;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        public EntityDefinition(Type entityType)
        {
            Debug.Assert((entityType != null), "实体类型参数不能为空。");
            Debug.Assert(
                    Attribute.IsDefined(entityType, typeof(TableAttribute)),
                    String.Format("实体类型 {0} 应标记 TableAttribute 特性。", entityType.FullName)
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

            // 构造属性和列
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

            // 设置实体属性定义集合和列定义集合
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

        #region 公共属性

        /// <summary>
        /// 获取列集合。
        /// </summary>
        public ColumnDefinitionCollection Columns
        {
            get { return m_columns; }
        }

        /// <summary>
        /// 获取一个值，该值指示此实体的数据源是否为存储过程，否则为表。
        /// </summary>
        public Boolean IsStoredProcedure
        {
            get { return m_isStoredProcedure; }
        }

        /// <summary>
        /// 获取加载策略。
        /// </summary>
        public LoadStrategyAttribute LoadStrategy
        {
            get { return m_loadStrategy; }
        }

        /// <summary>
        /// 获取原生主键信息。
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
                            "如果主键标记为原生的，则要么该主键为自增长（标记）字段，要么必须标记候选键。"
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
        /// 获取主键列集合。
        /// </summary>
        public ColumnDefinitionCollection PrimaryKeys
        {
            get { return m_primaryKeys; }
        }

        /// <summary>
        /// 获取实体属性集合。
        /// </summary>
        public EntityPropertyDefinitionCollection Properties
        {
            get { return m_properties; }
        }

        /// <summary>
        /// 获取实体定义提供者。
        /// </summary>
        public IEntityDefinitionProvider Provider
        {
            get { return m_provider; }
        }

        /// <summary>
        /// 获取实体类型。
        /// </summary>
        public Type Type
        {
            get { return m_type; }
        }

        /// <summary>
        /// 获取表名称。
        /// </summary>
        public String TableName
        {
            get { return m_tableName; }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 为实体创建过滤器。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <returns>创建好的过滤器。</returns>
        public Filter ComposeLoadFilter(Object entity)
        {
            #region 前置条件

            Debug.Assert((entity != null), "实体不能为空。");
            Debug.Assert((entity.GetType() == Type), "实体的类型必须与实体定义的类型相同");

            #endregion

            FilterExpression expression = new FilterExpression();

            foreach (ColumnDefinition columnDef in this.PrimaryKeys)
            {
                expression.And.Property(columnDef.PropertyChain).Is.EqualTo(columnDef.GetDbValue(entity));
            }

            return expression.Resolve();
        }

        /// <summary>
        /// 获取外部引用属性集合。
        /// </summary>
        /// <returns>外部引用属性列表。</returns>
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
        /// 显示实体定义的类型名称。
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return Type.Name;
        }

        #endregion

        #region 内部属性

        /// <summary>
        /// 获取所有外部引用属性列表。
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
        /// 获取外部引用属性的子列名列表集合，键为外部属性。。
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
        /// 获取外部引用属性的父列名列表集合，键为外部属性。
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

        #region 内部方法

        /// <summary>
        /// 第二阶段构造，设置外部引用属性及其列的属性。
        /// </summary>
        /// <param name="provider">实体定义提供者。</param>
        internal void Initialize(IEntityDefinitionProvider provider)
        {
            m_provider = provider;

            // 实体不存在外部引用属性
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

                // 设置外部引用属性的关系。
                entityProperty.Relation = new EntityPropertyDefinitionRelation(childColumns, parentColumns);
            }

            // 释放临时缓存
            m_allForeignReferencePropertiesInternal = null;
            m_childColumnNamesSet = null;
            m_parentColumnNamesSet = null;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 提取外部引用属性。
        /// </summary>
        /// <param name="propertyInfo">属性信息。</param>
        /// <param name="propertyList">要放入的实体属性列表。</param>
        /// <param name="columnList">要放入的列定义列表。</param>
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

            // 缓存外引用信息供第二阶级构造
            AllForeignReferenceProperties.Add(entityProperty);
            ChildColumnNamesSet.Add(entityProperty, childColumnNames);
            ParentColumnNamesSet.Add(entityProperty, parentColumnNames); // TODO：如果引到自身，且为同一字段，则应抛出异常，否则会由于无限递归而报“堆栈溢出”错误
        }

        /// <summary>
        /// 提取基本（值）属性。
        /// </summary>
        /// <param name="propertyInfo">属性信息。</param>
        /// <param name="propertyList">要放入的实体属性列表。</param>
        /// <param name="columnList">要放入的列定义列表。</param>
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

        #region 相等性

        /// <summary>
        /// 重写相等性定义，只要实体定义的基础类型相同，则认为二者等同。
        /// </summary>
        /// <param name="obj">另一个对象。</param>
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
        /// 重写，总是返回基础类型的哈希值。
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return Type.GetHashCode();
        }

        /// <summary>
        /// 重写。
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
        /// 重写。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(EntityDefinition left, EntityDefinition right)
        {
            return !(left == right);
        }

        #region IEquatable<EntityDefinition> 成员

        /// <summary>
        /// 指示当前实体对象是否等同于另一个实体定义对象。
        /// </summary>
        /// <param name="other">另一个实体对象。</param>
        /// <returns>如果当前实体定义等同于另一个实体定义对象，则返回 true；否则返回 false。</returns>
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