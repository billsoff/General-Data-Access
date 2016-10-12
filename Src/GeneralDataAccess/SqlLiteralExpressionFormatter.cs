#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlLiteralExpressionFormatter.cs
// �ļ�������������ʾԭ�������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110713
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
	/// ��ʾԭ�������
	/// </summary>
	internal sealed class SqlLiteralExpressionFormatter : SqlInlineExpressionFormatter
	{
		#region ���캯��

		/// <summary>
		/// ���캯�������� SQL ָ����ʽ��
		/// </summary>
		/// <param name="sqlExpression">SQL ָ����ʽ��</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">��λ������</param>
		/// <param name="level">����</param>
		public SqlLiteralExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// ����ԭ������Ҫ��ʽ�����ı���
		/// </summary>
		protected override void Format()
		{
			AppendParagraph(SqlParagraph.Create(FormatOperators(FormattingText)));
		}
	}
}