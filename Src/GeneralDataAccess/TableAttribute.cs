#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：TableAttribute.cs
// 文件功能描述：用于标记实体，使其与一个数据库表建立映射关系。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008132245
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
	/// 用于标记实体，使其与一个数据库表建立映射关系。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class TableAttribute : Attribute
	{
		private readonly String m_name;
		private Boolean m_isStoredProcedure;

		#region 构造函数

		/// <summary>
		/// 构造函数，设置映射表名。
		/// </summary>
		/// <param name="name">映射表名。</param>
		public TableAttribute(String name)
		{
			m_name = name;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取映射表名。
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// 获取或设置一个值，该值指示数据源是否为存储过程。
		/// </summary>
		public Boolean IsStoredProcedure
		{
			get { return m_isStoredProcedure; }
			set { m_isStoredProcedure = value; }
		}

		#endregion
	}
}