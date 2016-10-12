#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlCriteriaExpressionFormatter.cs
// 文件功能描述：过滤条件格式化器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110714
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
	/// 过滤条件格式化器。
	/// </summary>
	internal sealed class SqlCriteriaExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region 私有字段

		private readonly Boolean m_statementLevel;

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
		public SqlCriteriaExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: this(sqlExpression, startIndex, length, indent, level, false)
		{
		}

		/// <summary>
		/// 构造函数，设置要格式化的 SQL 指令等。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <param name="startIndex">开始索引。</param>
		/// <param name="length">长度。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="level">级别。</param>
		/// <param name="statementLevel">指示是否为语句级别，如果为 true，则即使只包含一个子项，该子项也应缩进，否则，不需要缩进。</param>
		public SqlCriteriaExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level, Boolean statementLevel)
			: base(sqlExpression, startIndex, length, indent, level)
		{
			m_statementLevel = statementLevel;
		}

		#endregion

		/// <summary>
		/// 格式化。
		/// </summary>
		protected override void Format()
		{
			if (TryDelimitByOr())
			{
				return;
			}

			if (TryDelimitByAnd())
			{
				return;
			}

			if (m_statementLevel)
			{
				AppendComposite(CreateCriteriaFormatter(0, Length));
			}
			else
			{
				FormatCriterion();
			}
		}

		#region 辅助方法

		/// <summary>
		/// 尝试使用 OR 进行分割。
		/// </summary>
		/// <returns>如果分割成功，则返回 true，否则返回 false。</returns>
		private Boolean TryDelimitByOr()
		{
			FieldItem[] allFields = GetAllFieldItems(@"\bOR\b");

			if (allFields.Length == 1)
			{
				return false;
			}

			for (Int32 i = 0; i < allFields.Length; i++)
			{
				if (i != 0)
				{
					AppendLiteral("OR", true);
				}

				FieldItem field = allFields[i];

				AppendComposite(CreateCriteriaFormatter(field.Index, field.Length));
			}

			return true;
		}

		/// <summary>
		/// 尝试使用 AND 进行分割。
		/// </summary>
		/// <returns>如果分割成功，则返回 true；否则返回 false。</returns>
		private Boolean TryDelimitByAnd()
		{
			// 如果存在顶级 BETWEEN 分割符，则表明这是一个简单项
			Match between = FindValidDelimiter(@"\bBETWEEN\b");

			if (between != null)
			{
				return false;
			}

			FieldItem[] allFields = GetAllFieldItems(@"\bAND\b");

			if (allFields.Length == 1)
			{
				return false;
			}

			for (Int32 i = 0; i < allFields.Length; i++)
			{
				if (i != 0)
				{
					AppendLiteral("AND", true);
				}

				FieldItem field = allFields[i];

				AppendComposite(CreateCriteriaFormatter(field.Index, field.Length));
			}

			return true;
		}

		/// <summary>
		/// 格式化 NOT 关键字。
		/// </summary>
		/// <param name="currentIndex">设置格式化后的位置。</param>
		/// <returns>如果存在 NOT 关键字，则返回 true；否则返回 false。</returns>
		private Boolean FormatNot(out Int32 currentIndex)
		{
			Match not = Regex.Match(
					FormattingText,
					@"^\s*NOT\b",
					DEFAULT_REGEX_OPTIONS
				);

			if (not.Success)
			{
				AppendLiteral("NOT ", true);
				currentIndex = not.Length;

				return true;
			}
			else
			{
				currentIndex = 0;

				return false;
			}
		}

		/// <summary>
		/// 格式化单一条件项。
		/// </summary>
		private void FormatCriterion()
		{
			Int32 currentIndex;
			Boolean startsWithNotKeyword = FormatNot(out currentIndex);

			// 查找最左侧的括号
			Int32 leftOpenParenthesisIndex = FindLeftOpenParenthesisIndex(currentIndex);

			// 没有找到最左侧的括号，当前的条件表达是一个基本的表达式
			if (leftOpenParenthesisIndex == -1)
			{
				// 测试当前表达是否是一个 IN 表达式
				Int32 inListOpenParenthesisIndex = FindInListOpenParenthesisIndex(currentIndex);

				// 当前的表达是不是 IN 表达式
				if (inListOpenParenthesisIndex == -1)
				{
					AppendLiteral(FormatOperators(FormattingText.Substring(currentIndex)), !startsWithNotKeyword);
				}
				// 当前的表达式是 IN 表达式
				else
				{
					FormatInList(startsWithNotKeyword, currentIndex, inListOpenParenthesisIndex);
				}
			}
			// 找到了最左侧的括号，表明这是一个复合表达式
			else
			{
				FormatCompositeExpression(startsWithNotKeyword, leftOpenParenthesisIndex);
			}
		}

		/// <summary>
		/// 格式化 IN 表达式。
		/// </summary>
		/// <param name="startsWithNotKeyword">指示是否以 NOT 关键字开始。</param>
		/// <param name="currentIndex">当前索引。</param>
		/// <param name="inListOpenParenthesisIndex">IN 列表左括号索引。</param>
		private void FormatInList(Boolean startsWithNotKeyword, Int32 currentIndex, Int32 inListOpenParenthesisIndex)
		{
			// 输出 IN 列表之前的字符
			AppendLiteral(
					FormattingText.Substring(
							currentIndex,
							inListOpenParenthesisIndex - currentIndex
						).TrimStart(),
					!startsWithNotKeyword
				);

			// 处理 IN 列表
			SqlExpressionFormatter formatter = EvaluateParenthesesExpression(
					inListOpenParenthesisIndex,
					delegate(
							String sqlExpression,
							Int32 startIndex,
							Int32 length,
							String indent,
							Int32 level,
							SqlParenthesesExpressionFormattingInfo formattingInfo
						)
					{
						String expression = sqlExpression.Substring(startIndex, length);
						SqlStatementType statementType = SqlExpressionFormattingHelper.Recognize(expression);

						if (statementType != SqlStatementType.Unrecognisable)
						{
							formattingInfo.Formatter = Recognize((startIndex - this.StartIndex), length);
						}
						else
						{
							formattingInfo.Formatter = CreateSimpleCommaDelimitingListFormatter((startIndex - this.StartIndex), length);
						}

						formattingInfo.OuterOpenParenthesisStartsAtNewLine = false;
					},
					out inListOpenParenthesisIndex
				);

			if (formatter != null)
			{
				AppendComposite(formatter);
			}
		}

		/// <summary>
		/// 格式化复合表达式（即以括号括起的表达式）。
		/// </summary>
		/// <param name="startsWithNotKeyword">指示是否以 NOT 关键字开头。</param>
		/// <param name="leftOpenParenthesisIndex">左侧括号索引。</param>
		private void FormatCompositeExpression(Boolean startsWithNotKeyword, Int32 leftOpenParenthesisIndex)
		{
			SqlExpressionFormatter formatter = EvaluateParenthesesExpression(
					leftOpenParenthesisIndex,
					delegate(
							String sqlExpression,
							Int32 startIndex,
							Int32 length,
							String indent,
							Int32 level,
							SqlParenthesesExpressionFormattingInfo formattingInfo
						)
					{
						formattingInfo.OuterOpenParenthesisStartsAtNewLine = !startsWithNotKeyword;

						String expression = sqlExpression.Substring(startIndex, length);

						if (!StartsWithNotKeyword(expression))
						{
							Int32 index;

							// 测试表达式是否以括号开头
							index = FindLeftOpenParenthesisIndex(expression, 0);

							// 这是一个基本表达式
							if (index == -1)
							{
								// 测试是否为 IN 条件表达式
								index = FindInListOpenParenthesisIndex(expression, 0);

								// 不是 IN 条件表达式
								if (index == -1)
								{
									formattingInfo.Formatter = CreateLiteralFormatter((startIndex - this.StartIndex), length);

									return;
								}
							}
						}

						// 这是一个复合表达式。
						formattingInfo.Formatter = CreateCriteriaFormatter((startIndex - this.StartIndex), length);
					},
					out leftOpenParenthesisIndex
				);

			if (formatter != null)
			{
				AppendComposite(formatter);
			}
			else
			{
				AppendLiteral(FormattingText.Substring(leftOpenParenthesisIndex, (this.Length - leftOpenParenthesisIndex)).TrimStart());
			}
		}

		/// <summary>
		/// 查找选择值列表的左侧括号。
		/// </summary>
		/// <param name="startIndex">开始查找位置。</param>
		/// <returns>位置，如果没有找到，则返回 －1。</returns>
		private Int32 FindInListOpenParenthesisIndex(Int32 startIndex)
		{
			return FindInListOpenParenthesisIndex(FormattingText, startIndex);
		}

		/// <summary>
		/// 查找选择值列表的左侧括号。
		/// </summary>
		/// <param name="expression">要查找的表达式。</param>
		/// <param name="startIndex">开始查找的位置。</param>
		/// <returns>值列表左括号的位置，如果没有找到，则返回 -1;</returns>
		private static Int32 FindInListOpenParenthesisIndex(String expression, Int32 startIndex)
		{
			Regex exInList = new Regex(@"(?<=\bIN\b\s*)\(", DEFAULT_REGEX_OPTIONS);
			Match mInList = exInList.Match(expression, startIndex);

			if (mInList.Success)
			{
				return mInList.Index;
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// 查找最左侧的括号。
		/// </summary>
		/// <param name="startIndex">开始查找位置。</param>
		/// <returns>位置，如果没有找到，则返回 -1。</returns>
		private Int32 FindLeftOpenParenthesisIndex(Int32 startIndex)
		{
			return FindLeftOpenParenthesisIndex(FormattingText, startIndex);
		}

		/// <summary>
		/// 查找表达中的左括号。
		/// </summary>
		/// <param name="expression">表达式。</param>
		/// <param name="startIndex">开始索引。</param>
		/// <returns>结束索引。</returns>
		private static Int32 FindLeftOpenParenthesisIndex(String expression, Int32 startIndex)
		{
			Regex exOpenParenthesis = new Regex(@"(?<=^\s*)\(", DEFAULT_REGEX_OPTIONS);
			Match mOpenParenthesis = exOpenParenthesis.Match(expression, startIndex, (expression.Length - startIndex));

			if (mOpenParenthesis.Success)
			{
				return mOpenParenthesis.Index;
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// 判断表达式是否以 NOT 关键字开头。
		/// </summary>
		/// <param name="expression">条件表达式。</param>
		/// <returns>如果以 NOT 开头，则返回 true；否则返回 false。</returns>
		private static Boolean StartsWithNotKeyword(String expression)
		{
			return Regex.IsMatch(expression, @"^\s*NOT\b", DEFAULT_REGEX_OPTIONS);
		}

		#endregion
	}
}