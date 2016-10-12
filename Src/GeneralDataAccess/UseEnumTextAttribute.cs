#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����UseEnumTextAttribute.cs
// �ļ������������������������Ϊö����������ϣ�����ö������ת��Ϊ�ı��������ö�����ͱ���Ϊ��ײ���������ͣ���ʹ�ô˱�ǡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110802
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
	/// �������������Ϊö����������ϣ�����ö������ת��Ϊ�ı��������ö�����ͱ���Ϊ��ײ���������ͣ���ʹ�ô˱�ǡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class UseEnumTextAttribute : Attribute
	{
		#region ˽���ֶ�

		private EnumTextOption m_text;
		private EnumParseErrorFollowup m_onParseError;
		private Boolean m_parseCaseSensitive;
		private Object m_defaultValue;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public UseEnumTextAttribute()
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�������ı�ѡ�Ĭ��ֵ�� Name��
		/// </summary>
		public EnumTextOption Text
		{
			get { return m_text; }
			set { m_text = value; }
		}

		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾ����ʱ�Ƿ����ִ�Сд��Ĭ��Ϊ false�������ִ�Сд����
		/// </summary>
		public Boolean ParseCaseSensitive
		{
			get { return m_parseCaseSensitive; }
			set { m_parseCaseSensitive = value; }
		}

		/// <summary>
		/// ��ȡ�����õ�����ʧ�ܺ���ж����ԣ�Ĭ���� Broadcast��
		/// </summary>
		public EnumParseErrorFollowup OnParseError
		{
			get { return m_onParseError; }
			set { m_onParseError = value; }
		}

		/// <summary>
		/// ��ȡ������Ĭ��ֵ��������ʧ��ʱ��� OnParseError ��ֵΪ SetDefault��������Ϊ��ֵ��Ĭ��Ϊ 0��
		/// </summary>
		public Object DefaultValue
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}

		#endregion
	}
}