#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ColumnAttribute.cs
// 文件功能描述：用于标记实体中与表的列有映射关系的值属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008132251
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于标记实体中与表的列有映射关系的值属性。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ColumnAttribute : Attribute
	{
		#region 私有字段

		private readonly String m_name;
		private readonly DbType m_dbType;

		private Boolean m_isPrimaryKey;

		#endregion

		/// <summary>
		/// 构造函数，设置列名和数据库类型。
		/// </summary>
		/// <param name="name">列名。</param>
		/// <param name="dbType">数据库名称。</param>
		public ColumnAttribute(String name, DbType dbType)
		{
			m_name = name;
			m_dbType = dbType;
		}

		#region 公共属性

		/// <summary>
		/// 获取列名称。
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// 获取数据库类型。
		/// </summary>
		public DbType DbType
		{
			get { return m_dbType; }
		}

		/// <summary>
		/// 获取一个值，该值指示当前列是否为主键，默认为 false。
		/// </summary>
		public Boolean IsPrimaryKey
		{
			get { return m_isPrimaryKey; }
			set { m_isPrimaryKey = value; }
		}

		#endregion
	}
}