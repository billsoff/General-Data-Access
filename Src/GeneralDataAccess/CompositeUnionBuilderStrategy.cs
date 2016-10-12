#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeUnionBuilderStrategy.cs
// 文件功能描述：生成策略的并集。
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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 生成策略的并集。
	/// </summary>
	[Serializable]
	internal sealed class CompositeUnionBuilderStrategy : CompositeBuilderStrategy
	{
		#region 私有字段

		private readonly List<CompositeBuilderStrategy> m_strategies;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置策略列表，策略列表不能为空。
		/// </summary>
		/// <param name="strategies">策略列表。</param>
		public CompositeUnionBuilderStrategy(IList<CompositeBuilderStrategy> strategies)
		{
			#region 前置条件

#if DEBUG

			Debug.Assert(((strategies != null) && (strategies.Count != 0)), "策略列表不能为空。");

			// 应至少有一个元素不为空
			Boolean atLeastOneNotNull = false;

			foreach (CompositeBuilderStrategy strategy in strategies)
			{
				if (strategy != null)
				{
					atLeastOneNotNull = true;

					break;
				}
			}

			Debug.Assert(atLeastOneNotNull, "策略列表中所有的元素都为空。");

#endif

			#endregion

			m_strategies = new List<CompositeBuilderStrategy>();

			foreach (CompositeBuilderStrategy strategy in strategies)
			{
				if ((strategy == null) || m_strategies.Contains(strategy))
				{
					continue;
				}

				if (!(strategy is CompositeUnionBuilderStrategy))
				{
					m_strategies.Add(strategy);
				}
				else
				{
					foreach (CompositeBuilderStrategy childStrategy in ((CompositeUnionBuilderStrategy)strategy).Strategies)
					{
						if (!m_strategies.Contains(childStrategy))
						{
							m_strategies.Add(childStrategy);
						}
					}
				}
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取策略列表。
		/// </summary>
		public IList<CompositeBuilderStrategy> Strategies
		{
			get { return m_strategies; }
		}

		#endregion

		#region 策略

		/// <summary>
		/// 不限制级别。
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// 指示是否选择全部属性。
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				foreach (CompositeBuilderStrategy strategy in Strategies)
				{
					if (!strategy.SelectAllProperties)
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// 指示是否加载实体架构。
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			foreach (CompositeBuilderStrategy strategy in Strategies)
			{
				if (strategy.LoadFromSchema(schema))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 判断是否不选择实体架构的任何属性。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果策略列表中的每一项策略都指示不选择任何属性，则返回 true；否则返回 false。</returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			foreach (CompositeBuilderStrategy strategy in Strategies)
			{
				if (!strategy.SelectNothingFrom(schema))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 指示是否选择属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			foreach (CompositeBuilderStrategy strategy in Strategies)
			{
				if (strategy.SelectAllProperties || strategy.SelectProperty(property))
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
			const String PADDING = "    "; // 缩进 4 个空格
			Regex ex = new Regex("^", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			StringBuilder buffer = new StringBuilder();

			foreach (CompositeBuilderStrategy strategy in Strategies)
			{
				if (buffer.Length != 0)
				{
					buffer.AppendLine();
				}

				buffer.AppendLine(ex.Replace(strategy.Dump(), PADDING));
			}

			return String.Format(
					"{0}，是如下策略的并集：\r\n{1}",
					GetType().FullName,
					buffer
				);
		}

		#endregion
	}
}