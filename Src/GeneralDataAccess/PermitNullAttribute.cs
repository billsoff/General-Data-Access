#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PermitNullAttribute.cs
// �ļ��������������ڱ��ʵ�����ԣ�ָʾ�����Ϊ�գ��ڲ�ѯʱ�����������Ϊ�����ӣ�����Ϊ�����ӣ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008132309
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
	/// ���ڱ��ʵ�����ԣ�ָʾ�����Ϊ�գ��ڲ�ѯʱ�����������Ϊ�����ӣ�����Ϊ�����ӣ���
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class PermitNullAttribute : Attribute
	{
	}
}