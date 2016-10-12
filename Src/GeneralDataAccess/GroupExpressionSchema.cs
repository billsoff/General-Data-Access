#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupExpressionSchema.cs
// �ļ�����������������ʽ�ܹ���
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ������ʽ�ܹ���
	/// </summary>
	internal sealed class GroupExpressionSchema : ExpressionSchema
	{
		#region ˽���ֶ�

		private GroupSchema m_groupSchema;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ѡ���м��Ϻ� SQL ָ�
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="sqlExpression"></param>
		public GroupExpressionSchema(ExpressionSchemaColumn[] columns, String sqlExpression)
			: base(columns, sqlExpression)
		{
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ�����÷���ܹ���
		/// </summary>
		internal GroupSchema GroupSchema
		{
			get { return m_groupSchema; }
			set { m_groupSchema = value; }
		}

		#endregion

		/// <summary>
		/// ��������ʵ�塣
		/// </summary>
		/// <param name="dbValues">��¼��</param>
		/// <returns>�����õķ���ʵ�塣</returns>
		public override Object Compose(Object[] dbValues)
		{
			return m_groupSchema.Compose(dbValues);
		}

		/// <summary>
		/// ��λ�С�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>��λ���м��ϡ�</returns>
		public override Column[] this[ColumnLocator colLocator]
		{
			get
			{
				Column[] columns = m_groupSchema[colLocator];
				Column[] expressionSchemaColumns = GetExpressionSchemaColumns(columns);

				return expressionSchemaColumns;
			}
		}

		/// <summary>
		/// ��ʾ����ʵ����ʽ����ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>����ʵ����ʽ����ϸ��Ϣ��</returns>
		public override String Dump()
		{
			return GroupSchema.Dump();
		}

		#region ��������

		/// <summary>
		/// ��ȡ����ܹ��еİ�װ�С�
		/// </summary>
		/// <param name="columns">����ܹ��м��ϡ�</param>
		/// <returns>���м��ϵİ�װ�м��ϡ�</returns>
		private Column[] GetExpressionSchemaColumns(Column[] columns)
		{
			Column[] schemaColumns = new Column[columns.Length];

			for (Int32 i = 0; i < schemaColumns.Length; i++)
			{
				Int32 index = Array.FindIndex<Column>(
						m_groupSchema.SelectColumns,
						delegate(Column col)
						{
							return WrappedColumn.GetRootColumn(col).Equals(WrappedColumn.GetRootColumn(columns[i]));
						}
					);

				schemaColumns[i] = this.Columns[index];
			}

			return schemaColumns;
		}

		#endregion
	}
}