#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����VarPAttribute.cs
// �ļ��������������ڱ�Ƕ��������巽��ľۺϼ�������ԡ�
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ڱ�Ƕ��������巽��ľۺϼ�������ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class VarPAttribute : AggregationAttribute
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ���оۺϼ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">���ԡ�</param>
		public VarPAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
		}

		#endregion

		/// <summary>
		/// VARP ( [ ALL | DISTINCT ] expression )
		/// </summary>
		protected override String FunctionName
		{
			get
			{
				return "VARP";
			}
		}

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public override String Name
		{
			get { return Distinct ? "VarP Distinct" : "VarP"; }
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