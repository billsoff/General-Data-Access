#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupSchema.cs
// �ļ�����������������ʵ��ܹ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110629
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ������ʵ��ܹ���
	/// </summary>
	public sealed class GroupSchema : IColumnLocatable, IDebugInfoProvider
	{
		#region ˽���ֶ�

		private GroupDefinition m_definition;

		private Column[] m_selectColumns;
		private Column[] m_groupColumns;

		private Column[] m_primitiveColumns;
		private GroupForeignReference[] m_foreignReferences;
		private EntitySchemaComposite m_composite;

		private String m_selectList;
		private String m_groupList;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		internal GroupSchema()
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// �����ж�λ������ƥ����м��ϡ�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>�ҵ����м��ϡ�</returns>
		public Column[] this[ColumnLocator colLocator]
		{
			get
			{
				// �ȶ�λ�ڷ�����ʵ������
				String propertyName = colLocator.PropertyPath[0];
				GroupPropertyDefinition propertyDef = m_definition.Properties[propertyName];

				#region ǰ������

				Debug.Assert(
						propertyDef != null,
						String.Format("���� {0} �в����ھۺ����� {1}����������ǡ�", m_definition.Type.FullName, propertyName)
					);

				#endregion

				if (propertyDef.IsPrimitive)
				{
					Column primitiveColumn = FindPrimitiveColumn(propertyDef.Name);

					if (primitiveColumn != null)
					{
						return new Column[] { primitiveColumn };
					}
				}
				else
				{
					EntitySchema schema = FindSchema(propertyDef.Name);

					if (schema != null)
					{
						if (colLocator.PropertyPath.Length == 1)
						{
							Column[] primaryKeyColumns = new Column[schema.PrimaryKeyColumns.Count];
							schema.PrimaryKeyColumns.CopyTo(primaryKeyColumns, 0);

							return primaryKeyColumns;
						}
						else
						{
							String[] childPropertyPath = new String[colLocator.PropertyPath.Length - 1];
							Array.Copy(colLocator.PropertyPath, 1, childPropertyPath, 0, childPropertyPath.Length);
							Column[] allColumns = schema[new ColumnLocator(childPropertyPath)];

							return allColumns;
						}
					}
				}

				Debug.Fail(String.Format("�ڷ���ʵ�� {0} �ж�λ���� {1} ʧ�ܡ�", m_definition.Type.FullName, colLocator.FullName));

				return null;
			}
		}

		/// <summary>
		/// ��ȡҪ�����ʵ��ܹ���ϡ�
		/// </summary>
		public EntitySchemaComposite Composite
		{
			get { return m_composite; }
			internal set { m_composite = value; }
		}

		/// <summary>
		/// ��ȡ�����ӱ��ʽ��
		/// </summary>
		public String FromList
		{
			get { return Composite.FromList; }
		}

		/// <summary>
		/// ��ȡ�����м��ϡ�
		/// </summary>
		public Column[] GroupColumns
		{
			get { return m_groupColumns; }
			internal set { m_groupColumns = value; }
		}

		/// <summary>
		/// ��ȡ�����б���������ڷ����б��򷵻� null��
		/// </summary>
		public String GroupList
		{
			get
			{
				if (m_groupList == null)
				{
					if ((m_groupColumns == null) || (m_groupColumns.Length == 0))
					{
						return null;
					}

					StringBuilder buffer = new StringBuilder();

					foreach (Column col in m_groupColumns)
					{
						if (buffer.Length != 0)
						{
							buffer.Append(",");
						}

						buffer.Append(col.FullName);
					}

					m_groupList = buffer.ToString();
				}

				return m_groupList;
			}
		}

		/// <summary>
		/// ��ȡ�����м��ϡ�
		/// </summary>
		public Column[] PrimitiveColumns
		{
			get { return m_primitiveColumns; }
			internal set { m_primitiveColumns = value; }
		}

		/// <summary>
		/// ��ȡѡ���м��ϡ�
		/// </summary>
		public Column[] SelectColumns
		{
			get { return m_selectColumns; }
			internal set { m_selectColumns = value; }
		}

		/// <summary>
		/// ��ȡѡ���б������� SELECT �ؼ��֡�
		/// </summary>
		public String SelectList
		{
			get
			{
				if (m_selectList == null)
				{
					#region ǰ������

					Debug.Assert((m_selectColumns != null) && (m_selectColumns.Length != 0), "δ����ѡ���С�");

					#endregion

					StringBuilder buffer = new StringBuilder();

					foreach (Column col in m_selectColumns)
					{
						if (buffer.Length != 0)
						{
							buffer.Append(",");
						}

						buffer.AppendFormat("{0} {1}", col.Expression, col.Alias);
					}

					m_selectList = buffer.ToString();
				}

				return m_selectList;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// �������� HAVING �Ӿ�Ĺ��������ʽ����ѯ������ǰ׺Ϊ��@����
		/// </summary>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>�����õ����� HAVING �Ӿ�Ĺ��������ʽ��</returns>
		public FilterCompilationResult ComposeFilterExpression(Filter havingFilter)
		{
			return ComposeFilterExpression(havingFilter, (String)null);
		}

		/// <summary>
		/// �������� HAVING �Ӿ�Ĺ��������ʽ�������ò�ѯ����ǰ׺��
		/// </summary>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="parameterPrifix">��ѯ����ǰ׺��</param>
		/// <returns>�����õ����� HAVING �Ӿ�Ĺ��������ʽ��</returns>
		public FilterCompilationResult ComposeFilterExpression(Filter havingFilter, String parameterPrifix)
		{
			GroupSchemaFilterCompilationContext context = new GroupSchemaFilterCompilationContext(this, parameterPrifix);
			FilterCompilationResult result = Filter.Compile(context, havingFilter);

			return result;
		}

		/// <summary>
		/// �ϳɷ�������
		/// </summary>
		/// <param name="record">��¼��</param>
		/// <returns>�ϳɺõķ�������</returns>
		public GroupResult Compose(IDataRecord record)
		{
			GroupResult result = (GroupResult)m_definition.Constructor.Invoke((Object[])null);

			Compose(result, record);

			return result;
		}

		/// <summary>
		/// ���÷�����������ֵ��
		/// </summary>
		/// <param name="result">������ʵ�塣</param>
		/// <param name="record">��¼��</param>
		public void Compose(GroupResult result, IDataRecord record)
		{
			#region ǰ������

			Debug.Assert(result != null, "���� result ����Ϊ�ա�");
			Debug.Assert(record != null, "���� record ����Ϊ�ա�");

			#endregion

			#region ���ܼ���

			Timing.Start("���������", "GroupSchema.Compose {0EC306B0-770C-43dc-B913-7A0D56CDB1FB}");

			#endregion

			Object[] dbValues = new Object[record.FieldCount];
			record.GetValues(dbValues);

			Compose(result, dbValues);

			#region ���ܼ���

			Timing.Stop("GroupSchema.Compose {0EC306B0-770C-43dc-B913-7A0D56CDB1FB}");

			#endregion
		}

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="dbValues">��¼��</param>
		/// <returns>�����õķ�������</returns>
		public Object Compose(Object[] dbValues)
		{
			GroupResult result = (GroupResult)m_definition.Constructor.Invoke((Object[])null);

			Compose(result, dbValues);

			return result;
		}

		/// <summary>
		/// �����ֶ�����ƫ�ơ�
		/// </summary>
		/// <param name="fieldIndexOffset">�ֶ�����ƫ�ơ�</param>
		public void SetFieldIndexOffset(Int32 fieldIndexOffset)
		{
			foreach (Column primitiveColumn in m_primitiveColumns)
			{
				primitiveColumn.FieldIndexOffset = fieldIndexOffset;
			}

			m_composite.SetFieldIndexOffset(fieldIndexOffset);
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ�����÷�����ʵ�嶨�塣
		/// </summary>
		internal GroupDefinition Definition
		{
			get { return m_definition; }
			set { m_definition = value; }
		}

		/// <summary>
		/// ��ȡ�����÷������ⲿ���ü��ϡ�
		/// </summary>
		internal GroupForeignReference[] ForeignReferences
		{
			get { return m_foreignReferences; }
			set { m_foreignReferences = value; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ���÷�����������ֵ��
		/// </summary>
		/// <param name="result">������ʵ�塣</param>
		/// <param name="dbValues">��¼��</param>
		private void Compose(GroupResult result, Object[] dbValues)
		{
			#region ǰ������

			Debug.Assert(result != null, "���� result ����Ϊ�ա�");
			Debug.Assert(dbValues != null, "���� dbValues ����Ϊ�ա�");

			#endregion

			foreach (Column col in m_primitiveColumns)
			{
				result[col.PropertyName] = DbConverter.ConvertFrom(dbValues[col.FieldIndex], col);
			}

			foreach (GroupForeignReference foreignRef in m_foreignReferences)
			{
				result[foreignRef.Property.Name] = foreignRef.Compose(dbValues);
			}
		}

		/// <summary>
		/// �������еľۺϡ�������еļ��ϣ���������е��о����˰�װ��������м����е��в�ͬ��
		/// </summary>
		private Column[] CreateAllColumns()
		{
			List<Column> allColumns = new List<Column>();
			allColumns.AddRange(PrimitiveColumns);
			Column[] primitiveRootColumns = Array.ConvertAll<Column, Column>(
					PrimitiveColumns,
					delegate(Column col)
					{
						return WrappedColumn.GetRootColumn(col);
					}
				);

			// �������г�ȥ�ڻ��������ѳ��ֵ���
			List<Column> others = new List<Column>();

			foreach (Column col in GroupColumns)
			{
				if (Array.IndexOf<Column>(primitiveRootColumns, col) == -1)
				{
					others.Add(col);
				}
			}

			allColumns.AddRange(
					others.ConvertAll<Column>(
						delegate(Column col)
						{
							return new GroupItemColumn(col);
						}
					)
				);

			return allColumns.ToArray();
		}

		/// <summary>
		/// ���Ҿ���ָ���������ƵĻ����У�ӳ���ڻ������ԣ���
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>��û�������ӳ����У����û���ҵ����򷵻� null��</returns>
		private Column FindPrimitiveColumn(String propertyName)
		{
			foreach (Column col in m_primitiveColumns)
			{
				if (col.PropertyName.Equals(propertyName, CommonPolicies.PropertyNameComparison))
				{
					return col;
				}
			}

			return null;
		}

		/// <summary>
		/// ���������ָ���������Ƶ��ⲿ����������ƥ���ʵ��ܹ���
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>���������ƥ���ʵ��ܹ������δ�ҵ����򷵻� null��</returns>
		private EntitySchema FindSchema(String propertyName)
		{
			foreach (GroupForeignReference foreignRef in m_foreignReferences)
			{
				if (foreignRef.Property.Name.Equals(propertyName, CommonPolicies.PropertyNameComparison))
				{
					return foreignRef.Schema;
				}
			}

			return null;
		}

		#endregion

		#region IDebugInfoProvider ��Ա

		/// <summary>
		/// ��ȡʵ������ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>��ϸ��Ϣ��</returns>
		public String Dump()
		{
			StringBuilder output = new StringBuilder();
			String line = String.Empty.PadRight(120, '-');

			output.AppendLine();
			output.AppendLine(line);

			output.AppendFormat("����ʵ��ܹ���ϣ�{0}", m_definition.Type.FullName);

			output.AppendLine();
			output.AppendLine(line);

			// ������������е���Ϣ�б�
			DumpAllColumns(output);

			// Ȼ�����Ŀ��ʵ��ܹ�����ϸ��Ϣ
			DumpComposite(output);

			// Ȼ�����Ŀ��ʵ������ɲ�����ϸ��Ϣ
			DumpBuilderStrategy(output);

			// ����
			output.AppendLine();
			output.AppendLine(line);

			return output.ToString();
		}

		#region ��������

		/// <summary>
		/// �򻺴�д�������е���ϸ��Ϣ��
		/// </summary>
		/// <param name="output">������档</param>
		private void DumpAllColumns(StringBuilder output)
		{
			const Int32 INDEX_NAME = 0;
			const Int32 INDEX_ALIAS = 1;
			const Int32 INDEX_TABLE = 2;
			const Int32 INDEX_AGGREGATION_NAME = 3;
			const Int32 INDEX_SELECT = 4;
			const Int32 INDEX_TYPE = 5;
			const Int32 INDEX_DB_TYPE = 6;

			output.AppendLine("�����У�");

			String[] caption = new String[] { "����", "����", "��", "��������", "ѡ��", "����", "���ݿ�����" };
			TabularWriter writer = new TabularWriter(caption.Length, DbEntityDebugger.DEFAULT_INDENT);

			writer.WriteLine(caption);

			foreach (Column col in CreateAllColumns())
			{
				String[] line = writer.NewLine();

				line[INDEX_NAME] = col.Name ?? AggregationAttribute.KEY_WORD_WILDCARD;
				line[INDEX_ALIAS] = col.Alias;
				line[INDEX_TABLE] = (col.Property != null)
					? String.Format("{0}({1})", col.Property.Schema.TableName, col.Property.Schema.TableAlias)
					: DbEntityDebugger.GetTristateBoolean((Boolean?)null);
				line[INDEX_AGGREGATION_NAME] = col.AggregationName;
				line[INDEX_SELECT] = DbEntityDebugger.GetTristateBoolean(col.Selected);
				line[INDEX_TYPE] = col.Type.Name;
				line[INDEX_DB_TYPE] = col.DbType.ToString();

				writer.WriteLine(line);
			}

			output.Append(writer.ToString());
		}

		/// <summary>
		/// �򻺴�д��Ŀ��ʵ��ܹ�����ϸ��Ϣ��
		/// </summary>
		/// <param name="output">������档</param>
		private void DumpComposite(StringBuilder output)
		{
			output.AppendLine("Ŀ��ʵ��ܹ���ϣ�");
			output.AppendLine(Composite.Dump(1));
		}

		/// <summary>
		/// �򻺴�д��Ŀ��ʵ��ܹ������ɲ��Ե���ϸ��Ϣ��
		/// </summary>
		/// <param name="output">������档</param>
		private void DumpBuilderStrategy(StringBuilder output)
		{
			output.AppendLine("Ŀ��ʵ��ܹ������ɲ��ԣ�");
			output.AppendLine(Composite.BuilderStrategy.Dump(1));
		}

		#endregion

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="indent"></param>
		/// <returns></returns>
		public String Dump(String indent)
		{
			return DbEntityDebugger.IndentText(Dump());
		}

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), level);
		}

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
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