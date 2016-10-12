#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NewFilterFactoryOperator.cs
// 文件功能描述：操作符，用于创建 FilterFactory。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110326
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
	/// 操作符，用于创建 FilterFactory。
	/// </summary>
	[Serializable, Precedence(FilterOperatorPrecedences.HIGHEST, FilterOperatorPrecedences.HIGHEST)]
	internal sealed class NewFilterFactoryOperator : FilterOperator, IFilterProvider
	{
		#region 私有字段

		private readonly ColumnLocator m_columnLocator;
		private readonly FilterInfo m_filterInfo;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置列信息和过滤器信息。
		/// </summary>
		/// <param name="colLocator">列信息。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		public NewFilterFactoryOperator(ColumnLocator colLocator, FilterInfo filterInfo)
		{
			m_columnLocator = colLocator;
			m_filterInfo = filterInfo;
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 执行运算。
		/// </summary>
		/// <returns>运算结果，FilterFactory。</returns>
		public override FilterFactory Calculate()
		{
			return new FilterFactory(m_columnLocator.PropertyPath, m_filterInfo);
		}

		#endregion
	}
}