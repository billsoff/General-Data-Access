#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntitySchema.cs
// �ļ�����������ʵ��ܹ���
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
using System.Diagnostics;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ʵ��ܹ���
	/// </summary>
	public sealed partial class EntitySchema : IColumnLocatable
	{
		#region ˽���ֶ�

		#region ����

		/// <summary>
		/// �����ǰ׺��t__����
		/// </summary>
		private const String TABLE_ALIAS_PREFIX = "t__";

		#endregion

		private Int32 m_level;

		private EntitySchemaComposite m_compositer;
		private readonly EntityDefinition m_entity;

		private readonly EntityPropertyCollection m_properties;
		private readonly EntityPropertyCollection m_primaryKeyProperties;

		private readonly ColumnCollection m_columns;
		private readonly ColumnCollection m_primaryKeyColumns;

		private EntitySchemaRelation m_leftRelation;
		private EntitySchemaRelation[] m_rightRelations;

		/// <summary>
		/// ����ʵ�����Ե����ơ�
		/// </summary>
		private Dictionary<String, EntitySchemaRelation> m_rightRelationLookups;

		private Int32 m_index;
		private String m_propertyPath;

		private Boolean m_selectNothing;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ�嶨�塣
		/// </summary>
		/// <param name="entity">ʵ�嶨�塣</param>
		internal EntitySchema(EntityDefinition entity)
			: this((EntitySchemaComposite)null, entity)
		{
		}

		/// <summary>
		/// ���캯��������ʵ�嶨�塣
		/// </summary>
		/// <param name="composite">ʵ��ܹ������ļܹ���ϡ�</param>
		/// <param name="entity">ʵ�嶨�塣</param>
		internal EntitySchema(EntitySchemaComposite composite, EntityDefinition entity)
		{
			#region ǰ������

			Debug.Assert((entity != null), "ʵ�嶨����� definition ����Ϊ�ա�");

			#endregion

			m_compositer = composite;
			m_entity = entity;

			List<EntityProperty> properties = new List<EntityProperty>();
			List<EntityProperty> primaryKeyProperties = new List<EntityProperty>();

			List<Column> columns = new List<Column>();
			List<Column> primaryKeyColumns = new List<Column>();

			List<EntityPropertyDefinition> selectProperties = new List<EntityPropertyDefinition>();

			foreach (EntityPropertyDefinition propertyDef in entity.Properties)
			{
				if (!propertyDef.IsChildren)
				{
					// ����ʵ������
					EntityProperty property = new EntityProperty(this, propertyDef);

					properties.Add(property);
					columns.AddRange(property.Columns);

					if (propertyDef.IsPrimaryKey)
					{
						primaryKeyProperties.Add(property);
						primaryKeyColumns.AddRange(property.Columns);
					}
				}
			}

			m_properties = new EntityPropertyCollection(properties);
			m_primaryKeyProperties = new EntityPropertyCollection(primaryKeyProperties);

			m_columns = new ColumnCollection(columns);
			m_primaryKeyColumns = new ColumnCollection(primaryKeyColumns);
		}

		#endregion

		#region ��������

		/// <summary>
		/// �����ж�λ������ƥ����м��ϡ�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>�м��ϡ�</returns>
		public Column[] this[ColumnLocator colLocator]
		{
			get
			{
				IPropertyChain chain = colLocator.Create(Type);
				IPropertyChain head = chain.Head;

				EntitySchema schema = this;
				String propertyName = head.Name;
				IPropertyChain next = head.Next;

				while (next != null)
				{
					EntitySchemaRelation relation = schema.GetRightRelation(propertyName);

					schema = relation.ParentSchema;
					propertyName = next.Name;

					next = next.Next;
				}

				Column[] results = schema.GetColumnsByPropertyName(propertyName);

				if (results == null)
				{
					throw new InvalidOperationException(String.Format("��ʵ�� {0} �в��������� {1}��", Entity.Type.FullName, colLocator.FullName));
				}

				return results;
			}
		}

		/// <summary>
		/// ��ȡ�����С�
		/// </summary>
		public ColumnCollection Columns
		{
			get { return m_columns; }
		}

		/// <summary>
		/// ��ȡ��ʾ���ơ�
		/// </summary>
		public String DisplayName
		{
			get { return String.Format("{0} AS {1}", TableName, TableAlias); }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ��и������á�
		/// </summary>
		public Boolean HasRightRelations
		{
			get { return ((m_rightRelations != null) && (m_rightRelations.Length != 0)); }
		}

		/// <summary>
		/// ��ȡʵ��ܹ������š�
		/// </summary>
		public Int32 Index
		{
			get { return m_index; }
			internal set { m_index = value; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ����Դ�Ƿ�Ϊ�洢���̡�
		/// </summary>
		public Boolean IsStoredProcedure
		{
			get { return Entity.IsStoredProcedure; }
		}

		/// <summary>
		/// ��ȡ��ʵ��ܹ���ϵ��
		/// </summary>
		public EntitySchemaRelation LeftRelation
		{
			get { return m_leftRelation; }
			internal set { m_leftRelation = value; }
		}

		/// <summary>
		/// ��ȡʵ��ܹ��ļ���
		/// </summary>
		public Int32 Level
		{
			get { return m_level; }
			internal set { m_level = value; }
		}

		/// <summary>
		/// ��ȡʵ��ܹ���������ϡ�
		/// </summary>
		public EntitySchemaComposite Composite
		{
			get { return m_compositer; }
			internal set { m_compositer = value; }
		}

		/// <summary>
		/// ��ȡ�����м��ϡ�
		/// </summary>
		public ColumnCollection PrimaryKeyColumns
		{
			get { return m_primaryKeyColumns; }
		}

		/// <summary>
		/// ��ȡ�������Լ��ϡ�
		/// </summary>
		public EntityPropertyCollection PrimaryKeyProperties
		{
			get { return m_primaryKeyProperties; }
		}

		/// <summary>
		/// ��ȡ���Լ��ϡ�
		/// </summary>
		public EntityPropertyCollection Properties
		{
			get { return m_properties; }
		}

		/// <summary>
		/// ��ȡʵ��ܹ�������·����
		/// </summary>
		public String PropertyPath
		{
			get
			{
				if (m_propertyPath == null)
				{
					EntitySchemaRelation relation = LeftRelation;

					if (relation == null)
					{
						m_propertyPath = Type.Name;
					}
					else
					{
						m_propertyPath = relation.ChildProperty.PropertyChain.FullName;
					}
				}

				return m_propertyPath;
			}
		}

		/// <summary>
		/// ��ȡ��ʵ��ܹ���ϵ�б�
		/// </summary>
		public EntitySchemaRelation[] RightRelations
		{
			get
			{
				if (m_rightRelations == null)
				{
					m_rightRelations = new EntitySchemaRelation[0];
				}

				return m_rightRelations;
			}

			internal set
			{
				m_rightRelations = value;

				if (m_rightRelations != null)
				{
					m_rightRelationLookups = new Dictionary<String, EntitySchemaRelation>();

					foreach (EntitySchemaRelation relation in m_rightRelations)
					{
						m_rightRelationLookups.Add(relation.ChildColumns[0].Definition.Property.Name, relation);
					}
				}
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ�ѡ���κ����ԡ�
		/// </summary>
		public Boolean SelectNothing
		{
			get { return m_selectNothing; }
			internal set { m_selectNothing = value; }
		}

		/// <summary>
		/// ��ȡ��ı�����
		/// </summary>
		public String TableAlias
		{
			get { return (TABLE_ALIAS_PREFIX + Index.ToString()); }
		}

		/// <summary>
		/// ��ȡ������ơ�
		/// </summary>
		public String TableName
		{
			get { return Entity.TableName; }
		}

		/// <summary>
		/// ��ȡʵ������͡�
		/// </summary>
		public Type Type
		{
			get { return Entity.Type; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// �ϳ�ʵ�壬�ݹ��ִ�С�
		/// </summary>
		/// <param name="dbValues">���ݿ��¼ֵ�б�</param>
		/// <returns>�ϳɺõ�ʵ�塣</returns>
		public Object Compose(Object[] dbValues)
		{
			if (IsNull(dbValues))
			{
				return null;
			}

			Object entity = EtyBusinessObject.CreateCompletely(Type);

			foreach (EntityProperty property in Composite.GetSelectProperties(this))
			{
				Object propertyValue;

				if (property.IsPrimitive)
				{
					Column col = property.Columns[0];
					propertyValue = DbConverter.ConvertFrom(dbValues[col.FieldIndex], col);

					property.PropertyInfo.SetValue(entity, propertyValue, null);
				}
				else
				{
					EntitySchema parentSchema = GetParentSchema(property);

					if ((parentSchema != null) && !parentSchema.SelectNothing)
					{
						if (!Convert.IsDBNull(dbValues[property.Columns[0].FieldIndex]))
						{
							propertyValue = parentSchema.Compose(dbValues);
						}
						else
						{
							propertyValue = null;
						}

						property.PropertyInfo.SetValue(entity, propertyValue, null);
					}
					else
					{
						SetForeignReferencePropertyValuePartially(entity, property, dbValues);
					}
				}
			}

			return entity;
		}

		/// <summary>
		/// ��ȡ��������Ӧ���м��ϡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>�м��ϡ�</returns>
		public Column[] GetColumnsByPropertyName(String propertyName)
		{
			EntityPropertyDefinition property = Entity.Properties[propertyName];

			Column[] results;

			if (property != null)
			{
				results = Columns.GetColumnsByDefinitions(property.Columns);
			}
			else
			{
				results = new Column[0];
			}

			return results;
		}

		/// <summary>
		/// �ж�ʵ��ܹ��Ƿ�������ԣ�ֱ�Ӱ�������
		/// </summary>
		/// <param name="propertyChain">��������</param>
		/// <returns>���ʵ��ܹ����������ԣ��򷵻� true�����򷵻� false��</returns>
		public Boolean OwnProperty(IPropertyChain propertyChain)
		{
			#region ǰ������

			Debug.Assert(propertyChain != null, "���� property ����Ϊ�ա�");

			#endregion

			if (Composite.Target.Type != propertyChain.Type)
			{
				return false;
			}

			if ((propertyChain.Depth - Level) != 1)
			{
				return false;
			}

			return Properties.Contains(propertyChain.Name);
		}

		/// <summary>
		/// ��ʾ������
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Format("{0} -> {1}", PropertyPath, TableAlias);
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡʵ�嶨�塣
		/// </summary>
		internal EntityDefinition Entity
		{
			get { return m_entity; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ���ܹ���
		/// </summary>
		/// <param name="property">ӵ�и��ܹ���ʵ�����ԡ�</param>
		/// <returns></returns>
		private EntitySchema GetParentSchema(EntityProperty property)
		{
			if (HasRightRelations)
			{
				EntitySchemaRelation relationFound = Array.Find<EntitySchemaRelation>(
						RightRelations,
						delegate(EntitySchemaRelation relation) { return (relation.ChildProperty == property); }
					);

				if (relationFound != null)
				{
					return relationFound.ParentSchema;
				}
			}

			return null;
		}

		/// <summary>
		/// ��ȡ���Թ�ϵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>���и����Ƶ����Թ�ϵ��</returns>
		private EntitySchemaRelation GetRightRelation(String propertyName)
		{
			if ((m_rightRelationLookups != null) && m_rightRelationLookups.ContainsKey(propertyName))
			{
				return m_rightRelationLookups[propertyName];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// �ж�ʵ���Ƿ�Ϊ��ֵ��
		/// </summary>
		/// <param name="dbValues">��¼��</param>
		/// <returns>�����¼�в������κ�ֵ���򷵻� true�����򷵻� false��</returns>
		private Boolean IsNull(Object[] dbValues)
		{
			if (SelectNothing)
			{
				return true;
			}

			foreach (Column col in Composite.GetSelectColumns(this))
			{
				if (!Convert.IsDBNull(dbValues[col.FieldIndex]))
				{
					return false;

				}
				else if (col.IsPrimaryKey)
				{
					return true;
				}
			}

			return true;
		}

		/// <summary>
		/// ʹ�������е�ֵ�����������ԣ���������ǵݹ�ġ�
		/// </summary>
		/// <param name="entity">Ҫ�������Ե�ʵ�塣</param>
		/// <param name="property">����ʵ�����ԡ�</param>
		/// <param name="dbValues">���ݿ�ֵ���ϡ�</param>
		private void SetForeignReferencePropertyValuePartially(Object entity, EntityProperty property, Object[] dbValues)
		{
			Object[] values;

			if (!property.HasComproundColumns)
			{
				Column firstColumn = property.Columns[0];
				Object firstValue = DbConverter.ConvertFrom(dbValues[firstColumn.FieldIndex], firstColumn);
				values = new Object[] { firstValue };
			}
			else
			{
				values = new Object[property.Columns.Count];

				for (Int32 i = 0; i < values.Length; i++)
				{
					Column col = property.Columns[i];
					values[i] = DbConverter.ConvertFrom(dbValues[col.FieldIndex], col);
				}
			}

			property.Definition.SetValue(entity, values);
		}

		#endregion
	}
}