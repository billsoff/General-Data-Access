#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityDataBag.cs
// 文件功能描述：实体数据包，用于存储实体数据和保存点（供以后回滚），目前仅支持延迟属性和回滚，在将来的版本可全面支持属性历史记录。
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
using System.Diagnostics;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 实体数据包，用于存储实体数据和保存点（供以后回滚）。
	/// </summary>
	[Serializable]
	internal sealed class EntityDataBag
	{
		#region 私有字段

		private readonly Type m_type;
		private readonly EtyBusinessObject m_entity;

		// 键是延迟加载的属性名称
		private Dictionary<String, Object> m_lazyLoads;

		[NonSerialized]
		private Boolean m_hasSavePoint;

		// 按属性定义顺序成的值列表
		[NonSerialized]
		private Object[] m_savePoint;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体。
		/// </summary>
		/// <param name="entity">实体。</param>
		public EntityDataBag(EtyBusinessObject entity)
		{
			#region 前置条件

			Debug.Assert((entity != null), "实体参数 entity 不能为空。");

			#endregion

			m_type = entity.GetType();
			m_entity = entity;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取实体。
		/// </summary>
		public EtyBusinessObject Entity
		{
			get { return m_entity; }
		}

		/// <summary>
		/// 获取一个值，该值指示是否设置了保存点。
		/// </summary>
		public Boolean HasSavePoint
		{
			get { return m_hasSavePoint; }
		}

		/// <summary>
		/// 获取实体类型。
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 清除保存点。
		/// </summary>
		public void ClearSavePoint()
		{
			m_hasSavePoint = false;
		}

		/// <summary>
		/// 获取属性的值（采用显示类型强制转换）。
		/// </summary>
		/// <typeparam name="TValue">属性类型。</typeparam>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>属性值。</returns>
		public TValue GetLazyLoadValue<TValue>(String propertyName)
		{
			#region 前置条件

#if DEBUG

			EntityPropertyDefinition propertyDef = Definition.Properties[propertyName];

			Debug.Assert(propertyDef.LazyLoad, String.Format("属性 {0} 没有标记为延迟加载。", propertyName));

#endif

			#endregion

			if (IsEmpty(propertyName) && Entity.MayLazyLoad)
			{
				LoadLazyLoadValue(propertyName);
			}

			Object value = LazyLoads[propertyName];

			if ((value != null) && !DbConverter.IsDBEmpty(value))
			{
				return (TValue)value;
			}
			else
			{
				return default(TValue);
			}
		}

		/// <summary>
		/// 判断指定的属性的值是否为 DBEmpty（即尚未初始化）。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>如果该属性的值尚未初始化，则返回 true；否则返回 false。</returns>
		public Boolean IsEmpty(String propertyName)
		{
			#region 前置条件

			Debug.Assert((propertyName != null), "属性名称参数 propertyName 不能为空。");

			#endregion

			return LazyLoads.ContainsKey(propertyName) && DbConverter.IsDBEmpty(LazyLoads[propertyName]);
		}

		/// <summary>
		/// 回滚到最近的一个保存点。
		/// </summary>
		/// <returns>如果保存点存在，则回滚成功，返回 true；否则返回 false。</returns>
		public Boolean Rollback()
		{
			if (!HasSavePoint)
			{
				return false;
			}

			for (Int32 i = 0; i < SavePoint.Length; i++)
			{
				EntityPropertyDefinition propertyDef = Definition.Properties[i];

				if (!propertyDef.LazyLoad || !IsEmpty(propertyDef.Name))
				{
					propertyDef.PropertyInfo.SetValue(Entity, SavePoint[i], null);
				}
				else
				{
					Entity.SetEmpty(propertyDef.Name);
				}
			}

			m_hasSavePoint = false;

			return true;
		}

		/// <summary>
		/// 将延迟加载属性置为 DBEmpty。
		/// </summary>
		/// <param name="propertyName">延迟加载属性名称。</param>
		public void SetEmpty(String propertyName)
		{
			#region 前置条件

#if DEBUG

			EntityPropertyDefinition propertyDef = Definition.Properties[propertyName];

			Debug.Assert(propertyDef.LazyLoad, String.Format("属性 {0} 没有标记为延迟加载。", propertyName));

#endif

			#endregion

			LazyLoads[propertyName] = DBEmpty.Value;
		}

		/// <summary>
		/// 设置延迟加载属性的值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <param name="propertyValue">属性值。</param>
		public void SetLazyLoadValue(String propertyName, Object propertyValue)
		{
			#region 前置条件

#if DEBUG

			EntityPropertyDefinition propertyDef = Definition.Properties[propertyName];

			Debug.Assert(propertyDef.LazyLoad, String.Format("属性 {0} 没有标记为延迟加载。", propertyName));

#endif

			#endregion

			LazyLoads[propertyName] = propertyValue;
		}

		/// <summary>
		/// 设置保存点。
		/// </summary>
		public void SetSavePoint()
		{
			EntityDefinition definition = EntityDefinitionBuilder.Build(Entity.GetType());

			for (Int32 i = 0; i < SavePoint.Length; i++)
			{
				EntityPropertyDefinition propertyDef = Definition.Properties[i];

				if (!propertyDef.LazyLoad || !IsEmpty(propertyDef.Name))
				{
					SavePoint[i] = propertyDef.PropertyInfo.GetValue(Entity, null);
				}
				else
				{
					SavePoint[i] = DBEmpty.Value;
				}
			}

			m_hasSavePoint = true;
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取实体定义。
		/// </summary>
		internal EntityDefinition Definition
		{
			get { return EntityDefinitionBuilder.Build(Type); }
		}

		#endregion

		#region 私有属性

		/// <summary>
		/// 获取延迟加载值集合，键是属性名称。
		/// </summary>
		private Dictionary<String, Object> LazyLoads
		{
			get
			{
				if (m_lazyLoads == null)
				{
					m_lazyLoads = new Dictionary<String, Object>();

					foreach (EntityPropertyDefinition propertyDef in Definition.Properties)
					{
						if (propertyDef.LazyLoad)
						{
							m_lazyLoads.Add(propertyDef.Name, DBEmpty.Value);
						}
					}
				}

				return m_lazyLoads;
			}
		}

		/// <summary>
		/// 获取存储点。
		/// </summary>
		private Object[] SavePoint
		{
			get
			{
				if (m_savePoint == null)
				{
					m_savePoint = new Object[Definition.Properties.Count];

					for (Int32 i = 0; i < m_savePoint.Length; i++)
					{
						m_savePoint[i] = DBEmpty.Value;
					}
				}

				return m_savePoint;
			}
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 加载延迟属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns></returns>
		private void LoadLazyLoadValue(String propertyName)
		{
			PropertyChain chain = new PropertyChain(Type, new String[] { propertyName });

			Object partialEntity = Entity.DatabaseSession.LoadFirst(
					Type,
					Select.Properties(chain),
					Definition.ComposeLoadFilter(Entity)
				);

			EntityPropertyDefinition propertyDef = Definition.Properties[propertyName];

			LazyLoads[propertyName] = propertyDef.PropertyInfo.GetValue(partialEntity, null);
		}

		#endregion
	}
}