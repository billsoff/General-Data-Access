#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ������ṩ���ݿ�ֵ��ת��������
// �ļ�����������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110415
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// �ṩ���ݿ�ֵ��ת��������
	/// </summary>
	public static class DbConverter
	{
		/// <summary>
		/// �����ݿ�ֵת��Ϊָ�������͡�
		/// </summary>
		/// <param name="dbValue">���ݿ�ֵ��</param>
		/// <param name="destinationType">Ŀ�����͡�</param>
		/// <returns>ת���õ����͡�</returns>
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

			// ö�����Ͳ�����ת������������������
			if (destinationType.IsEnum)
			{
				return dbValue;
			}

			Object destinationValue = Convert.ChangeType(dbValue, destinationType);

			return destinationValue;
		}

		/// <summary>
		/// ת�����ݿ�ֵ��
		/// </summary>
		/// <param name="dbValue">���ݿ�ֵ��</param>
		/// <param name="col">��Ӧ���С�</param>
		/// <returns>��������ͼ��ݵ�ֵ��</returns>
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
		/// �ж����ݿ�ֵ�Ƿ�Ϊ DBEmpty������ָʾ�ڲ�������ʱ����ӵ�и�ֵ���ֶΡ�
		/// </summary>
		/// <param name="dbValue">���ݿ�ֵ��</param>
		/// <returns>���ֵΪ DBEmpty���򷵻� true�����򷵻� false��</returns>
		public static Boolean IsDBEmpty(Object dbValue)
		{
			return (dbValue == DBEmpty.Value);
		}

		/// <summary>
		/// �ж�����ֵ�Ƿ�Ϊδ����ֵ�� DateTime ���ͣ���ֵΪ DateTime.MinValue����
		/// </summary>
		/// <param name="value">Ҫ�жϵ�ֵ��</param>
		/// <returns>���ֵΪδ���õ� DateTime ���ͣ��򷵻� true�����򷵻� false��</returns>
		public static Boolean IsDateTimeAndUninitialized(Object value)
		{
			return (value is DateTime) && ((DateTime)value == DateTime.MinValue);
		}

		private static readonly DateTimeZeroConverterAttribute DefaultDateTimeZeroConverter = new DateTimeZeroConverterAttribute(OddValueDbMode.Ignore);

		/// <summary>
		/// ��ȡ�ж��������ֵת������
		/// </summary>
		/// <param name="columnDef">�ж��塣</param>
		/// <returns>�ж��������ֵת������</returns>
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
		/// ת��ö�����͵����ԡ�
		/// </summary>
		/// <param name="val">����ֵ��</param>
		/// <param name="property">���ԡ�</param>
		/// <returns>ö�����͵�ֵ��</returns>
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