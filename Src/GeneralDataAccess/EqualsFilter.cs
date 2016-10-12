#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EqualsFilter.cs
// 文件功能描述：表示相等条件过滤器，支持实体过滤。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008151436
//
// 修改标识：宋冰 201009030850
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
	/// 表示相等条件过滤器，支持实体过滤。
	/// </summary>
	[Serializable]
	public class EqualsFilter : Filter
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性名称和属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		public EqualsFilter(String propertyName, Object propertyValue)
			: base(propertyName, new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		public EqualsFilter(String entityPropertyName, String propertyName, Object propertyValue)
			: base(entityPropertyName, propertyName, new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径和属性值。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="propertyValue">属性值。</param>
		public EqualsFilter(IList<String> propertyPath, Object propertyValue)
			: base(propertyPath, new Object[] { propertyValue })
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
			output.Append(ColumnFullName);

			if (!Negative)
			{
				output.Append("=");
			}
			else
			{
				output.Append("<>");
			}

			output.Append(ComposeParameter(Parameters[0].Name));
		}

		/// <summary>
		/// 生成实体过滤表达式。
		/// </summary>
		/// <param name="output">输入缓冲区。</param>
		private void GenerateEntityFilterExpression(StringBuilder output)
		{
			String compareOperator = (!Negative ? "=" : "<>");

			List<String> expressions = new List<String>();

			QueryParameterCollection parameters = EntityParameters[0];

			for (Int32 i = 0; i < EntityColumnFullNames.Length; i++)
			{
				String columnFullName = EntityColumnFullNames[i];
				String paramName = ComposeParameter(parameters[i].Name);

				expressions.Add(String.Format("{0}{1}{2}", columnFullName, compareOperator, paramName));
			}

			if (expressions.Count == 1)
			{
				output.Append(expressions[0]);
			}
			else
			{
				for (Int32 i = 0; i < expressions.Count; i++)
				{
					if (i != 0)
					{
						output.Append(AND);
					}

					output.AppendFormat("{0}{1}{2}", LEFT_BRACKET, expressions[i], RIGHT_BRACKET);
				}
			}
		}
	}
}