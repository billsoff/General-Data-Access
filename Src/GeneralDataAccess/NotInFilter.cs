#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NotInFilter.cs
// 文件功能描述：表示 NOT IN 过滤条件，支持实体过滤。
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
	/// 表示 NOT IN 过滤条件，支持实体过滤。
	/// </summary>
	[Serializable]
	public class NotInFilter : Filter
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性名称和值集合。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="discreteValues">属性值集合。</param>
		public NotInFilter(String propertyName, Object[] discreteValues)
			: base(propertyName, discreteValues)
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称、值属性名称和值集合。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="discreteValues">值集合。</param>
		public NotInFilter(String entityPropertyName, String propertyName, Object[] discreteValues)
			: base(entityPropertyName, propertyName, discreteValues)
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径和值集合。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="discreteValues">值集合。</param>
		public NotInFilter(IList<String> propertyPath, Object[] discreteValues)
			: base(propertyPath, discreteValues)
		{
		}

		/// <summary>
		/// 构造函数，设置属性名称和值集合。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="builder">查询列表生成器。</param>
		internal NotInFilter(String propertyName, QueryListBuilder builder)
			: base(propertyName, builder)
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称、值属性名称和值集合。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="builder">查询列表生成器。</param>
		internal NotInFilter(String entityPropertyName, String propertyName, QueryListBuilder builder)
			: base(entityPropertyName, propertyName, builder)
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径和值集合。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="builder">查询列表生成器。</param>
		internal NotInFilter(IList<String> propertyPath, QueryListBuilder builder)
			: base(propertyPath, builder)
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

			if (QueryListBuilder != null)
			{
				GenerateQueryFilterExpression(output);
			}
			else if ((!IsEntityFilter && (Parameters.Length == 0)) || (IsEntityFilter && (EntityParameters.Length == 0)))
			{
				if (!Negative)
				{
					output.Append("1=1");
				}
				else
				{
					output.Append("1<>1");
				}
			}
			else if (!IsEntityFilter)
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
				output.Append(NOT_IN);
			}
			else
			{
				output.Append(IN);
			}

			output.Append(LEFT_BRACKET);

			for (int i = 0; i < Parameters.Length; i++)
			{
				if (i > 0)
				{
					output.Append(COMMA);
				}

				output.Append(ComposeParameter(Parameters[i].Name));
			}

			output.Append(RIGHT_BRACKET);
		}

		/// <summary>
		/// 生成实体过滤表达式。
		/// </summary>
		/// <param name="output">输入缓冲区。</param>
		private void GenerateEntityFilterExpression(StringBuilder output)
		{
			if (EntityColumnFullNames.Length > 1)
			{
				GenerateComproundColumnEntityFilterExpression(output);
			}
			else
			{
				GenerateSingleColumnEntityFilterExpression(output);
			}
		}

		/// <summary>
		/// 创建复合列实体过滤表达式。
		/// </summary>
		/// <param name="output">输出缓冲区。</param>
		private void GenerateComproundColumnEntityFilterExpression(StringBuilder output)
		{
			List<String> expressions = new List<String>();

			String compareOperator = (!Negative ? "<>" : "=");

			foreach (QueryParameterCollection parameters in EntityParameters)
			{
				expressions.Add(ComposeEntityFilterExpression(parameters, compareOperator));
			}

			if (expressions.Count == 1)
			{
				output.Append(expressions[0]);
			}
			else if (!Negative)
			{
				// 用 AND 连接
				for (Int32 i = 0; i < expressions.Count; i++)
				{
					if (i != 0)
					{
						output.Append(AND);
					}

					output.Append(expressions[i]);
				}
			}
			else
			{
				// 用 OR 连接
				for (Int32 i = 0; i < expressions.Count; i++)
				{
					if (i != 0)
					{
						output.Append(OR);
					}

					output.AppendFormat("{0}{1}{2}", LEFT_BRACKET, expressions[i], RIGHT_BRACKET);
				}
			}
		}

		/// <summary>
		/// 创建单一列实体过滤表达式。
		/// </summary>
		/// <param name="output">输出缓冲区。</param>
		private void GenerateSingleColumnEntityFilterExpression(StringBuilder output)
		{
			output.Append(this.EntityColumnFullNames[0]);

			if (!Negative)
			{
				output.Append(NOT_IN);
			}
			else
			{
				output.Append(IN);
			}

			output.Append(LEFT_BRACKET);

			for (Int32 i = 0; i < EntityParameters.Length; i++)
			{
				if (i > 0)
				{
					output.Append(COMMA);
				}

				output.Append(ComposeParameter(EntityParameters[i][0].Name));
			}

			output.Append(RIGHT_BRACKET);
		}

		/// <summary>
		/// 生成查询列表过滤器表达式。
		/// </summary>
		/// <param name="output">输出缓冲区。</param>
		private void GenerateQueryFilterExpression(StringBuilder output)
		{
			String columnName;

			if (IsEntityFilter)
			{
				columnName = this.EntityColumnFullNames[0];
			}
			else
			{
				columnName = this.ColumnFullName;
			}

			output.Append(columnName);

			if (!Negative)
			{
				output.Append(NOT_IN);
			}
			else
			{
				output.Append(IN);
			}

			output.Append(LEFT_BRACKET);
			output.Append(QueryListSqlStatement);
			output.Append(RIGHT_BRACKET);
		}

		/// <summary>
		/// 合成实体过滤表达式。
		/// </summary>
		/// <param name="parameters">实体列的参数集合。</param>
		/// <param name="compareOperator">比较操作符。</param>
		/// <returns>合成好的过滤表达式。</returns>
		private String ComposeEntityFilterExpression(QueryParameterCollection parameters, String compareOperator)
		{
			List<String> expressions = new List<String>();

			for (Int32 i = 0; i < EntityColumnFullNames.Length; i++)
			{
				String columnFullName = EntityColumnFullNames[i];
				String paramName = ComposeParameter(parameters[i].Name);

				expressions.Add(String.Format("{0}{1}{2}", columnFullName, compareOperator, paramName));
			}

			if (expressions.Count == 1)
			{
				return expressions[0];
			}
			else
			{
				StringBuilder buffer = new StringBuilder();

				foreach (String e in expressions)
				{
					if (buffer.Length != 0)
					{
						buffer.Append(AND);
					}

					buffer.AppendFormat("{0}{1}{2}", LEFT_BRACKET, e, RIGHT_BRACKET);
				}

				return buffer.ToString();
			}
		}
	}
}