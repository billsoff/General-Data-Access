#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupDefinitionBuilder.cs
// �ļ�����������������ʵ�嶨�崴������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110628
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

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ������ʵ�嶨�崴������
	/// </summary>
	internal sealed class GroupDefinitionBuilder
	{
		#region ��̬��Ա

		#region ˽���ֶ�

		private static readonly GroupDefinitionBuilder m_instance;

		#endregion

		#region ���͹��캯��

		/// <summary>
		/// ���ͳ�ʼ������
		/// </summary>
		static GroupDefinitionBuilder()
		{
			m_instance = new GroupDefinitionBuilder();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����������ʵ�����Ͷ��塣
		/// </summary>
		/// <param name="groupType">������ʵ�����͡�</param>
		/// <returns>�����õķ�����ʵ�����Ͷ��塣</returns>
		public static GroupDefinition Build(Type groupType)
		{
			return m_instance.GetDefinition(groupType);
		}

		#endregion

		#endregion

		#region ˽���ֶ�

		private readonly Dictionary<Type, GroupDefinition> m_definitions = new Dictionary<Type, GroupDefinition>();

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����˽�л�����֧�ֵ�����
		/// </summary>
		private GroupDefinitionBuilder()
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ������ʵ������Ͷ��塣
		/// </summary>
		/// <param name="groupType">������ʵ�����͡�</param>
		/// <returns>������ʵ������Ͷ��塣</returns>
		public GroupDefinition GetDefinition(Type groupType)
		{
			if (!m_definitions.ContainsKey(groupType))
			{
				Object syncRoot = ((ICollection)m_definitions).SyncRoot;

				lock (syncRoot)
				{
					if (!m_definitions.ContainsKey(groupType))
					{
						m_definitions.Add(groupType, new GroupDefinition(groupType));
					}
				}
			}

			return m_definitions[groupType];
		}

		#endregion
	}
}