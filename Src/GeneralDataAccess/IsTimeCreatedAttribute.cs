#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IsTimeCreatedAttribute.cs
// �ļ��������������Ա�����TimeCreated����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110330
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
	/// ���Ա�����TimeCreated����
	/// </summary>
	public sealed class IsTimeCreatedAttribute : PropertyAliasAttribute
	{
		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public IsTimeCreatedAttribute()
			: base(WellKnown.TimeCreated)
		{
		}

		#endregion
	}
}