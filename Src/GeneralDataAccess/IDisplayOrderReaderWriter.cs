#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：IDisplayOrderReaderWriter.cs
// 文件功能描述：显示序号读写器，用于类 DisplayOrderAllocator。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110331
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
	/// 显示序号读写器，用于类 DisplayOrderAllocator。
	/// </summary>
	/// <typeparam name="TEntity">实体类型。</typeparam>
	public interface IDisplayOrderReaderWriter<TEntity> where TEntity : class
	{
		/// <summary>
		/// 获取项的显示序号。
		/// </summary>
		/// <param name="item">项。</param>
		/// <returns>显示序号。</returns>
		Int32 GetDisplayOrder(TEntity item);

		/// <summary>
		/// 设置项的显示序号。
		/// </summary>
		/// <param name="item">项。</param>
		/// <param name="displayOrder">要设置的显示序号。</param>
		void SetDisplayOrder(TEntity item, Int32 displayOrder);
	}
}