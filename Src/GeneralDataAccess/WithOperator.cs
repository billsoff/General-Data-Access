#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����WithOperator.cs
// �ļ�����������������ߺ��������������ȼ����൱����������е�Բ���š�
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
	/// ������ߺ��������������ȼ����൱����������е�Բ���š�
	/// </summary>
	[Serializable, Precedence(FilterOperatorPrecedences.HIGHEST, FilterOperatorPrecedences.BELOW_NORMAL)]
	internal sealed class WithOperator : PrefixOperator
	{
		#region ���캯��

		/// <summary>
		/// ���캯�������ù���������ջ��
		/// </summary>
		/// <param name="filterFactories">����������ջ��</param>
		public WithOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region �����ķ���

		/// <summary>
		/// NOP��ԭ�����ز�����
		/// </summary>
		/// <param name="operand">����������</param>
		/// <returns>������������</returns>
		protected override FilterFactory Process(FilterFactory operand)
		{
			return operand;
		}

		#endregion
	}
}