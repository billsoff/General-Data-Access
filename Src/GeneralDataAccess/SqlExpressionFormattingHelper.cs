#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlExpressionFormattingHelper.cs
// �ļ�������������ʽ�������ࡣ
//
//
// ������ʶ���α���billsoff@gmail.com�� 201107113
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
	/// ��ʽ�������ࡣ
	/// </summary>
	internal static class SqlExpressionFormattingHelper
	{
		/// <summary>
		/// �����š�
		/// </summary> 
		public const String CLOSE_PARENTHESIS = ")";

		/// <summary>
		/// ���š�
		/// </summary>
		public const String COMMA = ",";

		/// <summary>
		/// �����š�
		/// </summary>
		public const String OPEN_PARENTHESIS = "(";

		/// <summary>
		/// �ո�
		/// </summary>
		public const String SPACE = " ";

		/// <summary>
		/// �淶���ؼ��֣����ɹؼ�����һ���ո������
		/// </summary>
		/// <param name="keyword">�ؼ��֡�</param>
		/// <returns>�淶�õĹؼ��֡�</returns>
		public static String NormalizeKeyword(String keyword)
		{
			List<String> items = new List<String>();

			foreach (Match word in Regex.Matches(keyword, "[a-zA-Z]+", RegexOptions.IgnorePatternWhitespace))
			{
				items.Add(word.Value);
			}

			return String.Join(SPACE, items.ToArray());
		}

		/// <summary>
		/// ʶ�� SQL ָ������͡�
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <returns>ָ�����͡�</returns>
		public static SqlStatementType Recognize(String sqlExpression)
		{
			if (String.IsNullOrEmpty(sqlExpression))
			{
				return SqlStatementType.Unrecognisable;
			}

			const RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;

			String[] patterns = new String[]
			{
				@"^\s*SELECT\b",
				@"^\s*UPDATE\b",
				@"^\s*INSERT\s+INTO\b",
				@"^\s*DELETE\s+FROM\b"
			};

			SqlStatementType[] types = new SqlStatementType[patterns.Length];

			types[0] = SqlStatementType.Select;
			types[1] = SqlStatementType.Update;
			types[2] = SqlStatementType.Insert;
			types[3] = SqlStatementType.Delete;

			for (Int32 i = 0; i < patterns.Length; i++)
			{
				if (Regex.IsMatch(sqlExpression, patterns[i], REGEX_OPTIONS))
				{
					return types[i];
				}
			}

			return SqlStatementType.Unrecognisable;
		}
	}
}