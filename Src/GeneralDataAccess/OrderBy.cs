#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：OrderBy.cs
// 文件功能描述：用于生成排序器的辅助类。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于生成排序器的辅助类。
	/// </summary>
	public abstract class OrderBy
	{
		#region 操作

		/// <summary>
		/// 外部引用属性名称。
		/// </summary>
		/// <param name="entityPropertyName">外部引用属性名称。</param>
		/// <returns>新 OrderByExpression 实例。</returns>
		public static OrderByExpression ForeignRef(String entityPropertyName)
		{
			OrderByExpression expression = new OrderByExpression();

			expression.ForeignRef(entityPropertyName);

			return expression;
		}

		/// <summary>
		/// 值属性名称。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>新 OrderByExpression 实例。</returns>
		public static OrderByExpression Property(String propertyName)
		{
			OrderByExpression expression = new OrderByExpression();

			expression.Property(propertyName);

			return expression;
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>新 OrderByExpression 实例。</returns>
		public static OrderByExpression Property(IPropertyChainBuilder builder)
		{
			return Locator(builder);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>新 OrderByExpression 实例。</returns>
		public static OrderByExpression Property(IPropertyChain chain)
		{
			return Locator(chain);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>新 OrderByExpression 实例。</returns>
		public static OrderByExpression Property(IList<String> propertyPath)
		{
			return Locator(propertyPath);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>新 OrderByExpression 实例。</returns>
		public static OrderByExpression Locator(IPropertyChainBuilder builder)
		{
			return Locator(builder.Build());
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>新 OrderByExpression 实例。</returns>
		public static OrderByExpression Locator(IPropertyChain chain)
		{
			return Locator(chain.PropertyPath);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>新 OrderByExpression 实例。</returns>
		public static OrderByExpression Locator(IList<String> propertyPath)
		{
			OrderByExpression expression = new OrderByExpression();

			expression.Locator(propertyPath);

			return expression;
		}

		#region 过滤属性的一些快捷方式

		/// <summary>
		/// “Id”属性。
		/// </summary>
		public static OrderByExpression Id
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Id;
			}
		}

		/// <summary>
		/// “Name”属性。
		/// </summary>
		public static OrderByExpression Name
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Name;
			}
		}

		/// <summary>
		/// “FullName”属性。
		/// </summary>
		public static OrderByExpression FullName
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.FullName;
			}
		}

		/// <summary>
		/// “DisplayName”属性。
		/// </summary>
		public static OrderByExpression DisplayName
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.DisplayName;
			}
		}

		/// <summary>
		/// “DisplayOrder”属性。
		/// </summary>
		public static OrderByExpression DisplayOrder
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.DisplayOrder;
			}
		}

		/// <summary>
		/// “Description”属性。
		/// </summary>
		public static OrderByExpression Description
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Description;
			}
		}

		/// <summary>
		/// “Active”属性。
		/// </summary>
		public static OrderByExpression Active
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Active;
			}
		}

		/// <summary>
		/// “Deactive”属性。
		/// </summary>
		public static OrderByExpression Deactive
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Deactive;
			}
		}

		/// <summary>
		/// “Female”属性。
		/// </summary>
		public static OrderByExpression Female
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Female;
			}
		}

		/// <summary>
		/// “Male”属性。
		/// </summary>
		public static OrderByExpression Male
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Male;
			}
		}

		/// <summary>
		/// “NavigateUrl”属性。
		/// </summary>
		public static OrderByExpression NavigateUrl
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.NavigateUrl;
			}
		}

		/// <summary>
		/// “TimeCreated”属性。
		/// </summary>
		public static OrderByExpression TimeCreated
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.TimeCreated;
			}
		}

		/// <summary>
		/// “TimeModified”属性。
		/// </summary>
		public static OrderByExpression TimeModified
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.TimeModified;
			}
		}

		/// <summary>
		/// 外部引用“Category”属性。
		/// </summary>
		public static OrderByExpression Category
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Category;
			}
		}

		/// <summary>
		/// 外部引用“Organization”属性。
		/// </summary>
		public static OrderByExpression Organization
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Organization;
			}
		}

		/// <summary>
		/// 外部引用“Owner”属性。
		/// </summary>
		public static OrderByExpression Owner
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Owner;
			}
		}

		/// <summary>
		/// 外部引用“Parent”属性。
		/// </summary>
		public static OrderByExpression Parent
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Parent;
			}
		}

		/// <summary>
		/// 外部引用“Region”属性。
		/// </summary>
		public static OrderByExpression Region
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Region;
			}
		}

		/// <summary>
		/// 外部引用“Role”属性。
		/// </summary>
		public static OrderByExpression Role
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Role;
			}
		}

		/// <summary>
		/// 外部引用“User”属性。
		/// </summary>
		public static OrderByExpression User
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.User;
			}
		}

		#endregion

		#endregion
	}
}