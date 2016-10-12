#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ExpressionSchemaBuilderFactory.cs
// 文件功能描述：用于生成表达式架构生成器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110708
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于生成表达式架构生成器。
	/// </summary>
	internal static class ExpressionSchemaBuilderFactory
	{
		/// <summary>
		/// 创建指定类型的表达式架构生成器。
		/// </summary>
		/// <param name="propertyDef">复合实体属性定义。</param>
		/// <param name="parameterPrefix">查询参数前缀。</param>
		/// <returns>表达式架构生成器。</returns>
		public static ExpressionSchemaBuilder Create(CompositeForeignReferencePropertyDefinition propertyDef, String parameterPrefix)
		{
			ExpressionSchemaType type = GetExpressionSchemaType(propertyDef.Type);

			switch (type)
			{
				case ExpressionSchemaType.Entity:
					return new EntityExpressionSchemaBuilder(propertyDef, parameterPrefix);

				case ExpressionSchemaType.Group:
					return new GroupExpressionSchemaBuilder(propertyDef, parameterPrefix);

				case ExpressionSchemaType.Unknown:
				default:
					Debug.Fail(String.Format("目标类型 {0} 的表达式架构类型不可识别。", propertyDef.Type.FullName));
					return null;
			}
		}

		/// <summary>
		/// 获取目标类型的表达式架构类型。
		/// </summary>
		/// <param name="targetType">目标类型。</param>
		/// <returns>表达式架构类型。</returns>
		internal static ExpressionSchemaType GetExpressionSchemaType(Type targetType)
		{
			if (Attribute.IsDefined(targetType, typeof(TableAttribute)))
			{
				return ExpressionSchemaType.Entity;
			}
			else if (Attribute.IsDefined(targetType, typeof(GroupAttribute)))
			{
				return ExpressionSchemaType.Group;
			}
			else
			{
				return ExpressionSchemaType.Unknown;
			}
		}

		#region 辅助方法

		#endregion
	}
}