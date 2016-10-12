#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityChildrenPropertyDefinition.cs
// 文件功能描述：子实体列表属性定义。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110601
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
	/// 子实体列表属性定义。
	/// </summary>
	internal sealed class EntityChildrenPropertyDefinition : EntityPropertyDefinition
	{
		#region 私有字段

		private EntityPropertyDefinition m_childrenProperty;
		private readonly Sorter m_childrenSorter;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性所属的实体定义和属性信息。
		/// </summary>
		/// <param name="entity">实体定义。</param>
		/// <param name="propertyInfo">属性信息。</param>
		public EntityChildrenPropertyDefinition(EntityDefinition entity, PropertyInfo propertyInfo)
			: base(entity, propertyInfo)
		{
			#region 前置条件

			Debug.Assert(
					Attribute.IsDefined(propertyInfo, typeof(ChildrenAttribute)),
					String.Format("属性 {0}.{1} 上没有标记 Children。", entity.Type.FullName, propertyInfo.Name)
				);
			Debug.Assert(
					propertyInfo.CanWrite,
					String.Format("属性 {0}.{1} 不可写，无法支持自动装配，请设置为可写。", entity.Type.FullName, propertyInfo.Name)
				);

			Debug.Assert(propertyInfo.PropertyType.IsArray, String.Format("属性 {0}.{1} 类型应为数组。", entity.Type.FullName, propertyInfo.Name));

			#endregion

			m_childrenSorter = OrderByAttribute.ComposeSorter(propertyInfo);
		}

		#endregion

		#region 子实体列表属性相关成员

		#region 公共属性

		/// <summary>
		/// 总是返回 true。
		/// </summary>
		public override Boolean IsChildren
		{
			get { return true; }
		}

		/// <summary>
		/// 获取子实体列表元素与父实体的关联属性。
		/// </summary>
		public override EntityPropertyDefinition ChildrenProperty
		{
			get
			{
				if (m_childrenProperty == null)
				{
					ChildrenAttribute childrenAttr = (ChildrenAttribute)Attribute.GetCustomAttribute(PropertyInfo, typeof(ChildrenAttribute));
					Type elementType = Type.GetElementType();
					EntityDefinition childrenEntity = Entity.Provider.GetDefinition(elementType);
					m_childrenProperty = childrenEntity.Properties[childrenAttr.PropertyName];

					Debug.Assert(
							m_childrenProperty != null,
							String.Format("类型 {0} 中不存在属性 {1}。", elementType.FullName, childrenAttr.PropertyName)
						);

					Debug.Assert(m_childrenProperty.Relation.Parent == Entity, "子实体列表元素与父实体的关联属性没有引用到当前属性所属的实体");
				}

				return m_childrenProperty;
			}
		}

		/// <summary>
		/// 获取声明的排序器。
		/// </summary>
		public override Sorter ChildrenSorter
		{
			get { return m_childrenSorter; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建属性指定父实体的过滤器。
		/// </summary>
		/// <param name="parentEntity">父实体。</param>
		/// <returns>Filter 实例，父实体与指定的父实体相同。</returns>
		public override Filter ComposeChildrenFilter(Object parentEntity)
		{
			#region 前置条件

			Debug.Assert(parentEntity != null, "父实体参数 parentEntity 不能为空。");
			Debug.Assert(Type.IsAssignableFrom(parentEntity.GetType()), "父实体的类型与当前属性所属的类型不兼容。");

			#endregion

			return Filter.Create(ChildrenProperty.Name, Is.EqualTo(parentEntity));
		}

		/// <summary>
		/// 获取指定父实体的所有子实体。
		/// </summary>
		/// <param name="parentEntity">父实体。</param>
		/// <param name="dbSession">数据库会话引擎。</param>
		/// <returns>属性于指定父实体的所有子实体。</returns>
		public override Object[] GetAllChildren(Object parentEntity, IDatabaseSession dbSession)
		{
			#region 前置条件

			Debug.Assert(dbSession != null, "数据库会话引擎参数 dbSession 不能为空。");

			#endregion

			Object[] children = dbSession.Load(ChildrenProperty.Type, ComposeChildrenFilter(parentEntity), ChildrenSorter);

			foreach (Object child in children)
			{
				ChildrenProperty.PropertyInfo.SetValue(child, parentEntity, null);
			}

			return children;
		}

		#endregion

		#endregion

		#region 无关属性

		/// <summary>
		/// 不支持。
		/// </summary>
		public override ColumnDefinition[] Columns
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// 不支持。
		/// </summary>
		public override Boolean HasComproundColumns
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// 不支持。
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// 总是返回 false。
		/// </summary>
		public override Boolean IsPrimitive
		{
			get { return false; }
		}

		/// <summary>
		/// 总是返回 true。
		/// </summary>
		public override Boolean PermitNull
		{
			get { return true; }
		}

		/// <summary>
		/// 不支持。
		/// </summary>
		public override EntityPropertyDefinitionRelation Relation
		{
			get
			{
				throw new NotSupportedException();
			}
			internal set
			{
				throw new NotSupportedException();
			}
		}

		#endregion
	}
}