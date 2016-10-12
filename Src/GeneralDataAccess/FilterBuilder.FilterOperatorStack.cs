#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterOperatorStack.cs
// 文件功能描述：用于存放过滤器操作符。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110325。
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
	partial class FilterBuilder
	{
		/// <summary>
		/// 用于存放过滤器操作符。
		/// </summary>
		[Serializable]
		private class FilterOperatorStack
		{
			#region 私有字段

			private readonly Stack<FilterOperator> m_operators = new Stack<FilterOperator>();

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取一个值，该值指示操作符栈是否为空。
			/// </summary>
			public Boolean IsEmpty
			{
				get { return (m_operators.Count == 0); }
			}

			/// <summary>
			/// 获取栈顶的操作符（不弹出栈）。
			/// </summary>
			public FilterOperator Top
			{
				get
				{
					if (!IsEmpty)
					{
						return m_operators.Peek();
					}
					else
					{
						return null;
					}
				}
			}

			#endregion

			#region 公共方法

			/// <summary>
			/// 弹出栈顶的操作符。
			/// </summary>
			/// <returns>操作符。</returns>
			public FilterOperator Pop()
			{
				if (!IsEmpty)
				{
					return m_operators.Pop();
				}
				else
				{
					return null;
				}
			}

			/// <summary>
			/// 操作符压栈。
			/// </summary>
			/// <param name="op">操作符。</param>
			public void Push(FilterOperator op)
			{
				m_operators.Push(op);
			}

			/// <summary>
			/// 清空操作符栈。
			/// </summary>
			public void Clear()
			{
				m_operators.Clear();
			}

			#endregion
		}
	}
}