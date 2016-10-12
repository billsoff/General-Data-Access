#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：MaxAttribute.cs
// 文件功能描述：用于标记对列求最大值的聚合计算的属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110702
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
	/// 用于标记对列求最大值的聚合计算的属性。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class MaxAttribute : AggregationAttribute
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置聚合的属性。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		public MaxAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
		}

		#endregion

		/// <summary>
		/// MAX ( [ ALL | DISTINCT ] expression )
		/// </summary>
		protected override String FunctionName
		{
			get
			{
				return "MAX";
			}
		}

		/// <summary>
		/// 获取聚合名称。
		/// </summary>
		public override String Name
		{
			get { return Distinct ? "Max Distinct" : "Max"; }
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