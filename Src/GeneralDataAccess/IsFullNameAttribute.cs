#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IsFullNameAttribute.cs
// �ļ��������������Ա�����FullName����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110329
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
	/// ���Ա�����FullName����
	/// </summary>
	public sealed class IsFullNameAttribute : PropertyAliasAttribute
	{
		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public IsFullNameAttribute()
			: base(WellKnown.FullName)
		{
		}

		#endregion
	}
}