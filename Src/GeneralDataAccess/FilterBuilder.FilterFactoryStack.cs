#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterFactoryStack.cs
// 文件功能描述：用于存放过滤器工厂。
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
	partial class FilterBuilder
	{
		/// <summary>
		/// 用于存放过滤器工厂。
		/// </summary>
		[Serializable]
		private class FilterFactoryStack : IFilterFactoryOperands
		{
			#region 私有字段

			private readonly Stack<FilterFactory> m_filterFactories = new Stack<FilterFactory>();

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取一个值，该值指示过滤器工厂栈是否为空。
			/// </summary>
			public Boolean IsEmpty
			{
				get { return (m_filterFactories.Count == 0); }
			}

			/// <summary>
			/// 获取栈顶过滤器工厂（不弹出栈）。
			/// </summary>
			public FilterFactory Top
			{
				get
				{
					if (!IsEmpty)
					{
						return m_filterFactories.Peek();
					}
					else
					{
						return null;
					}
				}
			}

			#endregion

			#region 公共访法

			/// <summary>
			/// 将过滤器工厂压入栈。
			/// </summary>
			/// <param name="factory">过滤器工厂。</param>
			public void Push(FilterFactory factory)
			{
				m_filterFactories.Push(factory);
			}

			/// <summary>
			/// 清空过滤器工厂栈。
			/// </summary>
			public void Clear()
			{
				m_filterFactories.Clear();
			}

			#endregion

			#region IFilterFactoryOperands 成员

			/// <summary>
			/// 获取过滤器工厂参数。
			/// </summary>
			/// <returns></returns>
			public FilterFactory Pop()
			{
				return m_filterFactories.Pop();
			}

			#endregion
		}
	}
}