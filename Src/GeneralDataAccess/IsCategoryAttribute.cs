#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IsAsCategoryAttribute.cs
// 文件功能描述：属性别名“Category”。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 2011
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
	/// 属性别名“Category”。
	/// </summary>
	public sealed class IsCategoryAttribute : PropertyAliasAttribute
	{
		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public IsCategoryAttribute()
			: base(WellKnown.Category)
		{
		}

		#endregion
	}
}