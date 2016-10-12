#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeExplicitSelectionBulderStrategy.cs
// 文件功能描述：用于显式选择属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110518
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于显式选择属性。
	/// </summary>
	[Serializable]
	internal sealed class CompositeExplicitSelectionBulderStrategy : CompositeBuilderStrategy
	{
		#region 私有字段

		private readonly Boolean m_hasSelectors;

		/// <summary>
		/// 实体类型。
		/// </summary>
		private readonly Type m_type;

		private readonly IList<PropertySelector> m_selectorList;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性选择器集合。
		/// </summary>
		/// <param name="allSelectors">属性选择器集合。</param>
		public CompositeExplicitSelectionBulderStrategy(IList<PropertySelector> allSelectors)
		{
			#region 前置条件

#if DEBUG

			Debug.Assert(((allSelectors != null) && (allSelectors.Count != 0)), "属性选择器列表参数 selectors 不能为空或空列表。");

			// 验证束定于同一个实体
			Type t = null;

			foreach (PropertySelector selector in allSelectors)
			{
				if (t == null)
				{
					t = selector.Type;
				}
				else if (selector.Type != t)
				{
					Debug.Fail("属性选择器列表中有一项与前面项的实体类型不同。");
				}
			}

#endif

			#endregion

			// 不选择任何属性
			if ((allSelectors == null) || (allSelectors.Count == 0))
			{
				return;
			}

			m_hasSelectors = true;
			m_type = allSelectors[0].Type;
			m_selectorList = allSelectors;
		}

		#endregion

		#region 策略

		/// <summary>
		/// 计算所有级别。
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// 显式选择。
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 加载实体。
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			if (!m_hasSelectors)
			{
				return false;
			}

			foreach (PropertySelector selector in m_selectorList)
			{
				if (selector.LoadSchema(schema))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 根据属性选择器判断是否仅加载实体（不选择任何属性）。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果不选择任何属性，则返回 true；否则返回 false。</returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			if (!m_hasSelectors)
			{
				return true;
			}

			foreach (PropertySelector selector in m_selectorList)
			{
				if (!selector.SelectNothingFrom(schema))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 选择属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			if (!m_hasSelectors)
			{
				return false;
			}

			foreach (PropertySelector selector in m_selectorList)
			{
				if (selector.SelectProperty(property))
				{
					return true;
				}
			}

			return false;
		}

		#endregion

		#region 用于调试的方法

		/// <summary>
		/// 获取生成策略的详细信息，用于调试。
		/// </summary>
		/// <returns>生成策略的详细信息。</returns>
		public override String Dump()
		{
			if (!m_hasSelectors)
			{
				return String.Format("{0}，不选择任何属性。", GetType().FullName);
			}

			const String PADDING = "    "; // 缩进 4 个空格
			StringBuilder buffer = new StringBuilder();

			foreach (PropertySelector selector in m_selectorList)
			{
				buffer.AppendLine(PADDING + selector.DisplayName);
			}

			return String.Format(
					"{0}，选择如下属性：\r\n{1}",
					GetType().FullName,
					buffer
				);
		}

		#endregion
	}
}