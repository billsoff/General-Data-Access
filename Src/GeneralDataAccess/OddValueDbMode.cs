#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����OddValueDbMode.cs
// �ļ�����������ָʾ��������ֵ���ת�������ݿ�ֵ��
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
	/// ָʾ��������ֵ���ת�������ݿ�ֵ��
	/// </summary>
	public enum OddValueDbMode
	{
		/// <summary>
		/// ���ı���ֵ��ԭ�����档
		/// </summary>
		NotChange,

		/// <summary>
		/// ת��Ϊ���ݿ��ֵ��
		/// </summary>
		DBNull,

		/// <summary>
		/// д���ݿ�ʱֵ�����ݿ���������ڶ�����ѯָ���к��Ը��У���
		/// </summary>
		Ignore
	}
}