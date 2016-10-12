#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterCompilationResult.cs
// 文件功能描述：用于存放编译过滤器的结果。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110706
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
	/// 用于存放编译过滤器的结果。
	/// </summary>
	[Serializable]
	public sealed class FilterCompilationResult
	{
		#region 私有字段

		private readonly String m_whereClause;
		private readonly QueryParameter[] m_parameters;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，初始化编译结果。
		/// </summary>
		/// <param name="whereClause">条件子句（不包含 WHERE 关键字）。</param>
		/// <param name="parameters">查询参数（查询参数的名称和值）集合。</param>
		public FilterCompilationResult(String whereClause, QueryParameter[] parameters)
		{
			m_whereClause = whereClause;
			m_parameters = parameters;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取条件子句（不包含 WHERE 关键字）。
		/// </summary>
		public String WhereClause
		{
			get { return m_whereClause; }
		}

		/// <summary>
		/// 获取查询参数（查询参数的名称和值）集合。
		/// </summary>
		public QueryParameter[] Parameters
		{
			get { return m_parameters; }
		}

		#endregion
	}
}