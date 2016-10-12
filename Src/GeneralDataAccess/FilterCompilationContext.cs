#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：FilterCompilationContext.cs
// 文件功能描述：表示提供编译过滤条件的环境对象。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110706
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
	/// 表示提供编译过滤条件的环境对象。
	/// </summary>
	public abstract class FilterCompilationContext
	{
		#region 私有字段

		private readonly IColumnLocatable m_schema;
		private readonly String m_parameterPrefix;
		private readonly List<QueryParameter> m_parameters = new List<QueryParameter>();
		private Int32 m_index;
		private Int32 m_indexOffset;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		protected FilterCompilationContext()
		{
		}

		/// <summary>
		/// 构造函数，设置实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		protected FilterCompilationContext(IColumnLocatable schema)
		{
			m_schema = schema;
		}

		/// <summary>
		/// 构造函数，设置实体架构和参数前缀。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <param name="parameterPrifix">参数前缀。</param>
		protected FilterCompilationContext(IColumnLocatable schema, String parameterPrifix)
			: this(schema)
		{
			m_parameterPrefix = parameterPrifix;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取索引偏移。
		/// </summary>
		public Int32 IndexOffset
		{
			get { return m_indexOffset; }
			internal set { m_indexOffset = value; }
		}

		/// <summary>
		/// 获取所有的查询参数。
		/// </summary>
		public virtual QueryParameter[] Parameters
		{
			get { return m_parameters.ToArray(); }
		}

		/// <summary>
		/// 获取参数前缀。
		/// </summary>
		public virtual String ParameterPrefix
		{
			get
			{
				if (!String.IsNullOrEmpty(m_parameterPrefix))
				{
					return m_parameterPrefix;
				}
				else
				{
					return Filter.PARAMETER_PREFIX;
				}
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 根据列定位符生成查询参数。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <param name="paramValues">参数值集合。</param>
		/// <returns>生成的查询参数集合。</returns>
		public virtual QueryParameter[] GenerateParameters(ColumnLocator colLocator, Object[] paramValues)
		{
			Column[] allColumns = m_schema[colLocator];

			List<QueryParameter> parameters = new List<QueryParameter>();

			foreach (Object value in paramValues)
			{
				String paramName = CreateParameterName();
				DbType dbType = allColumns[0].DbType;

				Object paramValue = value;

				if (value == null)
				{
					paramValue = DBNull.Value;
				}

				QueryParameter parameter = GenerateQueryParameter(paramName, dbType, paramValue);

				parameters.Add(parameter);
			}

			return parameters.ToArray();
		}

		/// <summary>
		/// 根据列定位符获取列的全名称。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>该列的全名称。</returns>
		public virtual String GetColumnFullName(ColumnLocator colLocator)
		{
			Column[] cols = m_schema[colLocator];

			return cols[0].Expression;
		}

		/// <summary>
		/// 根据列定位符和查询参数值集合获取实体查询参数集合。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <param name="paramValues">参数值集合。</param>
		/// <returns>实体参数集合，其中第一个元素是与实体列的全名称集合一对一相映射的。</returns>
		public virtual QueryParameterCollection[] GenerateEntityParameters(ColumnLocator colLocator, Object[] paramValues)
		{
			Column[] allColumns = m_schema[colLocator];

			List<QueryParameterCollection> entityParameters = new List<QueryParameterCollection>();

			foreach (Object value in paramValues)
			{
				QueryParameter[] parameters = new QueryParameter[allColumns.Length];

				for (Int32 i = 0; i < allColumns.Length; i++)
				{
					Column col = allColumns[i];

					String paramName = CreateParameterName();

					Object paramValue = col.GetDbValue(value, true);

					parameters[i] = GenerateQueryParameter(paramName, col.DbType, paramValue);
				}

				entityParameters.Add(new QueryParameterCollection(parameters));
			}

			return entityParameters.ToArray();
		}

		/// <summary>
		/// 根据列定位符获取实体列的全名称集合。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>实体列的全名称集合。</returns>
		public virtual String[] GetEntityColumnFullNames(ColumnLocator colLocator)
		{
			Column[] allColumns = m_schema[colLocator];

			String[] entityColumnFullNames = Array.ConvertAll<Column, String>(
					allColumns,
					delegate(Column col) { return col.Expression; }
				);

			return entityColumnFullNames;
		}

		/// <summary>
		/// 判断列定位符是否为实体列。
		/// </summary>
		/// <param name="colLocator">列定位符。</param>
		/// <returns>如果列定位符指向实体属性，则其为实体列，返回 true，否则，返回 false。</returns>
		public virtual Boolean IsEntityColumns(ColumnLocator colLocator)
		{
			Column[] allColumns = m_schema[colLocator];

			return !allColumns[0].IsPrimitive;
		}

		/// <summary>
		/// 注册查询参数，用于将子查询的参数附加到当前的环境中。
		/// </summary>
		/// <param name="parameters">要注册的参数列表。</param>
		/// <returns>已注册的参数列表。</returns>
		public QueryParameter[] RegisterParameters(QueryParameter[] parameters)
		{
			#region 前置条件

			Debug.Assert(parameters != null, "参数 parameters 不能为空。");

			#endregion

			QueryParameter[] registeredParameters = Array.ConvertAll<QueryParameter, QueryParameter>(
					parameters,
					delegate(QueryParameter parm)
					{
						return parm.Clone(CreateParameterName());
					}
				);

			m_parameters.AddRange(registeredParameters);

			return registeredParameters;
		}

		#endregion

		#region 抽象成员

		#region 保护的属性

		/// <summary>
		/// 获取参数名称的前缀。
		/// </summary>
		protected abstract String ParameterNamePrefix { get; }

		#endregion

		#endregion

		#region 保护的方法

		/// <summary>
		/// 创建参数名称。
		/// </summary>
		/// <returns>创建好的参数名称。</returns>
		protected String CreateParameterName()
		{
			m_index++;

			String name = ParameterNamePrefix + (m_index + m_indexOffset).ToString();

			return name;
		}

		/// <summary>
		/// 生成查询参数并放入参数集合中。
		/// </summary>
		/// <param name="paramName">参数名称。</param>
		/// <param name="dbType">参数数据库类型。</param>
		/// <param name="paramValue">参数值。</param>
		/// <returns>生成好的参数。</returns>
		protected QueryParameter GenerateQueryParameter(String paramName, DbType dbType, Object paramValue)
		{
			QueryParameter parameter = new QueryParameter(paramName, dbType, paramValue);

			m_parameters.Add(parameter);

			return parameter;
		}

		#endregion
	}
}