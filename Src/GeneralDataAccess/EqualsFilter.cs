#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EqualsFilter.cs
// �ļ�������������ʾ���������������֧��ʵ����ˡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008151436
//
// �޸ı�ʶ���α� 201009030850
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
	/// ��ʾ���������������֧��ʵ����ˡ�
	/// </summary>
	[Serializable]
	public class EqualsFilter : Filter
	{
		#region ���캯��

		/// <summary>
		/// ���캯���������������ƺ�����ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		public EqualsFilter(String propertyName, Object propertyValue)
			: base(propertyName, new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		public EqualsFilter(String entityPropertyName, String propertyName, Object propertyValue)
			: base(entityPropertyName, propertyName, new Object[] { propertyValue })
		{
		}

		/// <summary>
		/// ���캯������������·��������ֵ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="propertyValue">����ֵ��</param>
		public EqualsFilter(IList<String> propertyPath, Object propertyValue)
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
		/// ����ֵ���˱��ʽ��
		/// </summary>
		/// <param name="output">���뻺������</param>
		private void GenerateValueFilterExpression(StringBuilder output)
		{
			output.Append(ColumnFullName);

			if (!Negative)
			{
				output.Append("=");
			}
			else
			{
				output.Append("<>");
			}

			output.Append(ComposeParameter(Parameters[0].Name));
		}

		/// <summary>
		/// ����ʵ����˱��ʽ��
		/// </summary>
		/// <param name="output">���뻺������</param>
		private void GenerateEntityFilterExpression(StringBuilder output)
		{
			String compareOperator = (!Negative ? "=" : "<>");

			List<String> expressions = new List<String>();

			QueryParameterCollection parameters = EntityParameters[0];

			for (Int32 i = 0; i < EntityColumnFullNames.Length; i++)
			{
				String columnFullName = EntityColumnFullNames[i];
				String paramName = ComposeParameter(parameters[i].Name);

				expressions.Add(String.Format("{0}{1}{2}", columnFullName, compareOperator, paramName));
			}

			if (expressions.Count == 1)
			{
				output.Append(expressions[0]);
			}
			else
			{
				for (Int32 i = 0; i < expressions.Count; i++)
				{
					if (i != 0)
					{
						output.Append(AND);
					}

					output.AppendFormat("{0}{1}{2}", LEFT_BRACKET, expressions[i], RIGHT_BRACKET);
				}
			}
		}
	}
}