#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterOperator.cs
// 文件功能描述：过滤器操作符，用于连接过滤器。
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
	/// 过滤器操作符，用于连接过滤器。
	/// </summary>
	[Serializable]
	internal abstract class FilterOperator
	{
		#region 私有字段

		private readonly PrecedenceAttribute m_precedences;
		private readonly IFilterFactoryOperands m_filterFactories;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		protected FilterOperator()
		{
			m_precedences = GetPrecedences();
		}

		/// <summary>
		/// 构造函数。
		/// </summary>
		/// <param name="filterFactories">过滤器工厂。</param>
		protected FilterOperator(IFilterFactoryOperands filterFactories)
			: this()
		{
			m_filterFactories = filterFactories;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取左优先级。
		/// </summary>
		public Int32 LeftPrecedence
		{
			get { return m_precedences.LeftPrecedence; }
		}

		/// <summary>
		/// 获取右优先级。
		/// </summary>
		public Int32 RightPrecedence
		{
			get { return m_precedences.RightPrecedence; }
		}

		#endregion

		#region 保护的属性

		/// <summary>
		/// 获取过滤器工厂栈。
		/// </summary>
		protected IFilterFactoryOperands FilterFactories
		{
			get { return m_filterFactories; }
		}

		#endregion

		#region 抽象成员

		#region 公共方法

		/// <summary>
		/// 进行计算。
		/// </summary>
		/// <returns>生成的过滤器工厂。</returns>
		public abstract FilterFactory Calculate();

		#endregion

		#endregion

		#region 辅助方法

		/// <summary>
		/// 获取操作符的优先级设置。
		/// </summary>
		/// <returns>操作符的优先级设置。</returns>
		private PrecedenceAttribute GetPrecedences()
		{
			PrecedenceAttribute precedences = (PrecedenceAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PrecedenceAttribute));

			if (precedences == null)
			{
				precedences = PrecedenceAttribute.CreateDefault();
			}

			return precedences;
		}

		#endregion
	}
}