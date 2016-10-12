#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AggregationColumn.cs
// �ļ�������������ʾһ���ۺ��С�
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
	/// ��ʾһ���ۺ��С�
	/// </summary>
	internal sealed class AggregationColumn : Column
	{
		#region ˽���ֶ�

		private readonly EntityProperty m_aggregationProperty;
		private readonly GroupPropertyDefinition m_groupProperty;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ�ۺϵ����ԣ����Ϊ�գ���ʾ������ʵ��ۺϣ��ͷ�����ʵ�����Զ��塣
		/// </summary>
		/// <param name="aggregationProperty">Ҫ�ۺϵ����ԡ�</param>
		/// <param name="propertyDef">������ʵ�����Զ��塣</param>
		public AggregationColumn(EntityProperty aggregationProperty, GroupPropertyDefinition propertyDef)
		{
			#region ǰ������

			Debug.Assert(
					(aggregationProperty == null) || aggregationProperty.IsPrimitive,
					String.Format(
						"���� {0} ����һ�������У��޷����оۺϼ��㡣",
						(aggregationProperty != null) ? aggregationProperty.PropertyChain.FullName : String.Empty
					)
				);

			#endregion

			m_aggregationProperty = aggregationProperty;
			m_groupProperty = propertyDef;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public override String AggregationName
		{
			get
			{
				return m_groupProperty.Aggregation.Name;
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
		/// ��ȡ�б��ʽ�����ھۺ��У�Ϊ�ۺϱ��ʽ��Ĭ��Ϊ�е�ȫ������
		/// </summary>
		public override String Expression
		{
			get
			{
				return m_groupProperty.Aggregation.GetAggregationExpression(FullName);
			}
		}

		/// <summary>
		/// ��ȡ�е�ȫ���ƣ������޶�����
		/// </summary>
		public override String FullName
		{
			get
			{
				return CommonPolicies.GetColumnFullName(Definition, Property);
			}
		}

		/// <summary>
		/// �Ե�ǰʵ�������壬���Ƿ��� false��
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// ���Ƿ��� true��
		/// </summary>
		public override Boolean IsPrimitive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// �Ե�ǰʵ�������壬���Ƿ��� false��
		/// </summary>
		public override Boolean LazyLoad
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// ��ȡ�����ơ�
		/// </summary>
		public override String Name
		{
			get
			{
				return (Definition != null) ? Definition.Name : null;
			}
		}

		/// <summary>
		/// ��ȡҪ�ۺϵ����ԡ�
		/// </summary>
		public override EntityProperty Property
		{
			get { return m_aggregationProperty; }
		}

		/// <summary>
		/// ��ȡ����ӳ������Ե����ơ�
		/// </summary>
		public override String PropertyName
		{
			get
			{
				return m_groupProperty.Name;
			}
		}

		/// <summary>
		/// ��ȡ�����͡�
		/// </summary>
		public override Type Type
		{
			get
			{
				return m_groupProperty.Type;
			}
		}

		/// <summary>
		/// ��ȡ�ж��塣
		/// </summary>
		internal override ColumnDefinition Definition
		{
			get { return (m_aggregationProperty != null) ? m_aggregationProperty.Definition.Column : null; }
		}

		#endregion
	}
}