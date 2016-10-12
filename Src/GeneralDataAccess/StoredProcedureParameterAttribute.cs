#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����StoredProcedureParameterAttribute.cs
// �ļ������������������ DbStoredProcedureParameters ������Ϊ�洢���̵Ĳ�����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110212
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
	/// ������� DbStoredProcedureParameters ������Ϊ�洢���̵Ĳ�����
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class StoredProcedureParameterAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly String m_name;
		private readonly DbType m_dbType;
		private readonly ParameterDirection m_direction;

		private Int32 m_size;
		private Boolean m_nullable;
		private Byte m_precision;
		private Byte m_scale;

		private String m_dbTypePropertyName;
		private Int32 m_dbTypePropertyValue;

		private Boolean m_suppressRetrieveValue;

		#endregion

		#region ���캯��

		/// <summary>
		/// ��ʼ���������ơ����ݿ����ͣ�����Ϊ Input��
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbType">�������ݿ����͡�</param>
		public StoredProcedureParameterAttribute(String name, DbType dbType)
			: this(name, dbType, ParameterDirection.Input)
		{
		}

		/// <summary>
		/// ��ʼ���������ơ����ݿ����ͺͷ���
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbType">�������ݿ����͡�</param>
		/// <param name="direction">��������</param>
		public StoredProcedureParameterAttribute(String name, DbType dbType, ParameterDirection direction)
		{
			m_name = name;
			m_dbType = dbType;
			m_direction = direction;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ���������ơ�
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// ��ȡ���������ݿ����͡�
		/// </summary>
		public DbType DbType
		{
			get { return m_dbType; }
		}

		/// <summary>
		/// ��ȡ�����ò����������͵��������ơ�
		/// </summary>
		public String DbTypePropertyName
		{
			get
			{
				return m_dbTypePropertyName;
			}

			set
			{
				if (value != null)
				{
					value = value.Trim();
				}

				if (!String.IsNullOrEmpty(value))
				{
					m_dbTypePropertyName = value;
				}
			}
		}

		/// <summary>
		/// ��ȡ�����ò����������͵�ֵ��
		/// </summary>
		public Int32 DbTypePropertyValue
		{
			get { return m_dbTypePropertyValue; }
			set { m_dbTypePropertyValue = value; }
		}

		/// <summary>
		/// ��ȡ�����ķ�����Ĭ��Ϊ Input��
		/// </summary>
		public ParameterDirection Direction
		{
			get { return m_direction; }
		}

		/// <summary>
		/// ��ȡ�����ò�����С��
		/// </summary>
		public Int32 Size
		{
			get { return m_size; }
			set { m_size = value; }
		}

		/// <summary>
		/// ��ȡ�����ò����Ƿ����Ϊ�ա�
		/// </summary>
		public Boolean Nullable
		{
			get { return m_nullable; }
			set { m_nullable = value; }
		}

		/// <summary>
		/// ��ȡ�����ò����ľ��ȣ���Ч���ָ�������
		/// </summary>
		public Byte Precision
		{
			get { return m_precision; }
			set { m_precision = value; }
		}

		/// <summary>
		/// ��ȡ�����ò�����С��λ����
		/// </summary>
		public Byte Scale
		{
			get { return m_scale; }
			set { m_scale = value; }
		}

		/// <summary>
		/// ��ȡ������һ��ֵ������������������������ִ����洢���̺������ֵΪ false����ȡ�ò���ֵ�����򣬲�ȥ��ȡ��Ĭ��ֵΪ false��
		/// </summary>
		public Boolean SuppressRetrieveValue
		{
			get { return m_suppressRetrieveValue; }
			set { m_suppressRetrieveValue = value; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ���� <see cref="QueryParameter"/> ʵ����
		/// </summary>
		/// <param name="value">����ֵ��</param>
		/// <returns>�����õĲ�����</returns>
		public QueryParameter CreateParameter(Object value)
		{
			QueryParameter parameter = new QueryParameter(m_name, m_dbType, value, m_direction);

			parameter.Size = m_size;
			parameter.Nullable = m_nullable;

			parameter.Precision = m_precision;
			parameter.Scale = m_scale;

			if (!String.IsNullOrEmpty(m_dbTypePropertyName))
			{
				parameter.DbTypePropertyName = m_dbTypePropertyName;
				parameter.DbTypePropertyValue = m_dbTypePropertyValue;
			}

			return parameter;
		}

		#endregion
	}
}