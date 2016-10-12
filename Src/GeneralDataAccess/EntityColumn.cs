#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityColumn.cs
// �ļ�����������ʵ���С�
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
using System.Data;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ʵ���С�
	/// </summary>
	internal sealed class EntityColumn : Column
	{
		#region ˽���ֶ�

		private ColumnDefinition m_definition;
		private EntityProperty m_property;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯����������������ʵ��ܹ����ж��塣
		/// </summary>
		/// <param name="property">��������ʵ��ܹ���</param>
		/// <param name="columnDef">�ж��塣</param>
		internal EntityColumn(EntityProperty property, ColumnDefinition columnDef)
		{
			m_property = property;
			m_definition = columnDef;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�е����ݿ����͡�
		/// </summary>
		public override DbType DbType
		{
			get { return m_definition.DbType; }
		}

		/// <summary>
		/// ��ȡ�б��ʽ�����ھۺ��У�Ϊ�ۺϱ��ʽ��Ĭ��Ϊ�е�ȫ������
		/// </summary>
		public override String Expression
		{
			get
			{
				return FullName;
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
		/// ��ȡһ��ֵ����ֵָʾ���Ƿ�Ϊ������
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return m_definition.IsPrimaryKey;
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ��ָʾ�����Ƿ�Ϊ�����С�
		/// </summary>
		public override Boolean IsPrimitive
		{
			get { return m_definition.Property.IsPrimitive; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ��ӳټ��ء�
		/// </summary>
		public override Boolean LazyLoad
		{
			get { return m_definition.LazyLoad; }
		}

		/// <summary>
		/// ��ȡ�����ơ�
		/// </summary>
		public override String Name
		{
			get { return m_definition.Name; }
		}

		/// <summary>
		/// ��ȡʵ�����ԡ�
		/// </summary>
		public override EntityProperty Property
		{
			get { return m_property; }
		}

		/// <summary>
		/// ��ȡ����ӳ������Ե����ơ�
		/// </summary>
		public override String PropertyName
		{
			get { return m_property.Name; }
		}

		/// <summary>
		/// ��ȡ�����͡�
		/// </summary>
		public override Type Type
		{
			get { return m_definition.Type; }
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ�ж��塣
		/// </summary>
		internal override ColumnDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion
	}
}