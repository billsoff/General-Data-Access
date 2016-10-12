#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SortExpression.cs
// 文件功能描述：表示一个排序表达式。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008151010
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
	/// 表示一个排序表达式。
	/// </summary>
	[Serializable]
	public class SortExpression
	{
		#region 私有字段

		private readonly ColumnLocator m_columnLocator;
		private readonly SortMethod m_sortMethod;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置值属性名称。
		/// </summary>
		/// <param name="propertyName">值属性名称。</param>
		public SortExpression(String propertyName)
			: this(null, propertyName, SortMethod.Ascending)
		{
		}

		/// <summary>
		/// 构造函数，设置值属性名称和排序方法。
		/// </summary>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="sortMethod">排序方法。</param>
		public SortExpression(String propertyName, SortMethod sortMethod)
			: this(null, propertyName, sortMethod)
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称和值属性名称。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		public SortExpression(String entityPropertyName, String propertyName)
			: this(entityPropertyName, propertyName, SortMethod.Ascending)
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称、值属性名称和排序方法。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="sortMethod">排序方法。</param>
		public SortExpression(String entityPropertyName, String propertyName, SortMethod sortMethod)
			: this(new ColumnLocator(entityPropertyName, propertyName), sortMethod)
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		public SortExpression(IList<String> propertyPath)
			: this(propertyPath, SortMethod.Ascending)
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径和排序方法。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="sortMethod">排序方法。</param>
		public SortExpression(IList<String> propertyPath, SortMethod sortMethod)
			: this(new ColumnLocator(propertyPath), sortMethod)
		{
		}

		/// <summary>
		/// 构造函数，设置列定位符。
		/// </summary>
		/// <param name="columnLocator">列定位符。</param>
		internal protected SortExpression(ColumnLocator columnLocator)
			: this(columnLocator, SortMethod.Ascending)
		{
		}

		/// <summary>
		/// 构造函数，设置列定位符和排序方法。
		/// </summary>
		/// <param name="columnLocator">列定位符。</param>
		/// <param name="sortMethod">排序方法。</param>
		internal protected SortExpression(ColumnLocator columnLocator, SortMethod sortMethod)
		{
			m_columnLocator = columnLocator;
			m_sortMethod = sortMethod;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取列定位符。
		/// </summary>
		public ColumnLocator ColumnLocator
		{
			get { return m_columnLocator; }
		}

		/// <summary>
		/// 获取排序方式。
		/// </summary>
		public SortMethod SortMethod
		{
			get { return m_sortMethod; }
		}

		#endregion
	}
}