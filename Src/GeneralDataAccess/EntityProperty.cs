#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityProperty.cs
// 文件功能描述：实体属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110426
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 实体属性。
	/// </summary>
	public sealed partial class EntityProperty
	{
		#region 私有字段

		private readonly EntitySchema m_schema;
		private readonly ColumnCollection m_columns;
		private readonly EntityPropertyDefinition m_definition;

		private PropertyChain m_propertyChain;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体架构和实体属性定义。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <param name="definition">实体属性定义。</param>
		internal EntityProperty(EntitySchema schema, EntityPropertyDefinition definition)
		{
			#region 前置条件

			Debug.Assert(!definition.IsChildren, "属性定义参数 definition 不能为子实体列表属性。");

			#endregion

			m_schema = schema;
			m_definition = definition;

			// 构造列
			m_columns = ComposeColumns();
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 实体属性所拥有的列集合。
		/// </summary>
		public ColumnCollection Columns
		{
			get { return m_columns; }
		}

		/// <summary>
		/// 获取一个值，该值指示实体属性是否映射为复合列。
		/// </summary>
		public Boolean HasComproundColumns
		{
			get { return m_definition.HasComproundColumns; }
		}

		/// <summary>
		/// 获取一个值，该值指示属性是否为主键。
		/// </summary>
		public Boolean IsPrimaryKey
		{
			get { return m_definition.IsPrimaryKey; }
		}

		/// <summary>
		/// 获取一个值，该值指示当前属性是否为基本属性，如果为 false，则其为外部引用属性。
		/// </summary>
		public Boolean IsPrimitive
		{
			get
			{
				return Definition.IsPrimitive;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示属性是否延迟加载。
		/// </summary>
		public Boolean LazyLoad
		{
			get
			{
				return Definition.LazyLoad;
			}
		}

		/// <summary>
		/// 获取属性名称。
		/// </summary>
		public String Name
		{
			get
			{
				return Definition.Name;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示此属性所映射的列是否允许为空。
		/// </summary>
		public Boolean PermitNull
		{
			get
			{
				return Definition.PermitNull;
			}
		}

		/// <summary>
		/// 获取属性信息。
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get
			{
				return Definition.PropertyInfo;
			}
		}

		/// <summary>
		/// 获取属性链。
		/// </summary>
		public IPropertyChain PropertyChain
		{
			get
			{
				if (m_propertyChain == null)
				{
					EntitySchemaRelation relation = Schema.LeftRelation;

					if (relation == null)
					{
						m_propertyChain = new PropertyChain(Schema.Type, new String[] { Name });
					}
					else
					{
						IPropertyChain childPropertyChain = relation.ChildProperty.PropertyChain;
						String[] childPropertyPath = childPropertyChain.PropertyPath;
						String[] propertyPath = new String[childPropertyPath.Length + 1];

						Array.Copy(childPropertyPath, propertyPath, childPropertyPath.Length);
						propertyPath[propertyPath.Length - 1] = Name;

						m_propertyChain = new PropertyChain(childPropertyChain.Type, propertyPath);
					}
				}

				return m_propertyChain;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示是否进一步加载外部引用。
		/// </summary>
		public Boolean SuppressExpand
		{
			get { return m_definition.SuppressExpand; }
		}

		/// <summary>
		/// 获取属性类型。
		/// </summary>
		public Type Type
		{
			get
			{
				return Definition.Type;
			}
		}

		/// <summary>
		/// 获取属性所属的实体架构。
		/// </summary>
		public EntitySchema Schema
		{
			get { return m_schema; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 检索用户属性的自定义标记。
		/// </summary>
		/// <param name="attributeType">要搜索的自定义标记的类型或基类型。</param>
		/// <returns>指定类型的自定义标记。</returns>
		public Attribute GetCustomAttribute(Type attributeType)
		{
			return Attribute.GetCustomAttribute(PropertyInfo, attributeType);
		}

		/// <summary>
		/// 检索用户属性的自定义标记列表。
		/// </summary>
		/// <param name="attributeType">要搜索的自定义标记的类型或基类型。</param>
		/// <returns>指定类型的自定义标记的列表。</returns>
		public Attribute[] GetCustomAttributes(Type attributeType)
		{
			return Attribute.GetCustomAttributes(PropertyInfo, attributeType);
		}

		/// <summary>
		/// 查询在属性上是否标记了指定类型的标记。
		/// </summary>
		/// <param name="attributeType">要查询的标记类型。</param>
		/// <returns>如果在属性上标记了该类型的标记，则返回 true；否则返回 false。</returns>
		public Boolean IsCustomAttributeDefined(Type attributeType)
		{
			return Attribute.IsDefined(PropertyInfo, attributeType);
		}

		/// <summary>
		/// 显示属性名称。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return PropertyChain.FullName;
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取实体属性定义。
		/// </summary>
		internal EntityPropertyDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 合成实体属性包含的列集合。
		/// </summary>
		/// <returns>合成好的列集合。</returns>
		private ColumnCollection ComposeColumns()
		{
			List<Column> columns = new List<Column>();

			foreach (ColumnDefinition columnDef in Definition.Columns)
			{
				columns.Add(new EntityColumn(this, columnDef));
			}

			return new ColumnCollection(columns);
		}

		#endregion
	}
}