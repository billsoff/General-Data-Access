#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertySelector.cs
// 文件功能描述：属性选择器，选择要加载的属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 2011
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
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 属性选择器，选择要加载的属性。
	/// </summary>
	[Serializable]
	public abstract class PropertySelector
	{
		#region 工厂方法

		/// <summary>
		/// 创建目标实体类型的选择器，只适用于 AllFromSchema、LoadSchemaOnly 和 PrimaryKey。
		/// </summary>
		/// <param name="selectMode">选择模式。</param>
		/// <param name="entityType">目标实体类型。</param>
		/// <returns>属性选择器。</returns>
		public static PropertySelector Create(PropertySelectMode selectMode, Type entityType)
		{
			switch (selectMode)
			{
				case PropertySelectMode.AllFromSchema:
					return new AllFromSchemaPropertySelector(entityType);

				case PropertySelectMode.AllExceptLazyLoadFromSchema:
					return new AllExceptLazyLoadFromSchemaPropertySelector(entityType);

				case PropertySelectMode.LoadSchemaOnly:
					return new LoadSchemaOnlyPropertySelector(entityType);

				case PropertySelectMode.PrimaryKey:
					return new PrimaryKeyPropertySelector(entityType);

				case PropertySelectMode.Property:
					Debug.Fail("不支持 Type 类型的参数。");
					break;

				default:
					Debug.Fail("不能识别 selectMode");
					break;
			}

			return null;
		}

		/// <summary>
		/// 创建属性选择器。
		/// </summary>
		/// <param name="selectMode">创建模式。</param>
		/// <param name="chain">属性链。</param>
		/// <returns>属性选择器。</returns>
		public static PropertySelector Create(PropertySelectMode selectMode, IPropertyChain chain)
		{
			switch (selectMode)
			{
				case PropertySelectMode.AllFromSchema:
					return new AllFromSchemaPropertySelector(chain);

				case PropertySelectMode.AllExceptLazyLoadFromSchema:
					return new AllExceptLazyLoadFromSchemaPropertySelector(chain);

				case PropertySelectMode.LoadSchemaOnly:
					return new LoadSchemaOnlyPropertySelector(chain);

				case PropertySelectMode.Property:
					return new ActualPropertySelector(chain);

				case PropertySelectMode.PrimaryKey:
					return new PrimaryKeyPropertySelector(chain);

				default:
					Debug.Fail("不能识别 selectMode");
					break;
			}

			return null;
		}

		/// <summary>
		/// 创建属性选择器。
		/// </summary>
		/// <param name="selectMode">创建模式。</param>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>属性选择器。</returns>
		public static PropertySelector Create(PropertySelectMode selectMode, IPropertyChainBuilder builder)
		{
			switch (selectMode)
			{
				case PropertySelectMode.AllFromSchema:
					return new AllFromSchemaPropertySelector(builder);

				case PropertySelectMode.AllExceptLazyLoadFromSchema:
					return new AllExceptLazyLoadFromSchemaPropertySelector(builder);

				case PropertySelectMode.LoadSchemaOnly:
					return new LoadSchemaOnlyPropertySelector(builder);

				case PropertySelectMode.Property:
					return new ActualPropertySelector(builder);

				case PropertySelectMode.PrimaryKey:
					return new PrimaryKeyPropertySelector(builder);

				default:
					Debug.Fail("不能识别 selectMode");
					break;
			}

			return null;
		}

		/// <summary>
		/// 创建 Property 模式的选择器。
		/// </summary>
		/// <param name="chain">属性链。</param>
		/// <returns>属性选择器。</returns>
		public static PropertySelector Create(IPropertyChain chain)
		{
			return Create(PropertySelectMode.Property, chain);
		}

		/// <summary>
		/// 创建 Property 模式的选择器。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		/// <returns>属性选择器。</returns>
		public static PropertySelector Create(IPropertyChainBuilder builder)
		{
			return Create(PropertySelectMode.Property, builder);
		}

		#endregion

		#region 私有字段

		private readonly Type m_type;
		private readonly IPropertyChain m_propertyChain;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置实体类型。
		/// </summary>
		/// <param name="entityType">实体类型，不能为空。</param>
		internal PropertySelector(Type entityType)
		{
			#region 前置条件

			Debug.Assert((entityType != null), "实体类型参数 entityType 不能为空。");

			#endregion

			m_type = entityType;
		}

		/// <summary>
		/// 构造函数，设置属性链生成器。
		/// </summary>
		/// <param name="builder">属性链生成器。</param>
		internal PropertySelector(IPropertyChainBuilder builder)
			: this(builder.Build())
		{
		}

		/// <summary>
		/// 构造函数，设置属性链。
		/// </summary>
		/// <param name="chain">属性链，不能为空。</param>
		internal PropertySelector(IPropertyChain chain)
		{
			#region 前置条件

			Debug.Assert((chain != null), "属性链参数 chain 不能这空。");

			#endregion

			m_type = chain.Type;
			m_propertyChain = chain;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取显示名称。
		/// </summary>
		public virtual String DisplayName
		{
			get { return Name; }
		}

		/// <summary>
		/// 获取选择器的名称。
		/// </summary>
		public String Name
		{
			get
			{
				if (PropertyChain != null)
				{
					return PropertyChain.FullName;
				}
				else
				{
					return m_type.Name;
				}
			}
		}

		/// <summary>
		/// 获取属性链。
		/// </summary>
		public IPropertyChain PropertyChain
		{
			get { return m_propertyChain; }
		}

		/// <summary>
		/// 获取实体类型。
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 指示是否加载指定的实体架构，总是加载所有位于属性链上的实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果需要加载该实体架构，则返回 true；否则返回 false。</returns>
		public Boolean LoadSchema(EntitySchema schema)
		{
			#region 前置条件

			Debug.Assert((schema != null), "要判断的实体架构参数 schema 不能为空。");

			#endregion

			return Contains(schema);
		}

		/// <summary>
		/// 指示是否不从指定的实体架构中选择任何属性。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果不从该实体架构中选择任何属性，则返回 true；否则返回 false。</returns>
		public Boolean SelectNothingFrom(EntitySchema schema)
		{
			#region 前置条件

			Debug.Assert((schema != null), "要判断的实体架构参数 schema 不能为空。");

			#endregion

			return SelectNothingFromImpl(schema);
		}

		/// <summary>
		/// 指示是否选择指定的属性。
		/// </summary>
		/// <param name="property">属性。</param>
		/// <returns>如果要选择该属性，则返回 true；否则返回 false。</returns>
		public Boolean SelectProperty(EntityProperty property)
		{
			#region 前置条件

			Debug.Assert((property != null), "要判断的属性参数 property 不能为空。");

			#endregion

			return SelectPropertyImpl(property);
		}

		/// <summary>
		/// 显示名称。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return DisplayName;
		}

		#endregion

		#region 抽象成员

		#region 公共属性

		/// <summary>
		/// 属性属性选择模式。
		/// </summary>
		public abstract PropertySelectMode SelectMode { get; }

		#endregion

		#region 保护的方法

		/// <summary>
		/// 指示是否不从指定的实体架构中选择任何属性。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果不从该实体架构中选择任何属性，则返回 true；否则返回 false。</returns>
		protected abstract Boolean SelectNothingFromImpl(EntitySchema schema);

		/// <summary>
		/// 指示是否选择指定的属性。
		/// </summary>
		/// <param name="property">属性。</param>
		/// <returns>如果要选择该属性，则返回 true；否则返回 false。</returns>
		protected abstract Boolean SelectPropertyImpl(EntityProperty property);

		#endregion

		#endregion

		#region 保护的方法

		/// <summary>
		/// 判断是否包含实体架构。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果包含实体架构，则返回 true；否则返回 false。</returns>
		protected Boolean Contains(EntitySchema schema)
		{
			#region 前置条件

			Debug.Assert((Type == schema.Composite.Target.Type), "实体架构的目标实体类型与选择器的目标实体类型不同。");

			#endregion

			return Name.StartsWith(schema.PropertyPath, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// 判断是否包含属性。
		/// </summary>
		/// <param name="property">属性。</param>
		/// <returns>如果包含属性，则返回 true；否则返回 false。</returns>
		protected Boolean Contains(EntityProperty property)
		{
			#region 前置条件

			Debug.Assert((Type == property.Schema.Composite.Target.Type), "属性的目标实体类型与选择器的目标实体类型不同。");

			#endregion

			return Name.StartsWith(property.PropertyChain.FullName, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// 判断是否拥有属性。
		/// </summary>
		/// <param name="property">属性。</param>
		/// <returns>如果拥有该属性，则返回 true；否则返回 false。</returns>
		protected Boolean OwnProperty(EntityProperty property)
		{
			#region 前置条件

			Debug.Assert((Type == property.Schema.Composite.Target.Type), "属性的目标实体类型与选择器的目标实体类型不同。");

			#endregion

			return Name.Equals(property.Schema.PropertyPath, CommonPolicies.PropertyNameComparison);
		}

		#endregion
	}
}