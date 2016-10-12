#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：用于标记键属性，指示在创建游离实体时自动为主键生成值。
// 文件功能描述：
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110406
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
	/// 用于标记键属性，指示在创建游离实体时自动为主键生成值。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class AutoGenerateOnNewAttribute : Attribute
	{
	}
}