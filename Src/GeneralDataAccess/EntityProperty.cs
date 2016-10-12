#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityProperty.cs
// �ļ�����������ʵ�����ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110426
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
	/// ʵ�����ԡ�
	/// </summary>
	public sealed partial class EntityProperty
	{
		#region ˽���ֶ�

		private readonly EntitySchema m_schema;
		private readonly ColumnCollection m_columns;
		private readonly EntityPropertyDefinition m_definition;

		private PropertyChain m_propertyChain;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ��ܹ���ʵ�����Զ��塣
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <param name="definition">ʵ�����Զ��塣</param>
		internal EntityProperty(EntitySchema schema, EntityPropertyDefinition definition)
		{
			#region ǰ������

			Debug.Assert(!definition.IsChildren, "���Զ������ definition ����Ϊ��ʵ���б����ԡ�");

			#endregion

			m_schema = schema;
			m_definition = definition;

			// ������
			m_columns = ComposeColumns();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ʵ��������ӵ�е��м��ϡ�
		/// </summary>
		public ColumnCollection Columns
		{
			get { return m_columns; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾʵ�������Ƿ�ӳ��Ϊ�����С�
		/// </summary>
		public Boolean HasComproundColumns
		{
			get { return m_definition.HasComproundColumns; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ�Ϊ������
		/// </summary>
		public Boolean IsPrimaryKey
		{
			get { return m_definition.IsPrimaryKey; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ�������ԣ����Ϊ false������Ϊ�ⲿ�������ԡ�
		/// </summary>
		public Boolean IsPrimitive
		{
			get
			{
				return Definition.IsPrimitive;
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ��ӳټ��ء�
		/// </summary>
		public Boolean LazyLoad
		{
			get
			{
				return Definition.LazyLoad;
			}
		}

		/// <summary>
		/// ��ȡ�������ơ�
		/// </summary>
		public String Name
		{
			get
			{
				return Definition.Name;
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��������ӳ������Ƿ�����Ϊ�ա�
		/// </summary>
		public Boolean PermitNull
		{
			get
			{
				return Definition.PermitNull;
			}
		}

		/// <summary>
		/// ��ȡ������Ϣ��
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get
			{
				return Definition.PropertyInfo;
			}
		}

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public IPropertyChain PropertyChain
		{
			get
			{
				if (m_propertyChain == null)
				{
					EntitySchemaRelation relation = Schema.LeftRelation;

					if (relation == null)
					{
						m_propertyChain = new PropertyChain(Schema.Type, new String[] { Name });
					}
					else
					{
						IPropertyChain childPropertyChain = relation.ChildProperty.PropertyChain;
						String[] childPropertyPath = childPropertyChain.PropertyPath;
						String[] propertyPath = new String[childPropertyPath.Length + 1];

						Array.Copy(childPropertyPath, propertyPath, childPropertyPath.Length);
						propertyPath[propertyPath.Length - 1] = Name;

						m_propertyChain = new PropertyChain(childPropertyChain.Type, propertyPath);
					}
				}

				return m_propertyChain;
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ��һ�������ⲿ���á�
		/// </summary>
		public Boolean SuppressExpand
		{
			get { return m_definition.SuppressExpand; }
		}

		/// <summary>
		/// ��ȡ�������͡�
		/// </summary>
		public Type Type
		{
			get
			{
				return Definition.Type;
			}
		}

		/// <summary>
		/// ��ȡ����������ʵ��ܹ���
		/// </summary>
		public EntitySchema Schema
		{
			get { return m_schema; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// �����û����Ե��Զ����ǡ�
		/// </summary>
		/// <param name="attributeType">Ҫ�������Զ����ǵ����ͻ�����͡�</param>
		/// <returns>ָ�����͵��Զ����ǡ�</returns>
		public Attribute GetCustomAttribute(Type attributeType)
		{
			return Attribute.GetCustomAttribute(PropertyInfo, attributeType);
		}

		/// <summary>
		/// �����û����Ե��Զ������б�
		/// </summary>
		/// <param name="attributeType">Ҫ�������Զ����ǵ����ͻ�����͡�</param>
		/// <returns>ָ�����͵��Զ����ǵ��б�</returns>
		public Attribute[] GetCustomAttributes(Type attributeType)
		{
			return Attribute.GetCustomAttributes(PropertyInfo, attributeType);
		}

		/// <summary>
		/// ��ѯ���������Ƿ�����ָ�����͵ı�ǡ�
		/// </summary>
		/// <param name="attributeType">Ҫ��ѯ�ı�����͡�</param>
		/// <returns>����������ϱ���˸����͵ı�ǣ��򷵻� true�����򷵻� false��</returns>
		public Boolean IsCustomAttributeDefined(Type attributeType)
		{
			return Attribute.IsDefined(PropertyInfo, attributeType);
		}

		/// <summary>
		/// ��ʾ�������ơ�
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return PropertyChain.FullName;
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡʵ�����Զ��塣
		/// </summary>
		internal EntityPropertyDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// �ϳ�ʵ�����԰������м��ϡ�
		/// </summary>
		/// <returns>�ϳɺõ��м��ϡ�</returns>
		private ColumnCollection ComposeColumns()
		{
			List<Column> columns = new List<Column>();

			foreach (ColumnDefinition columnDef in Definition.Columns)
			{
				columns.Add(new EntityColumn(this, columnDef));
			}

			return new ColumnCollection(columns);
		}

		#endregion
	}
}