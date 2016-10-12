#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IsUserAttribute.cs
// 文件功能描述：属性别名“User”。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110330
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
	/// 属性别名“User”。
	/// </summary>
	public sealed class IsUserAttribute : PropertyAliasAttribute
	{
		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public IsUserAttribute()
			: base(WellKnown.User)
		{
		}

		#endregion
	}
}