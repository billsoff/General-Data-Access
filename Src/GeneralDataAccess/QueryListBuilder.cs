#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����QueryListBuilder.cs
// �ļ�������������ѯ�б������������� IN ��������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110721
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
	/// ��ѯ�б������������� IN ��������
	/// </summary>
	[Serializable]
	internal abstract class QueryListBuilder
	{
		#region ˽���ֶ�

		private readonly IPropertyChain m_property;
		private readonly Filter m_where;
		private readonly Filter m_having;
		private readonly Boolean m_distinct;

		private String m_whereClause;
		private String m_havingClause;

		[NonSerialized]
		private FilterCompilationContext m_parentContext;

		#endregion

		#region ��̬����

		/// <summary>
		/// ������ѯ�б���������
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ�Ƿ��� SQL ָ���а��� DISTINCT �ؼ��֡�</param>
		/// <returns>��ѯ�б���������</returns>
		public static QueryListBuilder Create(IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			ExpressionSchemaType type = ExpressionSchemaBuilderFactory.GetExpressionSchemaType(property.Type);

			switch (type)
			{
				case ExpressionSchemaType.Entity:
					#region ǰ������

					Debug.Assert(havingFilter == null, "ʵ��ܹ����ܽ��� HAVING ��������");

					#endregion

					return new EntityQueryListBuilder(property, whereFilter, distinct);

				case ExpressionSchemaType.Group:
					return new GroupQueryListBuilder(property, whereFilter, havingFilter, distinct);

				case ExpressionSchemaType.Unknown:
				default:
					Debug.Fail(String.Format("���� {0} ����ʶ��", property.Type.FullName));
					return null;
			}
		}

		#endregion

		#region ���췽��

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		protected QueryListBuilder(IPropertyChain property, Filter where)
			: this(property, where, (Filter)null, false)
		{
		}

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		/// <param name="distinct">ָʾ�Ƿ���� DISTINC �ؼ��֡�</param>
		protected QueryListBuilder(IPropertyChain property, Filter where, Boolean distinct)
			: this(property, where, (Filter)null, distinct)
		{
		}

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		/// <param name="having">HAVING ��������</param>
		protected QueryListBuilder(IPropertyChain property, Filter where, Filter having)
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
		protected QueryListBuilder(IPropertyChain property, Filter where, Filter having, Boolean distinct)
		{
			m_property = property;
			m_where = where;
			m_having = having;
			m_distinct = distinct;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ��� SQL ָ���а��� DISTINCT �ؼ��֡�
		/// </summary>
		public Boolean Distinct
		{
			get { return m_distinct; }
		}

		/// <summary>
		/// ��ȡҪѡ������ԡ�
		/// </summary>
		public IPropertyChain Property
		{
			get { return m_property; }
		}

		/// <summary>
		/// ��ȡ WHERE ��������
		/// </summary>
		public Filter Where
		{
			get { return m_where; }
		}

		/// <summary>
		/// ��ȡ HAVING ��������
		/// </summary>
		public Filter Having
		{
			get { return m_having; }
		}

		/// <summary>
		/// ��ȡ���ɵ� WHERE �Ӿ䡣
		/// </summary>
		public String WhereClause
		{
			get { return m_whereClause; }
		}

		/// <summary>
		/// ��ȡ���ɵ� HAVING �Ӿ䡣
		/// </summary>
		public String HavingClause
		{
			get { return m_havingClause; }
		}

		/// <summary>
		/// ��ȡ��ѯ����ǰ׺����
		/// </summary>
		public String ParameterPrefix
		{
			get { return m_parentContext.ParameterPrefix; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ���� SQL ָ�
		/// </summary>
		/// <param name="parentContext">�����뻷����</param>
		/// <returns>�����õ� SQL ָ�</returns>
		public String Build(FilterCompilationContext parentContext)
		{
			if (m_parentContext == null)
			{
				m_parentContext = parentContext;
			}

			SetParentContext();
			BuildInfrastructure();
			CompileFilters();

			// ���� SQL ָ��
			return ComposeSqlStatement();
		}

		#endregion

		#region �����Ա

		/// <summary>
		/// ֪ͨ�����ײ�ṹ��
		/// </summary>
		protected abstract void BuildInfrastructure();

		/// <summary>
		/// ���� WHERE ��������
		/// </summary>
		/// <returns>��������</returns>
		protected abstract FilterCompilationResult CompileWhereFilter();

		/// <summary>
		/// ���� HAVING ��������
		/// </summary>
		/// <returns>��������</returns>
		protected abstract FilterCompilationResult CompileHavingFilter();

		/// <summary>
		/// ���� SQL ָ�
		/// </summary>
		/// <returns>�����õ� SQL ָ�</returns>
		protected abstract String ComposeSqlStatement();

		#endregion

		#region ��������

		/// <summary>
		/// Ϊ WHERE �� HAVING ���������ø����뻷����
		/// </summary>
		private void SetParentContext()
		{
			if (m_where != null)
			{
				QueryListBuilder[] whereBuilders = m_where.GetAllQueryListBuilders();

				SetParentContext(whereBuilders);
			}

			if (m_having != null)
			{
				QueryListBuilder[] havingBuilders = m_having.GetAllQueryListBuilders();

				SetParentContext(havingBuilders);
			}
		}

		/// <summary>
		/// ���ò�ѯ�б��������ĸ����뻷����
		/// </summary>
		/// <param name="allBuilders">��ѯ�б����������ϡ�</param>
		private void SetParentContext(QueryListBuilder[] allBuilders)
		{
			Array.ForEach<QueryListBuilder>(
					allBuilders,
					delegate(QueryListBuilder builder)
					{
						builder.m_parentContext = m_parentContext;
					}
				);
		}

		/// <summary>
		/// �����������
		/// </summary>
		private void CompileFilters()
		{
			if (m_where != null)
			{
				FilterCompilationResult whereResult = CompileWhereFilter();
				whereResult = Transform(whereResult);
				m_whereClause = whereResult.WhereClause;
			}

			if (m_having != null)
			{
				FilterCompilationResult havingResult = CompileHavingFilter();
				havingResult = Transform(havingResult);
				m_havingClause = havingResult.WhereClause;
			}
		}

		/// <summary>
		/// ת����������������
		/// </summary>
		/// <param name="compilationResult">��������</param>
		/// <returns>ת�����µĽ����</returns>
		private FilterCompilationResult Transform(FilterCompilationResult compilationResult)
		{
			QueryParameter[] registeredParameters = m_parentContext.RegisterParameters(compilationResult.Parameters);
			String newClause = compilationResult.WhereClause;

			for (Int32 i = 0; i < registeredParameters.Length; i++)
			{
				newClause = newClause.Replace(
						ParameterPrefix + compilationResult.Parameters[i].Name,
						ParameterPrefix + registeredParameters[i].Name
					);
			}

			return new FilterCompilationResult(newClause, registeredParameters);
		}

		#endregion
	}
}