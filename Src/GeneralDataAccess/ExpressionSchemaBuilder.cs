#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ExpressionSchemaBuilder.cs
// �ļ�������������ʾһ�����ʽ��������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110707
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾһ�����ʽ��������
	/// </summary>
	internal abstract class ExpressionSchemaBuilder
	{
		#region ˽���ֶ�

		private readonly CompositeForeignReferencePropertyDefinition m_compositePropertyDefinition;
		private readonly String m_parameterPrefix;

		private CompositeItemSettings m_itemSettings;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�����������ͺͲ�ѯ����ǰ׺��
		/// </summary>
		/// <param name="propertyDef">����ʵ�����Զ��塣</param>
		/// <param name="parameterPrefix">��ѯ����ǰ׺��</param>
		protected ExpressionSchemaBuilder(CompositeForeignReferencePropertyDefinition propertyDef, String parameterPrefix)
		{
			m_compositePropertyDefinition = propertyDef;
			m_parameterPrefix = parameterPrefix;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ǰ׺��
		/// </summary>
		public String ParameterPrefix
		{
			get { return m_parameterPrefix; }
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ����ʵ�����Զ��塣
		/// </summary>
		internal CompositeForeignReferencePropertyDefinition CompositePropertyDefinition
		{
			get { return m_compositePropertyDefinition; }
		}

		#endregion

		/// <summary>
		/// ��ȡ�����ù�������
		/// </summary>
		public CompositeItemSettings ItemSettings
		{
			get { return m_itemSettings; }
			set { m_itemSettings = value; }
		}

		/// <summary>
		/// ��չ���������ԡ�
		/// </summary>
		/// <param name="properties">Ҫ��չ�����������ϡ�</param>
		/// <param name="inline">
		/// <para>ָʾ�Ƿ��������ķ�ʽ��չ��</para>
		/// <para>true - ���� WHERE �� HAVING ��������ֻ��Ҫ����������������ʵ��ܹ������ڷ����ѯ�� HAVING ��������Ҫô������Ϊ�ۺ����ԣ�ҪôΪ�������ݣ���</para>
		/// <para>false - Ҫ��֤���Ա�ѡ��</para>
		/// </param>
		public abstract void ExtendProperties(IPropertyChain[] properties, Boolean inline);

		/// <summary>
		/// ֪ͨ�����ײ�ṹ��
		/// </summary>
		public abstract void BuildInfrastructure();

		/// <summary>
		/// ���� WHERE ���������ʽ��
		/// </summary>
		/// <param name="parameterIndexOffset">��������ƫ��ֵ��</param>
		/// <returns>���ɵĲ�ѯ�����б�</returns>
		public abstract QueryParameter[] ComposeWhereFilterExpression(Int32 parameterIndexOffset);

		/// <summary>
		/// ���� HAVING ���������ʽ��
		/// </summary>
		/// <param name="parameterIndexOffset">��������ƫ��ֵ��</param>
		/// <returns>���ɵĲ����б�</returns>
		public abstract QueryParameter[] ComposeHavingFilterExpression(Int32 parameterIndexOffset);

		/// <summary>
		/// ���ɱ��ʽʵ��ܹ���
		/// </summary>
		/// <param name="fieldIndexOffset">�ֶ�ƫ������</param>
		/// <returns>���ɺõı��ʽʵ��ܹ���</returns>
		public abstract ExpressionSchema Build(Int32 fieldIndexOffset);
	}
}