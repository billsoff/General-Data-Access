#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityPropertyDefinitionRelation.cs
// �ļ�������������ʾʵ���ⲿ�������Թ�ϵ��
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾʵ���ⲿ�������Թ�ϵ��
	/// </summary>
	internal sealed class EntityPropertyDefinitionRelation
	{
		#region ˽���ֶ�

		private readonly EntityPropertyDefinition m_childProperty;
		private readonly EntityPropertyDefinition[] m_parentProperties;

		private readonly EntityDefinition m_child;
		private readonly EntityDefinition m_parent;

		private readonly ColumnDefinition[] m_childColumns;
		private readonly ColumnDefinition[] m_parentColumns;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯����������ӳ������ж����б�͸��ж����б�
		/// </summary>
		/// <param name="childColumns">���ж����б�</param>
		/// <param name="parentColumns">���ж����б�</param>
		internal EntityPropertyDefinitionRelation(ColumnDefinition[] childColumns, ColumnDefinition[] parentColumns)
		{
			m_childProperty = childColumns[0].Property;

			m_child = childColumns[0].Property.Entity;
			m_parent = parentColumns[0].Property.Entity;

			m_childColumns = childColumns;
			m_parentColumns = parentColumns;

			// ȡ����ʵ������
			List<EntityPropertyDefinition> parentProperties = new List<EntityPropertyDefinition>();

			foreach (ColumnDefinition columnDef in parentColumns)
			{
				if (!parentProperties.Contains(columnDef.Property))
				{
					parentProperties.Add(columnDef.Property);
				}
			}

			m_parentProperties = parentProperties.ToArray();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�ⲿ�������ԡ�
		/// </summary>
		public EntityPropertyDefinition ChildProperty
		{
			get { return m_childProperty; }
		}

		/// <summary>
		/// ��ȡ����ʵ�嶨�塣
		/// </summary>
		public EntityDefinition Child
		{
			get { return m_child; }
		}

		/// <summary>
		/// ��ȡ������ʵ�嶨�塣
		/// </summary>
		public EntityDefinition Parent
		{
			get { return m_parent; }
		}

		/// <summary>
		/// ��ȡ�����ж����б�
		/// </summary>
		public ColumnDefinition[] ChildColumns
		{
			get { return m_childColumns; }
		}

		/// <summary>
		/// ��ȡ�������ж����б�
		/// </summary>
		public ColumnDefinition[] ParentColumns
		{
			get { return m_parentColumns; }
		}

		/// <summary>
		/// ��ȡ�����õ����ԡ�
		/// </summary>
		public EntityPropertyDefinition[] ParentProperties
		{
			get { return m_parentProperties; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡָ�����Ƶĸ����ԡ�
		/// </summary>
		/// <param name="parentPropertyName">���������ơ�</param>
		/// <returns>���и����Ƶĸ����ԣ����û���ҵ����򷵻� null��</returns>
		public EntityPropertyDefinition GetParentPropertyByName(String parentPropertyName)
		{
			EntityPropertyDefinition parentProperty = Array.Find<EntityPropertyDefinition>(
					this.ParentProperties,
					delegate(EntityPropertyDefinition property)
					{
						return property.Name.Equals(parentPropertyName, CommonPolicies.PropertyNameComparison);
					}
				);

			return parentProperty;
		}

		/// <summary>
		/// ʵ�����Թ�ϵ�ļ򵥱�ʾ��
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Format("{0} -> {1}", ChildProperty, Parent);
		}

		#endregion
	}
}