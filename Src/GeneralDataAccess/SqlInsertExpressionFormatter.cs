#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlInsertExpressionFormatter.cs
// 文件功能描述：插入指令格式化器。
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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 插入指令格式化器。
	/// </summary>
	internal sealed class SqlInsertExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region 私有字段

		private KeywordMatch m_insert;
		private KeywordMatch m_values;

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
		public SqlInsertExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// 格式化。
		/// </summary>
		protected override void Format()
		{
			FindKeywords();

			Int32 currentIndex = 0;

			if (!FormatInsert(ref currentIndex))
			{
				return;
			}

			FormatValues(ref currentIndex);
		}

		#region 辅助方法

		/// <summary>
		/// 查找关键字匹配。
		/// </summary>
		private void FindKeywords()
		{
			m_insert = FindInsertMatch();
			m_values = FindValuesMatch();
		}

		/// <summary>
		/// 查找 INSERT 关键字匹配。
		/// </summary>
		/// <returns>INSERT 关键字匹配。</returns>
		private KeywordMatch FindInsertMatch()
		{
			Match m = FindValidDelimiter(@"^\s*(?<Insert>INSERT)\s+(?<Into>INTO)\b");

			if (m != null)
			{
				String keyword = m.Groups["Insert"].Value + SqlExpressionFormattingHelper.SPACE + m.Groups["Into"].Value;

				return new KeywordMatch(m.Index, m.Length, keyword);
			}
			else
			{
				return new KeywordMatch();
			}
		}

		/// <summary>
		/// 查找 VALUES 关键字匹配。
		/// </summary>
		/// <returns>VALUES 关键字匹配。</returns>
		private KeywordMatch FindValuesMatch()
		{
			Match m = FindValidDelimiter(@"\bVALUES\b");

			if (m != null)
			{
				return new KeywordMatch(m.Index, m.Length, m.Value);
			}
			else
			{
				return new KeywordMatch();
			}
		}

		/// <summary>
		/// 格式化 INSERT 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		/// <returns>如果成功，则返回 true；否则返回 false。</returns>
		private Boolean FormatInsert(ref Int32 currentIndex)
		{
			if (!m_insert.Success)
			{
				AppendLiteral(FormattingText, true);
				currentIndex = this.Length;

				return false;
			}

			AppendLiteral(m_insert.Keyword, true);
			currentIndex += m_insert.Length;

			Int32 openParenthesisIndex;

			SqlExpressionFormatter formatter = EvaluateParenthesesExpression(
					currentIndex,
					delegate(
							String sqlExpression,
							Int32 startIndex,
							Int32 length,
							String indent,
							Int32 level,
							SqlParenthesesExpressionFormattingInfo formattingInfo
						)
					{
						formattingInfo.Formatter = CreateSimpleCommaDelimitingListFormatter((startIndex - this.StartIndex), length);
						formattingInfo.OuterOpenParenthesisStartsAtNewLine = true;
					},
					out openParenthesisIndex
				);

			if (formatter != null)
			{
				AppendSpace();
				AppendLiteral(FormattingText.Substring(currentIndex, (openParenthesisIndex - currentIndex)).Trim());
				currentIndex = openParenthesisIndex;

				AppendComposite(formatter);
				currentIndex += formatter.Length + 2; // 包括左右括号

				return true;
			}
			else
			{
				AppendLiteral(FormattingText.Substring(currentIndex, (this.Length - currentIndex)));
				currentIndex = this.Length;

				return false;
			}
		}

		/// <summary>
		/// 格式化 VALUES 子句。
		/// </summary>
		/// <param name="currentIndex">开始索引。</param>
		private void FormatValues(ref Int32 currentIndex)
		{
			if (!m_values.Success)
			{
				return;
			}

			AppendLiteral(m_values.Keyword, true);
			currentIndex += m_values.Length;

			Int32 openParenthesisIndex;

			SqlExpressionFormatter formatter = EvaluateParenthesesExpression(
					currentIndex,
					delegate(
							String sqlExpression,
							Int32 startIndex,
							Int32 length,
							String indent,
							Int32 level,
							SqlParenthesesExpressionFormattingInfo formattingInfo
						)
					{
						formattingInfo.Formatter = CreateSimpleCommaDelimitingListFormatter((startIndex - this.StartIndex), length);
						formattingInfo.OuterOpenParenthesisStartsAtNewLine = true;
					},
					out openParenthesisIndex
				);

			if (formatter != null)
			{
				AppendComposite(formatter);
				currentIndex = (openParenthesisIndex + formatter.Length + 2); // 包括左右括号
			}
			else
			{
				AppendLiteral(FormattingText.Substring(currentIndex, (this.Length - currentIndex)));
				currentIndex = this.Length;
			}
		}

		#endregion
	}
}