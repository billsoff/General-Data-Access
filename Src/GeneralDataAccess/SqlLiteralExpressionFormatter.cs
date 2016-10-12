#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlLiteralExpressionFormatter.cs
// 文件功能描述：表示原样输出。
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
	/// 表示原样输出。
	/// </summary>
	internal sealed class SqlLiteralExpressionFormatter : SqlInlineExpressionFormatter
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置 SQL 指令表达式。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令表达式。</param>
		/// <param name="startIndex">开始索引。</param>
		/// <param name="length">长度。</param>
		/// <param name="indent">单位缩进。</param>
		/// <param name="level">级别。</param>
		public SqlLiteralExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// 总是原样返回要格式化的文本。
		/// </summary>
		protected override void Format()
		{
			AppendParagraph(SqlParagraph.Create(FormatOperators(FormattingText)));
		}
	}
}