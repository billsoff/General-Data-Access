#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AllFromSchemaPropertySelector.cs
// 文件功能描述：指示选择所有的属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110524
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
	/// 指示选择所有的属性。
	/// </summary>
	[Serializable]
	internal sealed class AllFromSchemaPropertySelector : PropertySelector
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体类型，选择目标实体中的所有属性。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		public AllFromSchemaPropertySelector(Type entityType)
			: base(entityType)
		{
		}

		/// <summary>
		/// 构造函数，设置属性链，该链必须为外部引用，选择该外部引用中的所有属性。
		/// </summary>
		/// <param name="chain">属性链。</param>
		public AllFromSchemaPropertySelector(IPropertyChain chain)
			: base(chain)
		{
			#region 前置条件

			Debug.Assert(!chain.IsPrimitive, "当构造 AllFromSchemaPropertySelector 实例时，属性链参数 chain 必须映射为外部引用属性。");

			#endregion
		}

		/// <summary>
		/// 构造函数，由属性链生成器来确定包含属性的实体。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		public AllFromSchemaPropertySelector(IPropertyChainBuilder builder)
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
				return Name + ".*";
			}
		}

		/// <summary>
		/// AllFromSchema。
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.AllFromSchema; }
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
		/// 选择所有位于属性链的属性和当前引用属性所包含的属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return Contains(property) || OwnProperty(property);
		}
	}
}