#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeDefinitionBuilder.cs
// 文件功能描述：用于生成复合实体定义。
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于生成复合实体定义。
	/// </summary>
	internal sealed class CompositeDefinitionBuilder
	{
		#region 静态成员

		#region 私有字段

		private static readonly CompositeDefinitionBuilder m_instance;

		#endregion

		#region 类型构造函数

		/// <summary>
		/// 类型初始化器。
		/// </summary>
		static CompositeDefinitionBuilder()
		{
			m_instance = new CompositeDefinitionBuilder();
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建复合实体定义。
		/// </summary>
		/// <param name="compositeResultType">复合实体类型。</param>
		/// <returns>创建好的复合实体定义。</returns>
		public static CompositeDefinition Build(Type compositeResultType)
		{
			return m_instance.GetDefinition(compositeResultType);
		}

		#endregion

		#endregion

		#region 私有字段

		private readonly Dictionary<Type, CompositeDefinition> m_definitions = new Dictionary<Type, CompositeDefinition>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，私有化，以支持单例。
		/// </summary>
		private CompositeDefinitionBuilder()
		{
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取复合实体定义。
		/// </summary>
		/// <param name="compositeResultType">复合实体类型。</param>
		/// <returns>复合实体定义。</returns>
		public CompositeDefinition GetDefinition(Type compositeResultType)
		{
			if (!m_definitions.ContainsKey(compositeResultType))
			{
				Object syncRoot = ((ICollection)m_definitions).SyncRoot;

				lock (syncRoot)
				{
					if (!m_definitions.ContainsKey(compositeResultType))
					{
						m_definitions.Add(compositeResultType, new CompositeDefinition(compositeResultType));
					}
				}
			}

			return m_definitions[compositeResultType];
		}

		#endregion
	}
}