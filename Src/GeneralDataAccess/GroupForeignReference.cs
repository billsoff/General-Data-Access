#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupForeignReference.cs
// 文件功能描述：用于包装分组结果实体定义外部引用属性（用于分组）和与其匹配的实体架构。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110630
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
	/// 用于包装分组结果实体定义外部引用属性（用于分组）和与其匹配的实体架构。
	/// </summary>
	internal struct GroupForeignReference
	{
		#region 公有字段

		private readonly GroupPropertyDefinition m_property;
		private readonly EntitySchema m_schema;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置分组结果定义外部引用属性和与其相匹配的实体架构。
		/// </summary>
		/// <param name="property"></param>
		/// <param name="schema"></param>
		public GroupForeignReference(GroupPropertyDefinition property, EntitySchema schema)
		{
			m_property = property;
			m_schema = schema;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取分组结果实体定义的外部引用属性。
		/// </summary>
		public GroupPropertyDefinition Property
		{
			get { return m_property; }
		}

		/// <summary>
		/// 获取匹配的实体架构。
		/// </summary>
		public EntitySchema Schema
		{
			get { return m_schema; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 合成外部引用实体。
		/// </summary>
		/// <param name="dbValues">记录值。</param>
		/// <returns>合成好的外部引用实体。</returns>
		public Object Compose(Object[] dbValues)
		{
			return m_schema.Compose(dbValues);
		}

		#endregion
	}
}