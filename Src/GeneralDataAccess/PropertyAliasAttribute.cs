#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PropertyAliasAttribute.cs
// �ļ��������������ڱ�����Ա�����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110329
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
	/// ���ڱ�����Ա�����
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public abstract class PropertyAliasAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly String m_alias;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�����������Ա�����
		/// </summary>
		/// <param name="alias">Ҫ���õı�����</param>
		protected PropertyAliasAttribute(String alias)
		{
			m_alias = alias;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ������
		/// </summary>
		public String Alias
		{
			get { return m_alias; }
		}

		#endregion
	}
}