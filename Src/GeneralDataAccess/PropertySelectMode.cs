#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertySelectMode.cs
// 文件功能描述：属性选择模式。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110517
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
	/// 属性选择模式。
	/// </summary>
	public enum PropertySelectMode
	{
		/// <summary>
		/// 选择实体中的所有属性。
		/// </summary>
		AllFromSchema,

		/// <summary>
		/// 选择实体中的所有非延迟加载属性。
		/// </summary>
		AllExceptLazyLoadFromSchema,

		/// <summary>
		/// 不选择属性（即实体所映射的表只出现在 FROM 子句中（一般是因为 WHERE 或 ORDER 子句要用到其中的字段））。
		/// </summary>
		LoadSchemaOnly,

		/// <summary>
		/// 选择属性。
		/// </summary>
		Property,

		/// <summary>
		/// 选择主键。
		/// </summary>
		PrimaryKey
	}
}