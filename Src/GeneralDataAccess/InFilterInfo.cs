#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：InFilterInfo.cs
// 文件功能描述：IN 过滤器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110327
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
	/// IN 过滤器。
	/// </summary>
	[Serializable]
	internal sealed class InFilterInfo : FilterInfo
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置值列表。
		/// </summary>
		/// <param name="discreteValues">值列表。</param>
		public InFilterInfo(Object[] discreteValues)
			: base(discreteValues)
		{
		}

		/// <summary>
		/// 构造函数，设置是否执行“非”操作和值列表。
		/// </summary>
		/// <param name="negative">指示是否执行“非”操作。</param>
		/// <param name="discreteValues">值列表。</param>
		public InFilterInfo(Boolean negative, Object[] discreteValues)
			: base(negative, discreteValues)
		{
		}

		/// <summary>
		/// 构造函数，设置值列表。
		/// </summary>
		/// <param name="builder">查询列表生成器。</param>
		internal InFilterInfo(QueryListBuilder builder)
			: base(builder)
		{
		}

		/// <summary>
		/// 构造函数，设置是否执行“非”操作和值列表。
		/// </summary>
		/// <param name="negative">指示是否执行“非”操作。</param>
		/// <param name="builder">查询列表生成器。</param>
		internal InFilterInfo(Boolean negative, QueryListBuilder builder)
			: base(negative, builder)
		{
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 创建过滤器。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>创建好的过滤器。</returns>
		protected override Filter DoCreateFilter(IList<String> propertyPath)
		{
			if (QueryListBuilder == null)
			{
				return Filter.CreateInFilter(propertyPath, Values);
			}
			else
			{
				return Filter.CreateInFilter(
						propertyPath,
						QueryListBuilder.Property,
						QueryListBuilder.Where,
						QueryListBuilder.Having,
						QueryListBuilder.Distinct
					);
			}
		}

		#endregion
	}
}