#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ExpressionSchemaType.cs
// 文件功能描述：表达式架构类型。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110712
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
	/// 表达式架构类型。
	/// </summary>
	internal enum ExpressionSchemaType
	{
		/// <summary>
		/// 未知。
		/// </summary>
		Unknown,

		/// <summary>
		/// 实体。
		/// </summary>
		Entity,

		/// <summary>
		/// 分组。
		/// </summary>
		Group
	}
}
