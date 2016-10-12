#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeUnionBuilderStrategy.cs
// �ļ��������������ɲ��ԵĲ�����
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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ɲ��ԵĲ�����
	/// </summary>
	[Serializable]
	internal sealed class CompositeUnionBuilderStrategy : CompositeBuilderStrategy
	{
		#region ˽���ֶ�

		private readonly List<CompositeBuilderStrategy> m_strategies;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ò����б������б���Ϊ�ա�
		/// </summary>
		/// <param name="strategies">�����б�</param>
		public CompositeUnionBuilderStrategy(IList<CompositeBuilderStrategy> strategies)
		{
			#region ǰ������

#if DEBUG

			Debug.Assert(((strategies != null) && (strategies.Count != 0)), "�����б���Ϊ�ա�");

			// Ӧ������һ��Ԫ�ز�Ϊ��
			Boolean atLeastOneNotNull = false;

			foreach (CompositeBuilderStrategy strategy in strategies)
			{
				if (strategy != null)
				{
					atLeastOneNotNull = true;

					break;
				}
			}

			Debug.Assert(atLeastOneNotNull, "�����б������е�Ԫ�ض�Ϊ�ա�");

#endif

			#endregion

			m_strategies = new List<CompositeBuilderStrategy>();

			foreach (CompositeBuilderStrategy strategy in strategies)
			{
				if ((strategy == null) || m_strategies.Contains(strategy))
				{
					continue;
				}

				if (!(strategy is CompositeUnionBuilderStrategy))
				{
					m_strategies.Add(strategy);
				}
				else
				{
					foreach (CompositeBuilderStrategy childStrategy in ((CompositeUnionBuilderStrategy)strategy).Strategies)
					{
						if (!m_strategies.Contains(childStrategy))
						{
							m_strategies.Add(childStrategy);
						}
					}
				}
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�����б�
		/// </summary>
		public IList<CompositeBuilderStrategy> Strategies
		{
			get { return m_strategies; }
		}

		#endregion

		#region ����

		/// <summary>
		/// �����Ƽ���
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// ָʾ�Ƿ�ѡ��ȫ�����ԡ�
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				foreach (CompositeBuilderStrategy strategy in Strategies)
				{
					if (!strategy.SelectAllProperties)
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// ָʾ�Ƿ����ʵ��ܹ���
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			foreach (CompositeBuilderStrategy strategy in Strategies)
			{
				if (strategy.LoadFromSchema(schema))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// �ж��Ƿ�ѡ��ʵ��ܹ����κ����ԡ�
		/// </summary>
		/// <param name="schema">Ҫ�жϵ�ʵ��ܹ���</param>
		/// <returns>��������б��е�ÿһ����Զ�ָʾ��ѡ���κ����ԣ��򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			foreach (CompositeBuilderStrategy strategy in Strategies)
			{
				if (!strategy.SelectNothingFrom(schema))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// ָʾ�Ƿ�ѡ�����ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			foreach (CompositeBuilderStrategy strategy in Strategies)
			{
				if (strategy.SelectAllProperties || strategy.SelectProperty(property))
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
			const String PADDING = "    "; // ���� 4 ���ո�
			Regex ex = new Regex("^", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			StringBuilder buffer = new StringBuilder();

			foreach (CompositeBuilderStrategy strategy in Strategies)
			{
				if (buffer.Length != 0)
				{
					buffer.AppendLine();
				}

				buffer.AppendLine(ex.Replace(strategy.Dump(), PADDING));
			}

			return String.Format(
					"{0}�������²��ԵĲ�����\r\n{1}",
					GetType().FullName,
					buffer
				);
		}

		#endregion
	}
}