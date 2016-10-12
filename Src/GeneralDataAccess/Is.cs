#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Is.cs
// 文件功能描述：用于构建 FilterInfo 的辅助类。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110326
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

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于构建 FilterInfo 的辅助类。
	/// </summary>
	public abstract class Is
	{
		#region 操作

		#region 构造 FilterInfoExpression

		/// <summary>
		/// “非”操作符。
		/// </summary>
		public static FilterInfoExpression Not
		{
			get { return FilterInfoExpression.Not; }
		}

		#endregion

		#region Null

		/// <summary>
		/// 获取 IS NULL 过滤器。
		/// </summary>
		public static FilterInfo Null
		{
			get { return new NullFilterInfo(); }
		}

		/// <summary>
		/// 获取列为 NULL 或空字符串的过滤器。
		/// </summary>
		public static FilterInfo NullOrEmpty
		{
			get { return new NullOrEmptyFilterInfo(); }
		}

		#endregion

		#region EqualTo

		/// <summary>
		/// 相等过滤器。
		/// </summary>
		/// <param name="propertyValue">要比较的属性值。</param>
		/// <returns>过滤器信息。</returns>
		public static FilterInfo EqualTo(Object propertyValue)
		{
			return new EqualToFilterInfo(propertyValue);
		}

		#endregion

		#region True

		/// <summary>
		/// True 过滤器。
		/// </summary>
		public static FilterInfo True
		{
			get { return EqualTo(true); }
		}

		#endregion

		#region False

		/// <summary>
		/// False 过滤器。
		/// </summary>
		public static FilterInfo False
		{
			get { return EqualTo(false); }
		}

		#endregion

		#region GreaterThan

		/// <summary>
		/// 获得大于过滤器。
		/// </summary>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>大于过滤器。</returns>
		public static FilterInfo GreaterThan(Object propertyValue)
		{
			return new GreaterThanFilterInfo(propertyValue);
		}

		#endregion

		#region GreaterThanOrEqualTo

		/// <summary>
		/// 大于或等于过滤器。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>过滤器信息。</returns>
		public static FilterInfo GreaterThanOrEqualTo(Object propertyValue)
		{
			return new GreaterThanOrEqualToFilterInfo(propertyValue);
		}

		/// <summary>
		/// 大于或等于过滤器，与 GreaterThanOrEqualTo 相同。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>过滤器信息。</returns>
		public static FilterInfo AtLeast(Object propertyValue)
		{
			return GreaterThanOrEqualTo(propertyValue);
		}

		#endregion

		#region LessThan

		/// <summary>
		/// 小于过滤器。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>小于过滤器。</returns>
		public static FilterInfo LessThan(Object propertyValue)
		{
			return new LessThanFilterInfo(propertyValue);
		}

		#endregion

		#region LessThanOrEqualTo

		/// <summary>
		/// 小于或等于过滤器。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>小于或等于过滤器。</returns>
		public static FilterInfo LessThanOrEqualTo(Object propertyValue)
		{
			return new LessThanOrEqualToFilterInfo(propertyValue);
		}

		/// <summary>
		/// 小于或等于过滤器，与 LessThanOrEqualTo 相同。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>小于或等于过滤器。</returns>
		public static FilterInfo AtMost(Object propertyValue)
		{
			return LessThanOrEqualTo(propertyValue);
		}

		#endregion

		#region Between

		/// <summary>
		/// BETWEEN 过滤器。
		/// </summary>
		/// <param name="from">起始点。</param>
		/// <param name="to">终结点。</param>
		/// <returns>BETWEEN 过滤器。</returns>
		public static FilterInfo Between(Object from, Object to)
		{
			return new BetweenFilterInfo(from, to);
		}

		#endregion

		#region Like

		/// <summary>
		/// LIKE 过滤器。
		/// </summary>
		/// <param name="patternText">模式文本。</param>
		/// <returns>LIKE 过滤器。</returns>
		public static FilterInfo Like(String patternText)
		{
			return new LikeFilterInfo(patternText);
		}

		/// <summary>
		/// LIKE 过滤器。
		/// </summary>
		/// <param name="patternText">模式文本。</param>
		/// <param name="escapeChar">转义字符。</param>
		/// <returns>LIKE 过滤器。</returns>
		public static FilterInfo Like(String patternText, Char escapeChar)
		{
			if (patternText == null)
			{
				patternText = String.Empty;
			}

			if ((escapeChar != 0) && patternText.Contains(escapeChar.ToString()))
			{
				return new LikeFilterInfo(patternText, escapeChar);
			}
			else
			{
				return new LikeFilterInfo(patternText);
			}
		}

		/// <summary>
		/// 包含给定的文本。
		/// </summary>
		/// <param name="text">要包含的文本。</param>
		/// <returns>当前 FilterExpression 实例。</returns>
		public static FilterInfo Containing(String text)
		{
			return Like(String.Format("%{0}%", Filter.EscapeFuzzyText(text)), Filter.DEFAULT_FUZZY_ESCAPE_CHAR);
		}

		/// <summary>
		/// 以给定的文本开头。
		/// </summary>
		/// <param name="text">给定的文本。</param>
		/// <returns>当前 FilterExpression 实例。</returns>
		public static FilterInfo StartingWith(String text)
		{
			return Like(String.Format("{0}%", Filter.EscapeFuzzyText(text)), Filter.DEFAULT_FUZZY_ESCAPE_CHAR);
		}

		/// <summary>
		/// 以给定的文本结尾。
		/// </summary>
		/// <param name="text">给定的文本。</param>
		/// <returns>当前 FilterExpression 实例。</returns>
		public static FilterInfo EndingWith(String text)
		{
			return Like(String.Format("%{0}", Filter.EscapeFuzzyText(text)), Filter.DEFAULT_FUZZY_ESCAPE_CHAR);
		}

		#endregion

		#region IN

		/// <summary>
		/// IN 过滤器，使用值列表。
		/// </summary>
		/// <param name="discreteValues">值列表。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(params Object[] discreteValues)
		{
			return new InFilterInfo(discreteValues);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(SelectExpression select)
		{
			return InValues(select, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(SelectExpression select, Boolean distinct)
		{
			return InValues(select, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(SelectExpression select, Filter whereFilter)
		{
			return InValues(select, whereFilter, (Filter)null, false);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(SelectExpression select, Filter whereFilter, Boolean distinct)
		{
			return InValues(select, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(SelectExpression select, Filter whereFilter, Filter havingFilter)
		{
			return InValues(select, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(SelectExpression select, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region 前置断言

			Debug.Assert(
					(select != null)
					&& select.HasSelectors
					&& (select.Selectors.Length == 1)
					&& (select.Selector.SelectMode == PropertySelectMode.Property),
					"选择表达式参数 select 必须不能为空，有且仅有一个选择器，且选择模式为 Property。"
				);

			#endregion

			return InValues(select.Selector.PropertyChain, whereFilter, havingFilter, distinct);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(IPropertyChain property)
		{
			return InValues(property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(IPropertyChain property, Boolean distinct)
		{
			return InValues(property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(IPropertyChain property, Filter whereFilter)
		{
			return InValues(property, whereFilter, (Filter)null, false);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return InValues(property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return InValues(property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>IN 过滤器。</returns>
		public static FilterInfo InValues(IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region 前置断言

			Debug.Assert(property != null, "属性参数不能为空。");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new InFilterInfo(builder);
		}

		#endregion

		#endregion
	}
}