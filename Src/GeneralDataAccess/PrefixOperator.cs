#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PrefixOperator.cs
// �ļ�����������һԪ��������������
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
	/// һԪ��������������
	/// </summary>
	[Serializable]
	internal abstract class PrefixOperator : FilterOperator
	{
		#region ���캯��

		/// <summary>
		/// ���캯�������ù���������ջ��
		/// </summary>
		/// <param name="filterFactories">����������ջ��</param>
		public PrefixOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// �������㡣
		/// </summary>
		/// <returns>������������������ʵ����</returns>
		public sealed override FilterFactory Calculate()
		{
			FilterFactory operand = FilterFactories.Pop();

			FilterFactory result = Process(operand);

			return result;
		}

		#endregion

		#region �����Ա

		#region �����ķ���

		/// <summary>
		/// ����һԪ������
		/// </summary>
		/// <param name="operand">������</param>
		/// <returns>�������������������ʵ����</returns>
		protected abstract FilterFactory Process(FilterFactory operand);

		#endregion

		#endregion
	}
}