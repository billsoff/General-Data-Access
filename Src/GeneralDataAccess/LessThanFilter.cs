#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����LessThanFilter.cs
// �ļ�������������ʾС�ڹ���������
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
	/// ��ʾС�ڹ���������
	/// </summary>
	[Serializable]
	public class LessThanFilter : Filter
	{
		#region ���캯��

		/// <summary>
		/// ���캯���������������ƺ�����ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		public LessThanFilter(String propertyName, Object propertyValue)
			: base(propertyName, new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		public LessThanFilter(String entityPropertyName, String propertyName, Object propertyValue)
			: base(entityPropertyName, propertyName, new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// ���캯������������·��������ֵ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="propertyValue">����ֵ��</param>
		public LessThanFilter(IList<String> propertyPath, Object propertyValue)
			: base(propertyPath, new Object[] { propertyValue })
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
				output.Append("<");
			}
			else
			{
				output.Append(">=");
			}

			output.Append(ComposeParameter(Parameters[0].Name));
		}
	}
}