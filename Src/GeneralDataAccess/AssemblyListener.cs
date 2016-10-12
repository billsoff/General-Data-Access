#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：AssemblyListener.cs
// 文件功能描述：子实体装配监听器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110729
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
	/// 子实体装配监听器。
	/// </summary>
	[Serializable]
	public abstract class AssemblyListener
	{
		#region 私有字段

		private readonly Type m_type;
		private readonly IPropertyChain m_propertyChain;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置根类型。
		/// </summary>
		/// <param name="type">根类型。</param>
		protected AssemblyListener(Type type)
		{
			m_type = type;
		}

		/// <summary>
		/// 构造函数，设置子实体属性。
		/// </summary>
		/// <param name="builder">子实体属性链生成器。</param>
		protected AssemblyListener(IPropertyChainBuilder builder)
			: this(builder.Build())
		{
		}

		/// <summary>
		/// 构造函数，设置子实体属性。
		/// </summary>
		/// <param name="chain">子实体属性链。</param>
		protected AssemblyListener(IPropertyChain chain)
			: this(chain.Type)
		{
			m_propertyChain = chain;
		}

		#endregion

		#region 保护的属性

		/// <summary>
		/// 获取根类型。
		/// </summary>
		protected Type Type
		{
			get { return m_type; }
		}

		/// <summary>
		/// 获取子实体属性链。
		/// </summary>
		protected IPropertyChain PropertyChain
		{
			get { return m_propertyChain; }
		}

		#endregion

		/// <summary>
		/// 当开始装配时执行此方法。
		/// </summary>
		/// <param name="policy">装配方针。</param>
		/// <param name="parentEntities">根实体集合。</param>
		/// <param name="filter">根实体过滤器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		public virtual void OnAssemblyStart(AssemblyPolicy policy, Object[] parentEntities, Filter filter, IDatabaseSession databaseSession)
		{
		}

		/// <summary>
		/// 当开始装配子实体时执行此方法。
		/// </summary>
		/// <param name="policy">装配方针。</param>
		/// <param name="chain">子实体属性链（从根实体开始）。</param>
		/// <param name="parentEntities">父实体集合。</param>
		/// <param name="filter">过滤器。</param>
		/// <param name="sorter">排序器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		public virtual void OnChildrenAssemblyStart(AssemblyPolicy policy, IPropertyChain chain, Object[] parentEntities, Filter filter, Sorter sorter, IDatabaseSession databaseSession)
		{
		}

		/// <summary>
		/// 当子实体装配完成时执行此方法。
		/// </summary>
		/// <param name="policy">装配方针。</param>
		/// <param name="chain">子实体属性链（从根实体开始）。</param>
		/// <param name="parentEntities">父实体集合。</param>
		/// <param name="children">加载的子实体集合。</param>
		/// <param name="filter">用于加载子实体的过滤器。</param>
		/// <param name="sorter">用于加载子实体的排序器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		public virtual void OnChildrenAssemblyComplete(AssemblyPolicy policy, IPropertyChain chain, Object[] parentEntities, Object[] children, Filter filter, Sorter sorter, IDatabaseSession databaseSession)
		{
		}

		/// <summary>
		/// 当装配完成时执行此方法。
		/// </summary>
		/// <param name="policy">装配方针。</param>
		/// <param name="parentEnttities">根实体集合。</param>
		/// <param name="filter">根实体过滤器。</param>
		/// <param name="databaseSession">数据库会话引擎。</param>
		public virtual void OnAssemblyComplete(AssemblyPolicy policy, Object[] parentEnttities, Filter filter, IDatabaseSession databaseSession)
		{
		}

		#region 保护的方法

		/// <summary>
		/// 将面向根实体的子实体属性转换为面向子实体的属性路径。
		/// </summary>
		/// <param name="policy"></param>
		/// <param name="chain"></param>
		/// <returns></returns>
		protected static String[] Transform(AssemblyPolicy policy, IPropertyChain chain)
		{
			AssemblyChildrenEntry childrenEntry = policy.ChildrenEntries[chain.PropertyPath];
			List<String> propertyNames = new List<String>();
			AssemblyChildrenEntry current = childrenEntry;

			do
			{
				propertyNames.Add(current.ChildrenProperty.Name);
				current = current.Parent;
			} while (current != null);

			return propertyNames.ToArray();
		}

		#endregion
	}
}