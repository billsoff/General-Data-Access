#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntitySchema.EntityPropertyCollection.cs
// 文件功能描述：实体属性集合。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110504
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
	partial class EntitySchema
	{
		/// <summary>
		/// 实体属性集合。
		/// </summary>
		public sealed class EntityPropertyCollection : ReadOnlyCollection<EntityProperty>
		{
			#region 构造函数

			/// <summary>
			/// 默认构造函数。
			/// </summary>
			/// <param name="properties">属性列表。</param>
			public EntityPropertyCollection(IList<EntityProperty> properties)
				: base(properties)
			{
			}

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取具有指定名称的属性。
			/// </summary>
			/// <param name="propertyName">属性名称。</param>
			/// <returns>具有该名称的属性，如果未找到，则返回 null。</returns>
			public EntityProperty this[String propertyName]
			{
				get
				{
					foreach (EntityProperty property in Items)
					{
						if (property.Definition.Matches(propertyName))
						{
							return property;
						}
					}

					return null;
				}
			}

			#endregion

			#region 公共方法

			/// <summary>
			/// 判断是否包含指定名称的属性。
			/// </summary>
			/// <param name="propertyName">属性名称。</param>
			/// <returns>如果包含该名称的属性，则返回 true；否则返回 false。</returns>
			public Boolean Contains(String propertyName)
			{
				foreach (EntityProperty property in Items)
				{
					if (property.Definition.Matches(propertyName))
					{
						return true;
					}
				}

				return false;
			}

			#endregion
		}
	}
}