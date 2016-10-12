#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IColumnLocatable.cs
// 文件功能描述：通过列定位符获取指定的列集合。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110706
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
	/// 通过列定位符获取指定的列集合。
	/// </summary>
	public interface IColumnLocatable
	{
		/// <summary>
		/// 根据列定位符查找匹配的列集合。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>找到的列集合。</returns>
		Column[] this[ColumnLocator colLocator] { get; }
	}
}