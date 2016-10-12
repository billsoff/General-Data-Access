#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlSelectExpressionFormatter.cs
// �ļ�����������ѡ��ָ���ʽ������
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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ѡ��ָ���ʽ������
	/// </summary>
	internal sealed class SqlSelectExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region ˽���ֶ�

		private KeywordMatch m_select;
		private KeywordMatch m_from;
		private KeywordMatch m_where;
		private KeywordMatch m_groupBy;
		private KeywordMatch m_having;
		private KeywordMatch m_orderBy;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ��ʽ���� SQL ָ��ȡ�
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		public SqlSelectExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// ��ʽ����
		/// </summary>
		protected override void Format()
		{
			FindKeywordMatches();

			#region ����

			Debug.Assert(m_select.Success, String.Format("û���ҵ� SELECT �ؼ��֣�Ҫ��ʽ�����ı�Ϊ��{0}", FormattingText));
			Debug.Assert(m_from.Success, String.Format("û���ҵ� FROM �ؼ��֣�Ҫ��ʽ�����ı�Ϊ��{0}", FormattingText));

			#endregion

			Int32 currentIndex = 0;

			if (!FormatSelect(ref currentIndex))
			{
				return;
			}

			if (!FormatFrom(ref currentIndex))
			{
				return;
			}

			FormatWhere(ref currentIndex);
			FormatGroupBy(ref currentIndex);
			FormatHaving(ref currentIndex);
			FormatOrderBy(ref currentIndex);
		}

		/// <summary>
		/// ��ʽ�� SELECT �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		/// <returns>����ɹ����򷵻� true�����򷵻� false��</returns>
		private Boolean FormatSelect(ref Int32 currentIndex)
		{
			if (!m_select.Success)
			{
				AppendLiteral(FormattingText, true);
				currentIndex = FormattingText.Length;

				return false;
			}

			AppendLiteral(SqlExpressionFormattingHelper.NormalizeKeyword(m_select.Keyword), true);
			currentIndex += m_select.Length;

			Int32 position;

			if (m_from.Success)
			{
				position = m_from.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = (position - currentIndex);
			AppendComposite(CreateSelectListFormatter(currentIndex, length));

			currentIndex = position;

			return true;
		}

		/// <summary>
		/// ��ʽ�� FROM �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		/// <returns>����ɹ����򷵻� true�����򷵻� false��</returns>
		private Boolean FormatFrom(ref Int32 currentIndex)
		{
			if (!m_from.Success)
			{
				return false;
			}

			AppendLiteral(m_from.Keyword, true);
			currentIndex += m_from.Length;

			Int32 position;

			if (m_where.Success)
			{
				position = m_where.Index;
			}
			else if (m_groupBy.Success)
			{
				position = m_groupBy.Index;
			}
			else if (m_having.Success)
			{
				position = m_having.Index;
			}
			else if (m_orderBy.Success)
			{
				position = m_orderBy.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateFromListFormatter(currentIndex, length));

			currentIndex = position;

			return true;
		}

		/// <summary>
		/// ��ʽ�� WHERE �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		private void FormatWhere(ref Int32 currentIndex)
		{
			if (!m_where.Success)
			{
				return;
			}

			AppendLiteral(m_where.Keyword, true);
			currentIndex += m_where.Length;

			Int32 position;

			if (m_groupBy.Success)
			{
				position = m_groupBy.Index;
			}
			else if (m_having.Success)
			{
				position = m_having.Index;
			}
			else if (m_orderBy.Success)
			{
				position = m_orderBy.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateCriteriaFormatter(currentIndex, length, true));

			currentIndex = position;
		}

		/// <summary>
		/// ��ʽ�� GROUP BY �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		private void FormatGroupBy(ref Int32 currentIndex)
		{
			if (!m_groupBy.Success)
			{
				return;
			}

			AppendLiteral(m_groupBy.Keyword, true);

			currentIndex += m_groupBy.Length;

			Int32 position;

			if (m_having.Success)
			{
				position = m_having.Index;
			}
			else if (m_orderBy.Success)
			{
				position = m_orderBy.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateGroupByListFormatter(currentIndex, length));

			currentIndex = position;
		}

		/// <summary>
		/// ��ʽ�� HAVING �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		private void FormatHaving(ref Int32 currentIndex)
		{
			if (!m_having.Success)
			{
				return;
			}

			AppendLiteral(m_having.Keyword, true);
			currentIndex += m_having.Length;

			Int32 position;

			if (m_orderBy.Success)
			{
				position = m_orderBy.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateCriteriaFormatter(currentIndex, length, true));

			currentIndex = position;

			return;
		}

		/// <summary>
		/// ��ʽ�� ORDER BY �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		private void FormatOrderBy(ref Int32 currentIndex)
		{
			if (!m_orderBy.Success)
			{
				return;
			}

			AppendLiteral(m_orderBy.Keyword, true);
			currentIndex += m_orderBy.Length;

			Int32 length = this.Length - currentIndex;
			AppendComposite(CreateSimpleCommaDelimitingListFormatter(currentIndex, length));

			currentIndex = this.Length;
		}

		#region ��������

		/// <summary>
		/// ����ѡ���б��ʽ������
		/// </summary>
		/// <param name="startIndex">��Ҫ��ʽ�����ı��еĿ�ʼ������</param>
		/// <param name="length">ѡ���б��ȡ�</param>
		/// <returns>��ʽ������</returns>
		private SqlExpressionFormatter CreateSelectListFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlSelectListExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// ���������б��ʽ������
		/// </summary>
		/// <param name="startIndex">��Ҫ��ʽ�����ı��еĿ�ʼ������</param>
		/// <param name="length">�����б��ȡ�</param>
		/// <returns>��ʽ������</returns>
		private SqlExpressionFormatter CreateGroupByListFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlGroupByListExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// ��ȡ SELECT �ؼ���ƥ�䡣
		/// </summary>
		/// <returns>SELECT �ؼ���ƥ�䡣</returns>
		private KeywordMatch FindSelectMatch()
		{
			Match m = FindValidDelimiter(@"^\s*(?<Keyword>SELECT(\s+DISTINCT)?)\b", true);

			if (m != null)
			{
				return new KeywordMatch(m.Index, m.Length, m.Groups["Keyword"].Value);
			}
			else
			{
				return new KeywordMatch();
			}
		}

		/// <summary>
		/// ���� FROM �ؼ���ƥ�䡣
		/// </summary>
		/// <returns>FROM �ؼ���ƥ�䡣</returns>
		private KeywordMatch FindFromMath()
		{
			Match m = FindValidDelimiter(@"\bFROM\b");

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
		/// ���� WHERE �ؼ���ƥ�䡣
		/// </summary>
		/// <returns>WHERE �ؼ���ƥ�䡣</returns>
		private KeywordMatch FindWhereMatch()
		{
			Match m = FindValidDelimiter(@"\bWHERE\b");

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
		/// ���� GROUP BY �ؼ��֡�
		/// </summary>
		/// <returns>GROUP BY �ؼ��֡�</returns>
		private KeywordMatch FindGroupByMatch()
		{
			Match m = FindValidDelimiter(@"\b(?<Group>GROUP)\s+(?<By>BY)\b");

			if (m != null)
			{
				String keyword = m.Groups["Group"].Value + SqlExpressionFormattingHelper.SPACE + m.Groups["By"].Value;

				return new KeywordMatch(m.Index, m.Length, keyword);
			}
			else
			{
				return new KeywordMatch();
			}
		}

		/// <summary>
		/// ���� HAVING �ؼ��֡�
		/// </summary>
		/// <returns>HAVING �ؼ��֡�</returns>
		private KeywordMatch FindHavingMatch()
		{
			Match m = FindValidDelimiter(@"\bHAVING\b");

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
		/// ���� ORDER BY �ؼ��֡�
		/// </summary>
		/// <returns>ORDER BY �ؼ��֡�</returns>
		private KeywordMatch FindOrderByMatch()
		{
			Match m = FindValidDelimiter(@"\b(?<Order>ORDER)\s+(?<By>BY)\b");

			if (m != null)
			{
				String keyword = m.Groups["Order"].Value + " " + m.Groups["By"].Value;

				return new KeywordMatch(m.Index, m.Length, keyword);
			}
			else
			{
				return new KeywordMatch();
			}
		}

		/// <summary>
		/// ���ҹؼ���ƥ�䡣
		/// </summary>
		private void FindKeywordMatches()
		{
			m_select = FindSelectMatch();
			m_from = FindFromMath();
			m_where = FindWhereMatch();
			m_groupBy = FindGroupByMatch();
			m_having = FindHavingMatch();
			m_orderBy = FindOrderByMatch();
		}

		#endregion
	}
}