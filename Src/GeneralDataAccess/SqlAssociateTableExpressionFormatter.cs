#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlAssociateTableExpressionFormatter.cs
// 文件功能描述：（由连接条件分割的）关联表的格式化器。
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
	/// （由连接条件分割的）关联表的格式化器。
	/// </summary>
	internal sealed class SqlAssociateTableExpressionFormatter : SqlBlockExpressionFormatter
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
		public SqlAssociateTableExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// 格式化。
		/// </summary>
		protected override void Format()
		{
			Match delimiter = FindValidDelimiter(@"\bON\b");

			if (delimiter != null)
			{
				AppendComposite(CreateTableItemFormatter(0, delimiter.Index));

				AppendLiteral(delimiter.Value, true);

				Int32 currentIndex = delimiter.Index + delimiter.Length;
				Int32 length = this.Length - currentIndex;

				AppendComposite(CreateTableJoinCriteriaFormatter(currentIndex, length));
			}
			else
			{
				AppendComposite(CreateTableItemFormatter(0, this.Length));
			}
		}

		#region 私有方法

		/// <summary>
		/// 创建（ON 左侧的）表表达式格式化器。
		/// </summary>
		/// <param name="startIndex">起始索引。</param>
		/// <param name="length">长度。</param>
		/// <returns>格式化器。</returns>
		private SqlExpressionFormatter CreateTableItemFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlTableItemExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// 创建（ON 右侧的）表连接复合条件格式化器。
		/// </summary>
		/// <param name="startIndex">起始索引。</param>
		/// <param name="length">长度。</param>
		/// <returns>格式化器。</returns>
		private SqlExpressionFormatter CreateTableJoinCriteriaFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlTableJoinCriteriaExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		#endregion
	}
}