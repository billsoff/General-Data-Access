#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupDefinition.GroupPropertyDefinitionCollection.cs
// 文件功能描述：分组结果实体属性集合。
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	partial class GroupDefinition
	{
		/// <summary>
		/// 分组结果实体属性集合。
		/// </summary>
		internal sealed class GroupPropertyDefinitionCollection : ReadOnlyCollection<GroupPropertyDefinition>
		{
			#region 私有字段

			private readonly GroupDefinition m_group;

			#endregion

			#region 构造函数

			/// <summary>
			/// 构造函数，设置分组实体定义和属性集合。
			/// </summary>
			/// <param name="group">分组实体定义。</param>
			/// <param name="allProperties">属性集合。</param>
			public GroupPropertyDefinitionCollection(GroupDefinition group, IList<GroupPropertyDefinition> allProperties)
				: base(allProperties)
			{
				m_group = group;
			}

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取指定名称的分组结果实体的属性。
			/// </summary>
			/// <param name="propertyName">属性名称。</param>
			/// <returns>具有该名称的属性，如果未找到，则返回 null。</returns>
			public GroupPropertyDefinition this[String propertyName]
			{
				get
				{
					#region 前置条件

					Debug.Assert(!String.IsNullOrEmpty(propertyName), "参数 propertyName 不能为 null 或空字符串。");

					#endregion

					foreach (GroupPropertyDefinition propertyDef in Items)
					{
						if (propertyDef.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
						{
							return propertyDef;
						}
					}

					return null;
				}
			}

			/// <summary>
			/// 获取分组实体定义。
			/// </summary>
			public GroupDefinition Group
			{
				get { return m_group; }
			}

			#endregion

			#region 公共方法

			/// <summary>
			/// 判断是否存在指定名称的属性。
			/// </summary>
			/// <param name="propertyName">要判断的属性名称。</param>
			/// <returns>如果存在具有该名称的属性，则返回 true；否则返回 false。</returns>
			public Boolean Contains(String propertyName)
			{
				foreach (GroupPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
					{
						return true;
					}
				}

				return false;
			}

			#endregion
		}
	}
}