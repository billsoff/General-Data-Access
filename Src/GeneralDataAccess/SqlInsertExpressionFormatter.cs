#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlInsertExpressionFormatter.cs
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
	internal sealed class SqlInsertExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region ˽���ֶ�

		private KeywordMatch m_insert;
		private KeywordMatch m_values;

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
		public SqlInsertExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
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

			if (!FormatInsert(ref currentIndex))
			{
				return;
			}

			FormatValues(ref currentIndex);
		}

		#region ��������

		/// <summary>
		/// ���ҹؼ���ƥ�䡣
		/// </summary>
		private void FindKeywords()
		{
			m_insert = FindInsertMatch();
			m_values = FindValuesMatch();
		}

		/// <summary>
		/// ���� INSERT �ؼ���ƥ�䡣
		/// </summary>
		/// <returns>INSERT �ؼ���ƥ�䡣</returns>
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
		/// ���� VALUES �ؼ���ƥ�䡣
		/// </summary>
		/// <returns>VALUES �ؼ���ƥ�䡣</returns>
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
		/// ��ʽ�� INSERT �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ǰ������</param>
		/// <returns>����ɹ����򷵻� true�����򷵻� false��</returns>
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
				currentIndex += formatter.Length + 2; // ������������

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
		/// ��ʽ�� VALUES �Ӿ䡣
		/// </summary>
		/// <param name="currentIndex">��ʼ������</param>
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
				currentIndex = (openParenthesisIndex + formatter.Length + 2); // ������������
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