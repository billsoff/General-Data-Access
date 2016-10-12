#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlStatementType.cs
// 文件功能描述：SQL 指令类型。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110715
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
	/// SQL 指令类型。
	/// </summary>
	internal enum SqlStatementType
	{
		/// <summary>
		/// 不可识别。
		/// </summary>
		Unrecognisable,

		/// <summary>
		/// 选择。
		/// </summary>
		Select,

		/// <summary>
		/// 更新。
		/// </summary>
		Update,

		/// <summary>
		/// 插入。
		/// </summary>
		Insert,

		/// <summary>
		/// 删除。
		/// </summary>
		Delete
	}
}