#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：BetweenFilter.cs
// 文件功能描述：表示 BETWEEN 过滤表达式。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20100815
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
	/// 表示 BETWEEN 过滤表达式。
	/// </summary>
	[Serializable]
	public class BetweenFilter : Filter
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性值和左、右边界的属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		public BetweenFilter(String propertyName, Object from, Object to)
			: base(propertyName, new Object[] { from, to })
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称、值属性名称和左、右边界的属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		public BetweenFilter(String entityPropertyName, String propertyName, Object from, Object to)
			: base(entityPropertyName, propertyName, new Object[] { from, to })
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径、值属性名称和左、右边界的属性值。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="from">左边界的属性值。</param>
		/// <param name="to">右边界的属性值。</param>
		public BetweenFilter(IList<String> propertyPath, Object from, Object to)
			: base(propertyPath, new Object[] { from, to })
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

			output.Append(ColumnFullName);

			if (!Negative)
			{
				output.Append(BETWEEN);
			}
			else
			{
				output.Append(NOT_BETWEEN);
			}

			output.Append(ComposeParameter(Parameters[0].Name));

			output.Append(AND);

			output.Append(ComposeParameter(Parameters[1].Name));
		}
	}
}