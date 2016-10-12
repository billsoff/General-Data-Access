#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupSchemaBuilder.cs
// 文件功能描述：分组结果实体架构生成器，用于生成完整的分组结果实体架构。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110630
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
	/// 分组结果实体架构生成器，用于生成完整的分组结果实体架构。
	/// </summary>
	public sealed class GroupSchemaBuilder
	{
		#region 私有字段

		private readonly GroupDefinition m_definition;

		private readonly List<Column> m_selectColumns;
		private readonly List<Column> m_groupColumns;

		private readonly List<Column> m_primitiveColumns;
		private readonly List<GroupForeignReference> m_foreignReferences;

		private readonly List<IPropertyChain> m_schemas;
		private readonly List<IPropertyChain> m_properties;
		private readonly List<IPropertyChain> m_groupChains;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		/// <param name="groupResultType">分组结果实体类型。</param>
		public GroupSchemaBuilder(Type groupResultType)
		{
			#region 前置条件

			Debug.Assert(groupResultType != null, "分组结果实体类型参数 groupResultType 不能为空。");
			Debug.Assert(
					typeof(GroupResult).IsAssignableFrom(groupResultType),
					String.Format("分组结果实体类型 {0} 没有从类型 GroupResult 派生。", groupResultType.FullName)
				);
			Debug.Assert(
					Attribute.IsDefined(groupResultType, typeof(GroupAttribute)),
					String.Format("分组结果实体类型 {0} 上没有标记 GroupAttribute。", groupResultType.FullName)
				);

			#endregion

			m_definition = GroupDefinitionBuilder.Build(groupResultType);

			m_selectColumns = new List<Column>();
			m_groupColumns = new List<Column>();

			m_primitiveColumns = new List<Column>();
			m_foreignReferences = new List<GroupForeignReference>();

			m_schemas = new List<IPropertyChain>();
			m_properties = new List<IPropertyChain>();
			m_groupChains = new List<IPropertyChain>();
		}

		#endregion

		#region 公共方法

		#region 构造过程

		/// <summary>
		/// 使用用于 HAVING 子句的过滤器扩展架构信息。
		/// </summary>
		/// <param name="havingFilter">用于 HAVING 子句的过滤器。</param>
		public void ExtendHavingFilter(Filter havingFilter)
		{
			#region 性能计数

			Timing.Start("扩展 HAVING 过滤器", "GroupSchemaBuilder.ExtendHavingFilter {0CF8B5CA-30CA-479a-AEC8-BC1F65C3BA00}");

			#endregion

			if (havingFilter != null)
			{
				ColumnLocator[] allColumnLocators = havingFilter.GetAllColumnLocators();
				ExtendGroupColumnLocators(allColumnLocators);
			}

			#region 性能计数

			Timing.Stop("GroupSchemaBuilder.ExtendHavingFilter {0CF8B5CA-30CA-479a-AEC8-BC1F65C3BA00}");

			#endregion
		}

		/// <summary>
		/// 使用用于 WHERE 子句的过滤器扩展架构信息。
		/// </summary>
		/// <param name="whereFilter">用于 WHERE 子句的过滤器。</param>
		public void ExtendWhereFilter(Filter whereFilter)
		{
			#region 性能计数

			Timing.Start("扩展 WHERE 过滤器", "GroupSchemaBuilder.ExtendWhereFilter {BB499A8B-A946-4a8f-A10F-5B35A78E7A8F}");

			#endregion

			if (whereFilter == null)
			{
				#region 性能计数

				Timing.Stop("GroupSchemaBuilder.ExtendWhereFilter {BB499A8B-A946-4a8f-A10F-5B35A78E7A8F}");

				#endregion

				return;
			}

			ColumnLocator[] allColumnLocators = whereFilter.GetAllColumnLocators();

			foreach (ColumnLocator colLocator in allColumnLocators)
			{
				if (colLocator.PropertyPath.Length > 1)
				{
					String[] parentPropertyPath = new String[colLocator.PropertyPath.Length - 1];
					Array.Copy(colLocator.PropertyPath, 0, parentPropertyPath, 0, parentPropertyPath.Length);

					IPropertyChain reference = new PropertyChain(m_definition.Entity.Type, parentPropertyPath);

					if (!m_schemas.Contains(reference))
					{
						m_schemas.Add(reference);
					}
				}
			}

			#region 性能计数

			Timing.Stop("GroupSchemaBuilder.ExtendWhereFilter {BB499A8B-A946-4a8f-A10F-5B35A78E7A8F}");

			#endregion
		}

		/// <summary>
		/// 使用排序器扩展架构信息。
		/// </summary>
		/// <param name="sorter">排序器。</param>
		public void ExtendSorter(Sorter sorter)
		{
			#region 性能计数

			Timing.Start("扩展排序器", "ExtendSorter.GroupSchemaBuilder {149D86C2-6521-4a00-BA28-EE165EF42F70}");

			#endregion

			if (sorter != null)
			{
				ColumnLocator[] allColumnLocators = sorter.GetAllColumnLocators();
				ExtendGroupColumnLocators(allColumnLocators);
			}

			#region 性能计数

			Timing.Stop("ExtendSorter.GroupSchemaBuilder {149D86C2-6521-4a00-BA28-EE165EF42F70}");

			#endregion
		}

		/// <summary>
		/// 扩展属性。
		/// </summary>
		/// <param name="properties">属性集合。</param>
		/// <param name="inline">true - 仅加载属性所属的架构并使该属性参与分组；false - 还须选择该属性。</param>
		public void ExtendProperties(IPropertyChain[] properties, Boolean inline)
		{
			ColumnLocator[] allColumnLocators = Array.ConvertAll<IPropertyChain, ColumnLocator>(
					properties,
					delegate(IPropertyChain chain)
					{
						return new ColumnLocator(chain.PropertyPath);
					}
				);

			ExtendGroupColumnLocators(allColumnLocators);

			if (!inline)
			{
				foreach (IPropertyChain chain in properties)
				{
					if (chain.Previous == null)
					{
						continue;
					}

					IPropertyChain entityChain = GetForeignReferencePropertyChain(chain.PropertyPath);

					if (!m_properties.Contains(entityChain))
					{
						m_properties.Add(entityChain);
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// 创建分组结果实体架构。
		/// </summary>
		/// <returns>创建好的分组结果实体架构。</returns>
		public GroupSchema Build()
		{
			#region 性能计数

			Timing.Start("生成分组结果实体架构", "GroupSchemaBuilder.Build {AE38473C-17BC-4c6a-89C6-FC873EA5B7F6}");

			#endregion

			GroupSchema schema = new GroupSchema();

			schema.Definition = m_definition;

			schema.Composite = BuildComposite();
			BuildColumns(schema.Composite);

			schema.SelectColumns = m_selectColumns.ToArray();
			schema.GroupColumns = m_groupColumns.ToArray();

			schema.PrimitiveColumns = m_primitiveColumns.ToArray();
			schema.ForeignReferences = m_foreignReferences.ToArray();

			#region 性能计数

			Timing.Stop("GroupSchemaBuilder.Build {AE38473C-17BC-4c6a-89C6-FC873EA5B7F6}");

			#endregion

			return schema;
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 创建实体架构组合。
		/// </summary>
		/// <returns>创建好的实体架构组合。</returns>
		private EntitySchemaComposite BuildComposite()
		{
			CompositeBuilderStrategy strategy = CompositeBuilderStrategyFactory.Create4Group(m_definition.Type);

			List<PropertySelector> allSelectors = new List<PropertySelector>();

			if (m_schemas.Count != 0)
			{
				allSelectors.AddRange(
						m_schemas.ConvertAll<PropertySelector>(
							           delegate(IPropertyChain chain)
							           {
								           return PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain);
							           }
						           )
				           );

			}

			if (m_properties.Count != 0)
			{
				allSelectors.AddRange(
						m_properties.ConvertAll<PropertySelector>(
								delegate(IPropertyChain chain)
								{
									return PropertySelector.Create(PropertySelectMode.Property, chain);
								}
							)
					);
			}

			if (allSelectors.Count != 0)
			{
				strategy = CompositeBuilderStrategyFactory.Union(
						strategy,
						CompositeBuilderStrategyFactory.Create(allSelectors)
					);
			}

			PropertyTrimmer trimmer = PropertyTrimmer.CreateGroupIncapableTrimmer();

			strategy = CompositeBuilderStrategyFactory.TrimOff(strategy, trimmer);
			strategy.AlwaysSelectPrimaryKeyProperties = false;

			EntitySchemaComposite composite = EntitySchemaCompositeFactory.Create(m_definition.Entity.Type, strategy);

			return composite;
		}

		/// <summary>
		/// 生成列。
		/// </summary>
		/// <param name="composite">要聚合的实体架构组合。</param>
		private void BuildColumns(EntitySchemaComposite composite)
		{
			BuildSelectAndPrimitiveColumns(composite);
			BuildGroupColumns(composite);
			BuildForeignReferences(composite);
		}

		/// <summary>
		/// 生成外部引用。
		/// </summary>
		/// <param name="composite">要聚合的实体架构组合。</param>
		private void BuildForeignReferences(EntitySchemaComposite composite)
		{
			GroupPropertyDefinition[] foreignReferences = m_definition.GetForeignReferenceProperties();

			foreach (GroupPropertyDefinition propertyDef in foreignReferences)
			{
				EntitySchema schema = FindSchema(composite, propertyDef);

				#region 前置条件

				Debug.Assert(schema != null, String.Format("未找到与属性 {0} 相匹配的实体架构。", propertyDef));

				#endregion

				m_foreignReferences.Add(new GroupForeignReference(propertyDef, schema));
			}
		}

		/// <summary>
		/// 生成分组列。
		/// </summary>
		/// <param name="composite">要聚合的实体架构组合。</param>
		private void BuildGroupColumns(EntitySchemaComposite composite)
		{
			m_groupColumns.AddRange(composite.Columns);

			foreach (IPropertyChain chain in m_groupChains)
			{
				IPropertyChain previous = chain.Previous;
				EntitySchema schema;

				if (previous == null)
				{
					schema = composite.Target;
				}
				else
				{
					schema = composite[previous.FullName];
				}

				#region 前置条件

				Debug.Assert(schema != null, String.Format("未找到属性 {0} 所属的实体架构。", chain));

				#endregion

				EntityProperty property = schema.Properties[chain.Name];

				#region 前置条件

				Debug.Assert(property != null, String.Format("在实体架构 {0} 中不包含属性 {1}。", schema, chain.Name));

				#endregion

				// 判断要分组的列是否已被选择
				Boolean exists = false;

				if (!schema.SelectNothing)
				{
					EntityProperty[] selectProperties = composite.GetSelectProperties(schema);

					exists = Array.Exists<EntityProperty>(
							selectProperties,
							delegate(EntityProperty item)
							{
								return (item == property);
							}
						);
				}

				if (!exists)
				{
					m_groupColumns.AddRange(property.Columns);
				}
			}
		}

		/// <summary>
		/// 生成选择和基本列。
		/// </summary>
		/// <param name="composite">要聚合的实体架构组合。</param>
		private void BuildSelectAndPrimitiveColumns(EntitySchemaComposite composite)
		{
			List<Column> nonGroupColumns = new List<Column>();
			m_selectColumns.AddRange(composite.Columns);
			GroupPropertyDefinition[] primitiveProperties = m_definition.GetPrimitiveProperties();

			foreach (GroupPropertyDefinition groupPropertyDef in primitiveProperties)
			{
				EntitySchema schema = FindSchema(composite, groupPropertyDef);
				EntityProperty aggregationProperty;

				if (schema != null)
				{
					aggregationProperty = schema.Properties[groupPropertyDef.PropertyChain.Name];
				}
				else
				{
					aggregationProperty = null;
				}

				// 分组项（已选择）
				if (groupPropertyDef.IsGroupItem)
				{
					#region 前置条件

					Debug.Assert(
							aggregationProperty != null,
							String.Format("未找到与分组结果实体属性定义 {0} 相匹配的要聚合的属性。", groupPropertyDef)
						);

					#endregion

					GroupItemColumn itemColumn = new GroupItemColumn(aggregationProperty.Columns[0], groupPropertyDef);
					m_primitiveColumns.Add(itemColumn);
				}
				// 聚合项
				else
				{
					AggregationColumn primitiveColumn = new AggregationColumn(aggregationProperty, groupPropertyDef);
					primitiveColumn.Selected = true;

					m_primitiveColumns.Add(primitiveColumn);
					nonGroupColumns.Add(primitiveColumn);
				}
			}

			for (Int32 i = 0; i < nonGroupColumns.Count; i++)
			{
				Column primitiveColumn = nonGroupColumns[i];
				primitiveColumn.Index = (composite.Columns.Length + i);
			}

			m_selectColumns.AddRange(nonGroupColumns);
		}

		/// <summary>
		/// 查找属性定义所要聚合的属性所属或匹配的实体架构。
		/// </summary>
		/// <param name="composite">实体架构组合。</param>
		/// <param name="propertyDef">分组结果实体属性定义。</param>
		/// <returns>找到的实体架构，如果没有找到，则返回 null。</returns>
		private EntitySchema FindSchema(EntitySchemaComposite composite, GroupPropertyDefinition propertyDef)
		{
			if (propertyDef.PropertyChain == null)
			{
				return null;
			}

			String propertyPath;

			if (propertyDef.IsPrimitive)
			{
				IPropertyChain previous = propertyDef.PropertyChain.Previous;

				propertyPath = (previous != null) ? previous.FullName : propertyDef.PropertyChain.Type.Name;
			}
			else
			{
				propertyPath = propertyDef.PropertyChain.FullName;
			}

			return composite[propertyPath];
		}

		/// <summary>
		/// 扩展分组列定位符，总是加载属性所属的实体架构，并将属性放入分组列表中。
		/// </summary>
		/// <param name="allColumnLocators">列定位符集合。</param>
		private void ExtendGroupColumnLocators(ColumnLocator[] allColumnLocators)
		{
			foreach (ColumnLocator colLocator in allColumnLocators)
			{
				#region 前置条件

				Debug.Assert(
						m_definition.Properties.Contains(colLocator.PropertyPath[0]),
						String.Format(
								"类型 {0} 中不包含属性 {1}，请检查聚合属性标记和 HAVING 或 ORDER BY 表达式。",
								m_definition.Type.FullName,
								colLocator.PropertyPath[0]
							)
					);

				#endregion

				// 如果属性为实体的直属属性，则不需要扩展
				if (colLocator.PropertyPath.Length == 1)
				{
					continue;
				}

				// 如果属性为外部引用属性的子属性，则需要加载和分组
				IPropertyChain groupChain = GetForeignReferencePropertyChain(colLocator.PropertyPath);

				#region 前置条件

				Debug.Assert(
						!Attribute.IsDefined(
								EntityDefinitionBuilder.Build(groupChain.Type).Properties[groupChain].PropertyInfo,
								typeof(NotSupportGroupingAttribute)
							),
						String.Format(
								"用于 HAVING 过滤或排序的属性 {0} 上标记 NotSupportGroupingAttribute，该属性不能参与分组。",
								groupChain.FullName
							)
					);

				#endregion

				if (!m_groupChains.Contains(groupChain))
				{
					m_groupChains.Add(groupChain);
				}

				IPropertyChain previous = groupChain.Previous;

				if (!previous.IsImmediateProperty)
				{
					IPropertyChain reference = previous.Copy();

					if (!m_schemas.Contains(reference))
					{
						m_schemas.Add(reference);
					}
				}
			}
		}

		/// <summary>
		/// 获取面向要分组的实体的属性链。
		/// </summary>
		/// <param name="propertyPath">面向分组结果实体的属性路径。</param>
		/// <returns>对应的面向要分组的实体的属性链。</returns>
		private IPropertyChain GetForeignReferencePropertyChain(String[] propertyPath)
		{
			GroupPropertyDefinition groupPropertyDef = m_definition.Properties[propertyPath[0]];
			String[] newPropertyPath = new String[groupPropertyDef.PropertyChain.Depth + propertyPath.Length - 1];

			Array.Copy(groupPropertyDef.PropertyChain.PropertyPath, 0, newPropertyPath, 0, groupPropertyDef.PropertyChain.Depth);
			Array.Copy(propertyPath, 1, newPropertyPath, groupPropertyDef.PropertyChain.Depth, (propertyPath.Length - 1));

			IPropertyChain foreignRef = new PropertyChain(groupPropertyDef.PropertyChain.Type, newPropertyPath);

			return foreignRef;
		}

		#endregion
	}
}