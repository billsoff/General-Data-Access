#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlJoinCriterionExpressionFormatter.cs
// �ļ����������������������е�һ�������ĸ�ʽ������
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
	/// �����������е�һ�������ĸ�ʽ������
	/// </summary>
	internal sealed class SqlTableJoinCriterionExpressionFormatter : SqlBlockExpressionFormatter
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
		public SqlTableJoinCriterionExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// ��ʽ����
		/// </summary>
		protected override void Format()
		{
			AppendLiteral(FormatOperators(FormattingText), true);
		}
	}
}