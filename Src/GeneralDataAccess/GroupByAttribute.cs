#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupByAttribute.cs
// �ļ��������������ڱ����Ϊ�������ݵ����ԡ�
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ڱ����Ϊ�������ݵ����ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class GroupByAttribute : AggregationAttribute
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ��������Ե�·����
		/// </summary>
		/// <param name="propertyPath">Ҫ��������Ե�·����</param>
		public GroupByAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
		}

		#endregion

		/// <summary>
		/// ָʾ��ǵ�����Ϊ�����
		/// </summary>
		public override Boolean IsGroupItem
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// ��ȡ�ۺϱ��ʽ��
		/// </summary>
		/// <param name="columnName">�е��޶�����</param>
		/// <returns>ԭ�����ء�</returns>
		public override String GetAggregationExpression(String columnName)
		{
			return columnName;
		}

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public override String Name
		{
			get { return "GroupBy"; }
		}

		/// <summary>
		/// �ַ�����ʾ��
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return "GROUP BY ...";
		}
	}
}