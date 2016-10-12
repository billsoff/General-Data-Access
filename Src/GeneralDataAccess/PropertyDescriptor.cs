#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertyDescriptor.cs
// 文件功能描述：属性描述符，表示定义的属性列表。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110430
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 属性描述符，表示定义的属性列表。
	/// </summary>
	[Serializable]
	public abstract class PropertyDescriptor : IPropertyChainBuilder
	{
		#region 私有字段

		private Type m_type;
		private PropertyChainBuilder m_builder;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，如果调用此函数，则一定要在类型上标记 PropertyDescriptorAttribute。
		/// </summary>
		protected PropertyDescriptor()
		{
		}

		/// <summary>
		/// 构造函数，设置实体类型，用于定义属性链的归属。
		/// </summary>
		/// <param name="type"></param>
		protected PropertyDescriptor(Type type)
		{
			#region 前置条件

			Debug.Assert((type != null), "实体类型参数 type 不能为空。");

			#endregion

			m_type = type;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取一个值，指示当前可否生成属性链。
		/// </summary>
		public Boolean Available
		{
			get { return Builder.Available; }
		}

		/// <summary>
		/// 获取目标实体类型。
		/// </summary>
		public Type Type
		{
			get { return Builder.Type; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 生成属性链。
		/// </summary>
		/// <returns>生成好的属性链。</returns>
		public IPropertyChain Build()
		{
			if (!Available)
			{
				ExpressionSchemaType schemaType = ExpressionSchemaBuilderFactory.GetExpressionSchemaType(this.Type);

				if (schemaType == ExpressionSchemaType.Entity)
				{
					EntityDefinition definition = EntityDefinitionBuilder.Build(this.Type);
					EntityPropertyDefinition[] primaryKeys = definition.Properties.GetPrimaryKeys();

					if (primaryKeys.Length == 1)
					{
						Builder.LinkProperty(primaryKeys[0].Name);
					}
				}
			}

			#region 前置条件

			Debug.Assert(Available, "属性列表为空，应至少设置一个属性。");

			#endregion

			return Builder.Build();
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取属性链生成器。
		/// </summary>
		internal PropertyChainBuilder Builder
		{
			get
			{
				#region 前置断言

				Debug.Assert(
						(m_type != null) || Attribute.IsDefined(GetType(), typeof(PropertyDescriptorAttribute)),
						String.Format(
								"请在类型 {0} 上标记 PropertyDescriptorAttribute 或通过构造函数设置属性描述符的目标类型。",
								GetType().FullName
							)
					);

				#endregion

				if (m_builder == null)
				{
					if (m_type == null)
					{
						PropertyDescriptorAttribute descriptorAttr = (PropertyDescriptorAttribute)Attribute.GetCustomAttribute(
								GetType(),
								typeof(PropertyDescriptorAttribute)
							);

						m_type = descriptorAttr.Type;
					}

					m_builder = new PropertyChainBuilder(m_type);
				}

				return m_builder;
			}
		}

		#endregion

		#region 内部方法

		/// <summary>
		/// 链接基本属性。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns></returns>
		protected IPropertyChain LinkPrimitiveProperty(String propertyName)
		{
			Builder.LinkProperty(propertyName);

			IPropertyChain chain = Build();

			return chain;
		}

		/// <summary>
		/// 链接外部引用属性。
		/// </summary>
		/// <typeparam name="TForeignReference">外部引用属性类型定义。</typeparam>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>外部引用属性定义。</returns>
		protected TForeignReference LinkForeignReferenceProperty<TForeignReference>(String propertyName)
			where TForeignReference : PropertyDescriptor, new()
		{
			Builder.LinkProperty(propertyName);

			TForeignReference result = new TForeignReference();

			result.m_builder = Builder;

			return result;
		}

		#endregion
	}
}