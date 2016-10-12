#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupDefinitionBuilder.cs
// 文件功能描述：分组结果实体定义创建器。
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 分组结果实体定义创建器。
	/// </summary>
	internal sealed class GroupDefinitionBuilder
	{
		#region 静态成员

		#region 私有字段

		private static readonly GroupDefinitionBuilder m_instance;

		#endregion

		#region 类型构造函数

		/// <summary>
		/// 类型初始化器。
		/// </summary>
		static GroupDefinitionBuilder()
		{
			m_instance = new GroupDefinitionBuilder();
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建分组结果实体类型定义。
		/// </summary>
		/// <param name="groupType">分组结果实体类型。</param>
		/// <returns>创建好的分组结果实体类型定义。</returns>
		public static GroupDefinition Build(Type groupType)
		{
			return m_instance.GetDefinition(groupType);
		}

		#endregion

		#endregion

		#region 私有字段

		private readonly Dictionary<Type, GroupDefinition> m_definitions = new Dictionary<Type, GroupDefinition>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，私有化，以支持单例。
		/// </summary>
		private GroupDefinitionBuilder()
		{
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取分组结果实体的类型定义。
		/// </summary>
		/// <param name="groupType">分组结果实体类型。</param>
		/// <returns>分组结果实体的类型定义。</returns>
		public GroupDefinition GetDefinition(Type groupType)
		{
			if (!m_definitions.ContainsKey(groupType))
			{
				Object syncRoot = ((ICollection)m_definitions).SyncRoot;

				lock (syncRoot)
				{
					if (!m_definitions.ContainsKey(groupType))
					{
						m_definitions.Add(groupType, new GroupDefinition(groupType));
					}
				}
			}

			return m_definitions[groupType];
		}

		#endregion
	}
}