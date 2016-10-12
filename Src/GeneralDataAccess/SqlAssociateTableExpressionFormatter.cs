#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlAssociateTableExpressionFormatter.cs
// �ļ������������������������ָ�ģ�������ĸ�ʽ������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110714
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
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// �������������ָ�ģ�������ĸ�ʽ������
	/// </summary>
	internal sealed class SqlAssociateTableExpressionFormatter : SqlBlockExpressionFormatter
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ��ʽ���� SQL ָ��ȡ�
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		public SqlAssociateTableExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// ��ʽ����
		/// </summary>
		protected override void Format()
		{
			Match delimiter = FindValidDelimiter(@"\bON\b");

			if (delimiter != null)
			{
				AppendComposite(CreateTableItemFormatter(0, delimiter.Index));

				AppendLiteral(delimiter.Value, true);

				Int32 currentIndex = delimiter.Index + delimiter.Length;
				Int32 length = this.Length - currentIndex;

				AppendComposite(CreateTableJoinCriteriaFormatter(currentIndex, length));
			}
			else
			{
				AppendComposite(CreateTableItemFormatter(0, this.Length));
			}
		}

		#region ˽�з���

		/// <summary>
		/// ������ON ���ģ�����ʽ��ʽ������
		/// </summary>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>��ʽ������</returns>
		private SqlExpressionFormatter CreateTableItemFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlTableItemExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		/// <summary>
		/// ������ON �Ҳ�ģ������Ӹ���������ʽ������
		/// </summary>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>��ʽ������</returns>
		private SqlExpressionFormatter CreateTableJoinCriteriaFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlTableJoinCriteriaExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		#endregion
	}
}