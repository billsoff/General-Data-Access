#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AutoAssembling.cs
// 文件功能描述：用于构造子实体装配方针。
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
	/// 用于构造子实体装配方针。
	/// </summary>
	public static class AutoAssembling
	{
		/// <summary>
		/// 指示装载所有的（包括间接的）子实体。
		/// </summary>
		public static AssemblyPolicy AllChildren
		{
			get { return AssemblyPolicy.CreateLoadAllChildren(); }
		}

		/// <summary>
		/// 设置要加载的子属性。
		/// </summary>
		/// <param name="builder">子属性生成器。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChainBuilder builder)
		{
			return Children(builder.Build(), (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="builder">子属性生成器。</param>
		/// <param name="strategyOption">策略选项。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, LoadStrategyOption strategyOption)
		{
			return Children(builder.Build(), strategyOption);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="builder">子属性生成器。</param>
		/// <param name="level">加载级别。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, Int32 level)
		{
			return Children(builder.Build(), level);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="builder">子属性生成器。</param>
		/// <param name="builderStrategy">加载策略。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, CompositeBuilderStrategy builderStrategy)
		{
			return Children(builder.Build(), builderStrategy);
		}

		/// <summary>
		/// 设置要加载的子属性。
		/// </summary>
		/// <param name="chain">子属性。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChain chain)
		{
			return Children(chain, (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="chain">子属性。</param>
		/// <param name="strategyOption">策略选项。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChain chain, LoadStrategyOption strategyOption)
		{
			return Children(chain.PropertyPath, strategyOption);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="chain">子属性。</param>
		/// <param name="level">加载级别。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChain chain, Int32 level)
		{
			return Children(chain.PropertyPath, level);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="chain">子属性。</param>
		/// <param name="builderStrategy">加载策略。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChain chain, CompositeBuilderStrategy builderStrategy)
		{
			return Children(chain.PropertyPath, builderStrategy);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="propertyPath">子属性路径。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(String[] propertyPath)
		{
			return Children(propertyPath, (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="propertyPath">子属性路径。</param>
		/// <param name="strategyOption">策略选项。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(String[] propertyPath, LoadStrategyOption strategyOption)
		{
			AssemblyPolicyBuilder builder = new AssemblyPolicyBuilder();

			builder.Children(propertyPath, strategyOption);

			return builder;
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="propertyPath">子属性路径。</param>
		/// <param name="level">加载级别。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(String[] propertyPath, Int32 level)
		{
			AssemblyPolicyBuilder builder = new AssemblyPolicyBuilder();

			builder.Children(propertyPath, level);

			return builder;
		}

		/// <summary>
		/// 设置要加载的子属性和加载策略。
		/// </summary>
		/// <param name="propertyPath">子属性路径。</param>
		/// <param name="builderStrategy">加载策略。</param>
		/// <returns>装配方针生成器。</returns>
		public static AssemblyPolicyBuilder Children(String[] propertyPath, CompositeBuilderStrategy builderStrategy)
		{
			AssemblyPolicyBuilder builder = new AssemblyPolicyBuilder();

			builder.Children(propertyPath, builderStrategy);

			return builder;
		}
	}
}