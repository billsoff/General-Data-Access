#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CountAttribute.cs
// �ļ��������������ڱ�Ƕ��л��¼���м����ۺϼ�������ԡ�
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
	/// ���ڱ�Ƕ��л��¼���м����ۺϼ�������ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class CountAttribute : AggregationAttribute
	{
		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public CountAttribute()
			: this((String[])null)
		{
		}

		/// <summary>
		/// ���캯��������Ҫ����������·����
		/// </summary>
		/// <param name="propertyPath">Ҫ����������·����</param>
		public CountAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
			AcceptDbType = true;
			DbType = DbType.Int32;
		}

		#endregion

		/// <summary>
		/// ��������Ϊ�գ��Լ�¼���м�������
		/// </summary>
		protected override Boolean AllowNullColumnName
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// COUNT ( { [ [ ALL | DISTINCT ] expression ] | * } )
		/// </summary>
		protected override String FunctionName
		{
			get
			{
				return "COUNT";
			}
		}

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public override String Name
		{
			get { return Distinct ? "Count Distinct" : "Count"; }
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