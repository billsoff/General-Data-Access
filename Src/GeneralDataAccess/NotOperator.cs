#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NotOperator.cs
// �ļ��������������ǡ���������
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
	/// �ǡ���������
	/// </summary>
	[Serializable, Precedence(FilterOperatorPrecedences.HIGHEST, FilterOperatorPrecedences.HIGHEST)]
	internal sealed class NotOperator : PrefixOperator
	{
		#region ���캯��

		/// <summary>
		/// ���캯�������ù���������ջ��
		/// </summary>
		/// <param name="filterFactories">����������ջ��</param>
		public NotOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region �����ķ���

		/// <summary>
		/// ���С��ǡ�������
		/// </summary>
		/// <param name="operand">��������</param>
		/// <returns>�������������������ʵ����</returns>
		protected override FilterFactory Process(FilterFactory operand)
		{
			if (operand != null)
			{
				return operand.Not();
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}