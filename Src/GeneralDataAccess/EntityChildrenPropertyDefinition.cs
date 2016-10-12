#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityChildrenPropertyDefinition.cs
// �ļ�������������ʵ���б����Զ��塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110601
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʵ���б����Զ��塣
	/// </summary>
	internal sealed class EntityChildrenPropertyDefinition : EntityPropertyDefinition
	{
		#region ˽���ֶ�

		private EntityPropertyDefinition m_childrenProperty;
		private readonly Sorter m_childrenSorter;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯������������������ʵ�嶨���������Ϣ��
		/// </summary>
		/// <param name="entity">ʵ�嶨�塣</param>
		/// <param name="propertyInfo">������Ϣ��</param>
		public EntityChildrenPropertyDefinition(EntityDefinition entity, PropertyInfo propertyInfo)
			: base(entity, propertyInfo)
		{
			#region ǰ������

			Debug.Assert(
					Attribute.IsDefined(propertyInfo, typeof(ChildrenAttribute)),
					String.Format("���� {0}.{1} ��û�б�� Children��", entity.Type.FullName, propertyInfo.Name)
				);
			Debug.Assert(
					propertyInfo.CanWrite,
					String.Format("���� {0}.{1} ����д���޷�֧���Զ�װ�䣬������Ϊ��д��", entity.Type.FullName, propertyInfo.Name)
				);

			Debug.Assert(propertyInfo.PropertyType.IsArray, String.Format("���� {0}.{1} ����ӦΪ���顣", entity.Type.FullName, propertyInfo.Name));

			#endregion

			m_childrenSorter = OrderByAttribute.ComposeSorter(propertyInfo);
		}

		#endregion

		#region ��ʵ���б�������س�Ա

		#region ��������

		/// <summary>
		/// ���Ƿ��� true��
		/// </summary>
		public override Boolean IsChildren
		{
			get { return true; }
		}

		/// <summary>
		/// ��ȡ��ʵ���б�Ԫ���븸ʵ��Ĺ������ԡ�
		/// </summary>
		public override EntityPropertyDefinition ChildrenProperty
		{
			get
			{
				if (m_childrenProperty == null)
				{
					ChildrenAttribute childrenAttr = (ChildrenAttribute)Attribute.GetCustomAttribute(PropertyInfo, typeof(ChildrenAttribute));
					Type elementType = Type.GetElementType();
					EntityDefinition childrenEntity = Entity.Provider.GetDefinition(elementType);
					m_childrenProperty = childrenEntity.Properties[childrenAttr.PropertyName];

					Debug.Assert(
							m_childrenProperty != null,
							String.Format("���� {0} �в��������� {1}��", elementType.FullName, childrenAttr.PropertyName)
						);

					Debug.Assert(m_childrenProperty.Relation.Parent == Entity, "��ʵ���б�Ԫ���븸ʵ��Ĺ�������û�����õ���ǰ����������ʵ��");
				}

				return m_childrenProperty;
			}
		}

		/// <summary>
		/// ��ȡ��������������
		/// </summary>
		public override Sorter ChildrenSorter
		{
			get { return m_childrenSorter; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��������ָ����ʵ��Ĺ�������
		/// </summary>
		/// <param name="parentEntity">��ʵ�塣</param>
		/// <returns>Filter ʵ������ʵ����ָ���ĸ�ʵ����ͬ��</returns>
		public override Filter ComposeChildrenFilter(Object parentEntity)
		{
			#region ǰ������

			Debug.Assert(parentEntity != null, "��ʵ����� parentEntity ����Ϊ�ա�");
			Debug.Assert(Type.IsAssignableFrom(parentEntity.GetType()), "��ʵ��������뵱ǰ�������������Ͳ����ݡ�");

			#endregion

			return Filter.Create(ChildrenProperty.Name, Is.EqualTo(parentEntity));
		}

		/// <summary>
		/// ��ȡָ����ʵ���������ʵ�塣
		/// </summary>
		/// <param name="parentEntity">��ʵ�塣</param>
		/// <param name="dbSession">���ݿ�Ự���档</param>
		/// <returns>������ָ����ʵ���������ʵ�塣</returns>
		public override Object[] GetAllChildren(Object parentEntity, IDatabaseSession dbSession)
		{
			#region ǰ������

			Debug.Assert(dbSession != null, "���ݿ�Ự������� dbSession ����Ϊ�ա�");

			#endregion

			Object[] children = dbSession.Load(ChildrenProperty.Type, ComposeChildrenFilter(parentEntity), ChildrenSorter);

			foreach (Object child in children)
			{
				ChildrenProperty.PropertyInfo.SetValue(child, parentEntity, null);
			}

			return children;
		}

		#endregion

		#endregion

		#region �޹�����

		/// <summary>
		/// ��֧�֡�
		/// </summary>
		public override ColumnDefinition[] Columns
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// ��֧�֡�
		/// </summary>
		public override Boolean HasComproundColumns
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// ��֧�֡�
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// ���Ƿ��� false��
		/// </summary>
		public override Boolean IsPrimitive
		{
			get { return false; }
		}

		/// <summary>
		/// ���Ƿ��� true��
		/// </summary>
		public override Boolean PermitNull
		{
			get { return true; }
		}

		/// <summary>
		/// ��֧�֡�
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

		#endregion
	}
}