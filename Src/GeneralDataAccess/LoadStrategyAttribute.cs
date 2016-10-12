#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：LoadStrategyAttribute.cs
// 文件功能描述：实体加载策略，当调用数据库会话引擎时，如果未指定策略，则使用标记于实体上的策略。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110519
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
	/// 实体加载策略，当调用数据库会话引擎时，如果未指定策略，则使用标记于实体上的策略。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class LoadStrategyAttribute : Attribute
	{
		#region 私有字段

		private readonly LoadStrategyOption m_loadStrategyOption;
		private Int32 m_level;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置策略选项。
		/// </summary>
		/// <param name="option">策略选项。</param>
		public LoadStrategyAttribute(LoadStrategyOption option)
		{
			m_loadStrategyOption = option;
		}

		/// <summary>
		/// 构造函数，设置策略选项和级别。
		/// </summary>
		/// <param name="option">策略选项。</param>
		/// <param name="level">加载级别。</param>
		public LoadStrategyAttribute(LoadStrategyOption option, Int32 level)
			: this(option)
		{
			this.Level = level;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取加载策略选项。
		/// </summary>
		public LoadStrategyOption LoadStrategyOption
		{
			get { return m_loadStrategyOption; }
		}

		/// <summary>
		/// 获取或设置加载级别，仅当 LoadStrategyOption 为 SpecifyLevel 时有效，且不能小于零，默认为零。
		/// </summary>
		public Int32 Level
		{
			get { return m_level; }
			set { m_level = value; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 根据加载策略标记创建实体架构组合生成策略。
		/// </summary>
		/// <returns>创建好的实体架构组合生成策略。</returns>
		public CompositeBuilderStrategy Create()
		{
			switch (m_loadStrategyOption)
			{
				case LoadStrategyOption.Auto:
				default:
					return CompositeBuilderStrategyFactory.Default;

				case LoadStrategyOption.SelfOnly:
					return CompositeBuilderStrategyFactory.SelfOnly;

				case LoadStrategyOption.OneLevel:
					return CompositeBuilderStrategyFactory.OneLevel;

				case LoadStrategyOption.UnlimitedLevel:
					return CompositeBuilderStrategyFactory.UnlimitedLevel;

				case LoadStrategyOption.SpecifyLevel:
					return CompositeBuilderStrategyFactory.Create(m_level);
			}
		}

		#endregion
	}
}