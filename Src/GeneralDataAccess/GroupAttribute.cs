#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupAttribute.cs
// �ļ��������������ڱ�Ƿ�����ʵ�壬ָʾҪ�����ʵ�����͡�
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
	/// ���ڱ�Ƿ�����ʵ�壬ָʾҪ�����ʵ�����͡�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class GroupAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly Type m_type;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ�����ʵ������͡�
		/// </summary>
		/// <param name="entityType">Ҫ�����ʵ������͡�</param>
		public GroupAttribute(Type entityType)
		{
			m_type = entityType;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡҪ�����ʵ������͡�
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion
	}
}