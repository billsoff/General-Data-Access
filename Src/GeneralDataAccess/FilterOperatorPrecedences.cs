#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterOperatorPrecedences.cs
// 文件功能描述：用于枚举过滤器操作符的优先级。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110325
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
	/// 用于枚举过滤器操作符的优先级。
	/// </summary>
	internal static class FilterOperatorPrecedences
	{
		/// <summary>
		/// 最高优先级 1。
		/// </summary>
		public const Int32 HIGHEST = 1;

		/// <summary>
		/// 优先级 2。
		/// </summary>
		public const Int32 ABOVE_NORMAL = 2;

		/// <summary>
		/// 优先级 3。
		/// </summary>
		public const Int32 NORMAL = 3;

		/// <summary>
		/// 优先级 4。
		/// </summary>
		public const Int32 BELOW_NORMAL = 4;
	}
}