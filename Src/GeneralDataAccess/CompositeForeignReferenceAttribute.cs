#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeForeignReferenceAttribute.cs
// 文件功能描述：用于指示复合实体中的表达式实体架构。
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
	/// 用于指示复合实体中的表达式实体架构。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class CompositeForeignReferenceAttribute : Attribute
	{
		#region 私有字段

		private readonly PropertyJoinMode m_propertyJoinMode;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性连接模式。
		/// </summary>
		/// <param name="mode">连接模式。</param>
		public CompositeForeignReferenceAttribute(PropertyJoinMode mode)
		{
			m_propertyJoinMode = mode;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取属性连接模式。
		/// </summary>
		public PropertyJoinMode PropertyJoinMode
		{
			get { return m_propertyJoinMode; }
		}

		#endregion
	}
}