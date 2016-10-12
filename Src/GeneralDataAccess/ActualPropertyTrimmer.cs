#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ActualPropertyTrimmer.cs
// �ļ����������������Ƴ�ָ�������ԡ�
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
	/// �����Ƴ�ָ�������ԡ�
	/// </summary>
	[Serializable]
	internal sealed class ActualPropertyTrimmer : PropertyTrimmer
	{
		#region ˽���ֶ�

		private readonly IPropertyChain[] m_properties;

		#endregion

		#region ���캯��

		/// <summary>
		/// ����Ҫ�Ƴ��������б�
		/// </summary>
		/// <param name="properties">Ҫ�Ƴ��������б�����Ϊ null ����б�</param>
		public ActualPropertyTrimmer(params IPropertyChain[] properties)
		{
			#region ǰ������

			Debug.Assert((properties != null) && (properties.Length != 0), "Ҫ�Ƴ������Լ��ϲ��� properties ����Ϊ�ջ���б�");

			#endregion

			m_properties = properties;
		}

		/// <summary>
		/// ����Ҫ�Ƴ��������б�
		/// </summary>
		/// <param name="allBuilders">Ҫ�Ƴ����������������б�����Ϊ null ����б�</param>
		public ActualPropertyTrimmer(params IPropertyChainBuilder[] allBuilders)
		{
			#region ǰ������

			Debug.Assert((allBuilders != null) && (allBuilders.Length != 0), "���� builders ����Ϊ�ջ���б�");

			#endregion

			m_properties = Array.ConvertAll<IPropertyChainBuilder, IPropertyChain>(
					allBuilders,
					delegate(IPropertyChainBuilder builder)
					{
						return builder.Build();
					}
				);
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
				const String PADDING = "    ";
				StringBuilder buffer = new StringBuilder();

				foreach (IPropertyChain chain in m_properties)
				{
					buffer.AppendLine(PADDING + chain.FullName);
				}

				return String.Format("�޼�ʵ���е��������ԣ�\r\n{0}", buffer.ToString());
			}
		}

		/// <summary>
		/// ��������Դ���Ҫ�Ƴ��������б��У���ָʾ�Ƴ�֮��
		/// </summary>
		/// <param name="property">���ԡ�</param>
		/// <returns>���Ҫ�Ƴ������ԣ��򷵻� true�����򷵻� false��</returns>
		public override Boolean TrimOff(EntityProperty property)
		{
			return (Array.IndexOf<IPropertyChain>(m_properties, property.PropertyChain) >= 0);
		}

		#endregion
	}
}