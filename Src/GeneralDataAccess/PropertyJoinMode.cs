#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PropertyJoinMode.cs
// �ļ�������������������ģʽ������ָʾ����ʵ���и�ʵ��ܹ�����ʽʵ��ܹ�������ģʽ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110707
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
	/// ��������ģʽ������ָʾ����ʵ���и�ʵ��ܹ�����ʽʵ��ܹ�������ģʽ��
	/// </summary>
	public enum PropertyJoinMode
	{
		/// <summary>
		/// �����ӡ�
		/// </summary>
		Inner,

		/// <summary>
		/// �����ӡ�
		/// </summary>
		Left,

		/// <summary>
		/// �����ӡ�
		/// </summary>
		Right
	}
}