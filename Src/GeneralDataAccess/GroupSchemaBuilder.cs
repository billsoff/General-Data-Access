#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupSchemaBuilder.cs
// �ļ�����������������ʵ��ܹ����������������������ķ�����ʵ��ܹ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110630
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
	/// ������ʵ��ܹ����������������������ķ�����ʵ��ܹ���
	/// </summary>
	public sealed class GroupSchemaBuilder
	{
		#region ˽���ֶ�

		private readonly GroupDefinition m_definition;

		private readonly List<Column> m_selectColumns;
		private readonly List<Column> m_groupColumns;

		private readonly List<Column> m_primitiveColumns;
		private readonly List<GroupForeignReference> m_foreignReferences;

		private readonly List<IPropertyChain> m_schemas;
		private readonly List<IPropertyChain> m_properties;
		private readonly List<IPropertyChain> m_groupChains;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		/// <param name="groupResultType">������ʵ�����͡�</param>
		public GroupSchemaBuilder(Type groupResultType)
		{
			#region ǰ������

			Debug.Assert(groupResultType != null, "������ʵ�����Ͳ��� groupResultType ����Ϊ�ա�");
			Debug.Assert(
					typeof(GroupResult).IsAssignableFrom(groupResultType),
					String.Format("������ʵ������ {0} û�д����� GroupResult ������", groupResultType.FullName)
				);
			Debug.Assert(
					Attribute.IsDefined(groupResultType, typeof(GroupAttribute)),
					String.Format("������ʵ������ {0} ��û�б�� GroupAttribute��", groupResultType.FullName)
				);

			#endregion

			m_definition = GroupDefinitionBuilder.Build(groupResultType);

			m_selectColumns = new List<Column>();
			m_groupColumns = new List<Column>();

			m_primitiveColumns = new List<Column>();
			m_foreignReferences = new List<GroupForeignReference>();

			m_schemas = new List<IPropertyChain>();
			m_properties = new List<IPropertyChain>();
			m_groupChains = new List<IPropertyChain>();
		}

		#endregion

		#region ��������

		#region �������

		/// <summary>
		/// ʹ������ HAVING �Ӿ�Ĺ�������չ�ܹ���Ϣ��
		/// </summary>
		/// <param name="havingFilter">���� HAVING �Ӿ�Ĺ�������</param>
		public void ExtendHavingFilter(Filter havingFilter)
		{
			#region ���ܼ���

			Timing.Start("��չ HAVING ������", "GroupSchemaBuilder.ExtendHavingFilter {0CF8B5CA-30CA-479a-AEC8-BC1F65C3BA00}");

			#endregion

			if (havingFilter != null)
			{
				ColumnLocator[] allColumnLocators = havingFilter.GetAllColumnLocators();
				ExtendGroupColumnLocators(allColumnLocators);
			}

			#region ���ܼ���

			Timing.Stop("GroupSchemaBuilder.ExtendHavingFilter {0CF8B5CA-30CA-479a-AEC8-BC1F65C3BA00}");

			#endregion
		}

		/// <summary>
		/// ʹ������ WHERE �Ӿ�Ĺ�������չ�ܹ���Ϣ��
		/// </summary>
		/// <param name="whereFilter">���� WHERE �Ӿ�Ĺ�������</param>
		public void ExtendWhereFilter(Filter whereFilter)
		{
			#region ���ܼ���

			Timing.Start("��չ WHERE ������", "GroupSchemaBuilder.ExtendWhereFilter {BB499A8B-A946-4a8f-A10F-5B35A78E7A8F}");

			#endregion

			if (whereFilter == null)
			{
				#region ���ܼ���

				Timing.Stop("GroupSchemaBuilder.ExtendWhereFilter {BB499A8B-A946-4a8f-A10F-5B35A78E7A8F}");

				#endregion

				return;
			}

			ColumnLocator[] allColumnLocators = whereFilter.GetAllColumnLocators();

			foreach (ColumnLocator colLocator in allColumnLocators)
			{
				if (colLocator.PropertyPath.Length > 1)
				{
					String[] parentPropertyPath = new String[colLocator.PropertyPath.Length - 1];
					Array.Copy(colLocator.PropertyPath, 0, parentPropertyPath, 0, parentPropertyPath.Length);

					IPropertyChain reference = new PropertyChain(m_definition.Entity.Type, parentPropertyPath);

					if (!m_schemas.Contains(reference))
					{
						m_schemas.Add(reference);
					}
				}
			}

			#region ���ܼ���

			Timing.Stop("GroupSchemaBuilder.ExtendWhereFilter {BB499A8B-A946-4a8f-A10F-5B35A78E7A8F}");

			#endregion
		}

		/// <summary>
		/// ʹ����������չ�ܹ���Ϣ��
		/// </summary>
		/// <param name="sorter">��������</param>
		public void ExtendSorter(Sorter sorter)
		{
			#region ���ܼ���

			Timing.Start("��չ������", "ExtendSorter.GroupSchemaBuilder {149D86C2-6521-4a00-BA28-EE165EF42F70}");

			#endregion

			if (sorter != null)
			{
				ColumnLocator[] allColumnLocators = sorter.GetAllColumnLocators();
				ExtendGroupColumnLocators(allColumnLocators);
			}

			#region ���ܼ���

			Timing.Stop("ExtendSorter.GroupSchemaBuilder {149D86C2-6521-4a00-BA28-EE165EF42F70}");

			#endregion
		}

		/// <summary>
		/// ��չ���ԡ�
		/// </summary>
		/// <param name="properties">���Լ��ϡ�</param>
		/// <param name="inline">true - ���������������ļܹ���ʹ�����Բ�����飻false - ����ѡ������ԡ�</param>
		public void ExtendProperties(IPropertyChain[] properties, Boolean inline)
		{
			ColumnLocator[] allColumnLocators = Array.ConvertAll<IPropertyChain, ColumnLocator>(
					properties,
					delegate(IPropertyChain chain)
					{
						return new ColumnLocator(chain.PropertyPath);
					}
				);

			ExtendGroupColumnLocators(allColumnLocators);

			if (!inline)
			{
				foreach (IPropertyChain chain in properties)
				{
					if (chain.Previous == null)
					{
						continue;
					}

					IPropertyChain entityChain = GetForeignReferencePropertyChain(chain.PropertyPath);

					if (!m_properties.Contains(entityChain))
					{
						m_properties.Add(entityChain);
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// ����������ʵ��ܹ���
		/// </summary>
		/// <returns>�����õķ�����ʵ��ܹ���</returns>
		public GroupSchema Build()
		{
			#region ���ܼ���

			Timing.Start("���ɷ�����ʵ��ܹ�", "GroupSchemaBuilder.Build {AE38473C-17BC-4c6a-89C6-FC873EA5B7F6}");

			#endregion

			GroupSchema schema = new GroupSchema();

			schema.Definition = m_definition;

			schema.Composite = BuildComposite();
			BuildColumns(schema.Composite);

			schema.SelectColumns = m_selectColumns.ToArray();
			schema.GroupColumns = m_groupColumns.ToArray();

			schema.PrimitiveColumns = m_primitiveColumns.ToArray();
			schema.ForeignReferences = m_foreignReferences.ToArray();

			#region ���ܼ���

			Timing.Stop("GroupSchemaBuilder.Build {AE38473C-17BC-4c6a-89C6-FC873EA5B7F6}");

			#endregion

			return schema;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����ʵ��ܹ���ϡ�
		/// </summary>
		/// <returns>�����õ�ʵ��ܹ���ϡ�</returns>
		private EntitySchemaComposite BuildComposite()
		{
			CompositeBuilderStrategy strategy = CompositeBuilderStrategyFactory.Create4Group(m_definition.Type);

			List<PropertySelector> allSelectors = new List<PropertySelector>();

			if (m_schemas.Count != 0)
			{
				allSelectors.AddRange(
						m_schemas.ConvertAll<PropertySelector>(
							           delegate(IPropertyChain chain)
							           {
								           return PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain);
							           }
						           )
				           );

			}

			if (m_properties.Count != 0)
			{
				allSelectors.AddRange(
						m_properties.ConvertAll<PropertySelector>(
								delegate(IPropertyChain chain)
								{
									return PropertySelector.Create(PropertySelectMode.Property, chain);
								}
							)
					);
			}

			if (allSelectors.Count != 0)
			{
				strategy = CompositeBuilderStrategyFactory.Union(
						strategy,
						CompositeBuilderStrategyFactory.Create(allSelectors)
					);
			}

			PropertyTrimmer trimmer = PropertyTrimmer.CreateGroupIncapableTrimmer();

			strategy = CompositeBuilderStrategyFactory.TrimOff(strategy, trimmer);
			strategy.AlwaysSelectPrimaryKeyProperties = false;

			EntitySchemaComposite composite = EntitySchemaCompositeFactory.Create(m_definition.Entity.Type, strategy);

			return composite;
		}

		/// <summary>
		/// �����С�
		/// </summary>
		/// <param name="composite">Ҫ�ۺϵ�ʵ��ܹ���ϡ�</param>
		private void BuildColumns(EntitySchemaComposite composite)
		{
			BuildSelectAndPrimitiveColumns(composite);
			BuildGroupColumns(composite);
			BuildForeignReferences(composite);
		}

		/// <summary>
		/// �����ⲿ���á�
		/// </summary>
		/// <param name="composite">Ҫ�ۺϵ�ʵ��ܹ���ϡ�</param>
		private void BuildForeignReferences(EntitySchemaComposite composite)
		{
			GroupPropertyDefinition[] foreignReferences = m_definition.GetForeignReferenceProperties();

			foreach (GroupPropertyDefinition propertyDef in foreignReferences)
			{
				EntitySchema schema = FindSchema(composite, propertyDef);

				#region ǰ������

				Debug.Assert(schema != null, String.Format("δ�ҵ������� {0} ��ƥ���ʵ��ܹ���", propertyDef));

				#endregion

				m_foreignReferences.Add(new GroupForeignReference(propertyDef, schema));
			}
		}

		/// <summary>
		/// ���ɷ����С�
		/// </summary>
		/// <param name="composite">Ҫ�ۺϵ�ʵ��ܹ���ϡ�</param>
		private void BuildGroupColumns(EntitySchemaComposite composite)
		{
			m_groupColumns.AddRange(composite.Columns);

			foreach (IPropertyChain chain in m_groupChains)
			{
				IPropertyChain previous = chain.Previous;
				EntitySchema schema;

				if (previous == null)
				{
					schema = composite.Target;
				}
				else
				{
					schema = composite[previous.FullName];
				}

				#region ǰ������

				Debug.Assert(schema != null, String.Format("δ�ҵ����� {0} ������ʵ��ܹ���", chain));

				#endregion

				EntityProperty property = schema.Properties[chain.Name];

				#region ǰ������

				Debug.Assert(property != null, String.Format("��ʵ��ܹ� {0} �в��������� {1}��", schema, chain.Name));

				#endregion

				// �ж�Ҫ��������Ƿ��ѱ�ѡ��
				Boolean exists = false;

				if (!schema.SelectNothing)
				{
					EntityProperty[] selectProperties = composite.GetSelectProperties(schema);

					exists = Array.Exists<EntityProperty>(
							selectProperties,
							delegate(EntityProperty item)
							{
								return (item == property);
							}
						);
				}

				if (!exists)
				{
					m_groupColumns.AddRange(property.Columns);
				}
			}
		}

		/// <summary>
		/// ����ѡ��ͻ����С�
		/// </summary>
		/// <param name="composite">Ҫ�ۺϵ�ʵ��ܹ���ϡ�</param>
		private void BuildSelectAndPrimitiveColumns(EntitySchemaComposite composite)
		{
			List<Column> nonGroupColumns = new List<Column>();
			m_selectColumns.AddRange(composite.Columns);
			GroupPropertyDefinition[] primitiveProperties = m_definition.GetPrimitiveProperties();

			foreach (GroupPropertyDefinition groupPropertyDef in primitiveProperties)
			{
				EntitySchema schema = FindSchema(composite, groupPropertyDef);
				EntityProperty aggregationProperty;

				if (schema != null)
				{
					aggregationProperty = schema.Properties[groupPropertyDef.PropertyChain.Name];
				}
				else
				{
					aggregationProperty = null;
				}

				// �������ѡ��
				if (groupPropertyDef.IsGroupItem)
				{
					#region ǰ������

					Debug.Assert(
							aggregationProperty != null,
							String.Format("δ�ҵ��������ʵ�����Զ��� {0} ��ƥ���Ҫ�ۺϵ����ԡ�", groupPropertyDef)
						);

					#endregion

					GroupItemColumn itemColumn = new GroupItemColumn(aggregationProperty.Columns[0], groupPropertyDef);
					m_primitiveColumns.Add(itemColumn);
				}
				// �ۺ���
				else
				{
					AggregationColumn primitiveColumn = new AggregationColumn(aggregationProperty, groupPropertyDef);
					primitiveColumn.Selected = true;

					m_primitiveColumns.Add(primitiveColumn);
					nonGroupColumns.Add(primitiveColumn);
				}
			}

			for (Int32 i = 0; i < nonGroupColumns.Count; i++)
			{
				Column primitiveColumn = nonGroupColumns[i];
				primitiveColumn.Index = (composite.Columns.Length + i);
			}

			m_selectColumns.AddRange(nonGroupColumns);
		}

		/// <summary>
		/// �������Զ�����Ҫ�ۺϵ�����������ƥ���ʵ��ܹ���
		/// </summary>
		/// <param name="composite">ʵ��ܹ���ϡ�</param>
		/// <param name="propertyDef">������ʵ�����Զ��塣</param>
		/// <returns>�ҵ���ʵ��ܹ������û���ҵ����򷵻� null��</returns>
		private EntitySchema FindSchema(EntitySchemaComposite composite, GroupPropertyDefinition propertyDef)
		{
			if (propertyDef.PropertyChain == null)
			{
				return null;
			}

			String propertyPath;

			if (propertyDef.IsPrimitive)
			{
				IPropertyChain previous = propertyDef.PropertyChain.Previous;

				propertyPath = (previous != null) ? previous.FullName : propertyDef.PropertyChain.Type.Name;
			}
			else
			{
				propertyPath = propertyDef.PropertyChain.FullName;
			}

			return composite[propertyPath];
		}

		/// <summary>
		/// ��չ�����ж�λ�������Ǽ�������������ʵ��ܹ����������Է�������б��С�
		/// </summary>
		/// <param name="allColumnLocators">�ж�λ�����ϡ�</param>
		private void ExtendGroupColumnLocators(ColumnLocator[] allColumnLocators)
		{
			foreach (ColumnLocator colLocator in allColumnLocators)
			{
				#region ǰ������

				Debug.Assert(
						m_definition.Properties.Contains(colLocator.PropertyPath[0]),
						String.Format(
								"���� {0} �в��������� {1}������ۺ����Ա�Ǻ� HAVING �� ORDER BY ���ʽ��",
								m_definition.Type.FullName,
								colLocator.PropertyPath[0]
							)
					);

				#endregion

				// �������Ϊʵ���ֱ�����ԣ�����Ҫ��չ
				if (colLocator.PropertyPath.Length == 1)
				{
					continue;
				}

				// �������Ϊ�ⲿ�������Ե������ԣ�����Ҫ���غͷ���
				IPropertyChain groupChain = GetForeignReferencePropertyChain(colLocator.PropertyPath);

				#region ǰ������

				Debug.Assert(
						!Attribute.IsDefined(
								EntityDefinitionBuilder.Build(groupChain.Type).Properties[groupChain].PropertyInfo,
								typeof(NotSupportGroupingAttribute)
							),
						String.Format(
								"���� HAVING ���˻���������� {0} �ϱ�� NotSupportGroupingAttribute�������Բ��ܲ�����顣",
								groupChain.FullName
							)
					);

				#endregion

				if (!m_groupChains.Contains(groupChain))
				{
					m_groupChains.Add(groupChain);
				}

				IPropertyChain previous = groupChain.Previous;

				if (!previous.IsImmediateProperty)
				{
					IPropertyChain reference = previous.Copy();

					if (!m_schemas.Contains(reference))
					{
						m_schemas.Add(reference);
					}
				}
			}
		}

		/// <summary>
		/// ��ȡ����Ҫ�����ʵ�����������
		/// </summary>
		/// <param name="propertyPath">���������ʵ�������·����</param>
		/// <returns>��Ӧ������Ҫ�����ʵ�����������</returns>
		private IPropertyChain GetForeignReferencePropertyChain(String[] propertyPath)
		{
			GroupPropertyDefinition groupPropertyDef = m_definition.Properties[propertyPath[0]];
			String[] newPropertyPath = new String[groupPropertyDef.PropertyChain.Depth + propertyPath.Length - 1];

			Array.Copy(groupPropertyDef.PropertyChain.PropertyPath, 0, newPropertyPath, 0, groupPropertyDef.PropertyChain.Depth);
			Array.Copy(propertyPath, 1, newPropertyPath, groupPropertyDef.PropertyChain.Depth, (propertyPath.Length - 1));

			IPropertyChain foreignRef = new PropertyChain(groupPropertyDef.PropertyChain.Type, newPropertyPath);

			return foreignRef;
		}

		#endregion
	}
}