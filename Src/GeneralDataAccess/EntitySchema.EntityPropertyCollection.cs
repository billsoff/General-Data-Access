#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntitySchema.EntityPropertyCollection.cs
// �ļ�����������ʵ�����Լ��ϡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110504
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
	partial class EntitySchema
	{
		/// <summary>
		/// ʵ�����Լ��ϡ�
		/// </summary>
		public sealed class EntityPropertyCollection : ReadOnlyCollection<EntityProperty>
		{
			#region ���캯��

			/// <summary>
			/// Ĭ�Ϲ��캯����
			/// </summary>
			/// <param name="properties">�����б�</param>
			public EntityPropertyCollection(IList<EntityProperty> properties)
				: base(properties)
			{
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡ����ָ�����Ƶ����ԡ�
			/// </summary>
			/// <param name="propertyName">�������ơ�</param>
			/// <returns>���и����Ƶ����ԣ����δ�ҵ����򷵻� null��</returns>
			public EntityProperty this[String propertyName]
			{
				get
				{
					foreach (EntityProperty property in Items)
					{
						if (property.Definition.Matches(propertyName))
						{
							return property;
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
			/// <returns>������������Ƶ����ԣ��򷵻� true�����򷵻� false��</returns>
			public Boolean Contains(String propertyName)
			{
				foreach (EntityProperty property in Items)
				{
					if (property.Definition.Matches(propertyName))
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