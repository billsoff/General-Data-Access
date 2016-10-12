#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：MoveDirection.cs
// 文件功能描述：移位方向，用于类 DisplayOrderAllocator。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110331
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
	/// 移位方向。
	/// </summary>
	public enum MoveDirection
	{
		/// <summary>
		/// 移到最前。
		/// </summary>
		Top,

		/// <summary>
		/// 向上移一位。
		/// </summary>
		Up,

		/// <summary>
		/// 向下移一位。
		/// </summary>
		Down,

		/// <summary>
		/// 移到最后。
		/// </summary>
		Bottom,

		/// <summary>
		/// 进行规范化排序。
		/// </summary>
		Normalize
	}
}