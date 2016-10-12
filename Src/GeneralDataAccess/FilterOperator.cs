#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterOperator.cs
// �ļ��������������������������������ӹ�������
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
	/// ���������������������ӹ�������
	/// </summary>
	[Serializable]
	internal abstract class FilterOperator
	{
		#region ˽���ֶ�

		private readonly PrecedenceAttribute m_precedences;
		private readonly IFilterFactoryOperands m_filterFactories;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		protected FilterOperator()
		{
			m_precedences = GetPrecedences();
		}

		/// <summary>
		/// ���캯����
		/// </summary>
		/// <param name="filterFactories">������������</param>
		protected FilterOperator(IFilterFactoryOperands filterFactories)
			: this()
		{
			m_filterFactories = filterFactories;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�����ȼ���
		/// </summary>
		public Int32 LeftPrecedence
		{
			get { return m_precedences.LeftPrecedence; }
		}

		/// <summary>
		/// ��ȡ�����ȼ���
		/// </summary>
		public Int32 RightPrecedence
		{
			get { return m_precedences.RightPrecedence; }
		}

		#endregion

		#region ����������

		/// <summary>
		/// ��ȡ����������ջ��
		/// </summary>
		protected IFilterFactoryOperands FilterFactories
		{
			get { return m_filterFactories; }
		}

		#endregion

		#region �����Ա

		#region ��������

		/// <summary>
		/// ���м��㡣
		/// </summary>
		/// <returns>���ɵĹ�����������</returns>
		public abstract FilterFactory Calculate();

		#endregion

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�����������ȼ����á�
		/// </summary>
		/// <returns>�����������ȼ����á�</returns>
		private PrecedenceAttribute GetPrecedences()
		{
			PrecedenceAttribute precedences = (PrecedenceAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PrecedenceAttribute));

			if (precedences == null)
			{
				precedences = PrecedenceAttribute.CreateDefault();
			}

			return precedences;
		}

		#endregion
	}
}