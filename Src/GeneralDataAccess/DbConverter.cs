#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：提供数据库值的转换方法。
// 文件功能描述：
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110415
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
	/// 提供数据库值的转换方法。
	/// </summary>
	public static class DbConverter
	{
		/// <summary>
		/// 将数据库值转换为指定的类型。
		/// </summary>
		/// <param name="dbValue">数据库值。</param>
		/// <param name="destinationType">目标类型。</param>
		/// <returns>转换好的类型。</returns>
		public static Object ConvertFrom(Object dbValue, Type destinationType)
		{
			if (Convert.IsDBNull(dbValue) || (dbValue == null))
			{
				return null;
			}

			if (destinationType.IsGenericType && (destinationType.GetGenericTypeDefinition() == typeof(Nullable<>)))
			{
				destinationType = Nullable.GetUnderlyingType(destinationType);
			}

			if (dbValue.GetType() == destinationType)
			{
				return dbValue;
			}

			// 枚举类型不进行转换，由其它方法处理
			if (destinationType.IsEnum)
			{
				return dbValue;
			}

			Object destinationValue = Convert.ChangeType(dbValue, destinationType);

			return destinationValue;
		}

		/// <summary>
		/// 转换数据库值。
		/// </summary>
		/// <param name="dbValue">数据库值。</param>
		/// <param name="col">对应的列。</param>
		/// <returns>与该列类型兼容的值。</returns>
		public static Object ConvertFrom(Object dbValue, Column col)
		{
			Object val = ConvertFrom(dbValue, col.Type);

			if (col.Type.IsEnum)
			{
				if ((col.Property != null) && (col.Property.Definition != null))
				{
					val = ConvertEnumProperty(val, col.Property.Definition);
				}
				else
				{
					val = Enum.ToObject(col.Type, Convert.ChangeType(val, Enum.GetUnderlyingType(col.Type)));
				}
			}

			return val;
		}

		/// <summary>
		/// 判断数据库值是否为 DBEmpty，用于指示在插入或更新时忽略拥有该值的字段。
		/// </summary>
		/// <param name="dbValue">数据库值。</param>
		/// <returns>如果值为 DBEmpty，则返回 true；否则返回 false。</returns>
		public static Boolean IsDBEmpty(Object dbValue)
		{
			return (dbValue == DBEmpty.Value);
		}

		/// <summary>
		/// 判断属性值是否为未设置值的 DateTime 类型（即值为 DateTime.MinValue）。
		/// </summary>
		/// <param name="value">要判断的值。</param>
		/// <returns>如果值为未设置的 DateTime 类型，则返回 true；否则返回 false。</returns>
		public static Boolean IsDateTimeAndUninitialized(Object value)
		{
			return (value is DateTime) && ((DateTime)value == DateTime.MinValue);
		}

		private static readonly DateTimeZeroConverterAttribute DefaultDateTimeZeroConverter = new DateTimeZeroConverterAttribute(OddValueDbMode.Ignore);

		/// <summary>
		/// 获取列定义的奇异值转换器。
		/// </summary>
		/// <param name="columnDef">列定义。</param>
		/// <returns>列定义的奇异值转换器。</returns>
		internal static OddValueConverterAttribute GetOddValueConverter(ColumnDefinition columnDef)
		{
			if (columnDef.OddValueConverter != null)
			{
				return columnDef.Property.OddValueConverter;
			}
			else if (columnDef.Type == typeof(DateTime))
			{
				return DefaultDateTimeZeroConverter;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 转换枚举类型的属性。
		/// </summary>
		/// <param name="val">属性值。</param>
		/// <param name="property">属性。</param>
		/// <returns>枚举类型的值。</returns>
		internal static Object ConvertEnumProperty(Object val, EntityPropertyDefinition property)
		{
			if (!property.Type.IsEnum)
			{
				return val;
			}

			if (val == null)
			{
				val = Enum.ToObject(property.Type, 0);

				return val;
			}

			if (property.UseEnumText == null)
			{
				val = Enum.ToObject(property.Type, Convert.ChangeType(val, Enum.GetUnderlyingType(property.Type)));

				return val;
			}

			try
			{
				val = Enum.Parse(property.Type, val.ToString(), !property.UseEnumText.ParseCaseSensitive);
			}
			catch (ArgumentException)
			{
				switch (property.UseEnumText.OnParseError)
				{
					case EnumParseErrorFollowup.SetDefault:
						val = property.UseEnumText.DefaultValue;
						break;

					case EnumParseErrorFollowup.Broadcast:
					default:
						throw;
				}
			}

			if (val == null)
			{
				val = Enum.ToObject(property.Type, 0);
			}

			return val;
		}
	}
}