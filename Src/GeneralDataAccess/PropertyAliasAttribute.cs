#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertyAliasAttribute.cs
// 文件功能描述：用于标记属性别名。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110329
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
	/// 用于标记属性别名。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public abstract class PropertyAliasAttribute : Attribute
	{
		#region 私有字段

		private readonly String m_alias;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性别名。
		/// </summary>
		/// <param name="alias">要设置的别名。</param>
		protected PropertyAliasAttribute(String alias)
		{
			m_alias = alias;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取别名。
		/// </summary>
		public String Alias
		{
			get { return m_alias; }
		}

		#endregion
	}
}