#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupSchemaFilterCompilationContext.cs
// 文件功能描述：分组结果实体架构过滤器编译环境，用于 HAVING 过滤器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110630
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 分组结果实体架构过滤器编译环境，用于 HAVING 过滤器。
	/// </summary>
	internal class GroupSchemaFilterCompilationContext : FilterCompilationContext
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		public GroupSchemaFilterCompilationContext(GroupSchema schema)
			: base(schema)
		{
		}

		/// <summary>
		/// 构造函数，设置实体架构和参数前缀。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <param name="parameterPrifix">参数前缀。</param>
		public GroupSchemaFilterCompilationContext(GroupSchema schema, String parameterPrifix)
			: base(schema, parameterPrifix)
		{
		}

		#endregion

		/// <summary>
		/// 参数名称前缀“ph__”。
		/// </summary>
		protected override String ParameterNamePrefix
		{
			get { return CommonPolicies.HavingFilterParameterNamePrefix; }
		}
	}
}