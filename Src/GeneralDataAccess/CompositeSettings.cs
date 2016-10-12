#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeSettings.cs
// 文件功能描述：针对复合实体的过滤器集合。
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
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 针对复合实体的过滤器集合。
	/// </summary>
	[Serializable]
	public sealed class CompositeSettings : IEnumerable<CompositeItemSettings>
	{
		#region 私有字段

		private readonly CompositeDefinition m_definition;
		private readonly Dictionary<String, CompositeItemSettings> m_allItemSettings;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置复合实体的类型。
		/// </summary>
		/// <param name="type">复合实体类型。</param>
		public CompositeSettings(Type type)
		{
			m_definition = CompositeDefinitionBuilder.Build(type);
			m_allItemSettings = new Dictionary<String, CompositeItemSettings>();

			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				m_allItemSettings.Add(propertyDef.Name, new CompositeItemSettings(this, propertyDef.Name));
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取指定属性名称的过滤器。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>该属性的过滤器。</returns>
		public CompositeItemSettings this[String propertyName]
		{
			get
			{
				#region 前置条件

				Debug.Assert(m_allItemSettings.ContainsKey(propertyName), String.Format("不存在属性 {0}。", propertyName));

				#endregion

				return m_allItemSettings[propertyName];
			}
		}

		/// <summary>
		/// 获取指定属性的过滤器。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>该属性的过滤器。</returns>
		public CompositeItemSettings this[IPropertyChain chain]
		{
			get { return this[chain.Name]; }
		}

		/// <summary>
		/// 获取指定属性的过滤器。
		/// </summary>
		/// <param name="chainBuilder">属性链生成器。</param>
		/// <returns>该属性的过滤器。</returns>
		public CompositeItemSettings this[IPropertyChainBuilder chainBuilder]
		{
			get { return this[chainBuilder.Build()]; }
		}

		/// <summary>
		/// 获取复合实体类型。
		/// </summary>
		public Type Type
		{
			get { return Definition.Type; }
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取复合实体定义。
		/// </summary>
		internal CompositeDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion

		#region IEnumerable<CompositeItemSettings> 成员

		/// <summary>
		/// 获取 CompositeResultFilter 枚举器。
		/// </summary>
		/// <returns>CompositeResultFilter 枚举器。</returns>
		public IEnumerator<CompositeItemSettings> GetEnumerator()
		{
			IEnumerable<CompositeItemSettings> e = GetCompositeItemSettings();

			return e.GetEnumerator();
		}

		#endregion

		#region IEnumerable 成员

		/// <summary>
		/// 获取枚举器。
		/// </summary>
		/// <returns>枚举器。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 获取复合实体过滤器枚举。
		/// </summary>
		/// <returns>过滤器枚举。</returns>
		private IEnumerable<CompositeItemSettings> GetCompositeItemSettings()
		{
			foreach (CompositeItemSettings itemSettings in m_allItemSettings.Values)
			{
				yield return itemSettings;
			}
		}

		#endregion
	}
}