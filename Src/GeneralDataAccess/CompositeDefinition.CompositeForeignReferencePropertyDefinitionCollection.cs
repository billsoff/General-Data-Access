#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeDefinition.CompositeForeignReferencePropertyDefinitionCollection.cs
// �ļ�������������ʾ����ʵ���ⲿ�������Լ��ϡ�
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Useease.GeneralDataAccess
{
	partial class CompositeDefinition
	{
		/// <summary>
		/// ��ʾ����ʵ���ⲿ�������Լ��ϡ�
		/// </summary>
		internal sealed class CompositeForeignReferencePropertyDefinitionCollection : ReadOnlyCollection<CompositeForeignReferencePropertyDefinition>
		{
			#region ���캯��

			/// <summary>
			/// ���캯����
			/// </summary>
			/// <param name="properties">�ⲿ���������б�</param>
			public CompositeForeignReferencePropertyDefinitionCollection(IList<CompositeForeignReferencePropertyDefinition> properties)
				: base(properties)
			{
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡָ�����Ƶ����ԡ�
			/// </summary>
			/// <param name="propertyName">Ҫ���ҵ����Ե����ơ�</param>
			/// <returns>���и����Ƶ����ԣ����û���ҵ������� null��</returns>
			public CompositeForeignReferencePropertyDefinition this[String propertyName]
			{
				get
				{
					foreach (CompositeForeignReferencePropertyDefinition propertyDef in Items)
					{
						if (propertyDef.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
						{
							return propertyDef;
						}
					}

					return null;
				}
			}

			#endregion

			#region ��������

			/// <summary>
			/// �ж��Ƿ����ָ�����Ƶ����ԡ�
			/// </summary>
			/// <param name="propertyName">�������ơ�</param>
			/// <returns>����������и����Ƶ����ԣ��򷵻� true�����򷵻� false��</returns>
			public Boolean Contains(String propertyName)
			{
				foreach (CompositeForeignReferencePropertyDefinition propertyDef in Items)
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