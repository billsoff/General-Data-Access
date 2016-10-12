#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SuppressExpandAttribute.cs
// 文件功能描述：标记外部引用属性，指示当加载时不加载其所引用的外部引用。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110505
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
	/// 标记外部引用属性，指示当加载时不加载其所引用的外部引用。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class SuppressExpandAttribute : Attribute
	{
	}
}