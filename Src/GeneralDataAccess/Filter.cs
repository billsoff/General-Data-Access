#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����Filter.cs
// �ļ�������������ʾ��������
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008151129
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾ��������
	/// </summary>
	[Serializable]
	public abstract class Filter : ICloneable
	{
		#region ��̬��Ա

		/// <summary>
		/// ���������������������ʽ�Ͳ�ѯ������
		/// </summary>
		/// <param name="context">���뻷����</param>
		/// <param name="filter">��������</param>
		/// <returns>���������������󣨷�װ���������ʽ�Ͳ�ѯ�������ϣ���</returns>
		public static FilterCompilationResult Compile(FilterCompilationContext context, Filter filter)
		{
			if (filter == null)
			{
				return null;
			}

			filter.Compile(context);

			StringBuilder output = new StringBuilder();

			filter.Generate(output);

			String whereClause = output.ToString();
			QueryParameter[] parameters = context.Parameters;

			FilterCompilationResult result = new FilterCompilationResult(whereClause, parameters);

			return result;
		}

		/// <summary>
		/// ת��ģ��ƥ���ı����ڹ���ƥ��ģʽǰ����ʹ�÷�б�ܣ�\����ת�����ͨ�������Ϊ��_ %��
		/// </summary>
		/// <param name="text">�ı���</param>
		/// <returns>ͨ�����ת����ı���</returns>
		public static String EscapeFuzzyText(String text)
		{
			return EscapeFuzzyText(text, DEFAULT_FUZZY_ESCAPE_CHAR);
		}

		/// <summary>
		/// ת��ģ��ƥ���ı����ڹ���ƥ��ģʽǰ����ͨ�������Ϊ��_ %��
		/// </summary>
		/// <param name="text">�ı���</param>
		/// <param name="escpaeChar"></param>
		/// <returns>ͨ�����ת����ı���</returns>
		public static String EscapeFuzzyText(String text, Char escpaeChar)
		{
			return EscapeFuzzyText(text, escpaeChar, new Char[] { '%', '_' });
		}

		/// <summary>
		/// ת��ģ��ƥ���ı����ڹ���ƥ��ģʽǰ����
		/// </summary>
		/// <param name="text">�ı���</param>
		/// <param name="escapeChar">ת���ַ�������Ϊ nil��</param>
		/// <param name="wildcards">ͨ������ϡ�</param>
		/// <returns>�������ת����һ���ַ����򷵻� true�����򷵻� false��</returns>
		public static String EscapeFuzzyText(String text, Char escapeChar, Char[] wildcards)
		{
			Boolean hasWildcards;

			return EscapeFuzzyText(text, escapeChar, wildcards, out hasWildcards);
		}

		/// <summary>
		/// ת��ģ��ƥ���ı����ڹ���ƥ��ģʽǰ����
		/// </summary>
		/// <param name="text">�ı���</param>
		/// <param name="escapeChar">ת���ַ�������Ϊ nil��</param>
		/// <param name="wildcards">ͨ������ϡ�</param>
		/// <param name="hasWildcards">����ֵ��ָʾ�ı����Ƿ����ͨ�����</param>
		/// <returns>ͨ�����ת����ı���</returns>
		public static String EscapeFuzzyText(String text, Char escapeChar, Char[] wildcards, out Boolean hasWildcards)
		{
			if (String.IsNullOrEmpty(text))
			{
				hasWildcards = false;

				return text;
			}

			#region ǰ�ö���

			Debug.Assert(escapeChar != 0, "ģ��ƥ����ʽ��ת���ַ�����Ϊ nil��");
			Debug.Assert((wildcards != null) && (wildcards.Length != 0), "ͨ����ַ�������Ϊ�ջ���б�");

			#endregion

			StringBuilder buffer = new StringBuilder();

			foreach (Char c in wildcards)
			{
				buffer.Append(Regex.Escape(c.ToString()));
			}

			buffer.Append(Regex.Escape(escapeChar.ToString()));

			buffer.Insert(0, "[");
			buffer.Append("]");

			String pattern = buffer.ToString();

			Regex ex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			String escapeText = ex.Replace(
					text,
					delegate(Match m)
					{
						return escapeChar + m.Value;
					}
				);

			hasWildcards = ex.IsMatch(text);

			return escapeText;
		}

		#region ��������

		#region ͨ�÷���

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="info">����������</param>
		/// <returns>�����õĹ�������</returns>
		public static Filter Create(String propertyName, FilterInfo info)
		{
			return Create(null, propertyName, info);
		}

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="entityPropertyName">�ⲿ����ʵ���������ơ�</param>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="info">����������</param>
		/// <returns>�����õĹ�������</returns>
		public static Filter Create(String entityPropertyName, String propertyName, FilterInfo info)
		{
			ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);

			FilterFactory factory = new FilterFactory(colLocator.PropertyPath, info);

			Filter result = factory.Filter;

			return result;
		}

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="chain">��������</param>
		/// <param name="info">����������Ϣ���Ƚϲ�����Ƚ�ֵ����</param>
		/// <returns>�����õĹ�������</returns>
		public static Filter Create(IPropertyChain chain, FilterInfo info)
		{
			return Create(chain.PropertyPath, info);
		}

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <param name="info">����������Ϣ���Ƚϲ�����Ƚ�ֵ����</param>
		/// <returns>�����õĹ�������</returns>
		public static Filter Create(IPropertyChainBuilder builder, FilterInfo info)
		{
			return Create(builder.Build(), info);
		}

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="info">����������Ϣ���Ƚϲ�����Ƚ�ֵ����</param>
		/// <returns>�����õĹ�������</returns>
		public static Filter Create(IList<String> propertyPath, FilterInfo info)
		{
			FilterFactory factory = new FilterFactory(propertyPath, info);

			Filter result = factory.Filter;

			return result;
		}

		#endregion

		#region EqualsFilter

		/// <summary>
		/// ������ȹ������������������ƺ�����ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ���ȹ�������</returns>
		public static Filter CreateEqualsFilter(String propertyName, Object propertyValue)
		{
			return new EqualsFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// ������ȹ�����������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ���ȹ�������</returns>
		public static Filter CreateEqualsFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new EqualsFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// ������ȹ���������������·��������ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ���ȹ�������</returns>
		public static Filter CreateEqualsFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new EqualsFilter(propertyPath, propertyValue);
		}

		#endregion

		#region NotEqualFilter

		/// <summary>
		/// ��������ȹ������������������ƺ�����ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĲ���ȹ�����</returns>
		public static Filter CreateNotEqualFilter(String propertyName, Object propertyValue)
		{
			return new NotEqualFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// ��������ȹ�����������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĲ���ȹ�����</returns>
		public static Filter CreateNotEqualFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new NotEqualFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// ��������ȹ���������������·��������ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĲ���ȹ�����</returns>
		public static Filter CreateNotEqualFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new NotEqualFilter(propertyPath, propertyValue);
		}

		#endregion

		#region IsNullFilter

		/// <summary>
		/// �����ֶ�Ϊ�գ�IS NULL���������������������ƺ�����ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>�����õ��ֶ�Ϊ�գ�IS NULL����������</returns>
		public static Filter CreateIsNullFilter(String propertyName)
		{
			return new IsNullFilter(propertyName);
		}

		/// <summary>
		/// �����ֶ�Ϊ�գ�IS NULL��������������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <returns>�����õ��ֶ�Ϊ�գ�IS NULL����������</returns>
		public static Filter CreateIsNullFilter(String entityPropertyName, String propertyName)
		{
			return new IsNullFilter(entityPropertyName, propertyName);
		}

		/// <summary>
		/// �����ֶ�Ϊ�գ�IS NULL��������������ʵ������·��������ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>�����õ��ֶ�Ϊ�գ�IS NULL����������</returns>
		public static Filter CreateIsNullFilter(IList<String> propertyPath)
		{
			return new IsNullFilter(propertyPath);
		}

		#endregion

		#region IsNotNullFilter

		/// <summary>
		/// �����ǿչ���������IS NOT NULL���������������ƺ�����ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>�����õķǿչ���������IS NOT NULL����</returns>
		public static Filter CreateIsNotNullFilter(String propertyName)
		{
			return new IsNotNullFilter(propertyName);
		}

		/// <summary>
		/// �����ǿչ���������IS NOT NULL��������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <returns>�����õķǿչ���������IS NOT NULL����</returns>
		public static Filter CreateIsNotNullFilter(String entityPropertyName, String propertyName)
		{
			return new IsNotNullFilter(entityPropertyName, propertyName);
		}

		/// <summary>
		/// �����ǿչ���������IS NOT NULL������������·��������ֵ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>�����õķǿչ���������IS NOT NULL����</returns>
		public static Filter CreateIsNotNullFilter(IList<String> propertyPath)
		{
			return new IsNotNullFilter(propertyPath);
		}

		#endregion

		#region InFilter

		/// <summary>
		/// ���� IN ��������IN (...)���������������ƺ�ֵ���ϣ�֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="discreteValues">����ֵ���ϡ�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String propertyName, Object[] discreteValues)
		{
			return new InFilter(propertyName, discreteValues);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ơ�ֵ�������ƺ�ֵ���ϣ�֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="discreteValues">ֵ���ϡ�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, Object[] discreteValues)
		{
			return new InFilter(entityPropertyName, propertyName, discreteValues);
		}

		/// <summary>
		/// ���� IN ��������IN (...)������������·����ֵ���ϣ�֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="discreteValues">����ֵ���ϡ�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, Object[] discreteValues)
		{
			return new InFilter(propertyPath, discreteValues);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property)
		{
			return CreateInFilter(propertyName, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property)
		{
			return CreateInFilter(entityPropertyName, propertyName, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// ���� IN ��������IN (...)������������·�����б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property)
		{
			return CreateInFilter(propertyPath, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property, Boolean distinct)
		{
			return CreateInFilter(propertyName, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Boolean distinct)
		{
			return CreateInFilter(entityPropertyName, propertyName, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� IN ��������IN (...)������������·�����б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property, Boolean distinct)
		{
			return CreateInFilter(propertyPath, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateInFilter(propertyName, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateInFilter(entityPropertyName, propertyName, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� IN ��������IN (...)������������·�����б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateInFilter(propertyPath, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateInFilter(propertyName, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateInFilter(entityPropertyName, propertyName, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// ���� IN ��������IN (...)������������·�����б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateInFilter(propertyPath, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region ǰ�ö���

			Debug.Assert(property != null, "���Բ�������Ϊ�ա�");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new InFilter(propertyName, builder);
		}

		/// <summary>
		/// ���� IN ��������IN (...)���������������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region ǰ�ö���

			Debug.Assert(property != null, "���Բ�������Ϊ�ա�");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new InFilter(entityPropertyName, propertyName, builder);
		}

		/// <summary>
		/// ���� IN ��������IN (...)������������·�����б��ѯ��֧���ⲿ�������ԣ�ʹ�ò�ѯ�б�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� IN ��������IN (...)����</returns>
		public static Filter CreateInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region ǰ�ö���

			Debug.Assert(property != null, "���Բ�������Ϊ�ա�");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new InFilter(propertyPath, builder);
		}

		#endregion

		#region NotInFilter

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)���������������ƺ�ֵ���ϣ�֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="discreteValues">����ֵ���ϡ�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String propertyName, Object[] discreteValues)
		{
			return new NotInFilter(propertyName, discreteValues);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)��������ʵ���������ơ�ֵ�������ƺ�ֵ���ϣ�֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="discreteValues">ֵ���ϡ�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, Object[] discreteValues)
		{
			return new NotInFilter(entityPropertyName, propertyName, discreteValues);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)������������·����ֵ���ϣ�֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="discreteValues">����ֵ���ϡ�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, Object[] discreteValues)
		{
			return new NotInFilter(propertyPath, discreteValues);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property)
		{
			return CreateNotInFilter(propertyName, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)��������ʵ���������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property)
		{
			return CreateNotInFilter(entityPropertyName, propertyName, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)������������·�����б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property)
		{
			return CreateNotInFilter(propertyPath, property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property, Boolean distinct)
		{
			return CreateNotInFilter(propertyName, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)��������ʵ���������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Boolean distinct)
		{
			return CreateNotInFilter(entityPropertyName, propertyName, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)������������·�����б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property, Boolean distinct)
		{
			return CreateNotInFilter(propertyPath, property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateNotInFilter(propertyName, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)��������ʵ���������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateNotInFilter(entityPropertyName, propertyName, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)������������·�����б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return CreateNotInFilter(propertyPath, property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateNotInFilter(propertyName, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)��������ʵ���������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateNotInFilter(entityPropertyName, propertyName, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)������������·�����б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return CreateNotInFilter(propertyPath, property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)���������������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region ǰ�ö���

			Debug.Assert(property != null, "���Բ�������Ϊ�ա�");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new NotInFilter(propertyName, builder);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)��������ʵ���������ơ�ֵ�������ƺ��б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(String entityPropertyName, String propertyName, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region ǰ�ö���

			Debug.Assert(property != null, "���Բ�������Ϊ�ա�");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new NotInFilter(entityPropertyName, propertyName, builder);
		}

		/// <summary>
		/// ���� NOT IN ��������NOT IN (...)������������·�����б��ѯ��֧���ⲿ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>�����õ� NOT IN ��������NOT IN (...)����</returns>
		public static Filter CreateNotInFilter(IList<String> propertyPath, IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region ǰ�ö���

			Debug.Assert(property != null, "���Բ�������Ϊ�ա�");

			#endregion

			QueryListBuilder builder = QueryListBuilder.Create(property, whereFilter, havingFilter, distinct);

			return new NotInFilter(propertyPath, builder);
		}

		#endregion

		#region BetweenFilter

		/// <summary>
		/// ���� BETWEEN ����������������ֵ�����ұ߽������ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		/// <returns>�����õ� BETWEEN ��������</returns>
		public static Filter CreateBetweenFilter(String propertyName, Object from, Object to)
		{
			return new BetweenFilter(propertyName, from, to);
		}

		/// <summary>
		/// ���� BETWEEN ������������ʵ���������ơ�ֵ�������ƺ����ұ߽������ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		/// <returns>�����õ� BETWEEN ��������</returns>
		public static Filter CreateBetweenFilter(String entityPropertyName, String propertyName, Object from, Object to)
		{
			return new BetweenFilter(entityPropertyName, propertyName, from, to);
		}

		/// <summary>
		/// ���� BETWEEN ����������������·�������ұ߽������ֵ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		/// <returns>�����õ� BETWEEN ��������</returns>
		public static Filter CreateBetweenFilter(IList<String> propertyPath, Object from, Object to)
		{
			return new BetweenFilter(propertyPath, from, to);
		}

		#endregion

		#region NotBetweenFilter

		/// <summary>
		/// ���� NOT BETWEEN �������������������ƺ����ұ߽������ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		/// <returns>�����õ� NOT BETWEEN ��������</returns>
		public static Filter CreateNotBetweenFilter(String propertyName, Object from, Object to)
		{
			return new NotBetweenFilter(propertyName, from, to);
		}

		/// <summary>
		/// ���� NOT BETWEEN ������������ʵ���������ơ�ֵ�������ƺ����ұ߽������ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		/// <returns>�����õ� NOT BETWEEN ��������</returns>
		public static Filter CreateNotBetweenFilter(String entityPropertyName, String propertyName, Object from, Object to)
		{
			return new NotBetweenFilter(entityPropertyName, propertyName, from, to);
		}

		/// <summary>
		/// ���� NOT BETWEEN ����������������·�������ұ߽������ֵ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="from">��߽������ֵ��</param>
		/// <param name="to">�ұ߽������ֵ��</param>
		/// <returns>�����õ� NOT BETWEEN ��������</returns>
		public static Filter CreateNotBetweenFilter(IList<String> propertyPath, Object from, Object to)
		{
			return new NotBetweenFilter(propertyPath, from, to);
		}

		#endregion

		#region LikeFilter

		/// <summary>
		/// ���� LIKE �������������������ƺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="patternText">��ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <returns>�����õ� LIKE ��������</returns>
		public static Filter CreateLikeFilter(String propertyName, String patternText)
		{
			return new LikeFilter(propertyName, patternText);
		}

		/// <summary>
		/// ���� LIKE ������������ʵ���������ƺ�ʵ����ֵ���������Լ���ƥ��ģʽ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="patternText">��ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <returns>�����õ� LIKE ��������</returns>
		public static Filter CreateLikeFilter(String entityPropertyName, String propertyName, String patternText)
		{
			return new LikeFilter(entityPropertyName, propertyName, patternText);
		}

		/// <summary>
		/// ���� LIKE ����������������·���ͺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="patternText">��ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <returns>�����õ� LIKE ��������</returns>
		public static Filter CreateLikeFilter(IList<String> propertyPath, String patternText)
		{
			return new LikeFilter(propertyPath, patternText);
		}

		/// <summary>
		/// ���� LIKE �������������������ƺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="patternText">��ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		/// <returns>�����õ� LIKE ��������</returns>
		public static Filter CreateLikeFilter(String propertyName, String patternText, Char escapeChar)
		{
			return new LikeFilter(propertyName, patternText, escapeChar);
		}

		/// <summary>
		/// ���� LIKE ������������ʵ���������ƺ�ʵ����ֵ���������Լ���ƥ��ģʽ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="patternText">��ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		/// <returns>�����õ� LIKE ��������</returns>
		public static Filter CreateLikeFilter(String entityPropertyName, String propertyName, String patternText, Char escapeChar)
		{
			return new LikeFilter(entityPropertyName, propertyName, patternText, escapeChar);
		}

		/// <summary>
		/// ���� LIKE ����������������·���ͺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="patternText">��ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		/// <returns>�����õ� LIKE ��������</returns>
		public static Filter CreateLikeFilter(IList<String> propertyPath, String patternText, Char escapeChar)
		{
			return new LikeFilter(propertyPath, patternText, escapeChar);
		}

		#endregion

		#region NotLikeFilter

		/// <summary>
		/// ���� NOT LIKE �������������������ƺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <returns>�����õ� NOT LIKE ��������</returns>
		public static Filter CreateNotLikeFilter(String propertyName, String patternText)
		{
			return new NotLikeFilter(propertyName, patternText);
		}

		/// <summary>
		/// ���� NOT LIKE ������������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="patternText">����ֵ��ģ����ѯ���ʽ��</param>
		/// <returns>�����õ� NOT LIKE ��������</returns>
		public static Filter CreateNotLikeFilter(String entityPropertyName, String propertyName, String patternText)
		{
			return new NotLikeFilter(entityPropertyName, propertyName, patternText);
		}

		/// <summary>
		/// ���� NOT LIKE ����������������·����ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <returns>�����õ� NOT LIKE ��������</returns>
		public static Filter CreateNotLikeFilter(IList<String> propertyPath, String patternText)
		{
			return new NotLikeFilter(propertyPath, patternText);
		}

		/// <summary>
		/// ���� NOT LIKE �������������������ƺ�ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		/// <returns>�����õ� NOT LIKE ��������</returns>
		public static Filter CreateNotLikeFilter(String propertyName, String patternText, Char escapeChar)
		{
			return new NotLikeFilter(propertyName, patternText, escapeChar);
		}

		/// <summary>
		/// ���� NOT LIKE ������������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="patternText">����ֵ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		/// <returns>�����õ� NOT LIKE ��������</returns>
		public static Filter CreateNotLikeFilter(String entityPropertyName, String propertyName, String patternText, Char escapeChar)
		{
			return new NotLikeFilter(entityPropertyName, propertyName, patternText, escapeChar);
		}

		/// <summary>
		/// ���� NOT LIKE ����������������·����ƥ��ģʽ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="patternText">ƥ��ģʽ��ģ����ѯ���ʽ��</param>
		/// <param name="escapeChar">ת���ַ���</param>
		/// <returns>�����õ� NOT LIKE ��������</returns>
		public static Filter CreateNotLikeFilter(IList<String> propertyPath, String patternText, Char escapeChar)
		{
			return new NotLikeFilter(propertyPath, patternText, escapeChar);
		}

		#endregion

		#region GreaterThanFilter

		/// <summary>
		/// �������ڹ��������������������ƺ�����ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĴ��ڹ�������</returns>
		public static Filter CreateGreaterThanFilter(String propertyName, Object propertyValue)
		{
			return new GreaterThanFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// �������ڹ�����������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĴ��ڹ�������</returns>
		public static Filter CreateGreaterThanFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new GreaterThanFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// �������ڹ�����������������·��������ֵ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĴ��ڹ�������</returns>
		public static Filter CreateGreaterThanFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new GreaterThanFilter(propertyPath, propertyValue);
		}

		#endregion

		#region LessThanFilter

		/// <summary>
		/// ����С�ڹ������������������ƺ�����ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ�С�ڹ�������</returns>
		public static Filter CreateLessThanFilter(String propertyName, Object propertyValue)
		{
			return new LessThanFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// ����С�ڹ�����������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ�С�ڹ�������</returns>
		public static Filter CreateLessThanFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new LessThanFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// ����С�ڹ������������������ƺ�����ֵ��
		/// </summary>
		/// <param name="propertyPath">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ�С�ڹ�������</returns>
		public static Filter CreateLessThanFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new LessThanFilter(propertyPath, propertyValue);
		}

		#endregion

		#region GreaterThanEqualsFilter

		/// <summary>
		/// �������ڵ��ڹ������������������ƺ�����ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĴ��ڵ��ڹ�������</returns>
		public static Filter CreateGreaterThanEqualsFilter(String propertyName, Object propertyValue)
		{
			return new GreaterThanEqualsFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// �������ڵ��ڹ�����������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĴ��ڵ��ڹ�������</returns>
		public static Filter CreateGreaterThanEqualsFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new GreaterThanEqualsFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// �������ڵ��ڹ���������������·��������ֵ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õĴ��ڵ��ڹ�������</returns>
		public static Filter CreateGreaterThanEqualsFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new GreaterThanEqualsFilter(propertyPath, propertyValue);
		}

		#endregion

		#region LessThanEqualsFilter

		/// <summary>
		/// ����С�ڵ��ڹ������������������ƺ�����ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ�С�ڵ��ڹ�������</returns>
		public static Filter CreateLessThanEqualsFilter(String propertyName, Object propertyValue)
		{
			return new LessThanEqualsFilter(propertyName, propertyValue);
		}

		/// <summary>
		/// ����С�ڵ��ڹ�����������ʵ���������ƺ�ʵ����ֵ���������Լ�����ֵ��
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ʵ�������е�ֵ�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ�С�ڵ��ڹ�������</returns>
		public static Filter CreateLessThanEqualsFilter(String entityPropertyName, String propertyName, Object propertyValue)
		{
			return new LessThanEqualsFilter(entityPropertyName, propertyName, propertyValue);
		}

		/// <summary>
		/// ����С�ڵ��ڹ���������������·��������ֵ��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>�����õ�С�ڵ��ڹ�������</returns>
		public static Filter CreateLessThanEqualsFilter(IList<String> propertyPath, Object propertyValue)
		{
			return new LessThanEqualsFilter(propertyPath, propertyValue);
		}

		#endregion

		#endregion

		#endregion

		#region ˽���ֶ�

		private ColumnLocator m_columnLocator;
		private readonly Object[] m_values;
		private readonly QueryListBuilder m_queryListBuilder;
		private String m_queryListSqlStatement;

		private String m_parameterPrefix;

		// ��Ӧ��ֵ����
		private QueryParameter[] m_parameters;
		private String m_columnFullName;

		// ��Ӧ��ʵ������
		private Boolean m_isEntityFilter;
		private QueryParameterCollection[] m_entityParameters;
		private String[] m_entityColumnFullNames;

		private readonly LogicOperator m_logicOperator;

		private Collection<Filter> m_filters;

		private Boolean m_negative;
		private Boolean m_compiled;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		protected Filter()
		{
		}

		/// <summary>
		/// ���캯��������Ҫ���˵��������ơ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		protected Filter(String propertyName)
		{
			m_columnLocator = new ColumnLocator(propertyName);
		}

		/// <summary>
		/// ���캯��������ʵ���������ƺ͸�ʵ���е�ֵ�������ơ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ƣ����Ϊ null�����ʾΪ��ǰʵ�塣</param>
		/// <param name="propertyName">��ʵ���е�ֵ��������</param>
		protected Filter(String entityPropertyName, String propertyName)
		{
			m_columnLocator = new ColumnLocator(entityPropertyName, propertyName);
		}

		/// <summary>
		/// ���캯������������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		protected Filter(IList<String> propertyPath)
		{
			m_columnLocator = new ColumnLocator(propertyPath);
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="propertyValues">����ֵ���ϡ�</param>
		protected Filter(String propertyName, Object[] propertyValues)
		{
			m_columnLocator = new ColumnLocator(propertyName);
			m_values = propertyValues;
		}

		/// <summary>
		/// ���캯��������ʵ���������ƺ͸�ʵ�������е�ֵ�������ơ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="propertyValues">����ֵ���ϡ�</param>
		protected Filter(String entityPropertyName, String propertyName, Object[] propertyValues)
		{
			m_columnLocator = new ColumnLocator(entityPropertyName, propertyName);
			m_values = propertyValues;
		}

		/// <summary>
		/// ���캯������������·��������ֵ���ϡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="propertyValues">����ֵ���ϡ�</param>
		protected Filter(IList<String> propertyPath, Object[] propertyValues)
		{
			m_columnLocator = new ColumnLocator(propertyPath);
			m_values = propertyValues;
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="builder">��ѯ�б���������</param>
		internal Filter(String propertyName, QueryListBuilder builder)
		{
			m_columnLocator = new ColumnLocator(propertyName);
			m_queryListBuilder = builder;
		}

		/// <summary>
		/// ���캯��������ʵ���������ƺ͸�ʵ�������е�ֵ�������ơ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="builder">��ѯ�б���������</param>
		internal Filter(String entityPropertyName, String propertyName, QueryListBuilder builder)
		{
			m_columnLocator = new ColumnLocator(entityPropertyName, propertyName);
			m_queryListBuilder = builder;
		}

		/// <summary>
		/// ���캯������������·��������ֵ���ϡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="builder">��ѯ�б���������</param>
		internal Filter(IList<String> propertyPath, QueryListBuilder builder)
		{
			m_columnLocator = new ColumnLocator(propertyPath);
			m_queryListBuilder = builder;
		}

		/// <summary>
		/// ���캯���������߼����Ӳ�������
		/// </summary>
		/// <param name="logicOperator">�߼���������AND �� OR��</param>
		protected Filter(LogicOperator logicOperator)
		{
			m_logicOperator = logicOperator;
		}

		#endregion

		#region ����

		/// <summary>
		/// �� AND ����
		/// </summary>
		public const String AND = " AND ";

		/// <summary>
		/// �� OR ����
		/// </summary>
		public const String OR = " OR ";

		/// <summary>
		/// ��NOT ����
		/// </summary>
		public const String NOT = "NOT ";

		/// <summary>
		/// �� BETWEEN ����
		/// </summary>
		public const String BETWEEN = " BETWEEN ";

		/// <summary>
		/// ��NOT BETWEEN ����
		/// </summary>
		public const String NOT_BETWEEN = " NOT BETWEEN ";

		/// <summary>
		/// �� LIKE ����
		/// </summary>
		public const String LIKE = " LIKE ";

		/// <summary>
		/// �� NOT LIKE ����
		/// </summary>
		public const String NOT_LIKE = " NOT LIKE ";

		/// <summary>
		/// �� IN ����
		/// </summary>
		public const String IN = " IN ";

		/// <summary>
		/// �� NOT IN ����
		/// </summary>
		public const String NOT_IN = " NOT IN ";

		/// <summary>
		/// ��@����
		/// </summary>
		public const String PARAMETER_PREFIX = "@";

		/// <summary>
		/// ��(����
		/// </summary>
		public const String LEFT_BRACKET = "(";

		/// <summary>
		/// ��)����
		/// </summary>
		public const String RIGHT_BRACKET = ")";

		/// <summary>
		/// ��,����
		/// </summary>
		public const String COMMA = ",";

		/// <summary>
		/// �ո�
		/// </summary>
		public const Char SPACE = '\x20';

		/// <summary>
		/// Ĭ�ϵ�ģ��ƥ��ת�����\����
		/// </summary>
		public const Char DEFAULT_FUZZY_ESCAPE_CHAR = '\\';

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǰ�������Ƿ�Ҫ�� NOT ��������
		/// </summary>
		public Boolean Negative
		{
			get { return m_negative; }
		}

		/// <summary>
		/// ��ȡ�����ò���ǰ׺��
		/// </summary>
		public String ParameterPrefix
		{
			get
			{
				if (m_parameterPrefix == null)
				{
					m_parameterPrefix = PARAMETER_PREFIX;
				}

				return m_parameterPrefix;
			}

			protected set
			{
				if (value != null)
				{
					value = value.Trim();
				}

				if (!String.IsNullOrEmpty(value))
				{
					m_parameterPrefix = value;
				}
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ��ǰʵ����ǳ������
		/// </summary>
		/// <returns>��ǰʵ����ǳ������</returns>
		public virtual Filter Clone()
		{
			return (Filter)MemberwiseClone();
		}

		/// <summary>
		/// ���ӹ��������Թ��츴�ӵĹ�������
		/// </summary>
		/// <param name="logicOperator">���Ӳ�������AND �� OR ���֡�</param>
		/// <param name="filters">Ҫ���ӵĹ��������顣</param>
		/// <returns>����õ��¹�������</returns>
		public static Filter Combine(LogicOperator logicOperator, params Filter[] filters)
		{
			if (filters == null)
			{
				filters = new Filter[0];
			}

			// �Ƴ�Ϊ null �Ĺ�����
			filters = Array.FindAll<Filter>(
					filters,
					delegate(Filter item)
					{
						return (item != null);
					}
				);

			if (filters.Length == 0)
			{
				return null;
			}
			else if (filters.Length == 1)
			{
				return filters[0];
			}

			CompositeFilter f = new CompositeFilter(logicOperator, filters);

			return f;
		}

		/// <summary>
		/// �Թ��������б��룬
		/// </summary>
		/// <param name="context">�����ṩ����֧�ֵĻ�������</param>
		public virtual void Compile(FilterCompilationContext context)
		{
			ParameterPrefix = context.ParameterPrefix;

			// ����ж�λ������ڣ�Ҫȷ���е�����
			if (m_columnLocator != null)
			{
				m_isEntityFilter = context.IsEntityColumns(m_columnLocator);

				if (!IsEntityFilter)
				{
					m_columnFullName = context.GetColumnFullName(m_columnLocator);
				}
				else
				{
					m_entityColumnFullNames = context.GetEntityColumnFullNames(m_columnLocator);
				}
			}

			// ����ж�λ������ڣ���ֵ���ϲ�Ϊ�գ������ɲ�ѯ����
			if ((m_columnLocator != null) && (m_values != null) && (m_values.Length != 0))
			{
				if (!IsEntityFilter)
				{
					m_parameters = context.GenerateParameters(m_columnLocator, m_values);
				}
				else
				{
					m_entityParameters = context.GenerateEntityParameters(m_columnLocator, m_values);
				}
			}

			if (m_queryListBuilder != null)
			{
				m_queryListSqlStatement = m_queryListBuilder.Build(context);
			}

			// ������ӹ��������ϣ���������� Compile ����
			if (HasFilters)
			{
				foreach (Filter f in Filters)
				{
					f.Compile(context);
				}
			}

			// ���ñ���״̬
			m_compiled = true;
		}

		/// <summary>
		/// �ɲ������ƹ����ѯ������
		/// </summary>
		/// <param name="parameterName">�������ơ�</param>
		/// <returns>����õĲ�ѯ������</returns>
		public String ComposeParameter(String parameterName)
		{
			if (String.IsNullOrEmpty(parameterName))
			{
				return parameterName;
			}

			return ParameterPrefix + parameterName;
		}

		/// <summary>
		/// �Ե�ǰʵ��������ƣ����������Բ��ֽ�����ƣ�ֵ���֣������Ӳ�ѯ�б��������ⲿ���ǲ��ɱ�ģ���
		/// </summary>
		/// <returns>��ǰʵ������Ƶĸ�����</returns>
		public Filter DeepClone()
		{
			Filter clone = (Filter)MemberwiseClone();

			if (HasFilters)
			{
				clone.m_filters = new Collection<Filter>();

				foreach (Filter filter in this.Filters)
				{
					clone.m_filters.Add(filter.DeepClone());
				}
			}

			return clone;
		}

		/// <summary>
		/// ��չ������Ϊ������ʵ��Ĺ�������
		/// </summary>
		/// <param name="childrenPropertyName">��ʵ����ָ��ʵ����������ơ�</param>
		/// <returns>��չ���µĹ�������</returns>
		public Filter Extend(String childrenPropertyName)
		{
			return Extend(new String[] { childrenPropertyName });
		}

		/// <summary>
		/// ��չ������Ϊ������ʵ��Ĺ�������
		/// </summary>
		/// <param name="propertyPath">��ʵ����ָ��ʵ�������·����</param>
		/// <returns>��չ���µĹ�������</returns>
		public Filter Extend(String[] propertyPath)
		{
			Filter childrenFilter = Clone();

			if (childrenFilter.m_columnLocator != null)
			{
				String[] childrenPropertyPath = new String[childrenFilter.m_columnLocator.PropertyPath.Length + propertyPath.Length];

				Array.Copy(
						propertyPath,
						childrenPropertyPath,
						propertyPath.Length
					);
				Array.Copy(
						childrenFilter.m_columnLocator.PropertyPath,
						0,
						childrenPropertyPath,
						propertyPath.Length,
						childrenFilter.m_columnLocator.PropertyPath.Length
					);

				childrenFilter.m_columnLocator = new ColumnLocator(childrenPropertyPath);
			}

			if (this.HasFilters)
			{
				childrenFilter.m_filters = new Collection<Filter>();

				for (Int32 i = 0; i < this.Filters.Count; i++)
				{
					childrenFilter.m_filters.Add(this.Filters[i].Extend(propertyPath));
				}
			}

			return childrenFilter;
		}

		/// <summary>
		/// ��ȡ���е��ж�λ����
		/// </summary>
		/// <returns>�ж�λ�����ϡ�</returns>
		public ColumnLocator[] GetAllColumnLocators()
		{
			List<ColumnLocator> allColumnLocators = new List<ColumnLocator>();

			RetrieveAllColumnLoactors(this, allColumnLocators);

			return allColumnLocators.ToArray();
		}

		/// <summary>
		/// ��ȡ���е�����ѡ������ָʾ������ʵ�壩��
		/// </summary>
		/// <returns>���е�����ѡ������</returns>
		public PropertySelector[] GetAllSelectors(Type entityType)
		{
			List<ColumnLocator> container = new List<ColumnLocator>();

			RetrieveAllColumnLoactors(this, container);

			List<IPropertyChain> allForeignReferences = new List<IPropertyChain>();

			foreach (ColumnLocator colLocator in container)
			{
				if (colLocator.PropertyPath.Length > 1)
				{
					IPropertyChain chain = colLocator.Create(entityType);

					// ǰһ�����Խڵ����������´���������ᵼ������ǰ����
					IPropertyChain reference = chain.Previous.Copy();

					if (!allForeignReferences.Contains(reference))
					{
						allForeignReferences.Add(reference);
					}
				}
			}

			List<PropertySelector> allSelectors = allForeignReferences.ConvertAll<PropertySelector>(
					delegate(IPropertyChain chain)
					{
						return PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain);
					}
				);

			return allSelectors.ToArray();
		}

		/// <summary>
		/// ��ȡ���еĲ�ѯ�б���������
		/// </summary>
		/// <returns>���еĲ�ѯ�б���������</returns>
		internal QueryListBuilder[] GetAllQueryListBuilders()
		{
			List<QueryListBuilder> allBuilders = new List<QueryListBuilder>();

			RetrieveAllQueryListBuilders(this, allBuilders);

			return allBuilders.ToArray();
		}

		/// <summary>
		/// �����뻺����д���������ʽ��
		/// </summary>
		/// <param name="output">���뻺������</param>
		public virtual void Generate(StringBuilder output)
		{
			ThrowExceptionIfNotCompiled();

			if (HasFilters)
			{
				if (Negative)
				{
					output.Append(NOT);

					output.Append(LEFT_BRACKET);
				}

				for (Int32 i = 0; i < Filters.Count; i++)
				{
					if (i > 0)
					{
						output.Append(CombineWord);
					}

					output.Append(LEFT_BRACKET);

					Filters[i].Generate(output);

					output.Append(RIGHT_BRACKET);
				}

				if (Negative)
				{
					output.Append(RIGHT_BRACKET);
				}
			}
		}

		/// <summary>
		/// �Թ��������� NOT �߼���������ε�����Ч��
		/// </summary>
		public void Not()
		{
			m_negative = true;
		}

		#endregion

		#region ����

		/// <summary>
		/// ��ȡ�������е�ȫ���ơ�
		/// </summary>
		protected String ColumnFullName
		{
			get { return m_columnFullName; }
			set { m_columnFullName = value; }
		}

		/// <summary>
		/// ��ȡ�ж�λ����
		/// </summary>
		protected ColumnLocator ColumnLocator
		{
			get { return m_columnLocator; }
		}

		/// <summary>
		/// ��ȡ���� SQL �ؼ��֣�AND �� OR��
		/// </summary>
		protected String CombineWord
		{
			get { return (LogicOperator == LogicOperator.And) ? AND : OR; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǰ�������Ƿ��Ѿ����롣
		/// </summary>
		protected Boolean Compiled
		{
			get { return m_compiled; }
		}

		/// <summary>
		/// ��ȡ���������ϡ�
		/// </summary>
		internal protected Collection<Filter> Filters
		{
			get
			{
				if (m_filters == null)
				{
					m_filters = new Collection<Filter>();
				}

				return m_filters;
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ�����ӹ�������
		/// </summary>
		internal protected Boolean HasFilters
		{
			get { return (m_filters != null) && (m_filters.Count != 0); }
		}

		/// <summary>
		/// ��ȡ���Ӳ�������
		/// </summary>
		internal protected LogicOperator LogicOperator
		{
			get { return m_logicOperator; }
		}

		/// <summary>
		/// ��ȡ�����ò�ѯ������
		/// </summary>
		protected QueryParameter[] Parameters
		{
			get
			{
				if (m_parameters == null)
				{
					m_parameters = new QueryParameter[0];
				}

				return m_parameters;
			}

			set
			{
				m_parameters = value;
			}
		}

		/// <summary>
		/// ��ȡ��������ֵ���ϡ�
		/// </summary>
		protected Object[] Values
		{
			get { return m_values; }
		}

		/// <summary>
		/// ��ȡ��ѯ�б������������� IN ����������
		/// </summary>
		internal QueryListBuilder QueryListBuilder
		{
			get { return m_queryListBuilder; }
		}

		/// <summary>
		/// ��ȡ��ѯ�б� SQL ָ�
		/// </summary>
		public String QueryListSqlStatement
		{
			get { return m_queryListSqlStatement; }
		}

		/// <summary>
		/// ��ȡһ����ֵ����ֵָʾ�Ƿ�Ϊʵ���������
		/// </summary>
		protected Boolean IsEntityFilter
		{
			get { return m_isEntityFilter; }
		}

		/// <summary>
		/// ��ȡʵ��������ϡ�
		/// </summary>
		protected QueryParameterCollection[] EntityParameters
		{
			get
			{
				if (m_entityParameters == null)
				{
					m_entityParameters = new QueryParameterCollection[0];
				}

				return m_entityParameters;
			}
		}

		/// <summary>
		/// ��ȡʵ�����ȫ���Ƽ��ϡ�
		/// </summary>
		protected String[] EntityColumnFullNames
		{
			get
			{
				if (m_entityColumnFullNames == null)
				{
					m_entityColumnFullNames = new String[0];
				}

				return m_entityColumnFullNames;
			}
		}

		#endregion

		#region �����ķ���

		/// <summary>
		/// ���������Ƿ��Ѿ����룬���û�б��룬���׳� InvalidOperationException �쳣��
		/// </summary>
		protected void ThrowExceptionIfNotCompiled()
		{
			if (!Compiled)
			{
				throw new InvalidOperationException("��������û�б��룬���ܽ��������");
			}
		}

		#endregion

		#region ICloneable ��Ա

		/// <summary>
		/// ��õ�ǰʵ����ǳ������
		/// </summary>
		/// <returns>��ǰʵ����ǳ������</returns>
		Object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion

		#region ��������

		/// <summary>
		/// �ռ����л��������������Զ�λ����
		/// </summary>
		/// <param name="filter">��������</param>
		/// <param name="container">�ռ�������</param>
		private static void RetrieveAllColumnLoactors(Filter filter, List<ColumnLocator> container)
		{
			if (filter.ColumnLocator != null)
			{
				container.Add(filter.ColumnLocator);
			}
			else if (filter.HasFilters)
			{
				foreach (Filter childFilter in filter.Filters)
				{
					RetrieveAllColumnLoactors(childFilter, container);
				}
			}
		}

		/// <summary>
		/// �ռ����еĲ�ѯ�б���������
		/// </summary>
		/// <param name="filter">��������</param>
		/// <param name="container">�ռ�������</param>
		private static void RetrieveAllQueryListBuilders(Filter filter, List<QueryListBuilder> container)
		{
			if (filter.QueryListBuilder != null)
			{
				container.Add(filter.QueryListBuilder);
			}
			else if (filter.HasFilters)
			{
				foreach (Filter childFilter in filter.Filters)
				{
					RetrieveAllQueryListBuilders(childFilter, container);
				}
			}
		}

		#endregion
	}
}