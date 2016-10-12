#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CountAttribute.cs
// 文件功能描述：用于标记对列或记录进行计数聚合计算的属性。
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
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于标记对列或记录进行计数聚合计算的属性。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class CountAttribute : AggregationAttribute
	{
		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public CountAttribute()
			: this((String[])null)
		{
		}

		/// <summary>
		/// 构造函数，设置要计数的属性路径。
		/// </summary>
		/// <param name="propertyPath">要计数的属性路径。</param>
		public CountAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
			AcceptDbType = true;
			DbType = DbType.Int32;
		}

		#endregion

		/// <summary>
		/// 允许列名为空（对记录进行计数）。
		/// </summary>
		protected override Boolean AllowNullColumnName
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// COUNT ( { [ [ ALL | DISTINCT ] expression ] | * } )
		/// </summary>
		protected override String FunctionName
		{
			get
			{
				return "COUNT";
			}
		}

		/// <summary>
		/// 获取聚合名称。
		/// </summary>
		public override String Name
		{
			get { return Distinct ? "Count Distinct" : "Count"; }
		}

		/// <summary>
		/// 字符串表示。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return GetDisplayName();
		}
	}
}