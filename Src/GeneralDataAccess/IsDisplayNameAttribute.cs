#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IsDisplayNameAttribute.cs
// �ļ��������������Ա�����DisplayName����
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
	/// ���Ա�����DisplayName����
	/// </summary>
	public sealed class IsDisplayNameAttribute : PropertyAliasAttribute
	{
		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public IsDisplayNameAttribute()
			: base(WellKnown.DisplayName)
		{
		}

		#endregion
	}
}