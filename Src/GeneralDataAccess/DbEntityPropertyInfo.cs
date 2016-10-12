#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DbEntityPropertyInfo.cs
// 文件功能描述：实体属性包。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110406
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 实体属性包。
	/// </summary>
	internal class DbEntityPropertyInfo
	{
		#region 静态方法

		/// <summary>
		/// 创建实体属性信息。
		/// </summary>
		/// <typeparam name="TEntity">实体类型。</typeparam>
		/// <returns>该类型的属性信息。</returns>
		public static DbEntityPropertyInfo Create<TEntity>() where TEntity : EtyBusinessObject
		{
			return new DbEntityPropertyInfo(typeof(TEntity));
		}

		/// <summary>
		/// 创建实体属性信息。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>该类型的属性信息。</returns>
		public static DbEntityPropertyInfo Create(Type entityType)
		{
			return new DbEntityPropertyInfo(entityType);
		}

		#endregion

		#region 私有字段

		private readonly Type m_type;

		// 无参构造器
		private readonly ConstructorInfo m_constructor;

		private readonly Boolean m_isBusinessObject;

		// 在值属性中标记为主键的部分
		private readonly PropertyInfo[] m_primaryKeys;

		// 在值属性中非主键的部分
		private readonly PropertyInfo[] m_values;

		// 简单属性集合，包括所有的值属性
		private readonly PropertyInfo[] m_simples;

		// 引用部分
		private readonly PropertyInfo[] m_references;

		// 子实体集合属性名称，转换为全部大写
		private readonly String[] m_children;

		private Dictionary<String, PropertyInfo> m_childrenProperties;
		private Dictionary<String, Type> m_childrenElementTypes;
		private Dictionary<String, PropertyInfo> m_childrenElementProperties;
		private Dictionary<String, ChildrenAttribute> m_childrenAttributes;
		private Dictionary<String, Sorter> m_childrenSorters;

		// 主键连接链，键是列名，值是从当前实体开始的 PropertyInfo 链
		private Dictionary<String, PropertyInfo[]> m_primaryKeyChains = new Dictionary<String, PropertyInfo[]>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
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
										"在类型 {0} 中标记 ChildrenAttribute 的属性 {1} 不是数组类型。",
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

					#region 验证

					if (childProperty == null)
					{
						throw new InvalidOperationException(String.Format(
									"在 {0} 中声明了子实体集合 {1}，但 {2} 类中不存在属性 {3}。",
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
									"在 {0} 中声明的子实体集合 {1} 中，{2} 类中的属性 {3} 不是 {0} 类型。",
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

				#region 无参构造器等

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

		#region 公共属性

		/// <summary>
		/// 获取实体类型。
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		/// <summary>
		/// 获取无参构造器。
		/// </summary>
		public ConstructorInfo Constructor
		{
			get { return m_constructor; }
		}

		/// <summary>
		/// 获取一个值，该值指示实体类型是否为从 EtyBusinessObject 直接或间接派生。
		/// </summary>
		public Boolean IsBusinessObject
		{
			get { return m_isBusinessObject; }
		}

		/// <summary>
		/// 获取主键属性集合。
		/// </summary>
		public PropertyInfo[] PrimaryKeys
		{
			get { return m_primaryKeys; }
		}

		/// <summary>
		/// 获取值属性集合，只包含非主键的值属性。
		/// </summary>
		public PropertyInfo[] Values
		{
			get { return m_values; }
		}

		/// <summary>
		/// 获取值属性集合，包含主键和非主键的值属性。
		/// </summary>
		public PropertyInfo[] Primitives
		{
			get { return m_simples; }
		}

		/// <summary>
		/// 获取引用属性集合。
		/// </summary>
		public PropertyInfo[] References
		{
			get { return m_references; }
		}

		/// <summary>
		/// 获取主键属性链集合，键为列名，值为属性链。
		/// </summary>
		public Dictionary<String, PropertyInfo[]> PrimaryKeyChains
		{
			get { return m_primaryKeyChains; }
		}

		/// <summary>
		/// 获取所有的属性名称（全大写形式）。
		/// </summary>
		public String[] Children
		{
			get { return m_children; }
		}

		/// <summary>
		/// 获取子实体集合属性。
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
		/// 子类集合元素类型集合，键是属性名称，转换为全部大写。
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
		/// 获取子类集合指向父的属性信息集合。
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
		/// 子类集合 ChildrenAttribute 标记集合，键是属性名称，转换为全部大写。
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
		/// 子类集合排序器集合，键是属性名称，轮换为全部大写。
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

		#region 公共方法

		/// <summary>
		/// 为 EtyBusinessObject 附加数据库会话引擎对象。
		/// </summary>
		/// <param name="businessObject">实体。</param>
		/// <param name="databaseSession">数据库会话引擎对象。</param>
		public void AttachDatabaseSession(EtyBusinessObject businessObject, IDatabaseSession databaseSession)
		{
			// 如果当前的会话已设置，则中止
			if (Object.ReferenceEquals(businessObject.DatabaseSession, databaseSession))
			{
				return;
			}

			businessObject.DatabaseSession = databaseSession;

			using (EtyBusinessObject.SuppressEntityLazyLoadTransient(businessObject))
			{
				// 先为所有的父级引用设置
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

				// 为所有的子级引用设置
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
		/// 逐级进行值复制（不包括主键）。
		/// </summary>
		/// <param name="source">源。</param>
		/// <param name="destination">目标。</param>
		public void Copy(Object source, Object destination)
		{
			Boolean isSourcePartially = IsEntityPartially(source);

			// 仅当源实体完整时复制
			if (isSourcePartially)
			{
				return;
			}

			using (EtyBusinessObject.SuppressEntityLazyLoadTransient(source))
			{
				// 值属性
				foreach (PropertyInfo pf in Values)
				{
					// 跳过未加载的延迟属性
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
					// 引用属性
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
		/// 获取属性名称，如其存在，则转换为全部大写，否则抛出异常。
		/// </summary>
		/// <param name="propertyName">要查找的属性名称。</param>
		/// <returns>该属性名称全部大写的形式。</returns>
		public String GetChildPropertyName(String propertyName)
		{
			Boolean exists = Array.Exists<String>(
					Children,
					delegate(String childName) { return childName.Equals(propertyName, CommonPolicies.PropertyNameComparison); }
				);

			if (!exists)
			{
				throw new InvalidOperationException(String.Format("类型 {0} 中不存在属性 {1}。", Type.FullName, propertyName));
			}

			return propertyName;
		}

		/// <summary>
		/// 重新加载当前实体，不包含延迟加载属性。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="databaseSession">数据库会话。</param>
		/// <returns>新实体。</returns>
		public Object Load(Object entity, IDatabaseSession databaseSession)
		{
			return Load(entity, databaseSession, false);
		}

		/// <summary>
		/// 重新加载当前实体，可指定是否包含延迟加载属性。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="databaseSession">数据库会话。</param>
		/// <param name="includingLazyLoads">指示是否加载延迟属性。</param>
		/// <returns>新实体。</returns>
		public Object Load(Object entity, IDatabaseSession databaseSession, Boolean includingLazyLoads)
		{
			// 构造条件
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
		/// 加载子实体集合。
		/// </summary>
		/// <param name="propertyName">子实体集合属性名称。</param>
		/// <param name="parentEntity">父实体。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		/// <returns>子实体集合。</returns>
		public Object[] LoadChildren(String propertyName, Object parentEntity, IDatabaseSession databaseSession)
		{
			String name = GetChildPropertyName(propertyName);

			Type elementType = ChildrenElementTypes[name];
			PropertyInfo elementPropertyInfo = ChildrenElementProperties[name];
			ChildrenAttribute childrenAttr = ChildrenAttributes[name];
			Sorter s = ChildrenSorters[name];

			Filter f = childrenAttr.CreateFilter(parentEntity);

			Object[] children = databaseSession.Load(elementType, f, s);

			// 转换父实体
			foreach (Object child in children)
			{
				elementPropertyInfo.SetValue(child, parentEntity, null);
			}

			return children;
		}

		/// <summary>
		/// 初始化，设置主键属性链。
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

		#region 辅助方法

		/// <summary>
		/// 构造属性链，忽略列名大小写。
		/// </summary>
		/// <param name="chain">容器。</param>
		/// <param name="entityType">实体类型。</param>
		/// <param name="columnName">列名。</param>
		private static void ComposePropertyChain(List<PropertyInfo> chain, Type entityType, String columnName)
		{
			DbEntityPropertyInfo target = DbEntityPropertyInfoCache.GetProperty(entityType);

			PropertyInfo simple = target.GetValuePropertyInfo(columnName);

			if (simple != null)
			{
				chain.Add(simple);

				// 递归结束
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

						// 递归调用
						ComposePropertyChain(chain, pf.PropertyType, mapping.ParentColumnName);

						found = true;

						break;
					}
				}
			}

			// 如果列声明没有找到，则报错
			if (!found)
			{
				String[] typeNames = chain.ConvertAll<String>(
						delegate(PropertyInfo pf) { return pf.PropertyType.FullName; }
					)
					.ToArray();

				String path = String.Join(" -> ", typeNames);

				throw new InvalidOperationException(
						String.Format("列声明错误，在 {0} 类型中没有找到列 {1} 声明，声明路径为 {2} 。",
								entityType.FullName,
								columnName,
								path
							)
					);
			}
		}

		/// <summary>
		/// 获取得属性链。
		/// </summary>
		/// <param name="columName">列名。</param>
		/// <param name="parentColumnName">父列名。</param>
		/// <param name="currentPropertyInfo">当前属性。</param>
		/// <returns></returns>
		private static PropertyInfo[] GetPropertyChain(String columName, String parentColumnName, PropertyInfo currentPropertyInfo)
		{
			List<PropertyInfo> chain = new List<PropertyInfo>();

			chain.Add(currentPropertyInfo);

			ComposePropertyChain(chain, currentPropertyInfo.PropertyType, parentColumnName);

			return chain.ToArray();
		}

		/// <summary>
		/// 获取实体中属性的简单值。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="chain">属性链，最后一个属性是简单属性。</param>
		/// <returns>属性值。</returns>
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
		/// 获取与列名相映射的值属性，忽略大小写。
		/// </summary>
		/// <param name="columnName">列名。</param>
		/// <returns>与该相映射的值属性。如果没有与值属性相映射的列名（该列可能在引用属性中），则返回空。</returns>
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
		/// 判断实体是否为部分构造的。
		/// </summary>
		/// <param name="entity">要判断的实体。</param>
		/// <returns>如果实体为部分构造的，则返回 true；否则返回 false。</returns>
		private static Boolean IsEntityPartially(Object entity)
		{
			EtyBusinessObject bo = entity as EtyBusinessObject;

			if (bo != null)
			{
				return bo.IsPartially;
			}
			else
			{
				// 不支持部分构造
				return false;
			}
		}

		#endregion
	}
}