#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PrefixOperator.cs
// 文件功能描述：一元过滤器操作符。
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
	/// 一元过滤器操作符。
	/// </summary>
	[Serializable]
	internal abstract class PrefixOperator : FilterOperator
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置过滤器工厂栈。
		/// </summary>
		/// <param name="filterFactories">过滤器工厂栈。</param>
		public PrefixOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 进行运算。
		/// </summary>
		/// <returns>运算结果，过滤器工厂实例。</returns>
		public sealed override FilterFactory Calculate()
		{
			FilterFactory operand = FilterFactories.Pop();

			FilterFactory result = Process(operand);

			return result;
		}

		#endregion

		#region 抽象成员

		#region 保护的方法

		/// <summary>
		/// 进行一元操作。
		/// </summary>
		/// <param name="operand">参数。</param>
		/// <returns>操作结果，过滤器工厂实例。</returns>
		protected abstract FilterFactory Process(FilterFactory operand);

		#endregion

		#endregion
	}
}