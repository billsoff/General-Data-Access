#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlEmptyExpressionFormatter.cs
// 文件功能描述：表示没有需要格式化的内容。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110713
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
	/// 表示没有需要格式化的内容。
	/// </summary>
	internal sealed class SqlEmptyExpressionFormatter : SqlInlineExpressionFormatter
	{
		/// <summary>
		/// 空实现。
		/// </summary>
		protected override void Format()
		{
			// 空实现，没有可格式化的内容
		}
	}
}