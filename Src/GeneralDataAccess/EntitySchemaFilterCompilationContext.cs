#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntitySchemaFilterCompilationContext.cs
// 文件功能描述：实体架构过滤器编译环境，用于生成过滤器参数。
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
	/// 实体架构过滤器编译环境，用于生成过滤器参数。
	/// </summary>
	internal sealed class EntitySchemaFilterCompilationContext : FilterCompilationContext
	{

		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		public EntitySchemaFilterCompilationContext(EntitySchema schema)
			: base(schema)
		{
		}

		/// <summary>
		/// 构造函数，设置实体架构和参数前缀。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <param name="parameterPrifix">参数前缀。</param>
		public EntitySchemaFilterCompilationContext(EntitySchema schema, String parameterPrifix)
			: base(schema, parameterPrifix)
		{
		}

		#endregion

		/// <summary>
		/// 参数名称前缀“pw__”。
		/// </summary>
		protected override String ParameterNamePrefix
		{
			get { return CommonPolicies.WhereFilterParameterNamePrefix; }
		}
	}
}