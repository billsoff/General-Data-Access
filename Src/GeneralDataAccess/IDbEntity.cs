#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IDbEntity.cs
// �ļ�������������ʾһ�����ݿ�ʵ�壬EtyBusinessObject ʵ�ִ˽ӿڡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110221
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
	/// ����һ�����ݿ�ʵ�壬���ݿ��ɸ�����״̬������Ӧ�Ĳ�����
	/// </summary>
	public interface IDbEntity
	{
		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾʵ������Ƿ��Ѵ����ݿ�ɾ����Ĭ��ֵΪ false��
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">�� Transient Ϊ true��������Ϊ true��</exception>
		Boolean Deleted { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾ��ǰʵ���Ƿ��ѱ��޸ģ�Ĭ��ֵΪ false��
		/// </summary>
		Boolean Dirty { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾ�ͻ��Ƿ���ʾɾ��ʵ�壬Ĭ��ֵΪ false��
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">�� Transient Ϊ true��������Ϊ true��</exception>
		Boolean RequireDelete { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ������󣨼���û�н��г־û�����Ĭ��ֵΪ true��
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">��ǰ��״̬Ϊ false��������Ϊ true��</exception>
		Boolean Transient { get; set; }
	}
}