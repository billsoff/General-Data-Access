#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IPropertyChainBuilder.cs
// 文件功能描述：属性链生成器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110430
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
	/// 属性链生成器。
	/// </summary>
	public interface IPropertyChainBuilder
	{
		/// <summary>
		/// 获取一个值，该值批示当前是否可以生成属性链。
		/// </summary>
		Boolean Available { get; }

		/// <summary>
		/// 生成属性链。
		/// </summary>
		/// <returns>生成好的属性链。</returns>
		IPropertyChain Build();
	}
}