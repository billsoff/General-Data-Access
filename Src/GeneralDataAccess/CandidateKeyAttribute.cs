#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CandidateKeyAttribute.cs
// 文件功能描述：指示所标记的属性为候补键，可标记于实体的多个属性上。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110725
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
	/// 指示所标记的属性为候补键，可标记于实体的多个属性上。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class CandidateKeyAttribute : Attribute
	{
	}
}