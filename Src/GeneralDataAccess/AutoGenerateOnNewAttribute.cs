#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ��������ڱ�Ǽ����ԣ�ָʾ�ڴ�������ʵ��ʱ�Զ�Ϊ��������ֵ��
// �ļ�����������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110406
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
	/// ���ڱ�Ǽ����ԣ�ָʾ�ڴ�������ʵ��ʱ�Զ�Ϊ��������ֵ��
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class AutoGenerateOnNewAttribute : Attribute
	{
	}
}