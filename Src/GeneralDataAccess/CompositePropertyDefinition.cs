#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositePropertyDefinition.cs
// �ļ���������������ʵ�����Զ��塣
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
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����ʵ�����Զ��塣
	/// </summary>
	internal abstract class CompositePropertyDefinition
	{
		#region ˽���ֶ�

		private readonly CompositeDefinition m_composite;
		private readonly PropertyInfo m_propertyInfo;
		private readonly LoadStrategyAttribute m_loadStrategy;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������������Ϣ��
		/// </summary>
		/// <param name="definition">����ʵ�嶨�塣</param>
		/// <param name="pf">������Ϣ��</param>
		protected CompositePropertyDefinition(CompositeDefinition definition, PropertyInfo pf)
		{
			m_composite = definition;
			m_propertyInfo = pf;
			m_loadStrategy = (LoadStrategyAttribute)Attribute.GetCustomAttribute(pf, typeof(LoadStrategyAttribute));
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ʵ�嶨�塣
		/// </summary>
		internal CompositeDefinition Composite
		{
			get { return m_composite; }
		}

		/// <summary>
		/// ��ȡ�������ơ�
		/// </summary>
		public String Name
		{
			get { return m_propertyInfo.Name; }
		}

		/// <summary>
		/// ��ȡ�������͡�
		/// </summary>
		public Type Type
		{
			get { return m_propertyInfo.PropertyType; }
		}

		/// <summary>
		/// ��ȡ���ز��ԡ�
		/// </summary>
		public LoadStrategyAttribute LoadStrategy
		{
			get { return m_loadStrategy; }
		}

		/// <summary>
		/// ��ȡ������Ϣ��
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get { return m_propertyInfo; }
		}


		#endregion
	}
}