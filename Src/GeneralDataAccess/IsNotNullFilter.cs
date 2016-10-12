#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IsNotNullFilter.cs
// 文件功能描述：表示属性值不为空的过滤条件，支持实体过滤。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20100815
//
// 修改标识：宋冰 20100903
// 修改描述：增加了对实体过滤的支持。
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示属性值不为空的过滤条件，支持实体过滤。
	/// </summary>
	[Serializable]
	public class IsNotNullFilter : Filter
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性名称和属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		public IsNotNullFilter(String propertyName)
			: base(propertyName)
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		public IsNotNullFilter(String entityPropertyName, String propertyName)
			: base(entityPropertyName, propertyName)
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		public IsNotNullFilter(IList<String> propertyPath)
			: base(propertyPath)
		{
		}

		#endregion

		/// <summary>
		/// 向输入缓冲区写入条件表达式。
		/// </summary>
		/// <param name="output">输入缓冲区。</param>
		public override void Generate(StringBuilder output)
		{
			ThrowExceptionIfNotCompiled();

			if (!IsEntityFilter)
			{
				GenerateValueFilterExpression(output);
			}
			else
			{
				GenerateEntityFilterExpression(output);
			}
		}

		/// <summary>
		/// 生成值过滤表达式。
		/// </summary>
		/// <param name="output">输入缓冲区。</param>
		private void GenerateValueFilterExpression(StringBuilder output)
		{
			if (!Negative)
			{
				output.AppendFormat("{0} IS NOT NULL", ColumnFullName);
			}
			else
			{
				output.AppendFormat("{0} IS NULL", ColumnFullName);
			}
		}

		/// <summary>
		/// 生成实体过滤表达式。
		/// </summary>
		/// <param name="output">输入缓冲区。</param>
		private void GenerateEntityFilterExpression(StringBuilder output)
		{
			// 取实体列的第一个列
			String columnName = EntityColumnFullNames[0];

			if (!Negative)
			{
				output.AppendFormat("{0} IS NOT NULL", columnName);
			}
			else
			{
				output.AppendFormat("{0} IS NULL", columnName);
			}
		}
	}
}