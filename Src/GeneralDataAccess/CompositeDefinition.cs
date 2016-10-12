#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeDefinition.cs
// 文件功能描述：复合实体定义。
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
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 复合实体定义。
	/// </summary>
	internal sealed partial class CompositeDefinition
	{
		#region 私有字段

		private readonly Type m_type;
		private readonly ConstructorInfo m_constructor;
		private readonly CompositeRootPropertyDefinition m_root;
		private readonly CompositeForeignReferencePropertyDefinitionCollection m_foreignReferences;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置复合实体类型。
		/// </summary>
		/// <param name="compositeResultType">复合实体类型。</param>
		public CompositeDefinition(Type compositeResultType)
		{
			#region 前置条件

			Debug.Assert(
					typeof(CompositeResult).IsAssignableFrom(compositeResultType),
					String.Format("类型 {0} 没有从 CompositeResult 派生。", compositeResultType.FullName)
				);

			Debug.Assert(
					Attribute.IsDefined(compositeResultType, typeof(CompositeAttribute)),
					String.Format("类型 {0} 上没有标记 CompositeAttribute。", compositeResultType.FullName)
				);

			#endregion

			m_type = compositeResultType;

			CompositeAttribute compositeAttr = (CompositeAttribute)Attribute.GetCustomAttribute(compositeResultType, typeof(CompositeAttribute));

			PropertyInfo root = compositeResultType.GetProperty(compositeAttr.PropertyName, CommonPolicies.PropertyBindingFlags);
			m_root = new CompositeRootPropertyDefinition(this, root);

			m_constructor = compositeResultType.GetConstructor(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					(Binder)null,
					Type.EmptyTypes,
					(ParameterModifier[])null
				);

			#region 前置条件

			Debug.Assert(m_constructor != null, String.Format("类型 {0} 中不存在无参的构造函数。", compositeResultType.FullName));

			#endregion

			List<CompositeForeignReferencePropertyDefinition> foreignRefs = new List<CompositeForeignReferencePropertyDefinition>();

			foreach (PropertyInfo pf in compositeResultType.GetProperties(CommonPolicies.PropertyBindingFlags))
			{
				if (Attribute.IsDefined(pf, typeof(CompositeForeignReferenceAttribute)))
				{
					foreignRefs.Add(new CompositeForeignReferencePropertyDefinition(this, pf));
				}
			}

			m_foreignReferences = new CompositeForeignReferencePropertyDefinitionCollection(foreignRefs);
		}

		#endregion

		#region 属性

		/// <summary>
		/// 获取无参构造函数信息。
		/// </summary>
		public ConstructorInfo Constructor
		{
			get { return m_constructor; }
		}

		/// <summary>
		/// 获取外部引用属性集合。
		/// </summary>
		public CompositeForeignReferencePropertyDefinitionCollection ForeignReferences
		{
			get { return m_foreignReferences; }
		}

		/// <summary>
		/// 获取根属性。
		/// </summary>
		public CompositeRootPropertyDefinition Root
		{
			get { return m_root; }
		}

		/// <summary>
		/// 获取实体类型。
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion
	}
}