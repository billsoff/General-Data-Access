#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityPropertyDefinitionRelation.cs
// 文件功能描述：表示实体外部引用属性关系。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110413
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
	/// 表示实体外部引用属性关系。
	/// </summary>
	internal sealed class EntityPropertyDefinitionRelation
	{
		#region 私有字段

		private readonly EntityPropertyDefinition m_childProperty;
		private readonly EntityPropertyDefinition[] m_parentProperties;

		private readonly EntityDefinition m_child;
		private readonly EntityDefinition m_parent;

		private readonly ColumnDefinition[] m_childColumns;
		private readonly ColumnDefinition[] m_parentColumns;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置相映射的子列定义列表和父列定义列表。
		/// </summary>
		/// <param name="childColumns">子列定义列表。</param>
		/// <param name="parentColumns">父列定义列表。</param>
		internal EntityPropertyDefinitionRelation(ColumnDefinition[] childColumns, ColumnDefinition[] parentColumns)
		{
			m_childProperty = childColumns[0].Property;

			m_child = childColumns[0].Property.Entity;
			m_parent = parentColumns[0].Property.Entity;

			m_childColumns = childColumns;
			m_parentColumns = parentColumns;

			// 取出父实体属性
			List<EntityPropertyDefinition> parentProperties = new List<EntityPropertyDefinition>();

			foreach (ColumnDefinition columnDef in parentColumns)
			{
				if (!parentProperties.Contains(columnDef.Property))
				{
					parentProperties.Add(columnDef.Property);
				}
			}

			m_parentProperties = parentProperties.ToArray();
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取外部引用属性。
		/// </summary>
		public EntityPropertyDefinition ChildProperty
		{
			get { return m_childProperty; }
		}

		/// <summary>
		/// 获取引用实体定义。
		/// </summary>
		public EntityDefinition Child
		{
			get { return m_child; }
		}

		/// <summary>
		/// 获取被引用实体定义。
		/// </summary>
		public EntityDefinition Parent
		{
			get { return m_parent; }
		}

		/// <summary>
		/// 获取引用列定义列表。
		/// </summary>
		public ColumnDefinition[] ChildColumns
		{
			get { return m_childColumns; }
		}

		/// <summary>
		/// 获取被引用列定义列表。
		/// </summary>
		public ColumnDefinition[] ParentColumns
		{
			get { return m_parentColumns; }
		}

		/// <summary>
		/// 获取被引用的属性。
		/// </summary>
		public EntityPropertyDefinition[] ParentProperties
		{
			get { return m_parentProperties; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取指定名称的父属性。
		/// </summary>
		/// <param name="parentPropertyName">父属性名称。</param>
		/// <returns>具有该名称的父属性，如果没有找到，则返回 null。</returns>
		public EntityPropertyDefinition GetParentPropertyByName(String parentPropertyName)
		{
			EntityPropertyDefinition parentProperty = Array.Find<EntityPropertyDefinition>(
					this.ParentProperties,
					delegate(EntityPropertyDefinition property)
					{
						return property.Name.Equals(parentPropertyName, CommonPolicies.PropertyNameComparison);
					}
				);

			return parentProperty;
		}

		/// <summary>
		/// 实体属性关系的简单表示。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Format("{0} -> {1}", ChildProperty, Parent);
		}

		#endregion
	}
}