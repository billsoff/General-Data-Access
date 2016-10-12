#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityQueryListBuilder.cs
// 文件功能描述：实体查询列表生成器。
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
	/// 实体查询列表生成器。
	/// </summary>
	[Serializable]
	internal sealed class EntityQueryListBuilder : QueryListBuilder
	{
		#region 私有字段

		[NonSerialized]
		private EntitySchemaComposite m_composite;

		#endregion

		#region 构造方法

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		public EntityQueryListBuilder(IPropertyChain property, Filter where)
			: this(property, where, false)
		{
		}

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		/// <param name="distinct">指示是否包含 DISTINC 关键字。</param>
		public EntityQueryListBuilder(IPropertyChain property, Filter where, Boolean distinct)
			: base(property, where, distinct)
		{
			#region 前置断言

#if DEBUG

			EntityDefinition definition = EntityDefinitionBuilder.Build(property.Type);
			EntityPropertyDefinition propertyDef = definition.Properties[property];

			Debug.Assert(!propertyDef.HasComproundColumns, String.Format("属性 {0} 映射到复合列，不能用于过滤器子查询列表。", property.FullName));

#endif

			#endregion
		}

		#endregion

		/// <summary>
		/// 创建实体架构组合。
		/// </summary>
		protected override void BuildInfrastructure()
		{
			List<PropertySelector> allSelectors = new List<PropertySelector>();

			allSelectors.Add(PropertySelector.Create(Property));

			if (Where != null)
			{
				allSelectors.AddRange(Where.GetAllSelectors(Property.Type));
			}

			CompositeBuilderStrategy builderStrategy = CompositeBuilderStrategyFactory.Create(allSelectors);
			builderStrategy.AlwaysSelectPrimaryKeyProperties = false;

			m_composite = EntitySchemaCompositeFactory.Create(Property.Type, builderStrategy);
		}

		/// <summary>
		/// 编译 WHERE 过滤器。
		/// </summary>
		/// <returns>编译结果。</returns>
		protected override FilterCompilationResult CompileWhereFilter()
		{
			return m_composite.ComposeFilterExpression(Where, ParameterPrefix);
		}

		/// <summary>
		/// 编译 HAVING 过滤器。
		/// </summary>
		/// <returns>总是返回 null。</returns>
		protected override FilterCompilationResult CompileHavingFilter()
		{
			#region 前置断言

			Debug.Assert(Having == null, "实体选择列表不能接收 HAVING 过滤器。");

			#endregion

			return null;
		}

		/// <summary>
		/// 创建 SQL 指令。
		/// </summary>
		/// <returns>创建好的 SQL 指令。</returns>
		protected override String ComposeSqlStatement()
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			Column[] columns = m_composite.Target[new ColumnLocator(Property.PropertyPath)];

			sqlBuilder.Select = columns[0].FullName;
			sqlBuilder.From = m_composite.FromList;
			sqlBuilder.Where = WhereClause;

			sqlBuilder.Distinct = Distinct;

			return sqlBuilder.Build();
		}
	}
}