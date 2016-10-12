#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����DateTimeZeroConverterAttribute.cs
// �ļ�����������ָʾת��������ʱ�����͵ķ�ʽ��
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
	/// ָʾת��������ʱ�����͵ķ�ʽ��
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class DateTimeZeroConverterAttribute : OddValueConverterAttribute
	{
		#region ���캯��

		/// <summary>
		/// ָʾת����ʽ��
		/// </summary>
		/// <param name="mode"></param>
		public DateTimeZeroConverterAttribute(OddValueDbMode mode)
			: base(mode)
		{
		}

		#endregion

		private readonly Object m_oddValue = DateTime.MinValue;

		/// <summary>
		/// ��ȡ����ֵ��
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