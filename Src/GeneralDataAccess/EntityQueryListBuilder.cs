#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityQueryListBuilder.cs
// �ļ�����������ʵ���ѯ�б���������
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
	/// ʵ���ѯ�б���������
	/// </summary>
	[Serializable]
	internal sealed class EntityQueryListBuilder : QueryListBuilder
	{
		#region ˽���ֶ�

		[NonSerialized]
		private EntitySchemaComposite m_composite;

		#endregion

		#region ���췽��

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		public EntityQueryListBuilder(IPropertyChain property, Filter where)
			: this(property, where, false)
		{
		}

		/// <summary>
		/// ���캯�������ó�ʼֵ��
		/// </summary>
		/// <param name="property">Ҫѡ������ԡ�</param>
		/// <param name="where">WHERE ��������</param>
		/// <param name="distinct">ָʾ�Ƿ���� DISTINC �ؼ��֡�</param>
		public EntityQueryListBuilder(IPropertyChain property, Filter where, Boolean distinct)
			: base(property, where, distinct)
		{
			#region ǰ�ö���

#if DEBUG

			EntityDefinition definition = EntityDefinitionBuilder.Build(property.Type);
			EntityPropertyDefinition propertyDef = definition.Properties[property];

			Debug.Assert(!propertyDef.HasComproundColumns, String.Format("���� {0} ӳ�䵽�����У��������ڹ������Ӳ�ѯ�б�", property.FullName));

#endif

			#endregion
		}

		#endregion

		/// <summary>
		/// ����ʵ��ܹ���ϡ�
		/// </summary>
		protected override void BuildInfrastructure()
		{
			List<PropertySelector> allSelectors = new List<PropertySelector>();

			allSelectors.Add(PropertySelector.Create(Property));

			if (Where != null)
			{
				allSelectors.AddRange(Where.GetAllSelectors(Property.Type));
			}

			CompositeBuilderStrategy builderStrategy = CompositeBuilderStrategyFactory.Create(allSelectors);
			builderStrategy.AlwaysSelectPrimaryKeyProperties = false;

			m_composite = EntitySchemaCompositeFactory.Create(Property.Type, builderStrategy);
		}

		/// <summary>
		/// ���� WHERE ��������
		/// </summary>
		/// <returns>��������</returns>
		protected override FilterCompilationResult CompileWhereFilter()
		{
			return m_composite.ComposeFilterExpression(Where, ParameterPrefix);
		}

		/// <summary>
		/// ���� HAVING ��������
		/// </summary>
		/// <returns>���Ƿ��� null��</returns>
		protected override FilterCompilationResult CompileHavingFilter()
		{
			#region ǰ�ö���

			Debug.Assert(Having == null, "ʵ��ѡ���б��ܽ��� HAVING ��������");

			#endregion

			return null;
		}

		/// <summary>
		/// ���� SQL ָ�
		/// </summary>
		/// <returns>�����õ� SQL ָ�</returns>
		protected override String ComposeSqlStatement()
		{
			SqlSelectExpressionBuilder sqlBuilder = new SqlSelectExpressionBuilder();

			Column[] columns = m_composite.Target[new ColumnLocator(Property.PropertyPath)];

			sqlBuilder.Select = columns[0].FullName;
			sqlBuilder.From = m_composite.FromList;
			sqlBuilder.Where = WhereClause;

			sqlBuilder.Distinct = Distinct;

			return sqlBuilder.Build();
		}
	}
}