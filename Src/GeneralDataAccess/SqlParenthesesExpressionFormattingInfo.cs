#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlParenthesesExpressionFormattingInfo.cs
// �ļ����������������������ű��ʽ��ʽ��Ϣ��
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
	/// �����������ű��ʽ��ʽ��Ϣ��
	/// </summary>
	internal class SqlParenthesesExpressionFormattingInfo
	{
		#region ˽���ֶ�

		private Boolean m_outerOpenParenthesisStartsNewLine;
		private SqlExpressionFormatter m_formatter;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public SqlParenthesesExpressionFormattingInfo()
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾ�������Ƿ�λ�����У�Ĭ��Ϊ false��
		/// </summary>
		public Boolean OuterOpenParenthesisStartsAtNewLine
		{
			get { return m_outerOpenParenthesisStartsNewLine; }
			set { m_outerOpenParenthesisStartsNewLine = value; }
		}

		/// <summary>
		/// ��ȡ�����öԱ��ʽ�ĸ�ʽ���������Ϊ null�����ｫԭ�������
		/// </summary>
		internal SqlExpressionFormatter Formatter
		{
			get { return m_formatter; }
			set { m_formatter = value; }
		}

		#endregion
	}
}