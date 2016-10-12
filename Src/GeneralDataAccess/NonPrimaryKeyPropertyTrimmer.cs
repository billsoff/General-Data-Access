#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����NonPrimaryKeyPropertyTrimmer.cs
// �ļ�������������ʵ��ܹ����Ƴ����������ԡ�
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
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʵ��ܹ����Ƴ����������ԡ�
	/// </summary>
	[Serializable]
	internal sealed class NonPrimaryKeyPropertyTrimmer : PropertyTrimmer
	{
		#region ˽���ֶ�

		private readonly IPropertyChain m_propertyChain;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����ָʾ�Ƴ�ʵ����ֱ���ķ��������ԡ�
		/// </summary>
		public NonPrimaryKeyPropertyTrimmer()
		{
		}

		/// <summary>
		/// ���캯���������ⲿ���������������Ϊ�գ���ָʾ�Ƴ�ʵ���е�ֱ���ķ��������ԡ�
		/// </summary>
		/// <param name="propertyChain">�ⲿ������������</param>
		public NonPrimaryKeyPropertyTrimmer(IPropertyChain propertyChain)
		{
			#region ǰ������

			Debug.Assert((propertyChain == null) || !propertyChain.IsPrimitive, "����������Ӧ propertyChain Ϊ�������ԡ�");

			#endregion

			m_propertyChain = propertyChain;
		}

		/// <summary>
		/// ���캯���������ⲿ������������������
		/// </summary>
		/// <param name="builder">�ⲿ������������������</param>
		public NonPrimaryKeyPropertyTrimmer(IPropertyChainBuilder builder)
			: this(builder.Build())
		{
		}

		#endregion

		#region PropertyTrimmer ��Ա

		/// <summary>
		/// ��ȡ��ʾ���ƣ����ڵ��ԡ�
		/// </summary>
		public override String DisplayName
		{
			get
			{
				if (m_propertyChain != null)
				{
					return String.Format("�޼��� {0} �����з���������", m_propertyChain.FullName);
				}
				else
				{
					return "�޼���ʵ����ֱ�������з���������";
				}
			}
		}

		/// <summary>
		/// �Ƴ�ָ���ⲿ���õķ��������ԡ�
		/// </summary>
		/// <param name="property">���ԡ�</param>
		/// <returns>ָʾ�Ƿ��Ƴ�ָ�������ԡ�</returns>
		public override Boolean TrimOff(EntityProperty property)
		{
			if (property.IsPrimaryKey)
			{
				return false;
			}

			if (m_propertyChain != null)
			{
				return m_propertyChain.OwnProperty(property);
			}
			else
			{
				return property.PropertyChain.IsImmediateProperty;
			}
		}

		#endregion
	}
}