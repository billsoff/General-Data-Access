#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：LessThanFilter.cs
// 文件功能描述：表示小于过滤条件。
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
	/// 表示小于过滤条件。
	/// </summary>
	[Serializable]
	public class LessThanFilter : Filter
	{
		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性名称和属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		public LessThanFilter(String propertyName, Object propertyValue)
			: base(propertyName, new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称和实体中值属性名称以及属性值。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">实体属性中的值属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		public LessThanFilter(String entityPropertyName, String propertyName, Object propertyValue)
			: base(entityPropertyName, propertyName, new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// 构造函数，设置属性路径和属性值。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="propertyValue">属性值。</param>
		public LessThanFilter(IList<String> propertyPath, Object propertyValue)
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

			output.Append(ColumnFullName);

			if (!Negative)
			{
				output.Append("<");
			}
			else
			{
				output.Append(">=");
			}

			output.Append(ComposeParameter(Parameters[0].Name));
		}
	}
}