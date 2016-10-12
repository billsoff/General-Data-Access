#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：WithOperator.cs
// 文件功能描述：用于提高后续操作符的优先级，相当于算术表达中的圆括号。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110325
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
	/// 用于提高后续操作符的优先级，相当于算术表达中的圆括号。
	/// </summary>
	[Serializable, Precedence(FilterOperatorPrecedences.HIGHEST, FilterOperatorPrecedences.BELOW_NORMAL)]
	internal sealed class WithOperator : PrefixOperator
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置过滤器工厂栈。
		/// </summary>
		/// <param name="filterFactories">过滤器工厂栈。</param>
		public WithOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// NOP，原样返回参数。
		/// </summary>
		/// <param name="operand">操作参数。</param>
		/// <returns>过滤器工厂。</returns>
		protected override FilterFactory Process(FilterFactory operand)
		{
			return operand;
		}

		#endregion
	}
}