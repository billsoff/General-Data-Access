#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ExpressionSchema.cs
// �ļ�������������ʾһ�����ʽʵ��ܹ����Ӳ�ѯ����
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
	/// ��ʾһ�����ʽʵ��ܹ����Ӳ�ѯ����
	/// </summary>
	internal abstract class ExpressionSchema : IColumnLocatable, IDebugInfoProvider
	{
		#region ˽���ֶ�

		private readonly Column[] m_columns;
		private readonly String m_sqlExpression;

		private Int32 m_index;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ñ��ʽʵ���ѡ�����б�� SQL ���ʽ��
		/// </summary>
		/// <param name="columns">ѡ�����б�</param>
		/// <param name="sqlExpression">SQL ���ʽ��</param>
		protected ExpressionSchema(ExpressionSchemaColumn[] columns, String sqlExpression)
		{
			m_columns = columns;
			m_sqlExpression = sqlExpression;

			foreach (ExpressionSchemaColumn col in columns)
			{
				col.Schema = this;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����Ҫѡ����С�
		/// </summary>
		public Column[] Columns
		{
			get
			{
				return m_columns;
			}
		}

		/// <summary>
		/// ��ȡ������������
		/// </summary>
		public Int32 Index
		{
			get { return m_index; }
			set { m_index = value; }
		}

		/// <summary>
		/// ��ȡ���ơ�
		/// </summary>
		public String Name
		{
			get { return CommonPolicies.GetTableAlias(m_index); }
		}

		/// <summary>
		/// SQL ���ʽ��
		/// </summary>
		public String SqlExpression
		{
			get
			{
				return m_sqlExpression;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����ʵ�塣
		/// </summary>
		/// <param name="dbValues">��¼ֵ��</param>
		/// <returns>�����õ�ʵ�塣</returns>
		public abstract Object Compose(Object[] dbValues);

		#endregion

		#region IColumnLoactable ��Ա

		/// <summary>
		/// ��ȡ�ж�λ������λ���м��ϡ�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>�м��ϡ�</returns>
		public abstract Column[] this[ColumnLocator colLocator] { get; }

		#endregion

		#region IDebugInfoProvider ��Ա

		/// <summary>
		/// ���������Ϣ��Ĭ��ʵ����������͵�ȫ���ơ�
		/// </summary>
		/// <returns>������Ϣ��</returns>
		public virtual String Dump()
		{
			return GetType().FullName;
		}

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="indent"></param>
		/// <returns></returns>
		public String Dump(String indent)
		{
			return DbEntityDebugger.IndentText(Dump(), indent);
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