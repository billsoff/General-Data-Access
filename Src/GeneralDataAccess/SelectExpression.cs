#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：SelectExpression.cs
// 文件功能描述：选择表达式，用于选择要加载的属性。
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
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 选择表达式，用于选择要加载的属性。
	/// </summary>
	[Serializable]
	public class SelectExpression
	{
		#region 私有字段

		private readonly List<PropertySelector> m_selectors = new List<PropertySelector>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public SelectExpression()
		{
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取一个值，该值指示是否存在属性选择器。
		/// </summary>
		public Boolean HasSelectors
		{
			get { return (m_selectors.Count != 0); }
		}

		/// <summary>
		/// 获取所有的属性选择器。
		/// </summary>
		public PropertySelector[] Selectors
		{
			get { return m_selectors.ToArray(); }
		}

		/// <summary>
		/// 获取属性选择器。
		/// </summary>
		public PropertySelector Selector
		{
			get { return m_selectors[0]; }
		}

		#endregion

		#region 操作方法

		/// <summary>
		/// 选择目标实体中的所有属性。
		/// </summary>
		/// <param name="entityType">目标实体类型。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression AllFrom(Type entityType)
		{
			#region 前置条件

			Debug.Assert((entityType != null), "目标实体类型参数 entityType 不能为空。");

			#endregion

			m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllFromSchema, entityType));

			return this;
		}

		/// <summary>
		/// 选择外部引用列表中的所有属性。
		/// </summary>
		/// <param name="allChains">属性链列表，其中的元素必须是外部引用。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression AllFrom(params IPropertyChain[] allChains)
		{
			#region 前置条件

#if DEBUG

			Debug.Assert((allChains != null) && (allChains.Length != 0), "属性链列表参数 allChains 不能为空或空列表。");

			foreach (IPropertyChain chain in allChains)
			{
				Debug.Assert(chain != null, "属性链列表中包含 null 元素。");
				Debug.Assert(!chain.IsPrimitive && !chain.IsChildren, String.Format("属性链 {0} 不是外部引用属性或目标实体。", chain.FullName));
			}

#endif

			#endregion

			foreach (IPropertyChain chain in allChains)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllFromSchema, chain));
			}

			return this;
		}


		/// <summary>
		/// 选择外部引用列表中的所有属性。
		/// </summary>
		/// <param name="allChainBuilders">属性链生成器列表，其中的元素必须生成外部引用属性链。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression AllFrom(params IPropertyChainBuilder[] allChainBuilders)
		{
			#region 前置条件

#if DEBUG

			Debug.Assert((allChainBuilders != null) && (allChainBuilders.Length != 0), "属性链列表参数 allChains 不能为空或空列表。");

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				Debug.Assert(builder != null, "属性链列表中包含 null 元素。");

				IPropertyChain chain = builder.Build();

				Debug.Assert(!chain.IsPrimitive && !chain.IsChildren, String.Format("属性链 {0} 不是外部引用属性或目标实体。", chain.FullName));
			}

#endif

			#endregion

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllFromSchema, builder));
			}

			return this;
		}

		/// <summary>
		/// 选择目标实体中的所有非延迟加载属性。
		/// </summary>
		/// <param name="entityType">目标实体类型。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression AllExceptLazyLoadFrom(Type entityType)
		{
			#region 前置条件

			Debug.Assert((entityType != null), "目标实体类型参数 entityType 不能为空。");

			#endregion

			m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllExceptLazyLoadFromSchema, entityType));

			return this;
		}

		/// <summary>
		/// 选择外部引用列表中的所有非延迟加载属性。
		/// </summary>
		/// <param name="allChains">属性链列表，其中的元素必须是外部引用。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression AllExceptLazyLoadFrom(params IPropertyChain[] allChains)
		{
			#region 前置条件

#if DEBUG

			Debug.Assert((allChains != null) && (allChains.Length != 0), "属性链列表参数 allChains 不能为空或空列表。");

			foreach (IPropertyChain chain in allChains)
			{
				Debug.Assert(chain != null, "属性链列表中包含 null 元素。");
				Debug.Assert(!chain.IsPrimitive && !chain.IsChildren, String.Format("属性链 {0} 不是外部引用属性或目标实体。", chain.FullName));
			}

#endif

			#endregion

			foreach (IPropertyChain chain in allChains)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllExceptLazyLoadFromSchema, chain));
			}

			return this;
		}

		/// <summary>
		/// 选择外部引用列表中的所有非延迟加载属性。
		/// </summary>
		/// <param name="allChainBuilders">属性链列表，其中的元素必须是外部引用。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression AllExceptLazyLoadFrom(params IPropertyChainBuilder[] allChainBuilders)
		{
			#region 前置条件

#if DEBUG

			Debug.Assert((allChainBuilders != null) && (allChainBuilders.Length != 0), "属性链列表参数 allChainBuilders 不能为空或空列表。");

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				Debug.Assert(builder != null, "属性链生成器列表中包含 null 元素。");

				IPropertyChain chain = builder.Build();

				Debug.Assert(!chain.IsPrimitive && !chain.IsChildren, String.Format("属性链 {0} 不是外部引用属性或目标实体。", chain.FullName));
			}

#endif

			#endregion

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllExceptLazyLoadFromSchema, builder));
			}

			return this;
		}

		/// <summary>
		/// 选择一项属性。
		/// </summary>
		/// <param name="targetType">目标类型。</param>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression Property(Type targetType, String[] propertyPath)
		{
			#region 前置条件

			Debug.Assert(targetType != null, "目标类型参数 targetType 不能为空。");
			Debug.Assert((propertyPath != null) && (propertyPath.Length != 0), "属性路径参数 propertyPath 不能为空或空列表。");

			#endregion

			return Property(new PropertyChain(targetType, propertyPath));
		}

		/// <summary>
		/// 选择一项属性。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression Property(IPropertyChainBuilder builder)
		{
			#region 前置条件

			Debug.Assert(builder != null, "属性链生成器参数 builder 不能为空。");

			#endregion

			return Property(builder.Build());
		}

		/// <summary>
		/// 选择一项属性。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression Property(IPropertyChain chain)
		{
			#region 前置条件

			Debug.Assert(chain != null, "属性链参数 chain 不能为空。");

			#endregion

			m_selectors.Add(PropertySelector.Create(chain));

			return this;
		}

		/// <summary>
		/// 选择属性。
		/// </summary>
		/// <param name="allChains">属性链列表。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression Properties(params IPropertyChain[] allChains)
		{
			#region 前置条件

			Debug.Assert(((allChains != null) && (allChains.Length != 0)), "属性链列表参数 allChains 不能为空或空列表。");

			#endregion

			foreach (IPropertyChain chain in allChains)
			{
				m_selectors.Add(PropertySelector.Create(chain));
			}

			return this;
		}

		/// <summary>
		/// 选择属性。
		/// </summary>
		/// <param name="allChainBuilders">属性链生成器列表。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression Properties(params IPropertyChainBuilder[] allChainBuilders)
		{
			#region 前置条件

			Debug.Assert(((allChainBuilders != null) && (allChainBuilders.Length != 0)), "属性链列表参数 allChains 不能为空或空列表。");

			#endregion

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				m_selectors.Add(PropertySelector.Create(builder));
			}

			return this;
		}

		/// <summary>
		/// 选择目标实体中的主键属性。
		/// </summary>
		/// <param name="entityType">目标实体类型，不能为空。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression PrimaryKeyOf(Type entityType)
		{
			#region 前置条件

			Debug.Assert((entityType != null), "目标实体类型参数 entityType 不能为空。");

			#endregion

			m_selectors.Add(PropertySelector.Create(PropertySelectMode.PrimaryKey, entityType));

			return this;
		}

		/// <summary>
		/// 选择所有外部引用中的主键属性。
		/// </summary>
		/// <param name="allChains">属性链列表，其中的元素必须映射为引用属性。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression PrimaryKeysOf(params IPropertyChain[] allChains)
		{
			#region 前置条件

			Debug.Assert(((allChains != null) && (allChains.Length != 0)), "属性链列表参数 allChains 不能为空或空列表。");

			#endregion

			foreach (IPropertyChain chain in allChains)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.PrimaryKey, chain));
			}

			return this;
		}

		/// <summary>
		/// 选择所有外部引用中的主键属性。
		/// </summary>
		/// <param name="allChainBuilders">属性链生成器列表，其中的元素生成引用属性。</param>
		/// <returns>当前实例。</returns>
		public SelectExpression PrimaryKeysOf(params IPropertyChainBuilder[] allChainBuilders)
		{
			#region 前置条件

			Debug.Assert(((allChainBuilders != null) && (allChainBuilders.Length != 0)), "属性链列表参数 allChains 不能为空或空列表。");

			#endregion

			foreach (IPropertyChain builder in allChainBuilders)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.PrimaryKey, builder));
			}

			return this;
		}

		#endregion

		#region 解析

		/// <summary>
		/// 获取一个值，该值指示选择表达是否可以解析（即是否选择了属性）。
		/// </summary>
		public Boolean IsResolvable
		{
			get { return (m_selectors.Count != 0); }
		}

		/// <summary>
		/// 解析，获取属性选择加载策略。
		/// </summary>
		/// <returns>CompositeBuilderStrategy 实例，属性选择加载策略。</returns>
		public CompositeBuilderStrategy Resolve()
		{
			#region 前置条件

			Debug.Assert(IsResolvable, "应至少选择一个属性。");

			#endregion

			return CompositeBuilderStrategyFactory.Create(m_selectors.ToArray());
		}

		/// <summary>
		/// 隐式强制转换，将 SelectExpression 转换为 CompositeBuilderStrategy。
		/// </summary>
		/// <param name="expression">选择表达式。</param>
		/// <returns>转换而来的 CompositeBuilderStrategy。</returns>
		public static implicit operator CompositeBuilderStrategy(SelectExpression expression)
		{
			return expression.Resolve();
		}

		/// <summary>
		/// 重置，以供后续重用。
		/// </summary>
		public void Reset()
		{
			m_selectors.Clear();
		}

		#endregion
	}
}