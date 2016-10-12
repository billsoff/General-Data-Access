#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IDebugInfoProvider.cs
// 文件功能描述：可提供用于调试的实例的详细信息。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110720
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
	/// 可提供用于调试的实例的详细信息。
	/// </summary>
	public interface IDebugInfoProvider
	{
		/// <summary>
		/// 获取实例的详细信息，用于调试。
		/// </summary>
		/// <returns>实例的详细信息</returns>
		String Dump();

		/// <summary>
		/// 获取实例的详细信息，并指定缩进，用于调试。
		/// </summary>
		/// <param name="indent">缩进。</param>
		/// <returns>实例的详细信息。</returns>
		String Dump(String indent);

		/// <summary>
		/// 获取实例的详细信息，并指定缩进级别，用于调试。
		/// </summary>
		/// <param name="level">缩进级别。</param>
		/// <returns>实例的详细信息。</returns>
		String Dump(Int32 level);

		/// <summary>
		/// 获取实例的详细信息，并指定缩进和级别，用于调试。
		/// </summary>
		/// <param name="indent">缩进。</param>
		/// <param name="level">级别。</param>
		/// <returns>实例的详细信息。</returns>
		String Dump(String indent, Int32 level);
	}
}