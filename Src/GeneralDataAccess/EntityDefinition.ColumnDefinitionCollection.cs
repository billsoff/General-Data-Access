#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityDefinition.ColumnDefinitionCollection.cs
// 文件功能描述：列定义集合。
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
	partial class EntityDefinition
	{
		/// <summary>
		/// 列定义集合。
		/// </summary>
		public sealed class ColumnDefinitionCollection : ReadOnlyCollection<ColumnDefinition>
		{
			#region 构造函数

			/// <summary>
			/// 构造函数，设置要包装的列定义列表。
			/// </summary>
			/// <param name="allColumnDefinitions">列定义列表。</param>
			public ColumnDefinitionCollection(IList<ColumnDefinition> allColumnDefinitions)
				: base(allColumnDefinitions)
			{
			}

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取指定名称的列定义，不区分大小写。
			/// </summary>
			/// <param name="columnName">列名称。</param>
			/// <returns>具有该名称的列定义，如果未找到，则返回 null。</returns>
			public ColumnDefinition this[String columnName]
			{
				get
				{
					Debug.Assert(!String.IsNullOrEmpty(columnName), "列名称不能为空。");

					foreach (ColumnDefinition columnDef in Items)
					{
						if (columnDef.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
						{
							return columnDef;
						}
					}

					return null;
				}
			}

			#endregion
		}
	}
}