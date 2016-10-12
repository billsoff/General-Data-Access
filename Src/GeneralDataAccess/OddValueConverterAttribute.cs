#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：OddValueConverterAttribute.cs
// 文件功能描述：指示对于奇异值进行转换的规则。
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
	/// 指示对于奇异值进行转换的规则。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public abstract class OddValueConverterAttribute : Attribute
	{
		#region 私有字段

		private readonly Object m_oddValue;
		private readonly OddValueDbMode m_oddValueDbMode;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		protected OddValueConverterAttribute()
		{
		}

		/// <summary>
		/// 构造函数，设置奇异值。
		/// </summary>
		/// <param name="oddValue">奇异值。</param>
		protected OddValueConverterAttribute(Object oddValue)
		{
			m_oddValue = oddValue;
		}

		/// <summary>
		/// 构造函数，设置奇异值转换模式。
		/// </summary>
		/// <param name="mode">奇异值转换模式。</param>
		protected OddValueConverterAttribute(OddValueDbMode mode)
		{
			m_oddValueDbMode = mode;
		}

		/// <summary>
		/// 构造函数，设置奇异值与转换模式。
		/// </summary>
		/// <param name="oddValue">奇异值。</param>
		/// <param name="mode">转换模式。</param>
		protected OddValueConverterAttribute(Object oddValue, OddValueDbMode mode)
		{
			m_oddValue = oddValue;
			m_oddValueDbMode = mode;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取奇异值。
		/// </summary>
		public virtual Object OddValue
		{
			get { return m_oddValue; }
		}

		/// <summary>
		/// 获取奇异值的转换方式。
		/// </summary>
		public OddValueDbMode OddValueDbMode
		{
			get { return m_oddValueDbMode; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 指示将属性值转换到数据库值时如何处理奇异值，默认的实现是转换当值为指定的奇异值时按指定的方式转换。
		/// </summary>
		/// <param name="value">属性值。</param>
		/// <returns>转换模式。</returns>
		public OddValueDbMode ConvertToDbValue(Object value)
		{
			if (m_oddValueDbMode == OddValueDbMode.NotChange)
			{
				return OddValueDbMode.NotChange;
			}
			else
			{
				return IsOddValue(value) ? m_oddValueDbMode : OddValueDbMode.NotChange;
			}
		}

		/// <summary>
		/// 获取数据库值。
		/// </summary>
		/// <param name="value">属性值。</param>
		/// <returns>数据库值。</returns>
		public Object GetDbValue(Object value)
		{
			OddValueDbMode mode = ConvertToDbValue(value);

			switch (mode)
			{
				case OddValueDbMode.DBNull:
					return DBNull.Value;

				case OddValueDbMode.Ignore:
					return DBEmpty.Value;

				case OddValueDbMode.NotChange:
				default:
					if (value != null)
					{
						return value;
					}
					else
					{
						return DBNull.Value;
					}
			}
		}

		/// <summary>
		/// 判断给定的属性值是否为奇异值。
		/// </summary>
		/// <param name="value">属性值。</param>
		/// <returns>如果属性值为奇异值，则返回 true；否则返回 false。</returns>
		public Boolean IsOddValue(Object value)
		{
			if (Object.ReferenceEquals(OddValue, null))
			{
				return Object.ReferenceEquals(null, value);
			}
			else
			{
				return OddValue.Equals(value);
			}
		}

		#endregion
	}
}