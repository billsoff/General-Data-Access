#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����BatchModificationSchemaCache.cs
// �ļ��������������������޸ĵļܹ���Ϣ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110228
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Useease.GeneralDataAccess
{
	internal class BatchModificationSchemaCache
	{
		#region ��̬��Ա

		#region ˽���ֶ�

		private static readonly BatchModificationSchemaCache m_instance;

		#endregion

		#region ���ʼ����

		/// <summary>
		///  ���ʼ������
		/// </summary>
		static BatchModificationSchemaCache()
		{
			m_instance = new BatchModificationSchemaCache();
		}

		#endregion

		#region ��������

		/// <summary>
		/// �ӻ����л�ȡ�ܹ���Ϣ��
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <param name="db">���ݿ⡣</param>
		/// <returns>����ļܹ���Ϣ����������ڣ��򷵻� null��</returns>
		public static BatchModificationSchema GetSchema(Type entityType, Database db)
		{
			return m_instance.DoGetSchema(entityType, db);
		}

		/// <summary>
		/// ����ܹ���Ϣ�����渱����
		/// </summary>
		/// <param name="schema">�ܹ���Ϣ��</param>
		/// <param name="db">���ݿ�</param>
		public static void SetSchema(BatchModificationSchema schema, Database db)
		{
			m_instance.DoSetSchema(schema, db);
		}

		#endregion

		#endregion

		#region ˽���ֶ�

		private readonly Dictionary<Type, BatchModificationSchema> m_bachModificationSchemas;
		private readonly Object m_lock;

		#endregion

		#region ���캯��

		/// <summary>
		/// ˽�У�ʵ�ֵ�����
		/// </summary>
		private BatchModificationSchemaCache()
		{
			m_bachModificationSchemas = new Dictionary<Type, BatchModificationSchema>();
			m_lock = ((ICollection)m_bachModificationSchemas).SyncRoot;
		}

		#endregion

		#region ����ʵ�ַ���

		/// <summary>
		/// �ӻ����л�ȡ�ܹ���Ϣ��
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <param name="db">���ݿ⡣</param>
		/// <returns>����ļܹ���Ϣ����������ڣ��򷵻� null��</returns>
		private BatchModificationSchema DoGetSchema(Type entityType, Database db)
		{
			lock (m_lock)
			{
				if (m_bachModificationSchemas.ContainsKey(entityType))
				{
					BatchModificationSchema schema = m_bachModificationSchemas[entityType];

					return schema.Clone(db);
				}
			}

			return null;
		}

		/// <summary>
		/// ����ܹ���Ϣ�����渱����
		/// </summary>
		/// <param name="schema">�ܹ���Ϣ��</param>
		/// <param name="db">���ݿ�</param>
		private void DoSetSchema(BatchModificationSchema schema, Database db)
		{
			lock (m_lock)
			{
				m_bachModificationSchemas[schema.EntityType] = schema.Clone(db);
			}
		}

		#endregion
	}
}