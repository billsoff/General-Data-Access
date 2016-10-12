#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterBuilder.cs
// �ļ��������������ڹ�����������
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
	/// ���ڹ�����������
	/// </summary>
	[Serializable]
	internal partial class FilterBuilder
	{
		#region ˽���ֶ�

		private readonly FilterFactoryStack m_factories = new FilterFactoryStack();
		private readonly FilterOperatorStack m_operators = new FilterOperatorStack();

		private readonly Stack<WithOperator> m_withOps = new Stack<WithOperator>();

		private Object m_lastPushed;

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡһ��ֵ��ָʾ�Ƿ��� WithOperator ѹջ��
		/// </summary>
		public Boolean HasWithToken
		{
			get { return (m_withOps.Count != 0); }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�������Ƿ�Ϊ����״̬��
		/// </summary>
		public Boolean IsEmpty
		{
			get { return (m_factories.IsEmpty && m_operators.IsEmpty); }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ���Խ�����
		/// </summary>
		public Boolean IsResolvable
		{
			get
			{
				return (m_lastPushed is IFilterProvider);
			}
		}

		/// <summary>
		/// ��ȡ����������ջ��
		/// </summary>
		public IFilterFactoryOperands FilterFactories
		{
			get
			{
				return m_factories;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// �������һ�� With ��Ԫ��
		/// </summary>
		/// <returns>���ջ��û�� With ����������ʽ���������򷵻� false�����򷵻� true��</returns>
		public void EndWith()
		{
			// ˫�ر��������� FilterExpression �н������ж�
			if (!HasWithToken || !IsResolvable)
			{
				throw new InvalidOperationException("������ƥ��� With ��������");
			}

			WithOperator withOp = m_withOps.Pop();

			while ((m_operators.Top != null) && (m_operators.Top != withOp))
			{
				CalculateOnce();
			}

			// ���� WithOperator
			CalculateOnce();
		}

		/// <summary>
		/// ����������ѹջ��
		/// </summary>
		/// <param name="factory">������������</param>
		public void Push(FilterFactory factory)
		{
			m_factories.Push(factory);

			m_lastPushed = factory;
		}

		/// <summary>
		/// ������������ѹջ��
		/// </summary>
		/// <param name="op">��������</param>
		public void Push(FilterOperator op)
		{
			Compress(op.LeftPrecedence);

			m_operators.Push(op);

			m_lastPushed = op;

			WithOperator withOp = op as WithOperator;

			if (withOp != null)
			{
				m_withOps.Push(withOp);
			}
		}

		/// <summary>
		/// ���á�
		/// </summary>
		public void Reset()
		{
			m_factories.Clear();
			m_operators.Clear();

			m_lastPushed = null;

			m_withOps.Clear();
		}

		/// <summary>
		/// ��������ȡ��������
		/// </summary>
		/// <returns>��������</returns>
		public FilterFactory Resolve()
		{
			// ˫�ر������� FilterExpression ���ѽ������ж�
			if (!IsResolvable)
			{
				throw new InvalidOperationException("���ʽ����ȫ���޷�������");
			}

			while (!m_operators.IsEmpty)
			{
				CalculateOnce();
			}

			FilterFactory result = m_factories.Pop();

			Reset();

			return result;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����һ�Ρ�
		/// </summary>
		private void CalculateOnce()
		{
			FilterOperator op = m_operators.Pop();

			FilterFactory factory = op.Calculate();

			m_factories.Push(factory);
		}

		/// <summary>
		/// ѹ��ջ��ִ�и���Ŀ�����ȼ��Ĳ�������
		/// </summary>
		/// <param name="targetPrecedence">Ŀ�����ȼ���</param>
		private void Compress(Int32 targetPrecedence)
		{
			while ((m_operators.Top != null) && (m_operators.Top.RightPrecedence < targetPrecedence))
			{
				CalculateOnce();
			}
		}

		#endregion
	}
}