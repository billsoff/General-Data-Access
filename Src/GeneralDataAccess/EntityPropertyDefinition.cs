#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityPropertyDefinition.cs
// 文件功能描述：表示一个实体属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110413
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
    /// 表示一个实体属性。
    /// </summary>
    internal abstract class EntityPropertyDefinition : IEquatable<EntityPropertyDefinition>
    {
        #region 私有字段

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

        #region 构造函数

        /// <summary>
        /// 构造函数，设置属性所属的实体定义和属性信息。
        /// </summary>
        /// <param name="entity">属性所属的实体定义。</param>
        /// <param name="propertyInfo">属性信息。</param>
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

            // 设置别名集合
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

        #region 公共属性

        /// <summary>
        /// 获取一个值，该值指示当前属性的值由数据库会话引擎生成。
        /// </summary>
        public Boolean AutoGenerateOnNew
        {
            get { return !IsChildren && m_autoGenerateOnNew && IsPrimitive; }
        }

        /// <summary>
        /// 获取此属性所对应的列定义（对于引用属性，如果映射列为复合列，取第一个列定义）。
        /// </summary>
        public ColumnDefinition Column
        {
            get { return m_column; }
            internal set { m_column = value; }
        }

        /// <summary>
        /// 获取属性包含的列定义数。
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
        /// 获取一个值，该值指示当前属性为修补键。
        /// </summary>
        public Boolean IsCandidateKey
        {
            get { return !IsChildren && m_candidateKey; }
        }

        /// <summary>
        /// 获取一个值，该值指示此属性是否是原生主键（即由数据库生成的主键）。
        /// </summary>
        public Boolean IsPrimaryKeyNative
        {
            get { return !IsChildren && IsPrimaryKey && Entity.NativePrimaryKeyInfo.IsNative; }
        }

        /// <summary>
        /// 获取一个值，该值指示此属性是否为自增长（标识）属性。
        /// </summary>
        public Boolean IsPrimaryKeyAutoIncrement
        {
            get
            {
                return IsPrimaryKeyNative && Entity.NativePrimaryKeyInfo.AutoIncrement;
            }
        }

        /// <summary>
        /// 获取用于获取自增长（标识）字段的值的 SQL 指令。
        /// </summary>
        public String RetrieveIdentifierStatement
        {
            get
            {
                return IsPrimaryKeyAutoIncrement ? Entity.NativePrimaryKeyInfo.RetrieveIdentifierStatement : null;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示属性是否延迟加载。
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
        /// 获取属性名称。
        /// </summary>
        public String Name
        {
            get { return m_propertyInfo.Name; }
        }

        /// <summary>
        /// 获取拥有此属性的实体定义。
        /// </summary>
        public EntityDefinition Entity
        {
            get { return m_entity; }
        }

        /// <summary>
        /// 获取奇异值转换器，如果属性没有标记 OddValueConverterAttribute 的派生类，则返回 null。
        /// </summary>
        public OddValueConverterAttribute OddValueConverter
        {
            get { return m_oddValueConverter; }
        }

        /// <summary>
        /// 获取属性信息。
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get { return m_propertyInfo; }
        }

        /// <summary>
        /// 获取一个值，该值指示是否进一步加载外部引用。
        /// </summary>
        public Boolean SuppressExpand
        {
            get { return m_suppressExpand; }
        }

        /// <summary>
        /// 获取属性类型。
        /// </summary>
        public Type Type
        {
            get { return m_propertyInfo.PropertyType; }
        }

        /// <summary>
        /// 获取属性上的 UseEnumTextAttribute 标记，只有当属性的类型为枚举且该属性上标记有 UseEnumTextAttribute，才返回该标记实例，否则总是返回 null。
        /// </summary>
        public UseEnumTextAttribute UseEnumText
        {
            get
            {
                Debug.Assert(
                        (m_useEnumText == null) || Type.IsEnum,
                        String.Format("属性 {0}.{1} 不是枚举类型，不应标记 UseEnumTextAttribute。", Entity.Type.FullName, Name)
                    );

                return (Type.IsEnum ? m_useEnumText : null);
            }
        }

        #region 抽象属性

        /// <summary>
        /// 获取属性拥有的列定义列表。
        /// </summary>
        public abstract ColumnDefinition[] Columns { get; }

        /// <summary>
        /// 获取一个值，该值指示属性是否为复合列。
        /// </summary>
        public abstract Boolean HasComproundColumns { get; }

        /// <summary>
        /// 获取一个值，该值指示当前属性是否为主键。
        /// </summary>
        public abstract Boolean IsPrimaryKey { get; }

        /// <summary>
        /// 获取一个值，该值指示当前属性是否为基本属性，如果为 false，则其为外部引用属性。
        /// </summary>
        public abstract Boolean IsPrimitive { get; }

        /// <summary>
        /// 获取一个值，该值指示此属性所映射的列是否允许为空。
        /// </summary>
        public abstract Boolean PermitNull { get; }

        /// <summary>
        /// 获取属性引用关系。
        /// </summary>
        public abstract EntityPropertyDefinitionRelation Relation { get; internal set; }

        #region 子实体列表相关属性

        /// <summary>
        /// 获取一个值，该值指示此属性是否为子实体列表。
        /// </summary>
        public abstract Boolean IsChildren { get; }

        /// <summary>
        /// 获取子实体列表属性列表元素的属性定义。
        /// </summary>
        public abstract EntityPropertyDefinition ChildrenProperty { get; }

        /// <summary>
        /// 获取子实体列表属性的排序器，用于加载子实体列表。
        /// </summary>
        public abstract Sorter ChildrenSorter { get; }

        #endregion

        #endregion

        #endregion

        #region 公共方法

        /// <summary>
        /// 判断属性是否匹配给定的属性名称，考虑属性别名。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>如果与属性名称匹配，则返回 true；否则返回 false。</returns>
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
        /// 设置属性的值，这个过程是递归的。
        /// </summary>
        /// <param name="entity">包含属的实体。</param>
        /// <param name="values">属性列值列表。</param>
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
        /// 显示属性名称。
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return Entity.Type.Name + "." + Name;
        }

        #region 抽象方法

        /// <summary>
        /// 创建用于加载子实体的过滤器。
        /// </summary>
        /// <param name="parentEntity">父实体对象。</param>
        /// <returns>Filter 实例，用于得到该父实体拥有的所有子实体。</returns>
        public abstract Filter ComposeChildrenFilter(Object parentEntity);

        /// <summary>
        /// 获取所有的子实体。
        /// </summary>
        /// <param name="parentEntity">父实体。</param>
        /// <param name="dbSession">数据库会话引擎。</param>
        /// <returns>子实体列表。</returns>
        public abstract Object[] GetAllChildren(Object parentEntity, IDatabaseSession dbSession);

        #endregion

        #endregion

        #region 相等性

        /// <summary>
        /// 重写相等性，如果两者为同一实体的同一属性，则认为二者等同。
        /// </summary>
        /// <param name="obj">另一个对旬象。</param>
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
        /// 重写。
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return Entity.GetHashCode() ^ CommonPolicies.NormalizePropertyName(Name).GetHashCode();
        }

        /// <summary>
        /// 重写。
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
        /// 重写。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator !=(EntityPropertyDefinition left, EntityPropertyDefinition right)
        {
            return !(left == right);
        }

        #region IEquatable<EntityProperty> 成员

        /// <summary>
        /// 指示当前实体属性对象是否等同于另一个实体属性。
        /// </summary>
        /// <param name="other">另一个实体属性。</param>
        /// <returns>如果当前实体属性为同一实体的同一属性，则返回 true；否则返回 false。</returns>
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