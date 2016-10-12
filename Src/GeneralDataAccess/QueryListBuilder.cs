#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：QueryListBuilder.cs
// 文件功能描述：查询列表生成器，用于 IN 过滤器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110721
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
	/// 查询列表生成器，用于 IN 过滤器。
	/// </summary>
	[Serializable]
	internal abstract class QueryListBuilder
	{
		#region 私有字段

		private readonly IPropertyChain m_property;
		private readonly Filter m_where;
		private readonly Filter m_having;
		private readonly Boolean m_distinct;

		private String m_whereClause;
		private String m_havingClause;

		[NonSerialized]
		private FilterCompilationContext m_parentContext;

		#endregion

		#region 静态方法

		/// <summary>
		/// 创建查询列表生成器。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示是否在 SQL 指令中包含 DISTINCT 关键字。</param>
		/// <returns>查询列表生成器。</returns>
		public static QueryListBuilder Create(IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			ExpressionSchemaType type = ExpressionSchemaBuilderFactory.GetExpressionSchemaType(property.Type);

			switch (type)
			{
				case ExpressionSchemaType.Entity:
					#region 前置条件

					Debug.Assert(havingFilter == null, "实体架构不能接收 HAVING 过滤器。");

					#endregion

					return new EntityQueryListBuilder(property, whereFilter, distinct);

				case ExpressionSchemaType.Group:
					return new GroupQueryListBuilder(property, whereFilter, havingFilter, distinct);

				case ExpressionSchemaType.Unknown:
				default:
					Debug.Fail(String.Format("类型 {0} 不可识别。", property.Type.FullName));
					return null;
			}
		}

		#endregion

		#region 构造方法

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		protected QueryListBuilder(IPropertyChain property, Filter where)
			: this(property, where, (Filter)null, false)
		{
		}

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		/// <param name="distinct">指示是否包含 DISTINC 关键字。</param>
		protected QueryListBuilder(IPropertyChain property, Filter where, Boolean distinct)
			: this(property, where, (Filter)null, distinct)
		{
		}

		/// <summary>
		/// 构造函数，设置初始值。
		/// </summary>
		/// <param name="property">要选择的属性。</param>
		/// <param name="where">WHERE 过滤器。</param>
		/// <param name="having">HAVING 过滤器。</param>
		protected QueryListBuilder(IPropertyChain property, Filter where, Filter having)
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
		protected QueryListBuilder(IPropertyChain property, Filter where, Filter having, Boolean distinct)
		{
			m_property = property;
			m_where = where;
			m_having = having;
			m_distinct = distinct;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取一个值，该值指示是否在 SQL 指令中包含 DISTINCT 关键字。
		/// </summary>
		public Boolean Distinct
		{
			get { return m_distinct; }
		}

		/// <summary>
		/// 获取要选择的属性。
		/// </summary>
		public IPropertyChain Property
		{
			get { return m_property; }
		}

		/// <summary>
		/// 获取 WHERE 过滤器。
		/// </summary>
		public Filter Where
		{
			get { return m_where; }
		}

		/// <summary>
		/// 获取 HAVING 过滤器。
		/// </summary>
		public Filter Having
		{
			get { return m_having; }
		}

		/// <summary>
		/// 获取生成的 WHERE 子句。
		/// </summary>
		public String WhereClause
		{
			get { return m_whereClause; }
		}

		/// <summary>
		/// 获取生成的 HAVING 子句。
		/// </summary>
		public String HavingClause
		{
			get { return m_havingClause; }
		}

		/// <summary>
		/// 获取查询参数前缀。。
		/// </summary>
		public String ParameterPrefix
		{
			get { return m_parentContext.ParameterPrefix; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建 SQL 指令。
		/// </summary>
		/// <param name="parentContext">父编译环境。</param>
		/// <returns>创建好的 SQL 指令。</returns>
		public String Build(FilterCompilationContext parentContext)
		{
			if (m_parentContext == null)
			{
				m_parentContext = parentContext;
			}

			SetParentContext();
			BuildInfrastructure();
			CompileFilters();

			// 创建 SQL 指令
			return ComposeSqlStatement();
		}

		#endregion

		#region 抽象成员

		/// <summary>
		/// 通知创建底层结构。
		/// </summary>
		protected abstract void BuildInfrastructure();

		/// <summary>
		/// 编译 WHERE 过滤器。
		/// </summary>
		/// <returns>编译结果。</returns>
		protected abstract FilterCompilationResult CompileWhereFilter();

		/// <summary>
		/// 编译 HAVING 过滤器。
		/// </summary>
		/// <returns>编译结果。</returns>
		protected abstract FilterCompilationResult CompileHavingFilter();

		/// <summary>
		/// 创建 SQL 指令。
		/// </summary>
		/// <returns>创建好的 SQL 指令。</returns>
		protected abstract String ComposeSqlStatement();

		#endregion

		#region 辅助方法

		/// <summary>
		/// 为 WHERE 和 HAVING 过滤器设置父编译环境。
		/// </summary>
		private void SetParentContext()
		{
			if (m_where != null)
			{
				QueryListBuilder[] whereBuilders = m_where.GetAllQueryListBuilders();

				SetParentContext(whereBuilders);
			}

			if (m_having != null)
			{
				QueryListBuilder[] havingBuilders = m_having.GetAllQueryListBuilders();

				SetParentContext(havingBuilders);
			}
		}

		/// <summary>
		/// 设置查询列表生成器的父编译环境。
		/// </summary>
		/// <param name="allBuilders">查询列表生成器集合。</param>
		private void SetParentContext(QueryListBuilder[] allBuilders)
		{
			Array.ForEach<QueryListBuilder>(
					allBuilders,
					delegate(QueryListBuilder builder)
					{
						builder.m_parentContext = m_parentContext;
					}
				);
		}

		/// <summary>
		/// 编译过滤器。
		/// </summary>
		private void CompileFilters()
		{
			if (m_where != null)
			{
				FilterCompilationResult whereResult = CompileWhereFilter();
				whereResult = Transform(whereResult);
				m_whereClause = whereResult.WhereClause;
			}

			if (m_having != null)
			{
				FilterCompilationResult havingResult = CompileHavingFilter();
				havingResult = Transform(havingResult);
				m_havingClause = havingResult.WhereClause;
			}
		}

		/// <summary>
		/// 转换过滤器编译结果。
		/// </summary>
		/// <param name="compilationResult">编译结果。</param>
		/// <returns>转换后新的结果。</returns>
		private FilterCompilationResult Transform(FilterCompilationResult compilationResult)
		{
			QueryParameter[] registeredParameters = m_parentContext.RegisterParameters(compilationResult.Parameters);
			String newClause = compilationResult.WhereClause;

			for (Int32 i = 0; i < registeredParameters.Length; i++)
			{
				newClause = newClause.Replace(
						ParameterPrefix + compilationResult.Parameters[i].Name,
						ParameterPrefix + registeredParameters[i].Name
					);
			}

			return new FilterCompilationResult(newClause, registeredParameters);
		}

		#endregion
	}
}