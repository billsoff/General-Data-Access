#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityDefinitionBuilder.cs
// 文件功能描述：实体定义创建器。
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 实体定义创建器。
	/// </summary>
	internal sealed class EntityDefinitionBuilder : IEntityDefinitionProvider
	{
		#region 静态成员

		#region 私有字段

		private static readonly EntityDefinitionBuilder m_instance;

		#endregion

		#region 类型构造函数

		/// <summary>
		/// 类型初始化器。
		/// </summary>
		static EntityDefinitionBuilder()
		{
			m_instance = new EntityDefinitionBuilder();
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建类型定义。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>实体类型定义。</returns>
		public static EntityDefinition Build(Type entityType)
		{
			return m_instance.GetDefinition(entityType);
		}

		#endregion

		#endregion

		#region 私有字段

		private readonly Dictionary<Type, EntityDefinition> m_definitions = new Dictionary<Type, EntityDefinition>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，私有化，以支持单例。
		/// </summary>
		private EntityDefinitionBuilder()
		{
		}

		#endregion

		#region IEntityDefinitionProvider 成员

		/// <summary>
		/// 获取实体定义。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>实体定义。</returns>
		public EntityDefinition GetDefinition(Type entityType)
		{
			Debug.Assert((entityType != null), "实体类型参数不能为空。");

			EntityDefinition definition = null;

			if (!m_definitions.ContainsKey(entityType))
			{
				Object syncRoot = ((ICollection)m_definitions).SyncRoot;

				lock (syncRoot)
				{
					if (!m_definitions.ContainsKey(entityType))
					{
						definition = new EntityDefinition(entityType);

						m_definitions[entityType] = definition;

						definition.Initialize(this);
					}
				}
			}

			if (definition == null)
			{
				definition = m_definitions[entityType];
			}

			return definition;
		}

		#endregion
	}
}