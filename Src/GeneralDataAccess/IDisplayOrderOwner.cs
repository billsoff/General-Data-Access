#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IDisplayOrderOwner.cs
// �ļ�������������ʾʵ��ӵ�� DisplayOrder ���ԣ�������ʽ������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110331
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
	/// ��ʾʵ��ӵ�� DisplayOrder ���ԣ�������ʽ������
	/// </summary>
	public interface IDisplayOrderOwner
	{
		/// <summary>
		/// ��ȡ������ʵ�����ʾ��š�
		/// </summary>
		Int32 DisplayOrder { get; set; }
	}
}