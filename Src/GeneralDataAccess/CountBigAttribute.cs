#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CountBigAttribute.cs
// �ļ��������������ڱ�Ƕ��л��¼���м����ۺϼ�������ԣ�����ֵΪ Int64��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110702
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
	/// ���ڱ�Ƕ��л��¼���м����ۺϼ�������ԣ�����ֵΪ Int64��
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class CountBigAttribute : AggregationAttribute
	{
		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public CountBigAttribute()
			: this((String[])null)
		{
		}

		/// <summary>
		/// ���캯��������Ҫ����������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		public CountBigAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
			AcceptDbType = true;
			DbType = DbType.Int64;
		}

		#endregion

		/// <summary>
		/// ��������Ϊ�գ��Լ�¼��������
		/// </summary>
		protected override Boolean AllowNullColumnName
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// COUNT_BIG ( { [ ALL | DISTINCT ] expression } | * )
		/// </summary>
		protected override String FunctionName
		{
			get
			{
				return "COUNT_BIG";
			}
		}

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public override String Name
		{
			get { return Distinct ? "Count_Big Distinct" : "Count_Big"; }
		}

		/// <summary>
		/// �ַ�����ʾ��
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return GetDisplayName();
		}
	}
}