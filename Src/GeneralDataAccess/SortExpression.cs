#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SortExpression.cs
// �ļ�������������ʾһ��������ʽ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008151010
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
	/// ��ʾһ��������ʽ��
	/// </summary>
	[Serializable]
	public class SortExpression
	{
		#region ˽���ֶ�

		private readonly ColumnLocator m_columnLocator;
		private readonly SortMethod m_sortMethod;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ֵ�������ơ�
		/// </summary>
		/// <param name="propertyName">ֵ�������ơ�</param>
		public SortExpression(String propertyName)
			: this(null, propertyName, SortMethod.Ascending)
		{
		}

		/// <summary>
		/// ���캯��������ֵ�������ƺ����򷽷���
		/// </summary>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="sortMethod">���򷽷���</param>
		public SortExpression(String propertyName, SortMethod sortMethod)
			: this(null, propertyName, sortMethod)
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ƺ�ֵ�������ơ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		public SortExpression(String entityPropertyName, String propertyName)
			: this(entityPropertyName, propertyName, SortMethod.Ascending)
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ơ�ֵ�������ƺ����򷽷���
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="sortMethod">���򷽷���</param>
		public SortExpression(String entityPropertyName, String propertyName, SortMethod sortMethod)
			: this(new ColumnLocator(entityPropertyName, propertyName), sortMethod)
		{
		}

		/// <summary>
		/// ���캯������������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		public SortExpression(IList<String> propertyPath)
			: this(propertyPath, SortMethod.Ascending)
		{
		}

		/// <summary>
		/// ���캯������������·�������򷽷���
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="sortMethod">���򷽷���</param>
		public SortExpression(IList<String> propertyPath, SortMethod sortMethod)
			: this(new ColumnLocator(propertyPath), sortMethod)
		{
		}

		/// <summary>
		/// ���캯���������ж�λ����
		/// </summary>
		/// <param name="columnLocator">�ж�λ����</param>
		internal protected SortExpression(ColumnLocator columnLocator)
			: this(columnLocator, SortMethod.Ascending)
		{
		}

		/// <summary>
		/// ���캯���������ж�λ�������򷽷���
		/// </summary>
		/// <param name="columnLocator">�ж�λ����</param>
		/// <param name="sortMethod">���򷽷���</param>
		internal protected SortExpression(ColumnLocator columnLocator, SortMethod sortMethod)
		{
			m_columnLocator = columnLocator;
			m_sortMethod = sortMethod;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�ж�λ����
		/// </summary>
		public ColumnLocator ColumnLocator
		{
			get { return m_columnLocator; }
		}

		/// <summary>
		/// ��ȡ����ʽ��
		/// </summary>
		public SortMethod SortMethod
		{
			get { return m_sortMethod; }
		}

		#endregion
	}
}