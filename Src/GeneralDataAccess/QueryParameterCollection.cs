#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����QueryParameterCollection.cs
// �ļ�������������ѯ�������ϵ�һ����װ������ʵ�����������
//
//
// ������ʶ���α���billsoff@gmail.com�� 201009022255
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
	/// ��ѯ�������ϵ�һ����װ������ʵ�����������
	/// </summary>
	[Serializable]
	public class QueryParameterCollection : ReadOnlyCollection<QueryParameter>
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ��װ�ļ��ϡ�
		/// </summary>
		/// <param name="parameters"></param>
		public QueryParameterCollection(IList<QueryParameter> parameters)
			: base(parameters)
		{
		}

		#endregion
	}
}