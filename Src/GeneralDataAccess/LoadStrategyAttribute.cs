#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����LoadStrategyAttribute.cs
// �ļ�����������ʵ����ز��ԣ����������ݿ�Ự����ʱ�����δָ�����ԣ���ʹ�ñ����ʵ���ϵĲ��ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110519
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
	/// ʵ����ز��ԣ����������ݿ�Ự����ʱ�����δָ�����ԣ���ʹ�ñ����ʵ���ϵĲ��ԡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class LoadStrategyAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly LoadStrategyOption m_loadStrategyOption;
		private Int32 m_level;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ò���ѡ�
		/// </summary>
		/// <param name="option">����ѡ�</param>
		public LoadStrategyAttribute(LoadStrategyOption option)
		{
			m_loadStrategyOption = option;
		}

		/// <summary>
		/// ���캯�������ò���ѡ��ͼ���
		/// </summary>
		/// <param name="option">����ѡ�</param>
		/// <param name="level">���ؼ���</param>
		public LoadStrategyAttribute(LoadStrategyOption option, Int32 level)
			: this(option)
		{
			this.Level = level;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ���ز���ѡ�
		/// </summary>
		public LoadStrategyOption LoadStrategyOption
		{
			get { return m_loadStrategyOption; }
		}

		/// <summary>
		/// ��ȡ�����ü��ؼ��𣬽��� LoadStrategyOption Ϊ SpecifyLevel ʱ��Ч���Ҳ���С���㣬Ĭ��Ϊ�㡣
		/// </summary>
		public Int32 Level
		{
			get { return m_level; }
			set { m_level = value; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ���ݼ��ز��Ա�Ǵ���ʵ��ܹ�������ɲ��ԡ�
		/// </summary>
		/// <returns>�����õ�ʵ��ܹ�������ɲ��ԡ�</returns>
		public CompositeBuilderStrategy Create()
		{
			switch (m_loadStrategyOption)
			{
				case LoadStrategyOption.Auto:
				default:
					return CompositeBuilderStrategyFactory.Default;

				case LoadStrategyOption.SelfOnly:
					return CompositeBuilderStrategyFactory.SelfOnly;

				case LoadStrategyOption.OneLevel:
					return CompositeBuilderStrategyFactory.OneLevel;

				case LoadStrategyOption.UnlimitedLevel:
					return CompositeBuilderStrategyFactory.UnlimitedLevel;

				case LoadStrategyOption.SpecifyLevel:
					return CompositeBuilderStrategyFactory.Create(m_level);
			}
		}

		#endregion
	}
}