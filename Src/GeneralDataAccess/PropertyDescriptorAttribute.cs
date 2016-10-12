#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PropertyDescriptorAttribute.cs
// �ļ��������������ڱ��������������ָʾ������������Ŀ�����͡�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110713
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
	/// ���ڱ��������������ָʾ������������Ŀ�����͡�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class PropertyDescriptorAttribute : Attribute
	{
		#region

		private readonly Type m_type;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������Ŀ�����͡�
		/// </summary>
		/// <param name="type">Ŀ�����͡�</param>
		public PropertyDescriptorAttribute(Type type)
		{
			m_type = type;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡĿ�����͡�
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion
	}
}