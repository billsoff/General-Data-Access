#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IDebugInfoProvider.cs
// �ļ��������������ṩ���ڵ��Ե�ʵ������ϸ��Ϣ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110720
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
	/// ���ṩ���ڵ��Ե�ʵ������ϸ��Ϣ��
	/// </summary>
	public interface IDebugInfoProvider
	{
		/// <summary>
		/// ��ȡʵ������ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>ʵ������ϸ��Ϣ</returns>
		String Dump();

		/// <summary>
		/// ��ȡʵ������ϸ��Ϣ����ָ�����������ڵ��ԡ�
		/// </summary>
		/// <param name="indent">������</param>
		/// <returns>ʵ������ϸ��Ϣ��</returns>
		String Dump(String indent);

		/// <summary>
		/// ��ȡʵ������ϸ��Ϣ����ָ�������������ڵ��ԡ�
		/// </summary>
		/// <param name="level">��������</param>
		/// <returns>ʵ������ϸ��Ϣ��</returns>
		String Dump(Int32 level);

		/// <summary>
		/// ��ȡʵ������ϸ��Ϣ����ָ�������ͼ������ڵ��ԡ�
		/// </summary>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		/// <returns>ʵ������ϸ��Ϣ��</returns>
		String Dump(String indent, Int32 level);
	}
}