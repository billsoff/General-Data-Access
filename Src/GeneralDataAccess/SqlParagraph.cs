#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlParagraph.cs
// �ļ�������������ʾ SQL ָ���һ�Ρ�
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
	/// ��ʾ SQL ָ���һ�Ρ�
	/// </summary>
	internal abstract class SqlParagraph
	{
		#region ��������

		/// <summary>
		/// �����š�
		/// </summary>
		public static readonly SqlParagraph CloseParenthesis = new SqlLiteralParagraph(SqlExpressionFormattingHelper.CLOSE_PARENTHESIS);

		/// <summary>
		/// ���š�
		/// </summary>
		public static readonly SqlParagraph Comma = new SqlLiteralParagraph(SqlExpressionFormattingHelper.COMMA);

		/// <summary>
		/// ���С�
		/// </summary>
		public static readonly SqlParagraph NewLine = new SqlLiteralParagraph(Environment.NewLine);

		/// <summary>
		/// �����š�
		/// </summary>
		public static readonly SqlParagraph OpenParenthesis = new SqlLiteralParagraph(SqlExpressionFormattingHelper.OPEN_PARENTHESIS);

		/// <summary>
		/// �ո�
		/// </summary>
		public static readonly SqlParagraph Space = new SqlLiteralParagraph(SqlExpressionFormattingHelper.SPACE);

		/// <summary>
		/// ������������Ķ��䡣
		/// </summary>
		/// <param name="text">�ı���</param>
		/// <returns>���䡣</returns>
		public static SqlParagraph Create(String text)
		{
			return new SqlLiteralParagraph(text);
		}

		/// <summary>
		/// �������϶��䡣
		/// </summary>
		/// <param name="formatter">��ʽ������</param>
		/// <returns>���϶��䡣</returns>
		public static SqlParagraph Create(SqlExpressionFormatter formatter)
		{
			return new SqlCompositeParagraph(formatter);
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��������ı���
		/// </summary>
		/// <returns>�����ı���</returns>
		public sealed override String ToString()
		{
			return Output();
		}

		#endregion

		#region �����Ա

		/// <summary>
		/// ��������ı���
		/// </summary>
		/// <returns>�����ı���</returns>
		protected abstract String Output();

		#endregion
	}
}