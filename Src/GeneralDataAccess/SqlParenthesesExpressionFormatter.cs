#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlParenthesesExpressionFormatter.cs
// �ļ��������������ű��ʽ��ʽ������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110715
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
	/// ���ű��ʽ��ʽ���������ܵ������������ű��ʽ�������ڲ���ʽ���и�ʽ����
	/// </summary>
	internal sealed class SqlParenthesesExpressionFormatter : SqlExpressionFormatter
	{
		#region ˽���ֶ�

		private readonly Boolean m_isBlock;
		private readonly SqlExpressionFormatter m_formatter;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ��ʽ���� SQL ָ��ȡ�
		/// </summary>
		/// <param name="sqlExpression">SQL ָ�</param>
		/// <param name="startIndex">��ʼ������</param>
		/// <param name="length">���ȡ�</param>
		/// <param name="indent">������</param>
		/// <param name="level">����</param>
		/// <param name="evaluator">ί�У����ڻ�ȡ�����ű��ʽ���и�ʽ����ָʾ��</param>
		public SqlParenthesesExpressionFormatter(String sqlExpression, Int32 startIndex, Int32 length, String indent, Int32 level, SqlParenthesesExpressionEvaluator evaluator)
			: base(sqlExpression, startIndex, length, indent, level)
		{
			// �����ڵı��ʽ
			Int32 expressionStartIndex = 1; // �������ź�ʼ
			Int32 expressionLength = this.Length - 2; // ��ȥ��������

			SqlParenthesesExpressionFormattingInfo formattingInfo = new SqlParenthesesExpressionFormattingInfo();

			if (evaluator != null)
			{
				evaluator(SqlExpression, (this.StartIndex + expressionStartIndex), expressionLength, Indent, Level, formattingInfo);
			}

			m_isBlock = formattingInfo.OuterOpenParenthesisStartsAtNewLine;

			if (formattingInfo.Formatter != null)
			{
				m_formatter = formattingInfo.Formatter;
			}
			else
			{
				m_formatter = CreateLiteralFormatter(expressionStartIndex, expressionLength);
			}
		}

		#endregion

		/// <summary>
		/// �ж��Ƿ�Ϊ�������
		/// </summary>
		public override Boolean IsBlock
		{
			get { return m_isBlock; }
		}

		/// <summary>
		/// ��ʽ����
		/// </summary>
		protected override void Format()
		{
			AppendOpenParenthesis(true);
			AppendComposite(m_formatter);
			AppendCloseParenthesis(m_formatter.IsBlock);
		}
	}
}