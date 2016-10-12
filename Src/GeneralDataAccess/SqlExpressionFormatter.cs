#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlExpressionFormatter.cs
// �ļ��������������ڸ�ʽ�� SQL ָ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110713
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ڸ�ʽ�� SQL ָ�
	/// </summary>
	internal abstract class SqlExpressionFormatter
	{
		#region ��̬��Ա

		/// <summary>
		/// Ĭ��������Ϊ 4 ���ո�
		/// </summary>
		public const String DEFAULT_INDENT = "    ";

		/// <summary>
		/// ���� SQL ָ�����ʼ�ؼ��ִ������ʵĸ�ʽ������
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <returns>SQL ָ��ĸ�ʽ������</returns>
		public static SqlExpressionFormatter Recognize(String sqlExpression)
		{
			return Recognize(sqlExpression, DEFAULT_INDENT, 0);
		}

		/// <summary>
		/// ���� SQL ָ�����ʼ�ؼ��ִ������ʵĸ�ʽ������
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="indent">������</param>
		/// <returns>��ʽ������</returns>
		public static SqlExpressionFormatter Recognize(String sqlExpression, String indent)
		{
			return Recognize(sqlExpression, indent, 0);
		}

		/// <summary>
		/// ���� SQL ָ�����ʼ�ؼ��ִ������ʵĸ�ʽ������
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		/// <returns>��ʽ������</returns>
		public static SqlExpressionFormatter Recognize(String sqlExpression, String indent, Int32 level)
		{
			if (String.IsNullOrEmpty(sqlExpression))
			{
				return new SqlEmptyExpressionFormatter();
			}

			return Recognize(sqlExpression, 0, sqlExpression.Length, indent, level);
		}

		#region �������ֶ�

		/// <summary>
		/// Ĭ��������ʽѡ�
		/// </summary>
		protected static readonly RegexOptions DEFAULT_REGEX_OPTIONS =
			RegexOptions.IgnoreCase
			| RegexOptions.IgnorePatternWhitespace
			| RegexOptions.ExplicitCapture;

		#endregion

		#region �����ķ���

		/// <summary>
		/// ��������ã���ʼ�����ͳ���ָʾҪ�������ı��Ρ�
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">��ǰ������������Ϊ��ǰ������Լ���ֵ��</param>
		/// <returns>SQL ָ��ĸ�ʽ������</returns>
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

		#region ˽���ֶ�

		private readonly String m_sqlExpression;
		private readonly String m_formattingText;
		private readonly String m_indent;
		private readonly Int32 m_startIndex;
		private readonly Int32 m_length;
		private readonly Int32 m_level;
		private readonly SqlParagraph m_totalIndentParagraph;

		private List<SqlParagraph> m_paragraphs;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����������������Ρ�
		/// </summary>
		protected SqlExpressionFormatter()
		{
		}

		/// <summary>
		/// ���캯��������Ҫ��ʽ���� SQL ָ��ȡ�
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
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

		#region ��������

		/// <summary>
		/// ��ȡ�¼��������䡣
		/// </summary>
		public SqlParagraph TotalIndentParagraph
		{
			get { return m_totalIndentParagraph; }
		}

		/// <summary>
		/// ��ȡ SQL ָ�
		/// </summary>
		public String SqlExpression
		{
			get { return m_sqlExpression; }
		}

		/// <summary>
		/// Ҫ���и�ʽ�����ı���
		/// </summary>
		public String FormattingText
		{
			get { return m_formattingText; }
		}

		/// <summary>
		/// ��ȡ��λ������
		/// </summary>
		public String Indent
		{
			get { return m_indent; }
		}

		/// <summary>
		/// ��ȡ��ʼ������
		/// </summary>
		public Int32 StartIndex
		{
			get { return m_startIndex; }
		}

		/// <summary>
		/// ��ȡ���ȡ�
		/// </summary>
		public Int32 Length
		{
			get { return m_length; }
		}

		/// <summary>
		/// ��ȡ��ǰ����
		/// </summary>
		public Int32 Level
		{
			get { return m_level; }
		}

		#endregion

		#region ˽������

		/// <summary>
		/// �ж��Ƿ�������䡣
		/// </summary>
		private Boolean HasParagrahs
		{
			get { return (m_paragraphs != null) && (m_paragraphs.Count != 0); }
		}

		/// <summary>
		/// ��ȡ�����б�
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

		#region ��������

		/// <summary>
		/// ��ø�ʽ���õ� SQL ָ�
		/// </summary>
		/// <returns></returns>
		public sealed override String ToString()
		{
			#region ���ܼ���

#if DEBUG

			String id = "SqlExpressionFormatter.ToString " + "{" + Guid.NewGuid().ToString().ToUpper() + "}";

			Timing.Start("��ʽ�� SQL ָ��", id);

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

			#region ���ܼ���

#if DEBUG

			Timing.Stop(id);

#endif

			#endregion

			return result;
		}

		#endregion

		#region �����ķ���

		/// <summary>
		/// ���������š�
		/// </summary>
		protected void AppendCloseParenthesis()
		{
			AppendCloseParenthesis(false);
		}

		/// <summary>
		/// ���������ţ���ָʾ�Ƿ��Ȼ��С�
		/// </summary>
		/// <param name="newLineFeed">ָʾ�Ƿ��Ȼ��С�</param>
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
		/// ���Ӷ��š�
		/// </summary>
		protected void AppendComma()
		{
			AppendParagraph(SqlParagraph.Comma);
		}

		/// <summary>
		/// ���Ӹ��϶��䡣
		/// </summary>
		/// <param name="formatter">��ʽ������</param>
		protected void AppendComposite(SqlExpressionFormatter formatter)
		{
			AppendParagraph(SqlParagraph.Create(formatter));
		}

		/// <summary>
		/// ����������
		/// </summary>
		protected void AppendIndent()
		{
			AppendParagraph(TotalIndentParagraph);
		}

		/// <summary>
		/// ���Ӳ�����Ҫ��ʽ�����ı����䡣
		/// </summary>
		/// <param name="text">�ı���</param>
		protected void AppendLiteral(String text)
		{
			AppendLiteral(text, false);
		}

		/// <summary>
		/// ���Ӳ�����Ҫ��ʽ�����ı���
		/// </summary>
		/// <param name="text">�ı���</param>
		/// <param name="atHead">���Ϊ true���� IsBlock Ϊ true��������Ȼ��С�������</param>
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
		/// ���ӻ��С�
		/// </summary>
		protected void AppendNewLine()
		{
			AppendParagraph(SqlParagraph.NewLine);
		}

		/// <summary>
		/// ���������š�
		/// </summary>
		protected void AppendOpenParenthesis()
		{
			AppendOpenParenthesis(false);
		}

		/// <summary>
		/// ���������š�
		/// </summary>
		/// <param name="atHead">�Ƿ��Ƿ���ͷ�����������ͷ���� IsBlock Ϊ true������Ȼ��С�������</param>
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
		/// ���Ӷ��䡣
		/// </summary>
		/// <param name="paragraph">���䡣</param>
		protected void AppendParagraph(SqlParagraph paragraph)
		{
			Paragraphs.Add(paragraph);
		}

		/// <summary>
		/// ���ӿո�
		/// </summary>
		protected void AppendSpace()
		{
			Paragraphs.Add(SqlParagraph.Space);
		}

		/// <summary>
		/// �������������ʽ������
		/// </summary>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>��ʽ������</returns>
		protected SqlExpressionFormatter CreateLiteralFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlLiteralExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		/// <summary>
		/// ���������ӱ��ʽ��ʽ������
		/// </summary>
		/// <param name="startIndex">�ڸ�ʽ�����ı��еĿ�ʼ������</param>
		/// <param name="length">�����ӱ��ʽ�ĳ��ȡ�</param>
		/// <returns>��ʽ������</returns>
		protected SqlExpressionFormatter CreateFromListFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlFromListExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		/// <summary>
		/// �������������ʽ��ʽ������
		/// </summary>
		/// <param name="startIndex">�ڸ�ʽ�����ı��еĿ�ʼ������</param>
		/// <param name="length">���������ʽ�ĳ��ȡ�</param>
		/// <returns>��ʽ������</returns>
		protected SqlExpressionFormatter CreateCriteriaFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlCriteriaExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// �������������ʽ��ʽ������
		/// </summary>
		/// <param name="startIndex">�ڸ�ʽ�����ı��еĿ�ʼ������</param>
		/// <param name="length">���������ʽ�ĳ��ȡ�</param>
		/// <returns>��ʽ������</returns>
		/// <param name="statementLevel">��μ� SqlSortExpressionFormatter �е�˵����</param>
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
		/// �������ŷָ��б��ʽ�������˸�ʽ����ֻ�Ǽ򵥵����и��
		/// </summary>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>��ʽ������</returns>
		protected SqlExpressionFormatter CreateSimpleCommaDelimitingListFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlSimpleCommaDelimitingListExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// ��ʽ����������
		/// </summary>
		/// <param name="expression">���ʽ��</param>
		/// <returns>��ʽ����ı��ʽ��</returns>
		protected static String FormatOperators(String expression)
		{
			Regex exOpts = new Regex(@"<>|<=|>=|<|>|=", DEFAULT_REGEX_OPTIONS);
			return exOpts.Replace(expression, " $& ");
		}

		/// <summary>
		/// ��������ã���ʼ�����ͳ���ָʾҪ�������ı��Ρ�
		/// </summary>
		/// <param name="startIndex">��Ҫ��ʽ�����ı��еĿ�ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>SQL ָ��ĸ�ʽ������</returns>
		protected SqlExpressionFormatter Recognize(Int32 startIndex, Int32 length)
		{
			return Recognize(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// ����ȥ�����������ţ�FieldItem �� IsNull ���Ϊ false�����ʾ������������ţ����򲻰������š���
		/// </summary>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>FieldItem ʵ�������������Ϊ������� IsNull ����Ϊ false������Ϊ true��</returns>
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
		/// �������ű��ʽ��
		/// </summary>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="evaluator">����ί�У�����ָʾ��θ�ʽ�������ڲ��ı��ʽ��</param>
		/// <param name="openParenthesisIndex">�����ŵ�ʵ��������</param>
		/// <returns>���ű��ʽ������</returns>
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
		/// ����ƥ��������ŵ�λ�ã����û���ҵ����򷵻� -1��
		/// </summary>
		/// <param name="openParenthesisIndex">�����ŵ�������</param>
		/// <returns>ƥ��������ŵ�������</returns>
		private Int32 FindMatchCloseParenthesisIndex(Int32 openParenthesisIndex)
		{
			// δ�رյ������ŵ�����
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

		#region ���ҷֽ��

		/// <summary>
		/// ���ҺϷ��ķָ�ƥ�䣬����֤ʹ�䲻���������С�
		/// </summary>
		/// <param name="pattern">�ָ�ģʽ��</param>
		/// <returns>ƥ�䣬���û���ҵ����򷵻� null��</returns>
		protected Match FindValidDelimiter(String pattern)
		{
			return FindValidDelimiter(pattern, false);
		}

		/// <summary>
		/// ���ҺϷ��ķָ�ƥ�䣬�����ֻ�Ǽ��ͷ������Ҫ��֤�䲻���������С�
		/// </summary>
		/// <param name="pattern">�ָ�ģʽ��</param>
		/// <param name="atHead">�Ƿ��Ƿ�ֻ��ͷ��ƥ�䡣</param>
		/// <returns>ƥ�䣬���û���ҵ����򷵻� null��</returns>
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
		/// �������кϷ��ĵ��ֺ�ƥ�䣬ֻ����Щ�����������е�ƥ����ǺϷ��ġ�
		/// </summary>
		/// <param name="pattern">ƥ��ģʽ��</param>
		/// <returns>���кϷ��ķָ�ƥ�䡣</returns>
		protected Match[] FindValidDelimiters(String pattern)
		{
			return FindValidDelimiters(pattern, false);
		}

		/// <summary>
		/// �������кϷ��ķָ�ƥ�䣬������ǽ�����ͷ������÷ָ�λ�ñ��벻���������С�
		/// </summary>
		/// <param name="pattern">�ָ�ģʽ��</param>
		/// <param name="atHead">ָʾ�Ƿ���ͷ����</param>
		/// <returns>���кϷ���ƥ�䡣</returns>
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
		/// �������кϷ��ķָ�ƥ�䣬������ǽ�����ͷ������÷ָ�λ�ñ��벻���������С�
		/// </summary>
		/// <param name="expression">Ҫ�жϵı��ʽ��</param>
		/// <param name="pattern">�ָ�ģʽ��</param>
		/// <returns>���кϷ���ƥ�䡣</returns>
		protected static Match FindValidDelimiter(String expression, String pattern)
		{
			return FindValidDelimiter(expression, pattern, false);
		}

		/// <summary>
		/// �������кϷ��ķָ�ƥ�䣬������ǽ�����ͷ������÷ָ�λ�ñ��벻���������С�
		/// </summary>
		/// <param name="expression">Ҫ�жϵı��ʽ��</param>
		/// <param name="pattern">�ָ�ģʽ��</param>
		/// <param name="atHead">ָʾ�Ƿ���ͷ����</param>
		/// <returns>���кϷ���ƥ�䡣</returns>
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
		/// �������кϷ��ķָ�ƥ�䣬������ǽ�����ͷ������÷ָ�λ�ñ��벻���������С�
		/// </summary>
		/// <param name="expression">Ҫ�жϵı��ʽ��</param>
		/// <param name="pattern">�ָ�ģʽ��</param>
		/// <returns>���кϷ���ƥ�䡣</returns>
		protected static Match[] FindValidDelimiters(String expression, String pattern)
		{
			return FindValidDelimiters(expression, pattern, false);
		}

		/// <summary>
		/// �������кϷ��ķָ�ƥ�䣬������ǽ�����ͷ������÷ָ�λ�ñ��벻���������С�
		/// </summary>
		/// <param name="expression">Ҫ�жϵı��ʽ��</param>
		/// <param name="pattern">�ָ�ģʽ��</param>
		/// <param name="atHead">ָʾ�Ƿ���ͷ����</param>
		/// <returns>���кϷ���ƥ�䡣</returns>
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
		/// ��ȡ�ֶ��б��е������ֶ��
		/// </summary>
		/// <param name="pattern">�ָ�ģʽ��</param>
		/// <returns>�ֶ����б�</returns>
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
		/// �жϸ�����λ���Ƿ��������С�
		/// </summary>
		/// <param name="startIndex">Ҫ�жϵ�λ�á�</param>
		/// <returns>�����λ�ô��������У��򷵻� true�����򷵻� false��</returns>
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
		/// �жϸ�����λ���Ƿ��������С�
		/// </summary>
		/// <param name="expression">Ҫ�жϵı��ʽ��</param>
		/// <param name="position">Ҫ�жϵ�λ�á�</param>
		/// <returns>�����λ�ô��������У��򷵻� true�����򷵻� false��</returns>
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
		/// �ж���Ч�ı��ʽ�Ƿ���������
		/// </summary>
		/// <returns></returns>
		protected Boolean IsBetweenParentheses()
		{
			Regex openParenthesis = new Regex(@"^\s*\(");

			return openParenthesis.IsMatch(FormattingText);
		}

		/// <summary>
		/// �����Ƿ���������ģʽ��ƥ�䣬����ѡ���� IgnoreCase �� IgnorePatternWhitespace��
		/// </summary>
		/// <param name="pattern">ƥ��ģʽ��</param>
		/// <returns>�����ƥ�䣬�򷵻� true�����򷵻� false��</returns>
		protected Boolean IsMatch(String pattern)
		{
			return IsMatch(pattern, 0, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}

		/// <summary>
		/// ���� offset ����Ӵ��Ƿ���������ģʽ��ƥ�䣬����ѡ���� IgnoreCase �� IgnorePatternWhitespace��
		/// </summary>
		/// <param name="pattern">ƥ��ģʽ��</param>
		/// <param name="offset">�Ӵ���ʼ������</param>
		/// <returns>�����ƥ�䣬�򷵻� true�����򷵻� false��</returns>
		protected Boolean IsMatch(String pattern, Int32 offset)
		{
			return IsMatch(pattern, 0, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}

		/// <summary>
		/// ���� offset ����Ӵ��Ƿ���������ģʽ��ƥ�䡣
		/// </summary>
		/// <param name="pattern">ƥ��ģʽ��</param>
		/// <param name="options">����ѡ�</param>
		/// <returns>�����ƥ�䣬�򷵻� true�����򷵻� false��</returns>
		protected Boolean IsMatch(String pattern, RegexOptions options)
		{
			return IsMatch(pattern, 0, options);
		}

		/// <summary>
		/// �����Ƿ��������ģʽ��ƥ�䣬ע�⣬�Ƕ� offset ֮����Ӵ�����ƥ����ԡ�
		/// </summary>
		/// <param name="pattern">ƥ��ģʽ��</param>
		/// <param name="offset">Ҫ����ƥ����Ӵ�����ʼ������</param>
		/// <param name="options"></param>
		/// <returns>����ѡ�</returns>
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

		#region �����Ա

		#region ����������

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ�Ϊ�������
		/// </summary>
		public abstract Boolean IsBlock { get; }

		#endregion

		#region �����ķ���

		/// <summary>
		/// ��ø�ʽ���ı���
		/// </summary>
		/// <returns>��ʽ���ı���</returns>
		protected abstract void Format();

		#endregion

		#endregion


		#region �ؼ���ƥ��ṹ

		/// <summary>
		/// �ؼ���ƥ�䡣
		/// </summary>
		protected struct KeywordMatch
		{
			#region ˽���ֶ�

			private readonly Int32 m_index;
			private readonly Int32 m_length;
			private readonly String m_keyword;

			#endregion

			#region ���캯��

			/// <summary>
			/// ���캯����������ʼ���������ԡ�
			/// </summary>
			/// <param name="index">��ʼ������</param>
			/// <param name="length">���ȡ�</param>
			/// <param name="keyword">�ؼ��֡�</param>
			public KeywordMatch(Int32 index, Int32 length, String keyword)
			{
				m_index = index;
				m_length = length;
				m_keyword = keyword;
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡ��ʼ������
			/// </summary>
			public Int32 Index
			{
				get { return m_index; }
			}

			/// <summary>
			/// ��ȡ���ȡ�
			/// </summary>
			public Int32 Length
			{
				get { return m_length; }
			}

			/// <summary>
			/// ��ȡ�ؼ��֡�
			/// </summary>
			public String Keyword
			{
				get { return m_keyword; }
			}

			/// <summary>
			/// ָʾ�Ƿ�ƥ��ɹ���
			/// </summary>
			public Boolean Success
			{
				get { return (m_keyword != null); }
			}

			#endregion
		}

		#endregion


		#region �ֶ���ṹ

		/// <summary>
		/// �ֶ���ṹ��
		/// </summary>
		protected struct FieldItem
		{
			#region ���캯��

			private readonly Int32 m_index;
			private readonly String m_text;

			#endregion

			#region ���캯��

			/// <summary>
			/// ���캯���������ֶ������ʼ�������ı���
			/// </summary>
			/// <param name="index">��ʼ������</param>
			/// <param name="text">�ı���</param>
			public FieldItem(Int32 index, String text)
			{
				m_index = index;
				m_text = text;
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡ����ʼ������
			/// </summary>
			public Int32 Index
			{
				get { return m_index; }
			}

			/// <summary>
			/// ָʾ�ֶ����Ƿ���Ч��
			/// </summary>
			public Boolean IsNull
			{
				get { return (m_text == null); }
			}

			/// <summary>
			/// ��ȡ�ֶ����ı����ȡ�
			/// </summary>
			public Int32 Length
			{
				get { return Text.Length; }
			}

			/// <summary>
			/// ��ȡ���ı���
			/// </summary>
			public String Text
			{
				get { return m_text ?? String.Empty; }
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ʾ�ֶ��
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