#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupItemColumn.cs
// 文件功能描述：表示做为分组依据的列。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110707
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示做为分组依据的列。
	/// </summary>
	internal sealed class GroupItemColumn : WrappedColumn
	{
		#region 私有字段

		private readonly GroupPropertyDefinition m_groupProperty;
		private readonly AggregationAttribute m_aggregation;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置做为分组依据的实体列和分组结果实体属性定义。
		/// </summary>
		/// <param name="entityColumn">实体列。</param>
		public GroupItemColumn(Column entityColumn)
			: this(entityColumn, null)
		{
		}

		/// <summary>
		/// 构造函数，设置做为分组依据的实体列和分组结果实体属性定义。
		/// </summary>
		/// <param name="entityColumn">实体列。</param>
		/// <param name="propertyDef">分组结果实体属性定义。</param>
		public GroupItemColumn(Column entityColumn, GroupPropertyDefinition propertyDef)
			: base(entityColumn)
		{
			#region 前置条件

			Debug.Assert(entityColumn != null, "实体列 entityColumn 不能为空。");

			#endregion

			m_groupProperty = propertyDef;

			if (m_groupProperty != null)
			{
				m_aggregation = m_groupProperty.Aggregation;
			}
			else
			{
				m_aggregation = new GroupByAttribute(entityColumn.Property.PropertyChain.PropertyPath);
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取聚合标记。
		/// </summary>
		public AggregationAttribute Aggregation
		{
			get { return m_aggregation; }
		}

		/// <summary>
		/// 获取聚合名称。
		/// </summary>
		public override String AggregationName
		{
			get
			{
				return m_aggregation.Name;
			}
		}

		/// <summary>
		/// 获取列的数据库类型。
		/// </summary>
		public override DbType DbType
		{
			get
			{
				return CommonPolicies.GetGroupDbType(m_groupProperty, Definition);
			}
		}

		/// <summary>
		/// 获取列所映射的属性的名称。
		/// </summary>
		public override String PropertyName
		{
			get
			{
				return (m_groupProperty != null) ? m_groupProperty.Name : Definition.Name;
			}
		}

		/// <summary>
		/// 获取列类型。
		/// </summary>
		public override Type Type
		{
			get
			{
				return (m_groupProperty != null) ? m_groupProperty.Type : Definition.Type;
			}
		}

		#endregion
	}
}