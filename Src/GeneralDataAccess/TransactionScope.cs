#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����TransactionScope.cs
// �ļ����������������ռ��������ݿ����������ʵ�塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008150905
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// �����ռ��������ݿ����������ʵ�塣
	/// </summary>
	[Serializable]
	public sealed class TransactionScope
	{
		#region ˽���ֶ�

		private readonly IsolationLevel m_isolationLevel;
		private readonly Boolean m_useDefaultIsolationLevel = true;

		private readonly List<ActionQueryEntity> m_entities = new List<ActionQueryEntity>();

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public TransactionScope()
		{
		}

		/// <summary>
		/// ���캯��������������뼶��
		/// </summary>
		/// <param name="isolationLevel">������뼶��</param>
		public TransactionScope(IsolationLevel isolationLevel)
		{
			m_isolationLevel = isolationLevel;
			m_useDefaultIsolationLevel = false;
		}

		/// <summary>
		/// ���캯������ʼ��Ҫ�������������ʵ�弯�ϡ�
		/// </summary>
		/// <param name="entities">Ҫ�������������ʵ�弯�ϡ�</param>
		public TransactionScope(IList<ActionQueryEntity> entities)
		{
			m_entities.AddRange(entities);
		}

		/// <summary>
		/// ���캯������ʼ��Ҫ�������������ʵ�弯�Ϻ�������뼶��
		/// </summary>
		/// <param name="entities">Ҫ�������������ʵ�弯�ϡ�</param>
		/// <param name="isolationLevel">������뼶��</param>
		public TransactionScope(IList<ActionQueryEntity> entities, IsolationLevel isolationLevel)
			: this(entities)
		{
			m_isolationLevel = isolationLevel;
			m_useDefaultIsolationLevel = false;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡʵ�弯�ϡ�
		/// </summary>
		public ActionQueryEntity[] Entities
		{
			get { return m_entities.ToArray(); }
		}

		/// <summary>
		/// ��ȡ������뼶��
		/// </summary>
		public IsolationLevel IsolationLevel
		{
			get { return m_isolationLevel; }
		}

		/// <summary>
		/// ��ȡһ��ֵ��ָʾ�Ƿ�ʹ��Ĭ�ϵĸ��뼶��
		/// </summary>
		public Boolean UseDefaultIsolationLevel
		{
			get { return m_useDefaultIsolationLevel; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��Ӳ������ͺ�ʵ�塣
		/// </summary>
		/// <param name="actionQueryType">�������͡�</param>
		/// <param name="entity">ʵ�塣</param>
		/// <returns>��ǰ���������</returns>
		public TransactionScope Add(ActionQueryType actionQueryType, Object entity)
		{
			m_entities.Add(new ActionQueryEntity(actionQueryType, entity));

			return this;
		}

		#endregion
	}
}