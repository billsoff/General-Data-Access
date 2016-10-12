#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：LazyLoadAttribute.cs
// 文件功能描述：标记实体属性，指示延迟加载，只能用于非主键的值属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110415
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
	/// 标记实体属性，指示延迟加载，只能用于非主键的值属性。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class LazyLoadAttribute : Attribute
	{
	}
}