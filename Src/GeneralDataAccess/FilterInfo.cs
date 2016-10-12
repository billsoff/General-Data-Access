#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterFactoryInfo.cs
// �ļ������������������ڴ�����������������Ϣ�����ڴ���������������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110325
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
	/// �������ڴ�����������������Ϣ�����ڴ���������������
	/// <para>����Ӧʵ�ִ��Ͳ��� negative �����๹�캯����Ȼ������ FilterInofExpression��Is �� FilterExpression �й���ʵ������������Ԫ�ء�</para>
	/// </summary>
	[Serializable]
	public abstract class FilterInfo
	{
		#region ˽���ֶ�

		private readonly Object[] m_values;
		private readonly QueryListBuilder m_queryListBuilder;
		private readonly Boolean m_negative;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		protected FilterInfo()
		{
		}

		/// <summary>
		/// ���캯����
		/// </summary>
		/// <param name="negative">ָʾ�Ƿ���С��ǡ�������</param>
		protected FilterInfo(Boolean negative)
			: this(negative, (Object[])null)
		{
		}

		/// <summary>
		/// ���캯������������ֵ���ϡ�
		/// </summary>
		/// <param name="values">����ֵ���ϡ�</param>
		public FilterInfo(Object[] values)
			: this(false, values)
		{
		}

		/// <summary>
		/// ���캯������������ֵ���ϡ�
		/// </summary>
		/// <param name="negative">ָʾ�Ƿ���С��ǡ�������</param>
		/// <param name="values">����ֵ���ϡ�</param>
		protected FilterInfo(Boolean negative, Object[] values)
		{
			m_negative = negative;
			m_values = values;
		}

		/// <summary>
		/// ���캯������������ֵ���ϡ�
		/// </summary>
		/// <param name="builder">��ѯ�б���������</param>
		internal FilterInfo(QueryListBuilder builder)
			: this(false, builder)
		{
		}

		/// <summary>
		/// ���캯������������ֵ���ϡ�
		/// </summary>
		/// <param name="negative">ָʾ�Ƿ���С��ǡ�������</param>
		/// <param name="builder">��ѯ�б���������</param>
		internal FilterInfo(Boolean negative, QueryListBuilder builder)
		{
			m_negative = negative;
			m_queryListBuilder = builder;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ������������
		/// </summary>
		/// <param name="propertyPath">ֵ����·����</param>
		/// <returns>�����õĹ�����������</returns>
		public Filter CreateFilter(IList<String> propertyPath)
		{
			Filter result = DoCreateFilter(propertyPath);

			if (m_negative)
			{
				result.Not();
			}

			return result;
		}

		#endregion

		#region ����������

		/// <summary>
		/// ��ȡ����ֵ���ϡ�
		/// </summary>
		protected Object[] Values
		{
			get { return m_values; }
		}

		/// <summary>
		/// ��ȡ��ѯ�б���������
		/// </summary>
		internal QueryListBuilder QueryListBuilder
		{
			get { return m_queryListBuilder; }
		}

		#endregion

		#region �����Ա

		#region ��������

		/// <summary>
		/// ί������ʵ�ִ�����������
		/// </summary>
		/// <param name="propertyPath">ֵ����·����</param>
		/// <returns>�����õĹ�������</returns>
		protected abstract Filter DoCreateFilter(IList<String> propertyPath);

		#endregion

		#endregion
	}
}