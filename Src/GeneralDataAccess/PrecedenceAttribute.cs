#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PrecedenceAttribute.cs
// 文件功能描述：指示过滤器操作符的优先级。
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
	/// 指示过滤器操作符的优先级。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	internal sealed class PrecedenceAttribute : Attribute
	{
		#region 私有字段

		private readonly Int32 m_leftPrecedence;
		private readonly Int32 m_rightPrecedence;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，将左、右优先级设为 FilterOperatorPrecedences.BELOW_NORMAL。
		/// </summary>
		internal PrecedenceAttribute()
			: this(FilterOperatorPrecedences.BELOW_NORMAL, FilterOperatorPrecedences.BELOW_NORMAL)
		{
		}

		/// <summary>
		/// 构造函数，设置左、右优先级。
		/// </summary>
		/// <param name="leftPrecedence">左优先级。</param>
		/// <param name="rightPrecedence">右优先级。</param>
		public PrecedenceAttribute(Int32 leftPrecedence, Int32 rightPrecedence)
		{
			m_leftPrecedence = leftPrecedence;
			m_rightPrecedence = rightPrecedence;
		}

		#endregion

		#region 静态成员

		/// <summary>
		/// 创建默认实例。
		/// </summary>
		/// <returns>创建好的默认实例。</returns>
		public static PrecedenceAttribute CreateDefault()
		{
			return new PrecedenceAttribute();
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取左优先级。
		/// </summary>
		public Int32 LeftPrecedence
		{
			get { return m_leftPrecedence; }
		}

		/// <summary>
		/// 获取右优先级。
		/// </summary>
		public Int32 RightPrecedence
		{
			get { return m_rightPrecedence; }
		}

		#endregion
	}
}