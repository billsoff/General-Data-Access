#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ActualPropertySelector.cs
// �ļ�����������ѡ��һ�����ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110524
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
	/// ѡ��һ�����ԡ�
	/// </summary>
	[Serializable]
	internal sealed class ActualPropertySelector : PropertySelector
	{
		#region ���캯��

		/// <summary>
		/// ���캯����������������
		/// </summary>
		/// <param name="chain">��������</param>
		public ActualPropertySelector(IPropertyChain chain)
			: base(chain)
		{
		}

		/// <summary>
		/// ���캯����������������
		/// </summary>
		/// <param name="builder">��������������</param>
		public ActualPropertySelector(IPropertyChainBuilder builder)
			: base(builder)
		{
		}

		#endregion

		/// <summary>
		/// Property��
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.Property; }
		}

		/// <summary>
		/// ����λ���������е�ʵ��ܹ����ǻ�ѡ����������ԡ�
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean SelectNothingFromImpl(EntitySchema schema)
		{
			return !Contains(schema);
		}

		/// <summary>
		/// ѡ������λ���������е����ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return Contains(property);
		}
	}
}