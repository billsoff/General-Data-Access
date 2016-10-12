#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlTableJoinCriteriaExpressionFormatter.cs
// �ļ�������������ON �Ҳ�ģ�������������ʽ������
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
	/// ��ON �Ҳ�ģ�������������ʽ������
	/// </summary>
	internal sealed class SqlTableJoinCriteriaExpressionFormatter : SqlBlockExpressionFormatter
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
		public SqlTableJoinCriteriaExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// ��ʽ����
		/// </summary>
		protected override void Format()
		{
			Match[] allDelimiters = FindValidDelimiters(@"\bAND\b");

			Int32 currentIndex = 0;

			for (Int32 i = 0; i < allDelimiters.Length; i++)
			{
				Match delimiter = allDelimiters[i];
				Int32 position = delimiter.Index;
				Int32 length = position - currentIndex;

				AppendComposite(CreateTableJoinCriterionFormatter(currentIndex, length));

				AppendNewLine();
				AppendIndent();
				AppendLiteral(delimiter.Value);

				currentIndex = position + delimiter.Length;
			}

			if (currentIndex < this.Length)
			{
				AppendComposite(CreateTableJoinCriterionFormatter(currentIndex, (this.Length - currentIndex)));
			}
		}

		#region ��������

		/// <summary>
		/// ������������������ʽ������
		/// </summary>
		/// <param name="startIndex">��Ҫ��ʽ�����ı��е���ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>��ʽ������</returns>
		private SqlExpressionFormatter CreateTableJoinCriterionFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlTableJoinCriterionExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, (Level + 1));
		}

		#endregion
	}
}