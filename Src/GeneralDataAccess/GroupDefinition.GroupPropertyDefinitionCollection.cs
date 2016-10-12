#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupDefinition.GroupPropertyDefinitionCollection.cs
// �ļ�����������������ʵ�����Լ��ϡ�
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	partial class GroupDefinition
	{
		/// <summary>
		/// ������ʵ�����Լ��ϡ�
		/// </summary>
		internal sealed class GroupPropertyDefinitionCollection : ReadOnlyCollection<GroupPropertyDefinition>
		{
			#region ˽���ֶ�

			private readonly GroupDefinition m_group;

			#endregion

			#region ���캯��

			/// <summary>
			/// ���캯�������÷���ʵ�嶨������Լ��ϡ�
			/// </summary>
			/// <param name="group">����ʵ�嶨�塣</param>
			/// <param name="allProperties">���Լ��ϡ�</param>
			public GroupPropertyDefinitionCollection(GroupDefinition group, IList<GroupPropertyDefinition> allProperties)
				: base(allProperties)
			{
				m_group = group;
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡָ�����Ƶķ�����ʵ������ԡ�
			/// </summary>
			/// <param name="propertyName">�������ơ�</param>
			/// <returns>���и����Ƶ����ԣ����δ�ҵ����򷵻� null��</returns>
			public GroupPropertyDefinition this[String propertyName]
			{
				get
				{
					#region ǰ������

					Debug.Assert(!String.IsNullOrEmpty(propertyName), "���� propertyName ����Ϊ null ����ַ�����");

					#endregion

					foreach (GroupPropertyDefinition propertyDef in Items)
					{
						if (propertyDef.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
						{
							return propertyDef;
						}
					}

					return null;
				}
			}

			/// <summary>
			/// ��ȡ����ʵ�嶨�塣
			/// </summary>
			public GroupDefinition Group
			{
				get { return m_group; }
			}

			#endregion

			#region ��������

			/// <summary>
			/// �ж��Ƿ����ָ�����Ƶ����ԡ�
			/// </summary>
			/// <param name="propertyName">Ҫ�жϵ��������ơ�</param>
			/// <returns>������ھ��и����Ƶ����ԣ��򷵻� true�����򷵻� false��</returns>
			public Boolean Contains(String propertyName)
			{
				foreach (GroupPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
					{
						return true;
					}
				}

				return false;
			}

			#endregion
		}
	}
}