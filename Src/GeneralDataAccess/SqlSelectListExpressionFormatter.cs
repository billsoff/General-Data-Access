#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlSelectListExpressionFormatter.cs
// �ļ�����������SELECT ָ��ѡ���б��ʽ������
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
	/// SELECT ָ��ѡ���б��ʽ������
	/// </summary>
	internal sealed class SqlSelectListExpressionFormatter : SqlCommaDelimitingListExpressionFormatter
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
		public SqlSelectListExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
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
				FieldItem field = allFields[i];

				if (i != 0)
				{
					AppendComma();
				}

				AppendNewLine();
				AppendIndent();
				AppendComposite(CreateSelectFieldFormatter(field.Index, field.Length));
			}
		}

		#region ��������

		/// <summary>
		/// ����ѡ���ֶθ�ʽ������
		/// </summary>
		/// <param name="startIndex">��Ҫ��ʽ���ı��е���ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <returns>��ʽ������</returns>
		private SqlExpressionFormatter CreateSelectFieldFormatter(Int32 startIndex, Int32 length)
		{
			return new SqlSelectFieldExpressionFormatter(SqlExpression, (this.StartIndex + startIndex), length, Indent, Level);
		}

		#endregion
	}
}