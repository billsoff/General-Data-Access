#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlSelectExpressionBuilder.cs
// 文件功能描述：选择 SQL 指令生成器。
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
	/// 选择 SQL 指令生成器。
	/// </summary>
	public struct SqlSelectExpressionBuilder
	{
		#region 私有字段

		private Boolean m_distinct;

		private String m_from;
		private String m_select;
		private String m_where;
		private String m_groupBy;
		private String m_having;
		private String m_orderBy;

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取或设置一个值，该值指示是否在 SQL 指令中包含 DISTINCT 关键字，默认为 false。
		/// </summary>
		public Boolean Distinct
		{
			get { return m_distinct; }
			set { m_distinct = value; }
		}

		/// <summary>
		/// 获取或设置选择列表。
		/// </summary>
		public String Select
		{
			get { return m_select; }
			set { m_select = value; }
		}

		/// <summary>
		/// 获取或设置表连接表达式。
		/// </summary>
		public String From
		{
			get { return m_from; }
			set { m_from = value; }
		}

		/// <summary>
		/// 获取或设置 WHERE 过滤表达式。
		/// </summary>
		public String Where
		{
			get { return m_where; }
			set { m_where = value; }
		}

		/// <summary>
		/// 获取或设置分组表达式。
		/// </summary>
		public String GroupBy
		{
			get { return m_groupBy; }
			set { m_groupBy = value; }
		}

		/// <summary>
		/// 获取或设置 HAVING 过滤表达式。
		/// </summary>
		public String Having
		{
			get { return m_having; }
			set { m_having = value; }
		}

		/// <summary>
		/// 获取或设置排序表达式。
		/// </summary>
		public String OrderBy
		{
			get { return m_orderBy; }
			set { m_orderBy = value; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 生成 SQL 指令。
		/// </summary>
		/// <returns>生成好的 SQL 指令。</returns>
		public String Build()
		{
			#region 前置断言

			Debug.Assert(m_select != null, "选择列表为空，无法生成 SQL 指令。");
			Debug.Assert(m_from != null, "表连接表达式为空，无法生成 SQL 指令。");

			#endregion

			// 开始构造 SQL 指令
			StringBuilder buffer = new StringBuilder();

			buffer.AppendFormat(
					"SELECT{0} {1} FROM {2}",
					m_distinct ? " DISTINCT" : String.Empty,
					m_select,
					m_from
				);

			if (m_where != null)
			{
				buffer.AppendFormat(" WHERE {0}", m_where);
			}

			if (m_groupBy != null)
			{
				buffer.AppendFormat(" GROUP BY {0}", m_groupBy);

				if (m_having != null)
				{
					buffer.AppendFormat(" HAVING {0}", m_having);
				}
			}

			Debug.Assert((m_having == null) || (m_groupBy != null), "因分组表达式为空，HAVING 过滤表达式不能参与生成 SQL 指令。");

			if (m_orderBy != null)
			{
				buffer.AppendFormat(" ORDER BY {0}", m_orderBy);
			}

			String sql = buffer.ToString();

			return sql;
		}

		#endregion
	}
}