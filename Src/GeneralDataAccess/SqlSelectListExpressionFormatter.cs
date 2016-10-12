#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlSelectListExpressionFormatter.cs
// 文件功能描述：SELECT 指令选择列表格式化器。
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
	/// SELECT 指令选择列表格式化器。
	/// </summary>
	internal sealed class SqlSelectListExpressionFormatter : SqlCommaDelimitingListExpressionFormatter
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
		public SqlSelectListExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// 格式化。
		/// </summary>
		protected override void Format()
		{
			FieldItem[] allFields = GetAllFieldItems();

			for (Int32 i = 0; i < allFields.Length; i++)
			{
				FieldItem field = allFields[i];

				if (i != 0)
				{
					AppendComma();
				}

				AppendNewLine();
				AppendIndent();
				AppendComposite(CreateSelectFieldFormatter(field.Index, field.Length));
			}
		}

		#region 辅助方法

		/// <summary>
		/// 创建选择字段格式化器。
		/// </summary>
		/// <param name="startIndex">在要格式化文本中的起始索引。</param>
		/// <param name="length">长度。</param>
		/// <returns>格式化器。</returns>
		private SqlExpressionFormatter CreateSelectFieldFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlSelectFieldExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		#endregion
	}
}