#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityProperty.ColumnCollection.cs
// 文件功能描述：列集合。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110426
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Useease.GeneralDataAccess
{
	partial class EntityProperty
	{
		/// <summary>
		/// 列集合。
		/// </summary>
		public sealed class ColumnCollection : ReadOnlyCollection<Column>
		{
			#region 构造函数

			/// <summary>
			/// 构造函数。
			/// </summary>
			/// <param name="cols">列集合。</param>
			public ColumnCollection(IList<Column> cols)
				: base(cols)
			{
			}

			#endregion
		}
	}
}