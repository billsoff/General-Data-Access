#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IsAsOrganizationAttribute.cs
// �ļ��������������Ա�����Organization����
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
	/// ���Ա�����Organization����
	/// </summary>
	public sealed class IsOrganizationAttribute : PropertyAliasAttribute
	{
		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public IsOrganizationAttribute()
			: base(WellKnown.Organization)
		{
		}

		#endregion
	}
}