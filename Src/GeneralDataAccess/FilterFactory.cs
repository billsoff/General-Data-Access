#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterFactory.cs
// �ļ������������������ɹ�������
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
	/// �������ɹ�������
	/// </summary>
	[Serializable]
	public sealed class FilterFactory : IFilterProvider
	{
		#region ˽���ֶ�

		private readonly IList<String> m_propertyPath;
		private readonly FilterInfo m_filterInfo;

		private Filter m_filter;
		private Boolean m_created;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		internal FilterFactory()
		{
			m_created = true;
		}

		/// <summary>
		/// ���캯����ֱ�����ù�������
		/// </summary>
		/// <param name="filter"></param>
		public FilterFactory(Filter filter)
			: this()
		{
			m_filter = filter;
		}

		/// <summary>
		/// ���캯������������·���͹�������Ϣ�������Ƚϲ�������Ҫ�Ƚϵ�ֵ����
		/// </summary>
		/// <param name="propertyPath">ֵ����·����</param>
		/// <param name="filterInfo">��������Ϣ��</param>
		public FilterFactory(IList<String> propertyPath, FilterInfo filterInfo)
		{
			m_propertyPath = propertyPath;
			m_filterInfo = filterInfo;
		}

		#endregion

		#region ��̬��Ա

		/// <summary>
		/// ����Ĭ�Ϲ�����������
		/// </summary>
		/// <returns>������������</returns>
		public static FilterFactory CreateDefault()
		{
			return new FilterFactory();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public Filter Filter
		{
			get
			{
				if (!m_created)
				{
					m_filter = Create();

					m_created = true;
				}

				return m_filter;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ָ���Ĺ��������������롱������
		/// </summary>
		/// <param name="target">ָ���Ĺ�����������</param>
		/// <returns>���������</returns>
		public FilterFactory And(FilterFactory target)
		{
			return Combine(LogicOperator.And, target);
		}

		/// <summary>
		/// ��ָ���Ĺ��������������򡱲�����
		/// </summary>
		/// <param name="target">ָ���Ĺ�����������</param>
		/// <returns>���������</returns>
		public FilterFactory Or(FilterFactory target)
		{
			return Combine(LogicOperator.Or, target);
		}

		/// <summary>
		/// �Ե�ǰʵ��ִ�С��ǡ�������
		/// </summary>
		/// <returns>���������</returns>
		public FilterFactory Not()
		{
			FilterFactory result = this;

			if (Filter != null)
			{
				if (Filter.HasFilters)
				{
					Filter.Not();
				}
				else
				{
					Filter clone = Filter.Clone();

					clone.Not();

					result = new FilterFactory(clone);
				}
			}

			return result;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ָ�����߼����������ӵ�ǰʵ����ָ���Ĺ�����������
		/// </summary>
		/// <param name="logicOperator">�߼���������</param>
		/// <param name="target">Ŀ����˹�����������</param>
		/// <returns>���ӽ����������������</returns>
		private FilterFactory Combine(LogicOperator logicOperator, FilterFactory target)
		{
			if (target == null)
			{
				return this;
			}

			Filter left = Filter;
			Filter right = target.Filter;

			if (right == null)
			{
				return this;
			}
			else if (left == null)
			{
				return target;
			}

			// ����Ҳ��Ǹ��Ϲ�����
			if (right.HasFilters)
			{
				if (!right.Negative && (right.LogicOperator == logicOperator))
				{
					right.Filters.Insert(0, left);

					return target;
				}
			}
			// �������Ǹ��Ϲ�����
			else if (left.HasFilters)
			{
				if (!left.Negative && left.LogicOperator == logicOperator)
				{
					left.Filters.Add(right);

					return this;
				}
			}

			CompositeFilter compround = new CompositeFilter(logicOperator, new Filter[] { left, right });

			return new FilterFactory(compround);
		}

		/// <summary>
		/// ������������
		/// </summary>
		/// <returns>�����õĹ�������</returns>
		private Filter Create()
		{
			if (m_filterInfo != null)
			{
				return m_filterInfo.CreateFilter(m_propertyPath);
			}
			else
			{
				return m_filter;
			}
		}

		#endregion
	}
}