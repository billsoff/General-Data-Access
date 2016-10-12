#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ExpressionSchemaColumn.cs
// �ļ�������������ʾ���ʽ�ܹ��е��У����ڲ��н���һ�ΰ�װ��
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾ���ʽ�ܹ��е��У����ڲ��н���һ�ΰ�װ��
	/// </summary>
	internal sealed class ExpressionSchemaColumn : WrappedColumn
	{
		#region ˽���ֶ�

		private ExpressionSchema m_schema;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯���������������ļܹ���Ҫ��װ���С�
		/// </summary>
		/// <param name="column">Ҫ��װ���С�</param>
		public ExpressionSchemaColumn(Column column)
			: base(column)
		{
		}

		#endregion

		/// <summary>
		/// ��ȡ�����ƣ�Ϊ�ڲ��еı�����
		/// </summary>
		public override String Name
		{
			get
			{
				return InnerColumn.Alias;
			}
		}

		/// <summary>
		/// ��ȡ�е�ȫ���ơ�
		/// </summary>
		public override String FullName
		{
			get
			{
				return String.Format("{0}.{1}", Schema.Name, Name);
			}
		}

		/// <summary>
		/// ��ȡ�б��ʽ����ȫ������ͬ��
		/// </summary>
		public override String Expression
		{
			get
			{
				return FullName;
			}
		}

		/// <summary>
		/// ��ȡ�������ı��ʽ�ܹ���
		/// </summary>
		internal ExpressionSchema Schema
		{
			get { return m_schema; }
			set { m_schema = value; }
		}
	}
}