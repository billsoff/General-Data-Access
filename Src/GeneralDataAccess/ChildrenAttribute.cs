#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ChildrenAttribute.cs
// 文件功能描述：标注属性为子实体集合，属性类型只能为数组。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 标注属性为子实体集合，属性类型只能为数组。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ChildrenAttribute : Attribute
	{
		#region 私有字段

		private readonly String m_propertyName;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置引用实体的属性名称。
		/// </summary>
		/// <param name="propertyName">子实体中引用到当前实体的属性名称。</param>
		public ChildrenAttribute(String propertyName)
		{
			m_propertyName = propertyName;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取引用实体属性名称。
		/// </summary>
		public String PropertyName
		{
			get { return m_propertyName; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="parentEntity">父实体。</param>
		/// <returns>创建好的过滤器。</returns>
		public Filter CreateFilter(Object parentEntity)
		{
			Filter f = Filter.Create(PropertyName, Is.EqualTo(parentEntity));

			return f;
		}

		#endregion
	}
}