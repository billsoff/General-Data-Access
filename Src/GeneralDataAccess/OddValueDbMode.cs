#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：OddValueDbMode.cs
// 文件功能描述：指示对于奇异值如何转换成数据库值。
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
	/// 指示对于奇异值如何转换成数据库值。
	/// </summary>
	public enum OddValueDbMode
	{
		/// <summary>
		/// 不改变其值，原样保存。
		/// </summary>
		NotChange,

		/// <summary>
		/// 转换为数据库空值。
		/// </summary>
		DBNull,

		/// <summary>
		/// 写数据库时值由数据库决定（即在动作查询指令中忽略该列）。
		/// </summary>
		Ignore
	}
}