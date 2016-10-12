#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：LoadStrategyOption.cs
// 文件功能描述：加载策略选项。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110520
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
	/// 加载策略选项。
	/// </summary>
	public enum LoadStrategyOption
	{
		/// <summary>
		/// 由数据库会话引擎指定，这是默认选项。
		/// <para>当前的策略是加载所有的未标记 SuppressExpand 的外部引用，同一属性路径上类型重复的引用加载到第二级（包含）为止。</para>
		/// </summary>
		Auto,

		/// <summary>
		/// 仅加载实体本身。
		/// </summary>
		SelfOnly,

		/// <summary>
		/// 加载第一级外部引用。
		/// </summary>
		OneLevel,

		/// <summary>
		/// 指定级别。
		/// </summary>
		SpecifyLevel,

		/// <summary>
		/// 不限制级别。
		/// </summary>
		UnlimitedLevel
	}
}