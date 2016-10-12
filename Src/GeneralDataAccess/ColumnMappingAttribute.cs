#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ColumnMappingAttribute.cs
// 文件功能描述：标记实体属性的列映射关系。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008132332
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
	/// 标记实体属性的列映射关系，对于复合列，应标记多次。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class ColumnMappingAttribute : Attribute
	{
		#region 私有字段

		private readonly String m_childColumnName;
		private readonly String m_parentColumnName;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数。
		/// </summary>
		/// <param name="childColumnName">子列名。</param>
		/// <param name="parentColumnName">父列名。</param>
		public ColumnMappingAttribute(String childColumnName, String parentColumnName)
		{
			m_childColumnName = childColumnName;
			m_parentColumnName = parentColumnName;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取子列名。
		/// </summary>
		public String ChildColumnName
		{
			get { return m_childColumnName; }
		}

		/// <summary>
		/// 获取父列名。
		/// </summary>
		public String ParentColumnName
		{
			get { return m_parentColumnName; }
		}

		#endregion
	}
}