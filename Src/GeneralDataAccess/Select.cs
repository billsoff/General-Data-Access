#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Select.cs
// 文件功能描述：用于选择属性的辅助类。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110524
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
	/// 用于选择属性的辅助类。
	/// </summary>
	public static class Select
	{
		#region 操作方法

		/// <summary>
		/// 指示不选择任何属性。
		/// </summary>
		public static CompositeBuilderStrategy Nothing
		{
			get { return CompositeBuilderStrategyFactory.Nothing; }
		}

		/// <summary>
		/// 选择目标实体中的所有属性。
		/// </summary>
		/// <param name="entityType">目标实体类型。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression AllFrom(Type entityType)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllFrom(entityType);

			return expression;
		}

		/// <summary>
		/// 选择外部引用列表中的所有属性。
		/// </summary>
		/// <param name="allChains">属性链列表，其中的元素必须是外部引用。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression AllFrom(params IPropertyChain[] allChains)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllFrom(allChains);

			return expression;
		}

		/// <summary>
		/// 选择外部引用列表中的所有属性。
		/// </summary>
		/// <param name="allChainBuilders">属性链生成器列表，其中的元素必须生成外部引用属性链。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression AllFrom(params IPropertyChainBuilder[] allChainBuilders)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllFrom(allChainBuilders);

			return expression;
		}

		/// <summary>
		/// 选择目标实体中的所有非延迟加载属性。
		/// </summary>
		/// <param name="entityType">目标实体类型。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression AllExceptLazyLoadFrom(Type entityType)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllExceptLazyLoadFrom(entityType);

			return expression;
		}

		/// <summary>
		/// 选择外部引用列表中的所有非延迟加载属性。
		/// </summary>
		/// <param name="allChains">属性链列表，其中的元素必须是外部引用。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression AllExceptLazyLoadFrom(params IPropertyChain[] allChains)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllExceptLazyLoadFrom(allChains);

			return expression;
		}

		/// <summary>
		/// 选择外部引用列表中的所有非延迟加载属性。
		/// </summary>
		/// <param name="allChainBuilders">属性链生成器列表，其中的元素必须是外部引用。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression AllExceptLazyLoadFrom(params IPropertyChainBuilder[] allChainBuilders)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllExceptLazyLoadFrom(allChainBuilders);

			return expression;
		}

		/// <summary>
		/// 选择一项属性。
		/// </summary>
		/// <param name="targetType">目标类型。</param>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression Property(Type targetType, String[] propertyPath)
		{
			return Property(new PropertyChain(targetType, propertyPath));
		}

		/// <summary>
		/// 选择一项属性。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression Property(IPropertyChainBuilder builder)
		{
			return Property(builder.Build());
		}

		/// <summary>
		/// 选择一项属性。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression Property(IPropertyChain chain)
		{
			SelectExpression expression = new SelectExpression();

			expression.Property(chain);

			return expression;
		}

		/// <summary>
		/// 选择属性。
		/// </summary>
		/// <param name="allChains">属性链列表，其中的元素必须为值属性。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression Properties(params IPropertyChain[] allChains)
		{
			SelectExpression expression = new SelectExpression();

			expression.Properties(allChains);

			return expression;
		}

		/// <summary>
		/// 选择属性。
		/// </summary>
		/// <param name="allChainBuilders">属性链生成器列表。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression Properties(params IPropertyChainBuilder[] allChainBuilders)
		{
			SelectExpression expression = new SelectExpression();

			expression.Properties(allChainBuilders);

			return expression;
		}

		/// <summary>
		/// 选择目标实体中的主键属性。
		/// </summary>
		/// <param name="entityType">目标实体类型，不能为空。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression PrimaryKeyOf(Type entityType)
		{
			SelectExpression expression = new SelectExpression();

			expression.PrimaryKeyOf(entityType);

			return expression;
		}

		/// <summary>
		/// 选择所有外部引用中的主键属性。
		/// </summary>
		/// <param name="allChains">属性链列表，其中的元素必须映射为引用属性。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression PrimaryKeysOf(params IPropertyChain[] allChains)
		{
			SelectExpression expression = new SelectExpression();

			expression.PrimaryKeysOf(allChains);

			return expression;
		}

		/// <summary>
		/// 选择所有外部引用中的主键属性。
		/// </summary>
		/// <param name="allChainBuilders">属性链生成器列表，其中的元素生成引用属性。</param>
		/// <returns>选择表达式。</returns>
		public static SelectExpression PrimaryKeysOf(params IPropertyChainBuilder[] allChainBuilders)
		{
			SelectExpression expression = new SelectExpression();

			expression.PrimaryKeysOf(allChainBuilders);

			return expression;
		}

		#endregion
	}
}