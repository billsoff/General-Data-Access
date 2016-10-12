#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：OrderByExpression.cs
// 文件功能描述：用于构建排序器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110328
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
	/// 用于构建排序器。
	/// </summary>
	[Serializable]
	public sealed class OrderByExpression
	{
		#region 私有字段

		private readonly List<SortExpression> m_sortExpressions = new List<SortExpression>();

		private readonly List<String> PropertyPath = new List<String>();
		private SortMethod m_sortMethod = SortMethod.Ascending;

		private ExpressionTokens m_currentToken = ExpressionTokens.Nothing;

		#endregion

		#region 操作

		/// <summary>
		/// 外部引用属性名称。
		/// </summary>
		/// <param name="entityPropertyName">外部引用属性名称。</param>
		/// <returns>当前实例。</returns>
		public OrderByExpression ForeignRef(String entityPropertyName)
		{
			OrderByExpression expression = TryFlush();

			ExpressionTokens token = ExpressionTokens.ForeignRef;

			OnProcessing(token);

			PropertyPath.Add(entityPropertyName);
			m_sortMethod = SortMethod.Ascending;

			OnProcessed(token);

			return expression;
		}

		/// <summary>
		/// 值属性名称。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>当前实例。</returns>
		public OrderByExpression Property(String propertyName)
		{
			OrderByExpression expression = TryFlush();

			ExpressionTokens token = ExpressionTokens.Property;

			OnProcessing(token);

			PropertyPath.Add(propertyName);
			m_sortMethod = SortMethod.Ascending;

			OnProcessed(token);

			return expression;
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>当前实例。</returns>
		public OrderByExpression Property(IPropertyChainBuilder builder)
		{
			return Locator(builder);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>当前实例。</returns>
		public OrderByExpression Property(IPropertyChain chain)
		{
			return Locator(chain);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>当前实例。</returns>
		public OrderByExpression Property(IList<String> propertyPath)
		{
			return Locator(propertyPath);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>当前实例。</returns>
		public OrderByExpression Locator(IPropertyChainBuilder builder)
		{
			return Locator(builder.Build());
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>当前实例。</returns>
		public OrderByExpression Locator(IPropertyChain chain)
		{
			return Locator(chain.PropertyPath);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>当前实例。</returns>
		public OrderByExpression Locator(IList<String> propertyPath)
		{
			OrderByExpression expression = TryFlush();

			ExpressionTokens token = ExpressionTokens.Locator;

			OnProcessing(token);

			this.PropertyPath.AddRange(propertyPath);
			m_sortMethod = SortMethod.Ascending;

			OnProcessed(token);

			return expression;
		}

		/// <summary>
		/// 升序排序，这是默认值。
		/// </summary>
		public OrderByExpression Ascending
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Ascending;

				OnProcessing(token);

				m_sortMethod = SortMethod.Ascending;

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// 降序排序。
		/// </summary>
		public OrderByExpression Descending
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Descending;

				OnProcessing(token);

				m_sortMethod = SortMethod.Descending;

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// 下一个要排序的属性。
		/// </summary>
		public OrderByExpression Then
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Then;

				OnProcessing(token);

				MakeSortExpression();

				OnProcessed(token);

				return this;
			}
		}

		#region 过滤属性的一些快捷方式

		/// <summary>
		/// “Id”属性。
		/// </summary>
		public OrderByExpression Id
		{
			get { return Property(WellKnown.Id); }
		}

		/// <summary>
		/// “Name”属性。
		/// </summary>
		public OrderByExpression Name
		{
			get { return Property(WellKnown.Name); }
		}

		/// <summary>
		/// “FullName”属性。
		/// </summary>
		public OrderByExpression FullName
		{
			get { return Property(WellKnown.FullName); }
		}

		/// <summary>
		/// “DisplayName”属性。
		/// </summary>
		public OrderByExpression DisplayName
		{
			get { return Property(WellKnown.DisplayName); }
		}

		/// <summary>
		/// “DisplayOrder”属性。
		/// </summary>
		public OrderByExpression DisplayOrder
		{
			get { return Property(WellKnown.DisplayOrder); }
		}

		/// <summary>
		/// “Description”属性。
		/// </summary>
		public OrderByExpression Description
		{
			get { return Property(WellKnown.Description); }
		}

		/// <summary>
		/// “Active”属性。
		/// </summary>
		public OrderByExpression Active
		{
			get { return Property(WellKnown.Active); }
		}

		/// <summary>
		/// “Deactive”属性。
		/// </summary>
		public OrderByExpression Deactive
		{
			get { return Property(WellKnown.Deactive); }
		}

		/// <summary>
		/// “Female”属性。
		/// </summary>
		public OrderByExpression Female
		{
			get { return Property(WellKnown.Female); }
		}

		/// <summary>
		/// “Male”属性。
		/// </summary>
		public OrderByExpression Male
		{
			get { return Property(WellKnown.Male); }
		}

		/// <summary>
		/// “NavigateUrl”属性。
		/// </summary>
		public OrderByExpression NavigateUrl
		{
			get { return Property(WellKnown.NavigateUrl); }
		}

		/// <summary>
		/// “TimeCreated”属性。
		/// </summary>
		public OrderByExpression TimeCreated
		{
			get { return Property(WellKnown.TimeCreated); }
		}

		/// <summary>
		/// “TimeModified”属性。
		/// </summary>
		public OrderByExpression TimeModified
		{
			get { return Property(WellKnown.TimeModified); }
		}

		/// <summary>
		/// 外部引用“Category”属性。
		/// </summary>
		public OrderByExpression Category
		{
			get { return ForeignRef(WellKnown.Category); }
		}

		/// <summary>
		/// 外部引用“Organization”属性。
		/// </summary>
		public OrderByExpression Organization
		{
			get { return ForeignRef(WellKnown.Organization); }
		}

		/// <summary>
		/// 外部引用“Owner”属性。
		/// </summary>
		public OrderByExpression Owner
		{
			get { return ForeignRef(WellKnown.Owner); }
		}

		/// <summary>
		/// 外部引用“Parent”属性。
		/// </summary>
		public OrderByExpression Parent
		{
			get { return ForeignRef(WellKnown.Parent); }
		}

		/// <summary>
		/// 外部引用“Region”属性。
		/// </summary>
		public OrderByExpression Region
		{
			get { return ForeignRef(WellKnown.Region); }
		}

		/// <summary>
		/// 外部引用“Role”属性。
		/// </summary>
		public OrderByExpression Role
		{
			get { return ForeignRef(WellKnown.Role); }
		}

		/// <summary>
		/// 外部引用“User”属性。
		/// </summary>
		public OrderByExpression User
		{
			get { return ForeignRef(WellKnown.User); }
		}

		#endregion

		/// <summary>
		/// 重置，以备重新使用。
		/// </summary>
		public void Reset()
		{
			m_currentToken = ExpressionTokens.Nothing;

			PropertyPath.Clear();
			m_sortMethod = SortMethod.Ascending;

			m_sortExpressions.Clear();
		}

		#endregion

		#region 解析

		/// <summary>
		/// 解析，获得排序器。
		/// </summary>
		/// <returns>生成的排序器。</returns>
		public Sorter Resolve()
		{
			ExpressionTokens nextValidTokens = GetNextValidTokens();

			if ((nextValidTokens & ExpressionTokens.Finish) != 0)
			{
				// 生成最后一个排序表达式
				MakeSortExpression();

				Sorter result = null;

				if (m_sortExpressions.Count != 0)
				{
					result = new Sorter(m_sortExpressions.ToArray());
				}

				return result;
			}
			else
			{
				throw new InvalidOperationException("排序表达式不完整，正确的格式为：[ForeignRef(...).]Property(...)[.{Ascending|Descending}][.Then...]");
			}
		}

		#endregion

		#region 类型强制

		/// <summary>
		/// 从 OrderByExpression 到 Sorter 的类型强制转换。
		/// </summary>
		/// <param name="expression">OrderByExpression 实例。</param>
		/// <returns>OrderByExpression 解析得到的结果。</returns>
		public static implicit operator Sorter(OrderByExpression expression)
		{
			return expression.Resolve();
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 生成排序表达式。
		/// </summary>
		private void MakeSortExpression()
		{
			ExpressionTokens availableTokens = ExpressionTokens.Property
					| ExpressionTokens.Locator
					| ExpressionTokens.Ascending
					| ExpressionTokens.Descending;

			if (((m_currentToken & availableTokens) != 0) && (PropertyPath.Count != 0))
			{
				SortExpression expression = new SortExpression(PropertyPath.ToArray(), m_sortMethod);

				m_sortExpressions.Add(expression);

				// 清理
				PropertyPath.Clear();
				m_sortMethod = SortMethod.Ascending;
			}
		}

		/// <summary>
		/// 处理词法元素前调用。
		/// </summary>
		/// <param name="tokenAppending"></param>
		private void OnProcessing(ExpressionTokens tokenAppending)
		{
			SyntaxCheck(tokenAppending);
		}

		/// <summary>
		/// 进行语法检查。
		/// </summary>
		/// <param name="tokenAppending"></param>
		[Conditional("DEBUG")]
		private void SyntaxCheck(ExpressionTokens tokenAppending)
		{
			ExpressionTokens validTokens = GetNextValidTokens();

			if ((validTokens & tokenAppending) == 0)
			{
				throw new InvalidOperationException("排序表达式不完整，正确的格式为：[ForeignRef(...).]Property(...)[.{Ascending|Descending}][[.Then]...]");
			}
		}

		/// <summary>
		/// 完成词法元素处理后调用。
		/// </summary>
		/// <param name="tokenAppended">附加的词法元素。</param>
		private void OnProcessed(ExpressionTokens tokenAppended)
		{
			m_currentToken = tokenAppended;
		}

		/// <summary>
		/// 获取下一个合法的词法元素。
		/// </summary>
		/// <returns>下一个合法的词法元素。</returns>
		private ExpressionTokens GetNextValidTokens()
		{
			ExpressionTokens nextValidToken = ExpressionTokens.Nothing;

			switch (m_currentToken)
			{
				case ExpressionTokens.Start:
					nextValidToken = ExpressionTokens.ForeignRef
						| ExpressionTokens.Property
						| ExpressionTokens.Locator
						| ExpressionTokens.Finish;
					break;

				case ExpressionTokens.ForeignRef:
					nextValidToken = ExpressionTokens.Property;
					break;

				case ExpressionTokens.Locator:
				case ExpressionTokens.Property:
					nextValidToken = ExpressionTokens.Ascending
						| ExpressionTokens.Descending
						| ExpressionTokens.Then
						| ExpressionTokens.Finish;
					break;

				case ExpressionTokens.Ascending:
				case ExpressionTokens.Descending:
					nextValidToken = ExpressionTokens.Then | ExpressionTokens.Finish;
					break;

				case ExpressionTokens.Then:
					nextValidToken = ExpressionTokens.ForeignRef | ExpressionTokens.Property | ExpressionTokens.Locator;
					break;

				default:
					break;
			}

			return nextValidToken;
		}

		/// <summary>
		/// 尝试计算当前的排序表达式。
		/// </summary>
		/// <returns>当前实例。</returns>
		private OrderByExpression TryFlush()
		{
			OrderByExpression expression = this;

			ExpressionTokens nextValidTokens = GetNextValidTokens();

			if ((nextValidTokens & ExpressionTokens.Then) != 0)
			{
				expression = Then;
			}

			return expression;
		}

		#endregion
	}
}