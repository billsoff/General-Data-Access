#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterFactoryInfo.cs
// 文件功能描述：包含用于创建过滤器工厂的信息，用于创建过滤器工厂。
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
	/// 包含用于创建过滤器工厂的信息，用于创建过滤器工厂。
	/// <para>子类应实现传和不传 negative 的两类构造函数，然后在类 FilterInofExpression、Is 和 FilterExpression 中构建实例，创建语言元素。</para>
	/// </summary>
	[Serializable]
	public abstract class FilterInfo
	{
		#region 私有字段

		private readonly Object[] m_values;
		private readonly QueryListBuilder m_queryListBuilder;
		private readonly Boolean m_negative;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		protected FilterInfo()
		{
		}

		/// <summary>
		/// 构造函数。
		/// </summary>
		/// <param name="negative">指示是否进行“非”操作。</param>
		protected FilterInfo(Boolean negative)
			: this(negative, (Object[])null)
		{
		}

		/// <summary>
		/// 构造函数，设置属性值集合。
		/// </summary>
		/// <param name="values">属性值集合。</param>
		public FilterInfo(Object[] values)
			: this(false, values)
		{
		}

		/// <summary>
		/// 构造函数，设置属性值集合。
		/// </summary>
		/// <param name="negative">指示是否进行“非”操作。</param>
		/// <param name="values">属性值集合。</param>
		protected FilterInfo(Boolean negative, Object[] values)
		{
			m_negative = negative;
			m_values = values;
		}

		/// <summary>
		/// 构造函数，设置属性值集合。
		/// </summary>
		/// <param name="builder">查询列表生成器。</param>
		internal FilterInfo(QueryListBuilder builder)
			: this(false, builder)
		{
		}

		/// <summary>
		/// 构造函数，设置属性值集合。
		/// </summary>
		/// <param name="negative">指示是否进行“非”操作。</param>
		/// <param name="builder">查询列表生成器。</param>
		internal FilterInfo(Boolean negative, QueryListBuilder builder)
		{
			m_negative = negative;
			m_queryListBuilder = builder;
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="propertyPath">值属性路径。</param>
		/// <returns>创建好的过滤器工厂。</returns>
		public Filter CreateFilter(IList<String> propertyPath)
		{
			Filter result = DoCreateFilter(propertyPath);

			if (m_negative)
			{
				result.Not();
			}

			return result;
		}

		#endregion

		#region 保护的属性

		/// <summary>
		/// 获取属性值集合。
		/// </summary>
		protected Object[] Values
		{
			get { return m_values; }
		}

		/// <summary>
		/// 获取查询列表生成器。
		/// </summary>
		internal QueryListBuilder QueryListBuilder
		{
			get { return m_queryListBuilder; }
		}

		#endregion

		#region 抽象成员

		#region 公共方法

		/// <summary>
		/// 委托子类实现创建过滤器。
		/// </summary>
		/// <param name="propertyPath">值属性路径。</param>
		/// <returns>创建好的过滤器。</returns>
		protected abstract Filter DoCreateFilter(IList<String> propertyPath);

		#endregion

		#endregion
	}
}