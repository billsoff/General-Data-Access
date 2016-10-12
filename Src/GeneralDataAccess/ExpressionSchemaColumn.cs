#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ExpressionSchemaColumn.cs
// 文件功能描述：表示表达式架构中的列，对内部列进行一次包装。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示表达式架构中的列，对内部列进行一次包装。
	/// </summary>
	internal sealed class ExpressionSchemaColumn : WrappedColumn
	{
		#region 私有字段

		private ExpressionSchema m_schema;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置列所属的架构和要包装的列。
		/// </summary>
		/// <param name="column">要包装的列。</param>
		public ExpressionSchemaColumn(Column column)
			: base(column)
		{
		}

		#endregion

		/// <summary>
		/// 获取列名称，为内部列的别名。
		/// </summary>
		public override String Name
		{
			get
			{
				return InnerColumn.Alias;
			}
		}

		/// <summary>
		/// 获取列的全名称。
		/// </summary>
		public override String FullName
		{
			get
			{
				return String.Format("{0}.{1}", Schema.Name, Name);
			}
		}

		/// <summary>
		/// 获取列表达式，与全名称相同。
		/// </summary>
		public override String Expression
		{
			get
			{
				return FullName;
			}
		}

		/// <summary>
		/// 获取列所属的表达式架构。
		/// </summary>
		internal ExpressionSchema Schema
		{
			get { return m_schema; }
			set { m_schema = value; }
		}
	}
}