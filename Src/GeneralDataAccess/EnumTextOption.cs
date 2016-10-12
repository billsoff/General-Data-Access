#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EnumTextOption.cs
// 文件功能描述：指示如何取枚举的文本。
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
	/// 指示如何取枚举的文本。
	/// </summary>
	public enum EnumTextOption
	{
		/// <summary>
		/// 取枚举的名称，这是默认选项。
		/// </summary>
		Name,

		/// <summary>
		/// 取枚举的值。
		/// </summary>
		Value
	}
}