#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Filter.cs
// 文件功能描述：表示滤条件。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008151129
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示滤条件。
	/// </summary>
	[Serializable]
	public abstract class Filter : ICloneable
	{
		#region 静态成员

		/// <summary>
		/// 编译过滤器，获得条件表达式和查询参数。
		/// </summary>
		/// <param name="context">编译环境。</param>
		/// <param name="filter">过滤器。</param>
		/// <returns>过滤器编译结果对象（封装了条件表达式和查询参数集合）。</returns>
		public static FilterCompilationResult Compile(FilterCompilationContext context, Filter filter)
		{
			if (filter == null)
			{
				return null;
			}

			filter.Compile(context);

			StringBuilder output = new StringBuilder();

			filter.Generate(output);

			String whereClause = output.ToString();
			QueryParameter[] parameters = context.Parameters;

			FilterCompilationResult result = new FilterCompilationResult(whereClause, parameters);

			return result;
		}

		/// <summary>
		/// 转义模糊匹配文本（在构造匹配模式前），使用反斜杠（\）做转义符，通配符集合为：_ %。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <returns>通配符被转义的文本。</returns>
		public static String EscapeFuzzyText(String text)
		{
			return EscapeFuzzyText(text, DEFAULT_FUZZY_ESCAPE_CHAR);
		}

		/// <summary>
		/// 转义模糊匹配文本（在构造匹配模式前），通配符集合为：_ %。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <param name="escpaeChar"></param>
		/// <returns>通配符被转义的文本。</returns>
		public static String EscapeFuzzyText(String text, Char escpaeChar)
		{
			return EscapeFuzzyText(text, escpaeChar, new Char[] { '%', '_' });
		}

		/// <summary>
		/// 转义模糊匹配文本（在构造匹配模式前）。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <param name="escapeChar">转义字符，不能为 nil。</param>
		/// <param name="wildcards">通配符集合。</param>
		/// <returns>如果至少转义了一个字符，则返回 true，否则返回 false。</returns>
		public static String EscapeFuzzyText(String text, Char escapeChar, Char[] wildcards)
		{
			Boolean hasWildcards;

			return EscapeFuzzyText(text, escapeChar, wildcards, out hasWildcards);
		}

		/// <summary>
		/// 转义模糊匹配文本（在构造匹配模式前）。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <param name="escapeChar">转义字符，不能为 nil。</param>
		/// <param name="wildcards">通配符集合。</param>
		/// <param name="hasWildcards">返回值，指示文本中是否包含通配符。</param>
		/// <returns>通配符被转义的文本。</returns>
		public static String EscapeFuzzyText(String text, Char escapeChar, Char[] wildcards, out Boolean hasWildcards)
		{
			if (String.IsNullOrEmpty(text))
			{
				hasWildcards = false;

				return text;
			}

			#region 前置断言

			Debug.Assert(escapeChar != 0, "模糊匹配表达式的转义字符不能为 nil。");
			Debug.Assert((wildcards != null) && (wildcards.Length != 0), "通配符字符集不能为空或空列表。");

			#endregion

			StringBuilder buffer = new StringBuilder();

			foreach (Char c in wildcards)
			{
				buffer.Append(Regex.Escape(c.ToString()));
			}

			buffer.Append(Regex.Escape(escapeChar.ToString()));

			buffer.Insert(0, "[");
			buffer.Append("]");

			String pattern = buffer.ToString();

			Regex ex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			String escapeText = ex.Replace(
					text,
					delegate(Match m)
					{
						return escapeChar + m.Value;
					}
				);

			hasWildcards = ex.IsMatch(text);

			return escapeText;
		}

		#region 工厂方法

		#region 通用方法

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="info">过滤条件。</param>
		/// <returns>创建好的过滤器。</returns>
		public static Filter Create(String propertyName, FilterInfo info)
		{
			return Create(null, propertyName, info);
		}

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="entityPropertyName">外部引用实体属性名称。</param>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="info">过滤条件。</param>
		/// <returns>创建好的过滤器。</returns>
		public static Filter Create(String entityPropertyName, String propertyName, FilterInfo info)
		{
			ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);

			FilterFactory factory = new FilterFactory(colLocator.PropertyPath, info);

			Filter result = factory.Filter;

			return result;
		}

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <param name="info">过滤条件信息（比较操作与比较值）。</param>
		/// <returns>创建好的过滤器。</returns>
		public static Filter Create(IPropertyChain chain, FilterInfo info)
		{
			return Create(chain.PropertyPath, info);
		}

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <param name="info">过滤条件信息（比较操作与比较值）。</param>
		/// <returns>创建好的过滤器。</returns>
		public static Filter Create(IPropertyChainBuilder builder, FilterInfo info)
		{
			return Create(builder.Build(), info);
		}

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="info">过滤条件信息（比较操作与比较值）。</param>
		/// <returns>创建好的过滤器。</returns>
		public static Filter Create(IList<String> propertyPath, FilterInfo info)
		{
			FilterFactory factory = new FilterFactory(propertyPath, info);

			Filter result = factory.Filter;

			return result;
		}

		#endregion

		#region EqualsFilter

		/// <summary>
		/// 创建相等过滤器，设置属性名称和属性值，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的相等过滤器。</returns>
		public static Filter CreateEqualsFilter(String propertyName, Object propertyValue)
		{
			return new EqualsFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// 创建相等过滤器，设置实体属性名称和实体中值属性名称以及属性值，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的相等过滤器。</returns>
		public static Filter CreateEqualsFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new EqualsFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// 创建相等过滤器，设置属性路径和属性值，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的相等过滤器。</returns>
		public static Filter CreateEqualsFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new EqualsFilter(propertyPath, propertyValue);
		}

		#endregion

		#region NotEqualFilter

		/// <summary>
		/// 创建不相等过滤器，设置属性名称和属性值，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的不相等过滤器</returns>
		public static Filter CreateNotEqualFilter(String propertyName, Object propertyValue)
		{
			return new NotEqualFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// 创建不相等过滤器，设置实体属性名称和实体中值属性名称以及属性值，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的不相等过滤器</returns>
		public static Filter CreateNotEqualFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new NotEqualFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// 创建不相等过滤器，设置属性路径和属性值，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的不相等过滤器</returns>
		public static Filter CreateNotEqualFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new NotEqualFilter(propertyPath, propertyValue);
		}

		#endregion

		#region IsNullFilter

		/// <summary>
		/// 创建字段为空（IS NULL）过滤器，设置属性名称和属性值，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>创建好的字段为空（IS NULL）过滤器。</returns>
		public static Filter CreateIsNullFilter(String propertyName)
		{
			return new IsNullFilter(propertyName);
		}

		/// <summary>
		/// 创建字段为空（IS NULL）过滤器，设置实体属性名称和实体中值属性名称以及属性值，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <returns>创建好的字段为空（IS NULL）过滤器。</returns>
		public static Filter CreateIsNullFilter(String entityPropertyName, String propertyName)
		{
			return new IsNullFilter(entityPropertyName, propertyName);
		}

		/// <summary>
		/// 创建字段为空（IS NULL）过滤器，设置实体属性路径和属性值，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>创建好的字段为空（IS NULL）过滤器。</returns>
		public static Filter CreateIsNullFilter(IList<String> propertyPath)
		{
			return new IsNullFilter(propertyPath);
		}

		#endregion

		#region IsNotNullFilter

		/// <summary>
		/// 创建非空过滤条件（IS NOT NULL），设置属性名称和属性值，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>创建好的非空过滤条件（IS NOT NULL）。</returns>
		public static Filter CreateIsNotNullFilter(String propertyName)
		{
			return new IsNotNullFilter(propertyName);
		}

		/// <summary>
		/// 创建非空过滤条件（IS NOT NULL），设置实体属性名称和实体中值属性名称以及属性值，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <returns>创建好的非空过滤条件（IS NOT NULL）。</returns>
		public static Filter CreateIsNotNullFilter(String entityPropertyName, String propertyName)
		{
			return new IsNotNullFilter(entityPropertyName, propertyName);
		}

		/// <summary>
		/// 创建非空过滤条件（IS NOT NULL），设置属性路径和属性值，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>创建好的非空过滤条件（IS NOT NULL）。</returns>
		public static Filter CreateIsNotNullFilter(IList<String> propertyPath)
		{
			return new IsNotNullFilter(propertyPath);
		}

		#endregion

		#region InFilter

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称和值集合，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="discreteValues">属性值集合。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String propertyName, Object[] discreteValues)
		{
			return new InFilter(propertyName, discreteValues);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称、值属性名称和值集合，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="discreteValues">值集合。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, Object[] discreteValues)
		{
			return new InFilter(entityPropertyName, propertyName, discreteValues);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性路径和值集合，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="discreteValues">属性值集合。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, Object[] discreteValues)
		{
			return new InFilter(propertyPath, discreteValues);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property)
		{
			return CreateInFilter(propertyName, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称、值属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property)
		{
			return CreateInFilter(entityPropertyName, propertyName, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性路径和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property)
		{
			return CreateInFilter(propertyPath, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property, Boolean distinct)
		{
			return CreateInFilter(propertyName, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称、值属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Boolean distinct)
		{
			return CreateInFilter(entityPropertyName, propertyName, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性路径和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property, Boolean distinct)
		{
			return CreateInFilter(propertyPath, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateInFilter(propertyName, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称、值属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateInFilter(entityPropertyName, propertyName, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性路径和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateInFilter(propertyPath, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateInFilter(propertyName, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称、值属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateInFilter(entityPropertyName, propertyName, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性路径和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateInFilter(propertyPath, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region 前置断言

			Debug.Assert(property != null, "属性参数不能为空。");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new InFilter(propertyName, builder);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性名称、值属性名称和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region 前置断言

			Debug.Assert(property != null, "属性参数不能为空。");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new InFilter(entityPropertyName, propertyName, builder);
		}

		/// <summary>
		/// 创建 IN 过滤器（IN (...)），设置属性路径和列表查询，支持外部引用属性，使用查询列表。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 IN 过滤器（IN (...)）。</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region 前置断言

			Debug.Assert(property != null, "属性参数不能为空。");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new InFilter(propertyPath, builder);
		}

		#endregion

		#region NotInFilter

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性名称和值集合，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="discreteValues">属性值集合。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String propertyName, Object[] discreteValues)
		{
			return new NotInFilter(propertyName, discreteValues);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置实体属性名称、值属性名称和值集合，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="discreteValues">值集合。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, Object[] discreteValues)
		{
			return new NotInFilter(entityPropertyName, propertyName, discreteValues);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性路径和值集合，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="discreteValues">属性值集合。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, Object[] discreteValues)
		{
			return new NotInFilter(propertyPath, discreteValues);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property)
		{
			return CreateNotInFilter(propertyName, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置实体属性名称、值属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property)
		{
			return CreateNotInFilter(entityPropertyName, propertyName, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性路径和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property)
		{
			return CreateNotInFilter(propertyPath, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property, Boolean distinct)
		{
			return CreateNotInFilter(propertyName, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置实体属性名称、值属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Boolean distinct)
		{
			return CreateNotInFilter(entityPropertyName, propertyName, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性路径和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property, Boolean distinct)
		{
			return CreateNotInFilter(propertyPath, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateNotInFilter(propertyName, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置实体属性名称、值属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateNotInFilter(entityPropertyName, propertyName, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性路径和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateNotInFilter(propertyPath, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateNotInFilter(propertyName, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置实体属性名称、值属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateNotInFilter(entityPropertyName, propertyName, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性路径和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateNotInFilter(propertyPath, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region 前置断言

			Debug.Assert(property != null, "属性参数不能为空。");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new NotInFilter(propertyName, builder);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置实体属性名称、值属性名称和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region 前置断言

			Debug.Assert(property != null, "属性参数不能为空。");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new NotInFilter(entityPropertyName, propertyName, builder);
		}

		/// <summary>
		/// 创建 NOT IN 过滤器（NOT IN (...)），设置属性路径和列表查询，支持外部引用属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>创建好的 NOT IN 过滤器（NOT IN (...)）。</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region 前置断言

			Debug.Assert(property != null, "属性参数不能为空。");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new NotInFilter(propertyPath, builder);
		}

		#endregion

		#region BetweenFilter

		/// <summary>
		/// 创建 BETWEEN 过滤器，设置属性值和左、右边界的属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		/// <returns>创建好的 BETWEEN 过滤器。</returns>
		public static Filter CreateBetweenFilter(String propertyName, Object from, Object to)
		{
			return new BetweenFilter(propertyName, from, to);
		}

		/// <summary>
		/// 创建 BETWEEN 过滤器，设置实体属性名称、值属性名称和左、右边界的属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		/// <returns>创建好的 BETWEEN 过滤器。</returns>
		public static Filter CreateBetweenFilter(String entityPropertyName, String propertyName, Object from, Object to)
		{
			return new BetweenFilter(entityPropertyName, propertyName, from, to);
		}

		/// <summary>
		/// 创建 BETWEEN 过滤器，设置属性路径和左、右边界的属性值。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		/// <returns>创建好的 BETWEEN 过滤器。</returns>
		public static Filter CreateBetweenFilter(IList<String> propertyPath, Object from, Object to)
		{
			return new BetweenFilter(propertyPath, from, to);
		}

		#endregion

		#region NotBetweenFilter

		/// <summary>
		/// 创建 NOT BETWEEN 过滤器，设置属性名称和左、右边界的属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		/// <returns>创建好的 NOT BETWEEN 过滤器。</returns>
		public static Filter CreateNotBetweenFilter(String propertyName, Object from, Object to)
		{
			return new NotBetweenFilter(propertyName, from, to);
		}

		/// <summary>
		/// 创建 NOT BETWEEN 过滤器，设置实体属性名称、值属性名称和左、右边界的属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		/// <returns>创建好的 NOT BETWEEN 过滤器。</returns>
		public static Filter CreateNotBetweenFilter(String entityPropertyName, String propertyName, Object from, Object to)
		{
			return new NotBetweenFilter(entityPropertyName, propertyName, from, to);
		}

		/// <summary>
		/// 创建 NOT BETWEEN 过滤器，设置属性路径和左、右边界的属性值。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		/// <returns>创建好的 NOT BETWEEN 过滤器。</returns>
		public static Filter CreateNotBetweenFilter(IList<String> propertyPath, Object from, Object to)
		{
			return new NotBetweenFilter(propertyPath, from, to);
		}

		#endregion

		#region LikeFilter

		/// <summary>
		/// 创建 LIKE 过滤器，设置属性名称和匹配模式。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="patternText">和匹配模式，模糊查询表达式。</param>
		/// <returns>创建好的 LIKE 过滤器。</returns>
		public static Filter CreateLikeFilter(String propertyName, String patternText)
		{
			return new LikeFilter(propertyName, patternText);
		}

		/// <summary>
		/// 创建 LIKE 过滤器，设置实体属性名称和实体中值属性名称以及和匹配模式。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="patternText">和匹配模式，模糊查询表达式。</param>
		/// <returns>创建好的 LIKE 过滤器。</returns>
		public static Filter CreateLikeFilter(String entityPropertyName, String propertyName, String patternText)
		{
			return new LikeFilter(entityPropertyName, propertyName, patternText);
		}

		/// <summary>
		/// 创建 LIKE 过滤器，设置属性路径和和匹配模式。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="patternText">和匹配模式，模糊查询表达式。</param>
		/// <returns>创建好的 LIKE 过滤器。</returns>
		public static Filter CreateLikeFilter(IList<String> propertyPath, String patternText)
		{
			return new LikeFilter(propertyPath, patternText);
		}

		/// <summary>
		/// 创建 LIKE 过滤器，设置属性名称和匹配模式。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="patternText">和匹配模式，模糊查询表达式。</param>
		/// <param name="escapeChar">转义字符。</param>
		/// <returns>创建好的 LIKE 过滤器。</returns>
		public static Filter CreateLikeFilter(String propertyName, String patternText, Char escapeChar)
		{
			return new LikeFilter(propertyName, patternText, escapeChar);
		}

		/// <summary>
		/// 创建 LIKE 过滤器，设置实体属性名称和实体中值属性名称以及和匹配模式。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="patternText">和匹配模式，模糊查询表达式。</param>
		/// <param name="escapeChar">转义字符。</param>
		/// <returns>创建好的 LIKE 过滤器。</returns>
		public static Filter CreateLikeFilter(String entityPropertyName, String propertyName, String patternText, Char escapeChar)
		{
			return new LikeFilter(entityPropertyName, propertyName, patternText, escapeChar);
		}

		/// <summary>
		/// 创建 LIKE 过滤器，设置属性路径和和匹配模式。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="patternText">和匹配模式，模糊查询表达式。</param>
		/// <param name="escapeChar">转义字符。</param>
		/// <returns>创建好的 LIKE 过滤器。</returns>
		public static Filter CreateLikeFilter(IList<String> propertyPath, String patternText, Char escapeChar)
		{
			return new LikeFilter(propertyPath, patternText, escapeChar);
		}

		#endregion

		#region NotLikeFilter

		/// <summary>
		/// 创建 NOT LIKE 过滤器，设置属性名称和匹配模式。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		/// <returns>创建好的 NOT LIKE 过滤器。</returns>
		public static Filter CreateNotLikeFilter(String propertyName, String patternText)
		{
			return new NotLikeFilter(propertyName, patternText);
		}

		/// <summary>
		/// 创建 NOT LIKE 过滤器，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="patternText">属性值，模糊查询表达式。</param>
		/// <returns>创建好的 NOT LIKE 过滤器。</returns>
		public static Filter CreateNotLikeFilter(String entityPropertyName, String propertyName, String patternText)
		{
			return new NotLikeFilter(entityPropertyName, propertyName, patternText);
		}

		/// <summary>
		/// 创建 NOT LIKE 过滤器，设置属性路径和匹配模式。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		/// <returns>创建好的 NOT LIKE 过滤器。</returns>
		public static Filter CreateNotLikeFilter(IList<String> propertyPath, String patternText)
		{
			return new NotLikeFilter(propertyPath, patternText);
		}

		/// <summary>
		/// 创建 NOT LIKE 过滤器，设置属性名称和匹配模式。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		/// <param name="escapeChar">转义字符。</param>
		/// <returns>创建好的 NOT LIKE 过滤器。</returns>
		public static Filter CreateNotLikeFilter(String propertyName, String patternText, Char escapeChar)
		{
			return new NotLikeFilter(propertyName, patternText, escapeChar);
		}

		/// <summary>
		/// 创建 NOT LIKE 过滤器，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="patternText">属性值，模糊查询表达式。</param>
		/// <param name="escapeChar">转义字符。</param>
		/// <returns>创建好的 NOT LIKE 过滤器。</returns>
		public static Filter CreateNotLikeFilter(String entityPropertyName, String propertyName, String patternText, Char escapeChar)
		{
			return new NotLikeFilter(entityPropertyName, propertyName, patternText, escapeChar);
		}

		/// <summary>
		/// 创建 NOT LIKE 过滤器，设置属性路径和匹配模式。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="patternText">匹配模式，模糊查询表达式。</param>
		/// <param name="escapeChar">转义字符。</param>
		/// <returns>创建好的 NOT LIKE 过滤器。</returns>
		public static Filter CreateNotLikeFilter(IList<String> propertyPath, String patternText, Char escapeChar)
		{
			return new NotLikeFilter(propertyPath, patternText, escapeChar);
		}

		#endregion

		#region GreaterThanFilter

		/// <summary>
		/// 创建大于过滤器。，设置属性名称和属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的大于过滤器。</returns>
		public static Filter CreateGreaterThanFilter(String propertyName, Object propertyValue)
		{
			return new GreaterThanFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// 创建大于过滤器，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的大于过滤器。</returns>
		public static Filter CreateGreaterThanFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new GreaterThanFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// 创建大于过滤器。，设置属性路径和属性值。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的大于过滤器。</returns>
		public static Filter CreateGreaterThanFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new GreaterThanFilter(propertyPath, propertyValue);
		}

		#endregion

		#region LessThanFilter

		/// <summary>
		/// 创建小于过滤器，设置属性名称和属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的小于过滤器。</returns>
		public static Filter CreateLessThanFilter(String propertyName, Object propertyValue)
		{
			return new LessThanFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// 创建小于过滤器，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的小于过滤器。</returns>
		public static Filter CreateLessThanFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new LessThanFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// 创建小于过滤器，设置属性名称和属性值。
		/// </summary>
		/// <param name="propertyPath">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的小于过滤器。</returns>
		public static Filter CreateLessThanFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new LessThanFilter(propertyPath, propertyValue);
		}

		#endregion

		#region GreaterThanEqualsFilter

		/// <summary>
		/// 创建大于等于过滤器，设置属性名称和属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的大于等于过滤器。</returns>
		public static Filter CreateGreaterThanEqualsFilter(String propertyName, Object propertyValue)
		{
			return new GreaterThanEqualsFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// 创建大于等于过滤器，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的大于等于过滤器。</returns>
		public static Filter CreateGreaterThanEqualsFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new GreaterThanEqualsFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// 创建大于等于过滤器，设置属性路径和属性值。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的大于等于过滤器。</returns>
		public static Filter CreateGreaterThanEqualsFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new GreaterThanEqualsFilter(propertyPath, propertyValue);
		}

		#endregion

		#region LessThanEqualsFilter

		/// <summary>
		/// 创建小于等于过滤器，设置属性名称和属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的小于等于过滤器。</returns>
		public static Filter CreateLessThanEqualsFilter(String propertyName, Object propertyValue)
		{
			return new LessThanEqualsFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// 创建小于等于过滤器，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的小于等于过滤器。</returns>
		public static Filter CreateLessThanEqualsFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new LessThanEqualsFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// 创建小于等于过滤器，设置属性路径和属性值。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>创建好的小于等于过滤器。</returns>
		public static Filter CreateLessThanEqualsFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new LessThanEqualsFilter(propertyPath, propertyValue);
		}

		#endregion

		#endregion

		#endregion

		#region 私有字段

		private ColumnLocator m_columnLocator;
		private readonly Object[] m_values;
		private readonly QueryListBuilder m_queryListBuilder;
		private String m_queryListSqlStatement;

		private String m_parameterPrefix;

		// 对应于值属性
		private QueryParameter[] m_parameters;
		private String m_columnFullName;

		// 对应于实体属性
		private Boolean m_isEntityFilter;
		private QueryParameterCollection[] m_entityParameters;
		private String[] m_entityColumnFullNames;

		private readonly LogicOperator m_logicOperator;

		private Collection<Filter> m_filters;

		private Boolean m_negative;
		private Boolean m_compiled;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		protected Filter()
		{
		}

		/// <summary>
		/// 构造函数，设置要过滤的属性名称。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		protected Filter(String propertyName)
		{
			m_columnLocator = new ColumnLocator(propertyName);
		}

		/// <summary>
		/// 构造函数，设置实体属性名称和该实体中的值属性名称。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称，如果为 null，则表示为当前实体。</param>
		/// <param name="propertyName">该实体中的值属性名称</param>
		protected Filter(String entityPropertyName, String propertyName)
		{
			m_columnLocator = new ColumnLocator(entityPropertyName, propertyName);
		}

		/// <summary>
		/// 构造函数，设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		protected Filter(IList<String> propertyPath)
		{
			m_columnLocator = new ColumnLocator(propertyPath);
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="propertyValues">属性值集合。</param>
		protected Filter(String propertyName, Object[] propertyValues)
		{
			m_columnLocator = new ColumnLocator(propertyName);
			m_values = propertyValues;
		}

		/// <summary>
		/// 构造函数，设置实体属性名称和该实体属性中的值属性名称。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="propertyValues">属性值集合。</param>
		protected Filter(String entityPropertyName, String propertyName, Object[] propertyValues)
		{
			m_columnLocator = new ColumnLocator(entityPropertyName, propertyName);
			m_values = propertyValues;
		}

		/// <summary>
		/// 构造函数，设置属性路径和属性值集合。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="propertyValues">属性值集合。</param>
		protected Filter(IList<String> propertyPath, Object[] propertyValues)
		{
			m_columnLocator = new ColumnLocator(propertyPath);
			m_values = propertyValues;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="builder">查询列表生成器。</param>
		internal Filter(String propertyName, QueryListBuilder builder)
		{
			m_columnLocator = new ColumnLocator(propertyName);
			m_queryListBuilder = builder;
		}

		/// <summary>
		/// 构造函数，设置实体属性名称和该实体属性中的值属性名称。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="builder">查询列表生成器。</param>
		internal Filter(String entityPropertyName, String propertyName, QueryListBuilder builder)
		{
			m_columnLocator = new ColumnLocator(entityPropertyName, propertyName);
			m_queryListBuilder = builder;
		}

		/// <summary>
		/// 构造函数，设置属性路径和属性值集合。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="builder">查询列表生成器。</param>
		internal Filter(IList<String> propertyPath, QueryListBuilder builder)
		{
			m_columnLocator = new ColumnLocator(propertyPath);
			m_queryListBuilder = builder;
		}

		/// <summary>
		/// 构造函数，设置逻辑连接操作符。
		/// </summary>
		/// <param name="logicOperator">逻辑操作符，AND 和 OR。</param>
		protected Filter(LogicOperator logicOperator)
		{
			m_logicOperator = logicOperator;
		}

		#endregion

		#region 常量

		/// <summary>
		/// “ AND ”。
		/// </summary>
		public const String AND = " AND ";

		/// <summary>
		/// “ OR ”。
		/// </summary>
		public const String OR = " OR ";

		/// <summary>
		/// “NOT ”。
		/// </summary>
		public const String NOT = "NOT ";

		/// <summary>
		/// “ BETWEEN ”。
		/// </summary>
		public const String BETWEEN = " BETWEEN ";

		/// <summary>
		/// “NOT BETWEEN ”。
		/// </summary>
		public const String NOT_BETWEEN = " NOT BETWEEN ";

		/// <summary>
		/// “ LIKE ”。
		/// </summary>
		public const String LIKE = " LIKE ";

		/// <summary>
		/// “ NOT LIKE ”。
		/// </summary>
		public const String NOT_LIKE = " NOT LIKE ";

		/// <summary>
		/// “ IN ”。
		/// </summary>
		public const String IN = " IN ";

		/// <summary>
		/// “ NOT IN ”。
		/// </summary>
		public const String NOT_IN = " NOT IN ";

		/// <summary>
		/// “@”。
		/// </summary>
		public const String PARAMETER_PREFIX = "@";

		/// <summary>
		/// “(”。
		/// </summary>
		public const String LEFT_BRACKET = "(";

		/// <summary>
		/// “)”。
		/// </summary>
		public const String RIGHT_BRACKET = ")";

		/// <summary>
		/// “,”。
		/// </summary>
		public const String COMMA = ",";

		/// <summary>
		/// 空格。
		/// </summary>
		public const Char SPACE = '\x20';

		/// <summary>
		/// 默认的模糊匹配转义符“\”。
		/// </summary>
		public const Char DEFAULT_FUZZY_ESCAPE_CHAR = '\\';

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取一个值，该值指示当前过滤器是否要加 NOT 操作符。
		/// </summary>
		public Boolean Negative
		{
			get { return m_negative; }
		}

		/// <summary>
		/// 获取或设置参数前缀。
		/// </summary>
		public String ParameterPrefix
		{
			get
			{
				if (m_parameterPrefix == null)
				{
					m_parameterPrefix = PARAMETER_PREFIX;
				}

				return m_parameterPrefix;
			}

			protected set
			{
				if (value != null)
				{
					value = value.Trim();
				}

				if (!String.IsNullOrEmpty(value))
				{
					m_parameterPrefix = value;
				}
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取当前实例的浅表副本。
		/// </summary>
		/// <returns>当前实例的浅表副本。</returns>
		public virtual Filter Clone()
		{
			return (Filter)MemberwiseClone();
		}

		/// <summary>
		/// 连接过滤器，以构造复杂的过滤器。
		/// </summary>
		/// <param name="logicOperator">连接操作符，AND 和 OR 两种。</param>
		/// <param name="filters">要连接的过滤器数组。</param>
		/// <returns>构造好的新过滤器。</returns>
		public static Filter Combine(LogicOperator logicOperator, params Filter[] filters)
		{
			if (filters == null)
			{
				filters = new Filter[0];
			}

			// 移除为 null 的过滤器
			filters = Array.FindAll<Filter>(
					filters,
					delegate(Filter item)
					{
						return (item != null);
					}
				);

			if (filters.Length == 0)
			{
				return null;
			}
			else if (filters.Length == 1)
			{
				return filters[0];
			}

			CompositeFilter f = new CompositeFilter(logicOperator, filters);

			return f;
		}

		/// <summary>
		/// 对过滤器进行编译，
		/// </summary>
		/// <param name="context">用于提供编译支持的环境对象。</param>
		public virtual void Compile(FilterCompilationContext context)
		{
			ParameterPrefix = context.ParameterPrefix;

			// 如果列定位对象存在，要确定列的名称
			if (m_columnLocator != null)
			{
				m_isEntityFilter = context.IsEntityColumns(m_columnLocator);

				if (!IsEntityFilter)
				{
					m_columnFullName = context.GetColumnFullName(m_columnLocator);
				}
				else
				{
					m_entityColumnFullNames = context.GetEntityColumnFullNames(m_columnLocator);
				}
			}

			// 如果列定位对象存在，且值集合不为空，则生成查询参数
			if ((m_columnLocator != null) && (m_values != null) && (m_values.Length != 0))
			{
				if (!IsEntityFilter)
				{
					m_parameters = context.GenerateParameters(m_columnLocator, m_values);
				}
				else
				{
					m_entityParameters = context.GenerateEntityParameters(m_columnLocator, m_values);
				}
			}

			if (m_queryListBuilder != null)
			{
				m_queryListSqlStatement = m_queryListBuilder.Build(context);
			}

			// 如果有子过滤器集合，则遍历调用 Compile 方法
			if (HasFilters)
			{
				foreach (Filter f in Filters)
				{
					f.Compile(context);
				}
			}

			// 设置编译状态
			m_compiled = true;
		}

		/// <summary>
		/// 由参数名称构造查询参数。
		/// </summary>
		/// <param name="parameterName">参数名称。</param>
		/// <returns>构造好的查询参数。</returns>
		public String ComposeParameter(String parameterName)
		{
			if (String.IsNullOrEmpty(parameterName))
			{
				return parameterName;
			}

			return ParameterPrefix + parameterName;
		}

		/// <summary>
		/// 对当前实例进行深复制（条件的属性部分进行深复制，值部分（包括子查询列表）共享，因这部分是不可变的）。
		/// </summary>
		/// <returns>当前实例的深复制的副本。</returns>
		public Filter DeepClone()
		{
			Filter clone = (Filter)MemberwiseClone();

			if (HasFilters)
			{
				clone.m_filters = new Collection<Filter>();

				foreach (Filter filter in this.Filters)
				{
					clone.m_filters.Add(filter.DeepClone());
				}
			}

			return clone;
		}

		/// <summary>
		/// 扩展过滤器为面向子实体的过滤器。
		/// </summary>
		/// <param name="childrenPropertyName">子实体中指向父实体的属性名称。</param>
		/// <returns>扩展后新的过滤器。</returns>
		public Filter Extend(String childrenPropertyName)
		{
			return Extend(new String[] { childrenPropertyName });
		}

		/// <summary>
		/// 扩展过滤器为面向子实体的过滤器。
		/// </summary>
		/// <param name="propertyPath">子实体中指向父实体的属性路径。</param>
		/// <returns>扩展后新的过滤器。</returns>
		public Filter Extend(String[] propertyPath)
		{
			Filter childrenFilter = Clone();

			if (childrenFilter.m_columnLocator != null)
			{
				String[] childrenPropertyPath = new String[childrenFilter.m_columnLocator.PropertyPath.Length + propertyPath.Length];

				Array.Copy(
						propertyPath,
						childrenPropertyPath,
						propertyPath.Length
					);
				Array.Copy(
						childrenFilter.m_columnLocator.PropertyPath,
						0,
						childrenPropertyPath,
						propertyPath.Length,
						childrenFilter.m_columnLocator.PropertyPath.Length
					);

				childrenFilter.m_columnLocator = new ColumnLocator(childrenPropertyPath);
			}

			if (this.HasFilters)
			{
				childrenFilter.m_filters = new Collection<Filter>();

				for (Int32 i = 0; i < this.Filters.Count; i++)
				{
					childrenFilter.m_filters.Add(this.Filters[i].Extend(propertyPath));
				}
			}

			return childrenFilter;
		}

		/// <summary>
		/// 获取所有的列定位符。
		/// </summary>
		/// <returns>列定位符集合。</returns>
		public ColumnLocator[] GetAllColumnLocators()
		{
			List<ColumnLocator> allColumnLocators = new List<ColumnLocator>();

			RetrieveAllColumnLoactors(this, allColumnLocators);

			return allColumnLocators.ToArray();
		}

		/// <summary>
		/// 获取所有的属性选择器（指示仅加载实体）。
		/// </summary>
		/// <returns>所有的属性选择器。</returns>
		public PropertySelector[] GetAllSelectors(Type entityType)
		{
			List<ColumnLocator> container = new List<ColumnLocator>();

			RetrieveAllColumnLoactors(this, container);

			List<IPropertyChain> allForeignReferences = new List<IPropertyChain>();

			foreach (ColumnLocator colLocator in container)
			{
				if (colLocator.PropertyPath.Length > 1)
				{
					IPropertyChain chain = colLocator.Create(entityType);

					// 前一个属性节点链必须重新创建，否则会导航到当前属性
					IPropertyChain reference = chain.Previous.Copy();

					if (!allForeignReferences.Contains(reference))
					{
						allForeignReferences.Add(reference);
					}
				}
			}

			List<PropertySelector> allSelectors = allForeignReferences.ConvertAll<PropertySelector>(
					delegate(IPropertyChain chain)
					{
						return PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain);
					}
				);

			return allSelectors.ToArray();
		}

		/// <summary>
		/// 获取所有的查询列表生成器。
		/// </summary>
		/// <returns>所有的查询列表生成器。</returns>
		internal QueryListBuilder[] GetAllQueryListBuilders()
		{
			List<QueryListBuilder> allBuilders = new List<QueryListBuilder>();

			RetrieveAllQueryListBuilders(this, allBuilders);

			return allBuilders.ToArray();
		}

		/// <summary>
		/// 向输入缓冲区写入条件表达式。
		/// </summary>
		/// <param name="output">输入缓冲区。</param>
		public virtual void Generate(StringBuilder output)
		{
			ThrowExceptionIfNotCompiled();

			if (HasFilters)
			{
				if (Negative)
				{
					output.Append(NOT);

					output.Append(LEFT_BRACKET);
				}

				for (Int32 i = 0; i < Filters.Count; i++)
				{
					if (i > 0)
					{
						output.Append(CombineWord);
					}

					output.Append(LEFT_BRACKET);

					Filters[i].Generate(output);

					output.Append(RIGHT_BRACKET);
				}

				if (Negative)
				{
					output.Append(RIGHT_BRACKET);
				}
			}
		}

		/// <summary>
		/// 对过滤器进行 NOT 逻辑操作，多次调用无效。
		/// </summary>
		public void Not()
		{
			m_negative = true;
		}

		#endregion

		#region 属性

		/// <summary>
		/// 获取或设置列的全名称。
		/// </summary>
		protected String ColumnFullName
		{
			get { return m_columnFullName; }
			set { m_columnFullName = value; }
		}

		/// <summary>
		/// 获取列定位符。
		/// </summary>
		protected ColumnLocator ColumnLocator
		{
			get { return m_columnLocator; }
		}

		/// <summary>
		/// 获取连接 SQL 关键字，AND 或 OR。
		/// </summary>
		protected String CombineWord
		{
			get { return (LogicOperator == LogicOperator.And) ? AND : OR; }
		}

		/// <summary>
		/// 获取一个值，该值指示当前过滤器是否已经编译。
		/// </summary>
		protected Boolean Compiled
		{
			get { return m_compiled; }
		}

		/// <summary>
		/// 获取过滤器集合。
		/// </summary>
		internal protected Collection<Filter> Filters
		{
			get
			{
				if (m_filters == null)
				{
					m_filters = new Collection<Filter>();
				}

				return m_filters;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示是否存在子过滤器。
		/// </summary>
		internal protected Boolean HasFilters
		{
			get { return (m_filters != null) && (m_filters.Count != 0); }
		}

		/// <summary>
		/// 获取连接操作符。
		/// </summary>
		internal protected LogicOperator LogicOperator
		{
			get { return m_logicOperator; }
		}

		/// <summary>
		/// 获取或设置查询参数。
		/// </summary>
		protected QueryParameter[] Parameters
		{
			get
			{
				if (m_parameters == null)
				{
					m_parameters = new QueryParameter[0];
				}

				return m_parameters;
			}

			set
			{
				m_parameters = value;
			}
		}

		/// <summary>
		/// 获取过滤器的值集合。
		/// </summary>
		protected Object[] Values
		{
			get { return m_values; }
		}

		/// <summary>
		/// 获取查询列表生成器，用于 IN 过滤条件。
		/// </summary>
		internal QueryListBuilder QueryListBuilder
		{
			get { return m_queryListBuilder; }
		}

		/// <summary>
		/// 获取查询列表 SQL 指令。
		/// </summary>
		public String QueryListSqlStatement
		{
			get { return m_queryListSqlStatement; }
		}

		/// <summary>
		/// 获取一个个值，该值指示是否为实体过滤器。
		/// </summary>
		protected Boolean IsEntityFilter
		{
			get { return m_isEntityFilter; }
		}

		/// <summary>
		/// 获取实体参数集合。
		/// </summary>
		protected QueryParameterCollection[] EntityParameters
		{
			get
			{
				if (m_entityParameters == null)
				{
					m_entityParameters = new QueryParameterCollection[0];
				}

				return m_entityParameters;
			}
		}

		/// <summary>
		/// 获取实体的列全名称集合。
		/// </summary>
		protected String[] EntityColumnFullNames
		{
			get
			{
				if (m_entityColumnFullNames == null)
				{
					m_entityColumnFullNames = new String[0];
				}

				return m_entityColumnFullNames;
			}
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 检查过滤器是否已经编译，如果没有编译，则抛出 InvalidOperationException 异常。
		/// </summary>
		protected void ThrowExceptionIfNotCompiled()
		{
			if (!Compiled)
			{
				throw new InvalidOperationException("过滤器还没有编译，不能进行输出。");
			}
		}

		#endregion

		#region ICloneable 成员

		/// <summary>
		/// 获得当前实例的浅表副本。
		/// </summary>
		/// <returns>当前实例的浅表副本。</returns>
		Object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 收集所有基本过滤器的属性定位符。
		/// </summary>
		/// <param name="filter">过滤器。</param>
		/// <param name="container">收集容器。</param>
		private static void RetrieveAllColumnLoactors(Filter filter, List<ColumnLocator> container)
		{
			if (filter.ColumnLocator != null)
			{
				container.Add(filter.ColumnLocator);
			}
			else if (filter.HasFilters)
			{
				foreach (Filter childFilter in filter.Filters)
				{
					RetrieveAllColumnLoactors(childFilter, container);
				}
			}
		}

		/// <summary>
		/// 收集所有的查询列表生成器。
		/// </summary>
		/// <param name="filter">过滤器。</param>
		/// <param name="container">收集容器。</param>
		private static void RetrieveAllQueryListBuilders(Filter filter, List<QueryListBuilder> container)
		{
			if (filter.QueryListBuilder != null)
			{
				container.Add(filter.QueryListBuilder);
			}
			else if (filter.HasFilters)
			{
				foreach (Filter childFilter in filter.Filters)
				{
					RetrieveAllQueryListBuilders(childFilter, container);
				}
			}
		}

		#endregion
	}
}