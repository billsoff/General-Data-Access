#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeTrimmingBuilderStrategy.cs
// �ļ�����������ȥ��Ŀ������е�ĳЩ���ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110531
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ȥ��Ŀ������е�ĳЩ���ԡ�
	/// </summary>
	[Serializable]
	internal sealed class CompositeTrimmingBuilderStrategy : CompositeBuilderStrategy
	{
		#region ˽���ֶ�

		private readonly CompositeBuilderStrategy m_builderStrategy;
		private readonly PropertyTrimmer[] m_propertyTrimmers;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ�޼������������Ժ�Ҫ�޼����������б�
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼������������ԡ�</param>
		/// <param name="trimmingProperties">Ҫ�޼��������б�</param>
		public CompositeTrimmingBuilderStrategy(CompositeBuilderStrategy builderStrategyToTrim, params IPropertyChainBuilder[] trimmingProperties)
			: this(builderStrategyToTrim, new ActualPropertyTrimmer(trimmingProperties))
		{
		}

		/// <summary>
		/// ���캯��������Ҫ�޼������ɲ��Ժ�Ҫ�޼����������б�
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼��������ɲ��ԡ�</param>
		/// <param name="trimmingProperties">Ҫ�޼����������б�</param>
		public CompositeTrimmingBuilderStrategy(CompositeBuilderStrategy builderStrategyToTrim, params IPropertyChain[] trimmingProperties)
			: this(builderStrategyToTrim, new ActualPropertyTrimmer(trimmingProperties))
		{
		}

		/// <summary>
		/// ���캯��������Ҫ�޼������ɲ��Ժ��޼�����
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼������ɲ��ԡ�</param>
		/// <param name="propertyTrimmers">�޼����б�</param>
		public CompositeTrimmingBuilderStrategy(CompositeBuilderStrategy builderStrategyToTrim, params PropertyTrimmer[] propertyTrimmers)
		{
			#region ǰ������

			Debug.Assert(builderStrategyToTrim != null, "Ҫ�޼������ɲ��Բ��� builderStrategyToTrim ����Ϊ�ա�");
			Debug.Assert((propertyTrimmers != null) && (propertyTrimmers.Length != 0), "���ɲ����޼������� propertyTrimmer ����Ϊ�ա�");

			#endregion

			m_builderStrategy = builderStrategyToTrim;
			m_propertyTrimmers = propertyTrimmers;
		}

		#endregion

		#region ����

		/// <summary>
		/// ��ȡ����ⲿ���ü���
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return m_builderStrategy.MaxForeignReferenceLevel;
			}
		}

		/// <summary>
		/// ָʾ�Ƿ�ѡ����ӳټ���֮����������ԡ�
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// ָʾ�Ƿ����ָ����ʵ��ܹ���
		/// </summary>
		/// <param name="schema">Ҫ���ص�ʵ��ܹ���</param>
		/// <returns>���Ҫ����ָ����ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			return m_builderStrategy.LoadFromSchema(schema);
		}

		/// <summary>
		/// ָʾ�Ƿ񲻴�ʵ��ܹ���ѡ���κ����ԡ�
		/// </summary>
		/// <param name="schema">Ҫ�жϵ�ʵ��ܹ���</param>
		/// <returns>�����ѡ���κ����ԣ��򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			return m_builderStrategy.SelectNothingFrom(schema);
		}

		/// <summary>
		/// ѡ�����ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			foreach (PropertyTrimmer trimmer in m_propertyTrimmers)
			{
				if (trimmer.TrimOff(property))
				{
					return false;
				}
			}

			return m_builderStrategy.SelectProperty(property);
		}

		#endregion

		#region ������Ϣ

		/// <summary>
		/// ���������Ϣ��
		/// </summary>
		/// <returns></returns>
		public override String Dump()
		{
			String trimmerDescription;

			if (m_propertyTrimmers.Length == 1)
			{
				trimmerDescription = m_propertyTrimmers[0].DisplayName;
			}
			else
			{
				const String PADDING = "    ";
				StringBuilder buffer = new StringBuilder(Environment.NewLine);

				foreach (PropertyTrimmer trimmer in m_propertyTrimmers)
				{
					buffer.AppendLine(PADDING + trimmer.DisplayName);
				}

				trimmerDescription = buffer.ToString();
			}

			return String.Format(
					"{0}���Բ��� {1} �����޼����޼���Ϊ��{2}��\r\n���޼��Ĳ���Ϊ��{3}",
					GetType().FullName,
					m_builderStrategy.GetType().FullName,
					trimmerDescription,
					m_builderStrategy.Dump()
				);
		}

		#endregion
	}
}