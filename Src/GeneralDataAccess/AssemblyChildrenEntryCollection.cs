#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AssemblyChildrenEntryCollection.cs
// �ļ�������������ʵ��װ����ϡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110729
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʵ��װ����ϡ�
	/// </summary>
	[Serializable]
	internal class AssemblyChildrenEntryCollection : ReadOnlyCollection<AssemblyChildrenEntry>
	{
		#region ���캯��

		/// <summary>
		/// ���캯���������������б�
		/// </summary>
		/// <param name="childrenEntries">װ�����б�</param>
		public AssemblyChildrenEntryCollection(IList<AssemblyChildrenEntry> childrenEntries)
			: base(childrenEntries)
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����װ���
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>���������ƥ���װ������û���ҵ����򷵻� null��</returns>
		public AssemblyChildrenEntry this[String propertyName]
		{
			get
			{
				foreach (AssemblyChildrenEntry childrenEntry in Items)
				{
					if (childrenEntry.PropertyName.Equals(propertyName, CommonPolicies.PropertyNameComparison))
					{
						return childrenEntry;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// ����װ���
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>���ҵ���װ������û���ҵ�����Ϊ�ա�</returns>
		public AssemblyChildrenEntry this[String[] propertyPath]
		{
			get
			{
				AssemblyChildrenEntry current = this[propertyPath[0]];

				if ((current != null) && (propertyPath.Length > 1))
				{
					for (Int32 i = 1; i < propertyPath.Length; i++)
					{
						current = current.ChildrenEntries[propertyPath[i]];

						if (current == null)
						{
							break;
						}
					}
				}

				return current;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// �ж��Ƿ����װ���
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>�������װ����򷵻� true�����򷵻� false��</returns>
		public Boolean Contains(String propertyName)
		{
			foreach (AssemblyChildrenEntry childrenEntry in Items)
			{
				if (childrenEntry.PropertyName.Equals(propertyName, CommonPolicies.PropertyNameComparison))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// �ж��Ƿ����װ���
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>�������װ����򷵻� true�����򷵻� false��</returns>
		public Boolean Contains(String[] propertyPath)
		{
			AssemblyChildrenEntry childrenEntry = this[propertyPath];

			return (childrenEntry != null);
		}

		#endregion
	}
}