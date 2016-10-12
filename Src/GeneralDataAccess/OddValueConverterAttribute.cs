#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����OddValueConverterAttribute.cs
// �ļ�����������ָʾ��������ֵ����ת���Ĺ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110706
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
	/// ָʾ��������ֵ����ת���Ĺ���
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public abstract class OddValueConverterAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly Object m_oddValue;
		private readonly OddValueDbMode m_oddValueDbMode;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		protected OddValueConverterAttribute()
		{
		}

		/// <summary>
		/// ���캯������������ֵ��
		/// </summary>
		/// <param name="oddValue">����ֵ��</param>
		protected OddValueConverterAttribute(Object oddValue)
		{
			m_oddValue = oddValue;
		}

		/// <summary>
		/// ���캯������������ֵת��ģʽ��
		/// </summary>
		/// <param name="mode">����ֵת��ģʽ��</param>
		protected OddValueConverterAttribute(OddValueDbMode mode)
		{
			m_oddValueDbMode = mode;
		}

		/// <summary>
		/// ���캯������������ֵ��ת��ģʽ��
		/// </summary>
		/// <param name="oddValue">����ֵ��</param>
		/// <param name="mode">ת��ģʽ��</param>
		protected OddValueConverterAttribute(Object oddValue, OddValueDbMode mode)
		{
			m_oddValue = oddValue;
			m_oddValueDbMode = mode;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ֵ��
		/// </summary>
		public virtual Object OddValue
		{
			get { return m_oddValue; }
		}

		/// <summary>
		/// ��ȡ����ֵ��ת����ʽ��
		/// </summary>
		public OddValueDbMode OddValueDbMode
		{
			get { return m_oddValueDbMode; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ָʾ������ֵת�������ݿ�ֵʱ��δ�������ֵ��Ĭ�ϵ�ʵ����ת����ֵΪָ��������ֵʱ��ָ���ķ�ʽת����
		/// </summary>
		/// <param name="value">����ֵ��</param>
		/// <returns>ת��ģʽ��</returns>
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
		/// ��ȡ���ݿ�ֵ��
		/// </summary>
		/// <param name="value">����ֵ��</param>
		/// <returns>���ݿ�ֵ��</returns>
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
		/// �жϸ���������ֵ�Ƿ�Ϊ����ֵ��
		/// </summary>
		/// <param name="value">����ֵ��</param>
		/// <returns>�������ֵΪ����ֵ���򷵻� true�����򷵻� false��</returns>
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