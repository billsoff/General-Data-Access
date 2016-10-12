#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlParenthesesExpressionFormattingInfo.cs
// 文件功能描述：用于设置括号表达式格式信息。
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
	/// 用于设置括号表达式格式信息。
	/// </summary>
	internal class SqlParenthesesExpressionFormattingInfo
	{
		#region 私有字段

		private Boolean m_outerOpenParenthesisStartsNewLine;
		private SqlExpressionFormatter m_formatter;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public SqlParenthesesExpressionFormattingInfo()
		{
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取或设置一个值，该值指示左括号是否位于新行，默认为 false。
		/// </summary>
		public Boolean OuterOpenParenthesisStartsAtNewLine
		{
			get { return m_outerOpenParenthesisStartsNewLine; }
			set { m_outerOpenParenthesisStartsNewLine = value; }
		}

		/// <summary>
		/// 获取或设置对表达式的格式化器，如果为 null，则表达将原样输出。
		/// </summary>
		internal SqlExpressionFormatter Formatter
		{
			get { return m_formatter; }
			set { m_formatter = value; }
		}

		#endregion
	}
}