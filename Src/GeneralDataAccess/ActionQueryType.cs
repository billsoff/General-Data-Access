#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ActionQueryType.cs
// 文件功能描述：进行数据库事务操作时，对实体进行操作的类型（添加、删除和修改）。
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
	/// 进行数据库事务操作时，对实体进行操作的类型（添加、删除和修改）。
	/// </summary>
	public enum ActionQueryType
	{
		/// <summary>
		/// 添加。
		/// </summary>
		Add,

		/// <summary>
		/// 删除。
		/// </summary>
		Delete,

		/// <summary>
		/// 修改。
		/// </summary>
		Modify,
	}
}