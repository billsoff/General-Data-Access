#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeRootPropertyDefinition.cs
// 文件功能描述：复合实体根属性定义。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110708
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 复合实体根属性定义。
	/// </summary>
	internal sealed class CompositeRootPropertyDefinition : CompositePropertyDefinition
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性信息。
		/// </summary>
		/// <param name="definition">复合实体定义。</param>
		/// <param name="pf">设置属性信息。</param>
		public CompositeRootPropertyDefinition(CompositeDefinition definition, PropertyInfo pf)
			: base(definition, pf)
		{
		}

		#endregion
	}
}