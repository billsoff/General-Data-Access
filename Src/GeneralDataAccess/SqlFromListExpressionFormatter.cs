#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlFromListExpressionFormatter.cs
// �ļ����������������ӱ��ʽ��ʽ������
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
	/// �����ӱ��ʽ��ʽ������
	/// </summary>
	internal sealed class SqlFromListExpressionFormatter : SqlBlockExpressionFormatter
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
		public SqlFromListExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		protected override void Format()
		{
			Match[] allDelimiters = FindValidDelimiters(

			#region ����������

@"
\b
(?<Keyword>
	INNER\s+JOIN
	| LEFT\s+OUTER\s+JOIN
	| RIGHT\s+OUTER\s+JOIN
	| FULL\s+OUTER\s+JOIN
	| LEFT\s+JOIN
	| RIGHT\s+JOIN
	| FULL\s+JOIN
)
\b
"

			#endregion

);

			Int32 currentIndex = 0;

			for (Int32 i = 0; i < allDelimiters.Length; i++)
			{
				Match delimiter = allDelimiters[i];
				Int32 position = delimiter.Index;
				Int32 length = position - currentIndex;

				AppendComposite(CreateAssociateTableFormater(currentIndex, length));

				String keyword = SqlExpressionFormattingHelper.NormalizeKeyword(delimiter.Groups["Keyword"].Value);

				AppendLiteral(keyword, true);
				currentIndex = position + delimiter.Length;
			}

			if (currentIndex < this.Length)
			{
				AppendComposite(CreateAssociateTableFormater(currentIndex, (this.Length - currentIndex)));
			}
		}

		#region ��������

		/// <summary>
		/// ������������ INNER JOIN �ȷָ�����ʽ������
		/// </summary>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>��ʽ������</returns>
		private SqlExpressionFormatter CreateAssociateTableFormater(Int32 startIndex, Int32 length)
		{
			return new SqlAssociateTableExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		#endregion
	}
}