#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlTableJoinCriteriaExpressionFormatter.cs
// 文件功能描述：（ON 右侧的）表连接条件格式化器。
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
	/// （ON 右侧的）表连接条件格式化器。
	/// </summary>
	internal sealed class SqlTableJoinCriteriaExpressionFormatter : SqlBlockExpressionFormatter
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
		public SqlTableJoinCriteriaExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// 格式化。
		/// </summary>
		protected override void Format()
		{
			Match[] allDelimiters = FindValidDelimiters(@"\bAND\b");

			Int32 currentIndex = 0;

			for (Int32 i = 0; i < allDelimiters.Length; i++)
			{
				Match delimiter = allDelimiters[i];
				Int32 position = delimiter.Index;
				Int32 length = position - currentIndex;

				AppendComposite(CreateTableJoinCriterionFormatter(currentIndex, length));

				AppendNewLine();
				AppendIndent();
				AppendLiteral(delimiter.Value);

				currentIndex = position + delimiter.Length;
			}

			if (currentIndex < this.Length)
			{
				AppendComposite(CreateTableJoinCriterionFormatter(currentIndex, (this.Length - currentIndex)));
			}
		}

		#region 辅助方法

		/// <summary>
		/// 创建单项连接条件格式化器。
		/// </summary>
		/// <param name="startIndex">在要格式化的文本中的起始索引。</param>
		/// <param name="length">长度。</param>
		/// <returns>格式化器。</returns>
		private SqlExpressionFormatter CreateTableJoinCriterionFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlTableJoinCriterionExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		#endregion
	}
}