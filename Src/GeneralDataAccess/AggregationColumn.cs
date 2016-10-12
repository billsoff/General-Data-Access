#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AggregationColumn.cs
// 文件功能描述：表示一个聚合列。
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
	/// 表示一个聚合列。
	/// </summary>
	internal sealed class AggregationColumn : Column
	{
		#region 私有字段

		private readonly EntityProperty m_aggregationProperty;
		private readonly GroupPropertyDefinition m_groupProperty;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置要聚合的属性（如果为空，表示对整个实体聚合）和分组结果实体属性定义。
		/// </summary>
		/// <param name="aggregationProperty">要聚合的属性。</param>
		/// <param name="propertyDef">分组结果实体属性定义。</param>
		public AggregationColumn(EntityProperty aggregationProperty, GroupPropertyDefinition propertyDef)
		{
			#region 前置条件

			Debug.Assert(
					(aggregationProperty == null) || aggregationProperty.IsPrimitive,
					String.Format(
						"属性 {0} 不是一个基本列，无法进行聚合计算。",
						(aggregationProperty != null) ? aggregationProperty.PropertyChain.FullName : String.Empty
					)
				);

			#endregion

			m_aggregationProperty = aggregationProperty;
			m_groupProperty = propertyDef;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取聚合名称。
		/// </summary>
		public override String AggregationName
		{
			get
			{
				return m_groupProperty.Aggregation.Name;
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
		/// 获取列表达式（对于聚合列，为聚合表达式，默认为列的全名）。
		/// </summary>
		public override String Expression
		{
			get
			{
				return m_groupProperty.Aggregation.GetAggregationExpression(FullName);
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
		/// 对当前实例无意义，总是返回 false。
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 总是返回 true。
		/// </summary>
		public override Boolean IsPrimitive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// 对当前实例无意义，总是返回 false。
		/// </summary>
		public override Boolean LazyLoad
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 获取列名称。
		/// </summary>
		public override String Name
		{
			get
			{
				return (Definition != null) ? Definition.Name : null;
			}
		}

		/// <summary>
		/// 获取要聚合的属性。
		/// </summary>
		public override EntityProperty Property
		{
			get { return m_aggregationProperty; }
		}

		/// <summary>
		/// 获取列所映射的属性的名称。
		/// </summary>
		public override String PropertyName
		{
			get
			{
				return m_groupProperty.Name;
			}
		}

		/// <summary>
		/// 获取列类型。
		/// </summary>
		public override Type Type
		{
			get
			{
				return m_groupProperty.Type;
			}
		}

		/// <summary>
		/// 获取列定义。
		/// </summary>
		internal override ColumnDefinition Definition
		{
			get { return (m_aggregationProperty != null) ? m_aggregationProperty.Definition.Column : null; }
		}

		#endregion
	}
}