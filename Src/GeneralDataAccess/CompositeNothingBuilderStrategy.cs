#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeNothingBuilderStrategy.cs
// 文件功能描述：不选择任何属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110524
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
	/// 不选择任何属性。
	/// </summary>
	[Serializable]
	internal sealed class CompositeNothingBuilderStrategy : CompositeBuilderStrategy, ISerializable
	{
		#region 单例

		public static readonly CompositeNothingBuilderStrategy Value = new CompositeNothingBuilderStrategy();

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，私有化，以实现单例。
		/// </summary>
		private CompositeNothingBuilderStrategy()
		{
		}

		/// <summary>
		/// ISerializable 的构造函数，实际上从不会调用。
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private CompositeNothingBuilderStrategy(SerializationInfo info, StreamingContext context)
		{
		}

		#endregion

		#region 策略

		/// <summary>
		/// 总是为 0。
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// 只加载第 0 级实体架构。
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			return (schema.Level == 0);
		}

		/// <summary>
		/// 不加载任何属性。
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 不加载任何属性。
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			return true;
		}

		/// <summary>
		/// 不加载任何属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			return false;
		}

		#endregion

		#region 调试信息

		/// <summary>
		/// 获取策略的详细信息。
		/// </summary>
		/// <returns></returns>
		public override String Dump()
		{
			return String.Format("{0}：不选择任何属性", GetType().FullName);
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
			info.SetType(typeof(CompositeNothingBuilderStrategySerializationHelper));
		}

		#endregion


		#region 辅助类 CompositeNothingBuilderStrategySerializationHelper

		/// <summary>
		/// 保证序列化的对象指向应用程序域的单例。
		/// </summary>
		[Serializable]
		private class CompositeNothingBuilderStrategySerializationHelper : IObjectReference
		{
			#region IObjectReference 成员

			/// <summary>
			/// 获取 CompositeNothingBuilderStrategy，总是返回唯一实例。
			/// </summary>
			/// <param name="context"></param>
			/// <returns></returns>
			public Object GetRealObject(StreamingContext context)
			{
				return CompositeNothingBuilderStrategy.Value;
			}

			#endregion
		}

		#endregion
	}
}