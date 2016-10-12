#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ActionQueryEntity.cs
// 文件功能描述：封装用于执行数据库事件的实体（指明对实体进行操作的类型）。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20100815
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
	/// 封装用于执行数据库事件的实体（指明对实体进行操作的类型）。
	/// </summary>
	[Serializable]
	public class ActionQueryEntity
	{
		#region 私有字段

		private readonly ActionQueryType m_actionQueryType;
		private readonly Object m_entity;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，初始化操作类型和操作的实体。
		/// </summary>
		/// <param name="actionQueryType">操作类型，添加、删除或修改。</param>
		/// <param name="entity">操作的实体。</param>
		public ActionQueryEntity(ActionQueryType actionQueryType, Object entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity", "要操作的实体不能为空。");
			}

			m_actionQueryType = actionQueryType;
			m_entity = entity;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取操作类型。
		/// </summary>
		public ActionQueryType ActionQueryType
		{
			get { return m_actionQueryType; }
		}

		/// <summary>
		/// 获取操作实体。
		/// </summary>
		public Object Entity
		{
			get { return m_entity; }
		}

		#endregion
	}
}