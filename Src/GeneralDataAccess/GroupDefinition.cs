#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupDefinition.cs
// 文件功能描述：表示分组结果的类型定义。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110628
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
	/// 表示分组结果的类型定义。
	/// </summary>
	internal partial class GroupDefinition
	{
		#region 私有字段

		private readonly Type m_type;
		private readonly ConstructorInfo m_constructor;
		private readonly EntityDefinition m_entity;
		private readonly GroupPropertyDefinitionCollection m_properties;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置分组结果实体类型。
		/// </summary>
		/// <param name="groupType"></param>
		public GroupDefinition(Type groupType)
		{
			#region 前置条件

			Debug.Assert(groupType != null, "分组结果类型参数 groupType 不能为空。");
			Debug.Assert(
					typeof(GroupResult).IsAssignableFrom(groupType),
					String.Format("类型 {0} 没有从 GroupResult 派生。", groupType.FullName)
				);
			Debug.Assert(
					Attribute.IsDefined(groupType, typeof(GroupAttribute)),
					String.Format("类型 {0} 上没有标记 GroupAttribute。", groupType.FullName)
				);

			#endregion

			m_type = groupType;

			GroupAttribute groupAttr = (GroupAttribute)Attribute.GetCustomAttribute(groupType, typeof(GroupAttribute));

			#region 前置条件

			Debug.Assert(groupAttr.Type != null, String.Format("在类型 {0} 上标记的 GroupAttribute 中没有设置要分组的实体类型。", groupType.FullName));
			Debug.Assert(
					Attribute.IsDefined(groupAttr.Type, typeof(TableAttribute)),
					String.Format("要分组的类型 {0} 不是实体类型。", groupAttr.Type.FullName)
				);

			#endregion

			m_constructor = groupType.GetConstructor(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					(Binder)null,
					Type.EmptyTypes,
					(ParameterModifier[])null
				);

			#region 前置条件

			Debug.Assert(m_constructor != null, String.Format("类型 {0} 中不存在无参的构造函数。", groupType.FullName));

			#endregion

			m_entity = EntityDefinitionBuilder.Build(groupAttr.Type);

			List<GroupPropertyDefinition> allProperties = new List<GroupPropertyDefinition>();

			foreach (PropertyInfo pf in groupType.GetProperties(CommonPolicies.PropertyBindingFlags))
			{
				if (Attribute.IsDefined(pf, typeof(AggregationAttribute)))
				{
					allProperties.Add(new GroupPropertyDefinition(this, pf));
				}
			}

			m_properties = new GroupPropertyDefinitionCollection(this, allProperties);
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取分组结果实体类型的无参构造器。
		/// </summary>
		public ConstructorInfo Constructor
		{
			get { return m_constructor; }
		}

		/// <summary>
		/// 获取要分组的实体类型。
		/// </summary>
		public EntityDefinition Entity
		{
			get { return m_entity; }
		}

		/// <summary>
		/// 获取所有集合。
		/// </summary>
		public GroupPropertyDefinitionCollection Properties
		{
			get { return m_properties; }
		}

		/// <summary>
		/// 获取分组结果实体类型。
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取所有的外部引用属性。
		/// </summary>
		/// <returns>所有的外部引用属性集合。</returns>
		public GroupPropertyDefinition[] GetForeignReferenceProperties()
		{
			List<GroupPropertyDefinition> results = new List<GroupPropertyDefinition>();

			foreach (GroupPropertyDefinition propertyDef in m_properties)
			{
				if (!propertyDef.IsPrimitive)
				{
					results.Add(propertyDef);
				}
			}

			return results.ToArray();
		}

		/// <summary>
		/// 获取所有的分组项属性。
		/// </summary>
		/// <returns>分级项属性的集合。</returns>
		public GroupPropertyDefinition[] GetGroupItemProperties()
		{
			List<GroupPropertyDefinition> results = new List<GroupPropertyDefinition>();

			foreach (GroupPropertyDefinition propertyDef in Properties)
			{
				if (propertyDef.IsGroupItem)
				{
					results.Add(propertyDef);
				}
			}

			return results.ToArray();
		}

		/// <summary>
		/// 获取所有的基本属性（即值属性）。
		/// </summary>
		/// <returns>所有的基本属性的集合。</returns>
		public GroupPropertyDefinition[] GetPrimitiveProperties()
		{
			List<GroupPropertyDefinition> results = new List<GroupPropertyDefinition>();

			foreach (GroupPropertyDefinition propertyDef in m_properties)
			{
				if (propertyDef.IsPrimitive)
				{
					results.Add(propertyDef);
				}
			}

			return results.ToArray();
		}

		/// <summary>
		/// 字符串表示。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Format("GROUP OF {0}", m_entity.ToString());
		}

		#endregion
	}
}