#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AllExceptLazyLoadFromSchemaPropertySelector.cs
// �ļ�����������ѡ��ʵ���е����з��ӳټ������ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110525
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
	/// ѡ��ʵ���е����з��ӳټ������ԡ�
	/// </summary>
	[Serializable]
	internal sealed class AllExceptLazyLoadFromSchemaPropertySelector : PropertySelector
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ�����ͣ�ѡ��Ŀ��ʵ���г����Ϊ LazyLoad ֮����������ԡ�
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		public AllExceptLazyLoadFromSchemaPropertySelector(Type entityType)
			: base(entityType)
		{
		}

		/// <summary>
		/// ���캯������������������������Ϊ�ⲿ���ã�ѡ����ⲿ�����г����Ϊ LazyLoad ֮����������ԡ�
		/// </summary>
		/// <param name="chain">��������</param>
		public AllExceptLazyLoadFromSchemaPropertySelector(IPropertyChain chain)
			: base(chain)
		{
			#region ǰ������

			Debug.Assert(
					!chain.IsPrimitive && !chain.IsChildren,
					"������ AllExceptLazyLoadFromSchemaPropertySelector ʵ��ʱ������������ chain ����ӳ��Ϊ�ⲿ�������ԡ�"
				);

			#endregion
		}

		/// <summary>
		/// ���캯�������������������������ɵ�����������Ϊ�ⲿ�������ԣ�ѡ����ⲿ�����г����Ϊ LazyLoad ֮����������ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		public AllExceptLazyLoadFromSchemaPropertySelector(IPropertyChainBuilder builder)
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
				return String.Format("{0}.* - {0}.[Lazy Load]", Name);
			}
		}

		/// <summary>
		/// AllExceptLazyLoadFromSchema��
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.AllExceptLazyLoadFromSchema; }
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
		/// ѡ������λ�������������Ժ͵�ǰ���������������ķ��ӳټ������ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return Contains(property) || (OwnProperty(property) && !property.LazyLoad);
		}
	}
}