#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IsMaleAttribute.cs
// �ļ��������������Ա�����Male����
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
	/// ���Ա�����Male����
	/// </summary>
	public sealed class IsMaleAttribute : PropertyAliasAttribute
	{
		#region ���캯��

		/// <summary>
		/// ���Ա�����Male����
		/// </summary>
		public IsMaleAttribute()
			: base(WellKnown.Male)
		{
		}

		#endregion
	}
}