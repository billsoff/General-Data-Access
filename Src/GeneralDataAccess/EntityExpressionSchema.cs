#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityExpressionSchema.cs
// 文件功能描述：实体表达式架构。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110711
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 实体表达式架构。
	/// </summary>
	internal sealed class EntityExpressionSchema : ExpressionSchema, IDebugInfoProvider
	{
		#region 私有字段

		private Dictionary<EntityProperty, Column[]> m_columLookups;
		private EntitySchemaComposite m_composite;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置选择列集合和 SQL 指令。
		/// </summary>
		/// <param name="columns">选择列集合。</param>
		/// <param name="sqlExpression">SQL 指令。</param>
		internal EntityExpressionSchema(ExpressionSchemaColumn[] columns, String sqlExpression)
			: base(columns, sqlExpression)
		{
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取或设置列查找字典，键为选择属性，值为该属性所属的包装列。
		/// </summary>
		internal Dictionary<EntityProperty, Column[]> ColumLookups
		{
			get { return m_columLookups; }
			set { m_columLookups = value; }
		}

		/// <summary>
		/// 获取或设置实体架构组合。
		/// </summary>
		internal EntitySchemaComposite Composite
		{
			get { return m_composite; }
			set { m_composite = value; }
		}

		#endregion

		/// <summary>
		/// 构造实体。
		/// </summary>
		/// <param name="dbValues">记录值。</param>
		/// <returns>构造好的实体。</returns>
		public override Object Compose(Object[] dbValues)
		{
			return m_composite.Target.Compose(dbValues);
		}

		/// <summary>
		/// 定位列。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>定位到的列。</returns>
		public override Column[] this[ColumnLocator colLocator]
		{
			get
			{
				Column[] columns = m_composite.Target[colLocator];
				EntityProperty property = columns[0].Property;

				#region 前置条件

				Debug.Assert(m_columLookups.ContainsKey(property), String.Format("属性 {0} 未被选择。", colLocator.FullName));

				#endregion

				return m_columLookups[property];
			}
		}

		#region IDebugInfoProvider 成员

		/// <summary>
		/// 获取实体表达式架构的详细信息，用于调试。
		/// </summary>
		/// <returns>实体表达式架构的详细信息。</returns>
		public override String Dump()
		{
			return Composite.Dump() + "\r\n" + Composite.BuilderStrategy.Dump();
		}

		#endregion
	}
}