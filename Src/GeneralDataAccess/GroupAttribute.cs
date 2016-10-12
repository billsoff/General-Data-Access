#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupAttribute.cs
// 文件功能描述：用于标记分组结果实体，指示要分组的实体类型。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110627
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
	/// 用于标记分组结果实体，指示要分组的实体类型。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class GroupAttribute : Attribute
	{
		#region 私有字段

		private readonly Type m_type;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置要分组的实体的类型。
		/// </summary>
		/// <param name="entityType">要分组的实体的类型。</param>
		public GroupAttribute(Type entityType)
		{
			m_type = entityType;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取要分组的实体的类型。
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion
	}
}