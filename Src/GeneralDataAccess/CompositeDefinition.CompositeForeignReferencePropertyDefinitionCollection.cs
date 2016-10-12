#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeDefinition.CompositeForeignReferencePropertyDefinitionCollection.cs
// 文件功能描述：表示复合实体外部引用属性集合。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110708
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
	partial class CompositeDefinition
	{
		/// <summary>
		/// 表示复合实体外部引用属性集合。
		/// </summary>
		internal sealed class CompositeForeignReferencePropertyDefinitionCollection : ReadOnlyCollection<CompositeForeignReferencePropertyDefinition>
		{
			#region 构造函数

			/// <summary>
			/// 构造函数。
			/// </summary>
			/// <param name="properties">外部引用属性列表。</param>
			public CompositeForeignReferencePropertyDefinitionCollection(IList<CompositeForeignReferencePropertyDefinition> properties)
				: base(properties)
			{
			}

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取指定名称的属性。
			/// </summary>
			/// <param name="propertyName">要查找的属性的名称。</param>
			/// <returns>具有该名称的属性，如果没有找到，返回 null。</returns>
			public CompositeForeignReferencePropertyDefinition this[String propertyName]
			{
				get
				{
					foreach (CompositeForeignReferencePropertyDefinition propertyDef in Items)
					{
						if (propertyDef.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
						{
							return propertyDef;
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
			/// <returns>如果包含具有该名称的属性，则返回 true；否则返回 false。</returns>
			public Boolean Contains(String propertyName)
			{
				foreach (CompositeForeignReferencePropertyDefinition propertyDef in Items)
				{
					if (propertyDef.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
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