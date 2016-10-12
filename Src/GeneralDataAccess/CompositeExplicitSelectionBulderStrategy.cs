#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeExplicitSelectionBulderStrategy.cs
// �ļ�����������������ʽѡ�����ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110518
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ������ʽѡ�����ԡ�
	/// </summary>
	[Serializable]
	internal sealed class CompositeExplicitSelectionBulderStrategy : CompositeBuilderStrategy
	{
		#region ˽���ֶ�

		private readonly Boolean m_hasSelectors;

		/// <summary>
		/// ʵ�����͡�
		/// </summary>
		private readonly Type m_type;

		private readonly IList<PropertySelector> m_selectorList;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯������������ѡ�������ϡ�
		/// </summary>
		/// <param name="allSelectors">����ѡ�������ϡ�</param>
		public CompositeExplicitSelectionBulderStrategy(IList<PropertySelector> allSelectors)
		{
			#region ǰ������

#if DEBUG

			Debug.Assert(((allSelectors != null) && (allSelectors.Count != 0)), "����ѡ�����б���� selectors ����Ϊ�ջ���б�");

			// ��֤������ͬһ��ʵ��
			Type t = null;

			foreach (PropertySelector selector in allSelectors)
			{
				if (t == null)
				{
					t = selector.Type;
				}
				else if (selector.Type != t)
				{
					Debug.Fail("����ѡ�����б�����һ����ǰ�����ʵ�����Ͳ�ͬ��");
				}
			}

#endif

			#endregion

			// ��ѡ���κ�����
			if ((allSelectors == null) || (allSelectors.Count == 0))
			{
				return;
			}

			m_hasSelectors = true;
			m_type = allSelectors[0].Type;
			m_selectorList = allSelectors;
		}

		#endregion

		#region ����

		/// <summary>
		/// �������м���
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// ��ʽѡ��
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// ����ʵ�塣
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			if (!m_hasSelectors)
			{
				return false;
			}

			foreach (PropertySelector selector in m_selectorList)
			{
				if (selector.LoadSchema(schema))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// ��������ѡ�����ж��Ƿ������ʵ�壨��ѡ���κ����ԣ���
		/// </summary>
		/// <param name="schema">Ҫ�жϵ�ʵ��ܹ���</param>
		/// <returns>�����ѡ���κ����ԣ��򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			if (!m_hasSelectors)
			{
				return true;
			}

			foreach (PropertySelector selector in m_selectorList)
			{
				if (!selector.SelectNothingFrom(schema))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// ѡ�����ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			if (!m_hasSelectors)
			{
				return false;
			}

			foreach (PropertySelector selector in m_selectorList)
			{
				if (selector.SelectProperty(property))
				{
					return true;
				}
			}

			return false;
		}

		#endregion

		#region ���ڵ��Եķ���

		/// <summary>
		/// ��ȡ���ɲ��Ե���ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>���ɲ��Ե���ϸ��Ϣ��</returns>
		public override String Dump()
		{
			if (!m_hasSelectors)
			{
				return String.Format("{0}����ѡ���κ����ԡ�", GetType().FullName);
			}

			const String PADDING = "    "; // ���� 4 ���ո�
			StringBuilder buffer = new StringBuilder();

			foreach (PropertySelector selector in m_selectorList)
			{
				buffer.AppendLine(PADDING + selector.DisplayName);
			}

			return String.Format(
					"{0}��ѡ���������ԣ�\r\n{1}",
					GetType().FullName,
					buffer
				);
		}

		#endregion
	}
}