#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EnumParseErrorFollowup.cs
// 文件功能描述：指示当解析失败时采取的行动。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110802
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
	/// 指示当解析失败时采取的行动。
	/// </summary>
	public enum EnumParseErrorFollowup
	{
		/// <summary>
		/// 继续传播异常，这是默认选项。
		/// </summary>
		Broadcast,

		/// <summary>
		/// 设置为默认值。
		/// </summary>
		SetDefault
	}
}