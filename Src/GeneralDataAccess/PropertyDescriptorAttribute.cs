#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertyDescriptorAttribute.cs
// 文件功能描述：用于标记属性描述符，指示属性描述符的目标类型。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110713
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
	/// 用于标记属性描述符，指示属性描述符的目标类型。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class PropertyDescriptorAttribute : Attribute
	{
		#region

		private readonly Type m_type;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置目标类型。
		/// </summary>
		/// <param name="type">目标类型。</param>
		public PropertyDescriptorAttribute(Type type)
		{
			m_type = type;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取目标类型。
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion
	}
}