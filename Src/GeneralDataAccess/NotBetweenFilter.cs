#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NotBetweenFilter.cs
// �ļ�������������ʾ NOT BETWEEN ���������ʽ��
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
	/// ��ʾ NOT BETWEEN ���������ʽ��
	/// </summary>
	[Serializable]
	public class NotBetweenFilter : Filter
	{
		#region ���캯��

		/// <summary>
		/// ���캯������������ֵ�����ұ߽������ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		public NotBetweenFilter(String propertyName, Object from, Object to)
			: base(propertyName, new Object[] { from, to })
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ơ�ֵ�������ƺ����ұ߽������ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		public NotBetweenFilter(String entityPropertyName, String propertyName, Object from, Object to)
			: base(entityPropertyName, propertyName, new Object[] { from, to })
		{
		}

		/// <summary>
		/// ���캯������������·����ֵ�������ƺ����ұ߽������ֵ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		public NotBetweenFilter(IList<String> propertyPath, Object from, Object to)
			: base(propertyPath, new Object[] { from, to })
		{
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
				output.Append(NOT_BETWEEN);
			}
			else
			{
				output.Append(BETWEEN);
			}

			output.Append(ComposeParameter(Parameters[0].Name));

			output.Append(AND);

			output.Append(ComposeParameter(Parameters[1].Name));
		}
	}
}