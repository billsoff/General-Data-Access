#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PermitNullAttribute.cs
// 文件功能描述：用于标记实体属性，指示其可以为空（在查询时和主表的连接为左连接，否则为内连接）。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008132309
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
	/// 用于标记实体属性，指示其可以为空（在查询时和主表的连接为左连接，否则为内连接）。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class PermitNullAttribute : Attribute
	{
	}
}