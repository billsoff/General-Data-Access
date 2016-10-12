#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IDbEntity.cs
// 文件功能描述：表示一个数据库实体，EtyBusinessObject 实现此接口。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110221
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
	/// 代表一个数据库实体，数据库会可根据其状态进行相应的操作。
	/// </summary>
	public interface IDbEntity
	{
		/// <summary>
		/// 获取或设置一个值，该值指示实体对象是否已从数据库删除，默认值为 false。
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">当 Transient 为 true，设置其为 true。</exception>
		Boolean Deleted { get; set; }

		/// <summary>
		/// 获取或设置一个值，该值指示当前实体是否已被修改，默认值为 false。
		/// </summary>
		Boolean Dirty { get; set; }

		/// <summary>
		/// 获取或设置一个值，该值指示客户是否请示删除实体，默认值为 false。
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">当 Transient 为 true，设置其为 true。</exception>
		Boolean RequireDelete { get; set; }

		/// <summary>
		/// 获取或设置一个值，该值指示当前对象是否为游离对象（即还没有进行持久化），默认值为 true。
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">当前的状态为 false，设置其为 true。</exception>
		Boolean Transient { get; set; }
	}
}