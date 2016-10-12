#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterCompilationResult.cs
// �ļ��������������ڴ�ű���������Ľ����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110706
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ڴ�ű���������Ľ����
	/// </summary>
	[Serializable]
	public sealed class FilterCompilationResult
	{
		#region ˽���ֶ�

		private readonly String m_whereClause;
		private readonly QueryParameter[] m_parameters;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯������ʼ����������
		/// </summary>
		/// <param name="whereClause">�����Ӿ䣨������ WHERE �ؼ��֣���</param>
		/// <param name="parameters">��ѯ��������ѯ���������ƺ�ֵ�����ϡ�</param>
		public FilterCompilationResult(String whereClause, QueryParameter[] parameters)
		{
			m_whereClause = whereClause;
			m_parameters = parameters;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�����Ӿ䣨������ WHERE �ؼ��֣���
		/// </summary>
		public String WhereClause
		{
			get { return m_whereClause; }
		}

		/// <summary>
		/// ��ȡ��ѯ��������ѯ���������ƺ�ֵ�����ϡ�
		/// </summary>
		public QueryParameter[] Parameters
		{
			get { return m_parameters; }
		}

		#endregion
	}
}