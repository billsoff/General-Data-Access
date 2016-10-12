#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlAliasEntityExpressionFormatter.cs
// �ļ��������������ڱ��ֶεȾ��б�����ʵ��ĸ�ʽ������
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
	/// ���ڱ��ֶεȾ��б�����ʵ��ĸ�ʽ������
	/// </summary>
	internal abstract class SqlAliasEntityExpressionFormatter : SqlExpressionFormatter
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ��ʽ���� SQL ָ��ȡ�
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		protected SqlAliasEntityExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion
		/// <summary>
		/// ��ʽ����
		/// </summary>
		protected override void Format()
		{
			if (IsBlock)
			{
				AppendNewLine();
				AppendIndent();
			}

			Int32 remainIndex = TryHandleComplexEntity();

			if (remainIndex == 0)
			{
				AppendLiteral(FormattingText.TrimStart());
			}
			else if (remainIndex < this.Length)
			{
				AppendLiteral(FormattingText.Substring(remainIndex));
			}
		}

		/// <summary>
		/// Ĭ��ʵ�ַ��� false��
		/// </summary>
		public override Boolean IsBlock
		{
			get { return false; }
		}

		#region �����ķ���

		/// <summary>
		/// ���ֶΡ���ȿ����ñ���������зָ�ֱ�Ϊ���ơ�AS �ؼ��ֺͱ������֣������������ڣ�������Ϊ null��
		/// </summary>
		/// <returns>����������Ԫ����ɵ����顣</returns>
		public String[] ParseEntity()
		{
			String[] items = new String[] { FormattingText, null, null };
			Match delimiter = FindValidDelimiter(@"\b(?<Keyword>AS|\s+)\b");

			String name = FormattingText;
			String keyword = null;
			String alias = null;

			if (delimiter != null)
			{
				name = FormattingText.Substring(0, delimiter.Index);
				alias = FormattingText.Substring(delimiter.Index + delimiter.Length).Trim();

				keyword = delimiter.Groups["Keyword"].Value.Trim();

				if (keyword.Length == 0)
				{
					keyword = null;
				}

				if (alias.Length == 0)
				{
					alias = null;
				}

				items[0] = name;
				items[1] = keyword;
				items[2] = alias;
			}

			return items;
		}

		/// <summary>
		/// �鿴��һ����Ч�ַ��Ƿ��������š�
		/// </summary>
		/// <returns>����ǣ�����������ֵ�����򣬷��� -1��</returns>
		private Int32 FindLeftMostOpenParenthesisIndex()
		{
			Regex ex = new Regex(@"^\s*\(");
			Match m = ex.Match(FormattingText);

			if (m.Success)
			{
				return m.Index;
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// ���Դ����ӵ�ʵ���
		/// </summary>
		/// <returns>ʣ�����������</returns>
		private Int32 TryHandleComplexEntity()
		{
			Int32 openParenthesisIndex = FindLeftMostOpenParenthesisIndex();

			if (openParenthesisIndex == -1)
			{
				return 0;
			}

			SqlExpressionFormatter formatter = EvaluateParenthesesExpression(openParenthesisIndex, HandleParenthesesExpression, out openParenthesisIndex);

			if (formatter != null)
			{
				AppendComposite(formatter);

				return (formatter.Length + (formatter.StartIndex - this.StartIndex));
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// �������ű��ʽ��
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="startIndex">�����ڱ��ʽ����ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		/// <param name="formattingInfo">ָʾ��ʽ������Ϣ��</param>
		private void HandleParenthesesExpression(
			String sqlExpression,
			Int32 startIndex,
			Int32 length,
			String indent,
			Int32 level,
			SqlParenthesesExpressionFormattingInfo formattingInfo
		)
		{
			SqlExpressionFormatter formatter = Recognize(sqlExpression, startIndex, length, indent, (level + 1));

			formattingInfo.OuterOpenParenthesisStartsAtNewLine = false;
			formattingInfo.Formatter = formatter;
		}

		#endregion
	}
}
