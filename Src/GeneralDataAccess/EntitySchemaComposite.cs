#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntitySchemaComposite.cs
// 文件功能描述：数据架构组合，表示一个完整的表连接。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110413
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 数据架构组合，表示一个完整的表连接。
	/// </summary>
	public sealed class EntitySchemaComposite : IDebugInfoProvider
	{
		#region 私有字段

		private readonly CompositeBuilderStrategy m_builderStrategy;
		private readonly EntitySchema m_target;
		private EntitySchema[][] m_allSchemas;
		private readonly Dictionary<String, EntitySchema> m_schemaLookups = new Dictionary<String, EntitySchema>();

		private Column[] m_columns;

		private readonly Dictionary<EntitySchema, EntityProperty[]> m_allProperties = new Dictionary<EntitySchema, EntityProperty[]>();
		private readonly Dictionary<EntitySchema, Column[]> m_allColumns = new Dictionary<EntitySchema, Column[]>();

		private Int32 m_rank;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体架构。
		/// </summary>
		/// <param name="buildStrategy">生成策略。</param>
		/// <param name="target">实体架构。</param>
		public EntitySchemaComposite(CompositeBuilderStrategy buildStrategy, EntitySchema target)
		{
			m_builderStrategy = buildStrategy;
			m_target = target;
			m_target.Composite = this;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取指定级别的实体架构列表，级别基于零，最大级别为 Rank - 1。
		/// </summary>
		/// <param name="level">级别。</param>
		/// <returns>指定级别的实体架构列表。</returns>
		public EntitySchema[] this[Int32 level]
		{
			get { return m_allSchemas[level]; }
		}

		/// <summary>
		/// 获取由属性链定位的实体架构。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>由该属性链定位的实体架构，如果未找到，则返回 null。</returns>
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
		/// 获取由属性路径定位的实体架构。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>由该属性路径定位的实体架构，如果未找到，则返回 null。</returns>
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
		/// 获取生成策略。
		/// </summary>
		public CompositeBuilderStrategy BuilderStrategy
		{
			get { return m_builderStrategy; }
		}

		/// <summary>
		/// 获取总常规加载列数。
		/// </summary>
		public Int32 ColumnCount
		{
			get { return m_columns.Length; }
		}

		/// <summary>
		/// 选择列集合。
		/// </summary>
		public Column[] Columns
		{
			get { return m_columns; }
		}

		/// <summary>
		/// 获取一个值，判断是否为存储过程。
		/// </summary>
		public Boolean IsStoredProcedure
		{
			get { return Target.IsStoredProcedure; }
		}

		/// <summary>
		/// 获取架构组合的秩，则最大层级，如果只取第一级引用，则其值为 2。
		/// </summary>
		public Int32 Rank
		{
			get { return m_rank; }
		}

		/// <summary>
		/// 目标架构。
		/// </summary>
		public EntitySchema Target
		{
			get { return m_target; }
		}

		/// <summary>
		/// 获取包含的实体架构总数。
		/// </summary>
		public Int32 TotalSchemaCount
		{
			get { return m_allSchemas.Length; }
		}

		#region SQL 指令片断

		private String m_fromList;

		/// <summary>
		/// 获取查询的表连接表达式。
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
		/// 创建查询的表连接表达式。
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

					// Oracle 不支持在表名和表别名之间加 AS 关键字，用空格分隔二者
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
		/// 获取查询的选择列表。
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
		/// 创建查询的选择列表。
		/// </summary>
		/// <returns>选择列表。</returns>
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

		#region 公共方法

		/// <summary>
		/// 合成实体。
		/// </summary>
		/// <param name="record">记录。</param>
		/// <returns>合成好的实体。</returns>
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
		/// 创建用于 WHERE 子句的过滤器表达式，使用默认查询参数前缀“@”。
		/// </summary>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <returns>创建好的用于 WHERE 子句的过滤器表达式。</returns>
		public FilterCompilationResult ComposeFilterExpression(Filter whereFilter)
		{
			return ComposeFilterExpression(whereFilter, (String)null);
		}

		/// <summary>
		/// 创建用于 WHERE 子句的过滤器表达式，并指定查询参数前缀。
		/// </summary>
		/// <param name="whereFilter">WHERE 过滤器。</param>
		/// <param name="parameterPrefix">查询参数前缀。</param>
		/// <returns>创建好的用于 WHERE 子句的过滤器表达式。</returns>
		public FilterCompilationResult ComposeFilterExpression(Filter whereFilter, String parameterPrefix)
		{
			EntitySchemaFilterCompilationContext context = new EntitySchemaFilterCompilationContext(Target, parameterPrefix);
			FilterCompilationResult result = Filter.Compile(context, whereFilter);

			return result;
		}

		/// <summary>
		/// 获取所有的实体架构，按广度优先方式获取。
		/// </summary>
		/// <returns>实体架构列表。</returns>
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
		/// 获取所有的选择属性。
		/// </summary>
		/// <returns>所有的选择属性集合。</returns>
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
		/// 获取选择属性。
		/// </summary>
		/// <param name="schema">所属的实体架构。</param>
		/// <returns>属性列表。</returns>
		public EntityProperty[] GetSelectProperties(EntitySchema schema)
		{
			return m_allProperties[schema];
		}

		/// <summary>
		/// 获取选择列。
		/// </summary>
		/// <param name="schema">所属的实体架构。</param>
		/// <returns>列集合。</returns>
		public Column[] GetSelectColumns(EntitySchema schema)
		{
			return m_allColumns[schema];
		}

		/// <summary>
		/// 设置选择列的字段索引偏移。
		/// </summary>
		/// <param name="fieldIndexOffset">字段索引偏移。</param>
		public void SetFieldIndexOffset(Int32 fieldIndexOffset)
		{
			foreach (Column col in Columns)
			{
				col.FieldIndexOffset = fieldIndexOffset;
			}
		}

		#endregion

		#region 内部方法

		/// <summary>
		/// 初始化。
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

		#region 辅助方法

		/// <summary>
		/// 合成实体，记录为执行存储过程得到。
		/// </summary>
		/// <param name="record">记录。</param>
		/// <returns>合成的实体。</returns>
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
		/// 生成索引。
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
		/// 按级别取出各级的实体架构。
		/// </summary>
		/// <param name="targetschema">目标架构。</param>
		/// <param name="allSchemas">收集字典。</param>
		/// <param name="level">级别。</param>
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
		/// 选择列。
		/// </summary>
		/// <param name="allSchemas">实体架构列表。</param>
		/// <param name="allColumns">列容器。</param>
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

				// 获取选择属性和选择列
				foreach (EntityProperty property in schema.Properties)
				{
					if ((property.IsPrimaryKey && BuilderStrategy.AlwaysSelectPrimaryKeyProperties)
						|| BuilderStrategy.SelectAllProperties
						|| BuilderStrategy.SelectProperty(property))
					{
						// 将选择列放入收集容器
						allColumns.AddRange(property.Columns);

						// 收集当前架构的选择属性和选择列
						properties.Add(property);
						columns.AddRange(property.Columns);
					}
				}

				// 设置属性和列字典
				m_allProperties.Add(schema, properties.ToArray());
				m_allColumns.Add(schema, columns.ToArray());

				properties.Clear();
				columns.Clear();
			}
		}

		#endregion

		#region IDebugInfoProvider 成员

		/// <summary>
		/// 获取实体架构组合的详细信息，用于调试。
		/// </summary>
		/// <returns>实体架构组合的详细信息。</returns>
		public String Dump()
		{
			return DbEntityDebugger.Dump(this);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="indent"></param>
		/// <returns></returns>
		public String Dump(String indent)
		{
			return DbEntityDebugger.IndentText(Dump());
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), level);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
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