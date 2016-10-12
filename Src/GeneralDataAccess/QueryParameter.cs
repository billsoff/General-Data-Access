#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����QueryParameter.cs
// �ļ�������������ʾһ����ѯ������
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008151414
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
	/// ��ʾһ����ѯ������
	/// </summary>
	[Serializable]
	public class QueryParameter : ICloneable
	{
		#region ˽���ֶ�

		// Clone ʱ���Ը���
		private String m_name;

		private readonly DbType m_dbType;
		private Object m_value;

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
		/// ���캯�������ò�ѯ���������ơ����ݿ����ͺ�ֵ������Ϊ Input��
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbType">�������ݿ����͡�</param>
		/// <param name="value">����ֵ��</param>
		public QueryParameter(String name, DbType dbType, Object value)
			: this(name, dbType, value, ParameterDirection.Input)
		{
		}

		/// <summary>
		/// ���캯�������ò�ѯ���������ơ����ݿ����͡�ֵ�ͷ���
		/// </summary>
		/// <param name="name">��������</param>
		/// <param name="dbType">�������ݿ����͡�</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		public QueryParameter(String name, DbType dbType, Object value, ParameterDirection direction)
		{
			m_name = name;
			m_dbType = dbType;
			m_value = value;

			m_direction = direction;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ��ѯ���������ơ�
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// ��ȡ��ѯ���������ݿ����͡�
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
		/// ��ȡ��ѯ������ֵ��
		/// </summary>
		public Object Value
		{
			get { return m_value; }
			protected internal set { m_value = value; }
		}

		/// <summary>
		/// ��ȡ�����ķ���Ĭ��Ϊ Input��
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

		#region ICloneable ��Ա

		/// <summary>
		/// ���Ʋ��������ƣ����������Ϊ null ����ַ������򱣳�ԭ�������ơ�
		/// </summary>
		/// <param name="newName">�����ơ�</param>
		/// <returns>��ǰ�����ǳ������</returns>
		public QueryParameter Clone(String newName)
		{
			QueryParameter clone = (QueryParameter)MemberwiseClone();

			if (!String.IsNullOrEmpty(newName))
			{
				clone.m_name = newName;
			}

			return clone;
		}

		/// <summary>
		/// ���ص�ǰʵ����ǳ������
		/// </summary>
		/// <returns>��ǰʵ����ǳ������</returns>
		Object ICloneable.Clone()
		{
			return MemberwiseClone();
		}

		#endregion
	}
}