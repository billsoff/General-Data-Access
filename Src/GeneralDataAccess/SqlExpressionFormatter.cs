#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlExpressionFormatter.cs
// 文件功能描述：用于格式化 SQL 指令。
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
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于格式化 SQL 指令。
	/// </summary>
	internal abstract class SqlExpressionFormatter
	{
		#region 静态成员

		/// <summary>
		/// 默认缩进，为 4 个空格。
		/// </summary>
		public const String DEFAULT_INDENT = "    ";

		/// <summary>
		/// 根据 SQL 指令的起始关键字创建合适的格式化器。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <returns>SQL 指令的格式化器。</returns>
		public static SqlExpressionFormatter Recognize(String sqlExpression)
		{
			return Recognize(sqlExpression, DEFAULT_INDENT, 0);
		}

		/// <summary>
		/// 根据 SQL 指令的起始关键字创建合适的格式化器。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <param name="indent">缩进。</param>
		/// <returns>格式化器。</returns>
		public static SqlExpressionFormatter Recognize(String sqlExpression, String indent)
		{
			return Recognize(sqlExpression, indent, 0);
		}

		/// <summary>
		/// 根据 SQL 指令的起始关键字创建合适的格式化器。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="level">级别。</param>
		/// <returns>格式化器。</returns>
		public static SqlExpressionFormatter Recognize(String sqlExpression, String indent, Int32 level)
		{
			if (String.IsNullOrEmpty(sqlExpression))
			{
				return new SqlEmptyExpressionFormatter();
			}

			return Recognize(sqlExpression, 0, sqlExpression.Length, indent, level);
		}

		#region 保护的字段

		/// <summary>
		/// 默认正则表达式选项。
		/// </summary>
		protected static readonly RegexOptions DEFAULT_REGEX_OPTIONS =
			RegexOptions.IgnoreCase
			| RegexOptions.IgnorePatternWhitespace
			| RegexOptions.ExplicitCapture;

		#endregion

		#region 保护的方法

		/// <summary>
		/// 供子类调用，开始索引和长度指示要解析的文本段。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <param name="startIndex">开始索引。</param>
		/// <param name="length">长度。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="level">当前级别，整合缩进为当前级别乘以级别值。</param>
		/// <returns>SQL 指令的格式化器。</returns>
		protected static SqlExpressionFormatter Recognize(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
		{
			SqlStatementType type = SqlExpressionFormattingHelper.Recognize(sqlExpression.Substring(startIndex, length));

			switch (type)
			{
				case SqlStatementType.Select:
					return new SqlSelectExpressionFormatter(sqlExpression, startIndex, length, indent, level);

				case SqlStatementType.Update:
					return new SqlUpdateExpressionFormatter(sqlExpression, startIndex, length, indent, level);

				case SqlStatementType.Insert:
					return new SqlInsertExpressionFormatter(sqlExpression, startIndex, length, indent, level);

				case SqlStatementType.Delete:
					return new SqlDeleteExpressionFormatter(sqlExpression, startIndex, length, indent, level);

				case SqlStatementType.Unrecognisable:
				default:
					return new SqlLiteralExpressionFormatter(sqlExpression, startIndex, length, indent, level);
			}
		}

		#endregion

		#endregion

		#region 私有字段

		private readonly String m_sqlExpression;
		private readonly String m_formattingText;
		private readonly String m_indent;
		private readonly Int32 m_startIndex;
		private readonly Int32 m_length;
		private readonly Int32 m_level;
		private readonly SqlParagraph m_totalIndentParagraph;

		private List<SqlParagraph> m_paragraphs;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，用于特殊的情形。
		/// </summary>
		protected SqlExpressionFormatter()
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
		protected SqlExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
		{
			m_sqlExpression = sqlExpression;
			m_startIndex = startIndex;
			m_length = length;
			m_indent = indent;
			m_level = level;

			if ((startIndex == 0) && (length == sqlExpression.Length))
			{
				m_formattingText = sqlExpression;
			}
			else
			{
				m_formattingText = sqlExpression.Substring(startIndex, length);
			}

			m_totalIndentParagraph = new SqlLiteralParagraph(DbEntityDebugger.GetTotalIndent(indent, level));
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取事件缩进段落。
		/// </summary>
		public SqlParagraph TotalIndentParagraph
		{
			get { return m_totalIndentParagraph; }
		}

		/// <summary>
		/// 获取 SQL 指令。
		/// </summary>
		public String SqlExpression
		{
			get { return m_sqlExpression; }
		}

		/// <summary>
		/// 要进行格式化的文本。
		/// </summary>
		public String FormattingText
		{
			get { return m_formattingText; }
		}

		/// <summary>
		/// 获取单位缩进。
		/// </summary>
		public String Indent
		{
			get { return m_indent; }
		}

		/// <summary>
		/// 获取开始索引。
		/// </summary>
		public Int32 StartIndex
		{
			get { return m_startIndex; }
		}

		/// <summary>
		/// 获取长度。
		/// </summary>
		public Int32 Length
		{
			get { return m_length; }
		}

		/// <summary>
		/// 获取当前级别。
		/// </summary>
		public Int32 Level
		{
			get { return m_level; }
		}

		#endregion

		#region 私有属性

		/// <summary>
		/// 判断是否包含段落。
		/// </summary>
		private Boolean HasParagrahs
		{
			get { return (m_paragraphs != null) && (m_paragraphs.Count != 0); }
		}

		/// <summary>
		/// 获取段落列表。
		/// </summary>
		private List<SqlParagraph> Paragraphs
		{
			get
			{
				if (m_paragraphs == null)
				{
					m_paragraphs = new List<SqlParagraph>();
				}

				return m_paragraphs;
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获得格式化好的 SQL 指令。
		/// </summary>
		/// <returns></returns>
		public sealed override String ToString()
		{
			#region 性能计数

#if DEBUG

			String id = "SqlExpressionFormatter.ToString " + "{" + Guid.NewGuid().ToString().ToUpper() + "}";

			Timing.Start("格式化 SQL 指令", id);

#endif

			#endregion

			String result;

			if (HasParagrahs)
			{
				Paragraphs.Clear();
			}

			Format();

			if (HasParagrahs)
			{
				if (Paragraphs.Count == 1)
				{
					result = Paragraphs[0].ToString();
				}
				else
				{
					StringBuilder buffer = new StringBuilder();

					foreach (SqlParagraph paragraph in Paragraphs)
					{
						buffer.Append(paragraph);
					}

					result = buffer.ToString();
				}
			}
			else
			{
				result = String.Empty;
			}

			#region 性能计数

#if DEBUG

			Timing.Stop(id);

#endif

			#endregion

			return result;
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 附加右括号。
		/// </summary>
		protected void AppendCloseParenthesis()
		{
			AppendCloseParenthesis(false);
		}

		/// <summary>
		/// 附加右括号，并指示是否先换行。
		/// </summary>
		/// <param name="newLineFeed">指示是否先换行。</param>
		protected void AppendCloseParenthesis(Boolean newLineFeed)
		{
			if (newLineFeed)
			{
				AppendNewLine();
				AppendIndent();
			}

			AppendParagraph(SqlParagraph.CloseParenthesis);
		}

		/// <summary>
		/// 附加逗号。
		/// </summary>
		protected void AppendComma()
		{
			AppendParagraph(SqlParagraph.Comma);
		}

		/// <summary>
		/// 附加复合段落。
		/// </summary>
		/// <param name="formatter">格式化器。</param>
		protected void AppendComposite(SqlExpressionFormatter formatter)
		{
			AppendParagraph(SqlParagraph.Create(formatter));
		}

		/// <summary>
		/// 附加缩进。
		/// </summary>
		protected void AppendIndent()
		{
			AppendParagraph(TotalIndentParagraph);
		}

		/// <summary>
		/// 附加不再需要格式化的文本段落。
		/// </summary>
		/// <param name="text">文本。</param>
		protected void AppendLiteral(String text)
		{
			AppendLiteral(text, false);
		}

		/// <summary>
		/// 附加不再需要格式化的文本。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <param name="atHead">如果为 true，且 IsBlock 为 true，则会首先换行、缩进。</param>
		protected void AppendLiteral(String text, Boolean atHead)
		{
			if (atHead && IsBlock)
			{
				AppendNewLine();
				AppendIndent();

				if (text != null)
				{
					text = text.TrimStart();
				}
			}

			AppendParagraph(SqlParagraph.Create(text));
		}

		/// <summary>
		/// 附加换行。
		/// </summary>
		protected void AppendNewLine()
		{
			AppendParagraph(SqlParagraph.NewLine);
		}

		/// <summary>
		/// 附加左括号。
		/// </summary>
		protected void AppendOpenParenthesis()
		{
			AppendOpenParenthesis(false);
		}

		/// <summary>
		/// 附加左括号。
		/// </summary>
		/// <param name="atHead">是否是否处于头部，如果处于头部且 IsBlock 为 true，则会先换行、缩进。</param>
		protected void AppendOpenParenthesis(Boolean atHead)
		{
			if (atHead && IsBlock)
			{
				AppendNewLine();
				AppendIndent();
			}

			AppendParagraph(SqlParagraph.OpenParenthesis);
		}

		/// <summary>
		/// 附加段落。
		/// </summary>
		/// <param name="paragraph">段落。</param>
		protected void AppendParagraph(SqlParagraph paragraph)
		{
			Paragraphs.Add(paragraph);
		}

		/// <summary>
		/// 附加空格。
		/// </summary>
		protected void AppendSpace()
		{
			Paragraphs.Add(SqlParagraph.Space);
		}

		/// <summary>
		/// 创建逐字输出格式化器。
		/// </summary>
		/// <param name="startIndex">起始索引。</param>
		/// <param name="length">长度。</param>
		/// <returns>格式化器。</returns>
		protected SqlExpressionFormatter CreateLiteralFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlLiteralExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		/// <summary>
		/// 创建表连接表达式格式化器。
		/// </summary>
		/// <param name="startIndex">在格式化的文本中的开始索引。</param>
		/// <param name="length">表连接表达式的长度。</param>
		/// <returns>格式化器。</returns>
		protected SqlExpressionFormatter CreateFromListFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlFromListExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		/// <summary>
		/// 创建过滤器表达式格式化器。
		/// </summary>
		/// <param name="startIndex">在格式化的文本中的开始索引。</param>
		/// <param name="length">过滤器表达式的长度。</param>
		/// <returns>格式化器。</returns>
		protected SqlExpressionFormatter CreateCriteriaFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlCriteriaExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// 创建过滤器表达式格式化器。
		/// </summary>
		/// <param name="startIndex">在格式化的文本中的开始索引。</param>
		/// <param name="length">过滤器表达式的长度。</param>
		/// <returns>格式化器。</returns>
		/// <param name="statementLevel">请参见 SqlSortExpressionFormatter 中的说明。</param>
		protected SqlExpressionFormatter CreateCriteriaFormatter(Int32 startIndex, Int32 length, Boolean statementLevel)
		{
			return new SqlCriteriaExpressionFormatter(
					SqlExpression,
					(this.StartIndex + startIndex),
					length,
					Indent,
					statementLevel ? Level : (Level + 1),
					statementLevel
				);
		}

		/// <summary>
		/// 创建逗号分割列表格式化器，此格式化器只是简单地罗列各项。
		/// </summary>
		/// <param name="startIndex">开始索引。</param>
		/// <param name="length">长度。</param>
		/// <returns>格式化器。</returns>
		protected SqlExpressionFormatter CreateSimpleCommaDelimitingListFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlSimpleCommaDelimitingListExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// 格式化操作符。
		/// </summary>
		/// <param name="expression">表达式。</param>
		/// <returns>格式化后的表达式。</returns>
		protected static String FormatOperators(String expression)
		{
			Regex exOpts = new Regex(@"<>|<=|>=|<|>|=", DEFAULT_REGEX_OPTIONS);
			return exOpts.Replace(expression, " $& ");
		}

		/// <summary>
		/// 供子类调用，开始索引和长度指示要解析的文本段。
		/// </summary>
		/// <param name="startIndex">在要格式化的文本中的开始索引。</param>
		/// <param name="length">长度。</param>
		/// <returns>SQL 指令的格式化器。</returns>
		protected SqlExpressionFormatter Recognize(Int32 startIndex, Int32 length)
		{
			return Recognize(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// 尝试去除最外层的括号（FieldItem 的 IsNull 如果为 false，则表示输入项包含括号，否则不包含括号。）
		/// </summary>
		/// <param name="startIndex">起始索引。</param>
		/// <param name="length">长度。</param>
		/// <returns>FieldItem 实例，如果输入项为复杂项，则 IsNull 属性为 false，否则为 true。</returns>
		protected FieldItem TryStripParentheses(Int32 startIndex, Int32 length)
		{
			String input = FormattingText.Substring(startIndex, length);
			Int32 openParenthesisIndex = input.IndexOf(SqlExpressionFormattingHelper.OPEN_PARENTHESIS);

			if ((openParenthesisIndex == -1) || (openParenthesisIndex == (input.Length - 1)))
			{
				return new FieldItem();
			}

			Int32 closeParenthesisIndex = input.LastIndexOf(SqlExpressionFormattingHelper.CLOSE_PARENTHESIS);

			if (closeParenthesisIndex == -1)
			{
				closeParenthesisIndex = input.Length;
			}

			Int32 index = startIndex + openParenthesisIndex + 1;
			String text = input.Substring(openParenthesisIndex + 1, (closeParenthesisIndex - openParenthesisIndex - 1));

			return new FieldItem(index, text);
		}

		/// <summary>
		/// 处理括号表达式。
		/// </summary>
		/// <param name="startIndex">起始索引。</param>
		/// <param name="evaluator">处理委托，用于指示如何格式化括号内部的表达式。</param>
		/// <param name="openParenthesisIndex">左括号的实际索引。</param>
		/// <returns>括号表达式索引。</returns>
		protected SqlExpressionFormatter EvaluateParenthesesExpression(Int32 startIndex, SqlParenthesesExpressionEvaluator evaluator, out Int32 openParenthesisIndex)
		{
			openParenthesisIndex = FormattingText.IndexOf(SqlExpressionFormattingHelper.OPEN_PARENTHESIS, startIndex);

			if ((openParenthesisIndex == -1) || (openParenthesisIndex == (this.Length - 1)))
			{
				return null;
			}

			Int32 closeParenthesisIndex = FindMatchCloseParenthesisIndex(openParenthesisIndex);

			if (closeParenthesisIndex == -1)
			{
				openParenthesisIndex = -1;

				return null;
			}

			Int32 length = (closeParenthesisIndex + 1) - openParenthesisIndex;

			return new SqlParenthesesExpressionFormatter(SqlExpression, (this.StartIndex + openParenthesisIndex), length, Indent, Level, evaluator);
		}

		/// <summary>
		/// 查找匹配的右括号的位置，如果没有找到，则返回 -1。
		/// </summary>
		/// <param name="openParenthesisIndex">左括号的索引。</param>
		/// <returns>匹配的右括号的索引。</returns>
		private Int32 FindMatchCloseParenthesisIndex(Int32 openParenthesisIndex)
		{
			// 未关闭的左括号的数量
			Int32 count = 1;
			Regex ex = new Regex("[()]");
			Match m = ex.Match(FormattingText, (openParenthesisIndex + 1));

			while (m.Success)
			{
				if (m.Value.Equals(SqlExpressionFormattingHelper.CLOSE_PARENTHESIS))
				{
					if (count == 1)
					{
						return m.Index;
					}
					else
					{
						count--;
					}
				}
				else
				{
					count++;
				}

				m = m.NextMatch();
			}

			return -1;
		}

		#region 查找分界符

		/// <summary>
		/// 查找合法的分割匹配，并保证使其不处于括号中。
		/// </summary>
		/// <param name="pattern">分割模式。</param>
		/// <returns>匹配，如果没有找到，则返回 null。</returns>
		protected Match FindValidDelimiter(String pattern)
		{
			return FindValidDelimiter(pattern, false);
		}

		/// <summary>
		/// 查找合法的分割匹配，如果不只是检查头部，还要保证其不处于括号中。
		/// </summary>
		/// <param name="pattern">分割模式。</param>
		/// <param name="atHead">是否是否只在头部匹配。</param>
		/// <returns>匹配，如果没有找到，则返回 null。</returns>
		protected Match FindValidDelimiter(String pattern, Boolean atHead)
		{
			Match[] allDelimiters = FindValidDelimiters(pattern, atHead);

			if (allDelimiters.Length != 0)
			{
				return allDelimiters[0];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 查找所有合法的的侵害匹配，只有那些不处于括号中的匹配才是合法的。
		/// </summary>
		/// <param name="pattern">匹配模式。</param>
		/// <returns>所有合法的分割匹配。</returns>
		protected Match[] FindValidDelimiters(String pattern)
		{
			return FindValidDelimiters(pattern, false);
		}

		/// <summary>
		/// 查找所有合法的分割匹配，如果不是仅查找头部，则该分割位置必须不处于括号中。
		/// </summary>
		/// <param name="pattern">分割模式。</param>
		/// <param name="atHead">指示是否处于头部。</param>
		/// <returns>所有合法的匹配。</returns>
		protected Match[] FindValidDelimiters(String pattern, Boolean atHead)
		{
			Regex ex = new Regex(pattern, DEFAULT_REGEX_OPTIONS);

			List<Match> allValids = new List<Match>();

			foreach (Match m in ex.Matches(FormattingText))
			{
				if (atHead || !IsBetweenParentheses(m.Index + m.Length))
				{
					allValids.Add(m);
				}
			}

			return allValids.ToArray();
		}

		/// <summary>
		/// 查找所有合法的分割匹配，如果不是仅查找头部，则该分割位置必须不处于括号中。
		/// </summary>
		/// <param name="expression">要判断的表达式。</param>
		/// <param name="pattern">分割模式。</param>
		/// <returns>所有合法的匹配。</returns>
		protected static Match FindValidDelimiter(String expression, String pattern)
		{
			return FindValidDelimiter(expression, pattern, false);
		}

		/// <summary>
		/// 查找所有合法的分割匹配，如果不是仅查找头部，则该分割位置必须不处于括号中。
		/// </summary>
		/// <param name="expression">要判断的表达式。</param>
		/// <param name="pattern">分割模式。</param>
		/// <param name="atHead">指示是否处于头部。</param>
		/// <returns>所有合法的匹配。</returns>
		protected static Match FindValidDelimiter(String expression, String pattern, Boolean atHead)
		{
			Match[] allValidDelimiters = FindValidDelimiters(expression, pattern, atHead);

			if (allValidDelimiters.Length > 0)
			{
				return allValidDelimiters[0];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 查找所有合法的分割匹配，如果不是仅查找头部，则该分割位置必须不处于括号中。
		/// </summary>
		/// <param name="expression">要判断的表达式。</param>
		/// <param name="pattern">分割模式。</param>
		/// <returns>所有合法的匹配。</returns>
		protected static Match[] FindValidDelimiters(String expression, String pattern)
		{
			return FindValidDelimiters(expression, pattern, false);
		}

		/// <summary>
		/// 查找所有合法的分割匹配，如果不是仅查找头部，则该分割位置必须不处于括号中。
		/// </summary>
		/// <param name="expression">要判断的表达式。</param>
		/// <param name="pattern">分割模式。</param>
		/// <param name="atHead">指示是否处于头部。</param>
		/// <returns>所有合法的匹配。</returns>
		protected static Match[] FindValidDelimiters(String expression, String pattern, Boolean atHead)
		{
			Regex ex = new Regex(pattern, DEFAULT_REGEX_OPTIONS);

			List<Match> allValids = new List<Match>();

			foreach (Match m in ex.Matches(expression))
			{
				if (atHead || !IsBetweenParentheses(expression, m.Index + m.Length))
				{
					allValids.Add(m);
				}
			}

			return allValids.ToArray();
		}

		/// <summary>
		/// 获取字段列表中的所有字段项。
		/// </summary>
		/// <param name="pattern">分割模式。</param>
		/// <returns>字段项列表。</returns>
		protected FieldItem[] GetAllFieldItems(String pattern)
		{
			Match[] allCommas = FindValidDelimiters(pattern);
			List<FieldItem> allFields = new List<FieldItem>();

			Int32 currentIndex = 0;

			for (Int32 i = 0; i < allCommas.Length; i++)
			{
				Match m = allCommas[i];
				Int32 position = m.Index;

				Int32 length = position - currentIndex;
				String text = FormattingText.Substring(currentIndex, length);

				allFields.Add(new FieldItem(currentIndex, text));
				currentIndex = position + m.Length;
			}

			if (currentIndex < this.Length)
			{
				allFields.Add(new FieldItem(currentIndex, FormattingText.Substring(currentIndex)));
			}

			return allFields.ToArray();
		}

		/// <summary>
		/// 判断给定的位置是否处于括号中。
		/// </summary>
		/// <param name="startIndex">要判断的位置。</param>
		/// <returns>如果该位置处于括号中，则返回 true；否则返回 false。</returns>
		protected Boolean IsBetweenParentheses(Int32 startIndex)
		{
			if (startIndex >= FormattingText.Length)
			{
				return false;
			}

			String remains = FormattingText.Substring(startIndex);
			Int32 count = 0;
			Match m = Regex.Match(remains, "[()]");

			while (m.Success)
			{
				if (m.Value.Equals(SqlExpressionFormattingHelper.CLOSE_PARENTHESIS, StringComparison.Ordinal))
				{
					if (count == 0)
					{
						return true;
					}
					else
					{
						count--;
					}
				}
				else
				{
					count++;
				}

				m = m.NextMatch();
			}

			return false;
		}

		/// <summary>
		/// 判断给定的位置是否处于括号中。
		/// </summary>
		/// <param name="expression">要判断的表达式。</param>
		/// <param name="position">要判断的位置。</param>
		/// <returns>如果该位置处于括号中，则返回 true；否则返回 false。</returns>
		protected static Boolean IsBetweenParentheses(String expression, Int32 position)
		{
			if (position >= expression.Length)
			{
				return false;
			}

			String remains = expression.Substring(position);
			Int32 count = 0;
			Match m = Regex.Match(remains, "[()]");

			while (m.Success)
			{
				if (m.Value.Equals(SqlExpressionFormattingHelper.CLOSE_PARENTHESIS, StringComparison.Ordinal))
				{
					if (count == 0)
					{
						return true;
					}
					else
					{
						count--;
					}
				}
				else
				{
					count++;
				}

				m = m.NextMatch();
			}

			return false;
		}

		/// <summary>
		/// 判断有效的表达式是否处于括号中
		/// </summary>
		/// <returns></returns>
		protected Boolean IsBetweenParentheses()
		{
			Regex openParenthesis = new Regex(@"^\s*\(");

			return openParenthesis.IsMatch(FormattingText);
		}

		/// <summary>
		/// 测试是否与所给的模式相匹配，测试选项是 IgnoreCase 和 IgnorePatternWhitespace。
		/// </summary>
		/// <param name="pattern">匹配模式。</param>
		/// <returns>如果相匹配，则返回 true；否则返回 false。</returns>
		protected Boolean IsMatch(String pattern)
		{
			return IsMatch(pattern, 0, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}

		/// <summary>
		/// 测试 offset 后的子串是否与所给的模式相匹配，测试选项是 IgnoreCase 和 IgnorePatternWhitespace。
		/// </summary>
		/// <param name="pattern">匹配模式。</param>
		/// <param name="offset">子串起始索引。</param>
		/// <returns>如果相匹配，则返回 true；否则返回 false。</returns>
		protected Boolean IsMatch(String pattern, Int32 offset)
		{
			return IsMatch(pattern, 0, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}

		/// <summary>
		/// 测试 offset 后的子串是否与所给的模式相匹配。
		/// </summary>
		/// <param name="pattern">匹配模式。</param>
		/// <param name="options">测试选项。</param>
		/// <returns>如果相匹配，则返回 true；否则返回 false。</returns>
		protected Boolean IsMatch(String pattern, RegexOptions options)
		{
			return IsMatch(pattern, 0, options);
		}

		/// <summary>
		/// 测试是否与给定的模式相匹配，注意，是对 offset 之后的子串进行匹配测试。
		/// </summary>
		/// <param name="pattern">匹配模式。</param>
		/// <param name="offset">要进行匹配的子串的起始索引。</param>
		/// <param name="options"></param>
		/// <returns>测试选项。</returns>
		protected Boolean IsMatch(String pattern, Int32 offset, RegexOptions options)
		{
			String input;

			if (offset == 0)
			{
				input = FormattingText;
			}
			else
			{
				input = FormattingText.Substring(offset);
			}

			Regex engine = new Regex(pattern, options);

			return engine.IsMatch(input);
		}

		#endregion

		#endregion

		#region 抽象成员

		#region 公开的属性

		/// <summary>
		/// 获取一个值，该值指示是否为块输出。
		/// </summary>
		public abstract Boolean IsBlock { get; }

		#endregion

		#region 保护的方法

		/// <summary>
		/// 获得格式化文本。
		/// </summary>
		/// <returns>格式化文本。</returns>
		protected abstract void Format();

		#endregion

		#endregion


		#region 关键字匹配结构

		/// <summary>
		/// 关键字匹配。
		/// </summary>
		protected struct KeywordMatch
		{
			#region 私有字段

			private readonly Int32 m_index;
			private readonly Int32 m_length;
			private readonly String m_keyword;

			#endregion

			#region 构造函数

			/// <summary>
			/// 构造函数，设置起始索引等属性。
			/// </summary>
			/// <param name="index">起始索引。</param>
			/// <param name="length">长度。</param>
			/// <param name="keyword">关键字。</param>
			public KeywordMatch(Int32 index, Int32 length, String keyword)
			{
				m_index = index;
				m_length = length;
				m_keyword = keyword;
			}

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取起始索引。
			/// </summary>
			public Int32 Index
			{
				get { return m_index; }
			}

			/// <summary>
			/// 获取长度。
			/// </summary>
			public Int32 Length
			{
				get { return m_length; }
			}

			/// <summary>
			/// 获取关键字。
			/// </summary>
			public String Keyword
			{
				get { return m_keyword; }
			}

			/// <summary>
			/// 指示是否匹配成功。
			/// </summary>
			public Boolean Success
			{
				get { return (m_keyword != null); }
			}

			#endregion
		}

		#endregion


		#region 字段项结构

		/// <summary>
		/// 字段项结构。
		/// </summary>
		protected struct FieldItem
		{
			#region 构造函数

			private readonly Int32 m_index;
			private readonly String m_text;

			#endregion

			#region 构造函数

			/// <summary>
			/// 构造函数，设置字段项的起始索引和文本。
			/// </summary>
			/// <param name="index">起始索引。</param>
			/// <param name="text">文本。</param>
			public FieldItem(Int32 index, String text)
			{
				m_index = index;
				m_text = text;
			}

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取项起始索引。
			/// </summary>
			public Int32 Index
			{
				get { return m_index; }
			}

			/// <summary>
			/// 指示字段项是否无效。
			/// </summary>
			public Boolean IsNull
			{
				get { return (m_text == null); }
			}

			/// <summary>
			/// 获取字段项文本长度。
			/// </summary>
			public Int32 Length
			{
				get { return Text.Length; }
			}

			/// <summary>
			/// 获取项文本。
			/// </summary>
			public String Text
			{
				get { return m_text ?? String.Empty; }
			}

			#endregion

			#region 公共方法

			/// <summary>
			/// 显示字段项。
			/// </summary>
			/// <returns></returns>
			public override String ToString()
			{
				return String.Format("{0, 4}{1}", Index, Text);
			}

			#endregion
		}

		#endregion
	}
}