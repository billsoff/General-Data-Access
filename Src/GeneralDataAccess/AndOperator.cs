#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AndOperator.cs
// �ļ��������������롱��������
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
	/// ���롱��������
	/// </summary>
	[Serializable, Precedence(FilterOperatorPrecedences.ABOVE_NORMAL, FilterOperatorPrecedences.ABOVE_NORMAL)]
	internal sealed class AndOperator : BinaryOperator
	{
		#region ���캯��

		/// <summary>
		/// ���캯�������ù���������ջ��
		/// </summary>
		/// <param name="filterFactories">����������ջ��</param>
		public AndOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region �����ķ���

		/// <summary>
		/// ���С��롱������
		/// </summary>
		/// <param name="left">��Ρ�</param>
		/// <param name="right">�ҲΡ�</param>
		/// <returns>��������������ʵ����</returns>
		protected override FilterFactory Process(FilterFactory left, FilterFactory right)
		{
			if (left != null)
			{
				return left.And(right);
			}
			else
			{
				return right;
			}
		}

		#endregion
	}
}