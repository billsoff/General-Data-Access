#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlParenthesesExpressionFormatter.cs
// 文件功能描述：括号表达式格式化器。
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
	/// 括号表达式格式化器，接受的是完整的括号表达式，并对内层表达式进行格式化。
	/// </summary>
	internal sealed class SqlParenthesesExpressionFormatter : SqlExpressionFormatter
	{
		#region 私有字段

		private readonly Boolean m_isBlock;
		private readonly SqlExpressionFormatter m_formatter;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置要格式化的 SQL 指令等。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <param name="startIndex">开始索引。</param>
		/// <param name="length">长度。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="level">级别。</param>
		/// <param name="evaluator">委托，用于获取对括号表达式进行格式化的指示。</param>
		public SqlParenthesesExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level, SqlParenthesesExpressionEvaluator evaluator)
			: base(sqlExpression, startIndex, length, indent, level)
		{
			// 括号内的表达式
			Int32 expressionStartIndex = 1; // 从左括号后开始
			Int32 expressionLength = this.Length - 2; // 减去左右括号

			SqlParenthesesExpressionFormattingInfo formattingInfo = new SqlParenthesesExpressionFormattingInfo();

			if (evaluator != null)
			{
				evaluator(SqlExpression, (this.StartIndex + expressionStartIndex), expressionLength, Indent, Level, formattingInfo);
			}

			m_isBlock = formattingInfo.OuterOpenParenthesisStartsAtNewLine;

			if (formattingInfo.Formatter != null)
			{
				m_formatter = formattingInfo.Formatter;
			}
			else
			{
				m_formatter = CreateLiteralFormatter(expressionStartIndex, expressionLength);
			}
		}

		#endregion

		/// <summary>
		/// 判断是否为块输出。
		/// </summary>
		public override Boolean IsBlock
		{
			get { return m_isBlock; }
		}

		/// <summary>
		/// 格式化。
		/// </summary>
		protected override void Format()
		{
			AppendOpenParenthesis(true);
			AppendComposite(m_formatter);
			AppendCloseParenthesis(m_formatter.IsBlock);
		}
	}
}