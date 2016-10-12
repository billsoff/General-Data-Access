#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ExpressionSchema.cs
// 文件功能描述：表示一个表达式实体架构（子查询）。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示一个表达式实体架构（子查询）。
	/// </summary>
	internal abstract class ExpressionSchema : IColumnLocatable, IDebugInfoProvider
	{
		#region 私有字段

		private readonly Column[] m_columns;
		private readonly String m_sqlExpression;

		private Int32 m_index;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置表达式实体的选择列列表和 SQL 表达式。
		/// </summary>
		/// <param name="columns">选择列列表。</param>
		/// <param name="sqlExpression">SQL 表达式。</param>
		protected ExpressionSchema(ExpressionSchemaColumn[] columns, String sqlExpression)
		{
			m_columns = columns;
			m_sqlExpression = sqlExpression;

			foreach (ExpressionSchemaColumn col in columns)
			{
				col.Schema = this;
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取所有要选择的列。
		/// </summary>
		public Column[] Columns
		{
			get
			{
				return m_columns;
			}
		}

		/// <summary>
		/// 获取或设置索引。
		/// </summary>
		public Int32 Index
		{
			get { return m_index; }
			set { m_index = value; }
		}

		/// <summary>
		/// 获取名称。
		/// </summary>
		public String Name
		{
			get { return CommonPolicies.GetTableAlias(m_index); }
		}

		/// <summary>
		/// SQL 表达式。
		/// </summary>
		public String SqlExpression
		{
			get
			{
				return m_sqlExpression;
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建实体。
		/// </summary>
		/// <param name="dbValues">记录值。</param>
		/// <returns>创建好的实体。</returns>
		public abstract Object Compose(Object[] dbValues);

		#endregion

		#region IColumnLoactable 成员

		/// <summary>
		/// 获取列定位器所定位的列集合。
		/// </summary>
		/// <param name="colLocator">列定位器。</param>
		/// <returns>列集合。</returns>
		public abstract Column[] this[ColumnLocator colLocator] { get; }

		#endregion

		#region IDebugInfoProvider 成员

		/// <summary>
		/// 输出调试信息，默认实现是输出类型的全名称。
		/// </summary>
		/// <returns>调试信息。</returns>
		public virtual String Dump()
		{
			return GetType().FullName;
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="indent"></param>
		/// <returns></returns>
		public String Dump(String indent)
		{
			return DbEntityDebugger.IndentText(Dump(), indent);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), level);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="indent"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(String indent, Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), indent, level);
		}

		#endregion
	}
}