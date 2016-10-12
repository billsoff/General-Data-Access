#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupQueryListBuilder.cs
// �ļ���������������ʵ���ѯ�б���������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110722
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

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����ʵ���ѯ�б���������
	/// </summary>
	[Serializable]
	internal sealed class GroupQueryListBuilder : QueryListBuilder
	{
		#region ǰ������

		[NonSerialized]
		private GroupSchema m_schema;

		#endregion

		#region ���췽��

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		public GroupQueryListBuilder(IPropertyChain property, Filter where)
			: this(property, where, (Filter)null, false)
		{
		}

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		/// <param name="distinct">ָʾ�Ƿ���� DISTINC �ؼ��֡�</param>
		public GroupQueryListBuilder(IPropertyChain property, Filter where, Boolean distinct)
			: this(property, where, (Filter)null, distinct)
		{
		}

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		/// <param name="having">HAVING ��������</param>
		public GroupQueryListBuilder(IPropertyChain property, Filter where, Filter having)
			: this(property, where, having, false)
		{
		}

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		/// <param name="having">HAVING ��������</param>
		/// <param name="distinct">ָʾ�Ƿ���� DISTINC �ؼ��֡�</param>
		public GroupQueryListBuilder(IPropertyChain property, Filter where, Filter having, Boolean distinct)
			: base(property, where, having, distinct)
		{
		}

		#endregion

		/// <summary>
		/// ���ɻ����ܹ���
		/// </summary>
		protected override void BuildInfrastructure()
		{
			GroupSchemaBuilder schemaBuilder = new GroupSchemaBuilder(Property.Type);

			schemaBuilder.ExtendWhereFilter(Where);
			schemaBuilder.ExtendHavingFilter(Having);
			schemaBuilder.ExtendProperties(new IPropertyChain[] { Property }, false);

			m_schema = schemaBuilder.Build();
		}

		/// <summary>
		/// ���� WHERE ��������
		/// </summary>
		/// <returns>��������</returns>
		protected override FilterCompilationResult CompileWhereFilter()
		{
			return m_schema.Composite.ComposeFilterExpression(Where);
		}

		/// <summary>
		/// ���� HAVING ��������
		/// </summary>
		/// <returns>��������</returns>
		protected override FilterCompilationResult CompileHavingFilter()
		{
			return m_schema.ComposeFilterExpression(Having);
		}

		/// <summary>
		/// ���� SQL ָ�
		/// </summary>
		/// <returns>�����õ� SQL ָ�</returns>
		protected override String ComposeSqlStatement()
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			Column[] columns = m_schema[new ColumnLocator(Property.PropertyPath)];

			#region ǰ������

			Debug.Assert(columns.Length == 1, String.Format("���� {0} ӳ�����Ϊ�����л򲻴��ڡ�", Property.FullName));

			#endregion

			sqlBuilder.Select = columns[0].Expression;
			sqlBuilder.From = m_schema.FromList;
			sqlBuilder.Where = WhereClause;
			sqlBuilder.GroupBy = ComposeGroupList();
			sqlBuilder.Having = HavingClause;

			sqlBuilder.Distinct = Distinct;

			return sqlBuilder.Build();
		}

		#region ��������

		/// <summary>
		/// ���� GROUP BY �Ӿ䡣
		/// </summary>
		/// <returns>�����б�</returns>
		private String ComposeGroupList()
		{
			// ��ȡһ������ķ����б�
			Column[] conciseGroupColumns = GetConciseGroupColumns();

			// ���������ʽ
			StringBuilder output = new StringBuilder();

			for (Int32 i = 0; i < conciseGroupColumns.Length; i++)
			{
				if (i != 0)
				{
					output.Append(",");
				}

				output.Append(conciseGroupColumns[i].FullName);
			}

			return output.ToString();
		}

		/// <summary>
		/// ��ȡ����ķ����С�
		/// </summary>
		/// <returns>�����м��ϡ�</returns>
		private Column[] GetConciseGroupColumns()
		{
			List<Column> results = new List<Column>();

			foreach (GroupPropertyDefinition propertyDef in m_schema.Definition.Properties)
			{
				if (!propertyDef.IsGroupItem)
				{
					continue;
				}

				if (propertyDef.IsPrimitive)
				{
					results.AddRange(
							m_schema.Composite.Target[new ColumnLocator(propertyDef.PropertyChain.PropertyPath)]
						);
				}
				else
				{
					EntitySchema schema = m_schema.Composite[propertyDef.PropertyChain];

					if (schema.PrimaryKeyColumns.Count != 0)
					{
						results.AddRange(schema.PrimaryKeyColumns);
					}
					else
					{
						foreach (Column col in schema.Columns)
						{
							if (!col.Property.IsCustomAttributeDefined(typeof(NotSupportGroupingAttribute)))
							{
								results.Add(col);
							}
						}
					}
				}
			}

			results.AddRange(
					Array.FindAll<Column>(
							m_schema.GroupColumns,
							delegate(Column col)
							{
								if (results.Contains(col))
								{
									return false;
								}

								if (!col.Selected)
								{
									return true;
								}

								if (col.Property == null)
								{
									return true;
								}

								if (col.Property.PropertyChain.Equals(Property))
								{
									return true;
								}

								return false;
							}
						)
				);

			return results.ToArray();
		}

		#endregion
	}
}