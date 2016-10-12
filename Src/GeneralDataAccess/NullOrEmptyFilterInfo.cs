#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NullOrEmptyFilterInfo.cs
// 文件功能描述：表示列为 NULL 或空字符串的过滤器。
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
	/// 表示列为 NULL 或空字符串的过滤器。
	/// </summary>
	[Serializable]
	internal sealed class NullOrEmptyFilterInfo : FilterInfo
	{
		#region 构造方法

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public NullOrEmptyFilterInfo()
		{
		}

		/// <summary>
		/// 构造函数。
		/// </summary>
		/// <param name="negative">指示是否执行“非”操作。</param>
		public NullOrEmptyFilterInfo(Boolean negative)
			: base(negative)
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
			return Filter.Combine(
					LogicOperator.Or,
					Filter.CreateIsNullFilter(propertyPath),
					Filter.CreateEqualsFilter(propertyPath, String.Empty)
				);
		}

		#endregion
	}
}