#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SumAttribute.cs
// �ļ��������������ڱ�Ƕ��н�����;ۺϼ�������ԡ�
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
	/// ���ڱ�Ƕ��н�����;ۺϼ�������ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class SumAttribute : AggregationAttribute
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ��͵����ԡ�
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		public SumAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
		}

		#endregion

		/// <summary>
		/// SUM ( [ ALL | DISTINCT ] expression )
		/// </summary>
		protected override String FunctionName
		{
			get
			{
				return "SUM";
			}
		}

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public override String Name
		{
			get { return Distinct ? "Sum Distinct" : "Sum"; }
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