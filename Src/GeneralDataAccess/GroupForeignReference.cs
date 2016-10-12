#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupForeignReference.cs
// �ļ��������������ڰ�װ������ʵ�嶨���ⲿ�������ԣ����ڷ��飩������ƥ���ʵ��ܹ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110630
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
	/// ���ڰ�װ������ʵ�嶨���ⲿ�������ԣ����ڷ��飩������ƥ���ʵ��ܹ���
	/// </summary>
	internal struct GroupForeignReference
	{
		#region �����ֶ�

		private readonly GroupPropertyDefinition m_property;
		private readonly EntitySchema m_schema;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������÷����������ⲿ�������Ժ�������ƥ���ʵ��ܹ���
		/// </summary>
		/// <param name="property"></param>
		/// <param name="schema"></param>
		public GroupForeignReference(GroupPropertyDefinition property, EntitySchema schema)
		{
			m_property = property;
			m_schema = schema;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ������ʵ�嶨����ⲿ�������ԡ�
		/// </summary>
		public GroupPropertyDefinition Property
		{
			get { return m_property; }
		}

		/// <summary>
		/// ��ȡƥ���ʵ��ܹ���
		/// </summary>
		public EntitySchema Schema
		{
			get { return m_schema; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// �ϳ��ⲿ����ʵ�塣
		/// </summary>
		/// <param name="dbValues">��¼ֵ��</param>
		/// <returns>�ϳɺõ��ⲿ����ʵ�塣</returns>
		public Object Compose(Object[] dbValues)
		{
			return m_schema.Compose(dbValues);
		}

		#endregion
	}
}