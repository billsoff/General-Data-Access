#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����LikeFilter.cs
// �ļ�������������ʾ LIKE ����������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20100815
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
	/// ��ʾ LIKE ����������
	/// </summary>
	[Serializable]
	public class LikeFilter : Filter
	{
		#region ˽���ֶ�

		private readonly Char m_escapeChar;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯���������������ƺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		public LikeFilter(String propertyName, String patternText)
			: base(propertyName, new Object[] { patternText })
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ơ�ʵ����ֵ�������ƺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		public LikeFilter(String entityPropertyName, String propertyName, String patternText)
			: base(entityPropertyName, propertyName, new Object[] { patternText })
		{
		}

		/// <summary>
		/// ���캯������������·����ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		public LikeFilter(IList<String> propertyPath, String patternText)
			: base(propertyPath, new Object[] { patternText })
		{
		}

		/// <summary>
		/// ���캯���������������ƺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		public LikeFilter(String propertyName, String patternText, Char escapeChar)
			: base(propertyName, new Object[] { patternText })
		{
			m_escapeChar = escapeChar;
		}

		/// <summary>
		/// ���캯��������ʵ���������ơ�ʵ����ֵ�������ƺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		public LikeFilter(String entityPropertyName, String propertyName, String patternText, Char escapeChar)
			: base(entityPropertyName, propertyName, new Object[] { patternText })
		{
			m_escapeChar = escapeChar;
		}

		/// <summary>
		/// ���캯������������·����ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		public LikeFilter(IList<String> propertyPath, String patternText, Char escapeChar)
			: base(propertyPath, new Object[] { patternText })
		{
			m_escapeChar = escapeChar;
		}

		#endregion

		/// <summary>
		/// �����뻺����д���������ʽ��
		/// </summary>
		/// <param name="output">���뻺������</param>
		public override void Generate(StringBuilder output)
		{
			ThrowExceptionIfNotCompiled();

			output.Append(ColumnFullName);

			if (!Negative)
			{
				output.Append(LIKE);
			}
			else
			{
				output.Append(NOT_LIKE);
			}

			output.Append(ComposeParameter(Parameters[0].Name));

			if (m_escapeChar != 0)
			{
				output.AppendFormat(" ESCAPE '{0}'", m_escapeChar);
			}
		}
	}
}