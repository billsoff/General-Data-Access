#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����DbEntityPropertyInfo.cs
// �ļ�����������ʵ�����԰���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110406
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ʵ�����԰���
	/// </summary>
	internal class DbEntityPropertyInfo
	{
		#region ��̬����

		/// <summary>
		/// ����ʵ��������Ϣ��
		/// </summary>
		/// <typeparam name="TEntity">ʵ�����͡�</typeparam>
		/// <returns>�����͵�������Ϣ��</returns>
		public static DbEntityPropertyInfo Create<TEntity>() where TEntity : EtyBusinessObject
		{
			return new DbEntityPropertyInfo(typeof(TEntity));
		}

		/// <summary>
		/// ����ʵ��������Ϣ��
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>�����͵�������Ϣ��</returns>
		public static DbEntityPropertyInfo Create(Type entityType)
		{
			return new DbEntityPropertyInfo(entityType);
		}

		#endregion

		#region ˽���ֶ�

		private readonly Type m_type;

		// �޲ι�����
		private readonly ConstructorInfo m_constructor;

		private readonly Boolean m_isBusinessObject;

		// ��ֵ�����б��Ϊ�����Ĳ���
		private readonly PropertyInfo[] m_primaryKeys;

		// ��ֵ�����з������Ĳ���
		private readonly PropertyInfo[] m_values;

		// �����Լ��ϣ��������е�ֵ����
		private readonly PropertyInfo[] m_simples;

		// ���ò���
		private readonly PropertyInfo[] m_references;

		// ��ʵ�弯���������ƣ�ת��Ϊȫ����д
		private readonly String[] m_children;

		private Dictionary<String, PropertyInfo> m_childrenProperties;
		private Dictionary<String, Type> m_childrenElementTypes;
		private Dictionary<String, PropertyInfo> m_childrenElementProperties;
		private Dictionary<String, ChildrenAttribute> m_childrenAttributes;
		private Dictionary<String, Sorter> m_childrenSorters;

		// ����������������������ֵ�Ǵӵ�ǰʵ�忪ʼ�� PropertyInfo ��
		private Dictionary<String, PropertyInfo[]> m_primaryKeyChains = new Dictionary<String, PropertyInfo[]>();

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯����
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		private DbEntityPropertyInfo(Type entityType)
		{
			List<PropertyInfo> primaryKeys = new List<PropertyInfo>();
			List<PropertyInfo> values = new List<PropertyInfo>();
			List<PropertyInfo> references = new List<PropertyInfo>();
			List<String> childrenNames = new List<String>();

			foreach (PropertyInfo pf in entityType.GetProperties(CommonPolicies.PropertyBindingFlags))
			{
				if (Attribute.IsDefined(pf, typeof(ColumnAttribute)))
				{
					ColumnAttribute columnAttr = (ColumnAttribute)Attribute.GetCustomAttribute(pf, typeof(ColumnAttribute));

					if (columnAttr.IsPrimaryKey)
					{
						primaryKeys.Add(pf);
					}
					else
					{
						values.Add(pf);
					}
				}
				else if (Attribute.IsDefined(pf, typeof(ForeignReferenceAttribute)))
				{
					references.Add(pf);
				}
				else if (Attribute.IsDefined(pf, typeof(ChildrenAttribute)))
				{
					ChildrenAttribute childrenAttr = (ChildrenAttribute)Attribute.GetCustomAttribute(pf, typeof(ChildrenAttribute));

					if (!pf.PropertyType.IsArray)
					{
						throw new InvalidOperationException(
								String.Format(
										"������ {0} �б�� ChildrenAttribute ������ {1} �����������͡�",
										entityType.FullName,
										pf.Name
									)
							);
					}

					String name = pf.Name;
					Sorter s = OrderByAttribute.ComposeSorter(pf);
					Type elementType = pf.PropertyType.GetElementType();

					PropertyInfo childProperty = elementType.GetProperty(
							childrenAttr.PropertyName,
							CommonPolicies.PropertyBindingFlags
						);

					#region ��֤

					if (childProperty == null)
					{
						throw new InvalidOperationException(String.Format(
									"�� {0} ����������ʵ�弯�� {1}���� {2} ���в��������� {3}��",
									entityType.FullName,
									pf.PropertyType.FullName,
									elementType.FullName,
									childrenAttr.PropertyName
								)
							);
					}

					if (childProperty.PropertyType != entityType)
					{
						throw new InvalidOperationException(String.Format(
									"�� {0} ����������ʵ�弯�� {1} �У�{2} ���е����� {3} ���� {0} ���͡�",
									entityType.FullName,
									pf.PropertyType.FullName,
									elementType.FullName,
									childrenAttr.PropertyName
								)
							);
					}

					#endregion

					childrenNames.Add(name);
					ChildrenProperties.Add(name, pf);
					ChildrenElementTypes.Add(name, elementType);
					ChildrenElementProperties.Add(name, childProperty);
					ChildrenAttributes.Add(name, childrenAttr);
					ChildrenSorters.Add(name, s);
				}

				m_type = entityType;

				#region �޲ι�������

				m_constructor = entityType.GetConstructor(Type.EmptyTypes);

				m_isBusinessObject = typeof(EtyBusinessObject).IsAssignableFrom(entityType);

				#endregion

				m_primaryKeys = primaryKeys.ToArray();
				m_values = values.ToArray();
				m_references = references.ToArray();
				m_children = childrenNames.ToArray();

				m_simples = new PropertyInfo[m_primaryKeys.Length + m_values.Length];

				Array.Copy(m_primaryKeys, 0, m_simples, 0, m_primaryKeys.Length);
				Array.Copy(m_values, 0, m_simples, m_primaryKeys.Length, m_values.Length);
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡʵ�����͡�
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		/// <summary>
		/// ��ȡ�޲ι�������
		/// </summary>
		public ConstructorInfo Constructor
		{
			get { return m_constructor; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾʵ�������Ƿ�Ϊ�� EtyBusinessObject ֱ�ӻ���������
		/// </summary>
		public Boolean IsBusinessObject
		{
			get { return m_isBusinessObject; }
		}

		/// <summary>
		/// ��ȡ�������Լ��ϡ�
		/// </summary>
		public PropertyInfo[] PrimaryKeys
		{
			get { return m_primaryKeys; }
		}

		/// <summary>
		/// ��ȡֵ���Լ��ϣ�ֻ������������ֵ���ԡ�
		/// </summary>
		public PropertyInfo[] Values
		{
			get { return m_values; }
		}

		/// <summary>
		/// ��ȡֵ���Լ��ϣ����������ͷ�������ֵ���ԡ�
		/// </summary>
		public PropertyInfo[] Primitives
		{
			get { return m_simples; }
		}

		/// <summary>
		/// ��ȡ�������Լ��ϡ�
		/// </summary>
		public PropertyInfo[] References
		{
			get { return m_references; }
		}

		/// <summary>
		/// ��ȡ�������������ϣ���Ϊ������ֵΪ��������
		/// </summary>
		public Dictionary<String, PropertyInfo[]> PrimaryKeyChains
		{
			get { return m_primaryKeyChains; }
		}

		/// <summary>
		/// ��ȡ���е��������ƣ�ȫ��д��ʽ����
		/// </summary>
		public String[] Children
		{
			get { return m_children; }
		}

		/// <summary>
		/// ��ȡ��ʵ�弯�����ԡ�
		/// </summary>
		public Dictionary<String, PropertyInfo> ChildrenProperties
		{
			get
			{
				if (m_childrenProperties == null)
				{
					m_childrenProperties = new Dictionary<String, PropertyInfo>();
				}

				return m_childrenProperties;
			}
		}

		/// <summary>
		/// ���༯��Ԫ�����ͼ��ϣ������������ƣ�ת��Ϊȫ����д��
		/// </summary>
		public Dictionary<String, Type> ChildrenElementTypes
		{
			get
			{
				if (m_childrenElementTypes == null)
				{
					m_childrenElementTypes = new Dictionary<String, Type>();
				}

				return m_childrenElementTypes;
			}
		}

		/// <summary>
		/// ��ȡ���༯��ָ�򸸵�������Ϣ���ϡ�
		/// </summary>
		public Dictionary<String, PropertyInfo> ChildrenElementProperties
		{
			get
			{
				if (m_childrenElementProperties == null)
				{
					m_childrenElementProperties = new Dictionary<String, PropertyInfo>();
				}

				return m_childrenElementProperties;
			}
		}

		/// <summary>
		/// ���༯�� ChildrenAttribute ��Ǽ��ϣ������������ƣ�ת��Ϊȫ����д��
		/// </summary>
		public Dictionary<String, ChildrenAttribute> ChildrenAttributes
		{
			get
			{
				if (m_childrenAttributes == null)
				{
					m_childrenAttributes = new Dictionary<String, ChildrenAttribute>();
				}

				return m_childrenAttributes;
			}
		}

		/// <summary>
		/// ���༯�����������ϣ������������ƣ��ֻ�Ϊȫ����д��
		/// </summary>
		public Dictionary<String, Sorter> ChildrenSorters
		{
			get
			{
				if (m_childrenSorters == null)
				{
					m_childrenSorters = new Dictionary<String, Sorter>();
				}

				return m_childrenSorters;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// Ϊ EtyBusinessObject �������ݿ�Ự�������
		/// </summary>
		/// <param name="businessObject">ʵ�塣</param>
		/// <param name="databaseSession">���ݿ�Ự�������</param>
		public void AttachDatabaseSession(EtyBusinessObject businessObject, IDatabaseSession databaseSession)
		{
			// �����ǰ�ĻỰ�����ã�����ֹ
			if (Object.ReferenceEquals(businessObject.DatabaseSession, databaseSession))
			{
				return;
			}

			businessObject.DatabaseSession = databaseSession;

			using (EtyBusinessObject.SuppressEntityLazyLoadTransient(businessObject))
			{
				// ��Ϊ���еĸ�����������
				foreach (PropertyInfo pf in References)
				{
					Object entiy = pf.GetValue(businessObject, null);

					EtyBusinessObject foreignBO = entiy as EtyBusinessObject;

					if (foreignBO != null)
					{
						DbEntityPropertyInfo info = DbEntityPropertyInfoCache.GetProperty(pf.PropertyType);

						info.AttachDatabaseSession(foreignBO, databaseSession);
					}
				}

				// Ϊ���е��Ӽ���������
				foreach (String name in Children)
				{
					if (!businessObject.IsChildrenLoaded(name))
					{
						continue;
					}

					Type childElementType = ChildrenElementTypes[name];
					DbEntityPropertyInfo info = DbEntityPropertyInfoCache.GetProperty(childElementType);

					if (!info.IsBusinessObject)
					{
						continue;
					}

					PropertyInfo pf = ChildrenProperties[name];

					EtyBusinessObject[] childEntities = (EtyBusinessObject[])pf.GetValue(businessObject, null);

					foreach (EtyBusinessObject bo in childEntities)
					{
						info.AttachDatabaseSession(bo, databaseSession);
					}
				}
			} // Restore business object SuppressLazyLoad property value
		}

		/// <summary>
		/// �𼶽���ֵ���ƣ���������������
		/// </summary>
		/// <param name="source">Դ��</param>
		/// <param name="destination">Ŀ�ꡣ</param>
		public void Copy(Object source, Object destination)
		{
			Boolean isSourcePartially = IsEntityPartially(source);

			// ����Դʵ������ʱ����
			if (isSourcePartially)
			{
				return;
			}

			using (EtyBusinessObject.SuppressEntityLazyLoadTransient(source))
			{
				// ֵ����
				foreach (PropertyInfo pf in Values)
				{
					// ����δ���ص��ӳ�����
					if (!EtyBusinessObject.IsEmpty(destination, pf.Name))
					{
						Object sourceValue = pf.GetValue(source, null);

						pf.SetValue(destination, sourceValue, null);
					}
				}

				if (References.Length == 0)
				{
					return;
				}

				using (EtyBusinessObject.SuppressEntityLazyLoadTransient(destination))
				{
					// ��������
					foreach (PropertyInfo pf in References)
					{
						Object sourceValue = pf.GetValue(source, null);

						Object destinationValue = pf.GetValue(destination, null);

						if (destinationValue == null)
						{
							pf.SetValue(destination, sourceValue, null);
						}
						else if (sourceValue != null)
						{
							DbEntityPropertyInfo info = DbEntityPropertyInfoCache.GetProperty(pf.PropertyType);

							info.Copy(sourceValue, destinationValue);
						}
					}
				} // Restore destination SuppressLazyLoad property value
			} // Restore source SuppressLazyLoad property value
		}

		/// <summary>
		/// ��ȡ�������ƣ�������ڣ���ת��Ϊȫ����д�������׳��쳣��
		/// </summary>
		/// <param name="propertyName">Ҫ���ҵ��������ơ�</param>
		/// <returns>����������ȫ����д����ʽ��</returns>
		public String GetChildPropertyName(String propertyName)
		{
			Boolean exists = Array.Exists<String>(
					Children,
					delegate(String childName) { return childName.Equals(propertyName, CommonPolicies.PropertyNameComparison); }
				);

			if (!exists)
			{
				throw new InvalidOperationException(String.Format("���� {0} �в��������� {1}��", Type.FullName, propertyName));
			}

			return propertyName;
		}

		/// <summary>
		/// ���¼��ص�ǰʵ�壬�������ӳټ������ԡ�
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="databaseSession">���ݿ�Ự��</param>
		/// <returns>��ʵ�塣</returns>
		public Object Load(Object entity, IDatabaseSession databaseSession)
		{
			return Load(entity, databaseSession, false);
		}

		/// <summary>
		/// ���¼��ص�ǰʵ�壬��ָ���Ƿ�����ӳټ������ԡ�
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="databaseSession">���ݿ�Ự��</param>
		/// <param name="includingLazyLoads">ָʾ�Ƿ�����ӳ����ԡ�</param>
		/// <returns>��ʵ�塣</returns>
		public Object Load(Object entity, IDatabaseSession databaseSession, Boolean includingLazyLoads)
		{
			// ��������
			CompositeBuilderStrategy loadStrategy;

			if (includingLazyLoads)
			{
				loadStrategy = Select.AllFrom(Type);
			}
			else
			{
				loadStrategy = Select.AllExceptLazyLoadFrom(Type);
			}

			EntityDefinition definition = EntityDefinitionBuilder.Build(Type);
			Filter f = definition.ComposeLoadFilter(entity);

			Object newEntity = databaseSession.LoadFirst(
					Type,
					CompositeBuilderStrategyFactory.Compose(Type, loadStrategy, null),
					f
				);

			Debug.WriteLine(CompositeBuilderStrategyFactory.Compose(Type, loadStrategy, null).Dump());

			return newEntity;
		}

		/// <summary>
		/// ������ʵ�弯�ϡ�
		/// </summary>
		/// <param name="propertyName">��ʵ�弯���������ơ�</param>
		/// <param name="parentEntity">��ʵ�塣</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
		/// <returns>��ʵ�弯�ϡ�</returns>
		public Object[] LoadChildren(String propertyName, Object parentEntity, IDatabaseSession databaseSession)
		{
			String name = GetChildPropertyName(propertyName);

			Type elementType = ChildrenElementTypes[name];
			PropertyInfo elementPropertyInfo = ChildrenElementProperties[name];
			ChildrenAttribute childrenAttr = ChildrenAttributes[name];
			Sorter s = ChildrenSorters[name];

			Filter f = childrenAttr.CreateFilter(parentEntity);

			Object[] children = databaseSession.Load(elementType, f, s);

			// ת����ʵ��
			foreach (Object child in children)
			{
				elementPropertyInfo.SetValue(child, parentEntity, null);
			}

			return children;
		}

		/// <summary>
		/// ��ʼ��������������������
		/// </summary>
		public void Initialize()
		{
			foreach (PropertyInfo pf in m_primaryKeys)
			{
				ColumnAttribute columnAttr = (ColumnAttribute)Attribute.GetCustomAttribute(pf, typeof(ColumnAttribute));

				PrimaryKeyChains.Add(columnAttr.Name, new PropertyInfo[] { pf });
			}

			foreach (PropertyInfo pf in m_references)
			{
				if (Attribute.IsDefined(pf, typeof(PrimaryKeyAttribute)))
				{
					ColumnMappingAttribute[] allMappings = (ColumnMappingAttribute[])Attribute.GetCustomAttributes(pf, typeof(ColumnMappingAttribute));

					foreach (ColumnMappingAttribute mapping in allMappings)
					{
						PropertyInfo[] chain = GetPropertyChain(mapping.ChildColumnName, mapping.ParentColumnName, pf);

						PrimaryKeyChains.Add(mapping.ChildColumnName, chain);
					}
				}
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����������������������Сд��
		/// </summary>
		/// <param name="chain">������</param>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <param name="columnName">������</param>
		private static void ComposePropertyChain(List<PropertyInfo> chain, Type entityType, String columnName)
		{
			DbEntityPropertyInfo target = DbEntityPropertyInfoCache.GetProperty(entityType);

			PropertyInfo simple = target.GetValuePropertyInfo(columnName);

			if (simple != null)
			{
				chain.Add(simple);

				// �ݹ����
				return;
			}

			Boolean found = false;

			foreach (PropertyInfo pf in target.References)
			{
				ColumnMappingAttribute[] allMappings = (ColumnMappingAttribute[])Attribute.GetCustomAttributes(pf, typeof(ColumnMappingAttribute));

				foreach (ColumnMappingAttribute mapping in allMappings)
				{
					if (mapping.ChildColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase))
					{
						chain.Add(pf);

						// �ݹ����
						ComposePropertyChain(chain, pf.PropertyType, mapping.ParentColumnName);

						found = true;

						break;
					}
				}
			}

			// ���������û���ҵ����򱨴�
			if (!found)
			{
				String[] typeNames = chain.ConvertAll<String>(
						delegate(PropertyInfo pf) { return pf.PropertyType.FullName; }
					)
					.ToArray();

				String path = String.Join(" -> ", typeNames);

				throw new InvalidOperationException(
						String.Format("������������ {0} ������û���ҵ��� {1} ����������·��Ϊ {2} ��",
								entityType.FullName,
								columnName,
								path
							)
					);
			}
		}

		/// <summary>
		/// ��ȡ����������
		/// </summary>
		/// <param name="columName">������</param>
		/// <param name="parentColumnName">��������</param>
		/// <param name="currentPropertyInfo">��ǰ���ԡ�</param>
		/// <returns></returns>
		private static PropertyInfo[] GetPropertyChain(String columName, String parentColumnName, PropertyInfo currentPropertyInfo)
		{
			List<PropertyInfo> chain = new List<PropertyInfo>();

			chain.Add(currentPropertyInfo);

			ComposePropertyChain(chain, currentPropertyInfo.PropertyType, parentColumnName);

			return chain.ToArray();
		}

		/// <summary>
		/// ��ȡʵ�������Եļ�ֵ��
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="chain">�����������һ�������Ǽ����ԡ�</param>
		/// <returns>����ֵ��</returns>
		private static Object GetValue(Object entity, PropertyInfo[] chain)
		{
			Object value = entity;

			foreach (PropertyInfo pf in chain)
			{
				if ((value != null) && !EtyBusinessObject.IsEmpty(value, pf.Name) && !Attribute.IsDefined(pf, typeof(LazyLoadAttribute)))
				{
					value = pf.GetValue(value, null);

					Debug.WriteLine(value);
				}
				else
				{
					break;
				}
			}

			return value;
		}

		/// <summary>
		/// ��ȡ��������ӳ���ֵ���ԣ����Դ�Сд��
		/// </summary>
		/// <param name="columnName">������</param>
		/// <returns>�����ӳ���ֵ���ԡ����û����ֵ������ӳ������������п��������������У����򷵻ؿա�</returns>
		private PropertyInfo GetValuePropertyInfo(String columnName)
		{
			foreach (PropertyInfo pf in Primitives)
			{
				ColumnAttribute columnAttr = (ColumnAttribute)Attribute.GetCustomAttribute(pf, typeof(ColumnAttribute));

				if (columnAttr.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
				{
					return pf;
				}
			}

			return null;
		}

		/// <summary>
		/// �ж�ʵ���Ƿ�Ϊ���ֹ���ġ�
		/// </summary>
		/// <param name="entity">Ҫ�жϵ�ʵ�塣</param>
		/// <returns>���ʵ��Ϊ���ֹ���ģ��򷵻� true�����򷵻� false��</returns>
		private static Boolean IsEntityPartially(Object entity)
		{
			EtyBusinessObject bo = entity as EtyBusinessObject;

			if (bo != null)
			{
				return bo.IsPartially;
			}
			else
			{
				// ��֧�ֲ��ֹ���
				return false;
			}
		}

		#endregion
	}
}