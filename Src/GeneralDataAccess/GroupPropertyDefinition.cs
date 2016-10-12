#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupPropertyDefinition.cs
// 文件功能描述：表示分组结果的类型属性定义。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110628
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示分组结果的类型属性定义。
	/// </summary>
	internal sealed class GroupPropertyDefinition
	{
		#region 私有字段

		private readonly GroupDefinition m_group;
		private readonly PropertyInfo m_propertyInfo;

		private readonly AggregationAttribute m_aggregation;
		private readonly EntityPropertyDefinition m_definition;
		private readonly IPropertyChain m_propertyChain;

		private readonly Int32 m_level;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置分组类型定义与属性。
		/// </summary>
		/// <param name="group">分组结果类型定义。</param>
		/// <param name="propertyInfo">属性。</param>
		public GroupPropertyDefinition(GroupDefinition group, PropertyInfo propertyInfo)
		{
			m_group = group;
			m_propertyInfo = propertyInfo;

			m_aggregation = (AggregationAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(AggregationAttribute));

			if (m_aggregation.PropertyPath != null)
			{
				m_propertyChain = new PropertyChain(group.Entity.Type, m_aggregation.PropertyPath);
				m_definition = m_group.Entity.Properties[m_propertyChain];
				m_level = m_aggregation.PropertyPath.Length;
			}
			else
			{
				m_level = 1;
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取要进行聚合操作或分组的属性定义。
		/// </summary>
		internal EntityPropertyDefinition Definition
		{
			get { return m_definition; }
		}

		/// <summary>
		/// 获取属性所属的分组类型定义。
		/// </summary>
		internal GroupDefinition Group
		{
			get { return m_group; }
		}

		/// <summary>
		/// 获取分组项标记。
		/// </summary>
		public AggregationAttribute Aggregation
		{
			get { return m_aggregation; }
		}

		/// <summary>
		/// 获取一个值，该值指示当前属性是否为分组项。
		/// </summary>
		public Boolean IsGroupItem
		{
			get { return m_aggregation.IsGroupItem; }
		}

		/// <summary>
		/// 获取一个值，该值指示属性是否为基本属性（即值属性）。
		/// </summary>
		public Boolean IsPrimitive
		{
			get { return (m_definition == null) || m_definition.IsPrimitive; }
		}

		/// <summary>
		/// 获取要进行聚合计算或分组的属性的级别。
		/// </summary>
		public Int32 Level
		{
			get { return m_level; }
		}

		/// <summary>
		/// 获取属性名称。
		/// </summary>
		public String Name
		{
			get { return m_propertyInfo.Name; }
		}

		/// <summary>
		/// 获取要聚合的属性链。
		/// </summary>
		public IPropertyChain PropertyChain
		{
			get { return m_propertyChain; }
		}

		/// <summary>
		/// 获取属性。
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get { return m_propertyInfo; }
		}

		/// <summary>
		/// 获取属性类型。
		/// </summary>
		public Type Type
		{
			get { return m_propertyInfo.PropertyType; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 为参与分组的外部引用属性创建默认的实体架构组合生成策略。
		/// </summary>
		/// <returns>创建好的实体架构组合生成策略。</returns>
		public CompositeBuilderStrategy CreateDefaultBuilderStrategy()
		{
			#region 前置条件

			Debug.Assert(!IsPrimitive, "只有参与分组的外部引用属性才可以调用方法 GetBuilderStrategy。");

			#endregion

			LoadStrategyAttribute attr;

			attr = (LoadStrategyAttribute)Attribute.GetCustomAttribute(PropertyInfo, typeof(LoadStrategyAttribute));

			if (attr == null)
			{
				attr = Definition.Entity.LoadStrategy;
			}

			CompositeBuilderStrategy strategy = attr.Create();

			return strategy;
		}

		/// <summary>
		/// 字符串表示。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			if (m_aggregation.PropertyPath != null)
			{
				return String.Format(
						"{0} -> {1}.{2}",
						m_aggregation.ToString(),
						m_group.Entity.Type.Name,
						String.Join(".", m_aggregation.PropertyPath)
					);
			}
			else
			{
				return String.Format(
						"{0} -> {1}",
						m_aggregation.ToString(),
						m_group.Entity.Type.Name
					);
			}
		}

		#endregion
	}
}