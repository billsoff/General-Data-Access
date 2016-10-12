#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlSimpleCommaDelimitingListExpressionFormatter.cs
// �ļ��������������ö��ŷָ�������м򵥵����У����ٽ�һ����ʽ����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110718
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
	/// ���ö��ŷָ�������м򵥵����У����ٽ�һ����ʽ����
	/// </summary>
	internal sealed class SqlSimpleCommaDelimitingListExpressionFormatter : SqlCommaDelimitingListExpressionFormatter
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
		public SqlSimpleCommaDelimitingListExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
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

				AppendLiteral(FormatOperators(allFields[i].Text), true);
			}
		}
	}
}