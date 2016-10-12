#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlParagraph.cs
// 文件功能描述：表示 SQL 指令的一段。
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

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示 SQL 指令的一段。
	/// </summary>
	internal abstract class SqlParagraph
	{
		#region 工厂方法

		/// <summary>
		/// 右括号。
		/// </summary>
		public static readonly SqlParagraph CloseParenthesis = new SqlLiteralParagraph(SqlExpressionFormattingHelper.CLOSE_PARENTHESIS);

		/// <summary>
		/// 逗号。
		/// </summary>
		public static readonly SqlParagraph Comma = new SqlLiteralParagraph(SqlExpressionFormattingHelper.COMMA);

		/// <summary>
		/// 换行。
		/// </summary>
		public static readonly SqlParagraph NewLine = new SqlLiteralParagraph(Environment.NewLine);

		/// <summary>
		/// 左括号。
		/// </summary>
		public static readonly SqlParagraph OpenParenthesis = new SqlLiteralParagraph(SqlExpressionFormattingHelper.OPEN_PARENTHESIS);

		/// <summary>
		/// 空格。
		/// </summary>
		public static readonly SqlParagraph Space = new SqlLiteralParagraph(SqlExpressionFormattingHelper.SPACE);

		/// <summary>
		/// 创建逐字输出的段落。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <returns>段落。</returns>
		public static SqlParagraph Create(String text)
		{
			return new SqlLiteralParagraph(text);
		}

		/// <summary>
		/// 创建复合段落。
		/// </summary>
		/// <param name="formatter">格式化器。</param>
		/// <returns>复合段落。</returns>
		public static SqlParagraph Create(SqlExpressionFormatter formatter)
		{
			return new SqlCompositeParagraph(formatter);
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 输出段落文本。
		/// </summary>
		/// <returns>段落文本。</returns>
		public sealed override String ToString()
		{
			return Output();
		}

		#endregion

		#region 抽象成员

		/// <summary>
		/// 输出段落文本。
		/// </summary>
		/// <returns>段落文本。</returns>
		protected abstract String Output();

		#endregion
	}
}