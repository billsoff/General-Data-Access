#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����LazyLoadAttribute.cs
// �ļ��������������ʵ�����ԣ�ָʾ�ӳټ��أ�ֻ�����ڷ�������ֵ���ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110415
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
	/// ���ʵ�����ԣ�ָʾ�ӳټ��أ�ֻ�����ڷ�������ֵ���ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class LazyLoadAttribute : Attribute
	{
	}
}