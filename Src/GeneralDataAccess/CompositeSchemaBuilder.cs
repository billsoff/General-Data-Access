#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeSchemaBuilder.cs
// 文件功能描述：复合实体架构生成器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110707
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 复合实体架构生成器。
	/// </summary>
	public sealed class CompositeSchemaBuilder : IColumnLocatable
	{
		#region 私有字段

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

		#region 构造函数

		/// <summary>
		/// 构造函数，设置复合实体类型。
		/// </summary>
		/// <param name="type">复合实体类型。</param>
		/// <param name="parameterPrefix">查询参数前缀。</param>
		public CompositeSchemaBuilder(Type type, String parameterPrefix)
		{
			m_definition = CompositeDefinitionBuilder.Build(type);
			m_parameterPrefix = parameterPrefix;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取参数前缀。
		/// </summary>
		public String ParameterPrefix
		{
			get { return m_parameterPrefix; }
		}

		/// <summary>
		/// 获取或设置加载策略。
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
		/// 获取或设置复合实体过滤器列表。
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
		/// 获取或设置 WHERE 过滤器。
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
		/// 获取或设置排序器。
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

		#region 私有属性

		/// <summary>
		/// 获取所有的针对复合实体属性的列定位符。
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
		/// 获取所有的复合实体属性（包括根属性）的架构，以用于定位列。
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

		#region 公共方法

		/// <summary>
		/// 生成复合实体架构。
		/// </summary>
		/// <returns>复合实体架构。</returns>
		public CompositeSchema Build()
		{
			#region 性能计数

			Timing.Start("生成复合实体架构", "CompositeSchemaBuilder.Build {8317FA88-E185-4a38-B2F2-D4363C34352C}");

			#endregion

			CompositeSchema compositeSchema = new CompositeSchema(m_definition);

			BuildItemColumnLocators();

			List<QueryParameter> whereParameters = new List<QueryParameter>();
			List<QueryParameter> havingParameters = new List<QueryParameter>();

			Int32 schemaIndex = 0;
			Int32 fieldIndexOffset = 0;

			// 构建根实体架构组合
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

				// 扩展连接属性和 WHERE 过滤器和排序器属性
				List<IPropertyChain> extendingChains = new List<IPropertyChain>();

				extendingChains.AddRange(propertyDef.MappedPropertyChains);
				extendingChains.AddRange(GetAllItemProperties(propertyDef));

				builder.ExtendProperties(extendingChains.ToArray(), false);

				builder.BuildInfrastructure();

				whereParameters.AddRange(builder.ComposeWhereFilterExpression(whereParameters.Count));
				havingParameters.AddRange(builder.ComposeHavingFilterExpression(havingParameters.Count));

				// 在创建过滤器表达式前生成架构
				ExpressionSchema schema = builder.Build(fieldIndexOffset);

				schema.Index = schemaIndex++;

				fieldIndexOffset += schema.Columns.Length;

				ItemSchemas.Add(propertyDef.Name, schema);
				compositeSchema.ForeignReferences.Add(propertyDef.Name, schema);
			}

			// 创建 WHERE 过滤器表达式和排序表达式
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

			#region 性能计数

			Timing.Stop("CompositeSchemaBuilder.Build {8317FA88-E185-4a38-B2F2-D4363C34352C}");

			#endregion

			return compositeSchema;
		}

		#endregion

		#region IColumnLoactable 成员

		/// <summary>
		/// 获取列定位符所定位的列集合。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>列集合。</returns>
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

		#region 辅助方法

		/// <summary>
		/// 生成根实体架构组合。
		/// </summary>
		/// <param name="compositeDefinition">实体定义。</param>
		/// <returns>根实体架构组合。</returns>
		private EntitySchemaComposite BuildRootComposite(CompositeDefinition compositeDefinition)
		{
			CompositeRootPropertyDefinition root = compositeDefinition.Root;
			CompositeBuilderStrategy strategy = CreateRootBuilderStrategy(root);
			EntitySchemaComposite composite = EntitySchemaCompositeFactory.Create(root.Type, strategy);

			return composite;
		}

		/// <summary>
		/// 生成过滤器表达式。
		/// </summary>
		/// <returns>生成结果。</returns>
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
		/// 生成复合实体属性项列定位集合（由过滤器和排序器而来）。
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
		/// 创建 SQL 指令。
		/// </summary>
		/// <param name="schema">复合实体架构。</param>
		/// <returns>创建好的 SQL 指令。</returns>
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
		///  创建表连接表达式。
		/// </summary>
		/// <param name="schema">复合实体架构。</param>
		/// <returns>创建好的表连接表达式。</returns>
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
		///  创建选择列表。
		/// </summary>
		/// <param name="schema">复合实体架构。</param>
		/// <returns>创建好的选择列表。</returns>
		private String ComposeSelectList(CompositeSchema schema)
		{
			StringBuilder buffer = new StringBuilder();

			// 构造选择列表
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
		/// 将指向复合实体的列定位符转换为属性定位符。
		/// </summary>
		/// <param name="allColLocator">所有的列定位符。</param>
		/// <returns>指向复合实体的属性的列定位符。</returns>
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
		/// 将针对复合实体的列定位符转换为针对其中属性的列定位符。
		/// </summary>
		/// <param name="colLocator">针对复合实体的列定位符。</param>
		/// <returns>转换后针对实体中属性的列定位符。</returns>
		private ColumnLocator ConvertToItemColumnLocator(ColumnLocator colLocator)
		{
			#region 前置断言

			Debug.Assert(
					colLocator.PropertyPath.Length >= 2,
					String.Format(
							"列定位符 {0} 的长度应至少为 2（应指向复合实体中某个项的属性）。",
							String.Join(".", colLocator.PropertyPath)
						)
				);
			Debug.Assert(
					colLocator.PropertyPath[0].Equals(m_definition.Root.Name, CommonPolicies.PropertyNameComparison)
					|| m_definition.ForeignReferences.Contains(colLocator.PropertyPath[0]),
					String.Format("列定位符 {0} 无法定位到复合实体的属性。", String.Join(".", colLocator.PropertyPath))
				);

			#endregion

			String[] itemPropertyPath = new String[colLocator.PropertyPath.Length - 1];
			Array.Copy(colLocator.PropertyPath, 1, itemPropertyPath, 0, itemPropertyPath.Length);

			return new ColumnLocator(itemPropertyPath);
		}

		/// <summary>
		/// 创建根生成策略。
		/// </summary>
		/// <param name="propertyDef">根属性定义。</param>
		/// <returns>生成策略。</returns>
		private CompositeBuilderStrategy CreateRootBuilderStrategy(CompositeRootPropertyDefinition propertyDef)
		{
			// 获取所有的映射属性
			IPropertyChain[] allMappingProperties = GetAllMappingProperties();

			// 获取所有的加载属性
			IPropertyChain[] allLoadingRootSchemas = GetAllRootLoadingSchemas();

			List<PropertySelector> allSelectors = new List<PropertySelector>();

			// 选择所有的映射属性
			allSelectors.AddRange(
					Array.ConvertAll<IPropertyChain, PropertySelector>(
							allMappingProperties,
							delegate(IPropertyChain chain)
							{
								return PropertySelector.Create(PropertySelectMode.Property, chain);
							}
						)
				);

			// 加载实体架构
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
		/// 获取指定的复合实体属性中要选择的属性（由过滤器和排序器决定）。
		/// </summary>
		/// <param name="propertyDef">复合实体属性。</param>
		/// <returns>属性列表。</returns>
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
		/// 获取所有的（根属性实体中的）映射属性。
		/// </summary>
		/// <returns>映射属性列表。</returns>
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
		/// 获取所有的要属性要加载的实体架构。
		/// </summary>
		/// <returns>所有要加载的实体架构。</returns>
		private IPropertyChain[] GetAllRootLoadingSchemas()
		{
			List<IPropertyChain> schemas = new List<IPropertyChain>();

			if ((m_itemLocators != null) && (m_itemLocators.Count != 0))
			{
				ColumnLocator[] rootColLocators = m_itemLocators[m_definition.Root.Name];

				foreach (ColumnLocator colLocator in rootColLocators)
				{
					// 因第 0 级实体总量加载，所以只考虑深度大于 1 的实体
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
		/// 获取项属性映射列集合。
		/// </summary>
		/// <param name="schema">复合实体架构。</param>
		/// <param name="propertyDef">复合实体属性定义。</param>
		/// <returns>列集合。</returns>
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
		/// 获取根属性映射列集合。
		/// </summary>
		/// <param name="schema">复合实体架构。</param>
		/// <param name="propertyDef">复合实体属性定义。</param>
		/// <returns>列集合。</returns>
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