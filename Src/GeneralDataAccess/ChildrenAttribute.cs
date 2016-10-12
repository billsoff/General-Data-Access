#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ChildrenAttribute.cs
// �ļ�������������ע����Ϊ��ʵ�弯�ϣ���������ֻ��Ϊ���顣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110407
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
	/// ��ע����Ϊ��ʵ�弯�ϣ���������ֻ��Ϊ���顣
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ChildrenAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly String m_propertyName;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯������������ʵ����������ơ�
		/// </summary>
		/// <param name="propertyName">��ʵ�������õ���ǰʵ����������ơ�</param>
		public ChildrenAttribute(String propertyName)
		{
			m_propertyName = propertyName;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ʵ���������ơ�
		/// </summary>
		public String PropertyName
		{
			get { return m_propertyName; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="parentEntity">��ʵ�塣</param>
		/// <returns>�����õĹ�������</returns>
		public Filter CreateFilter(Object parentEntity)
		{
			Filter f = Filter.Create(PropertyName, Is.EqualTo(parentEntity));

			return f;
		}

		#endregion
	}
}