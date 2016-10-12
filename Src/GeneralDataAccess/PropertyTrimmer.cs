#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertyTrimmer.cs
// 文件功能描述：用于指示从实体架构组合策略中移除特定的属性。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于指示从实体架构组合策略中移除特定的属性。
	/// <para>实现者应支持序列化。</para>
	/// </summary>
	[Serializable]
	public abstract class PropertyTrimmer
	{
		#region 工厂方法

		/// <summary>
		/// 创建用于修剪不支持分组的属性的修剪器。
		/// </summary>
		/// <returns>创建好的修剪器。</returns>
		public static PropertyTrimmer CreateGroupIncapableTrimmer()
		{
			return new GroupIncapablePropertyTrimmer();
		}

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		protected PropertyTrimmer()
		{
		}

		#endregion

		/// <summary>
		/// 获取显示名称，用于调试。
		/// </summary>
		public virtual String DisplayName
		{
			get
			{
				return ToString();
			}
		}

		/// <summary>
		/// 指示是否从实体架构组合中移除此属性（即不加载）。
		/// </summary>
		/// <param name="property">实体架构属性。</param>
		/// <returns>如果要移除此属性，则返回 true；否则返回 false。</returns>
		public abstract Boolean TrimOff(EntityProperty property);
	}
}