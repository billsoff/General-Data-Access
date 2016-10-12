#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlInlineExpressionFormatter.cs
// �ļ�������������������ĸ�ʽ������
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
	/// ��������ĸ�ʽ������
	/// </summary>
	internal abstract class SqlInlineExpressionFormatter : SqlExpressionFormatter
	{
		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����������������Ρ�
		/// </summary>
		protected SqlInlineExpressionFormatter()
		{
		}

		/// <summary>
		/// ���캯��������Ҫ��ʽ���� SQL ָ��ȡ�
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		protected SqlInlineExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level)
			: base(sqlExpression, startIndex, length, indent, level)
		{
		}

		#endregion

		/// <summary>
		/// ���Ƿ��� false��
		/// </summary>
		public override Boolean IsBlock
		{
			get { return false; }
		}
	}
}