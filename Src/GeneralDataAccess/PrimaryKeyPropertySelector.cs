#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PrimaryKeyPropertySelector.cs
// 文件功能描述：只选择主键。
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
	/// 只选择主键。
	/// </summary>
	[Serializable]
	internal sealed class PrimaryKeyPropertySelector : PropertySelector
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置目标实体类型，选择目标实体中的主键。
		/// </summary>
		/// <param name="entityType"></param>
		public PrimaryKeyPropertySelector(Type entityType)
			: base(entityType)
		{
		}

		/// <summary>
		/// 构造函数，设置引用属性链，选择该引用属性中的主键。
		/// </summary>
		/// <param name="chain"></param>
		public PrimaryKeyPropertySelector(IPropertyChain chain)
			: base(chain)
		{
			#region 前置条件

			Debug.Assert(!chain.IsPrimitive, "当构造 PrimaryKeyPropertySelector 实体时，属性链参数 chain 必须映射到引用属性。");

			#endregion
		}

		/// <summary>
		/// 构造函数，通过属性链生成器设置引用属性。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		public PrimaryKeyPropertySelector(IPropertyChainBuilder builder)
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
				return Name + ".[Primary Key]";
			}
		}

		/// <summary>
		/// PrimaryKey。
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.PrimaryKey; }
		}

		/// <summary>
		/// 总是加载位于属性链中的实体架构的关联属性。
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean SelectNothingFromImpl(EntitySchema schema)
		{
			return !Contains(schema);
		}

		/// <summary>
		/// 加载所有关联属性和当前引用属性中的主键属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return Contains(property) || (property.IsPrimaryKey && OwnProperty(property));
		}
	}
}