#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����BinaryOperator.cs
// �ļ�������������Ԫ��������������
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
	/// ��Ԫ��������������
	/// </summary>
	[Serializable]
	internal abstract class BinaryOperator : FilterOperator
	{
		#region ���캯��

		/// <summary>
		/// ���캯�������ù���������ջ��
		/// </summary>
		/// <param name="filterFactories">����������ջ��</param>
		protected BinaryOperator(IFilterFactoryOperands filterFactories)
			: base(filterFactories)
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// �������㡣
		/// </summary>
		/// <returns>������ʵ����</returns>
		public sealed override FilterFactory Calculate()
		{
			FilterFactory right = FilterFactories.Pop();
			FilterFactory left = FilterFactories.Pop();

			FilterFactory result = Process(left, right);

			return result;
		}

		#endregion

		#region �����Ա

		#region �����ķ���

		/// <summary>
		/// ����Ŀ�������
		/// </summary>
		/// <param name="left">��Ρ�</param>
		/// <param name="right">�ҲΡ�</param>
		/// <returns>��������������ʵ����</returns>
		protected abstract FilterFactory Process(FilterFactory left, FilterFactory right);

		#endregion

		#endregion
	}
}