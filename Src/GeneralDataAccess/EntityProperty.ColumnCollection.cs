#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityProperty.ColumnCollection.cs
// �ļ������������м��ϡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110426
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
	partial class EntityProperty
	{
		/// <summary>
		/// �м��ϡ�
		/// </summary>
		public sealed class ColumnCollection : ReadOnlyCollection<Column>
		{
			#region ���캯��

			/// <summary>
			/// ���캯����
			/// </summary>
			/// <param name="cols">�м��ϡ�</param>
			public ColumnCollection(IList<Column> cols)
				: base(cols)
			{
			}

			#endregion
		}
	}
}