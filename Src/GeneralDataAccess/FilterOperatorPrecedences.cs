#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterOperatorPrecedences.cs
// �ļ���������������ö�ٹ����������������ȼ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110325
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
	/// ����ö�ٹ����������������ȼ���
	/// </summary>
	internal static class FilterOperatorPrecedences
	{
		/// <summary>
		/// ������ȼ� 1��
		/// </summary>
		public const Int32 HIGHEST = 1;

		/// <summary>
		/// ���ȼ� 2��
		/// </summary>
		public const Int32 ABOVE_NORMAL = 2;

		/// <summary>
		/// ���ȼ� 3��
		/// </summary>
		public const Int32 NORMAL = 3;

		/// <summary>
		/// ���ȼ� 4��
		/// </summary>
		public const Int32 BELOW_NORMAL = 4;
	}
}