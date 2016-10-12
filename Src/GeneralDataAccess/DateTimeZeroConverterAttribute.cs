#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DateTimeZeroConverterAttribute.cs
// 文件功能描述：指示转换零日期时间类型的方式。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110706
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
	/// 指示转换零日期时间类型的方式。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class DateTimeZeroConverterAttribute : OddValueConverterAttribute
	{
		#region 构造函数

		/// <summary>
		/// 指示转换方式。
		/// </summary>
		/// <param name="mode"></param>
		public DateTimeZeroConverterAttribute(OddValueDbMode mode)
			: base(mode)
		{
		}

		#endregion

		private readonly Object m_oddValue = DateTime.MinValue;

		/// <summary>
		/// 获取奇异值。
		/// </summary>
		public override Object OddValue
		{
			get
			{
				return m_oddValue;
			}
		}
	}
}