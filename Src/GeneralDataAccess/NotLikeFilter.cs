#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NotLikeFilter.cs
// 文件功能描述：表示 NOT LIKE 过滤器表达式。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20100815
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
	/// 表示 NOT LIKE 过滤器表达式。
	/// </summary>
	[Serializable]
	public class NotLikeFilter : Filter
	{
		#region 私有字段

		private readonly Char m_escapeChar;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性名称和匹配模式。。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		public NotLikeFilter(String propertyName, String patternText)
			: base(propertyName, new Object[] { patternText })
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称、实体中值属性名称和匹配模式。。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		public NotLikeFilter(String entityPropertyName, String propertyName, String patternText)
			: base(entityPropertyName, propertyName, new Object[] { patternText })
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径和匹配模式。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		public NotLikeFilter(IList<String> propertyPath, String patternText)
			: base(propertyPath, new Object[] { patternText })
		{
		}

		/// <summary>
		/// 构造函数，设置属性名称和匹配模式。。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		/// <param name="escapeChar">转义字符。</param>
		public NotLikeFilter(String propertyName, String patternText, Char escapeChar)
			: base(propertyName, new Object[] { patternText })
		{
			m_escapeChar = escapeChar;
		}

		/// <summary>
		/// 构造函数，设置实体属性名称、实体中值属性名称和匹配模式。。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		/// <param name="escapeChar">转义字符。</param>
		public NotLikeFilter(String entityPropertyName, String propertyName, String patternText, Char escapeChar)
			: base(entityPropertyName, propertyName, new Object[] { patternText })
		{
			m_escapeChar = escapeChar;
		}

		/// <summary>
		/// 构造函数，设置属性路径和匹配模式。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="escapeChar">转义字符。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		public NotLikeFilter(IList<String> propertyPath, String patternText, Char escapeChar)
			: base(propertyPath, new Object[] { patternText })
		{
			m_escapeChar = escapeChar;
		}

		#endregion

		/// <summary>
		/// 向输入缓冲区写入条件表达式。
		/// </summary>
		/// <param name="output">输入缓冲区。</param>
		public override void Generate(StringBuilder output)
		{
			ThrowExceptionIfNotCompiled();

			output.Append(ColumnFullName);

			if (!Negative)
			{
				output.Append(NOT_LIKE);
			}
			else
			{
				output.Append(LIKE);
			}

			output.Append(ComposeParameter(Parameters[0].Name));

			if (m_escapeChar != 0)
			{
				output.AppendFormat(" ESCAPE '{0}'", m_escapeChar);
			}
		}
	}
}