#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：StoredProcedureParameterAttribute.cs
// 文件功能描述：标记类型 DbStoredProcedureParameters 的属性为存储过程的参数。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110212
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 标记类型 DbStoredProcedureParameters 的属性为存储过程的参数。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class StoredProcedureParameterAttribute : Attribute
	{
		#region 私有字段

		private readonly String m_name;
		private readonly DbType m_dbType;
		private readonly ParameterDirection m_direction;

		private Int32 m_size;
		private Boolean m_nullable;
		private Byte m_precision;
		private Byte m_scale;

		private String m_dbTypePropertyName;
		private Int32 m_dbTypePropertyValue;

		private Boolean m_suppressRetrieveValue;

		#endregion

		#region 构造函数

		/// <summary>
		/// 初始化参数名称、数据库类型，方向为 Input。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbType">参数数据库类型。</param>
		public StoredProcedureParameterAttribute(String name, DbType dbType)
			: this(name, dbType, ParameterDirection.Input)
		{
		}

		/// <summary>
		/// 初始化参数名称、数据库类型和方向。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbType">参数数据库类型。</param>
		/// <param name="direction">参数方向。</param>
		public StoredProcedureParameterAttribute(String name, DbType dbType, ParameterDirection direction)
		{
			m_name = name;
			m_dbType = dbType;
			m_direction = direction;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取参数的名称。
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// 获取参数的数据库类型。
		/// </summary>
		public DbType DbType
		{
			get { return m_dbType; }
		}

		/// <summary>
		/// 获取或设置参数数据类型的属性名称。
		/// </summary>
		public String DbTypePropertyName
		{
			get
			{
				return m_dbTypePropertyName;
			}

			set
			{
				if (value != null)
				{
					value = value.Trim();
				}

				if (!String.IsNullOrEmpty(value))
				{
					m_dbTypePropertyName = value;
				}
			}
		}

		/// <summary>
		/// 获取或设置参数数据类型的值。
		/// </summary>
		public Int32 DbTypePropertyValue
		{
			get { return m_dbTypePropertyValue; }
			set { m_dbTypePropertyValue = value; }
		}

		/// <summary>
		/// 获取参数的方法，默认为 Input。
		/// </summary>
		public ParameterDirection Direction
		{
			get { return m_direction; }
		}

		/// <summary>
		/// 获取或设置参数大小。
		/// </summary>
		public Int32 Size
		{
			get { return m_size; }
			set { m_size = value; }
		}

		/// <summary>
		/// 获取或设置参数是否可以为空。
		/// </summary>
		public Boolean Nullable
		{
			get { return m_nullable; }
			set { m_nullable = value; }
		}

		/// <summary>
		/// 获取或设置参数的精度（有效数字个数）。
		/// </summary>
		public Byte Precision
		{
			get { return m_precision; }
			set { m_precision = value; }
		}

		/// <summary>
		/// 获取或设置参数的小数位数。
		/// </summary>
		public Byte Scale
		{
			get { return m_scale; }
			set { m_scale = value; }
		}

		/// <summary>
		/// 获取或设置一个值，如果参数不是输入参数，在执行完存储过程后，如果该值为 false，则取得参数值，否则，不去获取，默认值为 false。
		/// </summary>
		public Boolean SuppressRetrieveValue
		{
			get { return m_suppressRetrieveValue; }
			set { m_suppressRetrieveValue = value; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建 <see cref="QueryParameter"/> 实例。
		/// </summary>
		/// <param name="value">参数值。</param>
		/// <returns>创建好的参数。</returns>
		public QueryParameter CreateParameter(Object value)
		{
			QueryParameter parameter = new QueryParameter(m_name, m_dbType, value, m_direction);

			parameter.Size = m_size;
			parameter.Nullable = m_nullable;

			parameter.Precision = m_precision;
			parameter.Scale = m_scale;

			if (!String.IsNullOrEmpty(m_dbTypePropertyName))
			{
				parameter.DbTypePropertyName = m_dbTypePropertyName;
				parameter.DbTypePropertyValue = m_dbTypePropertyValue;
			}

			return parameter;
		}

		#endregion
	}
}