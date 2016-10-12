#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupIncapablePropertyTrimmer.cs
// 文件功能描述：用于从选择列表移除不支持分组的属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110630
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
	/// 用于从选择列表移除不支持分组的属性。
	/// </summary>
	[Serializable]
	internal sealed class GroupIncapablePropertyTrimmer : PropertyTrimmer
	{
		/// <summary>
		/// 获取显示名称。
		/// </summary>
		public override String DisplayName
		{
			get
			{
				return "从选择列表中移除标记为不支持分组的属性";
			}
		}

		/// <summary>
		/// 指示从选择列表中移除标记为不支持分组的属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public override Boolean TrimOff(EntityProperty property)
		{
			return property.IsCustomAttributeDefined(typeof(NotSupportGroupingAttribute));
		}
	}
}