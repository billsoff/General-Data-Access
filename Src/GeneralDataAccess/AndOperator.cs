#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AndOperator.cs
// 文件功能描述：“与”操作符。
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
	/// “与”操作符。
	/// </summary>
	[Serializable, Precedence(FilterOperatorPrecedences.ABOVE_NORMAL, FilterOperatorPrecedences.ABOVE_NORMAL)]
	internal sealed class AndOperator : BinaryOperator
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置过滤器工厂栈。
		/// </summary>
		/// <param name="filterFactories">过滤器工厂栈。</param>
		public AndOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 进行“与”操作。
		/// </summary>
		/// <param name="left">左参。</param>
		/// <param name="right">右参。</param>
		/// <returns>计算结果，过滤器实例。</returns>
		protected override FilterFactory Process(FilterFactory left, FilterFactory right)
		{
			if (left != null)
			{
				return left.And(right);
			}
			else
			{
				return right;
			}
		}

		#endregion
	}
}