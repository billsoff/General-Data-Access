#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IsAsCategoryAttribute.cs
// �ļ��������������Ա�����Category����
//
//
// ������ʶ���α���billsoff@gmail.com�� 2011
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
	/// ���Ա�����Category����
	/// </summary>
	public sealed class IsCategoryAttribute : PropertyAliasAttribute
	{
		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public IsCategoryAttribute()
			: base(WellKnown.Category)
		{
		}

		#endregion
	}
}