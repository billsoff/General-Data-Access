#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeForeignReferencePropertyDefinition.cs
// 文件功能描述：复合实体表达式架构属性定义。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110708
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 复合实体表达式架构属性定义。
	/// </summary>
	internal sealed class CompositeForeignReferencePropertyDefinition : CompositePropertyDefinition
	{
		#region 私有字段

		private readonly PropertyJoinMode m_joinMode;
		private readonly List<String[]> m_rootPropertyPaths = new List<String[]>();
		private readonly List<String[]> m_mappedPropertyPaths = new List<String[]>();

		private IPropertyChain[] m_rootPropertyChains;
		private IPropertyChain[] m_mappedPropertyChains;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置属性信息。
		/// </summary>
		/// <param name="definition">复合实体定义。</param>
		/// <param name="pf">属性信息。</param>
		public CompositeForeignReferencePropertyDefinition(CompositeDefinition definition, PropertyInfo pf)
			: base(definition, pf)
		{
			CompositeForeignReferenceAttribute refAttr = (CompositeForeignReferenceAttribute)Attribute.GetCustomAttribute(
					pf,
					typeof(CompositeForeignReferenceAttribute)
				);
			m_joinMode = refAttr.PropertyJoinMode;

			PropertyMappingAttribute[] allMappingAttrs = (PropertyMappingAttribute[])Attribute.GetCustomAttributes(
					pf,
					typeof(PropertyMappingAttribute)
				);

			foreach (PropertyMappingAttribute mappingAttr in allMappingAttrs)
			{
				m_rootPropertyPaths.Add(mappingAttr.RootPropertyPath);
				m_mappedPropertyPaths.Add(mappingAttr.ForeignReferencePropertyPath);
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取属性连接方式。
		/// </summary>
		public PropertyJoinMode JoinMode
		{
			get { return m_joinMode; }
		}

		/// <summary>
		/// 获取根属性链列表。
		/// </summary>
		public IPropertyChain[] RootPropertyChains
		{
			get
			{
				if (m_rootPropertyChains == null)
				{
					List<IPropertyChain> rootChains = m_rootPropertyPaths.ConvertAll<IPropertyChain>(
							delegate(String[] propertyPath)
							{
								return new PropertyChain(Composite.Root.Type, propertyPath);
							}
						);

					m_rootPropertyChains = rootChains.ToArray();
				}

				return m_rootPropertyChains;
			}
		}

		/// <summary>
		/// 获取映射属性链列表。
		/// </summary>
		public IPropertyChain[] MappedPropertyChains
		{
			get
			{
				if (m_mappedPropertyChains == null)
				{
					List<IPropertyChain> mappedChains = m_mappedPropertyPaths.ConvertAll<IPropertyChain>(
							delegate(String[] propertyPath)
							{
								return new PropertyChain(Type, propertyPath);
							}
						);

					m_mappedPropertyChains = mappedChains.ToArray();
				}

				return m_mappedPropertyChains;
			}
		}

		#endregion
	}
}