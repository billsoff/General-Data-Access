#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����DbTypeProperty.cs
// �ļ��������������ݿ�����������Ե����ơ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110302
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
	/// ���ݿ�����������Ե����ơ�
	/// </summary>
	public static class DbTypeProperty
	{
		/// <summary>
		/// ���� MySqlDbType��
		/// </summary>
		public const String MySqlDbType = "MySqlDbType";

		/// <summary>
		/// ���� OdbcType��
		/// </summary>
		public const String OdbcType = "OdbcType";

		/// <summary>
		/// ���� OleDbType��
		/// </summary>
		public const String OleDbType = "OleDbType";

		/// <summary>
		/// ���� OracleType��
		/// </summary>
		public const String OracleType = "OracleType";

		/// <summary>
		/// ���� SqlDbType��
		/// </summary>
		public const String SqlDbType = "SqlDbType";
	}
}