#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeAttribute.cs
// 文件功能描述：用于标记复合实体。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110707
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
	/// 用于标记复合实体。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class CompositeAttribute : Attribute
	{
		#region 私有字段

		private readonly String m_propertyName;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体类型和在复合实体中的属性名称。
		/// </summary>
		/// <param name="propertyName">在复合实体中的属性名称。</param>
		public CompositeAttribute(String propertyName)
		{
			m_propertyName = propertyName;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取复合实体中根实体的属性名称。
		/// </summary>
		public String PropertyName
		{
			get { return m_propertyName; }
		}

		#endregion
	}
}