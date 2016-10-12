#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：LoadSchemaOnlyPropertySelector.cs
// 文件功能描述：指示仅加载实体。
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
	/// 指示仅加载实体。
	/// </summary>
	[Serializable]
	internal sealed class LoadSchemaOnlyPropertySelector : PropertySelector
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置要加载的目标实体。
		/// </summary>
		/// <param name="entityType"></param>
		public LoadSchemaOnlyPropertySelector(Type entityType)
			: base(entityType)
		{
		}

		/// <summary>
		/// 构造函数，设置要加载的外部引用属性。
		/// </summary>
		/// <param name="chain">属性链。</param>
		public LoadSchemaOnlyPropertySelector(IPropertyChain chain)
			: base(chain)
		{
			#region 前置条件

			Debug.Assert(!chain.IsPrimitive, "当构造 LoadSchemaOnlyPropertySelector 实例时， 属性链参数 chain 必须映射为外部引用属性。");

			#endregion
		}

		/// <summary>
		/// 构造函数，设置要加载的外部引用属性。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		public LoadSchemaOnlyPropertySelector(IPropertyChainBuilder builder)
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
				return Name + ".[Nothing]";
			}
		}

		/// <summary>
		/// Nothing。
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.LoadSchemaOnly; }
		}

		/// <summary>
		/// 总是返回 true。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns></returns>
		protected override Boolean SelectNothingFromImpl(EntitySchema schema)
		{
			return true;
		}

		/// <summary>
		/// 总是返回 false。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return false;
		}
	}
}