#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeGroupBuilderStrategy.cs
// 文件功能描述：分组结果实体加载策略。
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
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 分组结果实体加载策略。
	/// </summary>
	[Serializable]
	internal sealed class CompositeGroupBuilderStrategy : CompositeBuilderStrategy
	{
		#region 私有字段

		private readonly Type m_groupResultType;

		[NonSerialized]
		private GroupDefinition m_group;

		/// <summary>
		/// 每一个参与分组的外部引用的加载策略，键是属性的名称。
		/// </summary>
		private readonly Dictionary<String, CompositeBuilderStrategy> m_strategies = new Dictionary<String, CompositeBuilderStrategy>();

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		/// <param name="groupResultType">分组结果实体类型。</param>
		public CompositeGroupBuilderStrategy(Type groupResultType)
		{
			m_groupResultType = groupResultType;
			BuildForeignReferenceBuilderStrategies();
		}

		#endregion

		#region 策略

		/// <summary>
		/// 指示不限制级别。
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// 指示要进行属性过滤。
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 加载所有的聚合属性所属的实体架构及上级实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果此实体架构位于聚合属性的路径上或生成策略要求加载，则返回 true；否则返回 false。</returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			foreach (GroupPropertyDefinition propertyDef in Group.Properties)
			{
				if (propertyDef.PropertyChain != null)
				{
					IPropertyChain chain = propertyDef.PropertyChain;

					if (chain.BelongsTo(schema))
					{
						return true;
					}

					if (!propertyDef.IsPrimitive && chain.Contains(schema))
					{
						CompositeBuilderStrategy strategy = GetForeignReferenceBuilderStrategy(propertyDef);

						if (strategy.LoadFromSchema(schema))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// 确定是否不选择给定的实体架构中的任何属性。
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			foreach (GroupPropertyDefinition propertyDef in GroupItems)
			{
				if (schema.OwnProperty(propertyDef.PropertyChain))
				{
					if (propertyDef.IsPrimitive)
					{
						return false;
					}
				}
				else if (!propertyDef.IsPrimitive)
				{
					CompositeBuilderStrategy strategy = GetForeignReferenceBuilderStrategy(propertyDef);

					if (!strategy.SelectNothingFrom(schema))
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// 选择属性。
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			foreach (GroupPropertyDefinition groupPropertyDef in GroupItems)
			{
				if (groupPropertyDef.PropertyChain.Equals(property.PropertyChain))
				{
					if (groupPropertyDef.IsPrimitive)
					{
						return true;
					}
				}
				else if (!groupPropertyDef.IsPrimitive && groupPropertyDef.PropertyChain.Contains(property))
				{
					CompositeBuilderStrategy strategy = GetForeignReferenceBuilderStrategy(groupPropertyDef);

					if (strategy.SelectProperty(property))
					{
						return true;
					}
				}
			}

			return false;
		}

		#endregion

		#region 调试方法

		/// <summary>
		/// 生成策略的详细信息，用于调试。
		/// </summary>
		/// <returns></returns>
		public override String Dump()
		{
			GroupPropertyDefinition[] allForeignRefs = Group.GetForeignReferenceProperties();

			if (allForeignRefs.Length == 0)
			{
				return base.Dump();
			}
			else
			{
				const String PADDING = "    "; // 缩进 4 个空格
				StringBuilder buffer = new StringBuilder();

				buffer.AppendFormat("{0}，各外部引用属性的生成策略为：", GetType().FullName);
				buffer.AppendLine();

				foreach (GroupPropertyDefinition propertyDef in allForeignRefs)
				{
					CompositeBuilderStrategy strategy = GetForeignReferenceBuilderStrategy(propertyDef);
					String info = strategy.Dump();
					info = Regex.Replace(info, "^", PADDING, RegexOptions.Multiline);
					buffer.AppendLine(info);
				}

				return buffer.ToString();
			}
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取分组结果实体定义。
		/// </summary>
		internal GroupDefinition Group
		{
			get
			{
				if (m_group == null)
				{
					m_group = GroupDefinitionBuilder.Build(m_groupResultType);
				}

				return m_group;
			}
		}

		private GroupPropertyDefinition[] m_foreignReferences;

		/// <summary>
		/// 获取所有的外部引用属性。
		/// </summary>
		internal GroupPropertyDefinition[] ForeignReferences
		{
			get
			{
				if (m_foreignReferences == null)
				{
					m_foreignReferences = Group.GetForeignReferenceProperties();
				}

				return m_foreignReferences;
			}
		}

		private GroupPropertyDefinition[] m_groupItems;

		/// <summary>
		/// 获取所有的分组项属性。
		/// </summary>
		internal GroupPropertyDefinition[] GroupItems
		{
			get
			{
				if (m_groupItems == null)
				{
					m_groupItems = Group.GetGroupItemProperties();
				}

				return m_groupItems;
			}
		}

		private GroupPropertyDefinition[] m_primitives;

		/// <summary>
		/// 获取所有的基本属性。
		/// </summary>
		internal GroupPropertyDefinition[] Primitives
		{
			get
			{
				if (m_primitives == null)
				{
					m_primitives = Group.GetPrimitiveProperties();
				}

				return m_primitives;
			}
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 创建外部引用的生成策略。
		/// </summary>
		private void BuildForeignReferenceBuilderStrategies()
		{
			GroupPropertyDefinition[] foreignReferences = Group.GetForeignReferenceProperties();

			foreach (GroupPropertyDefinition propertyDef in foreignReferences)
			{
				CompositeBuilderStrategy strategy = propertyDef.CreateDefaultBuilderStrategy();
				strategy.InitialLevel = propertyDef.Level;

				m_strategies.Add(propertyDef.Name, strategy);
			}
		}

		/// <summary>
		/// 获取参与分组的外部引用属性的实体架构组合生成策略。
		/// </summary>
		/// <param name="propertyDef">外部引用属性。</param>
		/// <returns>外部引用属性的实体架构组合生成策略。</returns>
		private CompositeBuilderStrategy GetForeignReferenceBuilderStrategy(GroupPropertyDefinition propertyDef)
		{
			return m_strategies[propertyDef.Name];
		}

		#endregion
	}
}