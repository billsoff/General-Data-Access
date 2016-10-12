#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlDeleteExpressionFormatter.cs
// �ļ�����������ɾ��ָ���ʽ������
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
	/// ɾ��ָ���ʽ������
	/// </summary>
	internal sealed class SqlDeleteExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region ˽���ֶ�

		private KeywordMatch m_delete;
		private KeywordMatch m_where;

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
		public SqlDeleteExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// ��ʽ����
		/// </summary>
		protected override void Format()
		{
			FindKeywords();

			Int32 currentIndex = 0;

			if (!FormatDelete(ref currentIndex))
			{
				return;
			}

			FormatWhere(ref currentIndex);
		}

		#region ��������

		/// <summary>
		/// ���ҹؼ���ƥ�䡣
		/// </summary>
		private void FindKeywords()
		{
			m_delete = FindDeleteMatch();
			m_where = FindWhereMatch();
		}

		/// <summary>
		/// ���� DELETE �ؼ���ƥ�䡣
		/// </summary>
		/// <returns>DELETE �ؼ���ƥ�䡣</returns>
		private KeywordMatch FindDeleteMatch()
		{
			Match m = FindValidDelimiter(@"^\s*(?<Delete>DELETE)\s+(?<From>FROM)\b");

			if (m != null)
			{
				String keyword = m.Groups["Delete"].Value + SqlExpressionFormattingHelper.SPACE + m.Groups["From"].Value;

				return new KeywordMatch(m.Index, m.Length, keyword);
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
		/// ��ʽ�� DELETE �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ʼ������</param>
		/// <returns>����ɹ������� true�����򷵻� false��</returns>
		private Boolean FormatDelete(ref Int32 currentIndex)
		{
			if (!m_delete.Success)
			{
				AppendLiteral(FormattingText, true);
				currentIndex = this.Length;

				return false;
			}

			AppendLiteral(m_delete.Keyword, true);
			currentIndex += m_delete.Length;

			Int32 position;
			Int32 length;

			if (m_where.Success)
			{
				position = m_where.Index;
				length = position - currentIndex;

				AppendComposite(CreateSimpleCommaDelimitingListFormatter(currentIndex, length)); // ���������ü򵥶��ŷָ��ʽ����
			}
			else
			{
				position = this.Length;
				length = position - currentIndex;

				AppendSpace();
				AppendLiteral(FormattingText.Substring(currentIndex, length).Trim());
			}

			currentIndex = position;

			return true;
		}

		/// <summary>
		/// ��ʽ�� WHERE �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ʼ������</param>
		private void FormatWhere(ref Int32 currentIndex)
		{
			if (m_where.Success)
			{
				AppendLiteral(m_where.Keyword, true);
				currentIndex += m_where.Length;

				Int32 length = this.Length - currentIndex;
				AppendComposite(CreateCriteriaFormatter(currentIndex, length, true));

				currentIndex = this.Length;
			}
		}

		#endregion
	}
}