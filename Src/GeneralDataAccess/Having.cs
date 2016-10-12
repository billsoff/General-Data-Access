#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Having.cs
// 文件功能描述：用于构建过滤器的辅助类。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110701
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
	/// 用于构建过滤器的辅助类。
	/// </summary>
	public static class Having
	{
		#region 操作符

		/// <summary>
		/// "非"操作。
		/// </summary>
		public static FilterExpression Not
		{
			get
			{
				FilterExpression expression = (new FilterExpression()).Not;

				return expression;
			}
		}

		/// <summary>
		/// “与”操作。
		/// </summary>
		public static FilterExpression And
		{
			get
			{
				FilterExpression expression = (new FilterExpression()).And;

				return expression;
			}
		}

		/// <summary>
		/// “或”操作。
		/// </summary>
		public static FilterExpression Or
		{
			get
			{
				FilterExpression expression = (new FilterExpression()).Or;

				return expression;
			}
		}

		/// <summary>
		/// 开始一个计算单元。
		/// </summary>
		public static FilterExpression With
		{
			get
			{
				FilterExpression expression = (new FilterExpression()).With;

				return expression;
			}
		}

		#endregion

		#region 过滤器工厂


		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="f">过滤器。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Filter(Filter f)
		{
			FilterExpression expression = new FilterExpression();

			expression.Filter(f);

			return expression;
		}

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Filter(String propertyName, FilterInfo filterInfo)
		{
			return Filter(null, propertyName, filterInfo);
		}

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="entityPropertyName">外部实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Filter(String entityPropertyName, String propertyName, FilterInfo filterInfo)
		{
			FilterExpression expression = new FilterExpression();

			expression.Filter(entityPropertyName, propertyName, filterInfo);

			return expression;
		}

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Filter(IPropertyChain chain, FilterInfo filterInfo)
		{
			return Filter(chain.PropertyPath, filterInfo);
		}

		/// <summary>
		/// 构造过滤器。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <param name="filterInfo">过滤器信息。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Filter(IList<String> propertyPath, FilterInfo filterInfo)
		{
			FilterExpression expression = new FilterExpression();

			expression.Filter(propertyPath, filterInfo);

			return expression;
		}

		#endregion

		#region 生成过滤器

		/// <summary>
		/// 外部引用属性名称。
		/// </summary>
		/// <param name="entityPropertyName">外部引用属性名称。</param>
		/// <returns>当前实例。</returns>
		public static FilterExpression ForeignRef(String entityPropertyName)
		{
			FilterExpression expression = new FilterExpression();

			expression.ForeignRef(entityPropertyName);

			return expression;
		}

		/// <summary>
		/// 生成过滤器的列信息。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Property(String propertyName)
		{
			FilterExpression expression = new FilterExpression();

			expression.Property(propertyName);

			return expression;
		}

		/// <summary>
		/// 设置属性链生成器。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Property(IPropertyChainBuilder builder)
		{
			return Locator(builder);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Property(IPropertyChain chain)
		{
			return Locator(chain);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Property(IList<String> propertyPath)
		{
			return Locator(propertyPath);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Locator(IPropertyChainBuilder builder)
		{
			return Locator(builder.Build());
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Locator(IPropertyChain chain)
		{
			return Locator(chain.PropertyPath);
		}

		/// <summary>
		/// 设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>过滤器表达式。</returns>
		public static FilterExpression Locator(IList<String> propertyPath)
		{
			FilterExpression expression = new FilterExpression();

			expression.Locator(propertyPath);

			return expression;
		}

		#region 过滤属性的一些快捷方式

		/// <summary>
		/// “Id”属性。
		/// </summary>
		public static FilterExpression Id
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Id;
			}
		}

		/// <summary>
		/// “Name”属性。
		/// </summary>
		public static FilterExpression Name
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Name;
			}
		}

		/// <summary>
		/// “FullName”属性。
		/// </summary>
		public static FilterExpression FullName
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.FullName;
			}
		}

		/// <summary>
		/// “DisplayName”属性。
		/// </summary>
		public static FilterExpression DisplayName
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.DisplayName;
			}
		}

		/// <summary>
		/// “DisplayOrder”属性。
		/// </summary>
		public static FilterExpression DisplayOrder
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.DisplayOrder;
			}
		}

		/// <summary>
		/// “Description”属性。
		/// </summary>
		public static FilterExpression Description
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Description;
			}
		}

		/// <summary>
		/// “Active”属性。
		/// </summary>
		public static FilterExpression Active
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Active;
			}
		}

		/// <summary>
		/// “Deactive”属性。
		/// </summary>
		public static FilterExpression Deactive
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Deactive;
			}
		}

		/// <summary>
		/// “Female”属性。
		/// </summary>
		public static FilterExpression Female
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Female;
			}
		}

		/// <summary>
		/// “Male”属性。
		/// </summary>
		public static FilterExpression Male
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Male;
			}
		}

		/// <summary>
		/// “NavigateUrl”属性。
		/// </summary>
		public static FilterExpression NavigateUrl
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.NavigateUrl;
			}
		}

		/// <summary>
		/// “TimeCreated”属性。
		/// </summary>
		public static FilterExpression TimeCreated
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.TimeCreated;
			}
		}

		/// <summary>
		/// “TimeModified”属性。
		/// </summary>
		public static FilterExpression TimeModified
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.TimeModified;
			}
		}

		/// <summary>
		/// 外部引用“Category”属性。
		/// </summary>
		public static FilterExpression Category
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Category;
			}
		}

		/// <summary>
		/// 外部引用“Organization”属性。
		/// </summary>
		public static FilterExpression Organization
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Organization;
			}
		}

		/// <summary>
		/// 外部引用“Owner”属性。
		/// </summary>
		public static FilterExpression Owner
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Owner;
			}
		}

		/// <summary>
		/// 外部引用“Parent”属性。
		/// </summary>
		public static FilterExpression Parent
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Parent;
			}
		}

		/// <summary>
		/// 外部引用“Region”属性。
		/// </summary>
		public static FilterExpression Region
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Region;
			}
		}

		/// <summary>
		/// 外部引用“Role”属性。
		/// </summary>
		public static FilterExpression Role
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Role;
			}
		}

		/// <summary>
		/// 外部引用“User”属性。
		/// </summary>
		public static FilterExpression User
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.User;
			}
		}

		#endregion

		#endregion
	}
}