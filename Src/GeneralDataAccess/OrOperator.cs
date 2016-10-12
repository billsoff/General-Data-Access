#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����OrOperator.cs
// �ļ��������������򡱲�������
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
	/// �򡱲�������
	/// </summary>
	[Serializable, Precedence(FilterOperatorPrecedences.NORMAL, FilterOperatorPrecedences.NORMAL)]
	internal sealed class OrOperator : BinaryOperator
	{
		#region ���캯��

		/// <summary>
		/// ���캯�������ù���������ջ��
		/// </summary>
		/// <param name="filterFactories">����������ջ��</param>
		public OrOperator(IFilterFactoryOperands filterFactories)
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
		/// <returns>���������</returns>
		protected override FilterFactory Process(FilterFactory left, FilterFactory right)
		{
			if (left != null)
			{
				return left.Or(right);
			}
			else
			{
				return right;
			}
		}

		#endregion
	}
}