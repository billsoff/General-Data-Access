#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeAcceptAllExpandableBuilderStrategy.cs
// 文件功能描述：实体架构组合生成策略，接受所有从第二级起允许展开的外部引用。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110505
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
	/// 实体架构组合生成策略，接受所有从第二级起允许展开的外部引用。
	/// </summary>
	[Serializable]
	internal sealed class CompositeAcceptAllExpandableBuilderStrategy : CompositeBuilderStrategy
	{
		#region 静态成员

		/// <summary>
		/// 实例。
		/// </summary>
		public static readonly CompositeAcceptAllExpandableBuilderStrategy Instance = new CompositeAcceptAllExpandableBuilderStrategy();

		#endregion

		#region 策略

		/// <summary>
		/// 最大外部引用级别，如果小于零，则表示不限制。
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get { return -1; }
		}

		/// <summary>
		/// 获取一个值，该值指示是否选择除标记为延迟加载的所有属性。
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get { return false; }
		}

		/// <summary>
		/// 指示是否加载实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果加载此实体架构，则返回 true；否则返回 false。</returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			// 总是取第一级外部引用
			if (schema.Level <= (InitialLevel + 1))
			{
				return true;
			}

			// 查找在目标类型中相映射的属性是否标记为阻止扩展
			return MayExpand(schema) && !IsSchemaTypeRepetitive(schema);
		}

		/// <summary>
		/// 指示是否选择属性，如果 SelectAllExceptLazyLoadProperties 为 true，则不会调用此方法过滤要选择的实体属性。
		/// </summary>
		/// <param name="property">实体属性。</param>
		/// <returns>如果要选择此属性，则返回 true；否则返回 false。</returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			return !property.LazyLoad;
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 判断当前的实体架构是否标记为阻止继续引用外部实体。
		/// </summary>
		/// <param name="currentSchema">起始的实体架构。</param>
		/// <returns>如果当前实体架构直接或间接地声明阻止扩展，则返回 true；否则返回 false。</returns>
		private Boolean MayExpand(EntitySchema currentSchema)
		{
			EntitySchemaRelation relation = currentSchema.LeftRelation;

			while (relation != null)
			{
				if (relation.ChildProperty.SuppressExpand)
				{
					return false;
				}

				relation = relation.ChildSchema.LeftRelation;
			}

			return true;
		}

		#endregion

		#region 用于调试的方法

		/// <summary>
		/// 获取生成策略的详细信息，用于调试。
		/// </summary>
		/// <returns>生成策略的详细信息。</returns>
		public override String Dump()
		{
			return String.Format(
					"{0}，接受所有从第二级起允许展开的外部引用，同一属性路径上重复的类型展开到第二级（含）外部引用。",
					GetType().FullName
				);
		}

		#endregion
	}
}