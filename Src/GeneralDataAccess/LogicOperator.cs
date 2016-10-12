#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：LogicOperator.cs
// 文件功能描述：表示逻辑连接符。
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
	/// 表示逻辑连接符。
	/// </summary>
	public enum LogicOperator
	{
		/// <summary>
		/// 与操作。
		/// </summary>
		And,

		/// <summary>
		/// 或操作。
		/// </summary>
		Or
	}
}