#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CommonPolicies.cs
// 文件功能描述：通用方针（属性束定、列对比等）。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110516
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 通用方针（属性束定、列对比等）。
	/// </summary>
	internal static class CommonPolicies
	{
		#region 私有字段

		private static readonly BindingFlags m_propertyBindingFlags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly StringComparison m_propertyNameComparison = StringComparison.Ordinal;
		private static readonly StringComparison m_columnNameComparison = StringComparison.OrdinalIgnoreCase;

		private static readonly String m_tableAliasPrefix = "t__";
		private static readonly String m_columnAliasPrefix = "f__";
		private static readonly String m_whereFilterParameterNamePrefix = "pw__";
		private static readonly String m_havingFilterParameterNamePrefix = "ph__";

		#endregion

		#region 公共常量

		/// <summary>
		///  点。
		/// </summary>
		public const String DOT = ".";

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取列名称匹配规则，当前的规则为不区分大小写。
		/// </summary>
		public static StringComparison ColumnNameComparison
		{
			get { return m_columnNameComparison; }
		}

		/// <summary>
		/// 获取属性束定方针，当前的方针为取所有类型层次中的公有的和非公有的实例属性。
		/// <para>允许实体继承，理解为一种合并关系。</para>
		/// </summary>
		public static BindingFlags PropertyBindingFlags
		{
			get { return m_propertyBindingFlags; }
		}

		/// <summary>
		/// 获取属性名称匹配规则，当前的规则为区分大小写。
		/// </summary>
		public static StringComparison PropertyNameComparison
		{
			get { return m_propertyNameComparison; }
		}

		/// <summary>
		/// 获取表别名前缀。
		/// </summary>
		public static String TableAliasPrefix
		{
			get { return CommonPolicies.m_tableAliasPrefix; }
		}

		/// <summary>
		/// 获取列别名前缀。
		/// </summary>
		public static String ColumnAliasPrefix
		{
			get { return CommonPolicies.m_columnAliasPrefix; }
		}

		/// <summary>
		/// 获取 WHERE 过滤器参数名前缀。
		/// </summary>
		public static String WhereFilterParameterNamePrefix
		{
			get { return CommonPolicies.m_whereFilterParameterNamePrefix; }
		}

		/// <summary>
		/// 获取 HAVING 过滤器参数名前缀。
		/// </summary>
		public static String HavingFilterParameterNamePrefix
		{
			get { return CommonPolicies.m_havingFilterParameterNamePrefix; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建属性全名称，用点将属性路径连接起来。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		/// <returns>属性全名称。</returns>
		public static String ComposePropertyFullName(String[] propertyPath)
		{
			return String.Join(DOT, propertyPath);
		}

		/// <summary>
		/// 获取列的全名称。
		/// </summary>
		/// <param name="columnDef">列定义。</param>
		/// <param name="property">列所属的实体属性。</param>
		/// <returns>列的全名称，可能为 null。</returns>
		public static String GetColumnFullName(ColumnDefinition columnDef, EntityProperty property)
		{
			if ((columnDef != null) && (property != null))
			{
				return String.Format("{0}.{1}", property.Schema.TableAlias, columnDef.Name);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 获取分组实体属性的列的数据库类型，默认为 Int32。
		/// </summary>
		/// <param name="propertyDef">分组实体属性定义。</param>
		/// <param name="columnDef">列定义。</param>
		/// <returns>分组实体的列的数据库类型，如果不能从参数中得出，则取 Int32。</returns>
		public static DbType GetGroupDbType(GroupPropertyDefinition propertyDef, ColumnDefinition columnDef)
		{
			if ((propertyDef != null) && propertyDef.Aggregation.AcceptDbType)
			{
				return propertyDef.Aggregation.DbType;
			}
			else if (columnDef != null)
			{
				return columnDef.DbType;
			}
			else
			{
				return DbType.Int32;
			}
		}

		/// <summary>
		/// 获取表别名。
		/// </summary>
		/// <param name="index">表索引号</param>
		/// <returns>表别名。</returns>
		public static String GetTableAlias(Int32 index)
		{
			return TableAliasPrefix + index.ToString();
		}

		/// <summary>
		/// 获取列别名。
		/// </summary>
		/// <param name="index">列索引。</param>
		/// <returns>构造好的列别名。</returns>
		public static String GetColumnAlias(Int32 index)
		{
			return ColumnAliasPrefix + index.ToString();
		}

		/// <summary>
		/// 获取属性名称。
		/// </summary>
		/// <param name="fullName">属性全名称。</param>
		/// <returns>属性名称。</returns>
		public static String GetPropertyName(String fullName)
		{
			if (!fullName.Contains(CommonPolicies.DOT))
			{
				return fullName;
			}
			else
			{
				Int32 index = fullName.LastIndexOf(CommonPolicies.DOT);

				return fullName.Substring(index + 1);
			}
		}

		/// <summary>
		/// 获取规范化的列名，因列名的比较规则是不区分大小写，因此当前的实现是返回列名的全大写形式。
		/// </summary>
		/// <param name="columnName">列名。</param>
		/// <returns>列名的规范化表示。</returns>
		public static String NormalizeColumnName(String columnName)
		{
			if (!String.IsNullOrEmpty(columnName))
			{
				return columnName.ToUpper();
			}
			else
			{
				return columnName;
			}
		}

		/// <summary>
		/// 获取规范化的属性名，因属性名的比较规则是区分大小写，因此当前的实现是原样返回属性名。
		/// </summary>
		/// <param name="propertyName">属性名。</param>
		/// <returns>属性名的规范化形式。</returns>
		public static String NormalizePropertyName(String propertyName)
		{
			return propertyName;
		}

		#endregion
	}
}