#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：BetweenFilterInfo.cs
// 文件功能描述：BETWEEN 过滤器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110327
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
	/// BETWEEN 过滤器。
	/// </summary>
	[Serializable]
	internal sealed class BetweenFilterInfo : FilterInfo
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置范围。
		/// </summary>
		/// <param name="from">起始点。</param>
		/// <param name="to">终结点。</param>
		public BetweenFilterInfo(Object from, Object to)
			: base(new Object[] { from, to })
		{
		}

		/// <summary>
		/// 构造函数，设置是否执行“非”操作和范围。
		/// </summary>
		/// <param name="negative">指示是否执行“非”操作。</param>
		/// <param name="from">起始点。</param>
		/// <param name="to">终结点。</param>
		public BetweenFilterInfo(Boolean negative, Object from, Object to)
			: base(negative, new Object[] { from, to })
		{
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>创建好的过滤器。</returns>
		protected override Filter DoCreateFilter(IList<String> propertyPath)
		{
			return Filter.CreateBetweenFilter(propertyPath, Values[0], Values[1]);
		}

		#endregion
	}
}