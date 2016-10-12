#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AssemblyPolicyBuilder.cs
// 文件功能描述：装配方针生成器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110727
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
	/// 装配方针生成器。
	/// </summary>
	public sealed class AssemblyPolicyBuilder
	{
		#region 私有字段

		private CompositeBuilderStrategy m_defaultBuilderStrategy;
		private readonly Dictionary<String, CompositeBuilderStrategy> m_children = new Dictionary<String, CompositeBuilderStrategy>();

		#endregion

		#region 公共方法

		#region 方针构造

		/// <summary>
		/// 设置装配方针的默认加载策略。
		/// </summary>
		/// <param name="strategyOption">策略选项。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Using(LoadStrategyOption strategyOption)
		{
			LoadStrategyAttribute attr = new LoadStrategyAttribute(strategyOption);
			CompositeBuilderStrategy strategy = attr.Create();

			return Using(strategy);
		}

		/// <summary>
		/// 设置装配方针的默认加载策略。
		/// </summary>
		/// <param name="level">加载级别。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Using(Int32 level)
		{
			return Using(CompositeBuilderStrategyFactory.Create(level));
		}

		/// <summary>
		/// 设置装配方针的默认加载策略，注意，此策略不能和类型相关。
		/// </summary>
		/// <param name="builderStrategy">默认加载策略。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Using(CompositeBuilderStrategy builderStrategy)
		{
			m_defaultBuilderStrategy = builderStrategy;

			return this;
		}

		/// <summary>
		/// 设置要加载的子属性。
		/// </summary>
		/// <param name="builder">子属性生成器。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(IPropertyChainBuilder builder)
		{
			return Children(builder.Build(), (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="builder">子属性生成器。</param>
		/// <param name="strategyOption">策略选项。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, LoadStrategyOption strategyOption)
		{
			return Children(builder.Build(), strategyOption);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="builder">子属性生成器。</param>
		/// <param name="level">加载级别。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, Int32 level)
		{
			return Children(builder.Build(), level);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="builder">子属性生成器。</param>
		/// <param name="builderStrategy">加载策略。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, CompositeBuilderStrategy builderStrategy)
		{
			return Children(builder.Build(), builderStrategy);
		}

		/// <summary>
		/// 设置要加载的子属性。
		/// </summary>
		/// <param name="chain">子属性。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(IPropertyChain chain)
		{
			return Children(chain, (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="chain">子属性。</param>
		/// <param name="strategyOption">策略选项。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(IPropertyChain chain, LoadStrategyOption strategyOption)
		{
			return Children(chain.PropertyPath, strategyOption);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="chain">子属性。</param>
		/// <param name="level">加载级别。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(IPropertyChain chain, Int32 level)
		{
			return Children(chain.PropertyPath, level);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="chain">子属性。</param>
		/// <param name="builderStrategy">加载策略。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(IPropertyChain chain, CompositeBuilderStrategy builderStrategy)
		{
			return Children(chain.PropertyPath, builderStrategy);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="propertyPath">子属性路径。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(String[] propertyPath)
		{
			return Children(propertyPath, (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="propertyPath">子属性路径。</param>
		/// <param name="strategyOption">策略选项。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(String[] propertyPath, LoadStrategyOption strategyOption)
		{
			LoadStrategyAttribute attr = new LoadStrategyAttribute(strategyOption);
			CompositeBuilderStrategy strategy = attr.Create();

			return Children(propertyPath, strategy);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="propertyPath">子属性路径。</param>
		/// <param name="level">加载级别。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(String[] propertyPath, Int32 level)
		{
			return Children(propertyPath, CompositeBuilderStrategyFactory.Create(level));
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="propertyPath">子属性路径。</param>
		/// <param name="builderStrategy">加载策略。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicyBuilder Children(String[] propertyPath, CompositeBuilderStrategy builderStrategy)
		{
			for (Int32 i = 0; i < propertyPath.Length; i++)
			{
				String fullName = String.Join(CommonPolicies.DOT, propertyPath, 0, (i + 1));

				if (!m_children.ContainsKey(fullName))
				{
					m_children.Add(fullName, builderStrategy);
				}
				else if ((m_children[fullName] == null) && (builderStrategy != null))
				{
					m_children[fullName] = builderStrategy;
				}
			}

			return this;
		}

		#endregion

		/// <summary>
		/// 生成装配策略。
		/// </summary>
		/// <returns></returns>
		public AssemblyPolicy Build()
		{
			if (m_children.Count == 0)
			{
				return null;
			}

			List<String> allProperties = new List<String>(m_children.Keys);
			allProperties.Sort(
					delegate(String left, String right)
					{
						String[] leftPropertyPath = left.Split(CommonPolicies.DOT.ToCharArray());
						String[] rightPropertyPath = right.Split(CommonPolicies.DOT.ToCharArray());

						return leftPropertyPath.Length.CompareTo(rightPropertyPath.Length);
					}
				);

			AssemblyPolicy policy = AssemblyPolicy.CreateRoot();

			foreach (String fullName in allProperties)
			{
				AssemblyChildrenEntry parent = policy.GetParentEntry(fullName);
				String propertyName = CommonPolicies.GetPropertyName(fullName);
				CompositeBuilderStrategy builderStrategy = m_children[fullName] ?? m_defaultBuilderStrategy;

				AssemblyChildrenEntry current = new AssemblyChildrenEntry(policy, parent, propertyName, builderStrategy);

				if (parent == null)
				{
					policy.InnerList.Add(current);
				}
				else
				{
					parent.InnerList.Add(current);
				}
			}

			return policy;
		}

		#endregion

		#region 类型强制转换

		/// <summary>
		/// 隐式类型强制转换，生成装配方针。
		/// </summary>
		/// <param name="builder">生成器。</param>
		/// <returns>生成好的装配方针。</returns>
		public static implicit operator AssemblyPolicy(AssemblyPolicyBuilder builder)
		{
			return builder.Build();
		}

		#endregion
	}
}