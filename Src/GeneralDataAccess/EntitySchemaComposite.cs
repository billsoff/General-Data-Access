#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntitySchemaComposite.cs
// �ļ��������������ݼܹ���ϣ���ʾһ�������ı����ӡ�
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
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ݼܹ���ϣ���ʾһ�������ı����ӡ�
	/// </summary>
	public sealed class EntitySchemaComposite : IDebugInfoProvider
	{
		#region ˽���ֶ�

		private readonly CompositeBuilderStrategy m_builderStrategy;
		private readonly EntitySchema m_target;
		private EntitySchema[][] m_allSchemas;
		private readonly Dictionary<String, EntitySchema> m_schemaLookups = new Dictionary<String, EntitySchema>();

		private Column[] m_columns;

		private readonly Dictionary<EntitySchema, EntityProperty[]> m_allProperties = new Dictionary<EntitySchema, EntityProperty[]>();
		private readonly Dictionary<EntitySchema, Column[]> m_allColumns = new Dictionary<EntitySchema, Column[]>();

		private Int32 m_rank;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ��ܹ���
		/// </summary>
		/// <param name="buildStrategy">���ɲ��ԡ�</param>
		/// <param name="target">ʵ��ܹ���</param>
		public EntitySchemaComposite(CompositeBuilderStrategy buildStrategy, EntitySchema target)
		{
			m_builderStrategy = buildStrategy;
			m_target = target;
			m_target.Composite = this;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡָ�������ʵ��ܹ��б���������㣬��󼶱�Ϊ Rank - 1��
		/// </summary>
		/// <param name="level">����</param>
		/// <returns>ָ�������ʵ��ܹ��б�</returns>
		public EntitySchema[] this[Int32 level]
		{
			get { return m_allSchemas[level]; }
		}

		/// <summary>
		/// ��ȡ����������λ��ʵ��ܹ���
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>�ɸ���������λ��ʵ��ܹ������δ�ҵ����򷵻� null��</returns>
		public EntitySchema this[IPropertyChain chain]
		{
			get
			{
				EntitySchema result;
				m_schemaLookups.TryGetValue(chain.FullName, out result);

				return result;
			}
		}

		/// <summary>
		/// ��ȡ������·����λ��ʵ��ܹ���
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>�ɸ�����·����λ��ʵ��ܹ������δ�ҵ����򷵻� null��</returns>
		public EntitySchema this[String propertyPath]
		{
			get
			{
				EntitySchema result;
				m_schemaLookups.TryGetValue(propertyPath, out result);

				return result;
			}
		}

		/// <summary>
		/// ��ȡ���ɲ��ԡ�
		/// </summary>
		public CompositeBuilderStrategy BuilderStrategy
		{
			get { return m_builderStrategy; }
		}

		/// <summary>
		/// ��ȡ�ܳ������������
		/// </summary>
		public Int32 ColumnCount
		{
			get { return m_columns.Length; }
		}

		/// <summary>
		/// ѡ���м��ϡ�
		/// </summary>
		public Column[] Columns
		{
			get { return m_columns; }
		}

		/// <summary>
		/// ��ȡһ��ֵ���ж��Ƿ�Ϊ�洢���̡�
		/// </summary>
		public Boolean IsStoredProcedure
		{
			get { return Target.IsStoredProcedure; }
		}

		/// <summary>
		/// ��ȡ�ܹ���ϵ��ȣ������㼶�����ֻȡ��һ�����ã�����ֵΪ 2��
		/// </summary>
		public Int32 Rank
		{
			get { return m_rank; }
		}

		/// <summary>
		/// Ŀ��ܹ���
		/// </summary>
		public EntitySchema Target
		{
			get { return m_target; }
		}

		/// <summary>
		/// ��ȡ������ʵ��ܹ�������
		/// </summary>
		public Int32 TotalSchemaCount
		{
			get { return m_allSchemas.Length; }
		}

		#region SQL ָ��Ƭ��

		private String m_fromList;

		/// <summary>
		/// ��ȡ��ѯ�ı����ӱ��ʽ��
		/// </summary>
		public String FromList
		{
			get
			{
				if (m_fromList == null)
				{
					m_fromList = ComposeFromList();
				}

				return m_fromList;
			}
		}

		/// <summary>
		/// ������ѯ�ı����ӱ��ʽ��
		/// </summary>
		/// <returns></returns>
		private String ComposeFromList()
		{
			StringBuilder fromListBuilder = new StringBuilder();

			fromListBuilder.AppendFormat("{0} {1}", Target.TableName, Target.TableAlias);

			for (Int32 level = 1; level < Rank; level++)
			{
				foreach (EntitySchema schema in this[level])
				{
					EntitySchemaRelation leftRelation = schema.LeftRelation;

					if (leftRelation.PermitNull)
					{
						fromListBuilder.Append(" LEFT JOIN ");
					}
					else
					{
						fromListBuilder.Append(" INNER JOIN ");
					}

					// Oracle ��֧���ڱ����ͱ����֮��� AS �ؼ��֣��ÿո�ָ�����
					fromListBuilder.AppendFormat(
							"{0} {1} ON ",
							leftRelation.ParentSchema.TableName,
							leftRelation.ParentSchema.TableAlias
						);

					for (Int32 i = 0; i < leftRelation.ChildColumns.Length; i++)
					{
						if (i > 0)
						{
							fromListBuilder.Append(" AND ");
						}

						fromListBuilder.AppendFormat(
								"{0}={1}",
								leftRelation.ChildColumns[i].FullName,
								leftRelation.ParentColumns[i].FullName
							);
					}
				}
			}

			return fromListBuilder.ToString();
		}

		private String m_selectList;

		/// <summary>
		/// ��ȡ��ѯ��ѡ���б�
		/// </summary>
		public String SelectList
		{
			get
			{
				if (m_selectList == null)
				{
					m_selectList = ComposeSelectList();
				}

				return m_selectList;
			}
		}

		/// <summary>
		/// ������ѯ��ѡ���б�
		/// </summary>
		/// <returns>ѡ���б�</returns>
		private String ComposeSelectList()
		{
			StringBuilder selectListBuilder = new StringBuilder();

			foreach (Column col in Columns)
			{
				if (selectListBuilder.Length != 0)
				{
					selectListBuilder.Append(",");
				}

				selectListBuilder.AppendFormat("{0} {1}", col.FullName, col.Alias);
			}

			return selectListBuilder.ToString();
		}

		#endregion

		#endregion

		#region ��������

		/// <summary>
		/// �ϳ�ʵ�塣
		/// </summary>
		/// <param name="record">��¼��</param>
		/// <returns>�ϳɺõ�ʵ�塣</returns>
		public Object Compose(IDataRecord record)
		{
			if (IsStoredProcedure)
			{
				return ComposeFromStoredProc(record);
			}
			else
			{
				Object[] dbValues = new Object[ColumnCount];

				record.GetValues(dbValues);

				return Target.Compose(dbValues);
			}
		}

		/// <summary>
		/// �������� WHERE �Ӿ�Ĺ��������ʽ��ʹ��Ĭ�ϲ�ѯ����ǰ׺��@����
		/// </summary>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <returns>�����õ����� WHERE �Ӿ�Ĺ��������ʽ��</returns>
		public FilterCompilationResult ComposeFilterExpression(Filter whereFilter)
		{
			return ComposeFilterExpression(whereFilter, (String)null);
		}

		/// <summary>
		/// �������� WHERE �Ӿ�Ĺ��������ʽ����ָ����ѯ����ǰ׺��
		/// </summary>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="parameterPrefix">��ѯ����ǰ׺��</param>
		/// <returns>�����õ����� WHERE �Ӿ�Ĺ��������ʽ��</returns>
		public FilterCompilationResult ComposeFilterExpression(Filter whereFilter, String parameterPrefix)
		{
			EntitySchemaFilterCompilationContext context = new EntitySchemaFilterCompilationContext(Target, parameterPrefix);
			FilterCompilationResult result = Filter.Compile(context, whereFilter);

			return result;
		}

		/// <summary>
		/// ��ȡ���е�ʵ��ܹ�����������ȷ�ʽ��ȡ��
		/// </summary>
		/// <returns>ʵ��ܹ��б�</returns>
		public EntitySchema[] GetAllSchemas()
		{
			List<EntitySchema> allSchemas = new List<EntitySchema>();

			for (Int32 level = 0; level < Rank; level++)
			{
				allSchemas.AddRange(this[level]);
			}

			return allSchemas.ToArray();
		}

		/// <summary>
		/// ��ȡ���е�ѡ�����ԡ�
		/// </summary>
		/// <returns>���е�ѡ�����Լ��ϡ�</returns>
		public EntityProperty[] GetAllSelectProperties()
		{
			List<EntityProperty> allProperties = new List<EntityProperty>();

			foreach (EntityProperty[] properties in m_allProperties.Values)
			{
				allProperties.AddRange(properties);
			}

			return allProperties.ToArray();
		}

		/// <summary>
		/// ��ȡѡ�����ԡ�
		/// </summary>
		/// <param name="schema">������ʵ��ܹ���</param>
		/// <returns>�����б�</returns>
		public EntityProperty[] GetSelectProperties(EntitySchema schema)
		{
			return m_allProperties[schema];
		}

		/// <summary>
		/// ��ȡѡ���С�
		/// </summary>
		/// <param name="schema">������ʵ��ܹ���</param>
		/// <returns>�м��ϡ�</returns>
		public Column[] GetSelectColumns(EntitySchema schema)
		{
			return m_allColumns[schema];
		}

		/// <summary>
		/// ����ѡ���е��ֶ�����ƫ�ơ�
		/// </summary>
		/// <param name="fieldIndexOffset">�ֶ�����ƫ�ơ�</param>
		public void SetFieldIndexOffset(Int32 fieldIndexOffset)
		{
			foreach (Column col in Columns)
			{
				col.FieldIndexOffset = fieldIndexOffset;
			}
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ʼ����
		/// </summary>
		internal void Initialize()
		{
			Dictionary<Int32, List<EntitySchema>> allSchemas = new Dictionary<Int32, List<EntitySchema>>();

			RetrieveEntityShemas(m_target, allSchemas, 0);

			m_rank = allSchemas.Count;

			m_allSchemas = new EntitySchema[m_rank][];

			List<Column> columns = new List<Column>();

			for (Int32 level = 0; level < m_rank; level++)
			{
				List<EntitySchema> schemas = allSchemas[level];
				m_allSchemas[level] = schemas.ToArray();

				foreach (EntitySchema schema in schemas)
				{
					m_schemaLookups.Add(schema.PropertyPath, schema);
				}

				SelectColumns(schemas, columns);
			}

			m_columns = columns.ToArray();

			foreach (Column col in m_columns)
			{
				col.Selected = true;
			}

			GenerateIndex();
		}

		#endregion

		#region ��������

		/// <summary>
		/// �ϳ�ʵ�壬��¼Ϊִ�д洢���̵õ���
		/// </summary>
		/// <param name="record">��¼��</param>
		/// <returns>�ϳɵ�ʵ�塣</returns>
		private Object ComposeFromStoredProc(IDataRecord record)
		{
			Object entity = EtyBusinessObject.CreateCompletely(Target.Type);

			foreach (EntityProperty property in Target.Properties)
			{
				Object[] values;

				if (!property.HasComproundColumns)
				{
					Column col = property.Columns[0];
					Object firstValue = DbConverter.ConvertFrom(record[col.Name], col);
					values = new Object[] { firstValue };
				}
				else
				{
					values = new Object[property.Columns.Count];

					for (Int32 i = 0; i < values.Length; i++)
					{
						Column col = Target.Columns[i];
						values[i] = DbConverter.ConvertFrom(record[col.Name], col);
					}
				}

				property.Definition.SetValue(entity, values);
			}

			return entity;
		}

		/// <summary>
		/// ����������
		/// </summary>
		private void GenerateIndex()
		{
			EntitySchema[] allSchemas = GetAllSchemas();

			for (Int32 i = 0; i < allSchemas.Length; i++)
			{
				allSchemas[i].Index = i;
			}

			for (Int32 i = 0; i < Columns.Length; i++)
			{
				Columns[i].Index = i;
			}
		}

		/// <summary>
		/// ������ȡ��������ʵ��ܹ���
		/// </summary>
		/// <param name="targetschema">Ŀ��ܹ���</param>
		/// <param name="allSchemas">�ռ��ֵ䡣</param>
		/// <param name="level">����</param>
		private static void RetrieveEntityShemas(EntitySchema targetschema, Dictionary<Int32, List<EntitySchema>> allSchemas, Int32 level)
		{
			if (!allSchemas.ContainsKey(level))
			{
				allSchemas.Add(level, new List<EntitySchema>());
			}

			allSchemas[level].Add(targetschema);

			if (targetschema.HasRightRelations)
			{
				foreach (EntitySchemaRelation relation in targetschema.RightRelations)
				{
					RetrieveEntityShemas(relation.ParentSchema, allSchemas, (level + 1));
				}
			}
		}

		/// <summary>
		/// ѡ���С�
		/// </summary>
		/// <param name="allSchemas">ʵ��ܹ��б�</param>
		/// <param name="allColumns">��������</param>
		private void SelectColumns(List<EntitySchema> allSchemas, List<Column> allColumns)
		{
			List<EntityProperty> properties = new List<EntityProperty>();
			List<Column> columns = new List<Column>();

			foreach (EntitySchema schema in allSchemas)
			{
				if (BuilderStrategy.SelectNothingFrom(schema))
				{
					schema.SelectNothing = true;
					continue;
				}

				// ��ȡѡ�����Ժ�ѡ����
				foreach (EntityProperty property in schema.Properties)
				{
					if ((property.IsPrimaryKey && BuilderStrategy.AlwaysSelectPrimaryKeyProperties)
						|| BuilderStrategy.SelectAllProperties
						|| BuilderStrategy.SelectProperty(property))
					{
						// ��ѡ���з����ռ�����
						allColumns.AddRange(property.Columns);

						// �ռ���ǰ�ܹ���ѡ�����Ժ�ѡ����
						properties.Add(property);
						columns.AddRange(property.Columns);
					}
				}

				// �������Ժ����ֵ�
				m_allProperties.Add(schema, properties.ToArray());
				m_allColumns.Add(schema, columns.ToArray());

				properties.Clear();
				columns.Clear();
			}
		}

		#endregion

		#region IDebugInfoProvider ��Ա

		/// <summary>
		/// ��ȡʵ��ܹ���ϵ���ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>ʵ��ܹ���ϵ���ϸ��Ϣ��</returns>
		public String Dump()
		{
			return DbEntityDebugger.Dump(this);
		}

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="indent"></param>
		/// <returns></returns>
		public String Dump(String indent)
		{
			return DbEntityDebugger.IndentText(Dump());
		}

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), level);
		}

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="indent"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(String indent, Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), indent, level);
		}

		#endregion
	}
}