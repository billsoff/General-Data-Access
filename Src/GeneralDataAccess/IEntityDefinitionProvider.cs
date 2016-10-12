#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IEntityDefinitionProvider.cs
// 文件功能描述：提供实体定义。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 提供实体定义。
	/// </summary>
	internal interface IEntityDefinitionProvider
	{
		/// <summary>
		/// 获取实体定义。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>实体定义。</returns>
		EntityDefinition GetDefinition(Type entityType);
	}
}