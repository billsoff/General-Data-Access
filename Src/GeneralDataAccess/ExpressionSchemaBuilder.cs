#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ExpressionSchemaBuilder.cs
// 文件功能描述：表示一个表达式生成器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110707
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
	/// 表示一个表达式生成器。
	/// </summary>
	internal abstract class ExpressionSchemaBuilder
	{
		#region 私有字段

		private readonly CompositeForeignReferencePropertyDefinition m_compositePropertyDefinition;
		private readonly String m_parameterPrefix;

		private CompositeItemSettings m_itemSettings;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置类型和查询参数前缀。
		/// </summary>
		/// <param name="propertyDef">复合实体属性定义。</param>
		/// <param name="parameterPrefix">查询参数前缀。</param>
		protected ExpressionSchemaBuilder(CompositeForeignReferencePropertyDefinition propertyDef, String parameterPrefix)
		{
			m_compositePropertyDefinition = propertyDef;
			m_parameterPrefix = parameterPrefix;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取参数前缀。
		/// </summary>
		public String ParameterPrefix
		{
			get { return m_parameterPrefix; }
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取复合实体属性定义。
		/// </summary>
		internal CompositeForeignReferencePropertyDefinition CompositePropertyDefinition
		{
			get { return m_compositePropertyDefinition; }
		}

		#endregion

		/// <summary>
		/// 获取或设置过滤器。
		/// </summary>
		public CompositeItemSettings ItemSettings
		{
			get { return m_itemSettings; }
			set { m_itemSettings = value; }
		}

		/// <summary>
		/// 扩展给定的属性。
		/// </summary>
		/// <param name="properties">要扩展的属性链集合。</param>
		/// <param name="inline">
		/// <para>指示是否以内联的方式扩展。</para>
		/// <para>true - 对于 WHERE 和 HAVING 过滤器，只需要加载属性所包含的实体架构（对于分组查询的 HAVING 过滤器，要么该属性为聚合属性，要么为分组依据）。</para>
		/// <para>false - 要保证属性被选择。</para>
		/// </param>
		public abstract void ExtendProperties(IPropertyChain[] properties, Boolean inline);

		/// <summary>
		/// 通知创建底层结构。
		/// </summary>
		public abstract void BuildInfrastructure();

		/// <summary>
		/// 创建 WHERE 过滤器表达式。
		/// </summary>
		/// <param name="parameterIndexOffset">参数索引偏移值。</param>
		/// <returns>生成的查询参数列表。</returns>
		public abstract QueryParameter[] ComposeWhereFilterExpression(Int32 parameterIndexOffset);

		/// <summary>
		/// 创建 HAVING 过滤器表达式。
		/// </summary>
		/// <param name="parameterIndexOffset">参数索引偏移值。</param>
		/// <returns>生成的参数列表。</returns>
		public abstract QueryParameter[] ComposeHavingFilterExpression(Int32 parameterIndexOffset);

		/// <summary>
		/// 生成表达式实体架构。
		/// </summary>
		/// <param name="fieldIndexOffset">字段偏移量。</param>
		/// <returns>生成好的表达式实体架构。</returns>
		public abstract ExpressionSchema Build(Int32 fieldIndexOffset);
	}
}