#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SuppressExpandAttribute.cs
// �ļ���������������ⲿ�������ԣ�ָʾ������ʱ�������������õ��ⲿ���á�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110505
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
	/// ����ⲿ�������ԣ�ָʾ������ʱ�������������õ��ⲿ���á�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class SuppressExpandAttribute : Attribute
	{
	}
}