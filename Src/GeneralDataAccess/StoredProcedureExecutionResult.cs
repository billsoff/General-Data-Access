#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����StoredProcedureExecutionResult.cs
// �ļ��������������ڰ�װ�洢���̵�ִ�н����
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ڰ�װ�洢���̵�ִ�н����
	/// </summary>
	[Serializable]
	public class StoredProcedureExecutionResult<TEntity> where TEntity : class, new()
	{
		#region ˽���ֶ�

		private readonly Int32 m_rowsAffected;
		private readonly DbStoredProcedureParameters m_storedProcedureParameters;
		private readonly TEntity[] m_entities;

		#endregion

		#region ���캯��

		/// <summary>
		/// ����ִ�д洢���̺�������ϡ�Ӱ������������ص�ʵ�弯��Ϊ null��
		/// </summary>
		/// <param name="storedProcedureParameters">�������ϡ�</param>
		/// <param name="rowsAffected">Ӱ���������</param>
		public StoredProcedureExecutionResult(DbStoredProcedureParameters storedProcedureParameters, Int32 rowsAffected)
			: this(storedProcedureParameters, rowsAffected, null)
		{
		}

		/// <summary>
		/// ����ִ�д洢���̺�������ϡ����ص�ʵ�弯�ϣ�Ӱ�������δ֪����Ϊ -1����
		/// </summary>
		/// <param name="storedProcedureParameters">�������ϡ�</param>
		/// <param name="entities">���ص�ʵ�弯�ϡ�</param>
		public StoredProcedureExecutionResult(DbStoredProcedureParameters storedProcedureParameters, TEntity[] entities)
			: this(storedProcedureParameters, -1, entities)
		{
		}

		/// <summary>
		/// ����ִ�д洢���̺�������ϡ�Ӱ��������ͼ��ص�ʵ�弯�ϡ�
		/// </summary>
		/// <param name="storedProcedureParameters">�������ϡ�</param>
		/// <param name="rowsAffected">Ӱ���������</param>
		/// <param name="entities">���ص�ʵ�弯�ϡ�</param>
		public StoredProcedureExecutionResult(DbStoredProcedureParameters storedProcedureParameters, Int32 rowsAffected, TEntity[] entities)
		{
			m_rowsAffected = rowsAffected;
			m_storedProcedureParameters = storedProcedureParameters;
			m_entities = entities;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡִ�д洢����Ӱ���������-1 ��ʾδ֪��
		/// </summary>
		public Int32 RowsAffected
		{
			get { return m_rowsAffected; }
		}

		/// <summary>
		/// ��ȡ�洢���̵Ĳ������ϡ�
		/// </summary>
		public DbStoredProcedureParameters StoredProcedureParameters
		{
			get { return m_storedProcedureParameters; }
		}

		/// <summary>
		/// ��ȡ���ص�ʵ�弯�ϡ�
		/// </summary>
		public TEntity[] Entities
		{
			get { return m_entities; }
		}

		/// <summary>
		/// ��ȡ������еĵ�һ��ʵ�塣
		/// </summary>
		public TEntity First
		{
			get
			{
				if ((m_entities != null) && (m_entities.Length != 0))
				{
					return m_entities[0];
				}
				else
				{
					return null;
				}
			}
		}

		#endregion

		#region ת����

		/// <summary>
		/// ��ʵ�弯��תΪǿ���͡�
		/// </summary>
		/// <param name="executionResult">Ҫת����ִ�н����</param>
		/// <returns>ת�����ִ�н����</returns>
		public static explicit operator StoredProcedureExecutionResult<TEntity>(StoredProcedureExecutionResult<Object> executionResult)
		{
			return new StoredProcedureExecutionResult<TEntity>(executionResult.StoredProcedureParameters, executionResult.RowsAffected, (TEntity[])executionResult.Entities);
		}

		#endregion
	}
}