#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：TransactionScope.cs
// 文件功能描述：用于收集进行数据库事务操作的实体。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 201008150905
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于收集进行数据库事务操作的实体。
	/// </summary>
	[Serializable]
	public sealed class TransactionScope
	{
		#region 私有字段

		private readonly IsolationLevel m_isolationLevel;
		private readonly Boolean m_useDefaultIsolationLevel = true;

		private readonly List<ActionQueryEntity> m_entities = new List<ActionQueryEntity>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public TransactionScope()
		{
		}

		/// <summary>
		/// 构造函数，设置事务隔离级别。
		/// </summary>
		/// <param name="isolationLevel">事务隔离级别。</param>
		public TransactionScope(IsolationLevel isolationLevel)
		{
			m_isolationLevel = isolationLevel;
			m_useDefaultIsolationLevel = false;
		}

		/// <summary>
		/// 构造函数，初始化要进行事务操作的实体集合。
		/// </summary>
		/// <param name="entities">要进行事务操作的实体集合。</param>
		public TransactionScope(IList<ActionQueryEntity> entities)
		{
			m_entities.AddRange(entities);
		}

		/// <summary>
		/// 构造函数，初始化要进行事务操作的实体集合和事务隔离级别。
		/// </summary>
		/// <param name="entities">要进行事务操作的实体集合。</param>
		/// <param name="isolationLevel">事务隔离级别。</param>
		public TransactionScope(IList<ActionQueryEntity> entities, IsolationLevel isolationLevel)
			: this(entities)
		{
			m_isolationLevel = isolationLevel;
			m_useDefaultIsolationLevel = false;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取实体集合。
		/// </summary>
		public ActionQueryEntity[] Entities
		{
			get { return m_entities.ToArray(); }
		}

		/// <summary>
		/// 获取事务隔离级别。
		/// </summary>
		public IsolationLevel IsolationLevel
		{
			get { return m_isolationLevel; }
		}

		/// <summary>
		/// 获取一个值，指示是否使用默认的隔离级别。
		/// </summary>
		public Boolean UseDefaultIsolationLevel
		{
			get { return m_useDefaultIsolationLevel; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 添加操作类型和实体。
		/// </summary>
		/// <param name="actionQueryType">操作类型。</param>
		/// <param name="entity">实体。</param>
		/// <returns>当前事务域对象。</returns>
		public TransactionScope Add(ActionQueryType actionQueryType, Object entity)
		{
			m_entities.Add(new ActionQueryEntity(actionQueryType, entity));

			return this;
		}

		#endregion
	}
}