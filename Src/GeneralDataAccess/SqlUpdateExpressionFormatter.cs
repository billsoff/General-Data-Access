#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlUpdateExpressionFormatter.cs
// �ļ���������������ָ���ʽ������
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
	/// ����ָ���ʽ������
	/// </summary>
	internal sealed class SqlUpdateExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region ˽���ֶ�

		private KeywordMatch m_update;
		private KeywordMatch m_set;
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
		public SqlUpdateExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
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

			if (!FormatUpdate(ref currentIndex))
			{
				return;
			}

			if (!FormatSet(ref currentIndex))
			{
				return;
			}

			FormatWhere(ref currentIndex);
		}

		#region ��������

		/// <summary>
		/// �������еĹؼ�ƥ�䡣
		/// </summary>
		private void FindKeywords()
		{
			m_update = FindUpdateMatch();
			m_set = FindSetMatch();
			m_where = FindWhereMatch();
		}

		/// <summary>
		/// ���� UPDATE �ؼ���ƥ�䡣
		/// </summary>
		/// <returns>UPDATE �ؼ���ƥ�䡣</returns>
		private KeywordMatch FindUpdateMatch()
		{
			Match m = FindValidDelimiter(@"^\s*(?<Keyword>UPDATE)\b");

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
		/// ���� SET �ؼ���ƥ�䡣
		/// </summary>
		/// <returns>SET �ؼ���ƥ�䡣</returns>
		private KeywordMatch FindSetMatch()
		{
			Match m = FindValidDelimiter(@"\bSET\b");

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
		///  ��ʽ�� UPDATE �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		/// <returns>����ɹ����򷵻� true�����򷵻� false��</returns>
		private Boolean FormatUpdate(ref Int32 currentIndex)
		{
			if (!m_update.Success)
			{
				AppendLiteral(FormattingText, true);
				currentIndex = this.Length;

				return false;
			}

			AppendLiteral(m_update.Keyword, true);
			currentIndex += m_update.Length;

			Int32 position;

			if (m_set.Success)
			{
				position = m_set.Index;
			}
			else if (m_where.Success)
			{
				position = m_where.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateSimpleCommaDelimitingListFormatter(currentIndex, length)); // ���������ü򵥶��ŷָ��ʽ����

			currentIndex = position;

			return true;
		}

		/// <summary>
		/// ��ʽ�� SET �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		/// <returns>����ɹ����򷵻� true�����򷵻� false��</returns>
		private Boolean FormatSet(ref Int32 currentIndex)
		{
			if (!m_set.Success)
			{
				return false;
			}

			AppendLiteral(m_set.Keyword, true);
			currentIndex += m_set.Length;

			Int32 position;

			if (m_where.Success)
			{
				position = m_where.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;

			AppendComposite(CreateSimpleCommaDelimitingListFormatter(currentIndex, length));
			currentIndex = position;

			return true;
		}

		/// <summary>
		/// ��ʽ�� WHERE �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
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