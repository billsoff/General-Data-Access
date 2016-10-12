#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlParenthesesExpressionEvaluator.cs
// 文件功能描述：用于指示格式化括号表达式的方法。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110715
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
	/// 用于指示格式化括号表达式的方法。
	/// </summary>
	/// <param name="sqlExpression">SQL 指令。</param>
	/// <param name="startIndex">要处理的开始索引。</param>
	/// <param name="length">要处理的长度。</param>
	/// <param name="indent">缩进。</param>
	/// <param name="level">当前级别。</param>
	/// <param name="formattingInfo">格式化信息，用于设置指示如何格式化括号表达的信息。</param>
	internal delegate void SqlParenthesesExpressionEvaluator(
			String sqlExpression,
			Int32 startIndex,
			Int32 length,
			String indent,
			Int32 level,
			SqlParenthesesExpressionFormattingInfo formattingInfo
		);
}