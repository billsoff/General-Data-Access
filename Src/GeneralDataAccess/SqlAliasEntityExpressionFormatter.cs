#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlAliasEntityExpressionFormatter.cs
// 文件功能描述：对于表、字段等具有别名的实体的格式化器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110714
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
	/// 对于表、字段等具有别名的实体的格式化器。
	/// </summary>
	internal abstract class SqlAliasEntityExpressionFormatter : SqlExpressionFormatter
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置要格式化的 SQL 指令等。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <param name="startIndex">开始索引。</param>
		/// <param name="length">长度。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="level">级别。</param>
		protected SqlAliasEntityExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion
		/// <summary>
		/// 格式化。
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
		/// 默认实现返回 false。
		/// </summary>
		public override Boolean IsBlock
		{
			get { return false; }
		}

		#region 保护的方法

		/// <summary>
		/// 对字段、表等可设置别名的项进行分割，分别为名称、AS 关键字和别名部分，如果后两项不存在，则设置为 null。
		/// </summary>
		/// <returns>上述三部分元素组成的数组。</returns>
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
		/// 查看第一个有效字符是否是左括号。
		/// </summary>
		/// <returns>如果是，返回其索引值，否则，返回 -1。</returns>
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
		/// 尝试处理复杂的实体项。
		/// </summary>
		/// <returns>剩余项的索引。</returns>
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
		/// 处理括号表达式。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <param name="startIndex">括号内表达式的起始索引。</param>
		/// <param name="length">长度。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="level">级别。</param>
		/// <param name="formattingInfo">指示格式化的信息。</param>
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
