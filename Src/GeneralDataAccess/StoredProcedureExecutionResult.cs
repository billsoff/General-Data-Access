#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：StoredProcedureExecutionResult.cs
// 文件功能描述：用于包装存储过程的执行结果。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于包装存储过程的执行结果。
	/// </summary>
	[Serializable]
	public class StoredProcedureExecutionResult<TEntity> where TEntity : class, new()
	{
		#region 私有字段

		private readonly Int32 m_rowsAffected;
		private readonly DbStoredProcedureParameters m_storedProcedureParameters;
		private readonly TEntity[] m_entities;

		#endregion

		#region 构造函数

		/// <summary>
		/// 设置执行存储过程后参数集合、影响的行数，加载的实体集合为 null。
		/// </summary>
		/// <param name="storedProcedureParameters">参数集合。</param>
		/// <param name="rowsAffected">影响的行数。</param>
		public StoredProcedureExecutionResult(DbStoredProcedureParameters storedProcedureParameters, Int32 rowsAffected)
			: this(storedProcedureParameters, rowsAffected, null)
		{
		}

		/// <summary>
		/// 设置执行存储过程后参数集合、加载的实体集合，影响的行数未知（设为 -1）。
		/// </summary>
		/// <param name="storedProcedureParameters">参数集合。</param>
		/// <param name="entities">加载的实体集合。</param>
		public StoredProcedureExecutionResult(DbStoredProcedureParameters storedProcedureParameters, TEntity[] entities)
			: this(storedProcedureParameters, -1, entities)
		{
		}

		/// <summary>
		/// 设置执行存储过程后参数集合、影响的行数和加载的实体集合。
		/// </summary>
		/// <param name="storedProcedureParameters">参数集合。</param>
		/// <param name="rowsAffected">影响的行数。</param>
		/// <param name="entities">加载的实体集合。</param>
		public StoredProcedureExecutionResult(DbStoredProcedureParameters storedProcedureParameters, Int32 rowsAffected, TEntity[] entities)
		{
			m_rowsAffected = rowsAffected;
			m_storedProcedureParameters = storedProcedureParameters;
			m_entities = entities;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取执行存储过程影响的行数，-1 表示未知。
		/// </summary>
		public Int32 RowsAffected
		{
			get { return m_rowsAffected; }
		}

		/// <summary>
		/// 获取存储过程的参数集合。
		/// </summary>
		public DbStoredProcedureParameters StoredProcedureParameters
		{
			get { return m_storedProcedureParameters; }
		}

		/// <summary>
		/// 获取加载的实体集合。
		/// </summary>
		public TEntity[] Entities
		{
			get { return m_entities; }
		}

		/// <summary>
		/// 获取结果集中的第一个实体。
		/// </summary>
		public TEntity First
		{
			get
			{
				if ((m_entities != null) && (m_entities.Length != 0))
				{
					return m_entities[0];
				}
				else
				{
					return null;
				}
			}
		}

		#endregion

		#region 转换器

		/// <summary>
		/// 将实体集合转为强类型。
		/// </summary>
		/// <param name="executionResult">要转换的执行结果。</param>
		/// <returns>转换后的执行结果。</returns>
		public static explicit operator StoredProcedureExecutionResult<TEntity>(StoredProcedureExecutionResult<Object> executionResult)
		{
			return new StoredProcedureExecutionResult<TEntity>(executionResult.StoredProcedureParameters, executionResult.RowsAffected, (TEntity[])executionResult.Entities);
		}

		#endregion
	}
}