#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DbEntityDebugger.cs
// 文件功能描述：用于调试。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110505
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


using System.Reflection;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于调试。
	/// </summary>
	public static class DbEntityDebugger
	{
		#region 私有常量

		private const String YES = "\u221A";
		private const String NOT = "x";
		private const String UNKNOWN = "-";

		#endregion

		#region 公共常量

		/// <summary>
		/// 默认缩进，为四个空格。
		/// </summary>
		public const String DEFAULT_INDENT = "    ";

		#endregion

		/// <summary>
		/// 打印实体的值。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <returns>实体的值。</returns>
		public static String Dump(Object entity)
		{
			return Dump(entity, DEFAULT_INDENT);
		}

		/// <summary>
		/// 打印实体的值。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="skips">要跳过的属性。</param>
		/// <returns>实体的值。</returns>
		internal static String Dump(Object entity, PropertyInfo[] skips)
		{
			return Dump(entity, DEFAULT_INDENT, skips);
		}

		/// <summary>
		/// 打印实体的值。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="indent">缩进。</param>
		/// <returns>实体的值。</returns>
		public static String Dump(Object entity, String indent)
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

			Debug.Assert((entity != null), "实体参数 entity 不能为空。");

			if (indent == null)
			{
				indent = String.Empty;
			}

			String line = String.Empty.PadRight(120, '-');

			StringBuilder output = new StringBuilder();

			output.AppendLine();
			output.AppendLine(line);

			output.Append(entity.GetType().FullName);

			String description = GetDescription(entity);

			if (description != null)
			{
				output.AppendFormat("（{0}）", description);
			}

			output.AppendLine();

			output.AppendLine(line);

			DoDump(entity, output, indent, 0);

			output.AppendLine(line);

			return output.ToString();
		}

		/// <summary>
		/// 打印实体的值。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="skips">要跳过的属性。</param>
		/// <returns>实体的值。</returns>
		internal static String Dump(Object entity, String indent, PropertyInfo[] skips)
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

			Debug.Assert((entity != null), "实体参数 entity 不能为空。");

			if (indent == null)
			{
				indent = String.Empty;
			}

			String line = String.Empty.PadRight(120, '-');

			StringBuilder output = new StringBuilder();

			output.AppendLine();
			output.AppendLine(line);

			output.Append(entity.GetType().FullName);

			String description = GetDescription(entity);

			if (description != null)
			{
				output.AppendFormat("（{0}）", description);
			}

			output.AppendLine();

			output.AppendLine(line);

			DoDump(entity, output, indent, 0, skips);

			output.AppendLine(line);

			return output.ToString();
		}

		/// <summary>
		/// 打印分组实体。
		/// </summary>
		/// <param name="result">分组实体。</param>
		/// <returns>分组实体的详细信息。</returns>
		public static String Dump(GroupResult result)
		{
			return result.Dump();
		}

		/// <summary>
		/// 打印复合实体。
		/// </summary>
		/// <param name="result">复合实体。</param>
		/// <returns>复合实体的详细信息。</returns>
		public static String Dump(CompositeResult result)
		{
			return result.Dump();
		}

		/// <summary>
		/// 生成实体组合的详细信息。
		/// </summary>
		/// <param name="composite">实体组合。</param>
		/// <returns>详细信息。</returns>
		public static String Dump(EntitySchemaComposite composite)
		{
			Debug.Assert((composite != null), "实体组合参数 composite 不能为空。");

			#region 格式

			/*
表名称:
表别名：
级别：
引用者：JR_UserBasicInfo (UserId) => (USERID)
引用到：
        表名                表别名                关系
        --------------------------------------------------------------
        JR_DistrictInfo     JR_DistrictInfo_1     (UserId) => (USERID)

表定义：

列        数据库类型        主键        允许空？        选择？        类型        属性        引用到        可展开引用？
-----------------------------------------------------------------------------------------------------------------------
*/

			#endregion

			// 实体组合名称
			String line = String.Empty.PadRight(120, '-');

			StringBuilder output = new StringBuilder();

			output.AppendLine();
			output.AppendLine(line);

			output.AppendFormat("实体架构组合：{0}", composite.Target.Type.FullName);

			output.AppendLine();

			output.AppendLine(line);

			// 实体组合属性
			GenerateCompositeDescription(output, composite);

			// 遍历实体集合
			for (Int32 level = 0; level < composite.Rank; level++)
			{
				output.AppendLine(line);

				output.AppendFormat("第 {0} 级实体", level.ToString());

				output.AppendLine();

				output.AppendLine(line);

				foreach (EntitySchema schema in composite[level])
				{
					GenerateSchemaDescription(output, schema);
				}
			}

			// 结束
			output.AppendLine(line);

			return output.ToString();
		}

		/// <summary>
		/// 获取分组架构的详细信息，用于调试。
		/// </summary>
		/// <param name="schema">分组架构。</param>
		/// <returns>分组架构的详细信息。</returns>
		public static String Dump(GroupSchema schema)
		{
			return schema.Dump();
		}

		/// <summary>
		/// 以十六进制方式显示字节数组，只取前 5 个元素。
		/// </summary>
		/// <param name="values">字节数组。</param>
		/// <returns>十六进制字符串。</returns>
		public static String Dump(Byte[] values)
		{
			const Int32 MAX_LENGTH = 5;

			return Dump(values, MAX_LENGTH, true);
		}

		/// <summary>
		/// 以十六进制方式显示字节数组，只取前 5 个元素。
		/// </summary>
		/// <param name="values">字节数组。</param>
		/// <param name="appendDigest">指示是否附加摘要信息。</param>
		/// <returns>十六进制字符串。</returns>
		public static String Dump(Byte[] values, Boolean appendDigest)
		{
			const Int32 MAX_LENGTH = 5;

			return Dump(values, MAX_LENGTH, appendDigest);
		}

		/// <summary>
		/// 以十六进制方式显示字节数组。
		/// </summary>
		/// <param name="values">字节数组。</param>
		/// <param name="truncatedLength">截断长度，如果为小于或等于零，则不截断。</param>
		/// <returns>十六进制字符串。</returns>
		public static String Dump(Byte[] values, Int32 truncatedLength)
		{
			return DumpBytes(values, truncatedLength, true);
		}

		/// <summary>
		/// 以十六进制方式显示字节数组。
		/// </summary>
		/// <param name="values">字节数组。</param>
		/// <param name="truncatedLength">截断长度，如果为小于或等于零，则不截断。</param>
		/// <param name="appendDigest">指示是否附加摘要信息。</param>
		/// <returns>十六进制字符串。</returns>
		public static String Dump(Byte[] values, Int32 truncatedLength, Boolean appendDigest)
		{
			return DumpBytes(values, truncatedLength, appendDigest);
		}

		/// <summary>
		/// 格式化 SQL 指令。
		/// </summary>
		/// <param name="sqlStatement">SQL 指令。</param>
		/// <returns>格式化好的指令。</returns>
		public static String FormatSqlStatement(String sqlStatement)
		{
			SqlExpressionFormatter formatter = SqlExpressionFormatter.Recognize(sqlStatement);

			return formatter.ToString();
		}

		/// <summary>
		/// 格式化 SQL 指令。
		/// </summary>
		/// <param name="sqlStatement">SQL 指令。</param>
		/// <param name="indent">缩进。</param>
		/// <returns>格式化好的 SQL 指令。</returns>
		public static String FormatSqlStatement(String sqlStatement, String indent)
		{
			SqlExpressionFormatter formatter = SqlExpressionFormatter.Recognize(sqlStatement, indent);

			return formatter.ToString();
		}

		/// <summary>
		/// 格式化 SQL 指令。
		/// </summary>
		/// <param name="sqlStatement">SQL 指令。</param>
		/// <param name="indent">缩进。</param>
		/// <param name="level">级别。</param>
		/// <returns>格式化好的 SQL 指令。</returns>
		public static String FormatSqlStatement(String sqlStatement, String indent, Int32 level)
		{
			SqlExpressionFormatter formatter = SqlExpressionFormatter.Recognize(sqlStatement, indent, level);

			return formatter.ToString();
		}

		/// <summary>
		/// 获取三态布尔值的字符串表示。
		/// </summary>
		/// <param name="state">状态。</param>
		/// <returns>状态字符串。</returns>
		public static String GetTristateBoolean(Boolean? state)
		{
			if (state == null)
			{
				return UNKNOWN;
			}
			else if (state.Value == true)
			{
				return YES;
			}
			else
			{
				return NOT;
			}
		}

		/// <summary>
		/// 整体缩进文本，缩进四个空格。
		/// </summary>
		/// <param name="text">要缩进的文本。</param>
		/// <returns>缩进后的文本。</returns>
		public static String IndentText(String text)
		{
			return IndentText(text, DEFAULT_INDENT, 1);
		}

		/// <summary>
		/// 整体缩进文本，指定缩进的级别，单位缩进为四个空格。
		/// </summary>
		/// <param name="text">要缩进的文本。</param>
		/// <param name="level">要缩进的级别。</param>
		/// <returns>缩进后的文本。</returns>
		public static String IndentText(String text, Int32 level)
		{
			return IndentText(text, DEFAULT_INDENT, level);
		}

		/// <summary>
		/// 整体缩进文本，指示要填充的串。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <param name="indent">要填充的串。</param>
		/// <returns>缩进后的文本。</returns>
		public static String IndentText(String text, String indent)
		{
			return IndentText(text, indent, 1);
		}

		/// <summary>
		/// 整体缩进文本，指示要填充的串和级别。
		/// </summary>
		/// <param name="text">文本。</param>
		/// <param name="indent">填充的串。</param>
		/// <param name="level">级别。</param>
		/// <returns>缩进后的文本。</returns>
		public static String IndentText(String text, String indent, Int32 level)
		{
			if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(indent) || (level == 0))
			{
				return text;
			}

			Regex head = new Regex("^", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
			String totalIndent = GetTotalIndent(indent, level);
			String result = head.Replace(text, totalIndent);

			return result;
		}

		#region 内部方法

		/// <summary>
		/// 获取整体缩进。
		/// </summary>
		/// <param name="indent">单位缩进。</param>
		/// <param name="level">级别。</param>
		/// <returns>整体缩进。</returns>
		internal static String GetTotalIndent(String indent, Int32 level)
		{
			if (String.IsNullOrEmpty(indent) || (level == 0))
			{
				return String.Empty;
			}

			Char[] indentChars = indent.ToCharArray();
			Char[] allIndentChars = new Char[level * indent.Length];

			for (Int32 i = 0; i < level; i++)
			{
				Array.Copy(indent.ToCharArray(), 0, allIndentChars, (i * indent.Length), indent.Length);
			}

			return new String(allIndentChars);
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 打印实体的值。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="output">输出缓冲区。</param>
		/// <param name="indent">单级缩进。</param>
		/// <param name="level">缩进级别。</param>
		private static void DoDump(Object entity, StringBuilder output, String indent, Int32 level)
		{
			DoDump(entity, output, indent, level, (PropertyInfo[])null);
		}

		/// <summary>
		/// 打印实体的值。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="output">输出缓冲区。</param>
		/// <param name="indent">单级缩进。</param>
		/// <param name="level">缩进级别。</param>
		/// <param name="skips">要跳过的属性。</param>
		private static void DoDump(Object entity, StringBuilder output, String indent, Int32 level, PropertyInfo[] skips)
		{
			using (EtyBusinessObject.SuppressEntityLazyLoadTransient(entity))
			{
				Type entityType = entity.GetType();
				DbEntityPropertyInfo info = DbEntityPropertyInfoCache.GetProperty(entityType);
				String leadings = GetLeadings(indent, level);

				foreach (PropertyInfo pf in info.Primitives)
				{
					DumpPrimitive(entity, pf, output, leadings);
				}

				foreach (PropertyInfo pf in info.References)
				{
					if (skips != null)
					{
						Boolean shouldSkipping = Array.Exists<PropertyInfo>(
								skips,
								delegate(PropertyInfo item)
								{
									return item.Name.Equals(pf.Name, StringComparison.Ordinal);
								}
							);

						if (shouldSkipping)
						{
							continue;
						}
					}

					output.AppendFormat("{0}{1}", leadings, pf.Name);

					Object parentEntity = pf.GetValue(entity, null);

					if (parentEntity == null)
					{
						output.AppendLine(": (Null)");
					}
					else
					{
						String description = GetDescription(parentEntity);

						if (description != null)
						{
							output.AppendLine(String.Format("（{0}）:", description));
						}
						else
						{
							output.AppendLine(":");
						}

						DoDump(parentEntity, output, indent, (level + 1));
					}
				}

				// 显示子实体集合
				EntityDefinition definition = EntityDefinitionBuilder.Build(entityType);

				foreach (EntityPropertyDefinition propertyDef in definition.Properties.GetAllChildrenProperties())
				{
					output.AppendFormat("{0}{1}：", leadings, propertyDef.Name);

					Object[] children = (Object[])propertyDef.PropertyInfo.GetValue(entity, null);

					if (children == null)
					{
						output.AppendLine("（未加载）");
					}
					else if (children.Length == 0)
					{
						output.AppendLine("（已加载，零个元素）");
					}
					else
					{
						output.AppendLine();

						foreach (Object childEntity in children)
						{
							// 要跳过父实体属性，否则会造成循环引用
							output.Append(
									IndentText(
											Dump(childEntity, new PropertyInfo[] { propertyDef.ChildrenProperty.PropertyInfo }),
											(leadings + indent)
										)
								);
						}
					}
				}
			}
		}

		/// <summary>
		/// 输出基本属性的值。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <param name="pf">属性信息。</param>
		/// <param name="output">输出缓冲。</param>
		/// <param name="leadings">缩进字符串。</param>
		private static void DumpPrimitive(Object entity, PropertyInfo pf, StringBuilder output, String leadings)
		{
			String valueStr;

			if (!EtyBusinessObject.IsEmpty(entity, pf.Name))
			{
				Object value = pf.GetValue(entity, null);

				if (value == null)
				{
					valueStr = "(Null)";
				}
				else if (value is Byte[])
				{
					valueStr = Dump((Byte[])value);
				}
				else if (DbConverter.IsDateTimeAndUninitialized(value))
				{
					valueStr = "(Empty)";
				}
				else
				{
					valueStr = value.ToString();
				}
			}
			else
			{
				valueStr = "(Empty)"; ;
			}

			output.AppendLine(String.Format("{0}{1}: {2}", leadings, pf.Name, valueStr));
		}

		/// <summary>
		/// 以十六进制方式显示字节数组，只取前 5 个元素。
		/// </summary>
		/// <param name="values">字节数组。</param>
		/// <returns>十六进制字符串。</returns>
		private static String DumpBytes(Byte[] values)
		{
			const Int32 MAX_LENGTH = 10;

			return DumpBytes(values, MAX_LENGTH);
		}

		/// <summary>
		/// 以十六进制方式显示字节数组。
		/// </summary>
		/// <param name="values">字节数组。</param>
		/// <param name="truncatedLength">截断长度，如果为小于或等于零，则不截断。</param>
		/// <returns>十六进制字符串。</returns>
		private static String DumpBytes(Byte[] values, Int32 truncatedLength)
		{
			return DumpBytes(values, truncatedLength, true);
		}

		/// <summary>
		/// 以十六进制方式显示字节数组。
		/// </summary>
		/// <param name="values">字节数组。</param>
		/// <param name="truncatedLength">截断长度，如果为小于或等于零，则不截断。</param>
		/// <param name="appendDigest">指示是否附加摘要信息。</param>
		/// <returns>十六进制字符串。</returns>
		private static String DumpBytes(Byte[] values, Int32 truncatedLength, Boolean appendDigest)
		{
			if (values == null)
			{
				return "(Null)";
			}

			if (values.Length == 0)
			{
				return "（零个元素）";
			}

			Int32 length;

			if (truncatedLength > 0)
			{
				length = Math.Min(truncatedLength, values.Length);
			}
			else
			{
				length = values.Length;
			}

			String result = "0x" + BitConverter.ToString(values, 0, length).Replace("-", String.Empty).ToUpper();

			if (appendDigest)
			{
				if (values.Length > length)
				{
					result += "...";
				}

				result += String.Format("({0:#,##0})", values.Length);
			}

			return result;
		}

		/// <summary>
		/// 获取实体状态描述。
		/// </summary>
		/// <param name="entity">实体。</param>
		/// <returns>描述文本。</returns>
		private static String GetDescription(Object entity)
		{
			EtyBusinessObject bo = entity as EtyBusinessObject;

			if (bo == null)
			{
				return null;
			}

			const String DELIMITER = " ";
			StringBuilder buffer = new StringBuilder();

			if (bo.Transient)
			{
				buffer.Append("游离");
			}
			else
			{
				buffer.Append("持久化");
			}

			if (bo.Deleted)
			{
				buffer.Append(DELIMITER);
				buffer.Append("已删除");
			}

			buffer.Append(DELIMITER);

			if (bo.Dirty)
			{
				buffer.Append("已修改");
			}
			else
			{
				buffer.Append("干净");
			}

			if (bo.IsPartially)
			{
				buffer.Append(DELIMITER);
				buffer.Append("仅部分加载");
			}

			return buffer.ToString();
		}

		/// <summary>
		/// 获取缩进字符串。
		/// </summary>
		/// <param name="indent">单级别缩进字符串。</param>
		/// <param name="level">级别。</param>
		/// <returns>总缩进字符串。</returns>
		private static String GetLeadings(String indent, Int32 level)
		{
			if (level == 0)
			{
				return String.Empty;
			}

			Char[] resultChars = new Char[indent.Length * level];
			Char[] indentChars = indent.ToCharArray();

			for (Int32 i = 0; i < level; i++)
			{
				Array.Copy(
						indentChars,
						0,
						resultChars,
						(indentChars.Length * i),
						indentChars.Length
					);
			}

			return new String(resultChars);
		}

		/// <summary>
		/// 生成实体架构的描述。
		/// </summary>
		/// <param name="output">输出缓冲。</param>
		/// <param name="schema">实体架构。</param>
		private static void GenerateSchemaDescription(StringBuilder output, EntitySchema schema)
		{
			#region 格式

			/*
表名称:
表别名：
级别：
引用者：JR_UserBasicInfo (UserId) => (USERID)
引用到：
        表名                表别名                关系
        --------------------------------------------------------------
        JR_DistrictInfo     JR_DistrictInfo_1     (UserId) => (USERID)
*/

			#endregion

			// 表格缩进
			String tabularIndent = String.Empty.PadRight(4);

			output.AppendLine(String.Format("实体: {0}", schema.Type.Name));
			output.AppendLine(String.Format("存储过程？{0}", GetTristateBoolean(schema.IsStoredProcedure)));
			output.AppendLine(String.Format("表名称: {0}", schema.TableName));
			output.AppendLine(String.Format("表别名: {0}", schema.TableAlias));
			output.AppendLine(String.Format("级别: {0}", schema.Level.ToString()));

			// 引用者
			if (schema.LeftRelation != null)
			{
				output.AppendLine(
						String.Format(
								"引用者: {0} {1}",
								schema.LeftRelation.ChildSchema.DisplayName,
								GetRelationDescription(schema.LeftRelation)
							)
					);
			}

			// 引用到
			if (schema.HasRightRelations)
			{
				output.AppendLine("引用到:");
				GenerateRelationsDescription(output, schema.RightRelations, tabularIndent);
			}

			// 表定义
			output.AppendLine("表定义:");
			GenerateTableDefinition(output, schema, tabularIndent);
		}

		/// <summary>
		/// 生成表定义。
		/// </summary>
		/// <param name="output">输出缓冲。</param>
		/// <param name="schema">实体架构。</param>
		/// <param name="tabularIndent">整体缩进。</param>
		private static void GenerateTableDefinition(StringBuilder output, EntitySchema schema, string tabularIndent)
		{
			#region 格式

			/*
列        数据库类型        主键?        允许空？        选择？        类型        属性        引用到        可展开引用？
-----------------------------------------------------------------------------------------------------------------------
*/

			#endregion

			String[] headers = new String[]
				{
					"列",
					"数据库类型",
					"主键？",
					"允许空？",
					"选择？",        
					"类型",
					"属性",
					"引用到",
					"可展开引用？",
					"延迟加载？"
				};

			TabularWriter writer = new TabularWriter(headers.Length, tabularIndent);

			writer.WriteLine(headers);

			const Int32 INDEX_COLUMN = 0;
			const Int32 INDEX_DB_TYPE = 1;
			const Int32 INDEX_PRIMARY_KEY = 2;
			const Int32 INDEX_PERMIT_NULL = 3;
			const Int32 INDEX_SELECTED = 4;
			const Int32 INDEX_TYPE = 5;
			const Int32 INDEX_PROPERTY = 6;
			const Int32 INDEX_REFER_TO = 7;
			const Int32 INDEX_EXPANDABLE = 8;
			const Int16 INDEX_LAZY_LOAD = 9;

			String NOT_APPLICABLE = GetTristateBoolean(null);

			foreach (Column col in schema.Columns)
			{
				String[] line = writer.NewLine();

				line[INDEX_COLUMN] = col.Name;
				line[INDEX_DB_TYPE] = col.DbType.ToString();
				line[INDEX_PRIMARY_KEY] = GetTristateBoolean(col.IsPrimaryKey);

				Boolean selected = (Array.IndexOf<Column>(schema.Composite.Columns, col) >= 0);

				line[INDEX_SELECTED] = GetTristateBoolean(selected);

				line[INDEX_TYPE] = col.Type.Name;
				line[INDEX_PROPERTY] = col.Property.Name;

				if (col.IsPrimitive)
				{
					line[INDEX_PERMIT_NULL] = NOT_APPLICABLE;

					line[INDEX_REFER_TO] = NOT_APPLICABLE;
					line[INDEX_EXPANDABLE] = NOT_APPLICABLE;
					line[INDEX_LAZY_LOAD] = GetTristateBoolean(col.LazyLoad);
				}
				else
				{
					line[INDEX_PERMIT_NULL] = GetTristateBoolean(col.Property.PermitNull);

					Column parentColumn = col.GetParentColumn();

					if (parentColumn != null)
					{
						line[INDEX_REFER_TO] = String.Format("{0}({1})", col.Property.Type.Name, parentColumn.Property.Schema.DisplayName);
					}
					else
					{
						line[INDEX_REFER_TO] = col.Property.Type.Name;
					}

					line[INDEX_EXPANDABLE] = GetTristateBoolean(!col.Property.SuppressExpand);
					line[INDEX_LAZY_LOAD] = NOT_APPLICABLE;
				}

				writer.WriteLine(line);
			}

			output.AppendLine(writer.ToString());
		}

		/// <summary>
		/// 生成关系列表的描述（表格形式）。
		/// </summary>
		/// <param name="output">输出缓冲。</param>
		/// <param name="allRelations">关系列表。</param>
		/// <param name="tabularIndent">表格的整体缩进。</param>
		private static void GenerateRelationsDescription(StringBuilder output, EntitySchemaRelation[] allRelations, String tabularIndent)
		{
			#region 格式

			/*
        表名                表别名                关系
        --------------------------------------------------------------
        JR_DistrictInfo     JR_DistrictInfo_1     (UserId) => (USERID)
*/

			#endregion

			const Int32 COLUMN_COUNT = 3;

			TabularWriter writer = new TabularWriter(COLUMN_COUNT, tabularIndent);

			// 标题
			writer.WriteLine(new String[] { "表名", "表别名", "关系" });

			foreach (EntitySchemaRelation relation in allRelations)
			{
				writer.WriteLine(
						new String[] 
							{
								relation.ParentSchema.TableName, 
								relation.ParentSchema.TableAlias, 
								GetRelationDescription(relation) 
							}
					);
			}

			output.AppendLine(writer.ToString());
		}

		/// <summary>
		/// 获取关系的描述。
		/// </summary>
		/// <param name="relation">实体关系。</param>
		/// <returns>关系描述。</returns>
		private static String GetRelationDescription(EntitySchemaRelation relation)
		{
			String[] childColumns = Array.ConvertAll<Column, String>(
					relation.ChildColumns,
					delegate(Column col) { return col.Name; }
				);

			String[] parentColumns = Array.ConvertAll<Column, String>(
					relation.ParentColumns,
					delegate(Column col) { return col.Name; }
				);

			return String.Format(
					"({0}) => ({1})",
					String.Join(", ", childColumns),
					String.Join(", ", parentColumns)
				);
		}

		/// <summary>
		/// 生成实体组合描述。
		/// </summary>
		/// <param name="output">输出缓冲。</param>
		/// <param name="composite">实体组合。</param>
		private static void GenerateCompositeDescription(StringBuilder output, EntitySchemaComposite composite)
		{
			output.AppendLine(String.Format("生成策略: {0}", composite.BuilderStrategy));
			output.AppendLine(String.Format("秩: {0}", composite.Rank.ToString()));
		}

		#endregion
	}
}