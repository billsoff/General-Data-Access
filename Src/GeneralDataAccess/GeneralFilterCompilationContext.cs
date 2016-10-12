#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GeneralFilterCompilationContext.cs
// 文件功能描述：一个过滤器编译环境的通用实现。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110711
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
	/// 一个过滤器编译环境的通用实现。
	/// </summary>
	internal class GeneralFilterCompilationContext : FilterCompilationContext
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置架构。
		/// </summary>
		/// <param name="schema">架构。</param>
		public GeneralFilterCompilationContext(IColumnLocatable schema)
			: base(schema)
		{
		}

		/// <summary>
		/// 构造函数，设置架构和参数前缀。
		/// </summary>
		/// <param name="schema">架构。</param>
		/// <param name="parameterPrefix">参数前缀。</param>
		public GeneralFilterCompilationContext(IColumnLocatable schema, String parameterPrefix)
			: base(schema, parameterPrefix)
		{
		}

		#endregion

		/// <summary>
		/// 参数前缀“pw__”。
		/// </summary>
		protected override String ParameterNamePrefix
		{
			get { return CommonPolicies.WhereFilterParameterNamePrefix; }
		}
	}
}