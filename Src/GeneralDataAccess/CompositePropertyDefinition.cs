#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositePropertyDefinition.cs
// 文件功能描述：复合实体属性定义。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110708
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
	/// 复合实体属性定义。
	/// </summary>
	internal abstract class CompositePropertyDefinition
	{
		#region 私有字段

		private readonly CompositeDefinition m_composite;
		private readonly PropertyInfo m_propertyInfo;
		private readonly LoadStrategyAttribute m_loadStrategy;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性信息。
		/// </summary>
		/// <param name="definition">复合实体定义。</param>
		/// <param name="pf">属性信息。</param>
		protected CompositePropertyDefinition(CompositeDefinition definition, PropertyInfo pf)
		{
			m_composite = definition;
			m_propertyInfo = pf;
			m_loadStrategy = (LoadStrategyAttribute)Attribute.GetCustomAttribute(pf, typeof(LoadStrategyAttribute));
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取复合实体定义。
		/// </summary>
		internal CompositeDefinition Composite
		{
			get { return m_composite; }
		}

		/// <summary>
		/// 获取属性名称。
		/// </summary>
		public String Name
		{
			get { return m_propertyInfo.Name; }
		}

		/// <summary>
		/// 获取属性类型。
		/// </summary>
		public Type Type
		{
			get { return m_propertyInfo.PropertyType; }
		}

		/// <summary>
		/// 获取加载策略。
		/// </summary>
		public LoadStrategyAttribute LoadStrategy
		{
			get { return m_loadStrategy; }
		}

		/// <summary>
		/// 获取属性信息。
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get { return m_propertyInfo; }
		}


		#endregion
	}
}