#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Column.cs
// 文件功能描述：列。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110413
//
// 修改标识：宋冰 20110707
// 修改描述：将类改为可继承的，并将 Name、FullName、Type、DbType 等属性设为虚拟的，增加了 Alias、SqlExpression 等属性。
//
// 修改标识：宋冰 20110712
// 修改描述：将类改为抽象的，将列相关的属性改为抽象的，增加 EntityColumn 保持原来的功能。
//
// 修改标识：
// 修改描述：
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 列，当前的实现是一个实体列。
	/// </summary>
	public abstract class Column
	{
		#region 私有字段

		private Int32 m_index;
		private Int32 m_fieldIndexOffset;
		private Boolean m_selected;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，用于派生类。
		/// </summary>
		protected Column()
		{
		}

		#endregion

		#region 抽象成员

		#region 公共属性

		/// <summary>
		/// 获取列的数据库类型。
		/// </summary>
		public abstract DbType DbType { get; }

		/// <summary>
		/// 获取列表达式（对于聚合列，为聚合表达式，默认为列的全名）。
		/// </summary>
		public abstract String Expression { get; }

		/// <summary>
		/// 获取列的全名称，即其限定名。
		/// </summary>
		public abstract String FullName { get; }

		/// <summary>
		/// 获取一个值，该值指示列是否为主键。
		/// </summary>
		public abstract Boolean IsPrimaryKey { get; }

		/// <summary>
		/// 获取一个值，指示此列是否为基本列。
		/// </summary>
		public abstract Boolean IsPrimitive { get; }

		/// <summary>
		/// 获取一个值，该值指示此列是否延迟加载。
		/// </summary>
		public abstract Boolean LazyLoad { get; }

		/// <summary>
		/// 获取列名称。
		/// </summary>
		public abstract String Name { get; }

		/// <summary>
		/// 获取实体属性。
		/// </summary>
		public abstract EntityProperty Property { get; }

		/// <summary>
		/// 获取列所映射的属性的名称。
		/// </summary>
		public abstract String PropertyName { get; }

		/// <summary>
		/// 获取列类型。
		/// </summary>
		public abstract Type Type { get; }

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取列定义。
		/// </summary>
		internal abstract ColumnDefinition Definition { get; }

		#endregion

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取聚合名称，默认为 null。
		/// </summary>
		public virtual String AggregationName
		{
			get { return null; }
		}

		/// <summary>
		/// 获取当前列的别名。
		/// <para>如果当前列被选择，则返回其别名；否则，返回其全名。</para>
		/// </summary>
		public String Alias
		{
			get
			{
				if (Selected)
				{
					return CommonPolicies.GetColumnAlias(Index);
				}
				else
				{
					return FullName;
				}
			}
		}

		/// <summary>
		/// 获取字段索引。
		/// </summary>
		public Int32 FieldIndex
		{
			get { return (m_index + m_fieldIndexOffset); }
		}

		/// <summary>
		/// 获取字段索引偏移。
		/// </summary>
		public Int32 FieldIndexOffset
		{
			get { return m_fieldIndexOffset; }
			internal protected set { m_fieldIndexOffset = value; }
		}

		/// <summary>
		/// 获取列（在记录中的）索引。
		/// </summary>
		public Int32 Index
		{
			get { return m_index; }
			internal protected set { m_index = value; }
		}

		/// <summary>
		/// 获取一个值，该值指示此属性是否是原生主键（即由数据库生成的主键）。
		/// </summary>
		public Boolean IsPrimaryKeyNative
		{
			get { return (Definition != null) ? Definition.IsPrimaryKeyNative : false; }
		}

		/// <summary>
		/// 获取一个值，该值指示此列是否被选择。
		/// </summary>
		public Boolean Selected
		{
			get { return m_selected; }
			internal protected set { m_selected = value; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取父列。
		/// </summary>
		/// <returns>父列。</returns>
		public Column GetParentColumn()
		{
			if (IsPrimitive || (Property == null) || !Property.Schema.HasRightRelations)
			{
				return null;
			}

			foreach (EntitySchemaRelation relation in Property.Schema.RightRelations)
			{
				if (relation.ChildProperty == Property)
				{
					Column parentColumn = relation.GetMappingParentColumn(this);

					return parentColumn;
				}
			}

			return null;
		}

		/// <summary>
		/// 获取列的数据库值，即如果该的值为 null，将其转换为 DBNull（如果是 DBEmpty，则指示插入或更新操作忽略该列 ），总是从列所属的实体开始解析。
		/// </summary>
		/// <param name="entity">要解析的实体。</param>
		/// <returns>实体中当前列的值。</returns>
		public Object GetDbValue(Object entity)
		{
			return GetDbValue(entity, false);
		}

		/// <summary>
		/// 获取列的数据库值，即如果该的值为 null，将其转换为 DBNull（如果是 DBEmpty，则指示插入或更新操作忽略该列 ）。
		/// </summary>
		/// <param name="entity">要解析的实体。</param>
		/// <param name="isProperty">
		/// <para>如果为 true，指示要解析的实体映射于列所属的属性（只对外部引用属性有意义）；</para>
		/// <para>否则，总是从列所属的实体定义开始解析。</para>
		/// </param>
		/// <returns>实体中当前列的值。</returns>
		public Object GetDbValue(Object entity, Boolean isProperty)
		{
			if (entity == null)
			{
				return DBNull.Value;
			}

			ColumnDefinition columnDef = Definition;

			if (!IsPrimitive && isProperty)
			{
				columnDef = columnDef.GetParentColumn();
			}

			#region 前置断言

			Debug.Assert(
					columnDef.Property.Entity.Type.IsAssignableFrom(entity.GetType()),
					String.Format(
							"实体值的类型（{0}）与列所属的实体类型（{1}）不兼容。",
							entity.GetType(),
							columnDef.Property.Entity.Type
						)
				);

			#endregion

			return columnDef.GetDbValue(entity);
		}

		/// <summary>
		/// 显示列的命名称。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return FullName;
		}

		#endregion
	}
}