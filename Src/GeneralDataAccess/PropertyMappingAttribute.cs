#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertyMappingAttribute.cs
// 文件功能描述：指示复合实体中根实体与表达式实体之间的映射关系。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110707
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
	/// 指示复合实体中根实体与表达式实体之间的映射关系。
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class PropertyMappingAttribute : Attribute
	{
		#region 私有字段

		private readonly String[] m_rootPropertyPath;
		private readonly String[] m_foreignReferencePropertyPath;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置根属性名称和外部引用属性名称。
		/// </summary>
		/// <param name="rootPropertyName">根属性名称。</param>
		/// <param name="foreignReferencePropertyName">外部引用属性名称。</param>
		public PropertyMappingAttribute(String rootPropertyName, String foreignReferencePropertyName)
			: this(new String[] { rootPropertyName }, new String[] { foreignReferencePropertyName })
		{
		}

		/// <summary>
		/// 构造函数，设置根属性名称和外部引用属性路径。
		/// </summary>
		/// <param name="rootPropertyName">根属性名称。</param>
		/// <param name="foreignReferencePropertyPath">外部引用属性路径。</param>
		public PropertyMappingAttribute(String rootPropertyName, String[] foreignReferencePropertyPath)
			: this(new String[] { rootPropertyName }, foreignReferencePropertyPath)
		{
		}

		/// <summary>
		/// 构造函数，设置根属性路径和外部引用属性名称。
		/// </summary>
		/// <param name="rootPropertyPath">根属性路径。</param>
		/// <param name="foreignReferencePropertyName">外部引用属性名称。</param>
		public PropertyMappingAttribute(String[] rootPropertyPath, String foreignReferencePropertyName)
			: this(rootPropertyPath, new String[] { foreignReferencePropertyName })
		{
		}

		/// <summary>
		/// 构造函数，设置根属性路径和外部引用属性路径。
		/// </summary>
		/// <param name="rootPropertyPath">根属性路径。</param>
		/// <param name="foriegnReferencePropertyPath">外部引用属性路径。</param>
		public PropertyMappingAttribute(String[] rootPropertyPath, String[] foriegnReferencePropertyPath)
		{
			m_rootPropertyPath = rootPropertyPath;
			m_foreignReferencePropertyPath = foriegnReferencePropertyPath;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取根属性路径。
		/// </summary>
		public String[] RootPropertyPath
		{
			get { return m_rootPropertyPath; }
		}

		/// <summary>
		/// 获取外部引用属性路径。
		/// </summary>
		public String[] ForeignReferencePropertyPath
		{
			get { return m_foreignReferencePropertyPath; }
		}

		#endregion
	}
}