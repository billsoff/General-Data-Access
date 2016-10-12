#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����MinAttribute.cs
// �ļ��������������ڱ�Ƕ�������Сֵ�ľۺϼ�������ԡ�
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
	/// ���ڱ�Ƕ�������Сֵ�ľۺϼ�������ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class MinAttribute : AggregationAttribute
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ���оۺϼ�������ԡ�
		/// </summary>
		/// <param name="propertyPath">���ԡ�</param>
		public MinAttribute(params String[] propertyPath)
			: base(propertyPath)
		{
		}

		#endregion

		/// <summary>
		/// MIN ( [ ALL | DISTINCT ] expression )
		/// </summary>
		protected override String FunctionName
		{
			get
			{
				return "MIN";
			}
		}

		/// <summary>
		/// ��ȡ�ۺ����ơ�
		/// </summary>
		public override String Name
		{
			get { return Distinct ? "Min Distinct" : "Min"; }
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