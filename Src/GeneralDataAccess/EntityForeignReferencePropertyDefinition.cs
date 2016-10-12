#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityForeignReferencePropertyDefinition.cs
// �ļ�������������ʾ�ⲿ�������ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110413
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
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾ�ⲿ�������ԡ�
	/// </summary>
	internal sealed class EntityForeignReferencePropertyDefinition : EntityPropertyDefinition
	{
		#region ˽���ֶ�

		private readonly Boolean m_permitNull;
		private EntityPropertyDefinitionRelation m_relation;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯������������������ʵ�嶨�塢������Ϣ��ָʾ�ⲿ���ÿɷ�Ϊ�ա�
		/// </summary>
		/// <param name="owner">����������ʵ�嶨�塣</param>
		/// <param name="propertyInfo">������Ϣ��</param>
		/// <param name="permitNull">ָʾ�ⲿ���ÿɷ�ա�</param>
		public EntityForeignReferencePropertyDefinition(EntityDefinition owner, PropertyInfo propertyInfo, Boolean permitNull)
			: base(owner, propertyInfo)
		{
			m_permitNull = permitNull;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�����������С�
		/// </summary>
		public override ColumnDefinition[] Columns
		{
			get { return m_relation.ChildColumns; }
		}

		/// <summary>
		/// �ж������Ƿ�ӳ��Ϊ�����С�
		/// </summary>
		public override Boolean HasComproundColumns
		{
			get { return (Columns.Length > 1); }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ������
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return Attribute.IsDefined(PropertyInfo, typeof(PrimaryKeyAttribute));
			}
		}

		/// <summary>
		/// ָʾ��ǰ����Ϊ�ⲿ�������ԡ�
		/// </summary>
		public override Boolean IsPrimitive
		{
			get { return false; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�ⲿ���ÿɷ�Ϊ�ա�
		/// </summary>
		public override Boolean PermitNull
		{
			get { return m_permitNull; }
		}

		/// <summary>
		/// ��ȡ���Թ�ϵ��
		/// </summary>
		public override EntityPropertyDefinitionRelation Relation
		{
			get { return m_relation; }
			internal set { m_relation = value; }
		}

		#region ��ʵ���б��������

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�������Ƿ�Ϊ��ʵ���б����Ƿ��� false��
		/// </summary>
		public override Boolean IsChildren
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// ��ȡ��ʵ���б������б�Ԫ�ص����Զ��塣
		/// </summary>
		public override EntityPropertyDefinition ChildrenProperty
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// ��ȡ��ʵ���б����Ե������������ڼ�����ʵ���б�
		/// </summary>
		public override Sorter ChildrenSorter
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		#endregion

		#endregion

		#region ��ʵ���б���ط���

		/// <summary>
		/// �������ڼ�����ʵ��Ĺ�������
		/// </summary>
		/// <param name="parentEntity">��ʵ�����</param>
		/// <returns>Filter ʵ�������ڵõ��ø�ʵ��ӵ�е�������ʵ�塣</returns>
		public override Filter ComposeChildrenFilter(Object parentEntity)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// ��ȡ���е���ʵ�塣
		/// </summary>
		/// <param name="parentEntity">��ʵ�塣</param>
		/// <param name="dbSession">���ݿ�Ự���档</param>
		/// <returns>��ʵ���б�</returns>
		public override Object[] GetAllChildren(Object parentEntity, IDatabaseSession dbSession)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}