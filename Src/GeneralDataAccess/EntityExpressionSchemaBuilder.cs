#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityExpressionSchemaBuilder.cs
// �ļ�����������ʵ����ʽ�ܹ���������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110711
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
	/// ʵ����ʽ�ܹ���������
	/// </summary>
	internal class EntityExpressionSchemaBuilder : ExpressionSchemaBuilder
	{
		#region ˽���ֶ�

		private readonly EntityDefinition m_definition;
		private List<PropertySelector> m_allSelectors;
		private EntitySchemaComposite m_composite;

		private String m_whereExpression;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø���ʵ�����Զ���Ͳ�ѯ����ǰ׺��
		/// </summary>
		/// <param name="propertyDef">����ʵ�����Զ��塣</param>
		/// <param name="parameterPrefix">��ѯ����ǰ׺��</param>
		public EntityExpressionSchemaBuilder(CompositeForeignReferencePropertyDefinition propertyDef, String parameterPrefix)
			: base(propertyDef, parameterPrefix)
		{
			m_definition = EntityDefinitionBuilder.Build(propertyDef.Type);
		}

		#endregion

		/// <summary>
		/// ��չ���ԡ�
		/// </summary>
		/// <param name="properties">Ҫ��չ�����Լ��ϡ�</param>
		/// <param name="inline">ָʾ�Ƿ������ʵ��ܹ���</param>
		public override void ExtendProperties(IPropertyChain[] properties, Boolean inline)
		{
			if (inline)
			{
				Array.ForEach<IPropertyChain>(
						properties,
						delegate(IPropertyChain chain)
						{
							AllSelectors.Add(PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain));
						}
					);
			}
			else
			{
				Array.ForEach<IPropertyChain>(
						properties,
						delegate(IPropertyChain chain)
						{
							AllSelectors.Add(PropertySelector.Create(PropertySelectMode.Property, chain));
						}
					);
			}
		}

		/// <summary>
		/// ����ʵ��ܹ���ϡ�
		/// </summary>
		public override void BuildInfrastructure()
		{
			BuildComposite();
		}

		/// <summary>
		/// ���� WHERE ���������ʽ��
		/// </summary>
		/// <param name="parameterIndexOffset">��������ƫ�ơ�</param>
		/// <returns>���ɵĲ������ϡ�</returns>
		public override QueryParameter[] ComposeWhereFilterExpression(Int32 parameterIndexOffset)
		{
			if ((ItemSettings != null) && (ItemSettings.Where != null))
			{
				EntitySchemaFilterCompilationContext context = new EntitySchemaFilterCompilationContext(m_composite.Target, ParameterPrefix);
				context.IndexOffset = parameterIndexOffset;

				FilterCompilationResult result = Filter.Compile(context, ItemSettings.Where);

				m_whereExpression = result.WhereClause;

				return result.Parameters;
			}
			else
			{
				return new QueryParameter[0];
			}
		}

		/// <summary>
		/// ���� HAVING ���������ʽ����ǰ��ʵ��Ϊ�����κβ�����
		/// </summary>
		/// <param name="parameterIndexOffset">��������ƫ�ơ�</param>
		/// <returns>��ѯ�������ϡ�</returns>
		public override QueryParameter[] ComposeHavingFilterExpression(Int32 parameterIndexOffset)
		{
			#region ǰ�ö���

			Debug.Assert((ItemSettings == null) || (ItemSettings.Having == null), "ʵ����ʽ�ܹ���֧�� HAVING ��������");

			#endregion

			return new QueryParameter[0];
		}

		/// <summary>
		/// ��ʼ���ɡ�
		/// </summary>
		/// <param name="fieldIndexOffset">�ֶ�����ƫ�ơ�</param>
		/// <returns>���ʽ�ܹ���</returns>
		public override ExpressionSchema Build(Int32 fieldIndexOffset)
		{
			m_composite.SetFieldIndexOffset(fieldIndexOffset);

			// ��װһ��ѡ���У�������ѡ�����ԺͰ�װѡ���е�ӳ���ϵ
			Dictionary<EntityProperty, Column[]> columnLookups = new Dictionary<EntityProperty, Column[]>();
			List<ExpressionSchemaColumn> allColumns = new List<ExpressionSchemaColumn>();

			foreach (EntityProperty property in m_composite.GetAllSelectProperties())
			{
				ExpressionSchemaColumn[] columns = new ExpressionSchemaColumn[property.Columns.Count];

				for (Int32 i = 0; i < columns.Length; i++)
				{
					columns[i] = new ExpressionSchemaColumn(property.Columns[i]);
					columns[i].Index += fieldIndexOffset;
				}

				columnLookups.Add(property, columns);
				allColumns.AddRange(columns);
			}

			// �ϳ� SQL ָ��
			String sqlExpression = ComposeSqlExpression();

			EntityExpressionSchema schema = new EntityExpressionSchema(allColumns.ToArray(), sqlExpression);

			schema.ColumLookups = columnLookups;
			schema.Composite = m_composite;

			return schema;
		}

		#region ˽������

		/// <summary>
		/// ��ȡ����ѡ�����б�
		/// </summary>
		public List<PropertySelector> AllSelectors
		{
			get
			{
				if (m_allSelectors == null)
				{
					m_allSelectors = new List<PropertySelector>();
				}

				return m_allSelectors;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����ʵ��ܹ���ϡ�
		/// </summary>
		private void BuildComposite()
		{
			if ((ItemSettings != null) && (ItemSettings.Where != null))
			{
				AllSelectors.AddRange(ItemSettings.Where.GetAllSelectors(CompositePropertyDefinition.Type));
			}

			CompositeBuilderStrategy loadStrategy;

			if ((ItemSettings != null) && (ItemSettings.Select != null))
			{
				loadStrategy = ItemSettings.Select;
			}
			else
			{
				LoadStrategyAttribute loadStrategyAttr = CompositePropertyDefinition.LoadStrategy ?? m_definition.LoadStrategy;
				loadStrategy = loadStrategyAttr.Create();
			}

			CompositeBuilderStrategy strategy = CompositeBuilderStrategyFactory.Compose(
					CompositePropertyDefinition.Type,
					loadStrategy,
					m_allSelectors
				);

			m_composite = EntitySchemaCompositeFactory.Create(CompositePropertyDefinition.Type, strategy);
		}

		/// <summary>
		/// ���� SQL ָ�
		/// </summary>
		/// <returns>����õ� SQL ָ�</returns>
		private String ComposeSqlExpression()
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			sqlBuilder.Select = m_composite.SelectList;
			sqlBuilder.From = m_composite.FromList;
			sqlBuilder.Where = m_whereExpression;

			return sqlBuilder.Build();
		}

		#endregion
	}
}