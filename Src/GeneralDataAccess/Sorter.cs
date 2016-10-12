#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Sorter.cs
// 文件功能描述：排序器，包含排序表达式序列。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008151033
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
	/// 排序器，包含排序表达式序列。
	/// </summary>
	[Serializable]
	public sealed class Sorter
	{
		#region 静态成员

		/// <summary>
		/// 合成排序表达式，如果排序器为空或不含有效的排序表达式，则返回 null。
		/// </summary>
		/// <param name="schema">架构，可通过列定位符来获取映射的列集合。</param>
		/// <param name="sorter">排序器。</param>
		/// <returns>合成好的排序表达式。</returns>
		public static String ComposeSortExpression(IColumnLocatable schema, Sorter sorter)
		{
			if ((sorter == null) || (sorter.Expressions.Length == 0))
			{
				return null;
			}

			StringBuilder buffer = new StringBuilder();

			foreach (SortExpression expression in sorter.Expressions)
			{
				ColumnLocator colLocator = expression.ColumnLocator;

				foreach (Column col in schema[colLocator])
				{
					if (buffer.Length != 0)
					{
						buffer.Append(",");
					}

					String columnFullName = col.FullName ?? col.Alias;

					buffer.Append(columnFullName);

					if (expression.SortMethod == SortMethod.Descending)
					{
						buffer.Append(" DESC");
					}
				}
			}

			return buffer.ToString();
		}

		#endregion

		#region 私有字段

		private readonly List<SortExpression> m_expressions = new List<SortExpression>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public Sorter()
		{
		}

		/// <summary>
		/// 构造函数。
		/// </summary>
		/// <param name="expressions">要加入的排序表达式。</param>
		public Sorter(params SortExpression[] expressions)
		{
			if (expressions != null)
			{
				m_expressions.AddRange(expressions);
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取其中包含的排序表达式。
		/// </summary>
		public SortExpression[] Expressions
		{
			get { return m_expressions.ToArray(); }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 附加排序表达式，按升序排序。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>当前的排序器。</returns>
		public Sorter Append(String propertyName)
		{
			return Append(null, propertyName, SortMethod.Ascending);
		}

		/// <summary>
		/// 附加排序表达式并指定排序方法、
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="sortMethod">排序方法。</param>
		/// <returns>当前排序器。</returns>
		public Sorter Append(String propertyName, SortMethod sortMethod)
		{
			return Append(null, propertyName, sortMethod);
		}

		/// <summary>
		/// 附加排序表达式，按升序排序。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <returns>当前排序器。</returns>
		public Sorter Append(String entityPropertyName, String propertyName)
		{
			return Append(entityPropertyName, propertyName, SortMethod.Ascending);
		}

		/// <summary>
		/// 附加排序表达式并指定排序方法。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="sortMethod">排序方法。</param>
		/// <returns></returns>
		public Sorter Append(String entityPropertyName, String propertyName, SortMethod sortMethod)
		{
			ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);
			SortExpression expression = new SortExpression(colLocator, sortMethod);

			m_expressions.Add(expression);

			return this;
		}

		/// <summary>
		/// 获取所有的列定位符。
		/// </summary>
		/// <returns>列定位符集合。</returns>
		public ColumnLocator[] GetAllColumnLocators()
		{
			ColumnLocator[] allColumnLocators = new ColumnLocator[Expressions.Length];

			for (Int32 i = 0; i < allColumnLocators.Length; i++)
			{
				allColumnLocators[i] = Expressions[i].ColumnLocator;
			}

			return allColumnLocators;
		}

		/// <summary>
		/// 获取所有的属性选择器（指示仅加载实体）。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>所有的属性选择器。</returns>
		public PropertySelector[] GetAllSelectors(Type entityType)
		{
			List<IPropertyChain> allForeignReferences = new List<IPropertyChain>();

			foreach (SortExpression expression in Expressions)
			{
				if (expression.ColumnLocator.PropertyPath.Length > 1)
				{
					PropertyChain chain = new PropertyChain(entityType, expression.ColumnLocator.PropertyPath);

					// 前一个属性节点链必须重新创建，否则会导航到当前属性
					IPropertyChain reference = chain.Previous.Copy();

					if (!allForeignReferences.Contains(reference))
					{
						allForeignReferences.Add(reference);
					}
				}
			}

			List<PropertySelector> allSelectors = allForeignReferences.ConvertAll<PropertySelector>(
					delegate(IPropertyChain chain)
					{
						return PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain);
					}
				);

			return allSelectors.ToArray();
		}

		#endregion
	}
}