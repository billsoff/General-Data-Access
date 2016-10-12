#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AssemblyPolicy.cs
// 文件功能描述：配置方针，指示如何加载子实体。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110726
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
	/// 配置方针，指示如何加载子实体。
	/// </summary>
	[Serializable]
	public sealed partial class AssemblyPolicy
	{
		#region 静态成员

		/// <summary>
		/// 创建方针根容器。
		/// </summary>
		/// <returns>根容器。</returns>
		internal static AssemblyPolicy CreateRoot()
		{
			return new AssemblyPolicy();
		}

		/// <summary>
		/// 创建方针根容器，指示加载所有的（包括间接的）子实体。
		/// </summary>
		/// <returns>根容器。</returns>
		internal static AssemblyPolicy CreateLoadAllChildren()
		{
			return new AssemblyPolicy(true);
		}

		#endregion

		#region 私有字段

		[NonSerialized]
		private Type m_type;

		private readonly Boolean m_loadAllChildren;
		private CompositeBuilderStrategy m_builderStrategy;

		private List<AssemblyChildrenEntry> m_innerList;
		private AssemblyChildrenEntryCollection m_childrenEntries;

		private List<AssemblyListener> m_listeners;

		// 不需要装配的子实体属性的全名称集合
		private List<String> m_excepts;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，用于创建根容器。
		/// </summary>
		private AssemblyPolicy()
		{
		}

		/// <summary>
		/// 默认构造函数，用于创建根容器，并设置指示是否加载全部的子实体（包括间接的）。
		/// </summary>
		/// <param name="loadAllChiren">指示是否加载全部的子实体（包括间接的）。</param>
		private AssemblyPolicy(Boolean loadAllChiren)
		{
			m_loadAllChildren = loadAllChiren;
		}

		#endregion

		#region 内部属性

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

		/// <summary>
		/// 获取加载策略。
		/// </summary>
		internal CompositeBuilderStrategy BuilderStrategy
		{
			get
			{
				return m_builderStrategy;
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
		/// 获取一个值，该值指示是否注册了监听器。
		/// </summary>
		internal Boolean HasListeners
		{
			get { return (m_listeners != null) && (m_listeners.Count != 0); }
		}

		/// <summary>
		/// 获取装配监听者列表。
		/// </summary>
		internal List<AssemblyListener> Listeners
		{
			get
			{
				if (m_listeners == null)
				{
					return m_listeners = new List<AssemblyListener>();
				}

				return m_listeners;
			}
		}

		/// <summary>
		/// 获取实体类型。
		/// </summary>
		internal Type Type
		{
			get { return m_type; }
		}

		#endregion

		#region 私有属性

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
		/// 获取不需要装配的子实体属性的全名称集合。
		/// </summary>
		private IList<String> Excepts
		{
			get
			{
				if (m_excepts == null)
				{
					m_excepts = new List<String>();
				}

				return m_excepts;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示是否包含装配方针。
		/// </summary>
		private Boolean HasChildrenEntries
		{
			get
			{
				return (m_innerList != null) && (m_innerList.Count != 0);
			}
		}

		/// <summary>
		/// 获取一个值，该值指示是否有不需要加载的子实体。
		/// </summary>
		private Boolean HasExcepts
		{
			get { return (m_excepts != null) && (m_excepts.Count != 0); }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 进行子实体装配。
		/// </summary>
		/// <param name="entityType">父级实体类型。</param>
		/// <param name="parentEntities">父级实体列表。</param>
		/// <param name="parentFilter">父级过滤器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		public void Enforce(Type entityType, Object[] parentEntities, Filter parentFilter, IDatabaseSession databaseSession)
		{
			#region 性能计数

#if DEBUG

			String metricId = Guid.NewGuid().ToString();
			Timing.Start("自动装配子实体集合", metricId);

#endif

			#endregion

			if (parentEntities.Length == 0)
			{
				#region 性能计数

#if DEBUG

				Timing.Stop(metricId);

#endif

				#endregion

				return;
			}

			m_type = entityType;
			EnsureAllChildrenBuilt();

			if (HasChildrenEntries)
			{
				OnAssemblyStart(parentEntities, parentFilter, databaseSession);

				// 遍历每一个子装配项
				foreach (AssemblyChildrenEntry childrenEntry in ChildrenEntries)
				{
					childrenEntry.Enforce(parentEntities, parentFilter, databaseSession);
				}

				OnAssemblyComplete(parentEntities, parentFilter, databaseSession);
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
		/// <param name="entityType">实体类型。</param>
		/// <returns>属性选择器集合。</returns>
		public PropertySelector[] GetAllSelectors(Type entityType)
		{
			m_type = entityType;
			EnsureAllChildrenBuilt();
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
												new PropertyChain(entityType, new String[] { propertyDef.Name })
											);
									}
								)
						);
				}
			}

			return allSelectors.ToArray();
		}

		/// <summary>
		/// 设置装配方针的默认加载策略。
		/// </summary>
		/// <param name="strategyOption">策略选项。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicy Using(LoadStrategyOption strategyOption)
		{
			LoadStrategyAttribute attr = new LoadStrategyAttribute(strategyOption);
			CompositeBuilderStrategy strategy = attr.Create();

			return Using(strategy);
		}

		/// <summary>
		/// 设置装配方针的默认加载策略。
		/// </summary>
		/// <param name="level">加载级别。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicy Using(Int32 level)
		{
			return Using(CompositeBuilderStrategyFactory.Create(level));
		}

		/// <summary>
		/// 设置装配方针的默认加载策略，注意，此策略不能和类型相关。
		/// </summary>
		/// <param name="builderStrategy">装配方针的默认加载策略。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicy Using(CompositeBuilderStrategy builderStrategy)
		{
			m_builderStrategy = builderStrategy;

			return this;
		}

		/// <summary>
		/// 排除子属性，仅对 AllChildren 起作用。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicy Except(IPropertyChainBuilder builder)
		{
			return Except(builder.Build());
		}

		/// <summary>
		/// 排除子属性，仅对 AllChildren 起作用。
		/// </summary>
		/// <param name="chain">属性路径。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicy Except(IPropertyChain chain)
		{
			return Except(chain.PropertyPath);
		}

		/// <summary>
		/// 排除子属性，仅对 AllChildren 起作用。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicy Except(String[] propertyPath)
		{
			String fullName = CommonPolicies.ComposePropertyFullName(propertyPath);

			if (!Excepts.Contains(fullName))
			{
				Excepts.Add(fullName);
			}

			return this;
		}

		/// <summary>
		/// 注册装配监听器，注意，为支持远程访问，监听器应是可序列化的。
		/// </summary>
		/// <param name="listeners">监听器集合。</param>
		/// <returns>当前实例。</returns>
		public AssemblyPolicy Register(params AssemblyListener[] listeners)
		{
			this.Listeners.AddRange(listeners);

			return this;
		}

		#endregion

		#region 内部方法

		/// <summary>
		/// 获取父装配项。
		/// </summary>
		/// <param name="fullName">属性全名称。</param>
		/// <returns>父装配项。</returns>
		internal AssemblyChildrenEntry GetParentEntry(String fullName)
		{
			if (!fullName.Contains(CommonPolicies.DOT))
			{
				return null;
			}
			else
			{
				Int32 index = fullName.LastIndexOf(CommonPolicies.DOT);

				String[] previousPropertyPath = fullName.Substring(0, index).Split(CommonPolicies.DOT.ToCharArray());

				return ChildrenEntries[previousPropertyPath];
			}
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 创建所有的子装配项。
		/// </summary>
		private void EnsureAllChildrenBuilt()
		{
			if (!m_loadAllChildren || HasChildrenEntries)
			{
				return;
			}

			foreach (EntityPropertyDefinition propertyDef in Definition.Properties.GetAllChildrenProperties())
			{
				AssemblyChildrenEntry childrenEntry = new AssemblyChildrenEntry(
						this,
						(AssemblyChildrenEntry)null,
						propertyDef.Name,
						(CompositeBuilderStrategy)null);
				InnerList.Add(childrenEntry);

				childrenEntry.BuildAllChildren();
			}

			RemoveExcepts();
		}

		/// <summary>
		/// 通告监听器开始装配。
		/// </summary>
		/// <param name="parentEntities">根实体集合。</param>
		/// <param name="filter">根过滤器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		private void OnAssemblyStart(Object[] parentEntities, Filter filter, IDatabaseSession databaseSession)
		{
			if (HasListeners)
			{
				foreach (AssemblyListener listener in Listeners)
				{
					listener.OnAssemblyStart(this, parentEntities, filter, databaseSession);
				}
			}
		}

		/// <summary>
		/// 通知监听器完成装配。
		/// </summary>
		/// <param name="parentEntities">根实体集合。</param>
		/// <param name="filter">根过滤器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		private void OnAssemblyComplete(Object[] parentEntities, Filter filter, IDatabaseSession databaseSession)
		{
			if (HasListeners)
			{
				foreach (AssemblyListener listener in Listeners)
				{
					listener.OnAssemblyComplete(this, parentEntities, filter, databaseSession);
				}
			}
		}

		/// <summary>
		/// 移除所有不需要加载的子实体装配项。
		/// </summary>
		private void RemoveExcepts()
		{
			if (HasExcepts)
			{
				foreach (String fullName in Excepts)
				{
					AssemblyChildrenEntry parent = GetParentEntry(fullName);
					String propertyName = CommonPolicies.GetPropertyName(fullName);
					AssemblyChildrenEntry removal = null;

					if (parent != null)
					{
						removal = parent.ChildrenEntries[propertyName];
					}
					else
					{
						removal = this.ChildrenEntries[propertyName];
					}

					if (removal != null)
					{
						if (parent != null)
						{
							parent.InnerList.Remove(removal);
						}
						else
						{
							this.InnerList.Remove(removal);
						}
					}
				}
			}
		}

		#endregion
	}
}