#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterCompilationContext.cs
// �ļ�������������ʾ�ṩ������������Ļ�������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110706
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾ�ṩ������������Ļ�������
	/// </summary>
	public abstract class FilterCompilationContext
	{
		#region ˽���ֶ�

		private readonly IColumnLocatable m_schema;
		private readonly String m_parameterPrefix;
		private readonly List<QueryParameter> m_parameters = new List<QueryParameter>();
		private Int32 m_index;
		private Int32 m_indexOffset;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		protected FilterCompilationContext()
		{
		}

		/// <summary>
		/// ���캯��������ʵ��ܹ���
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		protected FilterCompilationContext(IColumnLocatable schema)
		{
			m_schema = schema;
		}

		/// <summary>
		/// ���캯��������ʵ��ܹ��Ͳ���ǰ׺��
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <param name="parameterPrifix">����ǰ׺��</param>
		protected FilterCompilationContext(IColumnLocatable schema, String parameterPrifix)
			: this(schema)
		{
			m_parameterPrefix = parameterPrifix;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ƫ�ơ�
		/// </summary>
		public Int32 IndexOffset
		{
			get { return m_indexOffset; }
			internal set { m_indexOffset = value; }
		}

		/// <summary>
		/// ��ȡ���еĲ�ѯ������
		/// </summary>
		public virtual QueryParameter[] Parameters
		{
			get { return m_parameters.ToArray(); }
		}

		/// <summary>
		/// ��ȡ����ǰ׺��
		/// </summary>
		public virtual String ParameterPrefix
		{
			get
			{
				if (!String.IsNullOrEmpty(m_parameterPrefix))
				{
					return m_parameterPrefix;
				}
				else
				{
					return Filter.PARAMETER_PREFIX;
				}
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// �����ж�λ�����ɲ�ѯ������
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <param name="paramValues">����ֵ���ϡ�</param>
		/// <returns>���ɵĲ�ѯ�������ϡ�</returns>
		public virtual QueryParameter[] GenerateParameters(ColumnLocator colLocator, Object[] paramValues)
		{
			Column[] allColumns = m_schema[colLocator];

			List<QueryParameter> parameters = new List<QueryParameter>();

			foreach (Object value in paramValues)
			{
				String paramName = CreateParameterName();
				DbType dbType = allColumns[0].DbType;

				Object paramValue = value;

				if (value == null)
				{
					paramValue = DBNull.Value;
				}

				QueryParameter parameter = GenerateQueryParameter(paramName, dbType, paramValue);

				parameters.Add(parameter);
			}

			return parameters.ToArray();
		}

		/// <summary>
		/// �����ж�λ����ȡ�е�ȫ���ơ�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>���е�ȫ���ơ�</returns>
		public virtual String GetColumnFullName(ColumnLocator colLocator)
		{
			Column[] cols = m_schema[colLocator];

			return cols[0].Expression;
		}

		/// <summary>
		/// �����ж�λ���Ͳ�ѯ����ֵ���ϻ�ȡʵ���ѯ�������ϡ�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <param name="paramValues">����ֵ���ϡ�</param>
		/// <returns>ʵ��������ϣ����е�һ��Ԫ������ʵ���е�ȫ���Ƽ���һ��һ��ӳ��ġ�</returns>
		public virtual QueryParameterCollection[] GenerateEntityParameters(ColumnLocator colLocator, Object[] paramValues)
		{
			Column[] allColumns = m_schema[colLocator];

			List<QueryParameterCollection> entityParameters = new List<QueryParameterCollection>();

			foreach (Object value in paramValues)
			{
				QueryParameter[] parameters = new QueryParameter[allColumns.Length];

				for (Int32 i = 0; i < allColumns.Length; i++)
				{
					Column col = allColumns[i];

					String paramName = CreateParameterName();

					Object paramValue = col.GetDbValue(value, true);

					parameters[i] = GenerateQueryParameter(paramName, col.DbType, paramValue);
				}

				entityParameters.Add(new QueryParameterCollection(parameters));
			}

			return entityParameters.ToArray();
		}

		/// <summary>
		/// �����ж�λ����ȡʵ���е�ȫ���Ƽ��ϡ�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>ʵ���е�ȫ���Ƽ��ϡ�</returns>
		public virtual String[] GetEntityColumnFullNames(ColumnLocator colLocator)
		{
			Column[] allColumns = m_schema[colLocator];

			String[] entityColumnFullNames = Array.ConvertAll<Column, String>(
					allColumns,
					delegate(Column col) { return col.Expression; }
				);

			return entityColumnFullNames;
		}

		/// <summary>
		/// �ж��ж�λ���Ƿ�Ϊʵ���С�
		/// </summary>
		/// <param name="colLocator">�ж�λ����</param>
		/// <returns>����ж�λ��ָ��ʵ�����ԣ�����Ϊʵ���У����� true�����򣬷��� false��</returns>
		public virtual Boolean IsEntityColumns(ColumnLocator colLocator)
		{
			Column[] allColumns = m_schema[colLocator];

			return !allColumns[0].IsPrimitive;
		}

		/// <summary>
		/// ע���ѯ���������ڽ��Ӳ�ѯ�Ĳ������ӵ���ǰ�Ļ����С�
		/// </summary>
		/// <param name="parameters">Ҫע��Ĳ����б�</param>
		/// <returns>��ע��Ĳ����б�</returns>
		public QueryParameter[] RegisterParameters(QueryParameter[] parameters)
		{
			#region ǰ������

			Debug.Assert(parameters != null, "���� parameters ����Ϊ�ա�");

			#endregion

			QueryParameter[] registeredParameters = Array.ConvertAll<QueryParameter, QueryParameter>(
					parameters,
					delegate(QueryParameter parm)
					{
						return parm.Clone(CreateParameterName());
					}
				);

			m_parameters.AddRange(registeredParameters);

			return registeredParameters;
		}

		#endregion

		#region �����Ա

		#region ����������

		/// <summary>
		/// ��ȡ�������Ƶ�ǰ׺��
		/// </summary>
		protected abstract String ParameterNamePrefix { get; }

		#endregion

		#endregion

		#region �����ķ���

		/// <summary>
		/// �����������ơ�
		/// </summary>
		/// <returns>�����õĲ������ơ�</returns>
		protected String CreateParameterName()
		{
			m_index++;

			String name = ParameterNamePrefix + (m_index + m_indexOffset).ToString();

			return name;
		}

		/// <summary>
		/// ���ɲ�ѯ������������������С�
		/// </summary>
		/// <param name="paramName">�������ơ�</param>
		/// <param name="dbType">�������ݿ����͡�</param>
		/// <param name="paramValue">����ֵ��</param>
		/// <returns>���ɺõĲ�����</returns>
		protected QueryParameter GenerateQueryParameter(String paramName, DbType dbType, Object paramValue)
		{
			QueryParameter parameter = new QueryParameter(paramName, dbType, paramValue);

			m_parameters.Add(parameter);

			return parameter;
		}

		#endregion
	}
}