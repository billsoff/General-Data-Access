#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntitySchemaRelation.cs
// 文件功能描述：表示实体架构关系。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示实体架构关系。
	/// </summary>
	public sealed class EntitySchemaRelation
	{
		#region 私有字段

		private readonly EntityProperty m_childProperty;

		private readonly EntitySchema m_childSchema;
		private readonly EntitySchema m_parentSchema;

		private readonly Column[] m_childColumns;
		private readonly Column[] m_parentColumns;

		private Boolean m_permitNull;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置映射列列表。
		/// </summary>
		/// <param name="childColumns">子实体映射列列表。</param>
		/// <param name="parentColumns">父实体映射列列表。</param>
		public EntitySchemaRelation(Column[] childColumns, Column[] parentColumns)
		{
			m_childSchema = childColumns[0].Property.Schema;
			m_parentSchema = parentColumns[0].Property.Schema;

			m_childColumns = childColumns;
			m_parentColumns = parentColumns;

			m_childProperty = childColumns[0].Property;
			m_permitNull = childColumns[0].Definition.Property.PermitNull;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取子实体列列表。
		/// </summary>
		public Column[] ChildColumns
		{
			get { return m_childColumns; }
		}

		/// <summary>
		/// 获取子实体架构。
		/// </summary>
		public EntitySchema ChildSchema
		{
			get { return m_childSchema; }
		}

		/// <summary>
		/// 获取父实体列列表。
		/// </summary>
		public Column[] ParentColumns
		{
			get { return m_parentColumns; }
		}

		/// <summary>
		/// 获取一个值，该值指示引用列是否允许空值。
		/// </summary>
		public Boolean PermitNull
		{
			get { return m_permitNull; }
			internal set { m_permitNull = value; }
		}

		/// <summary>
		/// 获取父实体架构。
		/// </summary>
		public EntitySchema ParentSchema
		{
			get { return m_parentSchema; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取映射父列。
		/// </summary>
		/// <param name="childColumn">子列。</param>
		/// <returns>映射父列。</returns>
		public Column GetMappingParentColumn(Column childColumn)
		{
			Int32 index = Array.IndexOf<Column>(ChildColumns, childColumn);

			if (index < 0)
			{
				return null;
			}

			return ParentColumns[index];
		}

		/// <summary>
		/// 实体关系的简单表示。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Format("{0}({1}) -> {2}", ChildSchema, ChildProperty, ParentSchema);
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取关系所属的子属性。
		/// </summary>
		internal EntityProperty ChildProperty
		{
			get { return m_childProperty; }
		}

		#endregion
	}
}