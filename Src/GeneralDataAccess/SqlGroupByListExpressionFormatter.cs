#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlGroupByListExpressionFormatter.cs
// �ļ����������������б��ʽ������
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// �����б��ʽ������
	/// </summary>
	internal sealed class SqlGroupByListExpressionFormatter : SqlCommaDelimitingListExpressionFormatter
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
		public SqlGroupByListExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// ��ʽ����
		/// </summary>
		protected override void Format()
		{
			FieldItem[] allFields = GetAllFieldItems();

			for (Int32 i = 0; i < allFields.Length; i++)
			{
				if (i != 0)
				{
					AppendComma();
				}

				FieldItem field = allFields[i];

				AppendNewLine();
				AppendIndent();
				AppendLiteral(field.Text.Trim());
			}
		}
	}
}