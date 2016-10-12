#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IsNullFilter.cs
// �ļ���������������Ϊ��ֵ�Ĺ�������֧��ʵ����ˡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20100815
//
// �޸ı�ʶ���α� 20100903
// �޸������������˶�ʵ����˵�֧�֡�
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����Ϊ��ֵ�Ĺ�������
	/// </summary>
	[Serializable]
	public class IsNullFilter : Filter
	{
		#region ���캯��

		/// <summary>
		/// ���캯���������������ƺ�����ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		public IsNullFilter(String propertyName)
			: base(propertyName)
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		public IsNullFilter(String entityPropertyName, String propertyName)
			: base(entityPropertyName, propertyName)
		{
		}

		/// <summary>
		/// ���캯������������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		public IsNullFilter(IList<String> propertyPath)
			: base(propertyPath)
		{
		}

		#endregion

		/// <summary>
		/// �����뻺����д����������ʽ��
		/// </summary>
		/// <param name="output">���뻺������</param>
		public override void Generate(StringBuilder output)
		{
			ThrowExceptionIfNotCompiled();

			if (!IsEntityFilter)
			{
				GenerateValueFilterExpression(output);
			}
			else
			{
				GenerateEntityFilterExpression(output);
			}
		}

		/// <summary>
		/// ����ֵ���˱���ʽ��
		/// </summary>
		/// <param name="output">���뻺������</param>
		private void GenerateValueFilterExpression(StringBuilder output)
		{
			if (!Negative)
			{
				output.AppendFormat("{0} IS NULL", ColumnFullName);
			}
			else
			{
				output.AppendFormat("{0} IS NOT NULL", ColumnFullName);
			}
		}

		/// <summary>
		/// ����ʵ����˱���ʽ��
		/// </summary>
		/// <param name="output">���뻺������</param>
		private void GenerateEntityFilterExpression(StringBuilder output)
		{
			// ȡʵ���еĵ�һ����
			String columnName = EntityColumnFullNames[0];

			if (!Negative)
			{
				output.AppendFormat("{0} IS NULL", columnName);
			}
			else
			{
				output.AppendFormat("{0} IS NOT NULL", columnName);
			}
		}
	}
}