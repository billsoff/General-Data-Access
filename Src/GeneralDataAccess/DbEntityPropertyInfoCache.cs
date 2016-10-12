#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DbEntityPropertyInfoCache.cs
// 文件功能描述：缓存数据库实体的属性信息。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110407
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
	/// 缓存数据库实体的属性信息。
	/// </summary>
	internal class DbEntityPropertyInfoCache
	{
		#region 静态成员

		#region 私有字段

		private static readonly DbEntityPropertyInfoCache m_instance;

		#endregion

		#region 构造函数

		/// <summary>
		/// 静态构造函数。
		/// </summary>
		static DbEntityPropertyInfoCache()
		{
			m_instance = new DbEntityPropertyInfoCache();
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取实体属性信息。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>该类型的属性信息。</returns>
		public static DbEntityPropertyInfo GetProperty(Type entityType)
		{
			return m_instance.DoGetProperty(entityType);
		}

		#endregion

		#endregion

		#region 私有字段

		private readonly Object m_lock = new Object();
		private readonly Dictionary<Type, DbEntityPropertyInfo> m_propertyInfos = new Dictionary<Type, DbEntityPropertyInfo>();

		#endregion

		#region 构造方法

		/// <summary>
		/// 构造函数，私有化，以实现单例。
		/// </summary>
		private DbEntityPropertyInfoCache()
		{
		}

		#endregion

		#region 缓存的实现方法

		/// <summary>
		/// 获取实体属性。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>该实体类型的属性。</returns>
		public DbEntityPropertyInfo DoGetProperty(Type entityType)
		{
			DbEntityPropertyInfo info;

			m_propertyInfos.TryGetValue(entityType, out info);

			if (info == null)
			{
				info = DbEntityPropertyInfo.Create(entityType);

				lock (m_lock)
				{
					m_propertyInfos[entityType] = info;
				}

				info.Initialize();
			}

			return info;
		}

		#endregion
	}
}