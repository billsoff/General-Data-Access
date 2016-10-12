#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SqlCompositeParagraph.cs
// 文件功能描述：SQL 复合段落，包含一个格式化器。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	internal sealed class SqlCompositeParagraph : SqlParagraph
	{
		#region 私有字段

		private readonly SqlExpressionFormatter m_formatter;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置格式化器。。
		/// </summary>
		/// <param name="formatter">格式化器。</param>
		public SqlCompositeParagraph(SqlExpressionFormatter formatter)
		{
			m_formatter = formatter;
		}

		#endregion

		/// <summary>
		/// 使用引用的格式化器的输出。
		/// </summary>
		/// <returns></returns>
		protected override String Output()
		{
			return m_formatter.ToString();
		}
	}
}