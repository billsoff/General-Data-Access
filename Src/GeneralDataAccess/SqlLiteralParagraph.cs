#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SqlLiteralParagraph.cs
// �ļ�������������ʾ��������� SQL ���䡣
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
	/// ��ʾ��������� SQL ���䡣
	/// </summary>
	internal sealed class SqlLiteralParagraph : SqlParagraph
	{
		#region ˽���ֶ�

		private readonly String m_text;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ������ı���
		/// </summary>
		/// <param name="text">�ı���</param>
		public SqlLiteralParagraph(String text)
		{
			m_text = text;
		}

		#endregion

		/// <summary>
		/// ��������ı���
		/// </summary>
		/// <returns>ԭ�������</returns>
		protected override String Output()
		{
			return m_text;
		}
	}
}