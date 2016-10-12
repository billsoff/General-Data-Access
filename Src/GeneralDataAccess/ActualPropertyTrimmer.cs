#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ActualPropertyTrimmer.cs
// 文件功能描述：用于移除指定的属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110601
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
	/// 用于移除指定的属性。
	/// </summary>
	[Serializable]
	internal sealed class ActualPropertyTrimmer : PropertyTrimmer
	{
		#region 私有字段

		private readonly IPropertyChain[] m_properties;

		#endregion

		#region 构造函数

		/// <summary>
		/// 设置要移除的属性列表。
		/// </summary>
		/// <param name="properties">要移除的属性列表，不能为 null 或空列表。</param>
		public ActualPropertyTrimmer(params IPropertyChain[] properties)
		{
			#region 前置条件

			Debug.Assert((properties != null) && (properties.Length != 0), "要移除的属性集合参数 properties 不能为空或空列表。");

			#endregion

			m_properties = properties;
		}

		/// <summary>
		/// 设置要移除的属性列表。
		/// </summary>
		/// <param name="allBuilders">要移除的属性链生成器列表，不能为 null 或空列表。</param>
		public ActualPropertyTrimmer(params IPropertyChainBuilder[] allBuilders)
		{
			#region 前置条件

			Debug.Assert((allBuilders != null) && (allBuilders.Length != 0), "参数 builders 不能为空或空列表。");

			#endregion

			m_properties = Array.ConvertAll<IPropertyChainBuilder, IPropertyChain>(
					allBuilders,
					delegate(IPropertyChainBuilder builder)
					{
						return builder.Build();
					}
				);
		}

		#endregion

		#region PropertyTrimmer 成员

		/// <summary>
		/// 获取显示名称，用于调试。
		/// </summary>
		public override String DisplayName
		{
			get
			{
				const String PADDING = "    ";
				StringBuilder buffer = new StringBuilder();

				foreach (IPropertyChain chain in m_properties)
				{
					buffer.AppendLine(PADDING + chain.FullName);
				}

				return String.Format("修剪实体中的下列属性：\r\n{0}", buffer.ToString());
			}
		}

		/// <summary>
		/// 如果该属性处于要移除的属性列表中，则指示移除之。
		/// </summary>
		/// <param name="property">属性。</param>
		/// <returns>如果要移除此属性，则返回 true；否则返回 false。</returns>
		public override Boolean TrimOff(EntityProperty property)
		{
			return (Array.IndexOf<IPropertyChain>(m_properties, property.PropertyChain) >= 0);
		}

		#endregion
	}
}