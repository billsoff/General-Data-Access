#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PropertyTrimmer.cs
// �ļ���������������ָʾ��ʵ��ܹ���ϲ������Ƴ��ض������ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110601
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
	/// ����ָʾ��ʵ��ܹ���ϲ������Ƴ��ض������ԡ�
	/// <para>ʵ����Ӧ֧�����л���</para>
	/// </summary>
	[Serializable]
	public abstract class PropertyTrimmer
	{
		#region ��������

		/// <summary>
		/// ���������޼���֧�ַ�������Ե��޼�����
		/// </summary>
		/// <returns>�����õ��޼�����</returns>
		public static PropertyTrimmer CreateGroupIncapableTrimmer()
		{
			return new GroupIncapablePropertyTrimmer();
		}

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		protected PropertyTrimmer()
		{
		}

		#endregion

		/// <summary>
		/// ��ȡ��ʾ���ƣ����ڵ��ԡ�
		/// </summary>
		public virtual String DisplayName
		{
			get
			{
				return ToString();
			}
		}

		/// <summary>
		/// ָʾ�Ƿ��ʵ��ܹ�������Ƴ������ԣ��������أ���
		/// </summary>
		/// <param name="property">ʵ��ܹ����ԡ�</param>
		/// <returns>���Ҫ�Ƴ������ԣ��򷵻� true�����򷵻� false��</returns>
		public abstract Boolean TrimOff(EntityProperty property);
	}
}