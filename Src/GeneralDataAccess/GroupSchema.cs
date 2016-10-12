#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupSchema.cs
// 文件功能描述：分组结果实体架构。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110629
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
	/// 分组结果实体架构。
	/// </summary>
	public sealed class GroupSchema : IColumnLocatable, IDebugInfoProvider
	{
		#region 私有字段

		private GroupDefinition m_definition;

		private Column[] m_selectColumns;
		private Column[] m_groupColumns;

		private Column[] m_primitiveColumns;
		private GroupForeignReference[] m_foreignReferences;
		private EntitySchemaComposite m_composite;

		private String m_selectList;
		private String m_groupList;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		internal GroupSchema()
		{
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 根据列定位符查找匹配的列集合。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>找到的列集合。</returns>
		public Column[] this[ColumnLocator colLocator]
		{
			get
			{
				// 先定位于分组结果实体属性
				String propertyName = colLocator.PropertyPath[0];
				GroupPropertyDefinition propertyDef = m_definition.Properties[propertyName];

				#region 前置条件

				Debug.Assert(
						propertyDef != null,
						String.Format("类型 {0} 中不存在聚合属性 {1}，请检查分组标记。", m_definition.Type.FullName, propertyName)
					);

				#endregion

				if (propertyDef.IsPrimitive)
				{
					Column primitiveColumn = FindPrimitiveColumn(propertyDef.Name);

					if (primitiveColumn != null)
					{
						return new Column[] { primitiveColumn };
					}
				}
				else
				{
					EntitySchema schema = FindSchema(propertyDef.Name);

					if (schema != null)
					{
						if (colLocator.PropertyPath.Length == 1)
						{
							Column[] primaryKeyColumns = new Column[schema.PrimaryKeyColumns.Count];
							schema.PrimaryKeyColumns.CopyTo(primaryKeyColumns, 0);

							return primaryKeyColumns;
						}
						else
						{
							String[] childPropertyPath = new String[colLocator.PropertyPath.Length - 1];
							Array.Copy(colLocator.PropertyPath, 1, childPropertyPath, 0, childPropertyPath.Length);
							Column[] allColumns = schema[new ColumnLocator(childPropertyPath)];

							return allColumns;
						}
					}
				}

				Debug.Fail(String.Format("在分组实体 {0} 中定位属性 {1} 失败。", m_definition.Type.FullName, colLocator.FullName));

				return null;
			}
		}

		/// <summary>
		/// 获取要分组的实体架构组合。
		/// </summary>
		public EntitySchemaComposite Composite
		{
			get { return m_composite; }
			internal set { m_composite = value; }
		}

		/// <summary>
		/// 获取表连接表达式。
		/// </summary>
		public String FromList
		{
			get { return Composite.FromList; }
		}

		/// <summary>
		/// 获取分组列集合。
		/// </summary>
		public Column[] GroupColumns
		{
			get { return m_groupColumns; }
			internal set { m_groupColumns = value; }
		}

		/// <summary>
		/// 获取分组列表，如果不存在分组列表，则返回 null。
		/// </summary>
		public String GroupList
		{
			get
			{
				if (m_groupList == null)
				{
					if ((m_groupColumns == null) || (m_groupColumns.Length == 0))
					{
						return null;
					}

					StringBuilder buffer = new StringBuilder();

					foreach (Column col in m_groupColumns)
					{
						if (buffer.Length != 0)
						{
							buffer.Append(",");
						}

						buffer.Append(col.FullName);
					}

					m_groupList = buffer.ToString();
				}

				return m_groupList;
			}
		}

		/// <summary>
		/// 获取基本列集合。
		/// </summary>
		public Column[] PrimitiveColumns
		{
			get { return m_primitiveColumns; }
			internal set { m_primitiveColumns = value; }
		}

		/// <summary>
		/// 获取选择列集合。
		/// </summary>
		public Column[] SelectColumns
		{
			get { return m_selectColumns; }
			internal set { m_selectColumns = value; }
		}

		/// <summary>
		/// 获取选择列表，不包含 SELECT 关键字。
		/// </summary>
		public String SelectList
		{
			get
			{
				if (m_selectList == null)
				{
					#region 前置条件

					Debug.Assert((m_selectColumns != null) && (m_selectColumns.Length != 0), "未设置选择列。");

					#endregion

					StringBuilder buffer = new StringBuilder();

					foreach (Column col in m_selectColumns)
					{
						if (buffer.Length != 0)
						{
							buffer.Append(",");
						}

						buffer.AppendFormat("{0} {1}", col.Expression, col.Alias);
					}

					m_selectList = buffer.ToString();
				}

				return m_selectList;
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建用于 HAVING 子句的过滤器表达式，查询参数的前缀为“@”。
		/// </summary>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <returns>创建好的用于 HAVING 子句的过滤器表达式。</returns>
		public FilterCompilationResult ComposeFilterExpression(Filter havingFilter)
		{
			return ComposeFilterExpression(havingFilter, (String)null);
		}

		/// <summary>
		/// 创建用于 HAVING 子句的过滤器表达式，并设置查询参数前缀。
		/// </summary>
		/// <param name="havingFilter">HAVING 过滤器。</param>
		/// <param name="parameterPrifix">查询参数前缀。</param>
		/// <returns>创建好的用于 HAVING 子句的过滤器表达式。</returns>
		public FilterCompilationResult ComposeFilterExpression(Filter havingFilter, String parameterPrifix)
		{
			GroupSchemaFilterCompilationContext context = new GroupSchemaFilterCompilationContext(this, parameterPrifix);
			FilterCompilationResult result = Filter.Compile(context, havingFilter);

			return result;
		}

		/// <summary>
		/// 合成分组结果。
		/// </summary>
		/// <param name="record">记录。</param>
		/// <returns>合成好的分组结果。</returns>
		public GroupResult Compose(IDataRecord record)
		{
			GroupResult result = (GroupResult)m_definition.Constructor.Invoke((Object[])null);

			Compose(result, record);

			return result;
		}

		/// <summary>
		/// 设置分组结果的属性值。
		/// </summary>
		/// <param name="result">分组结果实体。</param>
		/// <param name="record">记录。</param>
		public void Compose(GroupResult result, IDataRecord record)
		{
			#region 前置条件

			Debug.Assert(result != null, "参数 result 不能为空。");
			Debug.Assert(record != null, "参数 record 不能为空。");

			#endregion

			#region 性能计数

			Timing.Start("构造分组结果", "GroupSchema.Compose {0EC306B0-770C-43dc-B913-7A0D56CDB1FB}");

			#endregion

			Object[] dbValues = new Object[record.FieldCount];
			record.GetValues(dbValues);

			Compose(result, dbValues);

			#region 性能计数

			Timing.Stop("GroupSchema.Compose {0EC306B0-770C-43dc-B913-7A0D56CDB1FB}");

			#endregion
		}

		/// <summary>
		/// 创建分组结果。
		/// </summary>
		/// <param name="dbValues">记录。</param>
		/// <returns>创建好的分组结果。</returns>
		public Object Compose(Object[] dbValues)
		{
			GroupResult result = (GroupResult)m_definition.Constructor.Invoke((Object[])null);

			Compose(result, dbValues);

			return result;
		}

		/// <summary>
		/// 设置字段索引偏移。
		/// </summary>
		/// <param name="fieldIndexOffset">字段索引偏移。</param>
		public void SetFieldIndexOffset(Int32 fieldIndexOffset)
		{
			foreach (Column primitiveColumn in m_primitiveColumns)
			{
				primitiveColumn.FieldIndexOffset = fieldIndexOffset;
			}

			m_composite.SetFieldIndexOffset(fieldIndexOffset);
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取或设置分组结果实体定义。
		/// </summary>
		internal GroupDefinition Definition
		{
			get { return m_definition; }
			set { m_definition = value; }
		}

		/// <summary>
		/// 获取或设置分组结果外部引用集合。
		/// </summary>
		internal GroupForeignReference[] ForeignReferences
		{
			get { return m_foreignReferences; }
			set { m_foreignReferences = value; }
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 设置分组结果的属性值。
		/// </summary>
		/// <param name="result">分组结果实体。</param>
		/// <param name="dbValues">记录。</param>
		private void Compose(GroupResult result, Object[] dbValues)
		{
			#region 前置条件

			Debug.Assert(result != null, "参数 result 不能为空。");
			Debug.Assert(dbValues != null, "参数 dbValues 不能为空。");

			#endregion

			foreach (Column col in m_primitiveColumns)
			{
				result[col.PropertyName] = DbConverter.ConvertFrom(dbValues[col.FieldIndex], col);
			}

			foreach (GroupForeignReference foreignRef in m_foreignReferences)
			{
				result[foreignRef.Property.Name] = foreignRef.Compose(dbValues);
			}
		}

		/// <summary>
		/// 创建所有的聚合、分组的列的集合，这个集合中的列经过了包装，与分组列集合中的列不同。
		/// </summary>
		private Column[] CreateAllColumns()
		{
			List<Column> allColumns = new List<Column>();
			allColumns.AddRange(PrimitiveColumns);
			Column[] primitiveRootColumns = Array.ConvertAll<Column, Column>(
					PrimitiveColumns,
					delegate(Column col)
					{
						return WrappedColumn.GetRootColumn(col);
					}
				);

			// 分组列中除去在基本列中已出现的列
			List<Column> others = new List<Column>();

			foreach (Column col in GroupColumns)
			{
				if (Array.IndexOf<Column>(primitiveRootColumns, col) == -1)
				{
					others.Add(col);
				}
			}

			allColumns.AddRange(
					others.ConvertAll<Column>(
						delegate(Column col)
						{
							return new GroupItemColumn(col);
						}
					)
				);

			return allColumns.ToArray();
		}

		/// <summary>
		/// 查找具有指定属性名称的基本列（映射于基本属性）。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>与该基本属性映射的列，如果没有找到，则返回 null。</returns>
		private Column FindPrimitiveColumn(String propertyName)
		{
			foreach (Column col in m_primitiveColumns)
			{
				if (col.PropertyName.Equals(propertyName, CommonPolicies.PropertyNameComparison))
				{
					return col;
				}
			}

			return null;
		}

		/// <summary>
		/// 查找与具有指定属性名称的外部引用属性相匹配的实体架构。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>与该属性相匹配的实体架构，如果未找到，则返回 null。</returns>
		private EntitySchema FindSchema(String propertyName)
		{
			foreach (GroupForeignReference foreignRef in m_foreignReferences)
			{
				if (foreignRef.Property.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
				{
					return foreignRef.Schema;
				}
			}

			return null;
		}

		#endregion

		#region IDebugInfoProvider 成员

		/// <summary>
		/// 获取实例的详细信息，用于调试。
		/// </summary>
		/// <returns>详细信息。</returns>
		public String Dump()
		{
			StringBuilder output = new StringBuilder();
			String line = String.Empty.PadRight(120, '-');

			output.AppendLine();
			output.AppendLine(line);

			output.AppendFormat("分组实体架构组合：{0}", m_definition.Type.FullName);

			output.AppendLine();
			output.AppendLine(line);

			// 首先输出所有列的信息列表
			DumpAllColumns(output);

			// 然后输出目标实体架构的详细信息
			DumpComposite(output);

			// 然后输出目标实体的生成策略详细信息
			DumpBuilderStrategy(output);

			// 结束
			output.AppendLine();
			output.AppendLine(line);

			return output.ToString();
		}

		#region 辅助方法

		/// <summary>
		/// 向缓存写入所有列的详细信息。
		/// </summary>
		/// <param name="output">输出缓存。</param>
		private void DumpAllColumns(StringBuilder output)
		{
			const Int32 INDEX_NAME = 0;
			const Int32 INDEX_ALIAS = 1;
			const Int32 INDEX_TABLE = 2;
			const Int32 INDEX_AGGREGATION_NAME = 3;
			const Int32 INDEX_SELECT = 4;
			const Int32 INDEX_TYPE = 5;
			const Int32 INDEX_DB_TYPE = 6;

			output.AppendLine("所有列：");

			String[] caption = new String[] { "名称", "别名", "表", "分组依据", "选择？", "类型", "数据库类型" };
			TabularWriter writer = new TabularWriter(caption.Length, DbEntityDebugger.DEFAULT_INDENT);

			writer.WriteLine(caption);

			foreach (Column col in CreateAllColumns())
			{
				String[] line = writer.NewLine();

				line[INDEX_NAME] = col.Name ?? AggregationAttribute.KEY_WORD_WILDCARD;
				line[INDEX_ALIAS] = col.Alias;
				line[INDEX_TABLE] = (col.Property != null)
					? String.Format("{0}({1})", col.Property.Schema.TableName, col.Property.Schema.TableAlias)
					: DbEntityDebugger.GetTristateBoolean((Boolean?)null);
				line[INDEX_AGGREGATION_NAME] = col.AggregationName;
				line[INDEX_SELECT] = DbEntityDebugger.GetTristateBoolean(col.Selected);
				line[INDEX_TYPE] = col.Type.Name;
				line[INDEX_DB_TYPE] = col.DbType.ToString();

				writer.WriteLine(line);
			}

			output.Append(writer.ToString());
		}

		/// <summary>
		/// 向缓存写入目标实体架构的详细信息。
		/// </summary>
		/// <param name="output">输出缓存。</param>
		private void DumpComposite(StringBuilder output)
		{
			output.AppendLine("目标实体架构组合：");
			output.AppendLine(Composite.Dump(1));
		}

		/// <summary>
		/// 向缓存写入目标实体架构的生成策略的详细信息。
		/// </summary>
		/// <param name="output">输出缓存。</param>
		private void DumpBuilderStrategy(StringBuilder output)
		{
			output.AppendLine("目标实体架构的生成策略：");
			output.AppendLine(Composite.BuilderStrategy.Dump(1));
		}

		#endregion

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