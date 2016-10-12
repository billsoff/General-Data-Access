#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupIncapablePropertyTrimmer.cs
// �ļ��������������ڴ�ѡ���б��Ƴ���֧�ַ�������ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110630
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
	/// ���ڴ�ѡ���б��Ƴ���֧�ַ�������ԡ�
	/// </summary>
	[Serializable]
	internal sealed class GroupIncapablePropertyTrimmer : PropertyTrimmer
	{
		/// <summary>
		/// ��ȡ��ʾ���ơ�
		/// </summary>
		public override String DisplayName
		{
			get
			{
				return "��ѡ���б����Ƴ����Ϊ��֧�ַ��������";
			}
		}

		/// <summary>
		/// ָʾ��ѡ���б����Ƴ����Ϊ��֧�ַ�������ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public override Boolean TrimOff(EntityProperty property)
		{
			return property.IsCustomAttributeDefined(typeof(NotSupportGroupingAttribute));
		}
	}
}