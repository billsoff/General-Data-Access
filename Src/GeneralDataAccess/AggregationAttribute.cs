#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupItemAttribute.cs
// �ļ��������������ڱ�ǲ����������ԣ�ָʾҪô��Ϊ�������ݣ�Ҫô�����Խ��оۺϼ��㣩��������������ࡣ
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110627
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
	/// ���ڱ�ǲ����������ԣ�ָʾҪô��Ϊ�������ݣ�Ҫô�����Խ��оۺϼ��㣩��������������ࡣ
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public abstract class AggregationAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly String[] m_propertyPath;
		private Boolean m_acceptDbType;
		private DbType m_dbType = DbType.Int32;

		private Boolean m_distinct;

		#endregion

		#region ��������

		/// <summary>
		/// ͨ�������*����
		/// </summary>
		public const String KEY_WORD_WILDCARD = "*";

		/// <summary>
		/// DISTINCT �ؼ��֡�
		/// </summary>
		public const String KEY_WORD_DISTINCT = "DISTINCT";

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯������ʾ�ۺϼ����������޹ء�
		/// </summary>
		protected AggregationAttribute()
		{
		}

		/// <summary>
		/// ���캯�������ò�����������·��������Ϊ null����ʾ�ۺϼ����������޹ء�
		/// </summary>
		/// <param name="propertyPath">������������·����</param>
		protected AggregationAttribute(String[] propertyPath)
		{
			if ((propertyPath != null) && (propertyPath.Length != 0))
			{
				m_propertyPath = propertyPath;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ָʾ�Ƿ���� DbType ���Ե�ֵ��Ĭ��Ϊ false��
		/// </summary>
		public Boolean AcceptDbType
		{
			get { return m_acceptDbType; }
			set { m_acceptDbType = value; }
		}

		/// <summary>
		/// ��ȡ���������ݿ����ͣ�Ĭ��Ϊ Int32��ֻ�е� AcceptDbType Ϊ true ʱ���Ž��ܴ�ֵ��Ĭ��ȡҪ�ۺϵ����Ե� DbType��
		/// <para>���δ����ֵ����ָ���ۺϵ�����ʱ��ȡ�����Ե����ݿ����ͣ�����ȡ Int32��</para>
		/// </summary>
		public DbType DbType
		{
			get { return m_dbType; }
			set { m_dbType = value; }
		}

		/// <summary>
		/// ָʾ�Ƿ��ھۺϱ��ʽ�м� DISTINCT �ؼ��֣�Ĭ��Ϊ false��
		/// </summary>
		public Boolean Distinct
		{
			get { return m_distinct; }
			set { m_distinct = value; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǵ������Ƿ�Ϊ�����Ĭ��ֵΪ false��
		/// </summary>
		public virtual Boolean IsGroupItem
		{
			get { return false; }
		}

		/// <summary>
		/// ��ȡ����·������·��ָʾҪ�����������ԣ����������Ϊ null�����ʾ�ۺϼ����������޹ء�
		/// </summary>
		public String[] PropertyPath
		{
			get { return m_propertyPath; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�ۺϱ��ʽ��
		/// </summary>
		/// <param name="columnName">�е��޶��������Ϊ null����ָʾ�ۺϼ��������޹ء�</param>
		/// <returns>�ۺϱ��ʽ��</returns>
		public virtual String GetAggregationExpression(String columnName)
		{
			#region ǰ������

			Debug.Assert(
					(columnName != null) || AllowNullColumnName,
					String.Format("���� columnName ����Ϊ�գ����ھۺϱ�� {0} ��ָ��Ҫ�ۺϵ����ԡ�", this)
				);

			#endregion

			if (!Distinct)
			{
				if ((columnName == null) && AllowNullColumnName)
				{
					columnName = KEY_WORD_WILDCARD;
				}

				return String.Format("{0}({1})", FunctionName, columnName);
			}
			else
			{
				#region ǰ������

				Debug.Assert(
						!String.IsNullOrEmpty(columnName),
						String.Format(
								"���� columnName ����Ϊ�գ���Ϊ Distinct ���Ա�����Ϊ true�������ھۺϱ�� {0} ��ָ��Ҫ�ۺϵ����ԡ�",
								this
							)
					);

				#endregion

				return String.Format("{0}({1} {2})", FunctionName, KEY_WORD_DISTINCT, columnName);
			}
		}

		#endregion

		#region �ܱ���������

		/// <summary>
		/// ָʾ�Ƿ������������Ĭ��Ϊ false��
		/// </summary>
		protected virtual Boolean AllowNullColumnName
		{
			get { return false; }
		}

		/// <summary>
		/// ��ȡ�ۺϺ��������ơ�
		/// </summary>
		protected virtual String FunctionName
		{
			get { throw new NotImplementedException("�ۺϺ���������δ֪���޷����ɾۺϱ��ʽ��"); }
		}

		#endregion

		#region �ܱ����ķ���

		/// <summary>
		/// ��ȡ��ʾ���ƣ�ʹ�� FunctionName ���ԣ����硰COUNT(...)����
		/// </summary>
		/// <returns>��ʾ���ơ�</returns>
		protected virtual String GetDisplayName()
		{
			if (!Distinct)
			{
				return String.Format("{0}(...)", FunctionName);
			}
			else
			{
				return String.Format("{0}({1} ...))", FunctionName, AggregationAttribute.KEY_WORD_DISTINCT);
			}
		}

		#endregion

		#region �����Ա

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public abstract String Name { get; }

		#endregion
	}
}