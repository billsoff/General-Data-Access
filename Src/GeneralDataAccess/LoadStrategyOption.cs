#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����LoadStrategyOption.cs
// �ļ��������������ز���ѡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110520
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
	/// ���ز���ѡ�
	/// </summary>
	public enum LoadStrategyOption
	{
		/// <summary>
		/// �����ݿ�Ự����ָ��������Ĭ��ѡ�
		/// <para>��ǰ�Ĳ����Ǽ������е�δ��� SuppressExpand ���ⲿ���ã�ͬһ����·���������ظ������ü��ص��ڶ�����������Ϊֹ��</para>
		/// </summary>
		Auto,

		/// <summary>
		/// ������ʵ�屾��
		/// </summary>
		SelfOnly,

		/// <summary>
		/// ���ص�һ���ⲿ���á�
		/// </summary>
		OneLevel,

		/// <summary>
		/// ָ������
		/// </summary>
		SpecifyLevel,

		/// <summary>
		/// �����Ƽ���
		/// </summary>
		UnlimitedLevel
	}
}