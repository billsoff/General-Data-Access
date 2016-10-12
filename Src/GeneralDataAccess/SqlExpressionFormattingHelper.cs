#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlExpressionFormattingHelper.cs
// 文件功能描述：格式化辅助类。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201107113
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
	/// 格式化辅助类。
	/// </summary>
	internal static class SqlExpressionFormattingHelper
	{
		/// <summary>
		/// 右括号。
		/// </summary> 
		public const String CLOSE_PARENTHESIS = ")";

		/// <summary>
		/// 逗号。
		/// </summary>
		public const String COMMA = ",";

		/// <summary>
		/// 左括号。
		/// </summary>
		public const String OPEN_PARENTHESIS = "(";

		/// <summary>
		/// 空格。
		/// </summary>
		public const String SPACE = " ";

		/// <summary>
		/// 规范化关键字，即可关键字以一个空格隔开。
		/// </summary>
		/// <param name="keyword">关键字。</param>
		/// <returns>规范好的关键字。</returns>
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
		/// 识别 SQL 指令的类型。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <returns>指令类型。</returns>
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