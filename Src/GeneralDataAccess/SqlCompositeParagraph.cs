#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlCompositeParagraph.cs
// �ļ�����������SQL ���϶��䣬����һ����ʽ������
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
	internal sealed class SqlCompositeParagraph : SqlParagraph
	{
		#region ˽���ֶ�

		private readonly SqlExpressionFormatter m_formatter;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø�ʽ��������
		/// </summary>
		/// <param name="formatter">��ʽ������</param>
		public SqlCompositeParagraph(SqlExpressionFormatter formatter)
		{
			m_formatter = formatter;
		}

		#endregion

		/// <summary>
		/// ʹ�����õĸ�ʽ�����������
		/// </summary>
		/// <returns></returns>
		protected override String Output()
		{
			return m_formatter.ToString();
		}
	}
}