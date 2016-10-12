#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupExpressionSchema.cs
// 文件功能描述：分组表达式架构。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110712
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
	/// 分组表达式架构。
	/// </summary>
	internal sealed class GroupExpressionSchema : ExpressionSchema
	{
		#region 私有字段

		private GroupSchema m_groupSchema;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置选择列集合和 SQL 指令。
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="sqlExpression"></param>
		public GroupExpressionSchema(ExpressionSchemaColumn[] columns, String sqlExpression)
			: base(columns, sqlExpression)
		{
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取或设置分组架构。
		/// </summary>
		internal GroupSchema GroupSchema
		{
			get { return m_groupSchema; }
			set { m_groupSchema = value; }
		}

		#endregion

		/// <summary>
		/// 创建分组实体。
		/// </summary>
		/// <param name="dbValues">记录。</param>
		/// <returns>创建好的分组实体。</returns>
		public override Object Compose(Object[] dbValues)
		{
			return m_groupSchema.Compose(dbValues);
		}

		/// <summary>
		/// 定位列。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>定位的列集合。</returns>
		public override Column[] this[ColumnLocator colLocator]
		{
			get
			{
				Column[] columns = m_groupSchema[colLocator];
				Column[] expressionSchemaColumns = GetExpressionSchemaColumns(columns);

				return expressionSchemaColumns;
			}
		}

		/// <summary>
		/// 显示分组实体表达式的详细信息，用于调试。
		/// </summary>
		/// <returns>分组实体表达式的详细信息。</returns>
		public override String Dump()
		{
			return GroupSchema.Dump();
		}

		#region 辅助方法

		/// <summary>
		/// 获取分组架构列的包装列。
		/// </summary>
		/// <param name="columns">分组架构列集合。</param>
		/// <returns>该列集合的包装列集合。</returns>
		private Column[] GetExpressionSchemaColumns(Column[] columns)
		{
			Column[] schemaColumns = new Column[columns.Length];

			for (Int32 i = 0; i < schemaColumns.Length; i++)
			{
				Int32 index = Array.FindIndex<Column>(
						m_groupSchema.SelectColumns,
						delegate(Column col)
						{
							return WrappedColumn.GetRootColumn(col).Equals(WrappedColumn.GetRootColumn(columns[i]));
						}
					);

				schemaColumns[i] = this.Columns[index];
			}

			return schemaColumns;
		}

		#endregion
	}
}