#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeItemSettings.cs
// �ļ�������������Ը���ʵ����һ����ʽ�ܹ��Ĺ�������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110708
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
	/// ��Ը���ʵ����һ����ʽ�ܹ��Ĺ�������
	/// </summary>
	[Serializable]
	public sealed class CompositeItemSettings
	{
		#region ˽���ֶ�

		private readonly CompositeSettings m_compositeSettings;
		private readonly String m_propertyName;

		private CompositeBuilderStrategy m_select;
		private Filter m_where;
		private Filter m_having;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ù������������б���������ơ�
		/// </summary>
		/// <param name="settings">�б�</param>
		/// <param name="propertyName">�������ơ�</param>
		internal CompositeItemSettings(CompositeSettings settings, String propertyName)
		{
			m_compositeSettings = settings;
			m_propertyName = propertyName;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ��ǰʵ�����ڵ��б�
		/// </summary>
		public CompositeSettings CompositeSettings
		{
			get { return m_compositeSettings; }
		}

		/// <summary>
		/// ��ȡ����ʵ����������ơ�
		/// </summary>
		public String PropertyName
		{
			get { return m_propertyName; }
		}

		/// <summary>
		/// ��ȡ�����ü��ز��ԡ�
		/// </summary>
		public CompositeBuilderStrategy Select
		{
			get { return m_select; }
			set { m_select = value; }
		}

		/// <summary>
		/// ��ȡ������ WHERE ��������
		/// </summary>
		public Filter Where
		{
			get { return m_where; }
			set { m_where = value; }
		}

		/// <summary>
		/// ��ȡ������ HAVING ��������
		/// </summary>
		public Filter Having
		{
			get { return m_having; }
			set { m_having = value; }
		}

		#endregion
	}
}