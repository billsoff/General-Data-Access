#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：BinaryOperator.cs
// 文件功能描述：二元过滤器操作符。
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
	/// 二元过滤器操作符。
	/// </summary>
	[Serializable]
	internal abstract class BinaryOperator : FilterOperator
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置过滤器工厂栈。
		/// </summary>
		/// <param name="filterFactories">过滤器工厂栈。</param>
		protected BinaryOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 进行运算。
		/// </summary>
		/// <returns>过滤器实例。</returns>
		public sealed override FilterFactory Calculate()
		{
			FilterFactory right = FilterFactories.Pop();
			FilterFactory left = FilterFactories.Pop();

			FilterFactory result = Process(left, right);

			return result;
		}

		#endregion

		#region 抽象成员

		#region 保护的方法

		/// <summary>
		/// 操作目标参数。
		/// </summary>
		/// <param name="left">左参。</param>
		/// <param name="right">右参。</param>
		/// <returns>计算结果，过滤器实例。</returns>
		protected abstract FilterFactory Process(FilterFactory left, FilterFactory right);

		#endregion

		#endregion
	}
}