#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ColumnCollection.cs
// 文件功能描述：列集合。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110413
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	partial class EntitySchema
	{
		/// <summary>
		/// 列集合。
		/// </summary>
		public sealed class ColumnCollection : ReadOnlyCollection<Column>
		{
			#region 私有字段

			/// <summary>
			/// 列集合，键是列（定义）的全名称。
			/// </summary>
			private readonly Dictionary<String, Column> m_columns;

			#endregion

			#region 构造函数

			/// <summary>
			/// 构造函数，设置基础列的列表。
			/// </summary>
			/// <param name="columnList">列的列表。</param>
			public ColumnCollection(IList<Column> columnList)
				: base(columnList)
			{
				m_columns = new Dictionary<String, Column>();

				foreach (Column col in columnList)
				{
					m_columns.Add(col.Definition.FullName, col);
				}
			}

			#endregion

			#region 内部方法

			/// <summary>
			/// 获取与列定义相匹配的列。
			/// </summary>
			/// <param name="definition">列定义。</param>
			/// <returns>与该列定义相匹配的列。</returns>
			internal Column GetColumnByDefinition(ColumnDefinition definition)
			{
				String fullName = definition.FullName;

				if (m_columns.ContainsKey(fullName))
				{
					return m_columns[fullName];
				}
				else
				{
					return null;
				}
			}

			/// <summary>
			/// 获取与列定义相匹配的列。
			/// </summary>
			/// <param name="definitions">列定义列表。</param>
			/// <returns>相匹配的列集合。</returns>
			internal Column[] GetColumnsByDefinitions(ColumnDefinition[] definitions)
			{
				Column[] results = new Column[definitions.Length];

				for (Int32 i = 0; i < results.Length; i++)
				{
					results[i] = GetColumnByDefinition(definitions[i]);
				}

				return results;
			}

			#endregion
		}
	}
}