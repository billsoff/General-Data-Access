#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlCommaDelimitingListExpressionFormatter.cs
// 文件功能描述：逗号分割列表格式化器。
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
	/// 逗号分割列表格式化器。
	/// </summary>
	internal abstract class SqlCommaDelimitingListExpressionFormatter : SqlBlockExpressionFormatter
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
		protected SqlCommaDelimitingListExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 获取字段列表中的所有字段项。
		/// </summary>
		/// <returns>字段项列表。</returns>
		protected FieldItem[] GetAllFieldItems()
		{
			return GetAllFieldItems(@"\s*,\s*");
		}

		#endregion
	}
}