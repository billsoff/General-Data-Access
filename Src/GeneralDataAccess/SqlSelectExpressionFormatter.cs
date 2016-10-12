#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlSelectExpressionFormatter.cs
// 文件功能描述：选择指令格式化器。
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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 选择指令格式化器。
	/// </summary>
	internal sealed class SqlSelectExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region 私有字段

		private KeywordMatch m_select;
		private KeywordMatch m_from;
		private KeywordMatch m_where;
		private KeywordMatch m_groupBy;
		private KeywordMatch m_having;
		private KeywordMatch m_orderBy;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置要格式化的 SQL 指令等。
		/// </summary>
		/// <param name="sqlExpression">SQL 指令。</param>
		/// <param name="startIndex">开始索引。</param>
		/// <param name="length">长度。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="level">级别。</param>
		public SqlSelectExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// 格式化。
		/// </summary>
		protected override void Format()
		{
			FindKeywordMatches();

			#region 断言

			Debug.Assert(m_select.Success, String.Format("没有找到 SELECT 关键字，要格式化的文本为：{0}", FormattingText));
			Debug.Assert(m_from.Success, String.Format("没有找到 FROM 关键字，要格式化的文本为：{0}", FormattingText));

			#endregion

			Int32 currentIndex = 0;

			if (!FormatSelect(ref currentIndex))
			{
				return;
			}

			if (!FormatFrom(ref currentIndex))
			{
				return;
			}

			FormatWhere(ref currentIndex);
			FormatGroupBy(ref currentIndex);
			FormatHaving(ref currentIndex);
			FormatOrderBy(ref currentIndex);
		}

		/// <summary>
		/// 格式化 SELECT 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		/// <returns>如果成功，则返回 true；否则返回 false。</returns>
		private Boolean FormatSelect(ref Int32 currentIndex)
		{
			if (!m_select.Success)
			{
				AppendLiteral(FormattingText, true);
				currentIndex = FormattingText.Length;

				return false;
			}

			AppendLiteral(SqlExpressionFormattingHelper.NormalizeKeyword(m_select.Keyword), true);
			currentIndex += m_select.Length;

			Int32 position;

			if (m_from.Success)
			{
				position = m_from.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = (position - currentIndex);
			AppendComposite(CreateSelectListFormatter(currentIndex, length));

			currentIndex = position;

			return true;
		}

		/// <summary>
		/// 格式化 FROM 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		/// <returns>如果成功，则返回 true；否则返回 false。</returns>
		private Boolean FormatFrom(ref Int32 currentIndex)
		{
			if (!m_from.Success)
			{
				return false;
			}

			AppendLiteral(m_from.Keyword, true);
			currentIndex += m_from.Length;

			Int32 position;

			if (m_where.Success)
			{
				position = m_where.Index;
			}
			else if (m_groupBy.Success)
			{
				position = m_groupBy.Index;
			}
			else if (m_having.Success)
			{
				position = m_having.Index;
			}
			else if (m_orderBy.Success)
			{
				position = m_orderBy.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateFromListFormatter(currentIndex, length));

			currentIndex = position;

			return true;
		}

		/// <summary>
		/// 格式化 WHERE 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		private void FormatWhere(ref Int32 currentIndex)
		{
			if (!m_where.Success)
			{
				return;
			}

			AppendLiteral(m_where.Keyword, true);
			currentIndex += m_where.Length;

			Int32 position;

			if (m_groupBy.Success)
			{
				position = m_groupBy.Index;
			}
			else if (m_having.Success)
			{
				position = m_having.Index;
			}
			else if (m_orderBy.Success)
			{
				position = m_orderBy.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateCriteriaFormatter(currentIndex, length, true));

			currentIndex = position;
		}

		/// <summary>
		/// 格式化 GROUP BY 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		private void FormatGroupBy(ref Int32 currentIndex)
		{
			if (!m_groupBy.Success)
			{
				return;
			}

			AppendLiteral(m_groupBy.Keyword, true);

			currentIndex += m_groupBy.Length;

			Int32 position;

			if (m_having.Success)
			{
				position = m_having.Index;
			}
			else if (m_orderBy.Success)
			{
				position = m_orderBy.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateGroupByListFormatter(currentIndex, length));

			currentIndex = position;
		}

		/// <summary>
		/// 格式化 HAVING 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		private void FormatHaving(ref Int32 currentIndex)
		{
			if (!m_having.Success)
			{
				return;
			}

			AppendLiteral(m_having.Keyword, true);
			currentIndex += m_having.Length;

			Int32 position;

			if (m_orderBy.Success)
			{
				position = m_orderBy.Index;
			}
			else
			{
				position = this.Length;
			}

			Int32 length = position - currentIndex;
			AppendComposite(CreateCriteriaFormatter(currentIndex, length, true));

			currentIndex = position;

			return;
		}

		/// <summary>
		/// 格式化 ORDER BY 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		private void FormatOrderBy(ref Int32 currentIndex)
		{
			if (!m_orderBy.Success)
			{
				return;
			}

			AppendLiteral(m_orderBy.Keyword, true);
			currentIndex += m_orderBy.Length;

			Int32 length = this.Length - currentIndex;
			AppendComposite(CreateSimpleCommaDelimitingListFormatter(currentIndex, length));

			currentIndex = this.Length;
		}

		#region 辅助方法

		/// <summary>
		/// 创建选择列表格式化器。
		/// </summary>
		/// <param name="startIndex">在要格式化的文本中的开始索引。</param>
		/// <param name="length">选择列表长度。</param>
		/// <returns>格式化器。</returns>
		private SqlExpressionFormatter CreateSelectListFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlSelectListExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// 创建分组列表格式化器。
		/// </summary>
		/// <param name="startIndex">在要格式化的文本中的开始索引。</param>
		/// <param name="length">分组列表长度。</param>
		/// <returns>格式化器。</returns>
		private SqlExpressionFormatter CreateGroupByListFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlGroupByListExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// 获取 SELECT 关键字匹配。
		/// </summary>
		/// <returns>SELECT 关键字匹配。</returns>
		private KeywordMatch FindSelectMatch()
		{
			Match m = FindValidDelimiter(@"^\s*(?<Keyword>SELECT(\s+DISTINCT)?)\b", true);

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
		/// 查找 FROM 关键字匹配。
		/// </summary>
		/// <returns>FROM 关键字匹配。</returns>
		private KeywordMatch FindFromMath()
		{
			Match m = FindValidDelimiter(@"\bFROM\b");

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
		/// 查找 WHERE 关键字匹配。
		/// </summary>
		/// <returns>WHERE 关键字匹配。</returns>
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
		/// 查找 GROUP BY 关键字。
		/// </summary>
		/// <returns>GROUP BY 关键字。</returns>
		private KeywordMatch FindGroupByMatch()
		{
			Match m = FindValidDelimiter(@"\b(?<Group>GROUP)\s+(?<By>BY)\b");

			if (m != null)
			{
				String keyword = m.Groups["Group"].Value + SqlExpressionFormattingHelper.SPACE + m.Groups["By"].Value;

				return new KeywordMatch(m.Index, m.Length, keyword);
			}
			else
			{
				return new KeywordMatch();
			}
		}

		/// <summary>
		/// 查找 HAVING 关键字。
		/// </summary>
		/// <returns>HAVING 关键字。</returns>
		private KeywordMatch FindHavingMatch()
		{
			Match m = FindValidDelimiter(@"\bHAVING\b");

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
		/// 查找 ORDER BY 关键字。
		/// </summary>
		/// <returns>ORDER BY 关键字。</returns>
		private KeywordMatch FindOrderByMatch()
		{
			Match m = FindValidDelimiter(@"\b(?<Order>ORDER)\s+(?<By>BY)\b");

			if (m != null)
			{
				String keyword = m.Groups["Order"].Value + " " + m.Groups["By"].Value;

				return new KeywordMatch(m.Index, m.Length, keyword);
			}
			else
			{
				return new KeywordMatch();
			}
		}

		/// <summary>
		/// 查找关键字匹配。
		/// </summary>
		private void FindKeywordMatches()
		{
			m_select = FindSelectMatch();
			m_from = FindFromMath();
			m_where = FindWhereMatch();
			m_groupBy = FindGroupByMatch();
			m_having = FindHavingMatch();
			m_orderBy = FindOrderByMatch();
		}

		#endregion
	}
}