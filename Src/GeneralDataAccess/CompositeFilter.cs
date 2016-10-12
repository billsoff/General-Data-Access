#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeFilter.cs
// 文件功能描述：表示一个复合过滤器，这是一个内部类，由 Filter.Combine 方法使用。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20100815
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
	/// 表示一个复合过滤器，这是一个内部类，由 Filter.Combine 方法使用。
	/// </summary>
	[Serializable]
	internal class CompositeFilter : Filter
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置逻辑连接符和子过滤器集合。
		/// </summary>
		/// <param name="logicOperator">逻辑连接符，AND 或 OR。</param>
		/// <param name="filters">子过滤器集合。</param>
		public CompositeFilter(LogicOperator logicOperator, IEnumerable<Filter> filters)
			: base(logicOperator)
		{
			foreach (Filter f in filters)
			{
				this.Filters.Add(f);
			}
		}

		#endregion
	}
}