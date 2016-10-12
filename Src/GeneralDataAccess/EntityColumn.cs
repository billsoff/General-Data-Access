#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityColumn.cs
// 文件功能描述：实体列。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110712
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 实体列。
	/// </summary>
	internal sealed class EntityColumn : Column
	{
		#region 私有字段

		private ColumnDefinition m_definition;
		private EntityProperty m_property;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置列所属的实体架构和列定义。
		/// </summary>
		/// <param name="property">列所属的实体架构。</param>
		/// <param name="columnDef">列定义。</param>
		internal EntityColumn(EntityProperty property, ColumnDefinition columnDef)
		{
			m_property = property;
			m_definition = columnDef;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取列的数据库类型。
		/// </summary>
		public override DbType DbType
		{
			get { return m_definition.DbType; }
		}

		/// <summary>
		/// 获取列表达式（对于聚合列，为聚合表达式，默认为列的全名）。
		/// </summary>
		public override String Expression
		{
			get
			{
				return FullName;
			}
		}

		/// <summary>
		/// 获取列的全名称，即其限定名。
		/// </summary>
		public override String FullName
		{
			get
			{
				return CommonPolicies.GetColumnFullName(Definition, Property);
			}
		}

		/// <summary>
		/// 获取一个值，该值指示列是否为主键。
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return m_definition.IsPrimaryKey;
			}
		}

		/// <summary>
		/// 获取一个值，指示此列是否为基本列。
		/// </summary>
		public override Boolean IsPrimitive
		{
			get { return m_definition.Property.IsPrimitive; }
		}

		/// <summary>
		/// 获取一个值，该值指示此列是否延迟加载。
		/// </summary>
		public override Boolean LazyLoad
		{
			get { return m_definition.LazyLoad; }
		}

		/// <summary>
		/// 获取列名称。
		/// </summary>
		public override String Name
		{
			get { return m_definition.Name; }
		}

		/// <summary>
		/// 获取实体属性。
		/// </summary>
		public override EntityProperty Property
		{
			get { return m_property; }
		}

		/// <summary>
		/// 获取列所映射的属性的名称。
		/// </summary>
		public override String PropertyName
		{
			get { return m_property.Name; }
		}

		/// <summary>
		/// 获取列类型。
		/// </summary>
		public override Type Type
		{
			get { return m_definition.Type; }
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取列定义。
		/// </summary>
		internal override ColumnDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion
	}
}