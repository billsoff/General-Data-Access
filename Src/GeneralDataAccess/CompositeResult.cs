#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeResult.cs
// 文件功能描述：表示复合结果（即由一个实体架构组合与多个表达式实体架构（映射为子查询）相连接而成）。
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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示复合结果（即由一个实体架构组合与多个表达式实体架构（映射为子查询）相连接而成）。
	/// </summary>
	[Serializable]
	public abstract class CompositeResult : IDebugInfoProvider
	{
		#region 私有字段

		private readonly Dictionary<String, Object> m_values;
		private readonly CompositeDefinition m_definition;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		protected CompositeResult()
		{
			Type selfType = GetType();
			m_definition = CompositeDefinitionBuilder.Build(selfType);
			m_values = new Dictionary<String, Object>();

			m_values.Add(m_definition.Root.Name, null);

			foreach (CompositePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				m_values.Add(propertyDef.Name, null);
			}
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取或设置属性的值。
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		internal Object this[String propertyName]
		{
			get
			{
				#region 前置条件

				Debug.Assert(
						m_values.ContainsKey(propertyName),
						String.Format("复合实体 {0} 中不包含属性 {1}。", m_definition.Type.FullName, propertyName)
					);

				#endregion

				return m_values[propertyName];
			}

			set
			{
				#region 前置条件

				Debug.Assert(
						m_values.ContainsKey(propertyName),
						String.Format("复合实体 {0} 中不包含属性 {1}。", m_definition.Type.FullName, propertyName)
					);

				#endregion

				m_values[propertyName] = value;
			}
		}

		/// <summary>
		/// 获取复合实体定义。
		/// </summary>
		internal CompositeDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 获取复合实体的详细信息，用于调试。
		/// </summary>
		/// <returns>复合实体的详细信息。</returns>
		public String Dump()
		{
			#region 格式

			/*
-----------------------------------------------------------------------------------------------------
GZPSS.Entity.MACL.EtyUser
-----------------------------------------------------------------------------------------------------
Id: xxx
UserName: xxx
UserAddress:
    Id: 23412325-afed-
    ParentAddress(EtyNewAddress): (Null)
-----------------------------------------------------------------------------------------------------
			 */

			#endregion

			const String PADDING = "    ";
			String line = String.Empty.PadRight(120, '-');

			StringBuilder output = new StringBuilder();

			output.AppendLine();
			output.AppendLine(line);

			output.AppendLine(GetType().FullName);

			output.AppendLine(line);

			// Root
			DumpProperty(m_definition.Root, output, PADDING);

			// 外部引用
			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				DumpProperty(propertyDef, output, PADDING);
			}

			output.AppendLine(line);

			return output.ToString();
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 获取属性值。
		/// </summary>
		/// <typeparam name="TValue">属性值类型。</typeparam>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>属性值。</returns>
		protected TValue GetValue<TValue>(String propertyName) where TValue : class
		{
			return (TValue)this[propertyName];
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 输入属性的详细信息。
		/// </summary>
		/// <param name="propertyDef">属性定义。</param>
		/// <param name="output">输出缓存。</param>
		/// <param name="indent">缩进。</param>
		private void DumpProperty(CompositePropertyDefinition propertyDef, StringBuilder output, String indent)
		{
			output.AppendFormat("{0}:", propertyDef.Name);

			Object value = propertyDef.PropertyInfo.GetValue(this, null);

			if (value == null)
			{
				output.AppendLine(" (Null)");
			}
			else
			{
				ExpressionSchemaType schemaType = ExpressionSchemaBuilderFactory.GetExpressionSchemaType(propertyDef.Type);

				String info;

				switch (schemaType)
				{
					case ExpressionSchemaType.Entity:
						info = DbEntityDebugger.Dump(value);
						break;

					case ExpressionSchemaType.Group:
						info = ((GroupResult)value).Dump();
						break;

					case ExpressionSchemaType.Unknown:
					default:
						info = value.ToString();
						break;
				}

				info = Regex.Replace(info, "^", indent, RegexOptions.Multiline);

				output.AppendLine(info);
			}
		}

		#endregion

		#region IDebugInfoProvider 成员

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="indent"></param>
		/// <returns></returns>
		public String Dump(String indent)
		{
			return DbEntityDebugger.IndentText(Dump(), indent);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), level);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="indent"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(String indent, Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), indent, level);
		}

		#endregion
	}
}