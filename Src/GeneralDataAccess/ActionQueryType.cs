#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ActionQueryType.cs
// �ļ������������������ݿ��������ʱ����ʵ����в��������ͣ���ӡ�ɾ�����޸ģ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20100815
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
	/// �������ݿ��������ʱ����ʵ����в��������ͣ���ӡ�ɾ�����޸ģ���
	/// </summary>
	public enum ActionQueryType
	{
		/// <summary>
		/// ��ӡ�
		/// </summary>
		Add,

		/// <summary>
		/// ɾ����
		/// </summary>
		Delete,

		/// <summary>
		/// �޸ġ�
		/// </summary>
		Modify,
	}
}