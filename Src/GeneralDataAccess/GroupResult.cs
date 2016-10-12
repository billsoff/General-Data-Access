#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：GroupResult.cs
// 文件功能描述：用于做分组结果实体的基类。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110627
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
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于做分组结果实体的基类。
	/// </summary>
	[Serializable]
	public abstract class GroupResult : IDebugInfoProvider
	{
		#region 私有字段

		private readonly Dictionary<String, Object> m_values;
		private readonly GroupDefinition m_definition;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		protected GroupResult()
		{
			Type selfType = GetType();
			m_definition = GroupDefinitionBuilder.Build(selfType);
			m_values = new Dictionary<String, Object>(m_definition.Properties.Count);

			foreach (GroupPropertyDefinition propertyDef in m_definition.Properties)
			{
				m_values.Add(propertyDef.Name, null);
			}
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取要分组的实体类型。
		/// </summary>
		public Type Type
		{
			get { return m_definition.Entity.Type; }
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取或设置属性值。
		/// </summary>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>具有该名称的属性的值。</returns>
		internal Object this[String propertyName]
		{
			get
			{
				#region 前置条件

				Debug.Assert(propertyName != null, "参数 propertyName 不能为空（null）。");
				Debug.Assert(m_values.ContainsKey(propertyName), String.Format("属性名称“{0}”不存在，请检查聚合标记。", propertyName));

				#endregion

				return m_values[propertyName];
			}

			set
			{
				#region 前置条件

				Debug.Assert(propertyName != null, "参数 propertyName 不能为空（null）。");
				Debug.Assert(m_values.ContainsKey(propertyName), String.Format("属性名称“{0}”不存在，请检查聚合标记。", propertyName));

				#endregion

				m_values[propertyName] = value;
			}
		}

		/// <summary>
		/// 获取分组结果实体定义。
		/// </summary>
		internal GroupDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 打印分组结果的详细信息，用于调试。
		/// </summary>
		/// <returns></returns>
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

			foreach (GroupPropertyDefinition propertyDef in Definition.GetPrimitiveProperties())
			{
				output.AppendFormat("{0}: {1}{2}", propertyDef.Name, propertyDef.PropertyInfo.GetValue(this, null), Environment.NewLine);
			}

			foreach (GroupPropertyDefinition propertyDef in Definition.GetForeignReferenceProperties())
			{
				output.AppendFormat("{0}:", propertyDef.Name);

				Object value = propertyDef.PropertyInfo.GetValue(this, null);

				if (value == null)
				{
					output.AppendLine(" (Null)");
				}
				else
				{
					String info = DbEntityDebugger.Dump(value);
					info = Regex.Replace(info, "^", PADDING, RegexOptions.Multiline);

					output.AppendLine(info);
				}
			}

			output.AppendLine(line);

			return output.ToString();
		}

		#endregion

		#region 受保护的方法

		/// <summary>
		/// 获取属性值。
		/// </summary>
		/// <typeparam name="TValue">值类型。</typeparam>
		/// <param name="propertyName">属性名称。</param>
		/// <returns>属性值。</returns>
		protected TValue GetValue<TValue>(String propertyName)
		{
			#region 前置条件

			Debug.Assert(
					m_definition.Properties.Contains(propertyName),
					String.Format("分组结果类型 {0} 不包含属性 {1}。", m_definition.Type.FullName, propertyName)
				);

			Debug.Assert(
					m_definition.Properties[propertyName].Type == typeof(TValue),
					String.Format(
							"属性  {0}  值的类型 {1} 与请求的类型 {2} 不同。",
							propertyName,
							m_definition.Properties[propertyName].Type.Name,
							typeof(TValue).Name
						)
				);

			#endregion

			Object value = this[propertyName];

			if (value != null)
			{
				return (TValue)value;
			}
			else
			{
				return default(TValue);
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