#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NotSupportGroupingAttribute.cs
// �ļ��������������ڱ�ǲ�֧�ַ�������ԣ��������ݿ������� BLOB ����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110627
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
	/// ���ڱ�ǲ�֧�ַ�������ԣ��������ݿ������� BLOB ����
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class NotSupportGroupingAttribute : Attribute
	{
	}
}