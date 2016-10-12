#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityPrimitivePropertyDefinition.cs
// 文件功能描述：表示实体的基本（值）属性。
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
	/// 表示实体的基本（值）属性。
	/// </summary>
	internal sealed class EntityPrimitivePropertyDefinition : EntityPropertyDefinition
	{
		#region 私有字段

		private ColumnDefinition[] m_columns;
		private readonly ColumnAttribute m_columnAttr;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性所属的实体定义和属性信息。
		/// </summary>
		/// <param name="entity">属性所属的实体定义。</param>
		/// <param name="propertyInfo">属性信息。</param>
		public EntityPrimitivePropertyDefinition(EntityDefinition entity, PropertyInfo propertyInfo)
			: base(entity, propertyInfo)
		{
			m_columnAttr = (ColumnAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ColumnAttribute));
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取属性拥有的列定义集合。
		/// </summary>
		public override ColumnDefinition[] Columns
		{
			get
			{
				if (m_columns == null)
				{
					m_columns = new ColumnDefinition[] { Column };
				}

				return m_columns;
			}
		}

		/// <summary>
		/// 指示此属性只映射一列。
		/// </summary>
		public override Boolean HasComproundColumns
		{
			get { return false; }
		}

		/// <summary>
		/// 获取一个值，该值指示当前属性是否为主键。
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return m_columnAttr.IsPrimaryKey;
			}
		}

		/// <summary>
		/// 指示此属性为基本（值）属性。
		/// </summary>
		public override Boolean IsPrimitive
		{
			get { return true; }
		}

		/// <summary>
		/// 总是返回 false。
		/// </summary>
		public override bool PermitNull
		{
			get { return false; }
		}

		/// <summary>
		/// 不支持此属性。
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