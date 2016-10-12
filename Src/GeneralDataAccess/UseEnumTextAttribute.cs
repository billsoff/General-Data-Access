#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：UseEnumTextAttribute.cs
// 文件功能描述：标记于属性类型为枚举类的属性上，将该枚举类型转换为文本，如果将枚举类型保存为其底层的整数类型，则不使用此标记。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110802
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
	/// 标记于属性类型为枚举类的属性上，将该枚举类型转换为文本，如果将枚举类型保存为其底层的整数类型，则不使用此标记。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class UseEnumTextAttribute : Attribute
	{
		#region 私有字段

		private EnumTextOption m_text;
		private EnumParseErrorFollowup m_onParseError;
		private Boolean m_parseCaseSensitive;
		private Object m_defaultValue;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public UseEnumTextAttribute()
		{
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取或设置文本选项，默认值是 Name。
		/// </summary>
		public EnumTextOption Text
		{
			get { return m_text; }
			set { m_text = value; }
		}

		/// <summary>
		/// 获取或设置一个值，该值指示解析时是否区分大小写，默认为 false（不区分大小写）。
		/// </summary>
		public Boolean ParseCaseSensitive
		{
			get { return m_parseCaseSensitive; }
			set { m_parseCaseSensitive = value; }
		}

		/// <summary>
		/// 获取或设置当解析失败后的行动策略，默认是 Broadcast。
		/// </summary>
		public EnumParseErrorFollowup OnParseError
		{
			get { return m_onParseError; }
			set { m_onParseError = value; }
		}

		/// <summary>
		/// 获取或设置默认值，当解析失败时如果 OnParseError 的值为 SetDefault，则设置为该值，默认为 0。
		/// </summary>
		public Object DefaultValue
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}

		#endregion
	}
}