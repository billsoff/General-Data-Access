#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlStatementType.cs
// �ļ�����������SQL ָ�����͡�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110715
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
	/// SQL ָ�����͡�
	/// </summary>
	internal enum SqlStatementType
	{
		/// <summary>
		/// ����ʶ��
		/// </summary>
		Unrecognisable,

		/// <summary>
		/// ѡ��
		/// </summary>
		Select,

		/// <summary>
		/// ���¡�
		/// </summary>
		Update,

		/// <summary>
		/// ���롣
		/// </summary>
		Insert,

		/// <summary>
		/// ɾ����
		/// </summary>
		Delete
	}
}