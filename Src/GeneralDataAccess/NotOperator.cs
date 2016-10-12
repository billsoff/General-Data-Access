#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NotOperator.cs
// 文件功能描述：“非”操作符。
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
	/// 非”操作符。
	/// </summary>
	[Serializable, Precedence(FilterOperatorPrecedences.HIGHEST, FilterOperatorPrecedences.HIGHEST)]
	internal sealed class NotOperator : PrefixOperator
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置过滤器工厂栈。
		/// </summary>
		/// <param name="filterFactories">过滤器工厂栈。</param>
		public NotOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 进行“非”操作。
		/// </summary>
		/// <param name="operand">操作数。</param>
		/// <returns>操作结果，过滤器工厂实例。</returns>
		protected override FilterFactory Process(FilterFactory operand)
		{
			if (operand != null)
			{
				return operand.Not();
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}