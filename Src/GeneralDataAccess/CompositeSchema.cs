#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeSchema.cs
// 文件功能描述：表示复合实体架构。
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
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 表示复合实体架构。
	/// </summary>
	public sealed class CompositeSchema : IDebugInfoProvider
	{
		#region 私有字段

		private readonly CompositeDefinition m_definition;
		private readonly Dictionary<String, ExpressionSchema> m_foreignReferences = new Dictionary<String, ExpressionSchema>();

		private String m_sqlExpression;
		private QueryParameter[] m_parameters;
		private EntitySchemaComposite m_root;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置复合实体定义。
		/// </summary>
		/// <param name="definition">复合实体定义。</param>
		internal CompositeSchema(CompositeDefinition definition)
		{
			m_definition = definition;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取 SQL 表达式。
		/// </summary>
		public String SqlExpression
		{
			get { return m_sqlExpression; }
			internal set { m_sqlExpression = value; }
		}

		/// <summary>
		/// 获取查询参数集合。
		/// </summary>
		public QueryParameter[] Parameters
		{
			get { return m_parameters; }
			internal set { m_parameters = value; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建复合实体。
		/// </summary>
		/// <param name="record">记录。</param>
		/// <returns>创建好的复合实体。</returns>
		public CompositeResult Compose(IDataRecord record)
		{
			CompositeResult result = (CompositeResult)m_definition.Constructor.Invoke((Object[])null);

			Compose(result, record);

			return result;
		}

		/// <summary>
		/// 创建复合实体。
		/// </summary>
		/// <param name="result">要填充值的复合实体。</param>
		/// <param name="record">记录。</param>
		public void Compose(CompositeResult result, IDataRecord record)
		{
			Object[] dbValues = new Object[record.FieldCount];
			record.GetValues(dbValues);

			result[m_definition.Root.Name] = Root.Target.Compose(dbValues);

			foreach (CompositePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				String name = propertyDef.Name;
				ExpressionSchema schema = m_foreignReferences[name];
				result[name] = schema.Compose(dbValues);
			}
		}

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取或设置根实体架构组合。
		/// </summary>
		internal EntitySchemaComposite Root
		{
			get { return m_root; }
			set { m_root = value; }
		}

		/// <summary>
		/// 获取复合实体中包含的表达式架构集合。
		/// </summary>
		internal Dictionary<String, ExpressionSchema> ForeignReferences
		{
			get { return m_foreignReferences; }
		}

		#endregion

		#region IDebugInfoProvider 成员

		/// <summary>
		/// 获取复合实体架构的详细信息，用于调试。
		/// </summary>
		/// <returns></returns>
		public String Dump()
		{
			// 实体组合名称
			String line = String.Empty.PadRight(120, '-');
			StringBuilder output = new StringBuilder();

			output.AppendLine();
			output.AppendLine(line);

			output.AppendFormat("复合实体架构组合：{0}", m_definition.Type.FullName);

			output.AppendLine();
			output.AppendLine(line);

			// 根实体架构
			output.AppendFormat("根实体架构：{0}\r\n{1}", Root.Dump(1), Root.BuilderStrategy.Dump(1));

			// 外部引用实体架构
			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				List<String> mappings = new List<String>();

				for (Int32 i = 0; i < propertyDef.RootPropertyChains.Length; i++)
				{
					mappings.Add(
							String.Format(
									"{0} -> {1}",
									propertyDef.RootPropertyChains[i].FullName,
									propertyDef.MappedPropertyChains[i].FullName
								)
						);
				}

				output.AppendLine();

				switch (propertyDef.JoinMode)
				{
					case PropertyJoinMode.Left:
						output.Append("LEFT JOIN ");
						break;

					case PropertyJoinMode.Right:
						output.Append("RIGHT JOIN ");
						break;

					case PropertyJoinMode.Inner:
						output.Append("INNER JOIN ");
						break;

					default:
						break;
				}

				output.AppendFormat("{1} ({2})", propertyDef.JoinMode.ToString(), propertyDef.Name, String.Join(" ", mappings.ToArray()));
				output.AppendLine();

				output.Append(ForeignReferences[propertyDef.Name].Dump(1));
			}

			// 结束
			output.AppendLine();
			output.AppendLine(line);

			return output.ToString();
		}

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