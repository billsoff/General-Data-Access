#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeTrimmingBuilderStrategy.cs
// 文件功能描述：去除目标策略中的某些属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110531
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
	/// 去掉目标策略中的某些属性。
	/// </summary>
	[Serializable]
	internal sealed class CompositeTrimmingBuilderStrategy : CompositeBuilderStrategy
	{
		#region 私有字段

		private readonly CompositeBuilderStrategy m_builderStrategy;
		private readonly PropertyTrimmer[] m_propertyTrimmers;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置要修剪的生成器策略和要修剪掉的属性列表。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪的生成器策略。</param>
		/// <param name="trimmingProperties">要修剪的属性列表。</param>
		public CompositeTrimmingBuilderStrategy(CompositeBuilderStrategy builderStrategyToTrim, params IPropertyChainBuilder[] trimmingProperties)
			: this(builderStrategyToTrim, new ActualPropertyTrimmer(trimmingProperties))
		{
		}

		/// <summary>
		/// 构造函数，设置要修剪的生成策略和要修剪掉的属性列表。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪掉的生成策略。</param>
		/// <param name="trimmingProperties">要修剪掉的属性列表。</param>
		public CompositeTrimmingBuilderStrategy(CompositeBuilderStrategy builderStrategyToTrim, params IPropertyChain[] trimmingProperties)
			: this(builderStrategyToTrim, new ActualPropertyTrimmer(trimmingProperties))
		{
		}

		/// <summary>
		/// 构造函数，设置要修剪的生成策略和修剪器。
		/// </summary>
		/// <param name="builderStrategyToTrim">要修剪的生成策略。</param>
		/// <param name="propertyTrimmers">修剪器列表。</param>
		public CompositeTrimmingBuilderStrategy(CompositeBuilderStrategy builderStrategyToTrim, params PropertyTrimmer[] propertyTrimmers)
		{
			#region 前置条件

			Debug.Assert(builderStrategyToTrim != null, "要修剪的生成策略参数 builderStrategyToTrim 不能为空。");
			Debug.Assert((propertyTrimmers != null) && (propertyTrimmers.Length != 0), "生成策略修剪器参数 propertyTrimmer 不能为空。");

			#endregion

			m_builderStrategy = builderStrategyToTrim;
			m_propertyTrimmers = propertyTrimmers;
		}

		#endregion

		#region 策略

		/// <summary>
		/// 获取最大外部引用级别。
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return m_builderStrategy.MaxForeignReferenceLevel;
			}
		}

		/// <summary>
		/// 指示是否选择除延迟加载之外的所有属性。
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 指示是否加载指定的实体架构。
		/// </summary>
		/// <param name="schema">要加载的实体架构。</param>
		/// <returns>如果要加载指定的实体架构，则返回 true；否则返回 false。</returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			return m_builderStrategy.LoadFromSchema(schema);
		}

		/// <summary>
		/// 指示是否不从实体架构中选择任何属性。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果不选择任何属性，则返回 true；否则返回 false。</returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			return m_builderStrategy.SelectNothingFrom(schema);
		}

		/// <summary>
		/// 选择属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			foreach (PropertyTrimmer trimmer in m_propertyTrimmers)
			{
				if (trimmer.TrimOff(property))
				{
					return false;
				}
			}

			return m_builderStrategy.SelectProperty(property);
		}

		#endregion

		#region 调试信息

		/// <summary>
		/// 输出调试信息。
		/// </summary>
		/// <returns></returns>
		public override String Dump()
		{
			String trimmerDescription;

			if (m_propertyTrimmers.Length == 1)
			{
				trimmerDescription = m_propertyTrimmers[0].DisplayName;
			}
			else
			{
				const String PADDING = "    ";
				StringBuilder buffer = new StringBuilder(Environment.NewLine);

				foreach (PropertyTrimmer trimmer in m_propertyTrimmers)
				{
					buffer.AppendLine(PADDING + trimmer.DisplayName);
				}

				trimmerDescription = buffer.ToString();
			}

			return String.Format(
					"{0}，对策略 {1} 进行修剪，修剪器为：{2}，\r\n被修剪的策略为：{3}",
					GetType().FullName,
					m_builderStrategy.GetType().FullName,
					trimmerDescription,
					m_builderStrategy.Dump()
				);
		}

		#endregion
	}
}