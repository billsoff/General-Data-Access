#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityDefinitionBuilder.cs
// �ļ�����������ʵ�嶨�崴������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110413
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ʵ�嶨�崴������
	/// </summary>
	internal sealed class EntityDefinitionBuilder : IEntityDefinitionProvider
	{
		#region ��̬��Ա

		#region ˽���ֶ�

		private static readonly EntityDefinitionBuilder m_instance;

		#endregion

		#region ���͹��캯��

		/// <summary>
		/// ���ͳ�ʼ������
		/// </summary>
		static EntityDefinitionBuilder()
		{
			m_instance = new EntityDefinitionBuilder();
		}

		#endregion

		#region ��������

		/// <summary>
		/// �������Ͷ��塣
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>ʵ�����Ͷ��塣</returns>
		public static EntityDefinition Build(Type entityType)
		{
			return m_instance.GetDefinition(entityType);
		}

		#endregion

		#endregion

		#region ˽���ֶ�

		private readonly Dictionary<Type, EntityDefinition> m_definitions = new Dictionary<Type, EntityDefinition>();

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯����˽�л�����֧�ֵ�����
		/// </summary>
		private EntityDefinitionBuilder()
		{
		}

		#endregion

		#region IEntityDefinitionProvider ��Ա

		/// <summary>
		/// ��ȡʵ�嶨�塣
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>ʵ�嶨�塣</returns>
		public EntityDefinition GetDefinition(Type entityType)
		{
			Debug.Assert((entityType != null), "ʵ�����Ͳ�������Ϊ�ա�");

			EntityDefinition definition = null;

			if (!m_definitions.ContainsKey(entityType))
			{
				Object syncRoot = ((ICollection)m_definitions).SyncRoot;

				lock (syncRoot)
				{
					if (!m_definitions.ContainsKey(entityType))
					{
						definition = new EntityDefinition(entityType);

						m_definitions[entityType] = definition;

						definition.Initialize(this);
					}
				}
			}

			if (definition == null)
			{
				definition = m_definitions[entityType];
			}

			return definition;
		}

		#endregion
	}
}