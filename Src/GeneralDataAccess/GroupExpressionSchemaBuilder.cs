#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupExpressionSchemaBuilder.cs
// �ļ�����������������ʽ�ܹ���������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110712
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
	/// ������ʽ�ܹ���������
	/// </summary>
	internal sealed class GroupExpressionSchemaBuilder : ExpressionSchemaBuilder
	{
		#region ˽���ֶ�

		private readonly GroupSchemaBuilder m_groupSchemaBuilder;
		private GroupSchema m_groupSchema;

		private String m_whereExpression;
		private String m_havingExpression;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø���ʵ�����ԺͲ�ѯ����ǰ׺��
		/// </summary>
		/// <param name="propertyDef">����ʵ�����ԡ�</param>
		/// <param name="parameterPrefix">��ѯ����ǰ׺��</param>
		public GroupExpressionSchemaBuilder(CompositeForeignReferencePropertyDefinition propertyDef, String parameterPrefix)
			: base(propertyDef, parameterPrefix)
		{
			m_groupSchemaBuilder = new GroupSchemaBuilder(propertyDef.Type);
		}

		#endregion

		/// <summary>
		/// ��չ���ԡ�
		/// </summary>
		/// <param name="properties">�μ� ExpressionSchemaBuilder �е�ע�͡�</param>
		/// <param name="inline">�μ� ExpressionSchemaBuilder �е�ע�͡�</param>
		public override void ExtendProperties(IPropertyChain[] properties, Boolean inline)
		{
			m_groupSchemaBuilder.ExtendProperties(properties, inline);
		}

		/// <summary>
		/// ��������ܹ���
		/// </summary>
		public override void BuildInfrastructure()
		{
			if (ItemSettings != null)
			{
				#region ǰ�ö���

				Debug.Assert(ItemSettings.Select == null, "������ʽ�ܹ���֧����ʾ����ѡ��");

				#endregion

				if (ItemSettings.Where != null)
				{
					m_groupSchemaBuilder.ExtendWhereFilter(ItemSettings.Where);
				}

				if (ItemSettings.Having != null)
				{
					m_groupSchemaBuilder.ExtendHavingFilter(ItemSettings.Having);
				}
			}

			m_groupSchema = m_groupSchemaBuilder.Build();
		}

		/// <summary>
		/// ���� WHERE ���������ʽ��
		/// </summary>
		/// <param name="parameterIndexOffset">��������ƫ�ơ�</param>
		/// <returns>���ɵĲ�ѯ�������ϡ�</returns>
		public override QueryParameter[] ComposeWhereFilterExpression(Int32 parameterIndexOffset)
		{
			if ((ItemSettings != null) && (ItemSettings.Where != null))
			{
				GeneralFilterCompilationContext context = new GeneralFilterCompilationContext(m_groupSchema.Composite.Target, ParameterPrefix);
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
		/// ���� HAVING ���������ʽ��
		/// </summary>
		/// <param name="parameterIndexOffset">��������ƫ�ơ�</param>
		/// <returns>���ɵĲ������ϡ�</returns>
		public override QueryParameter[] ComposeHavingFilterExpression(Int32 parameterIndexOffset)
		{
			if ((ItemSettings != null) && (ItemSettings.Having != null))
			{
				GroupSchemaFilterCompilationContext context = new GroupSchemaFilterCompilationContext(m_groupSchema, ParameterPrefix);
				context.IndexOffset = parameterIndexOffset;

				FilterCompilationResult result = Filter.Compile(context, ItemSettings.Having);
				m_havingExpression = result.WhereClause;

				return result.Parameters;
			}
			else
			{
				return new QueryParameter[0];
			}
		}

		/// <summary>
		/// �������ʽ�ܹ���
		/// </summary>
		/// <param name="fieldIndexOffset">�ֶ�����ƫ�ơ�</param>
		/// <returns>���ʽ�ܹ���</returns>
		public override ExpressionSchema Build(Int32 fieldIndexOffset)
		{
			m_groupSchema.SetFieldIndexOffset(fieldIndexOffset);

			// ���ɰ�װ��
			ExpressionSchemaColumn[] columns = new ExpressionSchemaColumn[m_groupSchema.SelectColumns.Length];

			for (Int32 i = 0; i < columns.Length; i++)
			{
				columns[i] = new ExpressionSchemaColumn(m_groupSchema.SelectColumns[i]);
				columns[i].Index += fieldIndexOffset;
			}

			// ���� SQL ָ��
			String sqlExpression = ComposeSqlExpression();

			GroupExpressionSchema schema = new GroupExpressionSchema(columns, sqlExpression);
			schema.GroupSchema = m_groupSchema;

			return schema;
		}

		#region ��������

		/// <summary>
		/// ���� SQL ָ�
		/// </summary>
		/// <returns>�����õ� SQL ָ�</returns>
		private String ComposeSqlExpression()
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			sqlBuilder.Select = m_groupSchema.SelectList;
			sqlBuilder.From = m_groupSchema.FromList;
			sqlBuilder.Where = m_whereExpression;
			sqlBuilder.GroupBy = m_groupSchema.GroupList;
			sqlBuilder.Having = m_havingExpression;

			return sqlBuilder.Build();
		}

		#endregion
	}
}