#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ColumnAttribute.cs
// �ļ��������������ڱ��ʵ������������ӳ���ϵ��ֵ���ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008132251
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ڱ��ʵ������������ӳ���ϵ��ֵ���ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ColumnAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly String m_name;
		private readonly DbType m_dbType;

		private Boolean m_isPrimaryKey;

		#endregion

		/// <summary>
		/// ���캯�����������������ݿ����͡�
		/// </summary>
		/// <param name="name">������</param>
		/// <param name="dbType">���ݿ����ơ�</param>
		public ColumnAttribute(String name, DbType dbType)
		{
			m_name = name;
			m_dbType = dbType;
		}

		#region ��������

		/// <summary>
		/// ��ȡ�����ơ�
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// ��ȡ���ݿ����͡�
		/// </summary>
		public DbType DbType
		{
			get { return m_dbType; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǰ���Ƿ�Ϊ������Ĭ��Ϊ false��
		/// </summary>
		public Boolean IsPrimaryKey
		{
			get { return m_isPrimaryKey; }
			set { m_isPrimaryKey = value; }
		}

		#endregion
	}
}