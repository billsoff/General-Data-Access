#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityDefinition.ColumnDefinitionCollection.cs
// �ļ������������ж��弯�ϡ�
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	partial class EntityDefinition
	{
		/// <summary>
		/// �ж��弯�ϡ�
		/// </summary>
		public sealed class ColumnDefinitionCollection : ReadOnlyCollection<ColumnDefinition>
		{
			#region ���캯��

			/// <summary>
			/// ���캯��������Ҫ��װ���ж����б�
			/// </summary>
			/// <param name="allColumnDefinitions">�ж����б�</param>
			public ColumnDefinitionCollection(IList<ColumnDefinition> allColumnDefinitions)
				: base(allColumnDefinitions)
			{
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡָ�����Ƶ��ж��壬�����ִ�Сд��
			/// </summary>
			/// <param name="columnName">�����ơ�</param>
			/// <returns>���и����Ƶ��ж��壬���δ�ҵ����򷵻� null��</returns>
			public ColumnDefinition this[String columnName]
			{
				get
				{
					Debug.Assert(!String.IsNullOrEmpty(columnName), "�����Ʋ���Ϊ�ա�");

					foreach (ColumnDefinition columnDef in Items)
					{
						if (columnDef.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
						{
							return columnDef;
						}
					}

					return null;
				}
			}

			#endregion
		}
	}
}