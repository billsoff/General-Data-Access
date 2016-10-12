#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityDefinition.EntityPropertyDefinitionCollection.cs
// 文件功能描述：实体属性集合。
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	partial class EntityDefinition
	{
		/// <summary>
		/// 实体属性集合。
		/// </summary>
		internal sealed class EntityPropertyDefinitionCollection : ReadOnlyCollection<EntityPropertyDefinition>
		{
			#region 私有字段

			private readonly EntityDefinition m_entity;

			private readonly Dictionary<String, EntityPropertyDefinition> m_properties;
			private readonly Dictionary<String, Int32> m_propertyIndices;

			#endregion

			#region 构造函数

			/// <summary>
			/// 构造函数，设置要包装的实体属性列表。
			/// </summary>
			/// <param name="entity">所属的实体定义。</param>
			/// <param name="allEntityProperties">实体属性列表。</param>
			internal EntityPropertyDefinitionCollection(EntityDefinition entity, IList<EntityPropertyDefinition> allEntityProperties)
				: base(allEntityProperties)
			{
				m_entity = entity;

				m_properties = new Dictionary<String, EntityPropertyDefinition>(allEntityProperties.Count);
				m_propertyIndices = new Dictionary<String, Int32>(allEntityProperties.Count);

				for (Int32 i = 0; i < allEntityProperties.Count; i++)
				{
					EntityPropertyDefinition propertyDef = allEntityProperties[i];

					m_properties.Add(propertyDef.Name, propertyDef);
					m_propertyIndices.Add(propertyDef.Name, i);
				}
			}

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取指定名称的实体属性，考虑属性别名，区分大小写。
			/// </summary>
			/// <param name="propertyName">属性名称。</param>
			/// <returns>具有该名称的实体属性。</returns>
			public EntityPropertyDefinition this[String propertyName]
			{
				get
				{
					#region 前置条件

					Debug.Assert(!String.IsNullOrEmpty(propertyName), "属性名称不能为空。");

					#endregion

					foreach (EntityPropertyDefinition property in Items)
					{
						if (property.Matches(propertyName))
						{
							return property;
						}
					}

					return null;
				}
			}

			/// <summary>
			/// 由给定的属性链定位到属性定义，区分大小写，不考虑别名。
			/// </summary>
			/// <param name="chain">属性链。</param>
			/// <returns>属性定义。</returns>
			public EntityPropertyDefinition this[IPropertyChain chain]
			{
				get
				{
					#region 前置条件

					Debug.Assert((chain != null), "属性链参数 chain 不能为空。");
					Debug.Assert((chain.Type == Entity.Type), "属性链的实体类型与属性集合所属的实体类型不同。");

					#endregion

					EntityPropertyDefinition propertyDef = null;

					EntityDefinition definition = this.Entity;
					chain = chain.Head;

					while (true)
					{
						propertyDef = definition.Properties[chain.Name];

						Debug.Assert((propertyDef != null), String.Format("{0} 不存在属性 {1}", definition, chain.Name));

						chain = chain.Next;

						if (chain == null)
						{
							break;
						}

						Debug.Assert(
								!propertyDef.IsPrimitive,
								String.Format(
										"当使用属性链 {0} 定位属性时，中间的属性 {1} 为基本属性。",
										chain,
										propertyDef.Name
									)
							);

						definition = propertyDef.Relation.Parent;
					}

					return propertyDef;
				}
			}

			/// <summary>
			/// 获取所属的实体定义。
			/// </summary>
			public EntityDefinition Entity
			{
				get { return m_entity; }
			}

			#endregion

			#region 公共方法

			/// <summary>
			/// 判断是否存在指定名称的属性定义，名称区分大小写，不考虑别名。
			/// </summary>
			/// <param name="propertyName">属性名称。</param>
			/// <returns>如果存在指定名称的属性，则返回 true；否则返回 false。</returns>
			public Boolean Contains(String propertyName)
			{
				Debug.Assert((propertyName != null), "参数 propertyName 不能为空。");

				return m_properties.ContainsKey(propertyName);
			}

			/// <summary>
			/// 获取所有的子实体属性。
			/// </summary>
			/// <returns>子实体属性。</returns>
			public EntityPropertyDefinition[] GetAllChildrenProperties()
			{
				List<EntityPropertyDefinition> results = new List<EntityPropertyDefinition>();

				foreach (EntityPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.IsChildren)
					{
						results.Add(propertyDef);
					}
				}

				return results.ToArray();
			}

			/// <summary>
			/// 获取指定名称的属性定义，名称区分大小写，不考虑别名。
			/// </summary>
			/// <param name="propertyName">属性名称。</param>
			/// <returns>具有该名称的属性定义。</returns>
			public EntityPropertyDefinition GetDefinitionByName(String propertyName)
			{
				return m_properties[propertyName];
			}

			/// <summary>
			/// 获取指定名称的属性定义索引，名称区分大小写，不考虑别名。
			/// </summary>
			/// <param name="propertyName">属性名称。</param>
			/// <returns>属性定义的索引。</returns>
			public Int32 GetIndexByName(String propertyName)
			{
				return m_propertyIndices[propertyName];
			}

			/// <summary>
			/// 获取所有的主键属性。
			/// </summary>
			/// <returns>主键属性集合。</returns>
			public EntityPropertyDefinition[] GetPrimaryKeys()
			{
				List<EntityPropertyDefinition> all = new List<EntityPropertyDefinition>();

				foreach (EntityPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.IsChildren)
					{
						continue;
					}

					if (propertyDef.IsPrimaryKey)
					{
						all.Add(propertyDef);
					}
				}

				return all.ToArray();
			}

			/// <summary>
			/// 获取由数据库生成主键的属性。
			/// </summary>
			/// <returns>数据库生成主键的属性，如果不存在，则返回 null。</returns>
			public EntityPropertyDefinition GetNativePrimaryKey()
			{
				EntityPropertyDefinition nativePrimaryKey = null;

				foreach (EntityPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.IsChildren || !propertyDef.IsPrimaryKey)
					{
						continue;
					}

					if (Attribute.IsDefined(propertyDef.PropertyInfo, typeof(NativeAttribute)))
					{
						nativePrimaryKey = propertyDef;

						break;
					}
				}

				Debug.Assert((nativePrimaryKey == null) || nativePrimaryKey.IsPrimitive, "NativeAttribute 应只标记于做为主键的基本属性上。");

				return nativePrimaryKey;
			}

			/// <summary>
			/// 获取候选键属性。
			/// </summary>
			/// <returns>候选键属性集合。</returns>
			public EntityPropertyDefinition[] GetCandidateKeys()
			{
				List<EntityPropertyDefinition> all = new List<EntityPropertyDefinition>();

				foreach (EntityPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.IsCandidateKey)
					{
						all.Add(propertyDef);
					}
				}

				return all.ToArray();
			}

			/// <summary>
			/// 获取自动生成值的候选键。
			/// </summary>
			/// <returns>自动生成值的候选键。</returns>
			public EntityPropertyDefinition GetAutoGeneratedCandidateKey()
			{
				EntityPropertyDefinition[] candidates = GetCandidateKeys();

				if ((candidates.Length == 1) && candidates[0].AutoGenerateOnNew)
				{
					return candidates[0];
				}

				return null;
			}

			#endregion
		}
	}
}