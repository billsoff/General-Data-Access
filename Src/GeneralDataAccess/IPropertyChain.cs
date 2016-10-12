#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IPropertyChain.cs
// 文件功能描述：属性链。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110527
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
	/// 属性链。
	/// </summary>
	public interface IPropertyChain
	{
		#region 属性

		/// <summary>
		/// 获取属性链的深度。
		/// </summary>
		Int32 Depth { get; }

		/// <summary>
		/// 获取属性全名称（从实体类型名开始以点分隔的属性列表）。
		/// </summary>
		String FullName { get; }

		/// <summary>
		/// 获取头部。
		/// </summary>
		IPropertyChain Head { get; }

		/// <summary>
		/// 获取一个值，该值指示当前属性是否为子实体列表属性。
		/// </summary>
		Boolean IsChildren { get; }

		/// <summary>
		/// 获取一个值，该值指示属性是否为目标实体的直属属性。
		/// </summary>
		Boolean IsImmediateProperty { get; }

		/// <summary>
		/// 获取一个值，该值指示属性是否为基本属性。
		/// </summary>
		Boolean IsPrimitive { get; }

		/// <summary>
		/// 获取属性名称。
		/// </summary>
		String Name { get; }

		/// <summary>
		/// 获取下一个属性节点。
		/// </summary>
		IPropertyChain Next { get; }

		/// <summary>
		/// 获取前一个属性节点。
		/// </summary>
		IPropertyChain Previous { get; }

		/// <summary>
		/// 获取属性路径列表。
		/// </summary>
		String[] PropertyPath { get; }

		/// <summary>
		/// 获取目标实体类型。
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// 获取属性类型。
		/// </summary>
		Type PropertyType { get; }

		#endregion

		#region 方法

		/// <summary>
		/// 判断是否属于实体架构。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果属于该实体架构，则返回 true；否则返回 false。</returns>
		Boolean BelongsTo(EntitySchema schema);

		/// <summary>
		/// 判断是否属于属性。
		/// </summary>
		/// <param name="property">要判断的属性。</param>
		/// <returns>如果属于该属性，则返回 true；否则返回 false。</returns>
		Boolean BelongsTo(EntityProperty property);

		/// <summary>
		/// 判断是否包含实体架构。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果包含该实体架构，则返回 true；否则返回 false。</returns>
		Boolean Contains(EntitySchema schema);

		/// <summary>
		/// 判断是否包含属性。
		/// </summary>
		/// <param name="property">要判断的属性。</param>
		/// <returns>如果包含该属性，则返回 true；否则返回 false。</returns>
		Boolean Contains(EntityProperty property);

		/// <summary>
		/// 获取当前实例的副本，新实例有独立的 Head 和 Previous 属性，而 Next 属性为空。
		/// </summary>
		/// <returns>当前实例的副本。</returns>
		IPropertyChain Copy();

		/// <summary>
		/// 获取实体中属性的值。
		/// </summary>
		/// <param name="entity">实体，类型必须与属性链的目标实体类型相同。</param>
		/// <returns>属性的值，如果属性尚未加载（DBEmpty），则返回 null。</returns>
		Object GetPropertyValue(Object entity);

		/// <summary>
		/// 获取实体中属性的值。
		/// </summary>
		/// <typeparam name="TResult">属性类型。</typeparam>
		/// <param name="entity">实体，类型必须与属性链的目标实体类型相同。</param>
		/// <returns>属性的值，如果属性尚未加载（DBEmpty），则返回 null。</returns>
		TResult GetPropertyValue<TResult>(Object entity);

		/// <summary>
		/// 判断是否拥有属性。
		/// </summary>
		/// <param name="property">属性。</param>
		/// <returns>如果拥有该属性，则返回 true；否则返回 false。</returns>
		Boolean OwnProperty(EntityProperty property);

		/// <summary>
		/// 向前延伸属性链，属性链的类型为目标属性链的类型。
		/// </summary>
		/// <param name="target">目标属性链。</param>
		/// <returns>延伸后新创建的属性链。</returns>
		IPropertyChain Preppend(IPropertyChain target);

		#endregion
	}
}