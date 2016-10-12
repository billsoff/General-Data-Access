#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupQueryListBuilder.cs
// 文件功能描述：分组实体查询列表生成器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110722
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
	/// 分组实体查询列表生成器。
	/// </summary>
	[Serializable]
	internal sealed class GroupQueryListBuilder : QueryListBuilder
	{
		#region 前置条件

		[NonSerialized]
		private GroupSchema m_schema;

		#endregion

		#region 构造方法

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		public GroupQueryListBuilder(IPropertyChain property, Filter where)
			: this(property, where, (Filter)null, false)
		{
		}

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		/// <param name="distinct">指示是否包含 DISTINC 关键字。</param>
		public GroupQueryListBuilder(IPropertyChain property, Filter where, Boolean distinct)
			: this(property, where, (Filter)null, distinct)
		{
		}

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		/// <param name="having">HAVING 过滤器。</param>
		public GroupQueryListBuilder(IPropertyChain property, Filter where, Filter having)
			: this(property, where, having, false)
		{
		}

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		/// <param name="having">HAVING 过滤器。</param>
		/// <param name="distinct">指示是否包含 DISTINC 关键字。</param>
		public GroupQueryListBuilder(IPropertyChain property, Filter where, Filter having, Boolean distinct)
			: base(property, where, having, distinct)
		{
		}

		#endregion

		/// <summary>
		/// 生成基础架构。
		/// </summary>
		protected override void BuildInfrastructure()
		{
			GroupSchemaBuilder schemaBuilder = new GroupSchemaBuilder(Property.Type);

			schemaBuilder.ExtendWhereFilter(Where);
			schemaBuilder.ExtendHavingFilter(Having);
			schemaBuilder.ExtendProperties(new IPropertyChain[] { Property }, false);

			m_schema = schemaBuilder.Build();
		}

		/// <summary>
		/// 编译 WHERE 过滤器。
		/// </summary>
		/// <returns>编译结果。</returns>
		protected override FilterCompilationResult CompileWhereFilter()
		{
			return m_schema.Composite.ComposeFilterExpression(Where);
		}

		/// <summary>
		/// 编译 HAVING 过滤器。
		/// </summary>
		/// <returns>编译结果。</returns>
		protected override FilterCompilationResult CompileHavingFilter()
		{
			return m_schema.ComposeFilterExpression(Having);
		}

		/// <summary>
		/// 创建 SQL 指令。
		/// </summary>
		/// <returns>创建好的 SQL 指令。</returns>
		protected override String ComposeSqlStatement()
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			Column[] columns = m_schema[new ColumnLocator(Property.PropertyPath)];

			#region 前置条件

			Debug.Assert(columns.Length == 1, String.Format("属性 {0} 映射的列为复合列或不存在。", Property.FullName));

			#endregion

			sqlBuilder.Select = columns[0].Expression;
			sqlBuilder.From = m_schema.FromList;
			sqlBuilder.Where = WhereClause;
			sqlBuilder.GroupBy = ComposeGroupList();
			sqlBuilder.Having = HavingClause;

			sqlBuilder.Distinct = Distinct;

			return sqlBuilder.Build();
		}

		#region 辅助方法

		/// <summary>
		/// 创建 GROUP BY 子句。
		/// </summary>
		/// <returns>分组列表。</returns>
		private String ComposeGroupList()
		{
			// 获取一个精简的分组列表
			Column[] conciseGroupColumns = GetConciseGroupColumns();

			// 构造分组表达式
			StringBuilder output = new StringBuilder();

			for (Int32 i = 0; i < conciseGroupColumns.Length; i++)
			{
				if (i != 0)
				{
					output.Append(",");
				}

				output.Append(conciseGroupColumns[i].FullName);
			}

			return output.ToString();
		}

		/// <summary>
		/// 获取精简的分组列。
		/// </summary>
		/// <returns>分组列集合。</returns>
		private Column[] GetConciseGroupColumns()
		{
			List<Column> results = new List<Column>();

			foreach (GroupPropertyDefinition propertyDef in m_schema.Definition.Properties)
			{
				if (!propertyDef.IsGroupItem)
				{
					continue;
				}

				if (propertyDef.IsPrimitive)
				{
					results.AddRange(
							m_schema.Composite.Target[new ColumnLocator(propertyDef.PropertyChain.PropertyPath)]
						);
				}
				else
				{
					EntitySchema schema = m_schema.Composite[propertyDef.PropertyChain];

					if (schema.PrimaryKeyColumns.Count != 0)
					{
						results.AddRange(schema.PrimaryKeyColumns);
					}
					else
					{
						foreach (Column col in schema.Columns)
						{
							if (!col.Property.IsCustomAttributeDefined(typeof(NotSupportGroupingAttribute)))
							{
								results.Add(col);
							}
						}
					}
				}
			}

			results.AddRange(
					Array.FindAll<Column>(
							m_schema.GroupColumns,
							delegate(Column col)
							{
								if (results.Contains(col))
								{
									return false;
								}

								if (!col.Selected)
								{
									return true;
								}

								if (col.Property == null)
								{
									return true;
								}

								if (col.Property.PropertyChain.Equals(Property))
								{
									return true;
								}

								return false;
							}
						)
				);

			return results.ToArray();
		}

		#endregion
	}
}