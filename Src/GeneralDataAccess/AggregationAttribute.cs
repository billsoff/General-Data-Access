#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupItemAttribute.cs
// 文件功能描述：用于标记参与分组的属性（指示要么做为分组依据，要么对属性进行聚合计算），这个类用作基类。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110627
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于标记参与分组的属性（指示要么做为分组依据，要么对属性进行聚合计算），这个类用作基类。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public abstract class AggregationAttribute : Attribute
	{
		#region 私有字段

		private readonly String[] m_propertyPath;
		private Boolean m_acceptDbType;
		private DbType m_dbType = DbType.Int32;

		private Boolean m_distinct;

		#endregion

		#region 公共常量

		/// <summary>
		/// 通配符，“*”。
		/// </summary>
		public const String KEY_WORD_WILDCARD = "*";

		/// <summary>
		/// DISTINCT 关键字。
		/// </summary>
		public const String KEY_WORD_DISTINCT = "DISTINCT";

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，表示聚合计算与属性无关。
		/// </summary>
		protected AggregationAttribute()
		{
		}

		/// <summary>
		/// 构造函数，设置参与分组的属性路径，可以为 null，表示聚合计算与属性无关。
		/// </summary>
		/// <param name="propertyPath">参与分组的属性路径。</param>
		protected AggregationAttribute(String[] propertyPath)
		{
			if ((propertyPath != null) && (propertyPath.Length != 0))
			{
				m_propertyPath = propertyPath;
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 指示是否接受 DbType 属性的值，默认为 false。
		/// </summary>
		public Boolean AcceptDbType
		{
			get { return m_acceptDbType; }
			set { m_acceptDbType = value; }
		}

		/// <summary>
		/// 获取或设置数据库类型，默认为 Int32，只有当 AcceptDbType 为 true 时，才接受此值，默认取要聚合的属性的 DbType。
		/// <para>如果未设置值，当指定聚合的属性时，取该属性的数据库类型，否则取 Int32。</para>
		/// </summary>
		public DbType DbType
		{
			get { return m_dbType; }
			set { m_dbType = value; }
		}

		/// <summary>
		/// 指示是否在聚合表达式中加 DISTINCT 关键字，默认为 false。
		/// </summary>
		public Boolean Distinct
		{
			get { return m_distinct; }
			set { m_distinct = value; }
		}

		/// <summary>
		/// 获取一个值，该值指示标记的属性是否为分组项，默认值为 false。
		/// </summary>
		public virtual Boolean IsGroupItem
		{
			get { return false; }
		}

		/// <summary>
		/// 获取属性路径，该路径指示要参与分组的属性，如果此属性为 null，则表示聚合计算与属性无关。
		/// </summary>
		public String[] PropertyPath
		{
			get { return m_propertyPath; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取聚合表达式。
		/// </summary>
		/// <param name="columnName">列的限定名，如果为 null，则指示聚合计算与列无关。</param>
		/// <returns>聚合表达式。</returns>
		public virtual String GetAggregationExpression(String columnName)
		{
			#region 前置条件

			Debug.Assert(
					(columnName != null) || AllowNullColumnName,
					String.Format("参数 columnName 不能为空，请在聚合标记 {0} 中指定要聚合的属性。", this)
				);

			#endregion

			if (!Distinct)
			{
				if ((columnName == null) && AllowNullColumnName)
				{
					columnName = KEY_WORD_WILDCARD;
				}

				return String.Format("{0}({1})", FunctionName, columnName);
			}
			else
			{
				#region 前置条件

				Debug.Assert(
						!String.IsNullOrEmpty(columnName),
						String.Format(
								"参数 columnName 不能为空（因为 Distinct 属性被设置为 true），请在聚合标记 {0} 中指定要聚合的属性。",
								this
							)
					);

				#endregion

				return String.Format("{0}({1} {2})", FunctionName, KEY_WORD_DISTINCT, columnName);
			}
		}

		#endregion

		#region 受保护的属性

		/// <summary>
		/// 指示是否允许空列名，默认为 false。
		/// </summary>
		protected virtual Boolean AllowNullColumnName
		{
			get { return false; }
		}

		/// <summary>
		/// 获取聚合函数的名称。
		/// </summary>
		protected virtual String FunctionName
		{
			get { throw new NotImplementedException("聚合函数的名称未知，无法生成聚合表达式。"); }
		}

		#endregion

		#region 受保护的方法

		/// <summary>
		/// 获取显示名称，使用 FunctionName 属性，形如“COUNT(...)”。
		/// </summary>
		/// <returns>显示名称。</returns>
		protected virtual String GetDisplayName()
		{
			if (!Distinct)
			{
				return String.Format("{0}(...)", FunctionName);
			}
			else
			{
				return String.Format("{0}({1} ...))", FunctionName, AggregationAttribute.KEY_WORD_DISTINCT);
			}
		}

		#endregion

		#region 抽象成员

		/// <summary>
		/// 获取聚合名称。
		/// </summary>
		public abstract String Name { get; }

		#endregion
	}
}