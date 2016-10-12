#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ColumnLocator.cs
// �ļ���������������һ���������У�������λ�С�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20100815
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����һ���������У�������λ�С�
	/// </summary>
	[Serializable]
	public class ColumnLocator
	{
		#region ˽���ֶ�

		private readonly String m_scope;
		private readonly String m_propertyName;

		private String[] m_propertyPath;

		#endregion

		/// <summary>
		/// ���캯��������ֵ�������ơ�
		/// </summary>
		/// <param name="propertyName">ֵ�������ơ�</param>
		public ColumnLocator(String propertyName)
			: this(null, propertyName)
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ƺ�ֵ�������ơ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		public ColumnLocator(String entityPropertyName, String propertyName)
		{
			m_scope = entityPropertyName;
			m_propertyName = propertyName;
		}

		/// <summary>
		/// ���캯������������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		public ColumnLocator(IList<String> propertyPath)
		{
			#region ǰ������

			Debug.Assert(((propertyPath != null) && (propertyPath.Count != 0)), "����Ӧ�ṩһ�����ԡ�");

			#endregion

			if (propertyPath.Count == 1)
			{
				m_propertyName = propertyPath[0];
			}
			else
			{
				m_scope = propertyPath[0];
				m_propertyName = propertyPath[1];
			}

			#region ��������·��

			m_propertyPath = new String[propertyPath.Count];

			for (Int32 i = 0; i < m_propertyPath.Length; i++)
			{
				m_propertyPath[i] = propertyPath[i];
			}

			#endregion
		}

		#region ��������

		/// <summary>
		/// ��ȡ�Ե������ӵ�����·����
		/// </summary>
		public String FullName
		{
			get { return String.Join(".", PropertyPath); }
		}

		/// <summary>
		/// ��ȡʵ���������ơ�
		/// </summary>
		public String Scope
		{
			get { return m_scope; }
		}

		/// <summary>
		/// ��ȡֵ�������ơ�
		/// </summary>
		public String PropertyName
		{
			get { return m_propertyName; }
		}

		/// <summary>
		/// ��ȡ����·����
		/// </summary>
		public String[] PropertyPath
		{
			get
			{
				if (m_propertyPath == null)
				{
					if ((m_scope != null) && (m_propertyName != null))
					{
						m_propertyPath = new String[] { m_scope, m_propertyName };
					}
					else if (m_scope != null)
					{
						m_propertyPath = new String[] { m_scope };
					}
					else
					{
						m_propertyPath = new String[] { m_propertyName };
					}
				}

				return m_propertyPath;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>IPropertyChain ʵ����</returns>
		public IPropertyChain Create(Type entityType)
		{
			#region ǰ������

			Debug.Assert((entityType != null), "ʵ�����Ͳ��� entityType ����Ϊ�ա�");

			#endregion

			return new PropertyChain(entityType, PropertyPath);
		}

		#endregion
	}
}