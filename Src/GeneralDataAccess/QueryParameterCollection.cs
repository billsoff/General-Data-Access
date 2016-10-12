#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：QueryParameterCollection.cs
// 文件功能描述：查询参数集合的一个包装，用于实体过滤条件。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201009022255
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
	/// 查询参数集合的一个包装，用于实体过滤条件。
	/// </summary>
	[Serializable]
	public class QueryParameterCollection : ReadOnlyCollection<QueryParameter>
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置要包装的集合。
		/// </summary>
		/// <param name="parameters"></param>
		public QueryParameterCollection(IList<QueryParameter> parameters)
			: base(parameters)
		{
		}

		#endregion
	}
}