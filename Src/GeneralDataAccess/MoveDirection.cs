#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����MoveDirection.cs
// �ļ�������������λ���������� DisplayOrderAllocator��
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
	/// ��λ����
	/// </summary>
	public enum MoveDirection
	{
		/// <summary>
		/// �Ƶ���ǰ��
		/// </summary>
		Top,

		/// <summary>
		/// ������һλ��
		/// </summary>
		Up,

		/// <summary>
		/// ������һλ��
		/// </summary>
		Down,

		/// <summary>
		/// �Ƶ����
		/// </summary>
		Bottom,

		/// <summary>
		/// ���й淶������
		/// </summary>
		Normalize
	}
}