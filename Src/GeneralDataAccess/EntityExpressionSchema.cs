#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityExpressionSchema.cs
// �ļ�����������ʵ����ʽ�ܹ���
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
	/// ʵ����ʽ�ܹ���
	/// </summary>
	internal sealed class EntityExpressionSchema : ExpressionSchema, IDebugInfoProvider
	{
		#region ˽���ֶ�

		private Dictionary<EntityProperty, Column[]> m_columLookups;
		private EntitySchemaComposite m_composite;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ѡ���м��Ϻ� SQL ָ�
		/// </summary>
		/// <param name="columns">ѡ���м��ϡ�</param>
		/// <param name="sqlExpression">SQL ָ�</param>
		internal EntityExpressionSchema(ExpressionSchemaColumn[] columns, String sqlExpression)
			: base(columns, sqlExpression)
		{
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ�������в����ֵ䣬��Ϊѡ�����ԣ�ֵΪ�����������İ�װ�С�
		/// </summary>
		internal Dictionary<EntityProperty, Column[]> ColumLookups
		{
			get { return m_columLookups; }
			set { m_columLookups = value; }
		}

		/// <summary>
		/// ��ȡ������ʵ��ܹ���ϡ�
		/// </summary>
		internal EntitySchemaComposite Composite
		{
			get { return m_composite; }
			set { m_composite = value; }
		}

		#endregion

		/// <summary>
		/// ����ʵ�塣
		/// </summary>
		/// <param name="dbValues">��¼ֵ��</param>
		/// <returns>����õ�ʵ�塣</returns>
		public override Object Compose(Object[] dbValues)
		{
			return m_composite.Target.Compose(dbValues);
		}

		/// <summary>
		/// ��λ�С�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>��λ�����С�</returns>
		public override Column[] this[ColumnLocator colLocator]
		{
			get
			{
				Column[] columns = m_composite.Target[colLocator];
				EntityProperty property = columns[0].Property;

				#region ǰ������

				Debug.Assert(m_columLookups.ContainsKey(property), String.Format("���� {0} δ��ѡ��", colLocator.FullName));

				#endregion

				return m_columLookups[property];
			}
		}

		#region IDebugInfoProvider ��Ա

		/// <summary>
		/// ��ȡʵ����ʽ�ܹ�����ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>ʵ����ʽ�ܹ�����ϸ��Ϣ��</returns>
		public override String Dump()
		{
			return Composite.Dump() + "\r\n" + Composite.BuilderStrategy.Dump();
		}

		#endregion
	}
}