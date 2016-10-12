#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：BatchModificationSchemaCache.cs
// 文件功能描述：缓存批量修改的架构信息。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110228
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Useease.GeneralDataAccess
{
	internal class BatchModificationSchemaCache
	{
		#region 静态成员

		#region 私有字段

		private static readonly BatchModificationSchemaCache m_instance;

		#endregion

		#region 类初始化器

		/// <summary>
		///  类初始化器。
		/// </summary>
		static BatchModificationSchemaCache()
		{
			m_instance = new BatchModificationSchemaCache();
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 从缓存中获取架构信息。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <param name="db">数据库。</param>
		/// <returns>缓存的架构信息，如果不存在，则返回 null。</returns>
		public static BatchModificationSchema GetSchema(Type entityType, Database db)
		{
			return m_instance.DoGetSchema(entityType, db);
		}

		/// <summary>
		/// 缓存架构信息，缓存副本。
		/// </summary>
		/// <param name="schema">架构信息。</param>
		/// <param name="db">数据库</param>
		public static void SetSchema(BatchModificationSchema schema, Database db)
		{
			m_instance.DoSetSchema(schema, db);
		}

		#endregion

		#endregion

		#region 私有字段

		private readonly Dictionary<Type, BatchModificationSchema> m_bachModificationSchemas;
		private readonly Object m_lock;

		#endregion

		#region 构造函数

		/// <summary>
		/// 私有，实现单例。
		/// </summary>
		private BatchModificationSchemaCache()
		{
			m_bachModificationSchemas = new Dictionary<Type, BatchModificationSchema>();
			m_lock = ((ICollection)m_bachModificationSchemas).SyncRoot;
		}

		#endregion

		#region 缓存实现方法

		/// <summary>
		/// 从缓存中获取架构信息。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <param name="db">数据库。</param>
		/// <returns>缓存的架构信息，如果不存在，则返回 null。</returns>
		private BatchModificationSchema DoGetSchema(Type entityType, Database db)
		{
			lock (m_lock)
			{
				if (m_bachModificationSchemas.ContainsKey(entityType))
				{
					BatchModificationSchema schema = m_bachModificationSchemas[entityType];

					return schema.Clone(db);
				}
			}

			return null;
		}

		/// <summary>
		/// 缓存架构信息，缓存副本。
		/// </summary>
		/// <param name="schema">架构信息。</param>
		/// <param name="db">数据库</param>
		private void DoSetSchema(BatchModificationSchema schema, Database db)
		{
			lock (m_lock)
			{
				m_bachModificationSchemas[schema.EntityType] = schema.Clone(db);
			}
		}

		#endregion
	}
}