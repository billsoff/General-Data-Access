#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����DbEntityDebugger.cs
// �ļ��������������ڵ��ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110505
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;


using System.Reflection;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ڵ��ԡ�
	/// </summary>
	public static class DbEntityDebugger
	{
		#region ˽�г���

		private const String YES = "\u221A";
		private const String NOT = "x";
		private const String UNKNOWN = "-";

		#endregion

		#region ��������

		/// <summary>
		/// Ĭ��������Ϊ�ĸ��ո�
		/// </summary>
		public const String DEFAULT_INDENT = "    ";

		#endregion

		/// <summary>
		/// ��ӡʵ���ֵ��
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <returns>ʵ���ֵ��</returns>
		public static String Dump(Object entity)
		{
			return Dump(entity, DEFAULT_INDENT);
		}

		/// <summary>
		/// ��ӡʵ���ֵ��
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="skips">Ҫ���������ԡ�</param>
		/// <returns>ʵ���ֵ��</returns>
		internal static String Dump(Object entity, PropertyInfo[] skips)
		{
			return Dump(entity, DEFAULT_INDENT, skips);
		}

		/// <summary>
		/// ��ӡʵ���ֵ��
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="indent">������</param>
		/// <returns>ʵ���ֵ��</returns>
		public static String Dump(Object entity, String indent)
		{
			#region ��ʽ

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

			Debug.Assert((entity != null), "ʵ����� entity ����Ϊ�ա�");

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
				output.AppendFormat("��{0}��", description);
			}

			output.AppendLine();

			output.AppendLine(line);

			DoDump(entity, output, indent, 0);

			output.AppendLine(line);

			return output.ToString();
		}

		/// <summary>
		/// ��ӡʵ���ֵ��
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="indent">������</param>
		/// <param name="skips">Ҫ���������ԡ�</param>
		/// <returns>ʵ���ֵ��</returns>
		internal static String Dump(Object entity, String indent, PropertyInfo[] skips)
		{
			#region ��ʽ

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

			Debug.Assert((entity != null), "ʵ����� entity ����Ϊ�ա�");

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
				output.AppendFormat("��{0}��", description);
			}

			output.AppendLine();

			output.AppendLine(line);

			DoDump(entity, output, indent, 0, skips);

			output.AppendLine(line);

			return output.ToString();
		}

		/// <summary>
		/// ��ӡ����ʵ�塣
		/// </summary>
		/// <param name="result">����ʵ�塣</param>
		/// <returns>����ʵ�����ϸ��Ϣ��</returns>
		public static String Dump(GroupResult result)
		{
			return result.Dump();
		}

		/// <summary>
		/// ��ӡ����ʵ�塣
		/// </summary>
		/// <param name="result">����ʵ�塣</param>
		/// <returns>����ʵ�����ϸ��Ϣ��</returns>
		public static String Dump(CompositeResult result)
		{
			return result.Dump();
		}

		/// <summary>
		/// ����ʵ����ϵ���ϸ��Ϣ��
		/// </summary>
		/// <param name="composite">ʵ����ϡ�</param>
		/// <returns>��ϸ��Ϣ��</returns>
		public static String Dump(EntitySchemaComposite composite)
		{
			Debug.Assert((composite != null), "ʵ����ϲ��� composite ����Ϊ�ա�");

			#region ��ʽ

			/*
������:
�������
����
�����ߣ�JR_UserBasicInfo (UserId) => (USERID)
���õ���
        ����                �����                ��ϵ
        --------------------------------------------------------------
        JR_DistrictInfo     JR_DistrictInfo_1     (UserId) => (USERID)

���壺

��        ���ݿ�����        ����        ����գ�        ѡ��        ����        ����        ���õ�        ��չ�����ã�
-----------------------------------------------------------------------------------------------------------------------
*/

			#endregion

			// ʵ���������
			String line = String.Empty.PadRight(120, '-');

			StringBuilder output = new StringBuilder();

			output.AppendLine();
			output.AppendLine(line);

			output.AppendFormat("ʵ��ܹ���ϣ�{0}", composite.Target.Type.FullName);

			output.AppendLine();

			output.AppendLine(line);

			// ʵ���������
			GenerateCompositeDescription(output, composite);

			// ����ʵ�弯��
			for (Int32 level = 0; level < composite.Rank; level++)
			{
				output.AppendLine(line);

				output.AppendFormat("�� {0} ��ʵ��", level.ToString());

				output.AppendLine();

				output.AppendLine(line);

				foreach (EntitySchema schema in composite[level])
				{
					GenerateSchemaDescription(output, schema);
				}
			}

			// ����
			output.AppendLine(line);

			return output.ToString();
		}

		/// <summary>
		/// ��ȡ����ܹ�����ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <param name="schema">����ܹ���</param>
		/// <returns>����ܹ�����ϸ��Ϣ��</returns>
		public static String Dump(GroupSchema schema)
		{
			return schema.Dump();
		}

		/// <summary>
		/// ��ʮ�����Ʒ�ʽ��ʾ�ֽ����飬ֻȡǰ 5 ��Ԫ�ء�
		/// </summary>
		/// <param name="values">�ֽ����顣</param>
		/// <returns>ʮ�������ַ�����</returns>
		public static String Dump(Byte[] values)
		{
			const Int32 MAX_LENGTH = 5;

			return Dump(values, MAX_LENGTH, true);
		}

		/// <summary>
		/// ��ʮ�����Ʒ�ʽ��ʾ�ֽ����飬ֻȡǰ 5 ��Ԫ�ء�
		/// </summary>
		/// <param name="values">�ֽ����顣</param>
		/// <param name="appendDigest">ָʾ�Ƿ񸽼�ժҪ��Ϣ��</param>
		/// <returns>ʮ�������ַ�����</returns>
		public static String Dump(Byte[] values, Boolean appendDigest)
		{
			const Int32 MAX_LENGTH = 5;

			return Dump(values, MAX_LENGTH, appendDigest);
		}

		/// <summary>
		/// ��ʮ�����Ʒ�ʽ��ʾ�ֽ����顣
		/// </summary>
		/// <param name="values">�ֽ����顣</param>
		/// <param name="truncatedLength">�ضϳ��ȣ����ΪС�ڻ�����㣬�򲻽ضϡ�</param>
		/// <returns>ʮ�������ַ�����</returns>
		public static String Dump(Byte[] values, Int32 truncatedLength)
		{
			return DumpBytes(values, truncatedLength, true);
		}

		/// <summary>
		/// ��ʮ�����Ʒ�ʽ��ʾ�ֽ����顣
		/// </summary>
		/// <param name="values">�ֽ����顣</param>
		/// <param name="truncatedLength">�ضϳ��ȣ����ΪС�ڻ�����㣬�򲻽ضϡ�</param>
		/// <param name="appendDigest">ָʾ�Ƿ񸽼�ժҪ��Ϣ��</param>
		/// <returns>ʮ�������ַ�����</returns>
		public static String Dump(Byte[] values, Int32 truncatedLength, Boolean appendDigest)
		{
			return DumpBytes(values, truncatedLength, appendDigest);
		}

		/// <summary>
		/// ��ʽ�� SQL ָ�
		/// </summary>
		/// <param name="sqlStatement">SQL ָ�</param>
		/// <returns>��ʽ���õ�ָ�</returns>
		public static String FormatSqlStatement(String sqlStatement)
		{
			SqlExpressionFormatter formatter = SqlExpressionFormatter.Recognize(sqlStatement);

			return formatter.ToString();
		}

		/// <summary>
		/// ��ʽ�� SQL ָ�
		/// </summary>
		/// <param name="sqlStatement">SQL ָ�</param>
		/// <param name="indent">������</param>
		/// <returns>��ʽ���õ� SQL ָ�</returns>
		public static String FormatSqlStatement(String sqlStatement, String indent)
		{
			SqlExpressionFormatter formatter = SqlExpressionFormatter.Recognize(sqlStatement, indent);

			return formatter.ToString();
		}

		/// <summary>
		/// ��ʽ�� SQL ָ�
		/// </summary>
		/// <param name="sqlStatement">SQL ָ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		/// <returns>��ʽ���õ� SQL ָ�</returns>
		public static String FormatSqlStatement(String sqlStatement, String indent, Int32 level)
		{
			SqlExpressionFormatter formatter = SqlExpressionFormatter.Recognize(sqlStatement, indent, level);

			return formatter.ToString();
		}

		/// <summary>
		/// ��ȡ��̬����ֵ���ַ�����ʾ��
		/// </summary>
		/// <param name="state">״̬��</param>
		/// <returns>״̬�ַ�����</returns>
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
		/// ���������ı��������ĸ��ո�
		/// </summary>
		/// <param name="text">Ҫ�������ı���</param>
		/// <returns>��������ı���</returns>
		public static String IndentText(String text)
		{
			return IndentText(text, DEFAULT_INDENT, 1);
		}

		/// <summary>
		/// ���������ı���ָ�������ļ��𣬵�λ����Ϊ�ĸ��ո�
		/// </summary>
		/// <param name="text">Ҫ�������ı���</param>
		/// <param name="level">Ҫ�����ļ���</param>
		/// <returns>��������ı���</returns>
		public static String IndentText(String text, Int32 level)
		{
			return IndentText(text, DEFAULT_INDENT, level);
		}

		/// <summary>
		/// ���������ı���ָʾҪ���Ĵ���
		/// </summary>
		/// <param name="text">�ı���</param>
		/// <param name="indent">Ҫ���Ĵ���</param>
		/// <returns>��������ı���</returns>
		public static String IndentText(String text, String indent)
		{
			return IndentText(text, indent, 1);
		}

		/// <summary>
		/// ���������ı���ָʾҪ���Ĵ��ͼ���
		/// </summary>
		/// <param name="text">�ı���</param>
		/// <param name="indent">���Ĵ���</param>
		/// <param name="level">����</param>
		/// <returns>��������ı���</returns>
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

		#region �ڲ�����

		/// <summary>
		/// ��ȡ����������
		/// </summary>
		/// <param name="indent">��λ������</param>
		/// <param name="level">����</param>
		/// <returns>����������</returns>
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

		#region ��������

		/// <summary>
		/// ��ӡʵ���ֵ��
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="output">�����������</param>
		/// <param name="indent">����������</param>
		/// <param name="level">��������</param>
		private static void DoDump(Object entity, StringBuilder output, String indent, Int32 level)
		{
			DoDump(entity, output, indent, level, (PropertyInfo[])null);
		}

		/// <summary>
		/// ��ӡʵ���ֵ��
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="output">�����������</param>
		/// <param name="indent">����������</param>
		/// <param name="level">��������</param>
		/// <param name="skips">Ҫ���������ԡ�</param>
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
							output.AppendLine(String.Format("��{0}��:", description));
						}
						else
						{
							output.AppendLine(":");
						}

						DoDump(parentEntity, output, indent, (level + 1));
					}
				}

				// ��ʾ��ʵ�弯��
				EntityDefinition definition = EntityDefinitionBuilder.Build(entityType);

				foreach (EntityPropertyDefinition propertyDef in definition.Properties.GetAllChildrenProperties())
				{
					output.AppendFormat("{0}{1}��", leadings, propertyDef.Name);

					Object[] children = (Object[])propertyDef.PropertyInfo.GetValue(entity, null);

					if (children == null)
					{
						output.AppendLine("��δ���أ�");
					}
					else if (children.Length == 0)
					{
						output.AppendLine("���Ѽ��أ����Ԫ�أ�");
					}
					else
					{
						output.AppendLine();

						foreach (Object childEntity in children)
						{
							// Ҫ������ʵ�����ԣ���������ѭ������
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
		/// ����������Ե�ֵ��
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <param name="pf">������Ϣ��</param>
		/// <param name="output">������塣</param>
		/// <param name="leadings">�����ַ�����</param>
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
		/// ��ʮ�����Ʒ�ʽ��ʾ�ֽ����飬ֻȡǰ 5 ��Ԫ�ء�
		/// </summary>
		/// <param name="values">�ֽ����顣</param>
		/// <returns>ʮ�������ַ�����</returns>
		private static String DumpBytes(Byte[] values)
		{
			const Int32 MAX_LENGTH = 10;

			return DumpBytes(values, MAX_LENGTH);
		}

		/// <summary>
		/// ��ʮ�����Ʒ�ʽ��ʾ�ֽ����顣
		/// </summary>
		/// <param name="values">�ֽ����顣</param>
		/// <param name="truncatedLength">�ضϳ��ȣ����ΪС�ڻ�����㣬�򲻽ضϡ�</param>
		/// <returns>ʮ�������ַ�����</returns>
		private static String DumpBytes(Byte[] values, Int32 truncatedLength)
		{
			return DumpBytes(values, truncatedLength, true);
		}

		/// <summary>
		/// ��ʮ�����Ʒ�ʽ��ʾ�ֽ����顣
		/// </summary>
		/// <param name="values">�ֽ����顣</param>
		/// <param name="truncatedLength">�ضϳ��ȣ����ΪС�ڻ�����㣬�򲻽ضϡ�</param>
		/// <param name="appendDigest">ָʾ�Ƿ񸽼�ժҪ��Ϣ��</param>
		/// <returns>ʮ�������ַ�����</returns>
		private static String DumpBytes(Byte[] values, Int32 truncatedLength, Boolean appendDigest)
		{
			if (values == null)
			{
				return "(Null)";
			}

			if (values.Length == 0)
			{
				return "�����Ԫ�أ�";
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
		/// ��ȡʵ��״̬������
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		/// <returns>�����ı���</returns>
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
				buffer.Append("����");
			}
			else
			{
				buffer.Append("�־û�");
			}

			if (bo.Deleted)
			{
				buffer.Append(DELIMITER);
				buffer.Append("��ɾ��");
			}

			buffer.Append(DELIMITER);

			if (bo.Dirty)
			{
				buffer.Append("���޸�");
			}
			else
			{
				buffer.Append("�ɾ�");
			}

			if (bo.IsPartially)
			{
				buffer.Append(DELIMITER);
				buffer.Append("�����ּ���");
			}

			return buffer.ToString();
		}

		/// <summary>
		/// ��ȡ�����ַ�����
		/// </summary>
		/// <param name="indent">�����������ַ�����</param>
		/// <param name="level">����</param>
		/// <returns>�������ַ�����</returns>
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
		/// ����ʵ��ܹ���������
		/// </summary>
		/// <param name="output">������塣</param>
		/// <param name="schema">ʵ��ܹ���</param>
		private static void GenerateSchemaDescription(StringBuilder output, EntitySchema schema)
		{
			#region ��ʽ

			/*
������:
�������
����
�����ߣ�JR_UserBasicInfo (UserId) => (USERID)
���õ���
        ����                �����                ��ϵ
        --------------------------------------------------------------
        JR_DistrictInfo     JR_DistrictInfo_1     (UserId) => (USERID)
*/

			#endregion

			// �������
			String tabularIndent = String.Empty.PadRight(4);

			output.AppendLine(String.Format("ʵ��: {0}", schema.Type.Name));
			output.AppendLine(String.Format("�洢���̣�{0}", GetTristateBoolean(schema.IsStoredProcedure)));
			output.AppendLine(String.Format("������: {0}", schema.TableName));
			output.AppendLine(String.Format("�����: {0}", schema.TableAlias));
			output.AppendLine(String.Format("����: {0}", schema.Level.ToString()));

			// ������
			if (schema.LeftRelation != null)
			{
				output.AppendLine(
						String.Format(
								"������: {0} {1}",
								schema.LeftRelation.ChildSchema.DisplayName,
								GetRelationDescription(schema.LeftRelation)
							)
					);
			}

			// ���õ�
			if (schema.HasRightRelations)
			{
				output.AppendLine("���õ�:");
				GenerateRelationsDescription(output, schema.RightRelations, tabularIndent);
			}

			// ����
			output.AppendLine("����:");
			GenerateTableDefinition(output, schema, tabularIndent);
		}

		/// <summary>
		/// ���ɱ��塣
		/// </summary>
		/// <param name="output">������塣</param>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <param name="tabularIndent">����������</param>
		private static void GenerateTableDefinition(StringBuilder output, EntitySchema schema, string tabularIndent)
		{
			#region ��ʽ

			/*
��        ���ݿ�����        ����?        ����գ�        ѡ��        ����        ����        ���õ�        ��չ�����ã�
-----------------------------------------------------------------------------------------------------------------------
*/

			#endregion

			String[] headers = new String[]
				{
					"��",
					"���ݿ�����",
					"������",
					"����գ�",
					"ѡ��",        
					"����",
					"����",
					"���õ�",
					"��չ�����ã�",
					"�ӳټ��أ�"
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
		/// ���ɹ�ϵ�б�������������ʽ����
		/// </summary>
		/// <param name="output">������塣</param>
		/// <param name="allRelations">��ϵ�б�</param>
		/// <param name="tabularIndent">��������������</param>
		private static void GenerateRelationsDescription(StringBuilder output, EntitySchemaRelation[] allRelations, String tabularIndent)
		{
			#region ��ʽ

			/*
        ����                �����                ��ϵ
        --------------------------------------------------------------
        JR_DistrictInfo     JR_DistrictInfo_1     (UserId) => (USERID)
*/

			#endregion

			const Int32 COLUMN_COUNT = 3;

			TabularWriter writer = new TabularWriter(COLUMN_COUNT, tabularIndent);

			// ����
			writer.WriteLine(new String[] { "����", "�����", "��ϵ" });

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
		/// ��ȡ��ϵ��������
		/// </summary>
		/// <param name="relation">ʵ���ϵ��</param>
		/// <returns>��ϵ������</returns>
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
		/// ����ʵ�����������
		/// </summary>
		/// <param name="output">������塣</param>
		/// <param name="composite">ʵ����ϡ�</param>
		private static void GenerateCompositeDescription(StringBuilder output, EntitySchemaComposite composite)
		{
			output.AppendLine(String.Format("���ɲ���: {0}", composite.BuilderStrategy));
			output.AppendLine(String.Format("��: {0}", composite.Rank.ToString()));
		}

		#endregion
	}
}