#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntitySchema.cs
// 文件功能描述：实体架构。
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
using System.Diagnostics;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 实体架构。
	/// </summary>
	public sealed partial class EntitySchema : IColumnLocatable
	{
		#region 私有字段

		#region 常量

		/// <summary>
		/// 表别名前缀“t__”。
		/// </summary>
		private const String TABLE_ALIAS_PREFIX = "t__";

		#endregion

		private Int32 m_level;

		private EntitySchemaComposite m_compositer;
		private readonly EntityDefinition m_entity;

		private readonly EntityPropertyCollection m_properties;
		private readonly EntityPropertyCollection m_primaryKeyProperties;

		private readonly ColumnCollection m_columns;
		private readonly ColumnCollection m_primaryKeyColumns;

		private EntitySchemaRelation m_leftRelation;
		private EntitySchemaRelation[] m_rightRelations;

		/// <summary>
		/// 键是实体属性的名称。
		/// </summary>
		private Dictionary<String, EntitySchemaRelation> m_rightRelationLookups;

		private Int32 m_index;
		private String m_propertyPath;

		private Boolean m_selectNothing;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体定义。
		/// </summary>
		/// <param name="entity">实体定义。</param>
		internal EntitySchema(EntityDefinition entity)
			: this((EntitySchemaComposite)null, entity)
		{
		}

		/// <summary>
		/// 构造函数，设置实体定义。
		/// </summary>
		/// <param name="composite">实体架构所属的架构组合。</param>
		/// <param name="entity">实体定义。</param>
		internal EntitySchema(EntitySchemaComposite composite, EntityDefinition entity)
		{
			#region 前置条件

			Debug.Assert((entity != null), "实体定义参数 definition 不能为空。");

			#endregion

			m_compositer = composite;
			m_entity = entity;

			List<EntityProperty> properties = new List<EntityProperty>();
			List<EntityProperty> primaryKeyProperties = new List<EntityProperty>();

			List<Column> columns = new List<Column>();
			List<Column> primaryKeyColumns = new List<Column>();

			List<EntityPropertyDefinition> selectProperties = new List<EntityPropertyDefinition>();

			foreach (EntityPropertyDefinition propertyDef in entity.Properties)
			{
				if (!propertyDef.IsChildren)
				{
					// 构造实体属性
					EntityProperty property = new EntityProperty(this, propertyDef);

					properties.Add(property);
					columns.AddRange(property.Columns);

					if (propertyDef.IsPrimaryKey)
					{
						primaryKeyProperties.Add(property);
						primaryKeyColumns.AddRange(property.Columns);
					}
				}
			}

			m_properties = new EntityPropertyCollection(properties);
			m_primaryKeyProperties = new EntityPropertyCollection(primaryKeyProperties);

			m_columns = new ColumnCollection(columns);
			m_primaryKeyColumns = new ColumnCollection(primaryKeyColumns);
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 根据列定位符查找匹配的列集合。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>列集合。</returns>
		public Column[] this[ColumnLocator colLocator]
		{
			get
			{
				IPropertyChain chain = colLocator.Create(Type);
				IPropertyChain head = chain.Head;

				EntitySchema schema = this;
				String propertyName = head.Name;
				IPropertyChain next = head.Next;

				while (next != null)
				{
					EntitySchemaRelation relation = schema.GetRightRelation(propertyName);

					schema = relation.ParentSchema;
					propertyName = next.Name;

					next = next.Next;
				}

				Column[] results = schema.GetColumnsByPropertyName(propertyName);

				if (results == null)
				{
					throw new InvalidOperationException(String.Format("在实体 {0} 中不存在属性 {1}。", Entity.Type.FullName, colLocator.FullName));
				}

				return results;
			}
		}

		/// <summary>
		/// 获取所有列。
		/// </summary>
		public ColumnCollection Columns
		{
			get { return m_columns; }
		}

		/// <summary>
		/// 获取显示名称。
		/// </summary>
		public String DisplayName
		{
			get { return String.Format("{0} AS {1}", TableName, TableAlias); }
		}

		/// <summary>
		/// 获取一个值，该值指示是否含有父级引用。
		/// </summary>
		public Boolean HasRightRelations
		{
			get { return ((m_rightRelations != null) && (m_rightRelations.Length != 0)); }
		}

		/// <summary>
		/// 获取实体架构索引号。
		/// </summary>
		public Int32 Index
		{
			get { return m_index; }
			internal set { m_index = value; }
		}

		/// <summary>
		/// 获取一个值，该值指示数据源是否为存储过程。
		/// </summary>
		public Boolean IsStoredProcedure
		{
			get { return Entity.IsStoredProcedure; }
		}

		/// <summary>
		/// 获取子实体架构关系。
		/// </summary>
		public EntitySchemaRelation LeftRelation
		{
			get { return m_leftRelation; }
			internal set { m_leftRelation = value; }
		}

		/// <summary>
		/// 获取实体架构的级别。
		/// </summary>
		public Int32 Level
		{
			get { return m_level; }
			internal set { m_level = value; }
		}

		/// <summary>
		/// 获取实体架构所属的组合。
		/// </summary>
		public EntitySchemaComposite Composite
		{
			get { return m_compositer; }
			internal set { m_compositer = value; }
		}

		/// <summary>
		/// 获取主键列集合。
		/// </summary>
		public ColumnCollection PrimaryKeyColumns
		{
			get { return m_primaryKeyColumns; }
		}

		/// <summary>
		/// 获取主键属性集合。
		/// </summary>
		public EntityPropertyCollection PrimaryKeyProperties
		{
			get { return m_primaryKeyProperties; }
		}

		/// <summary>
		/// 获取属性集合。
		/// </summary>
		public EntityPropertyCollection Properties
		{
			get { return m_properties; }
		}

		/// <summary>
		/// 获取实体架构的属性路径。
		/// </summary>
		public String PropertyPath
		{
			get
			{
				if (m_propertyPath == null)
				{
					EntitySchemaRelation relation = LeftRelation;

					if (relation == null)
					{
						m_propertyPath = Type.Name;
					}
					else
					{
						m_propertyPath = relation.ChildProperty.PropertyChain.FullName;
					}
				}

				return m_propertyPath;
			}
		}

		/// <summary>
		/// 获取父实体架构关系列表。
		/// </summary>
		public EntitySchemaRelation[] RightRelations
		{
			get
			{
				if (m_rightRelations == null)
				{
					m_rightRelations = new EntitySchemaRelation[0];
				}

				return m_rightRelations;
			}

			internal set
			{
				m_rightRelations = value;

				if (m_rightRelations != null)
				{
					m_rightRelationLookups = new Dictionary<String, EntitySchemaRelation>();

					foreach (EntitySchemaRelation relation in m_rightRelations)
					{
						m_rightRelationLookups.Add(relation.ChildColumns[0].Definition.Property.Name, relation);
					}
				}
			}
		}

		/// <summary>
		/// 获取一个值，该值指示是否不选择任何属性。
		/// </summary>
		public Boolean SelectNothing
		{
			get { return m_selectNothing; }
			internal set { m_selectNothing = value; }
		}

		/// <summary>
		/// 获取表的别名。
		/// </summary>
		public String TableAlias
		{
			get { return (TABLE_ALIAS_PREFIX + Index.ToString()); }
		}

		/// <summary>
		/// 获取表的名称。
		/// </summary>
		public String TableName
		{
			get { return Entity.TableName; }
		}

		/// <summary>
		/// 获取实体的类型。
		/// </summary>
		public Type Type
		{
			get { return Entity.Type; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 合成实体，递归地执行。
		/// </summary>
		/// <param name="dbValues">数据库记录值列表。</param>
		/// <returns>合成好的实体。</returns>
		public Object Compose(Object[] dbValues)
		{
			if (IsNull(dbValues))
			{
				return null;
			}

			Object entity = EtyBusinessObject.CreateCompletely(Type);

			foreach (EntityProperty property in Composite.GetSelectProperties(this))
			{
				Object propertyValue;

				if (property.IsPrimitive)
				{
					Column col = property.Columns[0];
					propertyValue = DbConverter.ConvertFrom(dbValues[col.FieldIndex], col);

					property.PropertyInfo.SetValue(entity, propertyValue, null);
				}
				else
				{
					EntitySchema parentSchema = GetParentSchema(property);

					if ((parentSchema != null) && !parentSchema.SelectNothing)
					{
						if (!Convert.IsDBNull(dbValues[property.Columns[0].FieldIndex]))
						{
							propertyValue = parentSchema.Compose(dbValues);
						}
						else
						{
							propertyValue = null;
						}

						property.PropertyInfo.SetValue(entity, propertyValue, null);
					}
					else
					{
						SetForeignReferencePropertyValuePartially(entity, property, dbValues);
					}
				}
			}

			return entity;
		}

		/// <summary>
		/// 获取属性所对应的列集合。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>列集合。</returns>
		public Column[] GetColumnsByPropertyName(String propertyName)
		{
			EntityPropertyDefinition property = Entity.Properties[propertyName];

			Column[] results;

			if (property != null)
			{
				results = Columns.GetColumnsByDefinitions(property.Columns);
			}
			else
			{
				results = new Column[0];
			}

			return results;
		}

		/// <summary>
		/// 判断实体架构是否包含属性（直接包含）。
		/// </summary>
		/// <param name="propertyChain">属性链。</param>
		/// <returns>如果实体架构包含该属性，则返回 true；否则返回 false。</returns>
		public Boolean OwnProperty(IPropertyChain propertyChain)
		{
			#region 前置条件

			Debug.Assert(propertyChain != null, "参数 property 不能为空。");

			#endregion

			if (Composite.Target.Type != propertyChain.Type)
			{
				return false;
			}

			if ((propertyChain.Depth - Level) != 1)
			{
				return false;
			}

			return Properties.Contains(propertyChain.Name);
		}

		/// <summary>
		/// 显示别名。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Format("{0} -> {1}", PropertyPath, TableAlias);
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取实体定义。
		/// </summary>
		internal EntityDefinition Entity
		{
			get { return m_entity; }
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 获取父架构。
		/// </summary>
		/// <param name="property">拥有父架构的实体属性。</param>
		/// <returns></returns>
		private EntitySchema GetParentSchema(EntityProperty property)
		{
			if (HasRightRelations)
			{
				EntitySchemaRelation relationFound = Array.Find<EntitySchemaRelation>(
						RightRelations,
						delegate(EntitySchemaRelation relation) { return (relation.ChildProperty == property); }
					);

				if (relationFound != null)
				{
					return relationFound.ParentSchema;
				}
			}

			return null;
		}

		/// <summary>
		/// 获取属性关系。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>具有该名称的属性关系。</returns>
		private EntitySchemaRelation GetRightRelation(String propertyName)
		{
			if ((m_rightRelationLookups != null) && m_rightRelationLookups.ContainsKey(propertyName))
			{
				return m_rightRelationLookups[propertyName];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 判断实体是否为空值。
		/// </summary>
		/// <param name="dbValues">记录。</param>
		/// <returns>如果记录中不包含任何值，则返回 true；否则返回 false。</returns>
		private Boolean IsNull(Object[] dbValues)
		{
			if (SelectNothing)
			{
				return true;
			}

			foreach (Column col in Composite.GetSelectColumns(this))
			{
				if (!Convert.IsDBNull(dbValues[col.FieldIndex]))
				{
					return false;

				}
				else if (col.IsPrimaryKey)
				{
					return true;
				}
			}

			return true;
		}

		/// <summary>
		/// 使用引用列的值设置引用属性，这个过程是递归的。
		/// </summary>
		/// <param name="entity">要设置属性的实体。</param>
		/// <param name="property">引用实体属性。</param>
		/// <param name="dbValues">数据库值集合。</param>
		private void SetForeignReferencePropertyValuePartially(Object entity, EntityProperty property, Object[] dbValues)
		{
			Object[] values;

			if (!property.HasComproundColumns)
			{
				Column firstColumn = property.Columns[0];
				Object firstValue = DbConverter.ConvertFrom(dbValues[firstColumn.FieldIndex], firstColumn);
				values = new Object[] { firstValue };
			}
			else
			{
				values = new Object[property.Columns.Count];

				for (Int32 i = 0; i < values.Length; i++)
				{
					Column col = property.Columns[i];
					values[i] = DbConverter.ConvertFrom(dbValues[col.FieldIndex], col);
				}
			}

			property.Definition.SetValue(entity, values);
		}

		#endregion
	}
}