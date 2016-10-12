#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupExpressionSchemaBuilder.cs
// 文件功能描述：分组表达式架构生成器。
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
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 分组表达式架构生成器。
	/// </summary>
	internal sealed class GroupExpressionSchemaBuilder : ExpressionSchemaBuilder
	{
		#region 私有字段

		private readonly GroupSchemaBuilder m_groupSchemaBuilder;
		private GroupSchema m_groupSchema;

		private String m_whereExpression;
		private String m_havingExpression;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置复合实体属性和查询参数前缀。
		/// </summary>
		/// <param name="propertyDef">复合实体属性。</param>
		/// <param name="parameterPrefix">查询参数前缀。</param>
		public GroupExpressionSchemaBuilder(CompositeForeignReferencePropertyDefinition propertyDef, String parameterPrefix)
			: base(propertyDef, parameterPrefix)
		{
			m_groupSchemaBuilder = new GroupSchemaBuilder(propertyDef.Type);
		}

		#endregion

		/// <summary>
		/// 扩展属性。
		/// </summary>
		/// <param name="properties">参见 ExpressionSchemaBuilder 中的注释。</param>
		/// <param name="inline">参见 ExpressionSchemaBuilder 中的注释。</param>
		public override void ExtendProperties(IPropertyChain[] properties, Boolean inline)
		{
			m_groupSchemaBuilder.ExtendProperties(properties, inline);
		}

		/// <summary>
		/// 创建分组架构。
		/// </summary>
		public override void BuildInfrastructure()
		{
			if (ItemSettings != null)
			{
				#region 前置断言

				Debug.Assert(ItemSettings.Select == null, "分组表达式架构不支持显示属性选择。");

				#endregion

				if (ItemSettings.Where != null)
				{
					m_groupSchemaBuilder.ExtendWhereFilter(ItemSettings.Where);
				}

				if (ItemSettings.Having != null)
				{
					m_groupSchemaBuilder.ExtendHavingFilter(ItemSettings.Having);
				}
			}

			m_groupSchema = m_groupSchemaBuilder.Build();
		}

		/// <summary>
		/// 创建 WHERE 过滤器表达式。
		/// </summary>
		/// <param name="parameterIndexOffset">参数索引偏移。</param>
		/// <returns>生成的查询参数集合。</returns>
		public override QueryParameter[] ComposeWhereFilterExpression(Int32 parameterIndexOffset)
		{
			if ((ItemSettings != null) && (ItemSettings.Where != null))
			{
				GeneralFilterCompilationContext context = new GeneralFilterCompilationContext(m_groupSchema.Composite.Target, ParameterPrefix);
				context.IndexOffset = parameterIndexOffset;

				FilterCompilationResult result = Filter.Compile(context, ItemSettings.Where);
				m_whereExpression = result.WhereClause;

				return result.Parameters;
			}
			else
			{
				return new QueryParameter[0];
			}
		}

		/// <summary>
		/// 创建 HAVING 过滤器表达式。
		/// </summary>
		/// <param name="parameterIndexOffset">参数索引偏移。</param>
		/// <returns>生成的参数集合。</returns>
		public override QueryParameter[] ComposeHavingFilterExpression(Int32 parameterIndexOffset)
		{
			if ((ItemSettings != null) && (ItemSettings.Having != null))
			{
				GroupSchemaFilterCompilationContext context = new GroupSchemaFilterCompilationContext(m_groupSchema, ParameterPrefix);
				context.IndexOffset = parameterIndexOffset;

				FilterCompilationResult result = Filter.Compile(context, ItemSettings.Having);
				m_havingExpression = result.WhereClause;

				return result.Parameters;
			}
			else
			{
				return new QueryParameter[0];
			}
		}

		/// <summary>
		/// 创建表达式架构。
		/// </summary>
		/// <param name="fieldIndexOffset">字段索引偏移。</param>
		/// <returns>表达式架构。</returns>
		public override ExpressionSchema Build(Int32 fieldIndexOffset)
		{
			m_groupSchema.SetFieldIndexOffset(fieldIndexOffset);

			// 生成包装列
			ExpressionSchemaColumn[] columns = new ExpressionSchemaColumn[m_groupSchema.SelectColumns.Length];

			for (Int32 i = 0; i < columns.Length; i++)
			{
				columns[i] = new ExpressionSchemaColumn(m_groupSchema.SelectColumns[i]);
				columns[i].Index += fieldIndexOffset;
			}

			// 创建 SQL 指令
			String sqlExpression = ComposeSqlExpression();

			GroupExpressionSchema schema = new GroupExpressionSchema(columns, sqlExpression);
			schema.GroupSchema = m_groupSchema;

			return schema;
		}

		#region 辅助方法

		/// <summary>
		/// 创建 SQL 指令。
		/// </summary>
		/// <returns>创建好的 SQL 指令。</returns>
		private String ComposeSqlExpression()
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			sqlBuilder.Select = m_groupSchema.SelectList;
			sqlBuilder.From = m_groupSchema.FromList;
			sqlBuilder.Where = m_whereExpression;
			sqlBuilder.GroupBy = m_groupSchema.GroupList;
			sqlBuilder.Having = m_havingExpression;

			return sqlBuilder.Build();
		}

		#endregion
	}
}