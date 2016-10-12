#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeItemSettings.cs
// 文件功能描述：针对复合实体中一项表达式架构的过滤器。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110708
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
	/// 针对复合实体中一项表达式架构的过滤器。
	/// </summary>
	[Serializable]
	public sealed class CompositeItemSettings
	{
		#region 私有字段

		private readonly CompositeSettings m_compositeSettings;
		private readonly String m_propertyName;

		private CompositeBuilderStrategy m_select;
		private Filter m_where;
		private Filter m_having;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置过滤器所属的列表和属性名称。
		/// </summary>
		/// <param name="settings">列表。</param>
		/// <param name="propertyName">属性名称。</param>
		internal CompositeItemSettings(CompositeSettings settings, String propertyName)
		{
			m_compositeSettings = settings;
			m_propertyName = propertyName;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取当前实例所在的列表。
		/// </summary>
		public CompositeSettings CompositeSettings
		{
			get { return m_compositeSettings; }
		}

		/// <summary>
		/// 获取复合实体的属性名称。
		/// </summary>
		public String PropertyName
		{
			get { return m_propertyName; }
		}

		/// <summary>
		/// 获取或设置加载策略。
		/// </summary>
		public CompositeBuilderStrategy Select
		{
			get { return m_select; }
			set { m_select = value; }
		}

		/// <summary>
		/// 获取或设置 WHERE 过滤器。
		/// </summary>
		public Filter Where
		{
			get { return m_where; }
			set { m_where = value; }
		}

		/// <summary>
		/// 获取或设置 HAVING 过滤器。
		/// </summary>
		public Filter Having
		{
			get { return m_having; }
			set { m_having = value; }
		}

		#endregion
	}
}