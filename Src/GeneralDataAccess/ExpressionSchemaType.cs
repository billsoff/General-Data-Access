#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ExpressionSchemaType.cs
// �ļ��������������ʽ�ܹ����͡�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110712
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
	/// ���ʽ�ܹ����͡�
	/// </summary>
	internal enum ExpressionSchemaType
	{
		/// <summary>
		/// δ֪��
		/// </summary>
		Unknown,

		/// <summary>
		/// ʵ�塣
		/// </summary>
		Entity,

		/// <summary>
		/// ���顣
		/// </summary>
		Group
	}
}
