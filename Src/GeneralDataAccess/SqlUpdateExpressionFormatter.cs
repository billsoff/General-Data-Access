#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlUpdateExpressionFormatter.cs
// 文件功能描述：更新指令格式化器。
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
	/// 更新指令格式化器。
	/// </summary>
	internal sealed class SqlUpdateExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region 私有字段

		private KeywordMatch m_update;
		private KeywordMatch m_set;
		private KeywordMatch m_where;

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
		public SqlUpdateExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// 格式化。
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

		#region 辅助方法

		/// <summary>
		/// 查找所有的关键匹配。
		/// </summary>
		private void FindKeywords()
		{
			m_update = FindUpdateMatch();
			m_set = FindSetMatch();
			m_where = FindWhereMatch();
		}

		/// <summary>
		/// 查找 UPDATE 关键字匹配。
		/// </summary>
		/// <returns>UPDATE 关键字匹配。</returns>
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
		/// 查找 SET 关键字匹配。
		/// </summary>
		/// <returns>SET 关键字匹配。</returns>
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
		///  格式化 UPDATE 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		/// <returns>如果成功，则返回 true；否则返回 false。</returns>
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
			AppendComposite(CreateSimpleCommaDelimitingListFormatter(currentIndex, length)); // 表名，借用简单逗号分割格式化器

			currentIndex = position;

			return true;
		}

		/// <summary>
		/// 格式化 SET 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
		/// <returns>如果成功，则返回 true；否则返回 false。</returns>
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
		/// 格式化 WHERE 子句。
		/// </summary>
		/// <param name="currentIndex">当前索引。</param>
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