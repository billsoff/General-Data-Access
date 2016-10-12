#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeAcceptAllExpandableBuilderStrategy.cs
// �ļ�����������ʵ��ܹ�������ɲ��ԣ��������дӵڶ���������չ�����ⲿ���á�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110505
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
	/// ʵ��ܹ�������ɲ��ԣ��������дӵڶ���������չ�����ⲿ���á�
	/// </summary>
	[Serializable]
	internal sealed class CompositeAcceptAllExpandableBuilderStrategy : CompositeBuilderStrategy
	{
		#region ��̬��Ա

		/// <summary>
		/// ʵ����
		/// </summary>
		public static readonly CompositeAcceptAllExpandableBuilderStrategy Instance = new CompositeAcceptAllExpandableBuilderStrategy();

		#endregion

		#region ����

		/// <summary>
		/// ����ⲿ���ü������С���㣬���ʾ�����ơ�
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get { return -1; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ�ѡ������Ϊ�ӳټ��ص��������ԡ�
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get { return false; }
		}

		/// <summary>
		/// ָʾ�Ƿ����ʵ��ܹ���
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>������ش�ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			// ����ȡ��һ���ⲿ����
			if (schema.Level <= (InitialLevel + 1))
			{
				return true;
			}

			// ������Ŀ����������ӳ��������Ƿ���Ϊ��ֹ��չ
			return MayExpand(schema) && !IsSchemaTypeRepetitive(schema);
		}

		/// <summary>
		/// ָʾ�Ƿ�ѡ�����ԣ���� SelectAllExceptLazyLoadProperties Ϊ true���򲻻���ô˷�������Ҫѡ���ʵ�����ԡ�
		/// </summary>
		/// <param name="property">ʵ�����ԡ�</param>
		/// <returns>���Ҫѡ������ԣ��򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			return !property.LazyLoad;
		}

		#endregion

		#region ��������

		/// <summary>
		/// �жϵ�ǰ��ʵ��ܹ��Ƿ���Ϊ��ֹ���������ⲿʵ�塣
		/// </summary>
		/// <param name="currentSchema">��ʼ��ʵ��ܹ���</param>
		/// <returns>�����ǰʵ��ܹ�ֱ�ӻ��ӵ�������ֹ��չ���򷵻� true�����򷵻� false��</returns>
		private Boolean MayExpand(EntitySchema currentSchema)
		{
			EntitySchemaRelation relation = currentSchema.LeftRelation;

			while (relation != null)
			{
				if (relation.ChildProperty.SuppressExpand)
				{
					return false;
				}

				relation = relation.ChildSchema.LeftRelation;
			}

			return true;
		}

		#endregion

		#region ���ڵ��Եķ���

		/// <summary>
		/// ��ȡ���ɲ��Ե���ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>���ɲ��Ե���ϸ��Ϣ��</returns>
		public override String Dump()
		{
			return String.Format(
					"{0}���������дӵڶ���������չ�����ⲿ���ã�ͬһ����·�����ظ�������չ�����ڶ����������ⲿ���á�",
					GetType().FullName
				);
		}

		#endregion
	}
}