#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：LikeFilterInfo.cs
// 文件功能描述：LIKE 过滤器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110327
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
	/// LIKE 过滤器。
	/// </summary>
	[Serializable]
	internal sealed class LikeFilterInfo : FilterInfo
	{
		#region 私有字段

		private readonly Char m_escapeChar;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置模式文本。
		/// </summary>
		/// <param name="patternText">模式文本。</param>
		public LikeFilterInfo(String patternText)
			: base(new Object[] { patternText })
		{
		}

		/// <summary>
		/// 构造函数，设置是否执行“非”操作和模式文本。
		/// </summary>
		/// <param name="negative">指示是否执行“非”操作。</param>
		/// <param name="patternText">模式文本。</param>
		public LikeFilterInfo(Boolean negative, String patternText)
			: base(negative, new Object[] { patternText })
		{
		}

		/// <summary>
		/// 构造函数，设置模式文本。
		/// </summary>
		/// <param name="patternText">模式文本。</param>
		/// <param name="escapeChar">转义字符。</param>
		public LikeFilterInfo(String patternText, Char escapeChar)
			: base(new Object[] { patternText })
		{
			m_escapeChar = escapeChar;
		}

		/// <summary>
		/// 构造函数，设置是否执行“非”操作和模式文本。
		/// </summary>
		/// <param name="negative">指示是否执行“非”操作。</param>
		/// <param name="patternText">模式文本。</param>
		/// <param name="escapeChar">转义字符。</param>
		public LikeFilterInfo(Boolean negative, String patternText, Char escapeChar)
			: base(negative, new Object[] { patternText })
		{
			m_escapeChar = escapeChar;
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>创建好的过滤器。</returns>
		protected override Filter DoCreateFilter(IList<String> propertyPath)
		{
			return Filter.CreateLikeFilter(propertyPath, (String)Values[0], m_escapeChar);
		}

		#endregion
	}
}