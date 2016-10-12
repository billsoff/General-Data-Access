#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterFactoryStack.cs
// �ļ��������������ڴ�Ź�����������
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
	partial class FilterBuilder
	{
		/// <summary>
		/// ���ڴ�Ź�����������
		/// </summary>
		[Serializable]
		private class FilterFactoryStack : IFilterFactoryOperands
		{
			#region ˽���ֶ�

			private readonly Stack<FilterFactory> m_filterFactories = new Stack<FilterFactory>();

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡһ��ֵ����ֵָʾ����������ջ�Ƿ�Ϊ�ա�
			/// </summary>
			public Boolean IsEmpty
			{
				get { return (m_filterFactories.Count == 0); }
			}

			/// <summary>
			/// ��ȡջ��������������������ջ����
			/// </summary>
			public FilterFactory Top
			{
				get
				{
					if (!IsEmpty)
					{
						return m_filterFactories.Peek();
					}
					else
					{
						return null;
					}
				}
			}

			#endregion

			#region �����÷�

			/// <summary>
			/// ������������ѹ��ջ��
			/// </summary>
			/// <param name="factory">������������</param>
			public void Push(FilterFactory factory)
			{
				m_filterFactories.Push(factory);
			}

			/// <summary>
			/// ��չ���������ջ��
			/// </summary>
			public void Clear()
			{
				m_filterFactories.Clear();
			}

			#endregion

			#region IFilterFactoryOperands ��Ա

			/// <summary>
			/// ��ȡ����������������
			/// </summary>
			/// <returns></returns>
			public FilterFactory Pop()
			{
				return m_filterFactories.Pop();
			}

			#endregion
		}
	}
}