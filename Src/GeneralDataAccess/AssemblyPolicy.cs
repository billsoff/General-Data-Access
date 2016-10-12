#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AssemblyPolicy.cs
// �ļ��������������÷��룬ָʾ��μ�����ʵ�塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110726
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
	/// ���÷��룬ָʾ��μ�����ʵ�塣
	/// </summary>
	[Serializable]
	public sealed partial class AssemblyPolicy
	{
		#region ��̬��Ա

		/// <summary>
		/// ���������������
		/// </summary>
		/// <returns>��������</returns>
		internal static AssemblyPolicy CreateRoot()
		{
			return new AssemblyPolicy();
		}

		/// <summary>
		/// ���������������ָʾ�������еģ�������ӵģ���ʵ�塣
		/// </summary>
		/// <returns>��������</returns>
		internal static AssemblyPolicy CreateLoadAllChildren()
		{
			return new AssemblyPolicy(true);
		}

		#endregion

		#region ˽���ֶ�

		[NonSerialized]
		private Type m_type;

		private readonly Boolean m_loadAllChildren;
		private CompositeBuilderStrategy m_builderStrategy;

		private List<AssemblyChildrenEntry> m_innerList;
		private AssemblyChildrenEntryCollection m_childrenEntries;

		private List<AssemblyListener> m_listeners;

		// ����Ҫװ�����ʵ�����Ե�ȫ���Ƽ���
		private List<String> m_excepts;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯�������ڴ�����������
		/// </summary>
		private AssemblyPolicy()
		{
		}

		/// <summary>
		/// Ĭ�Ϲ��캯�������ڴ�����������������ָʾ�Ƿ����ȫ������ʵ�壨������ӵģ���
		/// </summary>
		/// <param name="loadAllChiren">ָʾ�Ƿ����ȫ������ʵ�壨������ӵģ���</param>
		private AssemblyPolicy(Boolean loadAllChiren)
		{
			m_loadAllChildren = loadAllChiren;
		}

		#endregion

		#region �ڲ�����

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

		/// <summary>
		/// ��ȡ���ز��ԡ�
		/// </summary>
		internal CompositeBuilderStrategy BuilderStrategy
		{
			get
			{
				return m_builderStrategy;
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
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ�ע���˼�������
		/// </summary>
		internal Boolean HasListeners
		{
			get { return (m_listeners != null) && (m_listeners.Count != 0); }
		}

		/// <summary>
		/// ��ȡװ��������б�
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
		/// ��ȡʵ�����͡�
		/// </summary>
		internal Type Type
		{
			get { return m_type; }
		}

		#endregion

		#region ˽������

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
		/// ��ȡ����Ҫװ�����ʵ�����Ե�ȫ���Ƽ��ϡ�
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
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ����װ�䷽�롣
		/// </summary>
		private Boolean HasChildrenEntries
		{
			get
			{
				return (m_innerList != null) && (m_innerList.Count != 0);
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ��в���Ҫ���ص���ʵ�塣
		/// </summary>
		private Boolean HasExcepts
		{
			get { return (m_excepts != null) && (m_excepts.Count != 0); }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ������ʵ��װ�䡣
		/// </summary>
		/// <param name="entityType">����ʵ�����͡�</param>
		/// <param name="parentEntities">����ʵ���б�</param>
		/// <param name="parentFilter">������������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
		public void Enforce(Type entityType, Object[] parentEntities, Filter parentFilter, IDatabaseSession databaseSession)
		{
			#region ���ܼ���

#if DEBUG

			String metricId = Guid.NewGuid().ToString();
			Timing.Start("�Զ�װ����ʵ�弯��", metricId);

#endif

			#endregion

			if (parentEntities.Length == 0)
			{
				#region ���ܼ���

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

				// ����ÿһ����װ����
				foreach (AssemblyChildrenEntry childrenEntry in ChildrenEntries)
				{
					childrenEntry.Enforce(parentEntities, parentFilter, databaseSession);
				}

				OnAssemblyComplete(parentEntities, parentFilter, databaseSession);
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
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>����ѡ�������ϡ�</returns>
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
		/// ����װ�䷽���Ĭ�ϼ��ز��ԡ�
		/// </summary>
		/// <param name="strategyOption">����ѡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicy Using(LoadStrategyOption strategyOption)
		{
			LoadStrategyAttribute attr = new LoadStrategyAttribute(strategyOption);
			CompositeBuilderStrategy strategy = attr.Create();

			return Using(strategy);
		}

		/// <summary>
		/// ����װ�䷽���Ĭ�ϼ��ز��ԡ�
		/// </summary>
		/// <param name="level">���ؼ���</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicy Using(Int32 level)
		{
			return Using(CompositeBuilderStrategyFactory.Create(level));
		}

		/// <summary>
		/// ����װ�䷽���Ĭ�ϼ��ز��ԣ�ע�⣬�˲��Բ��ܺ�������ء�
		/// </summary>
		/// <param name="builderStrategy">װ�䷽���Ĭ�ϼ��ز��ԡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicy Using(CompositeBuilderStrategy builderStrategy)
		{
			m_builderStrategy = builderStrategy;

			return this;
		}

		/// <summary>
		/// �ų������ԣ����� AllChildren �����á�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicy Except(IPropertyChainBuilder builder)
		{
			return Except(builder.Build());
		}

		/// <summary>
		/// �ų������ԣ����� AllChildren �����á�
		/// </summary>
		/// <param name="chain">����·����</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicy Except(IPropertyChain chain)
		{
			return Except(chain.PropertyPath);
		}

		/// <summary>
		/// �ų������ԣ����� AllChildren �����á�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>��ǰʵ����</returns>
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
		/// ע��װ���������ע�⣬Ϊ֧��Զ�̷��ʣ�������Ӧ�ǿ����л��ġ�
		/// </summary>
		/// <param name="listeners">���������ϡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicy Register(params AssemblyListener[] listeners)
		{
			this.Listeners.AddRange(listeners);

			return this;
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ��װ���
		/// </summary>
		/// <param name="fullName">����ȫ���ơ�</param>
		/// <returns>��װ���</returns>
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

		#region ��������

		/// <summary>
		/// �������е���װ���
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
		/// ͨ���������ʼװ�䡣
		/// </summary>
		/// <param name="parentEntities">��ʵ�弯�ϡ�</param>
		/// <param name="filter">����������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
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
		/// ֪ͨ���������װ�䡣
		/// </summary>
		/// <param name="parentEntities">��ʵ�弯�ϡ�</param>
		/// <param name="filter">����������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
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
		/// �Ƴ����в���Ҫ���ص���ʵ��װ���
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