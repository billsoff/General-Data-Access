#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlSimpleCommaDelimitingListExpressionFormatter.cs
// 文件功能描述：将用逗号分隔的项进行简单的罗列，不再进一步格式化。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110718
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
	/// 将用逗号分隔的项进行简单的罗列，不再进一步格式化。
	/// </summary>
	internal sealed class SqlSimpleCommaDelimitingListExpressionFormatter : SqlCommaDelimitingListExpressionFormatter
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
		public SqlSimpleCommaDelimitingListExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
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
				if (i != 0)
				{
					AppendComma();
				}

				AppendLiteral(FormatOperators(allFields[i].Text), true);
			}
		}
	}
}