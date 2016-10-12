#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupItemColumn.cs
// �ļ�������������ʾ��Ϊ�������ݵ��С�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110707
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾ��Ϊ�������ݵ��С�
	/// </summary>
	internal sealed class GroupItemColumn : WrappedColumn
	{
		#region ˽���ֶ�

		private readonly GroupPropertyDefinition m_groupProperty;
		private readonly AggregationAttribute m_aggregation;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯����������Ϊ�������ݵ�ʵ���кͷ�����ʵ�����Զ��塣
		/// </summary>
		/// <param name="entityColumn">ʵ���С�</param>
		public GroupItemColumn(Column entityColumn)
			: this(entityColumn, null)
		{
		}

		/// <summary>
		/// ���캯����������Ϊ�������ݵ�ʵ���кͷ�����ʵ�����Զ��塣
		/// </summary>
		/// <param name="entityColumn">ʵ���С�</param>
		/// <param name="propertyDef">������ʵ�����Զ��塣</param>
		public GroupItemColumn(Column entityColumn, GroupPropertyDefinition propertyDef)
			: base(entityColumn)
		{
			#region ǰ������

			Debug.Assert(entityColumn != null, "ʵ���� entityColumn ����Ϊ�ա�");

			#endregion

			m_groupProperty = propertyDef;

			if (m_groupProperty != null)
			{
				m_aggregation = m_groupProperty.Aggregation;
			}
			else
			{
				m_aggregation = new GroupByAttribute(entityColumn.Property.PropertyChain.PropertyPath);
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�ۺϱ�ǡ�
		/// </summary>
		public AggregationAttribute Aggregation
		{
			get { return m_aggregation; }
		}

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public override String AggregationName
		{
			get
			{
				return m_aggregation.Name;
			}
		}

		/// <summary>
		/// ��ȡ�е����ݿ����͡�
		/// </summary>
		public override DbType DbType
		{
			get
			{
				return CommonPolicies.GetGroupDbType(m_groupProperty, Definition);
			}
		}

		/// <summary>
		/// ��ȡ����ӳ������Ե����ơ�
		/// </summary>
		public override String PropertyName
		{
			get
			{
				return (m_groupProperty != null) ? m_groupProperty.Name : Definition.Name;
			}
		}

		/// <summary>
		/// ��ȡ�����͡�
		/// </summary>
		public override Type Type
		{
			get
			{
				return (m_groupProperty != null) ? m_groupProperty.Type : Definition.Type;
			}
		}

		#endregion
	}
}