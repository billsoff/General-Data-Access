#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeSchemaBuilder.cs
// �ļ���������������ʵ��ܹ���������
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
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����ʵ��ܹ���������
	/// </summary>
	public sealed class CompositeSchemaBuilder : IColumnLocatable
	{
		#region ˽���ֶ�

		private readonly CompositeDefinition m_definition;
		private readonly String m_parameterPrefix;

		private CompositeBuilderStrategy m_loadStrategy;
		private CompositeSettings m_settings;
		private Filter m_where;
		private Sorter m_orderBy;

		private Dictionary<String, ColumnLocator[]> m_itemLocators;
		private Dictionary<String, IColumnLocatable> m_itemSchemas;

		private String m_whereExpression;
		private String m_sortExpression;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø���ʵ�����͡�
		/// </summary>
		/// <param name="type">����ʵ�����͡�</param>
		/// <param name="parameterPrefix">��ѯ����ǰ׺��</param>
		public CompositeSchemaBuilder(Type type, String parameterPrefix)
		{
			m_definition = CompositeDefinitionBuilder.Build(type);
			m_parameterPrefix = parameterPrefix;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ǰ׺��
		/// </summary>
		public String ParameterPrefix
		{
			get { return m_parameterPrefix; }
		}

		/// <summary>
		/// ��ȡ�����ü��ز��ԡ�
		/// </summary>
		public CompositeBuilderStrategy LoadStrategy
		{
			get
			{
				return m_loadStrategy;
			}

			set
			{
				m_loadStrategy = value;
			}
		}

		/// <summary>
		/// ��ȡ�����ø���ʵ��������б�
		/// </summary>
		public CompositeSettings Settings
		{
			get
			{
				return m_settings;
			}

			set
			{
				m_settings = value;
			}
		}

		/// <summary>
		/// ��ȡ������ WHERE ��������
		/// </summary>
		public Filter Where
		{
			get
			{
				return m_where;
			}

			set
			{
				m_where = value;
			}
		}

		/// <summary>
		/// ��ȡ��������������
		/// </summary>
		public Sorter OrderBy
		{
			get
			{
				return m_orderBy;
			}

			set
			{
				m_orderBy = value;
			}
		}

		#endregion

		#region ˽������

		/// <summary>
		/// ��ȡ���е���Ը���ʵ�����Ե��ж�λ����
		/// </summary>
		public Dictionary<String, ColumnLocator[]> ItemLocators
		{
			get
			{
				if (m_itemLocators == null)
				{
					m_itemLocators = new Dictionary<String, ColumnLocator[]>();
				}

				return m_itemLocators;
			}
		}

		/// <summary>
		/// ��ȡ���еĸ���ʵ�����ԣ����������ԣ��ļܹ��������ڶ�λ�С�
		/// </summary>
		public Dictionary<String, IColumnLocatable> ItemSchemas
		{
			get
			{
				if (m_itemSchemas == null)
				{
					m_itemSchemas = new Dictionary<String, IColumnLocatable>();
				}

				return m_itemSchemas;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ���ɸ���ʵ��ܹ���
		/// </summary>
		/// <returns>����ʵ��ܹ���</returns>
		public CompositeSchema Build()
		{
			#region ���ܼ���

			Timing.Start("���ɸ���ʵ��ܹ�", "CompositeSchemaBuilder.Build {8317FA88-E185-4a38-B2F2-D4363C34352C}");

			#endregion

			CompositeSchema compositeSchema = new CompositeSchema(m_definition);

			BuildItemColumnLocators();

			List<QueryParameter> whereParameters = new List<QueryParameter>();
			List<QueryParameter> havingParameters = new List<QueryParameter>();

			Int32 schemaIndex = 0;
			Int32 fieldIndexOffset = 0;

			// ������ʵ��ܹ����
			EntitySchemaComposite rootComposite = BuildRootComposite(m_definition);
			ItemSchemas.Add(m_definition.Root.Name, rootComposite.Target);
			compositeSchema.Root = rootComposite;

			schemaIndex += rootComposite.TotalSchemaCount;
			fieldIndexOffset += rootComposite.Columns.Length;

			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				ExpressionSchemaBuilder builder = ExpressionSchemaBuilderFactory.Create(propertyDef, m_parameterPrefix);

				if (m_settings != null)
				{
					builder.ItemSettings = m_settings[propertyDef.Name];
				}

				// ��չ�������Ժ� WHERE ������������������
				List<IPropertyChain> extendingChains = new List<IPropertyChain>();

				extendingChains.AddRange(propertyDef.MappedPropertyChains);
				extendingChains.AddRange(GetAllItemProperties(propertyDef));

				builder.ExtendProperties(extendingChains.ToArray(), false);

				builder.BuildInfrastructure();

				whereParameters.AddRange(builder.ComposeWhereFilterExpression(whereParameters.Count));
				havingParameters.AddRange(builder.ComposeHavingFilterExpression(havingParameters.Count));

				// �ڴ������������ʽǰ���ɼܹ�
				ExpressionSchema schema = builder.Build(fieldIndexOffset);

				schema.Index = schemaIndex++;

				fieldIndexOffset += schema.Columns.Length;

				ItemSchemas.Add(propertyDef.Name, schema);
				compositeSchema.ForeignReferences.Add(propertyDef.Name, schema);
			}

			// ���� WHERE ���������ʽ��������ʽ
			FilterCompilationResult result = ComposeFilterExpression(whereParameters.Count);

			if (result != null)
			{
				m_whereExpression = result.WhereClause;
				whereParameters.AddRange(result.Parameters);
			}

			m_sortExpression = Sorter.ComposeSortExpression(this, m_orderBy);

			QueryParameter[] allParameters = new QueryParameter[whereParameters.Count + havingParameters.Count];

			whereParameters.CopyTo(allParameters);
			havingParameters.CopyTo(allParameters, whereParameters.Count);

			compositeSchema.SqlExpression = ComposeSqlExpression(compositeSchema);
			compositeSchema.Parameters = allParameters;

			#region ���ܼ���

			Timing.Stop("CompositeSchemaBuilder.Build {8317FA88-E185-4a38-B2F2-D4363C34352C}");

			#endregion

			return compositeSchema;
		}

		#endregion

		#region IColumnLoactable ��Ա

		/// <summary>
		/// ��ȡ�ж�λ������λ���м��ϡ�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>�м��ϡ�</returns>
		public Column[] this[ColumnLocator colLocator]
		{
			get
			{
				String name = colLocator.PropertyPath[0];
				ColumnLocator itemColLocator = ConvertToItemColumnLocator(colLocator);
				IColumnLocatable schema = ItemSchemas[name];

				return schema[itemColLocator];
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ���ɸ�ʵ��ܹ���ϡ�
		/// </summary>
		/// <param name="compositeDefinition">ʵ�嶨�塣</param>
		/// <returns>��ʵ��ܹ���ϡ�</returns>
		private EntitySchemaComposite BuildRootComposite(CompositeDefinition compositeDefinition)
		{
			CompositeRootPropertyDefinition root = compositeDefinition.Root;
			CompositeBuilderStrategy strategy = CreateRootBuilderStrategy(root);
			EntitySchemaComposite composite = EntitySchemaCompositeFactory.Create(root.Type, strategy);

			return composite;
		}

		/// <summary>
		/// ���ɹ��������ʽ��
		/// </summary>
		/// <returns>���ɽ����</returns>
		private FilterCompilationResult ComposeFilterExpression(Int32 indexOffset)
		{
			if (m_where == null)
			{
				return null;
			}

			GeneralFilterCompilationContext context = new GeneralFilterCompilationContext(this, m_parameterPrefix);
			context.IndexOffset = indexOffset;

			FilterCompilationResult result = Filter.Compile(context, m_where);

			return result;
		}

		/// <summary>
		/// ���ɸ���ʵ���������ж�λ���ϣ��ɹ���������������������
		/// </summary>
		private void BuildItemColumnLocators()
		{
			List<ColumnLocator> allColumnLocators = new List<ColumnLocator>();

			if (m_where != null)
			{
				allColumnLocators.AddRange(m_where.GetAllColumnLocators());
			}

			if (m_orderBy != null)
			{
				allColumnLocators.AddRange(m_orderBy.GetAllColumnLocators());
			}

			if (allColumnLocators.Count != 0)
			{
				ItemLocators.Clear();

				List<String> allItems = new List<String>();

				allItems.Add(m_definition.Root.Name);

				foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
				{
					allItems.Add(propertyDef.Name);
				}

				foreach (String name in allItems)
				{
					List<ColumnLocator> colLoactors = allColumnLocators.FindAll(
							delegate(ColumnLocator item)
							{
								return item.PropertyPath[0].Equals(name, CommonPolicies.PropertyNameComparison);
							}
						);

					ItemLocators[name] = ConvertToItemColumnLocator(colLoactors.ToArray());
				}
			}
		}

		/// <summary>
		/// ���� SQL ָ�
		/// </summary>
		/// <param name="schema">����ʵ��ܹ���</param>
		/// <returns>�����õ� SQL ָ�</returns>
		private String ComposeSqlExpression(CompositeSchema schema)
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			sqlBuilder.Select = ComposeSelectList(schema);
			sqlBuilder.From = ComposeFromList(schema);
			sqlBuilder.Where = m_whereExpression;
			sqlBuilder.OrderBy = m_sortExpression;

			return sqlBuilder.Build();
		}

		/// <summary>
		///  ���������ӱ��ʽ��
		/// </summary>
		/// <param name="schema">����ʵ��ܹ���</param>
		/// <returns>�����õı����ӱ��ʽ��</returns>
		private String ComposeFromList(CompositeSchema schema)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(schema.Root.FromList);

			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				ExpressionSchema expression = schema.ForeignReferences[propertyDef.Name];

				buffer.Append(" ");

				switch (propertyDef.JoinMode)
				{
					case PropertyJoinMode.Left:
						buffer.Append("LEFT JOIN");
						break;

					case PropertyJoinMode.Right:
						buffer.Append("RIGHT JOIN");
						break;

					case PropertyJoinMode.Inner:
					default:
						buffer.Append("INNER JOIN");
						break;
				}

				buffer.Append(" ");

				buffer.AppendFormat("({0}) {1}", expression.SqlExpression, expression.Name);

				buffer.Append(" ON ");

				Column[] rootMappings = GetMappingRootColumns(schema, propertyDef);
				Column[] itemMappeds = GetMappedItemColumns(schema, propertyDef);

				for (Int32 i = 0; i < rootMappings.Length; i++)
				{
					if (i != 0)
					{
						buffer.Append(" AND ");
					}

					Column left = rootMappings[i];
					Column right = itemMappeds[i];

					buffer.AppendFormat("{0}={1}", left.FullName, right.FullName);
				}
			}

			return buffer.ToString();
		}

		/// <summary>
		///  ����ѡ���б�
		/// </summary>
		/// <param name="schema">����ʵ��ܹ���</param>
		/// <returns>�����õ�ѡ���б�</returns>
		private String ComposeSelectList(CompositeSchema schema)
		{
			StringBuilder buffer = new StringBuilder();

			// ����ѡ���б�
			buffer.Append(schema.Root.SelectList);

			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				ExpressionSchema expression = schema.ForeignReferences[propertyDef.Name];

				foreach (Column col in expression.Columns)
				{
					if (buffer.Length != 0)
					{
						buffer.Append(",");
					}

					buffer.AppendFormat("{0} {1}", col.FullName, col.Alias);
				}
			}

			return buffer.ToString();
		}

		/// <summary>
		/// ��ָ�򸴺�ʵ����ж�λ��ת��Ϊ���Զ�λ����
		/// </summary>
		/// <param name="allColLocator">���е��ж�λ����</param>
		/// <returns>ָ�򸴺�ʵ������Ե��ж�λ����</returns>
		private ColumnLocator[] ConvertToItemColumnLocator(ColumnLocator[] allColLocator)
		{
			ColumnLocator[] allItemColLocators = new ColumnLocator[allColLocator.Length];

			for (Int32 i = 0; i < allItemColLocators.Length; i++)
			{
				ColumnLocator colLocator = allColLocator[i];
				allItemColLocators[i] = ConvertToItemColumnLocator(colLocator);
			}

			return allItemColLocators;
		}

		/// <summary>
		/// ����Ը���ʵ����ж�λ��ת��Ϊ����������Ե��ж�λ����
		/// </summary>
		/// <param name="colLocator">��Ը���ʵ����ж�λ����</param>
		/// <returns>ת�������ʵ�������Ե��ж�λ����</returns>
		private ColumnLocator ConvertToItemColumnLocator(ColumnLocator colLocator)
		{
			#region ǰ�ö���

			Debug.Assert(
					colLocator.PropertyPath.Length >= 2,
					String.Format(
							"�ж�λ�� {0} �ĳ���Ӧ����Ϊ 2��Ӧָ�򸴺�ʵ����ĳ��������ԣ���",
							String.Join(".", colLocator.PropertyPath)
						)
				);
			Debug.Assert(
					colLocator.PropertyPath[0].Equals(m_definition.Root.Name, CommonPolicies.PropertyNameComparison)
					|| m_definition.ForeignReferences.Contains(colLocator.PropertyPath[0]),
					String.Format("�ж�λ�� {0} �޷���λ������ʵ������ԡ�", String.Join(".", colLocator.PropertyPath))
				);

			#endregion

			String[] itemPropertyPath = new String[colLocator.PropertyPath.Length - 1];
			Array.Copy(colLocator.PropertyPath, 1, itemPropertyPath, 0, itemPropertyPath.Length);

			return new ColumnLocator(itemPropertyPath);
		}

		/// <summary>
		/// ���������ɲ��ԡ�
		/// </summary>
		/// <param name="propertyDef">�����Զ��塣</param>
		/// <returns>���ɲ��ԡ�</returns>
		private CompositeBuilderStrategy CreateRootBuilderStrategy(CompositeRootPropertyDefinition propertyDef)
		{
			// ��ȡ���е�ӳ������
			IPropertyChain[] allMappingProperties = GetAllMappingProperties();

			// ��ȡ���еļ�������
			IPropertyChain[] allLoadingRootSchemas = GetAllRootLoadingSchemas();

			List<PropertySelector> allSelectors = new List<PropertySelector>();

			// ѡ�����е�ӳ������
			allSelectors.AddRange(
					Array.ConvertAll<IPropertyChain, PropertySelector>(
							allMappingProperties,
							delegate(IPropertyChain chain)
							{
								return PropertySelector.Create(PropertySelectMode.Property, chain);
							}
						)
				);

			// ����ʵ��ܹ�
			allSelectors.AddRange(
					Array.ConvertAll<IPropertyChain, PropertySelector>(
							allLoadingRootSchemas,
							delegate(IPropertyChain chain)
							{
								return PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain);
							}
						)
				);

			EntityDefinition definition = EntityDefinitionBuilder.Build(propertyDef.Type);

			CompositeBuilderStrategy loadStrategy = m_loadStrategy;

			if (loadStrategy == null)
			{
				LoadStrategyAttribute loadStrategyAttr = propertyDef.LoadStrategy ?? definition.LoadStrategy;
				loadStrategy = loadStrategyAttr.Create();
			}

			CompositeBuilderStrategy strategy = CompositeBuilderStrategyFactory.Compose(
					propertyDef.Type,
					loadStrategy,
					allSelectors
				);

			return strategy;
		}

		/// <summary>
		/// ��ȡָ���ĸ���ʵ��������Ҫѡ������ԣ��ɹ���������������������
		/// </summary>
		/// <param name="propertyDef">����ʵ�����ԡ�</param>
		/// <returns>�����б�</returns>
		private IPropertyChain[] GetAllItemProperties(CompositePropertyDefinition propertyDef)
		{
			List<IPropertyChain> allProperties = new List<IPropertyChain>();

			if ((m_itemLocators != null) && (m_itemLocators.Count != 0))
			{
				ColumnLocator[] allItemColumnLocators = m_itemLocators[propertyDef.Name];

				foreach (ColumnLocator colLocator in allItemColumnLocators)
				{
					IPropertyChain chain = new PropertyChain(propertyDef.Type, colLocator.PropertyPath);

					if (!allProperties.Contains(chain))
					{
						allProperties.Add(chain);
					}
				}
			}

			return allProperties.ToArray();
		}

		/// <summary>
		/// ��ȡ���еģ�������ʵ���еģ�ӳ�����ԡ�
		/// </summary>
		/// <returns>ӳ�������б�</returns>
		private IPropertyChain[] GetAllMappingProperties()
		{
			List<IPropertyChain> allMappingProperties = new List<IPropertyChain>();

			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				foreach (IPropertyChain chain in propertyDef.RootPropertyChains)
				{
					if (!allMappingProperties.Contains(chain))
					{
						allMappingProperties.Add(chain);
					}
				}
			}

			return allMappingProperties.ToArray();
		}

		/// <summary>
		/// ��ȡ���е�Ҫ����Ҫ���ص�ʵ��ܹ���
		/// </summary>
		/// <returns>����Ҫ���ص�ʵ��ܹ���</returns>
		private IPropertyChain[] GetAllRootLoadingSchemas()
		{
			List<IPropertyChain> schemas = new List<IPropertyChain>();

			if ((m_itemLocators != null) && (m_itemLocators.Count != 0))
			{
				ColumnLocator[] rootColLocators = m_itemLocators[m_definition.Root.Name];

				foreach (ColumnLocator colLocator in rootColLocators)
				{
					// ��� 0 ��ʵ���������أ�����ֻ������ȴ��� 1 ��ʵ��
					if (colLocator.PropertyPath.Length > 1)
					{
						IPropertyChain chain = new PropertyChain(m_definition.Root.Type, colLocator.PropertyPath);

						if (!schemas.Contains(chain.Previous))
						{
							schemas.Add(chain.Previous.Copy());
						}
					}
				}
			}

			return schemas.ToArray();
		}

		/// <summary>
		/// ��ȡ������ӳ���м��ϡ�
		/// </summary>
		/// <param name="schema">����ʵ��ܹ���</param>
		/// <param name="propertyDef">����ʵ�����Զ��塣</param>
		/// <returns>�м��ϡ�</returns>
		private Column[] GetMappedItemColumns(CompositeSchema schema, CompositeForeignReferencePropertyDefinition propertyDef)
		{
			List<Column> allMappedColumns = new List<Column>();
			ExpressionSchema expr = schema.ForeignReferences[propertyDef.Name];

			foreach (IPropertyChain chain in propertyDef.MappedPropertyChains)
			{
				ColumnLocator colLocator = new ColumnLocator(chain.PropertyPath);
				allMappedColumns.AddRange(expr[colLocator]);
			}

			return allMappedColumns.ToArray();
		}

		/// <summary>
		/// ��ȡ������ӳ���м��ϡ�
		/// </summary>
		/// <param name="schema">����ʵ��ܹ���</param>
		/// <param name="propertyDef">����ʵ�����Զ��塣</param>
		/// <returns>�м��ϡ�</returns>
		private Column[] GetMappingRootColumns(CompositeSchema schema, CompositeForeignReferencePropertyDefinition propertyDef)
		{
			List<Column> allMappingColumns = new List<Column>();

			foreach (IPropertyChain chain in propertyDef.RootPropertyChains)
			{
				ColumnLocator colLocator = new ColumnLocator(chain.PropertyPath);
				allMappingColumns.AddRange(schema.Root.Target[colLocator]);
			}

			return allMappingColumns.ToArray();
		}

		#endregion
	}
}