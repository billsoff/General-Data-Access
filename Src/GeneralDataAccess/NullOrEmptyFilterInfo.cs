#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NullOrEmptyFilterInfo.cs
// �ļ�������������ʾ��Ϊ NULL ����ַ����Ĺ�������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110706
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
	/// ��ʾ��Ϊ NULL ����ַ����Ĺ�������
	/// </summary>
	[Serializable]
	internal sealed class NullOrEmptyFilterInfo : FilterInfo
	{
		#region ���췽��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public NullOrEmptyFilterInfo()
		{
		}

		/// <summary>
		/// ���캯����
		/// </summary>
		/// <param name="negative">ָʾ�Ƿ�ִ�С��ǡ�������</param>
		public NullOrEmptyFilterInfo(Boolean negative)
			: base(negative)
		{
		}

		#endregion

		#region �����ķ���

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>�����õĹ�������</returns>
		protected override Filter DoCreateFilter(IList<String> propertyPath)
		{
			return Filter.Combine(
					LogicOperator.Or,
					Filter.CreateIsNullFilter(propertyPath),
					Filter.CreateEqualsFilter(propertyPath, String.Empty)
				);
		}

		#endregion
	}
}