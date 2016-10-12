#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EqualToFilterInfo.cs
// 文件功能描述：相等过滤器。
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
	/// 相等过滤器。
	/// </summary>
	[Serializable]
	internal sealed class EqualToFilterInfo : FilterInfo
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性值。
		/// </summary>
		/// <param name="propertyValue">要比较的属性值。</param>
		public EqualToFilterInfo(Object propertyValue)
			: base(new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// 构造函数，设置是否为“非”操作和要比较的值。
		/// </summary>
		/// <param name="negative">指示是否要进行“非”操作。</param>
		/// <param name="propertyValue">要比较的属性值。</param>
		public EqualToFilterInfo(Boolean negative, Object propertyValue)
			: base(negative, new Object[] { propertyValue })
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
			return Filter.CreateEqualsFilter(propertyPath, Values[0]);
		}

		#endregion
	}
}