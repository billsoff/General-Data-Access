#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：StoredProcedureParameterValueGotter.cs
// 文件功能描述：用于获取存储过程的非输入参数的值。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110214
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
	/// 用于获取存储过程的非输入参数的值。
	/// </summary>
	/// <param name="parameterName">参数名称。</param>
	/// <returns>参数值。</returns>
	public delegate Object StoredProcedureParameterValueGotter(String parameterName);
}