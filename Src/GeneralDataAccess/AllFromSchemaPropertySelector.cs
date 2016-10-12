#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AllFromSchemaPropertySelector.cs
// �ļ�����������ָʾѡ�����е����ԡ�
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
	/// ָʾѡ�����е����ԡ�
	/// </summary>
	[Serializable]
	internal sealed class AllFromSchemaPropertySelector : PropertySelector
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ�����ͣ�ѡ��Ŀ��ʵ���е��������ԡ�
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		public AllFromSchemaPropertySelector(Type entityType)
			: base(entityType)
		{
		}

		/// <summary>
		/// ���캯������������������������Ϊ�ⲿ���ã�ѡ����ⲿ�����е��������ԡ�
		/// </summary>
		/// <param name="chain">��������</param>
		public AllFromSchemaPropertySelector(IPropertyChain chain)
			: base(chain)
		{
			#region ǰ������

			Debug.Assert(!chain.IsPrimitive, "������ AllFromSchemaPropertySelector ʵ��ʱ������������ chain ����ӳ��Ϊ�ⲿ�������ԡ�");

			#endregion
		}

		/// <summary>
		/// ���캯��������������������ȷ���������Ե�ʵ�塣
		/// </summary>
		/// <param name="builder">��������������</param>
		public AllFromSchemaPropertySelector(IPropertyChainBuilder builder)
			: base(builder)
		{
		}

		#endregion

		/// <summary>
		/// ��ȡ��ʾ���ơ�
		/// </summary>
		public override String DisplayName
		{
			get
			{
				return Name + ".*";
			}
		}

		/// <summary>
		/// AllFromSchema��
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.AllFromSchema; }
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
		/// ѡ������λ�������������Ժ͵�ǰ�������������������ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return Contains(property) || OwnProperty(property);
		}
	}
}