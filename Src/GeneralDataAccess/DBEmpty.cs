#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DBEmpty.cs
// 文件功能描述：表示数据库记录中一个字段的值尚未设置（不同于 DBNull，当向数据库提交插入或修改指令时，前者提示跳过此字段，使用数据库的默认值，后者将字段值设为 NULL）。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110415
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示数据库记录中一个字段的值尚未设置（不同于 DBNull，当向数据库提交插入或修改指令时，前者提示跳过此字段，使用数据库的默认值，后者将字段值设为 NULL）。
	/// </summary>
	[Serializable]
	public sealed class DBEmpty : ISerializable
	{
		#region 单例字段

		/// <summary>
		/// 表示 <see cref="DBEmpty"/> 类的唯一实例。
		/// </summary>
		public static readonly DBEmpty Value = new DBEmpty();

		#endregion

		#region 构造函数

		/// <summary>
		/// 私有化，支持单例。
		/// </summary>
		private DBEmpty()
		{
		}

		/// <summary>
		/// ISerializable 的构造函数，实际上从不会调用。
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private DBEmpty(SerializationInfo info, StreamingContext context)
		{
		}

		#endregion

		#region ISerializable 成员

		/// <summary>
		/// 序列化，通过设置 IObjectReference 类型使反序列化的类型总是指向应用程序域的同一个单例。
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof(DBEmptySerializationHelper));
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 字符串表示，返回空串（String.Empty）。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Empty;
		}

		/// <summary>
		/// 字符串表示，返回空串（String.Empty）。
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public String ToString(IFormatProvider provider)
		{
			return String.Empty;
		}

		#endregion


		#region 辅助类 DBEmptySerializationHelper

		/// <summary>
		/// 保证序列化后的对象指向应用程序域的单例。
		/// </summary>
		[Serializable]
		private class DBEmptySerializationHelper : IObjectReference
		{
			#region IObjectReference 成员

			/// <summary>
			/// 获取 DBEmpty 的实例，总是返回其唯一实例。
			/// </summary>
			/// <param name="context"></param>
			/// <returns></returns>
			public Object GetRealObject(StreamingContext context)
			{
				return DBEmpty.Value;
			}

			#endregion
		}

		#endregion
	}
}