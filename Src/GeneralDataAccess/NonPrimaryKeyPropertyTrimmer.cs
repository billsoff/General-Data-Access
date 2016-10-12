#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NonPrimaryKeyPropertyTrimmer.cs
// 文件功能描述：从实体架构中移除非主键属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110601
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
	/// 从实体架构中移除非主键属性。
	/// </summary>
	[Serializable]
	internal sealed class NonPrimaryKeyPropertyTrimmer : PropertyTrimmer
	{
		#region 私有字段

		private readonly IPropertyChain m_propertyChain;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，指示移除实体中直属的非主键属性。
		/// </summary>
		public NonPrimaryKeyPropertyTrimmer()
		{
		}

		/// <summary>
		/// 构造函数，设置外部引用属性链，如果为空，则指示移除实体中的直属的非主键属性。
		/// </summary>
		/// <param name="propertyChain">外部引用属性链。</param>
		public NonPrimaryKeyPropertyTrimmer(IPropertyChain propertyChain)
		{
			#region 前置条件

			Debug.Assert((propertyChain == null) || !propertyChain.IsPrimitive, "属性链参数应 propertyChain 为引用属性。");

			#endregion

			m_propertyChain = propertyChain;
		}

		/// <summary>
		/// 构造函数，设置外部引用属性链生成器。
		/// </summary>
		/// <param name="builder">外部引用属性链生成器。</param>
		public NonPrimaryKeyPropertyTrimmer(IPropertyChainBuilder builder)
			: this(builder.Build())
		{
		}

		#endregion

		#region PropertyTrimmer 成员

		/// <summary>
		/// 获取显示名称，用于调试。
		/// </summary>
		public override String DisplayName
		{
			get
			{
				if (m_propertyChain != null)
				{
					return String.Format("修剪掉 {0} 中所有非主键属性", m_propertyChain.FullName);
				}
				else
				{
					return "修剪掉实体中直属的所有非主键属性";
				}
			}
		}

		/// <summary>
		/// 移除指定外部引用的非主键属性。
		/// </summary>
		/// <param name="property">属性。</param>
		/// <returns>指示是否移除指定的属性。</returns>
		public override Boolean TrimOff(EntityProperty property)
		{
			if (property.IsPrimaryKey)
			{
				return false;
			}

			if (m_propertyChain != null)
			{
				return m_propertyChain.OwnProperty(property);
			}
			else
			{
				return property.PropertyChain.IsImmediateProperty;
			}
		}

		#endregion
	}
}