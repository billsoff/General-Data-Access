#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����StoredProcedureParameterValueGotter.cs
// �ļ��������������ڻ�ȡ�洢���̵ķ����������ֵ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110214
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
	/// ���ڻ�ȡ�洢���̵ķ����������ֵ��
	/// </summary>
	/// <param name="parameterName">�������ơ�</param>
	/// <returns>����ֵ��</returns>
	public delegate Object StoredProcedureParameterValueGotter(String parameterName);
}