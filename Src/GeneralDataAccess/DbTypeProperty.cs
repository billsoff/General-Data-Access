#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DbTypeProperty.cs
// 文件功能描述：数据库参数类型属性的名称。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110302
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
	/// 数据库参数类型属性的名称。
	/// </summary>
	public static class DbTypeProperty
	{
		/// <summary>
		/// 属性 MySqlDbType。
		/// </summary>
		public const String MySqlDbType = "MySqlDbType";

		/// <summary>
		/// 属性 OdbcType。
		/// </summary>
		public const String OdbcType = "OdbcType";

		/// <summary>
		/// 属性 OleDbType。
		/// </summary>
		public const String OleDbType = "OleDbType";

		/// <summary>
		/// 属性 OracleType。
		/// </summary>
		public const String OracleType = "OracleType";

		/// <summary>
		/// 属性 SqlDbType。
		/// </summary>
		public const String SqlDbType = "SqlDbType";
	}
}