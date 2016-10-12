#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IFilterFactoryOperands.cs
// 文件功能描述：过滤器工厂操作数，供 FilterOperator 使用。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110325
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
	/// 过滤器工厂操作数，供 FilterOperator 使用。
	/// </summary>
	internal interface IFilterFactoryOperands
	{
		/// <summary>
		/// 获取一个参数。
		/// </summary>
		/// <returns>过滤器工厂。</returns>
		FilterFactory Pop();
	}
}