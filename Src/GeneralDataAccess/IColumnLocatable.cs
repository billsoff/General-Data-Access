#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IColumnLocatable.cs
// �ļ�����������ͨ���ж�λ����ȡָ�����м��ϡ�
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
	/// ͨ���ж�λ����ȡָ�����м��ϡ�
	/// </summary>
	public interface IColumnLocatable
	{
		/// <summary>
		/// �����ж�λ������ƥ����м��ϡ�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>�ҵ����м��ϡ�</returns>
		Column[] this[ColumnLocator colLocator] { get; }
	}
}