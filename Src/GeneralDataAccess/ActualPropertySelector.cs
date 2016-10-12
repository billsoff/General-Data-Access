#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ActualPropertySelector.cs
// 文件功能描述：选择一个属性。
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
	/// 选择一个属性。
	/// </summary>
	[Serializable]
	internal sealed class ActualPropertySelector : PropertySelector
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性链。
		/// </summary>
		/// <param name="chain">属性链。</param>
		public ActualPropertySelector(IPropertyChain chain)
			: base(chain)
		{
		}

		/// <summary>
		/// 构造函数，设置属性链。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		public ActualPropertySelector(IPropertyChainBuilder builder)
			: base(builder)
		{
		}

		#endregion

		/// <summary>
		/// Property。
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.Property; }
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
		/// 选择所有位于属性链中的属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return Contains(property);
		}
	}
}