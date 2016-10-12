#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ColumnLocator.cs
// 文件功能描述：包含一个属性序列，用来定位列。
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
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 包含一个属性序列，用来定位列。
	/// </summary>
	[Serializable]
	public class ColumnLocator
	{
		#region 私有字段

		private readonly String m_scope;
		private readonly String m_propertyName;

		private String[] m_propertyPath;

		#endregion

		/// <summary>
		/// 构造函数，设置值属性名称。
		/// </summary>
		/// <param name="propertyName">值属性名称。</param>
		public ColumnLocator(String propertyName)
			: this(null, propertyName)
		{
		}

		/// <summary>
		/// 构造函数，设置实体属性名称和值属性名称。
		/// </summary>
		/// <param name="entityPropertyName">实体属性名称。</param>
		/// <param name="propertyName">值属性名称。</param>
		public ColumnLocator(String entityPropertyName, String propertyName)
		{
			m_scope = entityPropertyName;
			m_propertyName = propertyName;
		}

		/// <summary>
		/// 构造函数，设置属性路径。
		/// </summary>
		/// <param name="propertyPath">属性路径。</param>
		public ColumnLocator(IList<String> propertyPath)
		{
			#region 前置条件

			Debug.Assert(((propertyPath != null) && (propertyPath.Count != 0)), "至少应提供一个属性。");

			#endregion

			if (propertyPath.Count == 1)
			{
				m_propertyName = propertyPath[0];
			}
			else
			{
				m_scope = propertyPath[0];
				m_propertyName = propertyPath[1];
			}

			#region 设置属性路径

			m_propertyPath = new String[propertyPath.Count];

			for (Int32 i = 0; i < m_propertyPath.Length; i++)
			{
				m_propertyPath[i] = propertyPath[i];
			}

			#endregion
		}

		#region 公共属性

		/// <summary>
		/// 获取以点相连接的属性路径。
		/// </summary>
		public String FullName
		{
			get { return String.Join(".", PropertyPath); }
		}

		/// <summary>
		/// 获取实体属性名称。
		/// </summary>
		public String Scope
		{
			get { return m_scope; }
		}

		/// <summary>
		/// 获取值属性名称。
		/// </summary>
		public String PropertyName
		{
			get { return m_propertyName; }
		}

		/// <summary>
		/// 获取属性路径。
		/// </summary>
		public String[] PropertyPath
		{
			get
			{
				if (m_propertyPath == null)
				{
					if ((m_scope != null) && (m_propertyName != null))
					{
						m_propertyPath = new String[] { m_scope, m_propertyName };
					}
					else if (m_scope != null)
					{
						m_propertyPath = new String[] { m_scope };
					}
					else
					{
						m_propertyPath = new String[] { m_propertyName };
					}
				}

				return m_propertyPath;
			}
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建属性链。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>IPropertyChain 实例。</returns>
		public IPropertyChain Create(Type entityType)
		{
			#region 前置条件

			Debug.Assert((entityType != null), "实体类型参数 entityType 不能为空。");

			#endregion

			return new PropertyChain(entityType, PropertyPath);
		}

		#endregion
	}
}