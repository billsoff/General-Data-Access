#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����TableAttribute.cs
// �ļ��������������ڱ��ʵ�壬ʹ����һ�����ݿ����ӳ���ϵ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008132245
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
	/// ���ڱ��ʵ�壬ʹ����һ�����ݿ����ӳ���ϵ��
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class TableAttribute : Attribute
	{
		private readonly String m_name;
		private Boolean m_isStoredProcedure;

		#region ���캯��

		/// <summary>
		/// ���캯��������ӳ�������
		/// </summary>
		/// <param name="name">ӳ�������</param>
		public TableAttribute(String name)
		{
			m_name = name;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡӳ�������
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾ����Դ�Ƿ�Ϊ�洢���̡�
		/// </summary>
		public Boolean IsStoredProcedure
		{
			get { return m_isStoredProcedure; }
			set { m_isStoredProcedure = value; }
		}

		#endregion
	}
}