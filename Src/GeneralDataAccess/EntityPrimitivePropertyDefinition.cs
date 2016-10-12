#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityPrimitivePropertyDefinition.cs
// �ļ�������������ʾʵ��Ļ�����ֵ�����ԡ�
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
	/// ��ʾʵ��Ļ�����ֵ�����ԡ�
	/// </summary>
	internal sealed class EntityPrimitivePropertyDefinition : EntityPropertyDefinition
	{
		#region ˽���ֶ�

		private ColumnDefinition[] m_columns;
		private readonly ColumnAttribute m_columnAttr;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯������������������ʵ�嶨���������Ϣ��
		/// </summary>
		/// <param name="entity">����������ʵ�嶨�塣</param>
		/// <param name="propertyInfo">������Ϣ��</param>
		public EntityPrimitivePropertyDefinition(EntityDefinition entity, PropertyInfo propertyInfo)
			: base(entity, propertyInfo)
		{
			m_columnAttr = (ColumnAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ColumnAttribute));
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ӵ�е��ж��弯�ϡ�
		/// </summary>
		public override ColumnDefinition[] Columns
		{
			get
			{
				if (m_columns == null)
				{
					m_columns = new ColumnDefinition[] { Column };
				}

				return m_columns;
			}
		}

		/// <summary>
		/// ָʾ������ֻӳ��һ�С�
		/// </summary>
		public override Boolean HasComproundColumns
		{
			get { return false; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ������
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return m_columnAttr.IsPrimaryKey;
			}
		}

		/// <summary>
		/// ָʾ������Ϊ������ֵ�����ԡ�
		/// </summary>
		public override Boolean IsPrimitive
		{
			get { return true; }
		}

		/// <summary>
		/// ���Ƿ��� false��
		/// </summary>
		public override bool PermitNull
		{
			get { return false; }
		}

		/// <summary>
		/// ��֧�ִ����ԡ�
		/// </summary>
		public override EntityPropertyDefinitionRelation Relation
		{
			get
			{
				throw new NotSupportedException();
			}

			internal set
			{
				throw new NotSupportedException();
			}
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