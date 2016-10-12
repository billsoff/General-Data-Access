#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterBuilder.cs
// 文件功能描述：用于构建过滤器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110325
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
	/// 用于构建过滤器。
	/// </summary>
	[Serializable]
	internal partial class FilterBuilder
	{
		#region 私有字段

		private readonly FilterFactoryStack m_factories = new FilterFactoryStack();
		private readonly FilterOperatorStack m_operators = new FilterOperatorStack();

		private readonly Stack<WithOperator> m_withOps = new Stack<WithOperator>();

		private Object m_lastPushed;

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取一个值，指示是否有 WithOperator 压栈。
		/// </summary>
		public Boolean HasWithToken
		{
			get { return (m_withOps.Count != 0); }
		}

		/// <summary>
		/// 获取一个值，该值指示构造器是否为重置状态。
		/// </summary>
		public Boolean IsEmpty
		{
			get { return (m_factories.IsEmpty && m_operators.IsEmpty); }
		}

		/// <summary>
		/// 获取一个值，该值指示是否可以解析。
		/// </summary>
		public Boolean IsResolvable
		{
			get
			{
				return (m_lastPushed is IFilterProvider);
			}
		}

		/// <summary>
		/// 获取过滤器工厂栈。
		/// </summary>
		public IFilterFactoryOperands FilterFactories
		{
			get
			{
				return m_factories;
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 计算最近一个 With 单元。
		/// </summary>
		/// <returns>如果栈中没有 With 操作符或表达式不完整，则返回 false，否则返回 true。</returns>
		public void EndWith()
		{
			// 双重保护，已在 FilterExpression 中进行了判断
			if (!HasWithToken || !IsResolvable)
			{
				throw new InvalidOperationException("不存在匹配的 With 操作符。");
			}

			WithOperator withOp = m_withOps.Pop();

			while ((m_operators.Top != null) && (m_operators.Top != withOp))
			{
				CalculateOnce();
			}

			// 弹出 WithOperator
			CalculateOnce();
		}

		/// <summary>
		/// 过滤器工厂压栈。
		/// </summary>
		/// <param name="factory">过滤器工厂。</param>
		public void Push(FilterFactory factory)
		{
			m_factories.Push(factory);

			m_lastPushed = factory;
		}

		/// <summary>
		/// 过滤器操作符压栈。
		/// </summary>
		/// <param name="op">操作符。</param>
		public void Push(FilterOperator op)
		{
			Compress(op.LeftPrecedence);

			m_operators.Push(op);

			m_lastPushed = op;

			WithOperator withOp = op as WithOperator;

			if (withOp != null)
			{
				m_withOps.Push(withOp);
			}
		}

		/// <summary>
		/// 重置。
		/// </summary>
		public void Reset()
		{
			m_factories.Clear();
			m_operators.Clear();

			m_lastPushed = null;

			m_withOps.Clear();
		}

		/// <summary>
		/// 解析，获取过滤器。
		/// </summary>
		/// <returns>过滤器。</returns>
		public FilterFactory Resolve()
		{
			// 双重保护，在 FilterExpression 中已进行了判断
			if (!IsResolvable)
			{
				throw new InvalidOperationException("表达式不完全，无法解析。");
			}

			while (!m_operators.IsEmpty)
			{
				CalculateOnce();
			}

			FilterFactory result = m_factories.Pop();

			Reset();

			return result;
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 计算一次。
		/// </summary>
		private void CalculateOnce()
		{
			FilterOperator op = m_operators.Pop();

			FilterFactory factory = op.Calculate();

			m_factories.Push(factory);
		}

		/// <summary>
		/// 压缩栈，执行高于目标优先级的操作符。
		/// </summary>
		/// <param name="targetPrecedence">目标优先级。</param>
		private void Compress(Int32 targetPrecedence)
		{
			while ((m_operators.Top != null) && (m_operators.Top.RightPrecedence < targetPrecedence))
			{
				CalculateOnce();
			}
		}

		#endregion
	}
}