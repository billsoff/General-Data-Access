#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityExpressionSchemaBuilder.cs
// 文件功能描述：实体表达式架构生成器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110711
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
	/// 实体表达式架构生成器。
	/// </summary>
	internal class EntityExpressionSchemaBuilder : ExpressionSchemaBuilder
	{
		#region 私有字段

		private readonly EntityDefinition m_definition;
		private List<PropertySelector> m_allSelectors;
		private EntitySchemaComposite m_composite;

		private String m_whereExpression;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置复合实体属性定义和查询参数前缀。
		/// </summary>
		/// <param name="propertyDef">复合实体属性定义。</param>
		/// <param name="parameterPrefix">查询参数前缀。</param>
		public EntityExpressionSchemaBuilder(CompositeForeignReferencePropertyDefinition propertyDef, String parameterPrefix)
			: base(propertyDef, parameterPrefix)
		{
			m_definition = EntityDefinitionBuilder.Build(propertyDef.Type);
		}

		#endregion

		/// <summary>
		/// 扩展属性。
		/// </summary>
		/// <param name="properties">要扩展的属性集合。</param>
		/// <param name="inline">指示是否仅加载实体架构。</param>
		public override void ExtendProperties(IPropertyChain[] properties, Boolean inline)
		{
			if (inline)
			{
				Array.ForEach<IPropertyChain>(
						properties,
						delegate(IPropertyChain chain)
						{
							AllSelectors.Add(PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain));
						}
					);
			}
			else
			{
				Array.ForEach<IPropertyChain>(
						properties,
						delegate(IPropertyChain chain)
						{
							AllSelectors.Add(PropertySelector.Create(PropertySelectMode.Property, chain));
						}
					);
			}
		}

		/// <summary>
		/// 创建实体架构组合。
		/// </summary>
		public override void BuildInfrastructure()
		{
			BuildComposite();
		}

		/// <summary>
		/// 创建 WHERE 过滤器表达式。
		/// </summary>
		/// <param name="parameterIndexOffset">参数索引偏移。</param>
		/// <returns>生成的参数集合。</returns>
		public override QueryParameter[] ComposeWhereFilterExpression(Int32 parameterIndexOffset)
		{
			if ((ItemSettings != null) && (ItemSettings.Where != null))
			{
				EntitySchemaFilterCompilationContext context = new EntitySchemaFilterCompilationContext(m_composite.Target, ParameterPrefix);
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
		/// 生成 HAVING 过滤器表达式，当前的实现为不做任何操作。
		/// </summary>
		/// <param name="parameterIndexOffset">参数索引偏移。</param>
		/// <returns>查询参数集合。</returns>
		public override QueryParameter[] ComposeHavingFilterExpression(Int32 parameterIndexOffset)
		{
			#region 前置断言

			Debug.Assert((ItemSettings == null) || (ItemSettings.Having == null), "实体表达式架构不支持 HAVING 过滤器。");

			#endregion

			return new QueryParameter[0];
		}

		/// <summary>
		/// 开始生成。
		/// </summary>
		/// <param name="fieldIndexOffset">字段索引偏移。</param>
		/// <returns>表达式架构。</returns>
		public override ExpressionSchema Build(Int32 fieldIndexOffset)
		{
			m_composite.SetFieldIndexOffset(fieldIndexOffset);

			// 包装一次选择列，并建立选择属性和包装选择列的映射关系
			Dictionary<EntityProperty, Column[]> columnLookups = new Dictionary<EntityProperty, Column[]>();
			List<ExpressionSchemaColumn> allColumns = new List<ExpressionSchemaColumn>();

			foreach (EntityProperty property in m_composite.GetAllSelectProperties())
			{
				ExpressionSchemaColumn[] columns = new ExpressionSchemaColumn[property.Columns.Count];

				for (Int32 i = 0; i < columns.Length; i++)
				{
					columns[i] = new ExpressionSchemaColumn(property.Columns[i]);
					columns[i].Index += fieldIndexOffset;
				}

				columnLookups.Add(property, columns);
				allColumns.AddRange(columns);
			}

			// 合成 SQL 指令
			String sqlExpression = ComposeSqlExpression();

			EntityExpressionSchema schema = new EntityExpressionSchema(allColumns.ToArray(), sqlExpression);

			schema.ColumLookups = columnLookups;
			schema.Composite = m_composite;

			return schema;
		}

		#region 私有属性

		/// <summary>
		/// 获取属性选择器列表。
		/// </summary>
		public List<PropertySelector> AllSelectors
		{
			get
			{
				if (m_allSelectors == null)
				{
					m_allSelectors = new List<PropertySelector>();
				}

				return m_allSelectors;
			}
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 创建实体架构组合。
		/// </summary>
		private void BuildComposite()
		{
			if ((ItemSettings != null) && (ItemSettings.Where != null))
			{
				AllSelectors.AddRange(ItemSettings.Where.GetAllSelectors(CompositePropertyDefinition.Type));
			}

			CompositeBuilderStrategy loadStrategy;

			if ((ItemSettings != null) && (ItemSettings.Select != null))
			{
				loadStrategy = ItemSettings.Select;
			}
			else
			{
				LoadStrategyAttribute loadStrategyAttr = CompositePropertyDefinition.LoadStrategy ?? m_definition.LoadStrategy;
				loadStrategy = loadStrategyAttr.Create();
			}

			CompositeBuilderStrategy strategy = CompositeBuilderStrategyFactory.Compose(
					CompositePropertyDefinition.Type,
					loadStrategy,
					m_allSelectors
				);

			m_composite = EntitySchemaCompositeFactory.Create(CompositePropertyDefinition.Type, strategy);
		}

		/// <summary>
		/// 构造 SQL 指令。
		/// </summary>
		/// <returns>构造好的 SQL 指令。</returns>
		private String ComposeSqlExpression()
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			sqlBuilder.Select = m_composite.SelectList;
			sqlBuilder.From = m_composite.FromList;
			sqlBuilder.Where = m_whereExpression;

			return sqlBuilder.Build();
		}

		#endregion
	}
}