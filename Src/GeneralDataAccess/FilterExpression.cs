#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterExpression.cs
// 文件功能描述：过滤器表达式，用于合成过滤器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110325
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 过滤器表达式，用于合成过滤器。
	/// </summary>
	[Serializable]
	public class FilterExpression
	{
		#region 私有字段

		private readonly FilterBuilder m_builder = new FilterBuilder();
		private FilterFactory m_current;

		private List<String> m_propertyPath;

		private FilterInfoExpression m_filterInfoExpression;

		private readonly List<ExpressionTokens> m_appendedTokens = new List<ExpressionTokens>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public FilterExpression()
		{
		}

		/// <summary>
		/// 构造函数，设置初始过滤器。
		/// </summary>
		/// <param name="initialFilter">初始过滤器。</param>
		public FilterExpression(Filter initialFilter)
		{
			m_current = new FilterFactory(initialFilter);
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 判断当前表达式是否可以解析。
		/// </summary>
		public Boolean IsResolvable
		{
			get
			{
				ExpressionTokens nextValidTokens = GetNextValidTokens();

				return ((nextValidTokens & ExpressionTokens.Finish) != 0);
			}
		}

		#region 操作符

		/// <summary>
		/// “非”操作符。
		/// </summary>
		public FilterExpression Not
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Not;

				OnProcessing(token);

				if (!IsBuildingFilter)
				{
					m_builder.Push(new NotOperator(FilterFactories));
				}
				else
				{
					m_filterInfoExpression = FilterInfoExpression.Not;
				}

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// 将随后直至下一个 With 或 EndWith 或直至末尾的操作视为一个操作单元（相当于算术表达式中的圆括号）。
		/// </summary>
		public FilterExpression With
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.With;

				OnProcessing(token);

				m_builder.Push(new WithOperator(FilterFactories));

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// 计算最近一个 With 单元。
		/// </summary>
		public FilterExpression EndWith
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.EndWith;

				OnProcessing(token);

				m_builder.EndWith();

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// “与”操作符。
		/// </summary>
		public FilterExpression And
		{
			get
			{
				if (CurrentToken == ExpressionTokens.Nothing)
				{
					Filter(Current.Filter);
				}

				ExpressionTokens token = ExpressionTokens.And;

				OnProcessing(token);

				m_builder.Push(new AndOperator(FilterFactories));

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// “或”操作符。
		/// </summary>
		public FilterExpression Or
		{
			get
			{
				if (CurrentToken == ExpressionTokens.Nothing)
				{
					Filter(Current.Filter);
				}

				ExpressionTokens token = ExpressionTokens.Or;

				OnProcessing(token);

				m_builder.Push(new OrOperator(FilterFactories));

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// 开始接收过滤条件。
		/// </summary>
		public FilterExpression Is
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Is;

				OnProcessing(token);

				OnProcessed(token);

				return this;
			}
		}

		#region FilterInfo

		/// <summary>
		/// IS NULL 过滤条件。
		/// </summary>
		public FilterExpression Null
		{
			get
			{
				OnFilterInfoProcessing();

				FilterInfo info = m_filterInfoExpression.Null;

				PushNewFilterFactoryOperator(info);

				OnFilterInfoProcessed();

				return this;
			}
		}

		/// <summary>
		/// 列为 NULL 或空字符串的过滤条件。
		/// </summary>
		public FilterExpression NullOrEmpty
		{
			get
			{
				OnFilterInfoProcessing();

				FilterInfo info = m_filterInfoExpression.NullOrEmpty;

				PushNewFilterFactoryOperator(info);

				OnFilterInfoProcessed();

				return this;
			}
		}

		#endregion

		#endregion

		#region FilterInfo

		/// <summary>
		/// True 过滤器。
		/// </summary>
		public FilterExpression True
		{
			get { return EqualTo(true); }
		}

		/// <summary>
		/// False 过滤器。
		/// </summary>
		public FilterExpression False
		{
			get { return EqualTo(false); }
		}

		/// <summary>
		/// Empty 过滤器（等于空字符串）。
		/// </summary>
		public FilterExpression Empty
		{
			get { return EqualTo(String.Empty); }
		}

		#endregion

		#endregion

		#region 公共方法

		#region 生成 FilterFactory

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="f">过滤器。</param>
		/// <returns>过滤器表达式。</returns>
		public FilterExpression Filter(Filter f)
		{
			FilterFactory factory = new FilterFactory(f);

			return Filter(factory);
		}

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		/// <returns>过滤器表达式。</returns>
		public FilterExpression Filter(String propertyName, FilterInfo filterInfo)
		{
			return Filter(null, propertyName, filterInfo);
		}

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="entityPropertyName">外部实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		/// <returns>过滤器表达式。</returns>
		public FilterExpression Filter(String entityPropertyName, String propertyName, FilterInfo filterInfo)
		{
			ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);

			FilterFactory factory = new FilterFactory(colLocator.PropertyPath, filterInfo);

			return Filter(factory);
		}

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		/// <returns>过滤器表达式。</returns>
		public FilterExpression Filter(IPropertyChain chain, FilterInfo filterInfo)
		{
			return Filter(chain.PropertyPath, filterInfo);
		}

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		/// <returns>过滤器表达式。</returns>
		public FilterExpression Filter(IList<String> propertyPath, FilterInfo filterInfo)
		{
			FilterFactory factory = new FilterFactory(propertyPath, filterInfo);

			return Filter(factory);
		}

		#endregion

		#region 生成过滤器

		/// <summary>
		/// 外部引用属性名称。
		/// </summary>
		/// <param name="entityPropertyName">外部引用属性名称。</param>
		/// <returns>当前实例。</returns>
		public FilterExpression ForeignRef(String entityPropertyName)
		{
			ExpressionTokens token = ExpressionTokens.ForeignRef;

			OnProcessing(token);

			PropertyPath.Add(entityPropertyName);
			m_filterInfoExpression = FilterInfoExpression.Yes;

			OnProcessed(token);

			return this;
		}

		/// <summary>
		/// 属性名称。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>当前实例。。</returns>
		public FilterExpression Property(String propertyName)
		{
			ExpressionTokens token = ExpressionTokens.Property;

			OnProcessing(token);

			PropertyPath.Add(propertyName);
			m_filterInfoExpression = FilterInfoExpression.Yes;

			OnProcessed(token);

			return this;
		}

		/// <summary>
		/// 设置属性链生成器。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>当前实例。</returns>
		public FilterExpression Property(IPropertyChainBuilder builder)
		{
			return Locator(builder);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>当前实例。</returns>
		public FilterExpression Property(IPropertyChain chain)
		{
			return Locator(chain);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>当前实例。</returns>
		public FilterExpression Property(IList<String> propertyPath)
		{
			return Locator(propertyPath);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>当前实例。</returns>
		public FilterExpression Locator(IPropertyChainBuilder builder)
		{
			return Locator(builder.Build());
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>当前实例。</returns>
		public FilterExpression Locator(IPropertyChain chain)
		{
			return Locator(chain.PropertyPath);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>当前实例。</returns>
		public FilterExpression Locator(IList<String> propertyPath)
		{
			ExpressionTokens token = ExpressionTokens.Locator;

			OnProcessing(token);

			PropertyPath.AddRange(propertyPath);
			m_filterInfoExpression = FilterInfoExpression.Yes;

			OnProcessed(token);

			return this;
		}

		#region 过滤属性的一些快捷方式

		/// <summary>
		/// “Id”属性。
		/// </summary>
		public FilterExpression Id
		{
			get { return Property(WellKnown.Id); }
		}

		/// <summary>
		/// “Name”属性。
		/// </summary>
		public FilterExpression Name
		{
			get { return Property(WellKnown.Name); }
		}

		/// <summary>
		/// “FullName”属性。
		/// </summary>
		public FilterExpression FullName
		{
			get { return Property(WellKnown.FullName); }
		}

		/// <summary>
		/// “DisplayName”属性。
		/// </summary>
		public FilterExpression DisplayName
		{
			get { return Property(WellKnown.DisplayName); }
		}

		/// <summary>
		/// “DisplayOrder”属性。
		/// </summary>
		public FilterExpression DisplayOrder
		{
			get { return Property(WellKnown.DisplayOrder); }
		}

		/// <summary>
		/// “Description”属性。
		/// </summary>
		public FilterExpression Description
		{
			get { return Property(WellKnown.Description); }
		}

		/// <summary>
		/// “Active”属性。
		/// </summary>
		public FilterExpression Active
		{
			get { return Property(WellKnown.Active); }
		}

		/// <summary>
		/// “Deactive”属性。
		/// </summary>
		public FilterExpression Deactive
		{
			get { return Property(WellKnown.Deactive); }
		}

		/// <summary>
		/// “Female”属性。
		/// </summary>
		public FilterExpression Female
		{
			get { return Property(WellKnown.Female); }
		}

		/// <summary>
		/// “Male”属性。
		/// </summary>
		public FilterExpression Male
		{
			get { return Property(WellKnown.Male); }
		}

		/// <summary>
		/// “NavigateUrl”属性。
		/// </summary>
		public FilterExpression NavigateUrl
		{
			get { return Property(WellKnown.NavigateUrl); }
		}

		/// <summary>
		/// “TimeCreated”属性。
		/// </summary>
		public FilterExpression TimeCreated
		{
			get { return Property(WellKnown.TimeCreated); }
		}

		/// <summary>
		/// “TimeModified”属性。
		/// </summary>
		public FilterExpression TimeModified
		{
			get { return Property(WellKnown.TimeModified); }
		}

		/// <summary>
		/// 外部引用“Category”属性。
		/// </summary>
		public FilterExpression Category
		{
			get { return ForeignRef(WellKnown.Category); }
		}

		/// <summary>
		/// 外部引用“Organization”属性。
		/// </summary>
		public FilterExpression Organization
		{
			get { return ForeignRef(WellKnown.Organization); }
		}

		/// <summary>
		/// 外部引用“Owner”属性。
		/// </summary>
		public FilterExpression Owner
		{
			get { return ForeignRef(WellKnown.Owner); }
		}

		/// <summary>
		/// “Parent”属性。
		/// </summary>
		public FilterExpression Parent
		{
			get
			{
				if (!IsBuildingFilter)
				{
					return ForeignRef(WellKnown.Parent);
				}
				else
				{
					return Property(WellKnown.Parent);
				}
			}
		}

		/// <summary>
		/// 外部引用“Region”属性。
		/// </summary>
		public FilterExpression Region
		{
			get { return ForeignRef(WellKnown.Region); }
		}

		/// <summary>
		/// 外部引用“Role”属性。
		/// </summary>
		public FilterExpression Role
		{
			get { return ForeignRef(WellKnown.Role); }
		}

		/// <summary>
		/// 外部引用“User”属性。
		/// </summary>
		public FilterExpression User
		{
			get { return ForeignRef(WellKnown.User); }
		}

		#endregion

		#endregion

		#region FilterInfo

		/// <summary>
		/// 相等过滤条件。
		/// </summary>
		/// <param name="propertyValue">值。</param>
		/// <returns>过滤器表达式。</returns>
		public FilterExpression EqualTo(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.EqualTo(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// 大于过滤器。
		/// </summary>
		/// <param name="propertyValue">属性值。</param>
		/// <returns>大于过滤器。</returns>
		public FilterExpression GreaterThan(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.GreaterThan(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#region GreaterThanOrEqualTo

		/// <summary>
		/// 大于或等于过滤器。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>过滤器信息。</returns>
		public FilterExpression GreaterThanOrEqualTo(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.GreaterThanOrEqualTo(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// 大于或等于过滤器，与 GreaterThanOrEqualTo 相同。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>过滤器信息。</returns>
		public FilterExpression AtLeast(Object propertyValue)
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
		public FilterExpression LessThan(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.LessThan(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#endregion

		#region LessThanOrEqualTo

		/// <summary>
		/// 小于或等于过滤器。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>小于或等于过滤器。</returns>
		public FilterExpression LessThanOrEqualTo(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.LessThanOrEqualTo(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// 小于或等于过滤器，与 LessThanOrEqualTo 相同。
		/// </summary>
		/// <param name="propertyValue">要比较的值。</param>
		/// <returns>小于或等于过滤器。</returns>
		public FilterExpression AtMost(Object propertyValue)
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
		public FilterExpression Between(Object from, Object to)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.Between(from, to);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#endregion

		#region Like

		/// <summary>
		/// LIKE 过滤器。
		/// </summary>
		/// <param name="patternText">模式文本。</param>
		/// <returns>LIKE 过滤器。</returns>
		public FilterExpression Like(String patternText)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.Like(patternText);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// LIKE 过滤器。
		/// </summary>
		/// <param name="patternText">模式文本。</param>
		/// <returns>LIKE 过滤器。</returns>
		/// <param name="escapeChar">转义字符。</param>
		public FilterExpression Like(String patternText, Char escapeChar)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.Like(patternText, escapeChar);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// 包含给定的文本。
		/// </summary>
		/// <param name="text">要包含的文本。</param>
		/// <returns>当前 FilterExpression 实例。</returns>
		public FilterExpression Containing(String text)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.Containing(text);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// 以给定的文本开头。
		/// </summary>
		/// <param name="text">给定的文本。</param>
		/// <returns>当前 FilterExpression 实例。</returns>
		public FilterExpression StartingWith(String text)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.StartingWith(text);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// 以给定的文本结尾。
		/// </summary>
		/// <param name="text">给定的文本。</param>
		/// <returns>当前 FilterExpression 实例。</returns>
		public FilterExpression EndingWith(String text)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.EndingWith(text);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#endregion

		#region IN

		/// <summary>
		/// IN 过滤器，使用值列表。
		/// </summary>
		/// <param name="discreteValues">值列表。</param>
		/// <returns>IN 过滤器。</returns>
		public FilterExpression InValues(params Object[] discreteValues)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.InValues(discreteValues);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <returns>IN 过滤器。</returns>
		public FilterExpression InValues(SelectExpression select)
		{
			return InValues(select, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>IN 过滤器。</returns>
		public FilterExpression InValues(SelectExpression select, Boolean distinct)
		{
			return InValues(select, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="select">选择表达式。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <returns>IN 过滤器。</returns>
		public FilterExpression InValues(SelectExpression select, Filter whereFilter)
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
		public FilterExpression InValues(SelectExpression select, Filter whereFilter, Boolean distinct)
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
		public FilterExpression InValues(SelectExpression select, Filter whereFilter, Filter havingFilter)
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
		public FilterExpression InValues(SelectExpression select, Filter whereFilter, Filter havingFilter, Boolean distinct)
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
		public FilterExpression InValues(IPropertyChain property)
		{
			return InValues(property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="distinct">指示 SQL 指令中是否包含 DISTINCT 关键字。</param>
		/// <returns>IN 过滤器。</returns>
		public FilterExpression InValues(IPropertyChain property, Boolean distinct)
		{
			return InValues(property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// IN 过滤器，使用子查询列表。
		/// </summary>
		/// <param name="property">要选择的属性，不能映射为复合列。</param>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <returns>IN 过滤器。</returns>
		public FilterExpression InValues(IPropertyChain property, Filter whereFilter)
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
		public FilterExpression InValues(IPropertyChain property, Filter whereFilter, Boolean distinct)
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
		public FilterExpression InValues(IPropertyChain property, Filter whereFilter, Filter havingFilter)
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
		public FilterExpression InValues(IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.InValues(property, whereFilter, havingFilter, distinct);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#endregion

		#endregion

		/// <summary>
		/// 重置，以备后续使用。
		/// </summary>
		public void Reset()
		{
			m_builder.Reset();
			m_current = null;

			if (m_propertyPath != null)
			{
				m_propertyPath.Clear();
			}

			m_appendedTokens.Clear();
		}

		/// <summary>
		/// 解析表达式，获取过滤器。
		/// </summary>
		/// <returns>过滤器。</returns>
		public Filter Resolve()
		{
			if (!IsResolvable)
			{
				throw new InvalidOperationException("表达式不完全，无法解析。");
			}

			if (CurrentToken == ExpressionTokens.Start)
			{
				return Current.Filter;
			}

			m_current = m_builder.Resolve();

			return Current.Filter;
		}

		#endregion

		#region 强制类型转换

		/// <summary>
		/// 隐式强制类型转换，将过滤器表达式转换为过滤器，调用其 Resolve 方法。
		/// </summary>
		/// <param name="expression">过滤器表达式。</param>
		/// <returns>过滤器。</returns>
		public static implicit operator Filter(FilterExpression expression)
		{
			return expression.Resolve();
		}

		#endregion

		#region 私有属性

		/// <summary>
		/// 获取当前的过滤器工厂。
		/// </summary>
		private FilterFactory Current
		{
			get
			{
				if (m_current == null)
				{
					m_current = FilterFactory.CreateDefault();
				}

				return m_current;
			}
		}

		/// <summary>
		/// 获取当前的词法元素。
		/// </summary>
		private ExpressionTokens CurrentToken
		{
			get
			{
				if (m_appendedTokens.Count != 0)
				{
					return m_appendedTokens[m_appendedTokens.Count - 1];
				}
				else
				{
					return ExpressionTokens.Nothing;
				}
			}
		}

		/// <summary>
		/// 获取过滤器工厂栈。
		/// </summary>
		private IFilterFactoryOperands FilterFactories
		{
			get { return m_builder.FilterFactories; }
		}

		/// <summary>
		/// 获取一个值，指示是否有 With 操作符压栈。
		/// </summary>
		private Boolean HasWithToken
		{
			get { return m_builder.HasWithToken; }
		}

		/// <summary>
		/// 判断当前是否处于构建过滤器的阶段。
		/// </summary>
		private Boolean IsBuildingFilter
		{
			get
			{
				if (CurrentToken == ExpressionTokens.Start)
				{
					return false;
				}

				ExpressionTokens buildingTokens = (ExpressionTokens.ForeignRef | ExpressionTokens.Property | ExpressionTokens.Is);

				if (CurrentToken != ExpressionTokens.Not)
				{
					return ((buildingTokens & CurrentToken) != 0);
				}
				else
				{
					return ((buildingTokens & PreviousToken) != 0);
				}
			}
		}

		/// <summary>
		/// 获取前一个词法元素。
		/// </summary>
		private ExpressionTokens PreviousToken
		{
			get
			{
				if (m_appendedTokens.Count > 1)
				{
					return m_appendedTokens[m_appendedTokens.Count - 2];
				}
				else
				{
					return ExpressionTokens.Nothing;
				}
			}
		}

		/// <summary>
		/// 获取属性路径。
		/// </summary>
		public List<String> PropertyPath
		{
			get
			{
				if (m_propertyPath == null)
				{
					m_propertyPath = new List<String>();
				}

				return m_propertyPath;
			}
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 压栈工厂过滤器。
		/// </summary>
		/// <param name="factory">要压栈的工厂过滤器。</param>
		/// <returns>当前实例。</returns>
		private FilterExpression Filter(FilterFactory factory)
		{
			ExpressionTokens token = ExpressionTokens.Filter;

			OnProcessing(token);

			m_builder.Push(factory);

			OnProcessed(token);

			return this;
		}

		/// <summary>
		/// 在实施过滤条件前调用。
		/// </summary>
		private void OnFilterInfoProcessing()
		{
			OnProcessing(ExpressionTokens.FilterInfo);
		}

		/// <summary>
		/// 在实施过滤条件后调用。
		/// </summary>
		private void OnFilterInfoProcessed()
		{
			OnProcessed(ExpressionTokens.FilterInfo);
		}

		/// <summary>
		/// 在词法元素附加前调用。
		/// </summary>
		/// <param name="tokenAppending">将要附加的词法元素。</param>
		private void OnProcessing(ExpressionTokens tokenAppending)
		{
			SyntaxCheck(tokenAppending);
		}

		/// <summary>
		/// 在词法元素附加后调用。
		/// </summary>
		/// <param name="tokenAppended">已被附加的词法元素。</param>
		private void OnProcessed(ExpressionTokens tokenAppended)
		{
			m_appendedTokens.Add(tokenAppended);
		}

		/// <summary>
		/// 压栈过滤器工厂创建操作符。
		/// </summary>
		/// <param name="info">过滤器信息。</param>
		private void PushNewFilterFactoryOperator(FilterInfo info)
		{
			ColumnLocator colLocator = new ColumnLocator(PropertyPath.ToArray());

			NewFilterFactoryOperator op = new NewFilterFactoryOperator(colLocator, info);

			m_builder.Push(op);

			// 重置列定位信息
			PropertyPath.Clear();
		}

		#region 语法检查

		/// <summary>
		/// 检查要附加的词法元素是否合法。
		/// </summary>
		/// <param name="tokenAppending"></param>
		[Conditional("DEBUG")]
		private void SyntaxCheck(ExpressionTokens tokenAppending)
		{
			ExpressionTokens validTokens = GetNextValidTokens();

			if ((validTokens & tokenAppending) == tokenAppending)
			{
				return;
			}

			// 构建调用踪迹
			List<String> currentTokens = m_appendedTokens.ConvertAll<String>(
					delegate(ExpressionTokens token)
					{
						return token.ToString();
					}
				);

			String traceString = String.Join(".", currentTokens.ToArray());

			traceString += String.Format(".[{0}]", tokenAppending.ToString());

			if (tokenAppending == ExpressionTokens.EndWith)
			{
				throw new InvalidOperationException(String.Format("没有与 EndWith 相匹配的 With 操作符，调用踪迹为：{0}", traceString));
			}

			// 从合法的标记中去掉 Finish
			validTokens &= ~ExpressionTokens.Finish;

			if (!m_appendedTokens.Contains(ExpressionTokens.FilterInfo) && ((validTokens & ExpressionTokens.FilterInfo) == 0))
			{
				throw new InvalidOperationException(String.Format("此处不能使用 {0}，可使用的元素为 {1}，调用踪迹为：{2}", tokenAppending.ToString(), validTokens.ToString(), traceString));
			}
			else
			{
				throw new InvalidOperationException(String.Format("此处不能使用 {0}，可使用的元素为 {1}，调用踪迹为：{2}（注：FilterInfo 指诸如 Null、True，EqualTo 等过滤条件）", tokenAppending.ToString(), validTokens.ToString(), traceString));
			}
		}

		/// <summary>
		/// 获得下一个合法的词法元素集合。
		/// </summary>
		/// <returns>合法的词法元素集合。</returns>
		private ExpressionTokens GetNextValidTokens()
		{
			ExpressionTokens nextTokens = ExpressionTokens.Nothing;

			switch (CurrentToken)
			{
				case ExpressionTokens.Start:
					nextTokens = ExpressionTokens.Filter
						| ExpressionTokens.ForeignRef
						| ExpressionTokens.Property
						| ExpressionTokens.Locator
						| ExpressionTokens.Not
						| ExpressionTokens.With
						| ExpressionTokens.Finish;
					break;

				case ExpressionTokens.Filter:
				case ExpressionTokens.FilterInfo:
					nextTokens = ExpressionTokens.And
						| ExpressionTokens.Or
						| ExpressionTokens.Finish;

					// 还要判断能否再附加 EndWith
					if (HasWithToken)
					{
						nextTokens |= ExpressionTokens.EndWith;
					}

					break;

				case ExpressionTokens.ForeignRef:
					nextTokens = ExpressionTokens.Property
						| ExpressionTokens.Is
						| ExpressionTokens.Not
						| ExpressionTokens.FilterInfo;
					break;

				case ExpressionTokens.Locator:
				case ExpressionTokens.Property:
					nextTokens = ExpressionTokens.Is
						| ExpressionTokens.Not
						| ExpressionTokens.FilterInfo;
					break;

				case ExpressionTokens.Not:
					if (!IsBuildingFilter)
					{
						nextTokens = ExpressionTokens.Filter
							| ExpressionTokens.ForeignRef
							| ExpressionTokens.Property
							| ExpressionTokens.Locator
							| ExpressionTokens.With;
					}
					else
					{
						nextTokens = ExpressionTokens.FilterInfo;
					}

					break;

				case ExpressionTokens.With:
					nextTokens = ExpressionTokens.Filter
						| ExpressionTokens.ForeignRef
						| ExpressionTokens.Property
						| ExpressionTokens.Locator
						| ExpressionTokens.Not
						| ExpressionTokens.With;
					break;

				case ExpressionTokens.And:
				case ExpressionTokens.Or:
					nextTokens = ExpressionTokens.Filter
						| ExpressionTokens.ForeignRef
						| ExpressionTokens.Property
						| ExpressionTokens.Locator
						| ExpressionTokens.With;
					break;

				case ExpressionTokens.EndWith:
					nextTokens = ExpressionTokens.And
						| ExpressionTokens.Or
						| ExpressionTokens.Finish;

					if (HasWithToken)
					{
						nextTokens |= ExpressionTokens.EndWith;
					}

					break;

				case ExpressionTokens.Is:
					nextTokens = ExpressionTokens.Not | ExpressionTokens.FilterInfo;
					break;

				case ExpressionTokens.Finish:
				default:
					break;
			}

			return nextTokens;
		}

		#endregion

		#endregion
	}
}