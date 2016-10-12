#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IDisplayOrderOwner.cs
// 文件功能描述：表示实体拥有 DisplayOrder 属性，可以显式地排序。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110331
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
	/// 表示实体拥有 DisplayOrder 属性，可以显式地排序。
	/// </summary>
	public interface IDisplayOrderOwner
	{
		/// <summary>
		/// 获取或设置实体的显示序号。
		/// </summary>
		Int32 DisplayOrder { get; set; }
	}
}