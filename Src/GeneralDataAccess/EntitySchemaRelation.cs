#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntitySchemaRelation.cs
// �ļ�������������ʾʵ��ܹ���ϵ��
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
	/// ��ʾʵ��ܹ���ϵ��
	/// </summary>
	public sealed class EntitySchemaRelation
	{
		#region ˽���ֶ�

		private readonly EntityProperty m_childProperty;

		private readonly EntitySchema m_childSchema;
		private readonly EntitySchema m_parentSchema;

		private readonly Column[] m_childColumns;
		private readonly Column[] m_parentColumns;

		private Boolean m_permitNull;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ӳ�����б�
		/// </summary>
		/// <param name="childColumns">��ʵ��ӳ�����б�</param>
		/// <param name="parentColumns">��ʵ��ӳ�����б�</param>
		public EntitySchemaRelation(Column[] childColumns, Column[] parentColumns)
		{
			m_childSchema = childColumns[0].Property.Schema;
			m_parentSchema = parentColumns[0].Property.Schema;

			m_childColumns = childColumns;
			m_parentColumns = parentColumns;

			m_childProperty = childColumns[0].Property;
			m_permitNull = childColumns[0].Definition.Property.PermitNull;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ��ʵ�����б�
		/// </summary>
		public Column[] ChildColumns
		{
			get { return m_childColumns; }
		}

		/// <summary>
		/// ��ȡ��ʵ��ܹ���
		/// </summary>
		public EntitySchema ChildSchema
		{
			get { return m_childSchema; }
		}

		/// <summary>
		/// ��ȡ��ʵ�����б�
		/// </summary>
		public Column[] ParentColumns
		{
			get { return m_parentColumns; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�������Ƿ������ֵ��
		/// </summary>
		public Boolean PermitNull
		{
			get { return m_permitNull; }
			internal set { m_permitNull = value; }
		}

		/// <summary>
		/// ��ȡ��ʵ��ܹ���
		/// </summary>
		public EntitySchema ParentSchema
		{
			get { return m_parentSchema; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡӳ�丸�С�
		/// </summary>
		/// <param name="childColumn">���С�</param>
		/// <returns>ӳ�丸�С�</returns>
		public Column GetMappingParentColumn(Column childColumn)
		{
			Int32 index = Array.IndexOf<Column>(ChildColumns, childColumn);

			if (index < 0)
			{
				return null;
			}

			return ParentColumns[index];
		}

		/// <summary>
		/// ʵ���ϵ�ļ򵥱�ʾ��
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Format("{0}({1}) -> {2}", ChildSchema, ChildProperty, ParentSchema);
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ��ϵ�����������ԡ�
		/// </summary>
		internal EntityProperty ChildProperty
		{
			get { return m_childProperty; }
		}

		#endregion
	}
}