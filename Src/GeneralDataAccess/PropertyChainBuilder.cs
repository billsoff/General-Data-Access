#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertyChainBuilder.cs
// 文件功能描述：属性链生成器的内部实现。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110430
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 属性链生成器的内部实现。
	/// </summary>
	[Serializable]
	internal sealed class PropertyChainBuilder : IPropertyChainBuilder
	{
		#region 私有字段

		private readonly Type m_type;
		private readonly List<String> m_propertyNames;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体类型。
		/// </summary>
		/// <param name="type">实体类型。</param>
		public PropertyChainBuilder(Type type)
		{
			#region 前置条件

			Debug.Assert((type != null), "实体类型参数不能为空。");

			#endregion

			m_type = type;
			m_propertyNames = new List<String>();
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取目标实体类型。
		/// </summary>
		public Type Type { get { return m_type; } }

		#endregion

		#region 内部方法

		/// <summary>
		/// 链接属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		internal void LinkProperty(String propertyName)
		{
			m_propertyNames.Add(propertyName);
		}

		#endregion

		#region IPropertyChainBuilder 成员

		/// <summary>
		/// 获取一个值，指示当前可否生成属性链表。
		/// </summary>
		public Boolean Available
		{
			get { return (m_propertyNames.Count != 0); }
		}

		/// <summary>
		/// 生成属性链表。
		/// </summary>
		/// <returns>生成好的属性链表。</returns>
		public IPropertyChain Build()
		{
			#region 前置条件

			Debug.Assert(Available, "属性列表为空，应至少设置一个属性。");

			#endregion

			PropertyChain chain = new PropertyChain(m_type, m_propertyNames.ToArray());

			return chain;
		}

		#endregion
	}
}