#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlParenthesesExpressionEvaluator.cs
// �ļ���������������ָʾ��ʽ�����ű��ʽ�ķ�����
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
	/// ����ָʾ��ʽ�����ű��ʽ�ķ�����
	/// </summary>
	/// <param name="sqlExpression">SQL ָ�</param>
	/// <param name="startIndex">Ҫ����Ŀ�ʼ������</param>
	/// <param name="length">Ҫ����ĳ��ȡ�</param>
	/// <param name="indent">������</param>
	/// <param name="level">��ǰ����</param>
	/// <param name="formattingInfo">��ʽ����Ϣ����������ָʾ��θ�ʽ�����ű�����Ϣ��</param>
	internal delegate void SqlParenthesesExpressionEvaluator(
			String sqlExpression,
			Int32 startIndex,
			Int32 length,
			String indent,
			Int32 level,
			SqlParenthesesExpressionFormattingInfo formattingInfo
		);
}