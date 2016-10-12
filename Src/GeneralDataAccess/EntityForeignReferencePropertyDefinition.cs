#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityForeignReferencePropertyDefinition.cs
// 文件功能描述：表示外部引用属性。
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
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示外部引用属性。
	/// </summary>
	internal sealed class EntityForeignReferencePropertyDefinition : EntityPropertyDefinition
	{
		#region 私有字段

		private readonly Boolean m_permitNull;
		private EntityPropertyDefinitionRelation m_relation;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性所属的实体定义、属性信息和指示外部引用可否为空。
		/// </summary>
		/// <param name="owner">属性所属的实体定义。</param>
		/// <param name="propertyInfo">属性信息。</param>
		/// <param name="permitNull">指示外部引用可否空。</param>
		public EntityForeignReferencePropertyDefinition(EntityDefinition owner, PropertyInfo propertyInfo, Boolean permitNull)
			: base(owner, propertyInfo)
		{
			m_permitNull = permitNull;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取包含的所有列。
		/// </summary>
		public override ColumnDefinition[] Columns
		{
			get { return m_relation.ChildColumns; }
		}

		/// <summary>
		/// 判断属性是否映射为复合列。
		/// </summary>
		public override Boolean HasComproundColumns
		{
			get { return (Columns.Length > 1); }
		}

		/// <summary>
		/// 获取一个值，该值指示当前属性是否为主键。
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return Attribute.IsDefined(PropertyInfo, typeof(PrimaryKeyAttribute));
			}
		}

		/// <summary>
		/// 指示当前属性为外部引用属性。
		/// </summary>
		public override Boolean IsPrimitive
		{
			get { return false; }
		}

		/// <summary>
		/// 获取一个值，该值指示外部引用可否为空。
		/// </summary>
		public override Boolean PermitNull
		{
			get { return m_permitNull; }
		}

		/// <summary>
		/// 获取属性关系。
		/// </summary>
		public override EntityPropertyDefinitionRelation Relation
		{
			get { return m_relation; }
			internal set { m_relation = value; }
		}

		#region 子实体列表相关属性

		/// <summary>
		/// 获取一个值，该值指示此属性是否为子实体列表，总是返回 false。
		/// </summary>
		public override Boolean IsChildren
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 获取子实体列表属性列表元素的属性定义。
		/// </summary>
		public override EntityPropertyDefinition ChildrenProperty
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// 获取子实体列表属性的排序器，用于加载子实体列表。
		/// </summary>
		public override Sorter ChildrenSorter
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		#endregion

		#endregion

		#region 子实体列表相关方法

		/// <summary>
		/// 创建用于加载子实体的过滤器。
		/// </summary>
		/// <param name="parentEntity">父实体对象。</param>
		/// <returns>Filter 实例，用于得到该父实体拥有的所有子实体。</returns>
		public override Filter ComposeChildrenFilter(Object parentEntity)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// 获取所有的子实体。
		/// </summary>
		/// <param name="parentEntity">父实体。</param>
		/// <param name="dbSession">数据库会话引擎。</param>
		/// <returns>子实体列表。</returns>
		public override Object[] GetAllChildren(Object parentEntity, IDatabaseSession dbSession)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}