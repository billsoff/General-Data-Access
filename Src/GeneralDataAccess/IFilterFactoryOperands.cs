#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IFilterFactoryOperands.cs
// �ļ������������������������������� FilterOperator ʹ�á�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110325
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
	/// �������������������� FilterOperator ʹ�á�
	/// </summary>
	internal interface IFilterFactoryOperands
	{
		/// <summary>
		/// ��ȡһ��������
		/// </summary>
		/// <returns>������������</returns>
		FilterFactory Pop();
	}
}