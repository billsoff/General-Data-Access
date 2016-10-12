#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupByAttribute.cs
// 文件功能描述：用于标记做为分组依据的属性。
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
	/// 用于标记做为分组依据的属性。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class GroupByAttribute : AggregationAttribute
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置要分组的属性的路径。
		/// </summary>
		/// <param name="propertyPath">要分组的属性的路径。</param>
		public GroupByAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
		}

		#endregion

		/// <summary>
		/// 指示标记的属性为分组项。
		/// </summary>
		public override Boolean IsGroupItem
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// 获取聚合表达式。
		/// </summary>
		/// <param name="columnName">列的限定名。</param>
		/// <returns>原样返回。</returns>
		public override String GetAggregationExpression(String columnName)
		{
			return columnName;
		}

		/// <summary>
		/// 获取聚合名称。
		/// </summary>
		public override String Name
		{
			get { return "GroupBy"; }
		}

		/// <summary>
		/// 字符串表示。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return "GROUP BY ...";
		}
	}
}