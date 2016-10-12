#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PrecedenceAttribute.cs
// �ļ�����������ָʾ�����������������ȼ���
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
	/// ָʾ�����������������ȼ���
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	internal sealed class PrecedenceAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly Int32 m_leftPrecedence;
		private readonly Int32 m_rightPrecedence;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯�������������ȼ���Ϊ FilterOperatorPrecedences.BELOW_NORMAL��
		/// </summary>
		internal PrecedenceAttribute()
			: this(FilterOperatorPrecedences.BELOW_NORMAL, FilterOperatorPrecedences.BELOW_NORMAL)
		{
		}

		/// <summary>
		/// ���캯���������������ȼ���
		/// </summary>
		/// <param name="leftPrecedence">�����ȼ���</param>
		/// <param name="rightPrecedence">�����ȼ���</param>
		public PrecedenceAttribute(Int32 leftPrecedence, Int32 rightPrecedence)
		{
			m_leftPrecedence = leftPrecedence;
			m_rightPrecedence = rightPrecedence;
		}

		#endregion

		#region ��̬��Ա

		/// <summary>
		/// ����Ĭ��ʵ����
		/// </summary>
		/// <returns>�����õ�Ĭ��ʵ����</returns>
		public static PrecedenceAttribute CreateDefault()
		{
			return new PrecedenceAttribute();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�����ȼ���
		/// </summary>
		public Int32 LeftPrecedence
		{
			get { return m_leftPrecedence; }
		}

		/// <summary>
		/// ��ȡ�����ȼ���
		/// </summary>
		public Int32 RightPrecedence
		{
			get { return m_rightPrecedence; }
		}

		#endregion
	}
}