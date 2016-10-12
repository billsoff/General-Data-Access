#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AllExceptLazyLoadFromSchemaPropertySelector.cs
// 文件功能描述：选择实体中的所有非延迟加载属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110525
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
	/// 选择实体中的所有非延迟加载属性。
	/// </summary>
	[Serializable]
	internal sealed class AllExceptLazyLoadFromSchemaPropertySelector : PropertySelector
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体类型，选择目标实体中除标记为 LazyLoad 之外的所有属性。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		public AllExceptLazyLoadFromSchemaPropertySelector(Type entityType)
			: base(entityType)
		{
		}

		/// <summary>
		/// 构造函数，设置属性链，该链必须为外部引用，选择该外部引用中除标记为 LazyLoad 之外的所有属性。
		/// </summary>
		/// <param name="chain">属性链。</param>
		public AllExceptLazyLoadFromSchemaPropertySelector(IPropertyChain chain)
			: base(chain)
		{
			#region 前置条件

			Debug.Assert(
					!chain.IsPrimitive && !chain.IsChildren,
					"当构造 AllExceptLazyLoadFromSchemaPropertySelector 实例时，属性链参数 chain 必须映射为外部引用属性。"
				);

			#endregion
		}

		/// <summary>
		/// 构造函数，设置属性链生成器，生成的属性链必须为外部引用属性，选择该外部引用中除标记为 LazyLoad 之外的所有属性。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		public AllExceptLazyLoadFromSchemaPropertySelector(IPropertyChainBuilder builder)
			: base(builder)
		{
		}

		#endregion

		/// <summary>
		/// 获取显示名称。
		/// </summary>
		public override String DisplayName
		{
			get
			{
				return String.Format("{0}.* - {0}.[Lazy Load]", Name);
			}
		}

		/// <summary>
		/// AllExceptLazyLoadFromSchema。
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.AllExceptLazyLoadFromSchema; }
		}

		/// <summary>
		/// 所有位于属性链中的实体架构总是会选择其关联属性。
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean SelectNothingFromImpl(EntitySchema schema)
		{
			return !Contains(schema);
		}

		/// <summary>
		/// 选择所有位于属性链的属性和当前引用属性所包含的非延迟加载属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return Contains(property) || (OwnProperty(property) && !property.LazyLoad);
		}
	}
}