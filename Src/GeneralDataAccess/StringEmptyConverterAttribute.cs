#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����StringEmptyConverterAttribute.cs
// �ļ�����������ָʾת�����ַ����ķ�ʽ��
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
	/// ָʾת�����ַ����ķ�ʽ��
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class StringEmptyConverterAttribute : OddValueConverterAttribute
	{
		#region ���캯��

		/// <summary>
		/// ���ÿ��ַ�����ת����ʽ��
		/// </summary>
		public StringEmptyConverterAttribute(OddValueDbMode mode)
			: base("", mode)
		{
		}

		#endregion
	}
}