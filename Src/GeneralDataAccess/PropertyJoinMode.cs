#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertyJoinMode.cs
// 文件功能描述：属性连接模式，用于指示复合实体中根实体架构与表达式实体架构的连接模式。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110707
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
	/// 属性连接模式，用于指示复合实体中根实体架构与表达式实体架构的连接模式。
	/// </summary>
	public enum PropertyJoinMode
	{
		/// <summary>
		/// 内连接。
		/// </summary>
		Inner,

		/// <summary>
		/// 左连接。
		/// </summary>
		Left,

		/// <summary>
		/// 右连接。
		/// </summary>
		Right
	}
}