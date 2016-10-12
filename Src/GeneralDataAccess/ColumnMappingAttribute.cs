#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ColumnMappingAttribute.cs
// �ļ��������������ʵ�����Ե���ӳ���ϵ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008132332
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
	/// ���ʵ�����Ե���ӳ���ϵ�����ڸ����У�Ӧ��Ƕ�Ρ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class ColumnMappingAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly String m_childColumnName;
		private readonly String m_parentColumnName;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯����
		/// </summary>
		/// <param name="childColumnName">��������</param>
		/// <param name="parentColumnName">��������</param>
		public ColumnMappingAttribute(String childColumnName, String parentColumnName)
		{
			m_childColumnName = childColumnName;
			m_parentColumnName = parentColumnName;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public String ChildColumnName
		{
			get { return m_childColumnName; }
		}

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public String ParentColumnName
		{
			get { return m_parentColumnName; }
		}

		#endregion
	}
}