#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeDefinitionBuilder.cs
// �ļ������������������ɸ���ʵ�嶨�塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110708
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
	/// �������ɸ���ʵ�嶨�塣
	/// </summary>
	internal sealed class CompositeDefinitionBuilder
	{
		#region ��̬��Ա

		#region ˽���ֶ�

		private static readonly CompositeDefinitionBuilder m_instance;

		#endregion

		#region ���͹��캯��

		/// <summary>
		/// ���ͳ�ʼ������
		/// </summary>
		static CompositeDefinitionBuilder()
		{
			m_instance = new CompositeDefinitionBuilder();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��������ʵ�嶨�塣
		/// </summary>
		/// <param name="compositeResultType">����ʵ�����͡�</param>
		/// <returns>�����õĸ���ʵ�嶨�塣</returns>
		public static CompositeDefinition Build(Type compositeResultType)
		{
			return m_instance.GetDefinition(compositeResultType);
		}

		#endregion

		#endregion

		#region ˽���ֶ�

		private readonly Dictionary<Type, CompositeDefinition> m_definitions = new Dictionary<Type, CompositeDefinition>();

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯����˽�л�����֧�ֵ�����
		/// </summary>
		private CompositeDefinitionBuilder()
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ʵ�嶨�塣
		/// </summary>
		/// <param name="compositeResultType">����ʵ�����͡�</param>
		/// <returns>����ʵ�嶨�塣</returns>
		public CompositeDefinition GetDefinition(Type compositeResultType)
		{
			if (!m_definitions.ContainsKey(compositeResultType))
			{
				Object syncRoot = ((ICollection)m_definitions).SyncRoot;

				lock (syncRoot)
				{
					if (!m_definitions.ContainsKey(compositeResultType))
					{
						m_definitions.Add(compositeResultType, new CompositeDefinition(compositeResultType));
					}
				}
			}

			return m_definitions[compositeResultType];
		}

		#endregion
	}
}