#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：QueryParameter.cs
// 文件功能描述：表示一个查询参数。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008151414
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
	/// 表示一个查询参数。
	/// </summary>
	[Serializable]
	public class QueryParameter : ICloneable
	{
		#region 私有字段

		// Clone 时可以更换
		private String m_name;

		private readonly DbType m_dbType;
		private Object m_value;

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
		/// 构造函数，设置查询参数的名称、数据库类型和值，方向为 Input。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbType">参数数据库类型。</param>
		/// <param name="value">参数值。</param>
		public QueryParameter(String name, DbType dbType, Object value)
			: this(name, dbType, value, ParameterDirection.Input)
		{
		}

		/// <summary>
		/// 构造函数，设置查询参数的名称、数据库类型、值和方向。
		/// </summary>
		/// <param name="name">参数名称</param>
		/// <param name="dbType">参数数据库类型。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		public QueryParameter(String name, DbType dbType, Object value, ParameterDirection direction)
		{
			m_name = name;
			m_dbType = dbType;
			m_value = value;

			m_direction = direction;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取查询参数的名称。
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// 获取查询参数的数据库类型。
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
		/// 获取查询参数的值。
		/// </summary>
		public Object Value
		{
			get { return m_value; }
			protected internal set { m_value = value; }
		}

		/// <summary>
		/// 获取参数的方向，默认为 Input。
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

		#region ICloneable 成员

		/// <summary>
		/// 复制并更换名称，如果新名称为 null 或空字符串，则保持原来的名称。
		/// </summary>
		/// <param name="newName">新名称。</param>
		/// <returns>当前对象的浅表副本。</returns>
		public QueryParameter Clone(String newName)
		{
			QueryParameter clone = (QueryParameter)MemberwiseClone();

			if (!String.IsNullOrEmpty(newName))
			{
				clone.m_name = newName;
			}

			return clone;
		}

		/// <summary>
		/// 返回当前实例的浅表副本。
		/// </summary>
		/// <returns>当前实例的浅表副本。</returns>
		Object ICloneable.Clone()
		{
			return MemberwiseClone();
		}

		#endregion
	}
}