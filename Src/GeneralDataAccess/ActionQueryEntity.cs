#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ActionQueryEntity.cs
// �ļ�������������װ����ִ�����ݿ��¼���ʵ�壨ָ����ʵ����в��������ͣ���
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��װ����ִ�����ݿ��¼���ʵ�壨ָ����ʵ����в��������ͣ���
	/// </summary>
	[Serializable]
	public class ActionQueryEntity
	{
		#region ˽���ֶ�

		private readonly ActionQueryType m_actionQueryType;
		private readonly Object m_entity;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯������ʼ���������ͺͲ�����ʵ�塣
		/// </summary>
		/// <param name="actionQueryType">�������ͣ���ӡ�ɾ�����޸ġ�</param>
		/// <param name="entity">������ʵ�塣</param>
		public ActionQueryEntity(ActionQueryType actionQueryType, Object entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity", "Ҫ������ʵ�岻��Ϊ�ա�");
			}

			m_actionQueryType = actionQueryType;
			m_entity = entity;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�������͡�
		/// </summary>
		public ActionQueryType ActionQueryType
		{
			get { return m_actionQueryType; }
		}

		/// <summary>
		/// ��ȡ����ʵ�塣
		/// </summary>
		public Object Entity
		{
			get { return m_entity; }
		}

		#endregion
	}
}