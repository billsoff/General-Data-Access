#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ExpressionTokens.cs
// 文件功能描述：过滤器表达式词法元素。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110327
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
	/// 过滤器表达式词法元素。
	/// </summary>
	[Flags]
	internal enum ExpressionTokens
	{
		#region 操作符

		/// <summary>
		/// “非”。
		/// </summary>
		Not = 0x0001,

		/// <summary>
		/// “与”。
		/// </summary>
		And = 0x0002,

		/// <summary>
		/// “或”。
		/// </summary>
		Or = 0x0004,

		/// <summary>
		/// 开始新计算单元。
		/// </summary>
		With = 0x0008,

		/// <summary>
		/// 结束计算单元。
		/// </summary>
		EndWith = 0x0010,

		#endregion

		#region FilterFactory

		/// <summary>
		/// Filter。
		/// </summary>
		Filter = 0x0020,

		/// <summary>
		/// 外部引用。
		/// </summary>
		ForeignRef = 0x0040,

		/// <summary>
		/// 属性。
		/// </summary>
		Property = 0x0080,

		/// <summary>
		/// 属性路径。
		/// </summary>
		Locator = 0x0100,

		/// <summary>
		/// Is。
		/// </summary>
		Is = 0x0200,

		/// <summary>
		/// 过滤条件。
		/// </summary>
		FilterInfo = 0x0400,

		#endregion

		#region 排序

		/// <summary>
		/// 升序。
		/// </summary>
		Ascending = 0x0800,

		/// <summary>
		/// 降序。
		/// </summary>
		Descending = 0x1000,

		/// <summary>
		/// 下一个排序元素。
		/// </summary>
		Then = 0x2000,

		#endregion

		/// <summary>
		/// 表示不存在词法元素。
		/// </summary>
		Nothing = 0x20000000,

		/// <summary>
		/// 特殊词法记号，表示过滤器表达式为空白，与 Nothing 相同。
		/// </summary>
		Start = Nothing,

		/// <summary>
		/// 特殊词法记号，表示过滤器表达式处于结尾状态。
		/// </summary>
		Finish = 0x40000000
	}
}