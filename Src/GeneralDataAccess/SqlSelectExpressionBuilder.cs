#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlSelectExpressionBuilder.cs
// �ļ�����������ѡ�� SQL ָ����������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110712
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

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ѡ�� SQL ָ����������
	/// </summary>
	public struct SqlSelectExpressionBuilder
	{
		#region ˽���ֶ�

		private Boolean m_distinct;

		private String m_from;
		private String m_select;
		private String m_where;
		private String m_groupBy;
		private String m_having;
		private String m_orderBy;

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾ�Ƿ��� SQL ָ���а��� DISTINCT �ؼ��֣�Ĭ��Ϊ false��
		/// </summary>
		public Boolean Distinct
		{
			get { return m_distinct; }
			set { m_distinct = value; }
		}

		/// <summary>
		/// ��ȡ������ѡ���б�
		/// </summary>
		public String Select
		{
			get { return m_select; }
			set { m_select = value; }
		}

		/// <summary>
		/// ��ȡ�����ñ����ӱ��ʽ��
		/// </summary>
		public String From
		{
			get { return m_from; }
			set { m_from = value; }
		}

		/// <summary>
		/// ��ȡ������ WHERE ���˱��ʽ��
		/// </summary>
		public String Where
		{
			get { return m_where; }
			set { m_where = value; }
		}

		/// <summary>
		/// ��ȡ�����÷�����ʽ��
		/// </summary>
		public String GroupBy
		{
			get { return m_groupBy; }
			set { m_groupBy = value; }
		}

		/// <summary>
		/// ��ȡ������ HAVING ���˱��ʽ��
		/// </summary>
		public String Having
		{
			get { return m_having; }
			set { m_having = value; }
		}

		/// <summary>
		/// ��ȡ������������ʽ��
		/// </summary>
		public String OrderBy
		{
			get { return m_orderBy; }
			set { m_orderBy = value; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ���� SQL ָ�
		/// </summary>
		/// <returns>���ɺõ� SQL ָ�</returns>
		public String Build()
		{
			#region ǰ�ö���

			Debug.Assert(m_select != null, "ѡ���б�Ϊ�գ��޷����� SQL ָ�");
			Debug.Assert(m_from != null, "�����ӱ��ʽΪ�գ��޷����� SQL ָ�");

			#endregion

			// ��ʼ���� SQL ָ��
			StringBuilder buffer = new StringBuilder();

			buffer.AppendFormat(
					"SELECT{0} {1} FROM {2}",
					m_distinct ? " DISTINCT" : String.Empty,
					m_select,
					m_from
				);

			if (m_where != null)
			{
				buffer.AppendFormat(" WHERE {0}", m_where);
			}

			if (m_groupBy != null)
			{
				buffer.AppendFormat(" GROUP BY {0}", m_groupBy);

				if (m_having != null)
				{
					buffer.AppendFormat(" HAVING {0}", m_having);
				}
			}

			Debug.Assert((m_having == null) || (m_groupBy != null), "�������ʽΪ�գ�HAVING ���˱��ʽ���ܲ������� SQL ָ�");

			if (m_orderBy != null)
			{
				buffer.AppendFormat(" ORDER BY {0}", m_orderBy);
			}

			String sql = buffer.ToString();

			return sql;
		}

		#endregion
	}
}