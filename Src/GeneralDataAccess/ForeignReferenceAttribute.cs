#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ForeignReferenceAttribute.cs
// �ļ��������������ʵ���һ������Ϊ�ⲿʵ�塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008132358
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
	/// ���ʵ���һ������Ϊ�ⲿʵ�壬����û��Ҫ���õ����ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class ForeignReferenceAttribute : Attribute
	{
	}
}