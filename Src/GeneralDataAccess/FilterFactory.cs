#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterFactory.cs
// 文件功能描述：用于生成过滤器。
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

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于生成过滤器。
	/// </summary>
	[Serializable]
	public sealed class FilterFactory : IFilterProvider
	{
		#region 私有字段

		private readonly IList<String> m_propertyPath;
		private readonly FilterInfo m_filterInfo;

		private Filter m_filter;
		private Boolean m_created;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		internal FilterFactory()
		{
			m_created = true;
		}

		/// <summary>
		/// 构造函数，直接设置过滤器。
		/// </summary>
		/// <param name="filter"></param>
		public FilterFactory(Filter filter)
			: this()
		{
			m_filter = filter;
		}

		/// <summary>
		/// 构造函数，设置属性路径和过滤器信息（包含比较操作符和要比较的值）。
		/// </summary>
		/// <param name="propertyPath">值属性路径。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		public FilterFactory(IList<String> propertyPath, FilterInfo filterInfo)
		{
			m_propertyPath = propertyPath;
			m_filterInfo = filterInfo;
		}

		#endregion

		#region 静态成员

		/// <summary>
		/// 创建默认过滤器工厂。
		/// </summary>
		/// <returns>过滤器工厂。</returns>
		public static FilterFactory CreateDefault()
		{
			return new FilterFactory();
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取过滤器。
		/// </summary>
		public Filter Filter
		{
			get
			{
				if (!m_created)
				{
					m_filter = Create();

					m_created = true;
				}

				return m_filter;
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 与指定的过滤器工厂做“与”操作。
		/// </summary>
		/// <param name="target">指定的过滤器工厂。</param>
		/// <returns>操作结果。</returns>
		public FilterFactory And(FilterFactory target)
		{
			return Combine(LogicOperator.And, target);
		}

		/// <summary>
		/// 与指定的过滤器工厂做“或”操作。
		/// </summary>
		/// <param name="target">指定的过滤器工厂。</param>
		/// <returns>操作结果。</returns>
		public FilterFactory Or(FilterFactory target)
		{
			return Combine(LogicOperator.Or, target);
		}

		/// <summary>
		/// 对当前实例执行“非”操作。
		/// </summary>
		/// <returns>操作结果。</returns>
		public FilterFactory Not()
		{
			FilterFactory result = this;

			if (Filter != null)
			{
				if (Filter.HasFilters)
				{
					Filter.Not();
				}
				else
				{
					Filter clone = Filter.Clone();

					clone.Not();

					result = new FilterFactory(clone);
				}
			}

			return result;
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 按指定的逻辑操作符连接当前实例和指定的过滤器工厂。
		/// </summary>
		/// <param name="logicOperator">逻辑操作符。</param>
		/// <param name="target">目标过滤过滤器工厂。</param>
		/// <returns>连接结果，过滤器工厂。</returns>
		private FilterFactory Combine(LogicOperator logicOperator, FilterFactory target)
		{
			if (target == null)
			{
				return this;
			}

			Filter left = Filter;
			Filter right = target.Filter;

			if (right == null)
			{
				return this;
			}
			else if (left == null)
			{
				return target;
			}

			// 如果右侧是复合过滤器
			if (right.HasFilters)
			{
				if (!right.Negative && (right.LogicOperator == logicOperator))
				{
					right.Filters.Insert(0, left);

					return target;
				}
			}
			// 如果左侧是复合过滤器
			else if (left.HasFilters)
			{
				if (!left.Negative && left.LogicOperator == logicOperator)
				{
					left.Filters.Add(right);

					return this;
				}
			}

			CompositeFilter compround = new CompositeFilter(logicOperator, new Filter[] { left, right });

			return new FilterFactory(compround);
		}

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <returns>创建好的过滤器。</returns>
		private Filter Create()
		{
			if (m_filterInfo != null)
			{
				return m_filterInfo.CreateFilter(m_propertyPath);
			}
			else
			{
				return m_filter;
			}
		}

		#endregion
	}
}