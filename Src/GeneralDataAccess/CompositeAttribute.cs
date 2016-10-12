#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeAttribute.cs
// �ļ��������������ڱ�Ǹ���ʵ�塣
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
	/// ���ڱ�Ǹ���ʵ�塣
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class CompositeAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly String m_propertyName;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ�����ͺ��ڸ���ʵ���е��������ơ�
		/// </summary>
		/// <param name="propertyName">�ڸ���ʵ���е��������ơ�</param>
		public CompositeAttribute(String propertyName)
		{
			m_propertyName = propertyName;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ʵ���и�ʵ����������ơ�
		/// </summary>
		public String PropertyName
		{
			get { return m_propertyName; }
		}

		#endregion
	}
}