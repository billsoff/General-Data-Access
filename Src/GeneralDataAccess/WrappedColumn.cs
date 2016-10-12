#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：WrappedColumn.cs
// 文件功能描述：表示对一个列的包装。
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
	/// 表示对一个列的包装。
	/// </summary>
	internal abstract class WrappedColumn : Column
	{
		#region 静态方法

		/// <summary>
		/// 查找最底层的被包装列。
		/// </summary>
		/// <param name="targentColumn">要查找的目标列。</param>
		/// <returns>最底层的被包装列，如果目标列不是 WrappedColumn，则返回其自身。</returns>
		public static Column GetRootColumn(Column targentColumn)
		{
			if (!(targentColumn is WrappedColumn))
			{
				return targentColumn;
			}

			return GetRootColumn(((WrappedColumn)targentColumn).InnerColumn);
		}

		#endregion

		#region 私有字段

		private readonly Column m_innerColumn;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置被包装的列。
		/// </summary>
		/// <param name="innerColumn">被包装的列。</param>
		protected WrappedColumn(Column innerColumn)
		{
			#region 前置条件

			Debug.Assert(innerColumn != null, "被包装的列参数 innerColumn 不能为空。");

			#endregion

			m_innerColumn = innerColumn;

			Index = innerColumn.Index;
			Selected = innerColumn.Selected;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 被包装的列。
		/// </summary>
		public Column InnerColumn
		{
			get { return m_innerColumn; }
		}

		/// <summary>
		/// 获取列的数据库类型。
		/// </summary>
		public override DbType DbType
		{
			get
			{
				return m_innerColumn.DbType;
			}
		}

		/// <summary>
		/// 获取列表达式（对于聚合列，为聚合表达式，默认为列的全名）。
		/// </summary>
		public override String Expression
		{
			get
			{
				return m_innerColumn.Expression;
			}
		}

		/// <summary>
		/// 获取列的全名称，即其限定名。
		/// </summary>
		public override String FullName
		{
			get
			{
				return m_innerColumn.FullName;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示列是否为主键。
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return m_innerColumn.IsPrimaryKey;
			}
		}

		/// <summary>
		/// 获取一个值，指示此列是否为基本列。
		/// </summary>
		public override Boolean IsPrimitive
		{
			get
			{
				return m_innerColumn.IsPrimitive;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示此列是否延迟加载。
		/// </summary>
		public override Boolean LazyLoad
		{
			get
			{
				return m_innerColumn.LazyLoad;
			}
		}

		/// <summary>
		/// 获取列名称。
		/// </summary>
		public override String Name
		{
			get
			{
				return m_innerColumn.Name;
			}
		}

		/// <summary>
		/// 获取列所属的属性。
		/// </summary>
		public override EntityProperty Property
		{
			get
			{
				return m_innerColumn.Property;
			}
		}

		/// <summary>
		/// 获取列所映射的属性的名称。
		/// </summary>
		public override String PropertyName
		{
			get
			{
				return m_innerColumn.PropertyName;
			}
		}

		/// <summary>
		/// 获取列类型。
		/// </summary>
		public override Type Type
		{
			get
			{
				return m_innerColumn.Type;
			}
		}

		/// <summary>
		/// 获取列定义。
		/// </summary>
		internal override ColumnDefinition Definition
		{
			get
			{
				return m_innerColumn.Definition;
			}
		}

		#endregion
	}
}