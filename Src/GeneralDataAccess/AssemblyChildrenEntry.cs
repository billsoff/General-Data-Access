#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AssemblyChildrenEntry.cs
// �ļ�������������ʵ��װ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110729
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʵ��װ���
	/// </summary>
	[Serializable]
	internal sealed class AssemblyChildrenEntry
	{
		#region ˽���ֶ�

		private readonly AssemblyPolicy m_assemblyPolicy;
		private readonly AssemblyChildrenEntry m_parent;

		private readonly String m_propertyName;
		private readonly CompositeBuilderStrategy m_builderStrategy;

		private List<AssemblyChildrenEntry> m_innerList;
		private AssemblyChildrenEntryCollection m_childrenEntries;

		private IPropertyChain m_propertyChain;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������װ�䷽�롢��װ����������ƺͼ��ز��ԡ�
		/// </summary>
		/// <param name="policy">װ�䷽�롣</param>
		/// <param name="parent">��װ���</param>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="builderStrategy">���ز��ԡ�</param>
		public AssemblyChildrenEntry(AssemblyPolicy policy, AssemblyChildrenEntry parent, String propertyName, CompositeBuilderStrategy builderStrategy)
		{
			m_assemblyPolicy = policy;
			m_parent = parent;
			m_propertyName = propertyName;
			m_builderStrategy = builderStrategy;
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ�����Զ��塣
		/// </summary>
		internal EntityPropertyDefinition ChildrenProperty
		{
			get
			{
				return Property.ChildrenProperty;
			}
		}

		/// <summary>
		/// ��ȡװ�䷽����ڲ��б�
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
		/// ��ȡ��װ��ָ�롣
		/// </summary>
		internal AssemblyChildrenEntry Parent
		{
			get { return m_parent; }
		}

		/// <summary>
		/// ��ȡ��װ�������������
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
		/// ��ȡ���������ơ�
		/// </summary>
		internal String PropertyName
		{
			get { return m_propertyName; }
		}

		/// <summary>
		/// ��ȡ������װ�䷽���б�
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

		#region ˽������

		/// <summary>
		/// ��ȡ���ط��롣
		/// </summary>
		private AssemblyPolicy AssemblyPolicy
		{
			get { return m_assemblyPolicy; }
		}

		/// <summary>
		/// ��ȡ���ز��ԡ�
		/// </summary>
		private CompositeBuilderStrategy BuilderStrategy
		{
			get
			{
				return m_builderStrategy ?? AssemblyPolicy.BuilderStrategy;
			}
		}

		/// <summary>
		/// ��ȡ��ʵ�嶨�塣
		/// </summary>
		private EntityDefinition ChildrenDefinition
		{
			get
			{
				return ChildrenProperty.Entity;
			}
		}

		/// <summary>
		/// ��ȡ�����Թ�������
		/// </summary>
		private Sorter ChildrenSorter
		{
			get
			{
				return Property.ChildrenSorter;
			}
		}

		/// <summary>
		/// ��ȡ��ʵ�����͡�
		/// </summary>
		private Type ChildrenType
		{
			get
			{
				return ChildrenDefinition.Type;
			}
		}

		/// <summary>
		/// ��ȡ��ʵ�嶨�塣
		/// </summary>
		private EntityDefinition Definition
		{
			get
			{
				return EntityDefinitionBuilder.Build(Type);
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ����ʵ��װ���
		/// </summary>
		private Boolean HasChildrenEntries
		{
			get
			{
				return (m_innerList != null) && (m_innerList.Count != 0);
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ�ע���˼�������
		/// </summary>
		internal Boolean HasListeners
		{
			get { return AssemblyPolicy.HasListeners; }
		}

		/// <summary>
		/// ��ȡװ��������б�
		/// </summary>
		internal IList<AssemblyListener> Listeners
		{
			get
			{
				return AssemblyPolicy.Listeners;
			}
		}

		/// <summary>
		/// ��ȡ�ڸ�ʵ���е����Զ��塣
		/// </summary>
		private EntityPropertyDefinition Property
		{
			get
			{
				return Definition.Properties[PropertyName];
			}
		}

		/// <summary>
		/// ��ȡʵ�����͡�
		/// </summary>
		private Type Type
		{
			get { return (Parent != null) ? Parent.ChildrenType : AssemblyPolicy.Type; }
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// �������е���ʵ��װ���
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
		/// ������ʵ��װ�䡣
		/// </summary>
		/// <param name="parentEntities">����ʵ���б�</param>
		/// <param name="parentFilter">������������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
		internal void Enforce(Object[] parentEntities, Filter parentFilter, IDatabaseSession databaseSession)
		{
			#region ���ܼ���

#if DEBUG

			String metricId = Guid.NewGuid().ToString();
			Timing.Start("�Զ�װ����ʵ�弯��", metricId);

#endif

			#endregion

			if (parentEntities.Length != 0)
			{
				// ��ʵ�������
				Filter childFilter = Transform(parentFilter);

				OnChildrenAssemblyStart(parentEntities, childFilter, databaseSession);

				// ������ʵ��
				Object[] childEntities = Load(childFilter, databaseSession);

				// װ��
				Assembly(parentEntities, childEntities);

				if (childEntities.Length != 0)
				{
					OnChildrenAssemblyComplete(parentEntities, childEntities, childFilter, databaseSession);
				}

				if (HasChildrenEntries)
				{
					// ����ÿһ����װ����
					foreach (AssemblyChildrenEntry childrenEntry in ChildrenEntries)
					{
						childrenEntry.Enforce(childEntities, childFilter, databaseSession);
					}
				}
			}

			#region ���ܼ���

#if DEBUG

			Timing.Stop(metricId);

#endif

			#endregion
		}

		/// <summary>
		/// ��ȡ��Ҫװ��������������������ѡ������
		/// </summary>
		/// <returns>����ѡ�������ϡ�</returns>
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

		#region ��������

		/// <summary>
		/// ����װ�������
		/// </summary>
		/// <param name="parentEntities">��ʵ�弯�ϡ�</param>
		/// <param name="childEntities">��ʵ�弯�ϡ�</param>
		private void Assembly(Object[] parentEntities, Object[] childEntities)
		{
			ColumnDefinition[] childColumns = ChildrenProperty.Relation.ChildColumns;
			ColumnDefinition[] parentColumns = ChildrenProperty.Relation.ParentColumns;

			foreach (Object parent in parentEntities)
			{
				Object[] children = FindChildren(parent, childEntities, childColumns, parentColumns);

				// Ϊ��װ����ʵ�塣
				Property.PropertyInfo.SetValue(parent, children, null);

				// ��ÿһ����ʵ��ĸ������û�
				foreach (Object child in children)
				{
					ChildrenProperty.PropertyInfo.SetValue(child, parent, null);
				}
			}
		}

		/// <summary>
		/// Ϊ��ǰ���ط��봴�����ʵĲ��ԡ�
		/// </summary>
		/// <param name="builderStrategy">���ز��ԡ�</param>
		/// <returns>�����õĺ��ʵĲ��ԡ�</returns>
		private CompositeBuilderStrategy ComposeBuilderStrategy(CompositeBuilderStrategy builderStrategy)
		{
			List<PropertySelector> additional = new List<PropertySelector>();

			// ��֤ѡ����������
			additional.Add(
					PropertySelector.Create(
							new PropertyChain(
									ChildrenType,
									new String[] { ChildrenProperty.Name }
								)
						)
				);

			// ��֤ѡ������ʵ�������������
			additional.AddRange(GetAllSelectors());

			// ��������
			CompositeBuilderStrategy strategy = CompositeBuilderStrategyFactory.Compose(ChildrenType, builderStrategy, additional);

			ChildrenRedundantPropertyTrimmer trimmer = new ChildrenRedundantPropertyTrimmer(ChildrenProperty);

			strategy = CompositeBuilderStrategyFactory.TrimOff(strategy, trimmer);
			strategy.AlwaysSelectPrimaryKeyProperties = false;

			return strategy;
		}

		/// <summary>
		/// ������ʵ�弯�ϡ�
		/// </summary>
		/// <param name="parent">��ʵ�塣</param>
		/// <param name="childEntities">���е���ʵ�弯�ϡ�</param>
		/// <param name="childColumns">���ж��弯�ϡ�</param>
		/// <param name="parentColumns">���ж��弯�ϡ�</param>
		/// <returns>�����ڸ�ʵ�����ʵ�弯�ϡ�</returns>
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
		/// �ж���ʵ��������Ƿ����ظ���
		/// </summary>
		/// <param name="childEntry">��װ���</param>
		/// <returns>������ظ����򷵻� true�����򷵻� false��</returns>
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
		/// ��ȡ��ʵ�弯�ϡ�
		/// </summary>
		/// <param name="filter">��������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
		/// <returns>��ʵ�弯�ϡ�</returns>
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
		/// ֪ͨ��������ʵ�忪ʼװ�䡣
		/// </summary>
		/// <param name="parentEntities">��ʵ�弯�ϡ�</param>
		/// <param name="filter">��������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
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
		/// ֪ͨ��������ʵ��װ����ɡ�
		/// </summary>
		/// <param name="parentEntities">��ʵ�弯�ϡ�</param>
		/// <param name="children">װ��õ���ʵ�弯�ϡ�</param>
		/// <param name="filter">��������</param>
		/// <param name="databaseSeesion">���ݿ�Ự���档</param>
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
		/// ������������ת��Ϊ��ʵ�����������
		/// </summary>
		/// <param name="parentFilter">������������</param>
		/// <returns>��ʵ�����������</returns>
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


		#region �����޼���

		/// <summary>
		/// �޼�����ʵ���г�������������������ԡ�
		/// </summary>
		private sealed class ChildrenRedundantPropertyTrimmer : PropertyTrimmer
		{
			#region ˽���ֶ�

			private readonly IPropertyChain m_schema;
			private readonly IPropertyChain[] m_parentProperties;

			#endregion

			#region ���캯��

			/// <summary>
			/// ���캯����������ʵ�����Զ��塣
			/// </summary>
			/// <param name="childrenProperty">��ʵ�����Զ��塣</param>
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
			/// �޼�����ʵ���ж�������ԡ�
			/// </summary>
			/// <param name="property">Ҫ�жϵ����ԡ�</param>
			/// <returns>����޼������ԣ��򷵻� true�����򷵻� false��</returns>
			public override Boolean TrimOff(EntityProperty property)
			{
				return m_schema.Contains(property)
					&& !m_schema.Equals(property.PropertyChain)
					&& (Array.IndexOf<IPropertyChain>(m_parentProperties, property.PropertyChain) != -1); // ��Ҫװ���Ŀ�길ʵ��
			}

			/// <summary>
			/// ��ȡ��ʾ���ƣ����ڵ��ԡ�
			/// </summary>
			public override String DisplayName
			{
				get
				{
					return "�޼�����ʵ��ܹ��еķ���������";
				}
			}
		}

		#endregion
	}
}