#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeSpecifyLevelBuilderStrategy.cs
// 文件功能描述：指定加载级别的生成策略，如果指定的级别为负数，则不限定级别，此时，同一属性路径上类型重复的引用加载到第二级（包含）为止。
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
	/// 指定加载级别的生成策略。
	/// <para>如果指定的级别为负数，则不限定级别，此时，同一属性路径上类型重复的引用加载到第二级（包含）为止。</para>
	/// </summary>
	[Serializable]
	internal sealed class CompositeSpecifyLevelBuilderStrategy : CompositeBuilderStrategy
	{
		#region 私有字段

		private readonly Int32 m_level;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，仅加载实体自身。
		/// </summary>
		public CompositeSpecifyLevelBuilderStrategy()
		{
		}

		/// <summary>
		/// 设置加载级别。
		/// </summary>
		/// <para>如果指定的级别为负数，则不限定级别，此时，同一属性路径上类型重复的引用加载到第二级（包含）为止。</para>
		/// <param name="level">加载级别。</param>
		public CompositeSpecifyLevelBuilderStrategy(Int32 level)
		{
			m_level = level;
		}

		#endregion

		#region 策略

		/// <summary>
		/// 最大外部引用级别，如果小于零，则表示不限制。
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get { return m_level; }
		}

		/// <summary>
		/// 指示是否加载实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果加载此实体架构，则返回 true；否则返回 false。</returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			// 如果限制级别，则只加载到限制级别
			if (m_level >= 0)
			{
				return (schema.Level <= (InitialLevel + m_level));
			}
			// 否则，判断架构类型是否重复出现（防止无限递归）
			else
			{
				return !IsSchemaTypeRepetitive(schema);
			}
		}

		/// <summary>
		/// 获取一个值，该值指示是否选择除标记为延迟加载的所有属性。
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get { return false; }
		}

		/// <summary>
		/// 选择所有属性。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>总是返回 false。</returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			return false;
		}

		/// <summary>
		/// 指示是否选择属性，如果 SelectAllProperties 为 true，则不会调用此方法过滤要选择的实体属性。
		/// </summary>
		/// <param name="property">实体属性。</param>
		/// <returns>如果要选择此属性，则返回 true；否则返回 false。</returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			return !property.LazyLoad;
		}


		#endregion

		#region 用于调试的方法

		/// <summary>
		/// 获取生成策略的详细信息，用于调试。
		/// </summary>
		/// <returns>生成策略的详细信息。</returns>
		public override String Dump()
		{
			String description;

			if (m_level > 0)
			{
				description = String.Format("展开到第 {0} 级外部引用", m_level.ToString());
			}
			else if (m_level == 0)
			{
				description = "只加载实体自身";
			}
			else
			{
				description = "展开所有的外部引用，同一属性路径上重复的类型展开到第二级（含）外部引用";
			}

			return String.Format(
					"{0}，{1}。",
					GetType().FullName,
					description
				);
		}

		#endregion
	}
}