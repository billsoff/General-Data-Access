#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AssemblyChildrenEntry.cs
// 文件功能描述：子实体装配项。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110729
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 子实体装配项。
	/// </summary>
	[Serializable]
	internal sealed class AssemblyChildrenEntry
	{
		#region 私有字段

		private readonly AssemblyPolicy m_assemblyPolicy;
		private readonly AssemblyChildrenEntry m_parent;

		private readonly String m_propertyName;
		private readonly CompositeBuilderStrategy m_builderStrategy;

		private List<AssemblyChildrenEntry> m_innerList;
		private AssemblyChildrenEntryCollection m_childrenEntries;

		private IPropertyChain m_propertyChain;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置装配方针、父装配项、属性名称和加载策略。
		/// </summary>
		/// <param name="policy">装配方针。</param>
		/// <param name="parent">父装配项。</param>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="builderStrategy">加载策略。</param>
		public AssemblyChildrenEntry(AssemblyPolicy policy, AssemblyChildrenEntry parent, String propertyName, CompositeBuilderStrategy builderStrategy)
		{
			m_assemblyPolicy = policy;
			m_parent = parent;
			m_propertyName = propertyName;
			m_builderStrategy = builderStrategy;
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取子属性定义。
		/// </summary>
		internal EntityPropertyDefinition ChildrenProperty
		{
			get
			{
				return Property.ChildrenProperty;
			}
		}

		/// <summary>
		/// 获取装配方针的内部列表。
		/// </summary>
		internal IList<AssemblyChildrenEntry> InnerList
		{
			get
			{
				if (m_innerList == null)
				{
					m_innerList = new List<AssemblyChildrenEntry>();
				}

				return m_innerList;
			}
		}

		/// <summary>
		/// 获取父装配指针。
		/// </summary>
		internal AssemblyChildrenEntry Parent
		{
			get { return m_parent; }
		}

		/// <summary>
		/// 获取子装配项的属性链。
		/// </summary>
		internal IPropertyChain PropertyChain
		{
			get
			{
				if (m_propertyChain == null)
				{
					if (Parent == null)
					{
						m_propertyChain = new PropertyChain(AssemblyPolicy.Type, new String[] { PropertyName });
					}
					else
					{
						List<String> propertyNames = new List<String>();
						AssemblyChildrenEntry current = this;

						do
						{
							propertyNames.Insert(0, current.PropertyName);
							current = current.Parent;
						} while (current != null);

						m_propertyChain = new PropertyChain(AssemblyPolicy.Type, propertyNames.ToArray());
					}
				}

				return m_propertyChain;
			}
		}

		/// <summary>
		/// 获取子属性名称。
		/// </summary>
		internal String PropertyName
		{
			get { return m_propertyName; }
		}

		/// <summary>
		/// 获取包含的装配方针列表。
		/// </summary>
		internal AssemblyChildrenEntryCollection ChildrenEntries
		{
			get
			{
				if (m_childrenEntries == null)
				{
					m_childrenEntries = new AssemblyChildrenEntryCollection(InnerList);
				}

				return m_childrenEntries;
			}
		}

		#endregion

		#region 私有属性

		/// <summary>
		/// 获取加载方针。
		/// </summary>
		private AssemblyPolicy AssemblyPolicy
		{
			get { return m_assemblyPolicy; }
		}

		/// <summary>
		/// 获取加载策略。
		/// </summary>
		private CompositeBuilderStrategy BuilderStrategy
		{
			get
			{
				return m_builderStrategy ?? AssemblyPolicy.BuilderStrategy;
			}
		}

		/// <summary>
		/// 获取子实体定义。
		/// </summary>
		private EntityDefinition ChildrenDefinition
		{
			get
			{
				return ChildrenProperty.Entity;
			}
		}

		/// <summary>
		/// 获取子属性过滤器。
		/// </summary>
		private Sorter ChildrenSorter
		{
			get
			{
				return Property.ChildrenSorter;
			}
		}

		/// <summary>
		/// 获取子实体类型。
		/// </summary>
		private Type ChildrenType
		{
			get
			{
				return ChildrenDefinition.Type;
			}
		}

		/// <summary>
		/// 获取父实体定义。
		/// </summary>
		private EntityDefinition Definition
		{
			get
			{
				return EntityDefinitionBuilder.Build(Type);
			}
		}

		/// <summary>
		/// 获取一个值，该值指示是否包子实体装配项。
		/// </summary>
		private Boolean HasChildrenEntries
		{
			get
			{
				return (m_innerList != null) && (m_innerList.Count != 0);
			}
		}

		/// <summary>
		/// 获取一个值，该值指示是否注册了监听器。
		/// </summary>
		internal Boolean HasListeners
		{
			get { return AssemblyPolicy.HasListeners; }
		}

		/// <summary>
		/// 获取装配监听者列表。
		/// </summary>
		internal IList<AssemblyListener> Listeners
		{
			get
			{
				return AssemblyPolicy.Listeners;
			}
		}

		/// <summary>
		/// 获取在父实体中的属性定义。
		/// </summary>
		private EntityPropertyDefinition Property
		{
			get
			{
				return Definition.Properties[PropertyName];
			}
		}

		/// <summary>
		/// 获取实体类型。
		/// </summary>
		private Type Type
		{
			get { return (Parent != null) ? Parent.ChildrenType : AssemblyPolicy.Type; }
		}

		#endregion

		#region 内部方法

		/// <summary>
		/// 创建所有的子实体装配项。
		/// </summary>
		internal void BuildAllChildren()
		{
			foreach (EntityPropertyDefinition propertyDef in ChildrenDefinition.Properties.GetAllChildrenProperties())
			{
				AssemblyChildrenEntry childernEntry = new AssemblyChildrenEntry(
						AssemblyPolicy,
						this,
						propertyDef.Name,
						(CompositeBuilderStrategy)null
					);
				InnerList.Add(childernEntry);

				if (!IsChildrenEntryTypeRepetitive(childernEntry))
				{
					childernEntry.BuildAllChildren();
				}
			}
		}

		/// <summary>
		/// 进行子实体装配。
		/// </summary>
		/// <param name="parentEntities">父级实体列表。</param>
		/// <param name="parentFilter">父级过滤器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		internal void Enforce(Object[] parentEntities, Filter parentFilter, IDatabaseSession databaseSession)
		{
			#region 性能计数

#if DEBUG

			String metricId = Guid.NewGuid().ToString();
			Timing.Start("自动装配子实体集合", metricId);

#endif

			#endregion

			if (parentEntities.Length != 0)
			{
				// 子实体过滤器
				Filter childFilter = Transform(parentFilter);

				OnChildrenAssemblyStart(parentEntities, childFilter, databaseSession);

				// 加载子实体
				Object[] childEntities = Load(childFilter, databaseSession);

				// 装配
				Assembly(parentEntities, childEntities);

				if (childEntities.Length != 0)
				{
					OnChildrenAssemblyComplete(parentEntities, childEntities, childFilter, databaseSession);
				}

				if (HasChildrenEntries)
				{
					// 遍历每一个子装配项
					foreach (AssemblyChildrenEntry childrenEntry in ChildrenEntries)
					{
						childrenEntry.Enforce(childEntities, childFilter, databaseSession);
					}
				}
			}

			#region 性能计数

#if DEBUG

			Timing.Stop(metricId);

#endif

			#endregion
		}

		/// <summary>
		/// 获取与要装配的子属性相关联的属性选择器。
		/// </summary>
		/// <returns>属性选择器集合。</returns>
		internal PropertySelector[] GetAllSelectors()
		{
			List<PropertySelector> allSelectors = new List<PropertySelector>();

			if (HasChildrenEntries)
			{
				foreach (AssemblyChildrenEntry childrenEntry in ChildrenEntries)
				{
					allSelectors.AddRange(
							Array.ConvertAll<EntityPropertyDefinition, PropertySelector>(
									childrenEntry.ChildrenProperty.Relation.ParentProperties,
									delegate(EntityPropertyDefinition propertyDef)
									{
										return PropertySelector.Create(
												new PropertyChain(ChildrenType, new String[] { propertyDef.Name })
											);
									}
								)
						);
				}
			}

			return allSelectors.ToArray();
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 进行装配操作。
		/// </summary>
		/// <param name="parentEntities">父实体集合。</param>
		/// <param name="childEntities">子实体集合。</param>
		private void Assembly(Object[] parentEntities, Object[] childEntities)
		{
			ColumnDefinition[] childColumns = ChildrenProperty.Relation.ChildColumns;
			ColumnDefinition[] parentColumns = ChildrenProperty.Relation.ParentColumns;

			foreach (Object parent in parentEntities)
			{
				Object[] children = FindChildren(parent, childEntities, childColumns, parentColumns);

				// 为父装配子实体。
				Property.PropertyInfo.SetValue(parent, children, null);

				// 将每一个子实体的父进行置换
				foreach (Object child in children)
				{
					ChildrenProperty.PropertyInfo.SetValue(child, parent, null);
				}
			}
		}

		/// <summary>
		/// 为当前加载方针创建合适的策略。
		/// </summary>
		/// <param name="builderStrategy">加载策略。</param>
		/// <returns>创建好的合适的策略。</returns>
		private CompositeBuilderStrategy ComposeBuilderStrategy(CompositeBuilderStrategy builderStrategy)
		{
			List<PropertySelector> additional = new List<PropertySelector>();

			// 保证选择连接属性
			additional.Add(
					PropertySelector.Create(
							new PropertyChain(
									ChildrenType,
									new String[] { ChildrenProperty.Name }
								)
						)
				);

			// 保证选择与子实体相关联的属性
			additional.AddRange(GetAllSelectors());

			// 基础策略
			CompositeBuilderStrategy strategy = CompositeBuilderStrategyFactory.Compose(ChildrenType, builderStrategy, additional);

			ChildrenRedundantPropertyTrimmer trimmer = new ChildrenRedundantPropertyTrimmer(ChildrenProperty);

			strategy = CompositeBuilderStrategyFactory.TrimOff(strategy, trimmer);
			strategy.AlwaysSelectPrimaryKeyProperties = false;

			return strategy;
		}

		/// <summary>
		/// 查找子实体集合。
		/// </summary>
		/// <param name="parent">父实体。</param>
		/// <param name="childEntities">所有的子实体集合。</param>
		/// <param name="childColumns">子列定义集合。</param>
		/// <param name="parentColumns">父列定义集合。</param>
		/// <returns>属性于父实体的子实体集合。</returns>
		private Object[] FindChildren(Object parent, Object[] childEntities, ColumnDefinition[] childColumns, ColumnDefinition[] parentColumns)
		{
			Object[] parentPropertyValues = new Object[parentColumns.Length];

			for (Int32 i = 0; i < parentColumns.Length; i++)
			{
				parentPropertyValues[i] = parentColumns[i].GetDbValue(parent);
			}

			Object[] results = Array.FindAll(
					childEntities,
					delegate(Object child)
					{
						for (Int32 i = 0; i < childColumns.Length; i++)
						{
							Object childPropertyValue = childColumns[i].GetDbValue(child);
							Object parentPropertyValue = parentPropertyValues[i];

							if (!childPropertyValue.Equals(parentPropertyValue))
							{
								return false;
							}
						}

						return true;
					}
				);

			Object[] children = (Object[])Array.CreateInstance(ChildrenType, results.Length);
			Array.Copy(results, children, results.Length);

			return children;
		}

		/// <summary>
		/// 判断子实体的类型是否已重复。
		/// </summary>
		/// <param name="childEntry">子装配项。</param>
		/// <returns>如果已重复，则返回 true；否则返回 false。</returns>
		private static Boolean IsChildrenEntryTypeRepetitive(AssemblyChildrenEntry childEntry)
		{
			Type childrenType = childEntry.ChildrenType;
			AssemblyChildrenEntry parent = childEntry.Parent;

			while (parent != null)
			{
				if (parent.ChildrenType == childrenType)
				{
					return true;
				}

				parent = parent.Parent;
			}

			return false;
		}

		/// <summary>
		/// 获取子实体集合。
		/// </summary>
		/// <param name="filter">过滤器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		/// <returns>子实体集合。</returns>
		private Object[] Load(Filter filter, IDatabaseSession databaseSession)
		{
			Object[] children = databaseSession.Load(
					ChildrenType,
					ComposeBuilderStrategy(BuilderStrategy),
					filter,
					ChildrenSorter
				);

			return children;
		}

		/// <summary>
		/// 通知监听器子实体开始装配。
		/// </summary>
		/// <param name="parentEntities">父实体集合。</param>
		/// <param name="filter">过滤器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		private void OnChildrenAssemblyStart(Object[] parentEntities, Filter filter, IDatabaseSession databaseSession)
		{
			if (HasListeners)
			{
				foreach (AssemblyListener listener in Listeners)
				{
					listener.OnChildrenAssemblyStart(
							AssemblyPolicy,
							PropertyChain,
							parentEntities,
							filter,
							ChildrenSorter,
							databaseSession
						);
				}
			}
		}

		/// <summary>
		/// 通知监听器子实体装配完成。
		/// </summary>
		/// <param name="parentEntities">父实体集合。</param>
		/// <param name="children">装配好的子实体集合。</param>
		/// <param name="filter">过滤器。</param>
		/// <param name="databaseSeesion">数据库会话引擎。</param>
		private void OnChildrenAssemblyComplete(Object[] parentEntities, Object[] children, Filter filter, IDatabaseSession databaseSeesion)
		{
			if (HasListeners)
			{
				foreach (AssemblyListener listener in Listeners)
				{
					listener.OnChildrenAssemblyComplete(
							AssemblyPolicy,
							PropertyChain,
							parentEntities,
							children,
							filter,
							ChildrenSorter,
							databaseSeesion
						);
				}
			}
		}

		/// <summary>
		/// 将父过滤条件转换为子实体过滤条件。
		/// </summary>
		/// <param name="parentFilter">父过滤条件。</param>
		/// <returns>子实体过滤条件。</returns>
		private Filter Transform(Filter parentFilter)
		{
			if (parentFilter == null)
			{
				return null;
			}

			Filter childrenFilter = parentFilter.Extend(ChildrenProperty.Name);

			return childrenFilter;
		}

		#endregion


		#region 属性修剪器

		/// <summary>
		/// 修剪掉父实体中除连接属性外的其他属性。
		/// </summary>
		private sealed class ChildrenRedundantPropertyTrimmer : PropertyTrimmer
		{
			#region 私有字段

			private readonly IPropertyChain m_schema;
			private readonly IPropertyChain[] m_parentProperties;

			#endregion

			#region 构造函数

			/// <summary>
			/// 构造函数，设置子实体属性定义。
			/// </summary>
			/// <param name="childrenProperty">子实体属性定义。</param>
			public ChildrenRedundantPropertyTrimmer(EntityPropertyDefinition childrenProperty)
			{
				Type childrenType = childrenProperty.Entity.Type;
				EntityPropertyDefinitionRelation relation = childrenProperty.Relation;

				m_schema = new PropertyChain(childrenType, new String[] { childrenProperty.Name });
				m_parentProperties = Array.ConvertAll<EntityPropertyDefinition, IPropertyChain>(
						relation.ParentProperties,
						delegate(EntityPropertyDefinition propertyDef)
						{
							return new PropertyChain(childrenType, new String[] { childrenProperty.Name, propertyDef.Name });
						}
					);
			}

			#endregion

			/// <summary>
			/// 修剪掉父实体中多余的属性。
			/// </summary>
			/// <param name="property">要判断的属性。</param>
			/// <returns>如果修剪此属性，则返回 true；否则返回 false。</returns>
			public override Boolean TrimOff(EntityProperty property)
			{
				return m_schema.Contains(property)
					&& !m_schema.Equals(property.PropertyChain)
					&& (Array.IndexOf<IPropertyChain>(m_parentProperties, property.PropertyChain) != -1); // 非要装配的目标父实体
			}

			/// <summary>
			/// 获取显示名称，用于调试。
			/// </summary>
			public override String DisplayName
			{
				get
				{
					return "修剪掉父实体架构中的非连接属性";
				}
			}
		}

		#endregion
	}
}