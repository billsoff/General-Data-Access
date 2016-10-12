#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ColumnCollection.cs
// �ļ������������м��ϡ�
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
	partial class EntitySchema
	{
		/// <summary>
		/// �м��ϡ�
		/// </summary>
		public sealed class ColumnCollection : ReadOnlyCollection<Column>
		{
			#region ˽���ֶ�

			/// <summary>
			/// �м��ϣ������У����壩��ȫ���ơ�
			/// </summary>
			private readonly Dictionary<String, Column> m_columns;

			#endregion

			#region ���캯��

			/// <summary>
			/// ���캯�������û����е��б�
			/// </summary>
			/// <param name="columnList">�е��б�</param>
			public ColumnCollection(IList<Column> columnList)
				: base(columnList)
			{
				m_columns = new Dictionary<String, Column>();

				foreach (Column col in columnList)
				{
					m_columns.Add(col.Definition.FullName, col);
				}
			}

			#endregion

			#region �ڲ�����

			/// <summary>
			/// ��ȡ���ж�����ƥ����С�
			/// </summary>
			/// <param name="definition">�ж��塣</param>
			/// <returns>����ж�����ƥ����С�</returns>
			internal Column GetColumnByDefinition(ColumnDefinition definition)
			{
				String fullName = definition.FullName;

				if (m_columns.ContainsKey(fullName))
				{
					return m_columns[fullName];
				}
				else
				{
					return null;
				}
			}

			/// <summary>
			/// ��ȡ���ж�����ƥ����С�
			/// </summary>
			/// <param name="definitions">�ж����б�</param>
			/// <returns>��ƥ����м��ϡ�</returns>
			internal Column[] GetColumnsByDefinitions(ColumnDefinition[] definitions)
			{
				Column[] results = new Column[definitions.Length];

				for (Int32 i = 0; i < results.Length; i++)
				{
					results[i] = GetColumnByDefinition(definitions[i]);
				}

				return results;
			}

			#endregion
		}
	}
}