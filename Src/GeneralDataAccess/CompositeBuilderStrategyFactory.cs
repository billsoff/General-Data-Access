#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeBuilderStrategyFactory.cs
// 文件功能描述：生成策略工厂。
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

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 生成策略工厂。
	/// </summary>
	public static class CompositeBuilderStrategyFactory
	{
		/// <summary>
		/// 默认生成策略。
		/// </summary>
		public static CompositeBuilderStrategy Default
		{
			get { return AcceptAllExpandable; }
		}

		/// <summary>
		/// 生成策略，仅加载自身。
		/// </summary>
		public static CompositeBuilderStrategy SelfOnly
		{
			get { return new CompositeSpecifyLevelBuilderStrategy(0); }
		}

		/// <summary>
		/// 生成策略，加载第一级外部引用。
		/// </summary>
		public static CompositeBuilderStrategy OneLevel
		{
			get { return new CompositeSpecifyLevelBuilderStrategy(1); }
		}

		/// <summary>
		/// 展开所有级别，除非在属性上标记了 SuppressExpand。
		/// </summary>
		public static CompositeBuilderStrategy AcceptAllExpandable
		{
			get { return new CompositeAcceptAllExpandableBuilderStrategy(); }
		}

		/// <summary>
		/// 生成策略，不限制级别，
		/// <para>同一属性路径上类型重复的引用加载到第二级（包含）为止。</para>
		/// </summary>
		public static CompositeBuilderStrategy UnlimitedLevel
		{
			get { return new CompositeSpecifyLevelBuilderStrategy(-1); }
		}

		/// <summary>
		/// 不选择任何属性。
		/// </summary>
		public static CompositeBuilderStrategy Nothing
		{
			get { return CompositeNothingBuilderStrategy.Value; }
		}

		/// <summary>
		/// 为实体合成生成策略。
		/// <para>如果没有显式指定加载策略，则使用在实体上声明的加载策略。</para>
		/// <para>如果既未显式指定也未声明加载策略，则使用默认策略。</para>
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <param name="loadStrategy">加载策略。</param>
		/// <param name="additionalSelectors">附加的选择器列表，可以为空。</param>
		/// <returns>合成的生成策略。</returns>
		public static CompositeBuilderStrategy Compose(Type entityType, CompositeBuilderStrategy loadStrategy, IList<PropertySelector> additionalSelectors)
		{
			if (loadStrategy == null)
			{
				loadStrategy = Create(entityType);
			}

			if ((additionalSelectors != null) && (additionalSelectors.Count != 0))
			{
				CompositeBuilderStrategy additional = Create(additionalSelectors);

				CompositeBuilderStrategy result = Union(loadStrategy, additional);

				return result;
			}
			else
			{
				return loadStrategy;
			}
		}

		/// <summary>
		/// 设置加载级别。
		/// </summary>
		/// <para>如果指定的级别为负数，则不限定级别，此时，同一属性路径上类型重复的引用加载到第二级（包含）为止。</para>
		/// <param name="level">加载级别。</param>
		public static CompositeBuilderStrategy Create(Int32 level)
		{
			return new CompositeSpecifyLevelBuilderStrategy(level);
		}

		/// <summary>
		/// 显式选择属性。
		/// </summary>
		/// <param name="allSelectors">属性选择器列表。</param>
		/// <returns>策略。</returns>
		public static CompositeBuilderStrategy Create(IList<PropertySelector> allSelectors)
		{
			if ((allSelectors == null) || (allSelectors.Count == 0))
			{
				return Nothing;
			}
			else
			{
				return new CompositeExplicitSelectionBulderStrategy(allSelectors);
			}
		}

		/// <summary>
		/// 为实体类型创建生成策略。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>创建好的生成策略。</returns>
		public static CompositeBuilderStrategy Create(Type entityType)
		{
			EntityDefinition definition = EntityDefinitionBuilder.Build(entityType);

			return definition.LoadStrategy.Create();
		}

		/// <summary>
		/// 创建用于分组的生成策略。
		/// </summary>
		/// <param name="groupResultType">分组结果实体类型。</param>
		/// <returns>创建好的生成策略。</returns>
		public static CompositeBuilderStrategy Create4Group(Type groupResultType)
		{
			return new CompositeGroupBuilderStrategy(groupResultType);
		}

		/// <summary>
		/// 对生成策略进行修剪。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪的生成策略。</param>
		/// <param name="trimmers">修剪器集合。</param>
		/// <returns>修剪后的生成策略。</returns>
		public static CompositeBuilderStrategy TrimOff(CompositeBuilderStrategy builderStrategyToTrim, params PropertyTrimmer[] trimmers)
		{
			#region 前置条件

			Debug.Assert(builderStrategyToTrim != null, "参数 builderStrategyToTrim 不能为空。");
			Debug.Assert((trimmers != null) && (trimmers.Length != 0), "参数 trimmers 不能为空或空数组。");

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, trimmers);
		}

		/// <summary>
		/// 对生成策略进行修剪。
		/// </summary>
		/// <param name="builderStategyToTrim">要修剪的生成策略。</param>
		/// <param name="trimmingProperties">要修剪掉的属性列表。</param>
		/// <returns>修剪后的生成策略。</returns>
		public static CompositeBuilderStrategy TrimOff(CompositeBuilderStrategy builderStategyToTrim, params IPropertyChain[] trimmingProperties)
		{
			#region 前置条件

#if DEBUG

			Debug.Assert(builderStategyToTrim != null, "要修剪的生成策略参数 builderStategyToTrim 不能为空。");
			Debug.Assert(
					(trimmingProperties != null) && (trimmingProperties.Length != 0),
					"要修剪掉的属性列表参数 trimmingProperties 不能为空或空列表。"
				);

			foreach (IPropertyChain chain in trimmingProperties)
			{
				if (chain.IsChildren)
				{
					Debug.Fail(String.Format("要移除的属性链列表中的属性链 {0} 是子实体列表属性，没有意义。", chain.FullName));
				}
			}

#endif

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStategyToTrim, trimmingProperties);
		}

		/// <summary>
		/// 对生成策略进行修剪。
		/// </summary>
		/// <param name="builderStategyToTrim">要修剪的生成策略。</param>
		/// <param name="trimmingProperties">要修剪掉的属性列表。</param>
		/// <returns>修剪后的生成策略。</returns>
		public static CompositeBuilderStrategy TrimOff(CompositeBuilderStrategy builderStategyToTrim, params IPropertyChainBuilder[] trimmingProperties)
		{
			#region 前置条件

#if DEBUG

			Debug.Assert(builderStategyToTrim != null, "要修剪的生成策略参数 builderStategyToTrim 不能为空。");
			Debug.Assert(
					(trimmingProperties != null) && (trimmingProperties.Length != 0),
					"要修剪掉的属性列表参数 trimmingProperties 不能为空或空列表。"
				);

			foreach (IPropertyChainBuilder builder in trimmingProperties)
			{
				IPropertyChain chain = builder.Build();

				if (chain.IsChildren)
				{
					Debug.Fail(String.Format("要移除的属性链列表中的属性链 {0} 是子实体列表属性，没有意义。", chain.FullName));
				}
			}

#endif

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStategyToTrim, trimmingProperties);
		}

		/// <summary>
		/// 修剪实体中直属的非主键的属性。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪的策略。</param>
		/// <returns>修剪后的策略。</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim)
		{
			#region 前置条件

			Debug.Assert(builderStrategyToTrim != null, "要修剪的生成策略参数 builderStrategyToTrim 不能为空。");

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, new NonPrimaryKeyPropertyTrimmer());
		}

		/// <summary>
		/// 修剪外部引用中的非主键属性。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪的策略。</param>
		/// <param name="from">要从中修剪的外部引用属性。</param>
		/// <returns>修剪后的策略。</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim, IPropertyChain from)
		{
			#region 前置条件

			Debug.Assert(builderStrategyToTrim != null, "要修剪的生成策略参数 builderStrategyToTrim 不能为空。");
			Debug.Assert(from != null, "要从中修剪的外部引用属性参数 from 不能为空。");

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, new NonPrimaryKeyPropertyTrimmer(from));
		}

		/// <summary>
		/// 修剪外部引用列表中非主键属性。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪的策略。</param>
		/// <param name="fromList">外部引用列表。</param>
		/// <returns>修剪后的策略。</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim, IPropertyChain[] fromList)
		{
			#region 前置条件

			Debug.Assert(builderStrategyToTrim != null, "要修剪的生成策略参数 builderStrategyToTrim 不能为空。");
			Debug.Assert((fromList != null) && (fromList.Length != 0), "要从中修剪的外部引用列表参数 fromList 不能为空或空列表。");

			#endregion

			PropertyTrimmer[] allTrimmers = Array.ConvertAll<IPropertyChain, PropertyTrimmer>(
					fromList,
					delegate(IPropertyChain chain)
					{
						return new NonPrimaryKeyPropertyTrimmer(chain);
					}
				);

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, allTrimmers);
		}

		/// <summary>
		/// 修剪外部引用属性中的非主键属性。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪的生成策略。</param>
		/// <param name="from">要从中修剪的外部引用属性。</param>
		/// <returns>修剪后的策略。</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim, IPropertyChainBuilder from)
		{
			#region 前置条件

			Debug.Assert(builderStrategyToTrim != null, "要修剪的生成策略参数 builderStrategyToTrim 不能为空。");
			Debug.Assert(from != null, "要从中修剪的外部引用属性链生成器参数 from 不能为空。");

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, new NonPrimaryKeyPropertyTrimmer(from));
		}

		/// <summary>
		/// 修剪外部引用列表中的非主键属性。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪的生成策略。</param>
		/// <param name="fromList">外部引用列表。</param>
		/// <returns>修剪后的生成策略。</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim, IPropertyChainBuilder[] fromList)
		{
			#region 前置条件

			Debug.Assert(builderStrategyToTrim != null, "要修剪的生成策略参数 builderStrategyToTrim 不能为空。");
			Debug.Assert((fromList != null) && (fromList.Length != 0), "要从中修剪的外部引用列表参数 fromList 不能为空或空列表。");

			#endregion

			PropertyTrimmer[] allTrimmers = Array.ConvertAll<IPropertyChainBuilder, PropertyTrimmer>(
					fromList,
					delegate(IPropertyChainBuilder builder)
					{
						return new NonPrimaryKeyPropertyTrimmer(builder);
					}
				);

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, allTrimmers);
		}

		/// <summary>
		/// 合并策略，如果合并的策略列表为空，则返回 null。
		/// </summary>
		/// <param name="strategies">要合并的策略列表。</param>
		/// <returns>合并结果，一个新的 CompositeBuilderStrategy 实例，或者为 null，如果策略列表为空。</returns>
		public static CompositeBuilderStrategy Union(params CompositeBuilderStrategy[] strategies)
		{
			if ((strategies == null) || (strategies.Length == 0))
			{
				return null;
			}

			List<CompositeBuilderStrategy> results = new List<CompositeBuilderStrategy>();

			foreach (CompositeBuilderStrategy strategy in strategies)
			{
				if (strategy != null)
				{
					results.Add(strategy);
				}
			}

			if (results.Count != 0)
			{
				if (results.Count == 1)
				{
					return results[0];
				}
				else
				{
					return new CompositeUnionBuilderStrategy(results);
				}
			}
			else
			{
				return null;
			}
		}
	}
}