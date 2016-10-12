#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeForeignReferenceAttribute.cs
// �ļ���������������ָʾ����ʵ���еı��ʽʵ��ܹ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110707
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
	/// ����ָʾ����ʵ���еı��ʽʵ��ܹ���
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class CompositeForeignReferenceAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly PropertyJoinMode m_propertyJoinMode;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯����������������ģʽ��
		/// </summary>
		/// <param name="mode">����ģʽ��</param>
		public CompositeForeignReferenceAttribute(PropertyJoinMode mode)
		{
			m_propertyJoinMode = mode;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ��������ģʽ��
		/// </summary>
		public PropertyJoinMode PropertyJoinMode
		{
			get { return m_propertyJoinMode; }
		}

		#endregion
	}
}