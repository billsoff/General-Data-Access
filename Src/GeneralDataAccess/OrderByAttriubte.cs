#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：OrderByAttriubte.cs
// 文件功能描述：指示排序字段。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110407
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 指示排序字段。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class OrderByAttribute : Attribute
	{
		#region 静态成员

		/// <summary>
		/// 合成排序器。
		/// </summary>
		/// <param name="children">子属性。</param>
		/// <returns>合成好的排序器，如果该属性上没有标记 OrderByAttribute，则返回 null。</returns>
		public static Sorter ComposeSorter(PropertyInfo children)
		{
			OrderByAttribute[] orderByAttrs = (OrderByAttribute[])Attribute.GetCustomAttributes(children, typeof(OrderByAttribute));

			if (orderByAttrs.Length == 0)
			{
				return null;
			}

			Array.Sort<OrderByAttribute>(
					orderByAttrs,
					delegate(OrderByAttribute left, OrderByAttribute right)
					{
						return left.OrderNumber.CompareTo(right.OrderNumber);
					}
				);

			OrderByExpression expression = new OrderByExpression();

			for (Int32 i = 0; i < orderByAttrs.Length; i++)
			{
				OrderByAttribute attr = orderByAttrs[i];

				expression.Property(attr.PropertyPath);

				if (attr.SortMethod == SortMethod.Descending)
				{
					expression = expression.Descending;
				}
			}

			Sorter s = expression.Resolve();

			return s;
		}

		#endregion

		#region 私有字段

		private readonly String[] m_proertyPath;
		private readonly Int32 m_orderNumber;
		private SortMethod m_sortMethod;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，内部使用。
		/// </summary>
		private OrderByAttribute()
		{
			m_sortMethod = SortMethod.Ascending;
		}

		/// <summary>
		/// 构造函数，设置要排序的值属性名称和排序顺序。
		/// </summary>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="orderNumber">排序顺序。</param>
		public OrderByAttribute(String propertyName, Int32 orderNumber)
			: this(new String[] { propertyName }, orderNumber)
		{
		}

		/// <summary>
		/// 构造函数，设置要排序的实体属性名称和值属性名称。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="orderNumber">排序顺序。</param>
		public OrderByAttribute(String entityPropertyName, String propertyName, Int32 orderNumber)
			: this(new String[] { entityPropertyName, propertyName }, orderNumber)
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径和排序顺序。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="orderNumber">排序顺序。</param>
		public OrderByAttribute(String[] propertyPath, Int32 orderNumber)
			: this()
		{
			m_proertyPath = propertyPath;
			m_orderNumber = orderNumber;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取排序顺序。
		/// </summary>
		public Int32 OrderNumber
		{
			get { return m_orderNumber; }
		}

		/// <summary>
		/// 获取属性路径。
		/// </summary>
		public String[] PropertyPath { get { return m_proertyPath; } }

		/// <summary>
		/// 获取或设置排序方式。
		/// </summary>
		public SortMethod SortMethod
		{
			get { return m_sortMethod; }
			set { m_sortMethod = value; }
		}

		#endregion
	}
}