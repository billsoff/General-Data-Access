#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CommonPolicies.cs
// �ļ�����������ͨ�÷��루�����������жԱȵȣ���
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110516
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ͨ�÷��루�����������жԱȵȣ���
	/// </summary>
	internal static class CommonPolicies
	{
		#region ˽���ֶ�

		private static readonly BindingFlags m_propertyBindingFlags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly StringComparison m_propertyNameComparison = StringComparison.Ordinal;
		private static readonly StringComparison m_columnNameComparison = StringComparison.OrdinalIgnoreCase;

		private static readonly String m_tableAliasPrefix = "t__";
		private static readonly String m_columnAliasPrefix = "f__";
		private static readonly String m_whereFilterParameterNamePrefix = "pw__";
		private static readonly String m_havingFilterParameterNamePrefix = "ph__";

		#endregion

		#region ��������

		/// <summary>
		///  �㡣
		/// </summary>
		public const String DOT = ".";

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ������ƥ����򣬵�ǰ�Ĺ���Ϊ�����ִ�Сд��
		/// </summary>
		public static StringComparison ColumnNameComparison
		{
			get { return m_columnNameComparison; }
		}

		/// <summary>
		/// ��ȡ�����������룬��ǰ�ķ���Ϊȡ�������Ͳ���еĹ��еĺͷǹ��е�ʵ�����ԡ�
		/// <para>����ʵ��̳У����Ϊһ�ֺϲ���ϵ��</para>
		/// </summary>
		public static BindingFlags PropertyBindingFlags
		{
			get { return m_propertyBindingFlags; }
		}

		/// <summary>
		/// ��ȡ��������ƥ����򣬵�ǰ�Ĺ���Ϊ���ִ�Сд��
		/// </summary>
		public static StringComparison PropertyNameComparison
		{
			get { return m_propertyNameComparison; }
		}

		/// <summary>
		/// ��ȡ�����ǰ׺��
		/// </summary>
		public static String TableAliasPrefix
		{
			get { return CommonPolicies.m_tableAliasPrefix; }
		}

		/// <summary>
		/// ��ȡ�б���ǰ׺��
		/// </summary>
		public static String ColumnAliasPrefix
		{
			get { return CommonPolicies.m_columnAliasPrefix; }
		}

		/// <summary>
		/// ��ȡ WHERE ������������ǰ׺��
		/// </summary>
		public static String WhereFilterParameterNamePrefix
		{
			get { return CommonPolicies.m_whereFilterParameterNamePrefix; }
		}

		/// <summary>
		/// ��ȡ HAVING ������������ǰ׺��
		/// </summary>
		public static String HavingFilterParameterNamePrefix
		{
			get { return CommonPolicies.m_havingFilterParameterNamePrefix; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��������ȫ���ƣ��õ㽫����·������������
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>����ȫ���ơ�</returns>
		public static String ComposePropertyFullName(String[] propertyPath)
		{
			return String.Join(DOT, propertyPath);
		}

		/// <summary>
		/// ��ȡ�е�ȫ���ơ�
		/// </summary>
		/// <param name="columnDef">�ж��塣</param>
		/// <param name="property">��������ʵ�����ԡ�</param>
		/// <returns>�е�ȫ���ƣ�����Ϊ null��</returns>
		public static String GetColumnFullName(ColumnDefinition columnDef, EntityProperty property)
		{
			if ((columnDef != null) && (property != null))
			{
				return String.Format("{0}.{1}", property.Schema.TableAlias, columnDef.Name);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// ��ȡ����ʵ�����Ե��е����ݿ����ͣ�Ĭ��Ϊ Int32��
		/// </summary>
		/// <param name="propertyDef">����ʵ�����Զ��塣</param>
		/// <param name="columnDef">�ж��塣</param>
		/// <returns>����ʵ����е����ݿ����ͣ�������ܴӲ����еó�����ȡ Int32��</returns>
		public static DbType GetGroupDbType(GroupPropertyDefinition propertyDef, ColumnDefinition columnDef)
		{
			if ((propertyDef != null) && propertyDef.Aggregation.AcceptDbType)
			{
				return propertyDef.Aggregation.DbType;
			}
			else if (columnDef != null)
			{
				return columnDef.DbType;
			}
			else
			{
				return DbType.Int32;
			}
		}

		/// <summary>
		/// ��ȡ�������
		/// </summary>
		/// <param name="index">��������</param>
		/// <returns>�������</returns>
		public static String GetTableAlias(Int32 index)
		{
			return TableAliasPrefix + index.ToString();
		}

		/// <summary>
		/// ��ȡ�б�����
		/// </summary>
		/// <param name="index">��������</param>
		/// <returns>����õ��б�����</returns>
		public static String GetColumnAlias(Int32 index)
		{
			return ColumnAliasPrefix + index.ToString();
		}

		/// <summary>
		/// ��ȡ�������ơ�
		/// </summary>
		/// <param name="fullName">����ȫ���ơ�</param>
		/// <returns>�������ơ�</returns>
		public static String GetPropertyName(String fullName)
		{
			if (!fullName.Contains(CommonPolicies.DOT))
			{
				return fullName;
			}
			else
			{
				Int32 index = fullName.LastIndexOf(CommonPolicies.DOT);

				return fullName.Substring(index + 1);
			}
		}

		/// <summary>
		/// ��ȡ�淶�����������������ıȽϹ����ǲ����ִ�Сд����˵�ǰ��ʵ���Ƿ���������ȫ��д��ʽ��
		/// </summary>
		/// <param name="columnName">������</param>
		/// <returns>�����Ĺ淶����ʾ��</returns>
		public static String NormalizeColumnName(String columnName)
		{
			if (!String.IsNullOrEmpty(columnName))
			{
				return columnName.ToUpper();
			}
			else
			{
				return columnName;
			}
		}

		/// <summary>
		/// ��ȡ�淶���������������������ıȽϹ��������ִ�Сд����˵�ǰ��ʵ����ԭ��������������
		/// </summary>
		/// <param name="propertyName">��������</param>
		/// <returns>�������Ĺ淶����ʽ��</returns>
		public static String NormalizePropertyName(String propertyName)
		{
			return propertyName;
		}

		#endregion
	}
}