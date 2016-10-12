#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterOperatorStack.cs
// �ļ��������������ڴ�Ź�������������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110325��
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
	partial class FilterBuilder
	{
		/// <summary>
		/// ���ڴ�Ź�������������
		/// </summary>
		[Serializable]
		private class FilterOperatorStack
		{
			#region ˽���ֶ�

			private readonly Stack<FilterOperator> m_operators = new Stack<FilterOperator>();

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡһ��ֵ����ֵָʾ������ջ�Ƿ�Ϊ�ա�
			/// </summary>
			public Boolean IsEmpty
			{
				get { return (m_operators.Count == 0); }
			}

			/// <summary>
			/// ��ȡջ���Ĳ�������������ջ����
			/// </summary>
			public FilterOperator Top
			{
				get
				{
					if (!IsEmpty)
					{
						return m_operators.Peek();
					}
					else
					{
						return null;
					}
				}
			}

			#endregion

			#region ��������

			/// <summary>
			/// ����ջ���Ĳ�������
			/// </summary>
			/// <returns>��������</returns>
			public FilterOperator Pop()
			{
				if (!IsEmpty)
				{
					return m_operators.Pop();
				}
				else
				{
					return null;
				}
			}

			/// <summary>
			/// ������ѹջ��
			/// </summary>
			/// <param name="op">��������</param>
			public void Push(FilterOperator op)
			{
				m_operators.Push(op);
			}

			/// <summary>
			/// ��ղ�����ջ��
			/// </summary>
			public void Clear()
			{
				m_operators.Clear();
			}

			#endregion
		}
	}
}