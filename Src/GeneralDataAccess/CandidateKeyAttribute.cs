#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CandidateKeyAttribute.cs
// �ļ�����������ָʾ����ǵ�����Ϊ�򲹼����ɱ����ʵ��Ķ�������ϡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110725
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
	/// ָʾ����ǵ�����Ϊ�򲹼����ɱ����ʵ��Ķ�������ϡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class CandidateKeyAttribute : Attribute
	{
	}
}