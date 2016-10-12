#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AssemblyChildrenEntryCollection.cs
// 文件功能描述：子实体装配项集合。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110729
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
	/// <summary>
	/// 子实体装配项集合。
	/// </summary>
	[Serializable]
	internal class AssemblyChildrenEntryCollection : ReadOnlyCollection<AssemblyChildrenEntry>
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置配置项列表。
		/// </summary>
		/// <param name="childrenEntries">装配项列表。</param>
		public AssemblyChildrenEntryCollection(IList<AssemblyChildrenEntry> childrenEntries)
			: base(childrenEntries)
		{
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 查找装配项。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>与该名称相匹配的装配项，如果没有找到，则返回 null。</returns>
		public AssemblyChildrenEntry this[String propertyName]
		{
			get
			{
				foreach (AssemblyChildrenEntry childrenEntry in Items)
				{
					if (childrenEntry.PropertyName.Equals(propertyName, CommonPolicies.PropertyNameComparison))
					{
						return childrenEntry;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// 查找装配项。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>查找到的装配项，如果没有找到，则为空。</returns>
		public AssemblyChildrenEntry this[String[] propertyPath]
		{
			get
			{
				AssemblyChildrenEntry current = this[propertyPath[0]];

				if ((current != null) && (propertyPath.Length > 1))
				{
					for (Int32 i = 1; i < propertyPath.Length; i++)
					{
						current = current.ChildrenEntries[propertyPath[i]];

						if (current == null)
						{
							break;
						}
					}
				}

				return current;
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 判断是否包含装配项。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>如果包含装配项，则返回 true；否则返回 false。</returns>
		public Boolean Contains(String propertyName)
		{
			foreach (AssemblyChildrenEntry childrenEntry in Items)
			{
				if (childrenEntry.PropertyName.Equals(propertyName, CommonPolicies.PropertyNameComparison))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 判断是否包含装配项。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>如果包含装配项，则返回 true；否则返回 false。</returns>
		public Boolean Contains(String[] propertyPath)
		{
			AssemblyChildrenEntry childrenEntry = this[propertyPath];

			return (childrenEntry != null);
		}

		#endregion
	}
}