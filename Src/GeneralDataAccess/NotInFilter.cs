#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NotInFilter.cs
// �ļ�������������ʾ NOT IN ����������֧��ʵ����ˡ�
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
	/// ��ʾ NOT IN ����������֧��ʵ����ˡ�
	/// </summary>
	[Serializable]
	public class NotInFilter : Filter
	{
		#region ���캯��

		/// <summary>
		/// ���캯���������������ƺ�ֵ���ϡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="discreteValues">����ֵ���ϡ�</param>
		public NotInFilter(String propertyName, Object[] discreteValues)
			: base(propertyName, discreteValues)
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ơ�ֵ�������ƺ�ֵ���ϡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="discreteValues">ֵ���ϡ�</param>
		public NotInFilter(String entityPropertyName, String propertyName, Object[] discreteValues)
			: base(entityPropertyName, propertyName, discreteValues)
		{
		}

		/// <summary>
		/// ���캯������������·����ֵ���ϡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="discreteValues">ֵ���ϡ�</param>
		public NotInFilter(IList<String> propertyPath, Object[] discreteValues)
			: base(propertyPath, discreteValues)
		{
		}

		/// <summary>
		/// ���캯���������������ƺ�ֵ���ϡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="builder">��ѯ�б���������</param>
		internal NotInFilter(String propertyName, QueryListBuilder builder)
			: base(propertyName, builder)
		{
		}

		/// <summary>
		/// ���캯��������ʵ���������ơ�ֵ�������ƺ�ֵ���ϡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="builder">��ѯ�б���������</param>
		internal NotInFilter(String entityPropertyName, String propertyName, QueryListBuilder builder)
			: base(entityPropertyName, propertyName, builder)
		{
		}

		/// <summary>
		/// ���캯������������·����ֵ���ϡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="builder">��ѯ�б���������</param>
		internal NotInFilter(IList<String> propertyPath, QueryListBuilder builder)
			: base(propertyPath, builder)
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

			if (QueryListBuilder != null)
			{
				GenerateQueryFilterExpression(output);
			}
			else if ((!IsEntityFilter && (Parameters.Length == 0)) || (IsEntityFilter && (EntityParameters.Length == 0)))
			{
				if (!Negative)
				{
					output.Append("1=1");
				}
				else
				{
					output.Append("1<>1");
				}
			}
			else if (!IsEntityFilter)
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
				output.Append(NOT_IN);
			}
			else
			{
				output.Append(IN);
			}

			output.Append(LEFT_BRACKET);

			for (int i = 0; i < Parameters.Length; i++)
			{
				if (i > 0)
				{
					output.Append(COMMA);
				}

				output.Append(ComposeParameter(Parameters[i].Name));
			}

			output.Append(RIGHT_BRACKET);
		}

		/// <summary>
		/// ����ʵ����˱��ʽ��
		/// </summary>
		/// <param name="output">���뻺������</param>
		private void GenerateEntityFilterExpression(StringBuilder output)
		{
			if (EntityColumnFullNames.Length > 1)
			{
				GenerateComproundColumnEntityFilterExpression(output);
			}
			else
			{
				GenerateSingleColumnEntityFilterExpression(output);
			}
		}

		/// <summary>
		/// ����������ʵ����˱��ʽ��
		/// </summary>
		/// <param name="output">�����������</param>
		private void GenerateComproundColumnEntityFilterExpression(StringBuilder output)
		{
			List<String> expressions = new List<String>();

			String compareOperator = (!Negative ? "<>" : "=");

			foreach (QueryParameterCollection parameters in EntityParameters)
			{
				expressions.Add(ComposeEntityFilterExpression(parameters, compareOperator));
			}

			if (expressions.Count == 1)
			{
				output.Append(expressions[0]);
			}
			else if (!Negative)
			{
				// �� AND ����
				for (Int32 i = 0; i < expressions.Count; i++)
				{
					if (i != 0)
					{
						output.Append(AND);
					}

					output.Append(expressions[i]);
				}
			}
			else
			{
				// �� OR ����
				for (Int32 i = 0; i < expressions.Count; i++)
				{
					if (i != 0)
					{
						output.Append(OR);
					}

					output.AppendFormat("{0}{1}{2}", LEFT_BRACKET, expressions[i], RIGHT_BRACKET);
				}
			}
		}

		/// <summary>
		/// ������һ��ʵ����˱��ʽ��
		/// </summary>
		/// <param name="output">�����������</param>
		private void GenerateSingleColumnEntityFilterExpression(StringBuilder output)
		{
			output.Append(this.EntityColumnFullNames[0]);

			if (!Negative)
			{
				output.Append(NOT_IN);
			}
			else
			{
				output.Append(IN);
			}

			output.Append(LEFT_BRACKET);

			for (Int32 i = 0; i < EntityParameters.Length; i++)
			{
				if (i > 0)
				{
					output.Append(COMMA);
				}

				output.Append(ComposeParameter(EntityParameters[i][0].Name));
			}

			output.Append(RIGHT_BRACKET);
		}

		/// <summary>
		/// ���ɲ�ѯ�б���������ʽ��
		/// </summary>
		/// <param name="output">�����������</param>
		private void GenerateQueryFilterExpression(StringBuilder output)
		{
			String columnName;

			if (IsEntityFilter)
			{
				columnName = this.EntityColumnFullNames[0];
			}
			else
			{
				columnName = this.ColumnFullName;
			}

			output.Append(columnName);

			if (!Negative)
			{
				output.Append(NOT_IN);
			}
			else
			{
				output.Append(IN);
			}

			output.Append(LEFT_BRACKET);
			output.Append(QueryListSqlStatement);
			output.Append(RIGHT_BRACKET);
		}

		/// <summary>
		/// �ϳ�ʵ����˱��ʽ��
		/// </summary>
		/// <param name="parameters">ʵ���еĲ������ϡ�</param>
		/// <param name="compareOperator">�Ƚϲ�������</param>
		/// <returns>�ϳɺõĹ��˱��ʽ��</returns>
		private String ComposeEntityFilterExpression(QueryParameterCollection parameters, String compareOperator)
		{
			List<String> expressions = new List<String>();

			for (Int32 i = 0; i < EntityColumnFullNames.Length; i++)
			{
				String columnFullName = EntityColumnFullNames[i];
				String paramName = ComposeParameter(parameters[i].Name);

				expressions.Add(String.Format("{0}{1}{2}", columnFullName, compareOperator, paramName));
			}

			if (expressions.Count == 1)
			{
				return expressions[0];
			}
			else
			{
				StringBuilder buffer = new StringBuilder();

				foreach (String e in expressions)
				{
					if (buffer.Length != 0)
					{
						buffer.Append(AND);
					}

					buffer.AppendFormat("{0}{1}{2}", LEFT_BRACKET, e, RIGHT_BRACKET);
				}

				return buffer.ToString();
			}
		}
	}
}