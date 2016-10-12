#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlCriteriaExpressionFormatter.cs
// �ļ���������������������ʽ������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110714
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
	/// ����������ʽ������
	/// </summary>
	internal sealed class SqlCriteriaExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region ˽���ֶ�

		private readonly Boolean m_statementLevel;

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
		public SqlCriteriaExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: this(sqlExpression, startIndex, length, indent, level, false)
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
		/// <param name="statementLevel">ָʾ�Ƿ�Ϊ��伶�����Ϊ true����ʹֻ����һ�����������ҲӦ���������򣬲���Ҫ������</param>
		public SqlCriteriaExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level, Boolean statementLevel)
			: base(sqlExpression, startIndex, length, indent, level)
		{
			m_statementLevel = statementLevel;
		}

		#endregion

		/// <summary>
		/// ��ʽ����
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

		#region ��������

		/// <summary>
		/// ����ʹ�� OR ���зָ
		/// </summary>
		/// <returns>����ָ�ɹ����򷵻� true�����򷵻� false��</returns>
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
		/// ����ʹ�� AND ���зָ
		/// </summary>
		/// <returns>����ָ�ɹ����򷵻� true�����򷵻� false��</returns>
		private Boolean TryDelimitByAnd()
		{
			// ������ڶ��� BETWEEN �ָ�������������һ������
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
		/// ��ʽ�� NOT �ؼ��֡�
		/// </summary>
		/// <param name="currentIndex">���ø�ʽ�����λ�á�</param>
		/// <returns>������� NOT �ؼ��֣��򷵻� true�����򷵻� false��</returns>
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
		/// ��ʽ����һ�����
		/// </summary>
		private void FormatCriterion()
		{
			Int32 currentIndex;
			Boolean startsWithNotKeyword = FormatNot(out currentIndex);

			// ��������������
			Int32 leftOpenParenthesisIndex = FindLeftOpenParenthesisIndex(currentIndex);

			// û���ҵ����������ţ���ǰ�����������һ�������ı��ʽ
			if (leftOpenParenthesisIndex == -1)
			{
				// ���Ե�ǰ����Ƿ���һ�� IN ���ʽ
				Int32 inListOpenParenthesisIndex = FindInListOpenParenthesisIndex(currentIndex);

				// ��ǰ�ı���ǲ��� IN ���ʽ
				if (inListOpenParenthesisIndex == -1)
				{
					AppendLiteral(FormatOperators(FormattingText.Substring(currentIndex)), !startsWithNotKeyword);
				}
				// ��ǰ�ı��ʽ�� IN ���ʽ
				else
				{
					FormatInList(startsWithNotKeyword, currentIndex, inListOpenParenthesisIndex);
				}
			}
			// �ҵ������������ţ���������һ�����ϱ��ʽ
			else
			{
				FormatCompositeExpression(startsWithNotKeyword, leftOpenParenthesisIndex);
			}
		}

		/// <summary>
		/// ��ʽ�� IN ���ʽ��
		/// </summary>
		/// <param name="startsWithNotKeyword">ָʾ�Ƿ��� NOT �ؼ��ֿ�ʼ��</param>
		/// <param name="currentIndex">��ǰ������</param>
		/// <param name="inListOpenParenthesisIndex">IN �б�������������</param>
		private void FormatInList(Boolean startsWithNotKeyword, Int32 currentIndex, Int32 inListOpenParenthesisIndex)
		{
			// ��� IN �б�֮ǰ���ַ�
			AppendLiteral(
					FormattingText.Substring(
							currentIndex,
							inListOpenParenthesisIndex - currentIndex
						).TrimStart(),
					!startsWithNotKeyword
				);

			// ���� IN �б�
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
		/// ��ʽ�����ϱ��ʽ��������������ı��ʽ����
		/// </summary>
		/// <param name="startsWithNotKeyword">ָʾ�Ƿ��� NOT �ؼ��ֿ�ͷ��</param>
		/// <param name="leftOpenParenthesisIndex">�������������</param>
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

							// ���Ա��ʽ�Ƿ������ſ�ͷ
							index = FindLeftOpenParenthesisIndex(expression, 0);

							// ����һ���������ʽ
							if (index == -1)
							{
								// �����Ƿ�Ϊ IN �������ʽ
								index = FindInListOpenParenthesisIndex(expression, 0);

								// ���� IN �������ʽ
								if (index == -1)
								{
									formattingInfo.Formatter = CreateLiteralFormatter((startIndex - this.StartIndex), length);

									return;
								}
							}
						}

						// ����һ�����ϱ��ʽ��
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
		/// ����ѡ��ֵ�б��������š�
		/// </summary>
		/// <param name="startIndex">��ʼ����λ�á�</param>
		/// <returns>λ�ã����û���ҵ����򷵻� ��1��</returns>
		private Int32 FindInListOpenParenthesisIndex(Int32 startIndex)
		{
			return FindInListOpenParenthesisIndex(FormattingText, startIndex);
		}

		/// <summary>
		/// ����ѡ��ֵ�б��������š�
		/// </summary>
		/// <param name="expression">Ҫ���ҵı��ʽ��</param>
		/// <param name="startIndex">��ʼ���ҵ�λ�á�</param>
		/// <returns>ֵ�б������ŵ�λ�ã����û���ҵ����򷵻� -1;</returns>
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
		/// �������������š�
		/// </summary>
		/// <param name="startIndex">��ʼ����λ�á�</param>
		/// <returns>λ�ã����û���ҵ����򷵻� -1��</returns>
		private Int32 FindLeftOpenParenthesisIndex(Int32 startIndex)
		{
			return FindLeftOpenParenthesisIndex(FormattingText, startIndex);
		}

		/// <summary>
		/// ���ұ���е������š�
		/// </summary>
		/// <param name="expression">���ʽ��</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <returns>����������</returns>
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
		/// �жϱ��ʽ�Ƿ��� NOT �ؼ��ֿ�ͷ��
		/// </summary>
		/// <param name="expression">�������ʽ��</param>
		/// <returns>����� NOT ��ͷ���򷵻� true�����򷵻� false��</returns>
		private static Boolean StartsWithNotKeyword(String expression)
		{
			return Regex.IsMatch(expression, @"^\s*NOT\b", DEFAULT_REGEX_OPTIONS);
		}

		#endregion
	}
}