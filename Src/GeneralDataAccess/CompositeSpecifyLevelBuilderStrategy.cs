#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeSpecifyLevelBuilderStrategy.cs
// �ļ�����������ָ�����ؼ�������ɲ��ԣ����ָ���ļ���Ϊ���������޶����𣬴�ʱ��ͬһ����·���������ظ������ü��ص��ڶ�����������Ϊֹ��
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
	/// ָ�����ؼ�������ɲ��ԡ�
	/// <para>���ָ���ļ���Ϊ���������޶����𣬴�ʱ��ͬһ����·���������ظ������ü��ص��ڶ�����������Ϊֹ��</para>
	/// </summary>
	[Serializable]
	internal sealed class CompositeSpecifyLevelBuilderStrategy : CompositeBuilderStrategy
	{
		#region ˽���ֶ�

		private readonly Int32 m_level;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����������ʵ������
		/// </summary>
		public CompositeSpecifyLevelBuilderStrategy()
		{
		}

		/// <summary>
		/// ���ü��ؼ���
		/// </summary>
		/// <para>���ָ���ļ���Ϊ���������޶����𣬴�ʱ��ͬһ����·���������ظ������ü��ص��ڶ�����������Ϊֹ��</para>
		/// <param name="level">���ؼ���</param>
		public CompositeSpecifyLevelBuilderStrategy(Int32 level)
		{
			m_level = level;
		}

		#endregion

		#region ����

		/// <summary>
		/// ����ⲿ���ü������С���㣬���ʾ�����ơ�
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get { return m_level; }
		}

		/// <summary>
		/// ָʾ�Ƿ����ʵ��ܹ���
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>������ش�ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			// ������Ƽ�����ֻ���ص����Ƽ���
			if (m_level >= 0)
			{
				return (schema.Level <= (InitialLevel + m_level));
			}
			// �����жϼܹ������Ƿ��ظ����֣���ֹ���޵ݹ飩
			else
			{
				return !IsSchemaTypeRepetitive(schema);
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ�ѡ������Ϊ�ӳټ��ص��������ԡ�
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get { return false; }
		}

		/// <summary>
		/// ѡ���������ԡ�
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>���Ƿ��� false��</returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			return false;
		}

		/// <summary>
		/// ָʾ�Ƿ�ѡ�����ԣ���� SelectAllProperties Ϊ true���򲻻���ô˷�������Ҫѡ���ʵ�����ԡ�
		/// </summary>
		/// <param name="property">ʵ�����ԡ�</param>
		/// <returns>���Ҫѡ������ԣ��򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			return !property.LazyLoad;
		}


		#endregion

		#region ���ڵ��Եķ���

		/// <summary>
		/// ��ȡ���ɲ��Ե���ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>���ɲ��Ե���ϸ��Ϣ��</returns>
		public override String Dump()
		{
			String description;

			if (m_level > 0)
			{
				description = String.Format("չ������ {0} ���ⲿ����", m_level.ToString());
			}
			else if (m_level == 0)
			{
				description = "ֻ����ʵ������";
			}
			else
			{
				description = "չ�����е��ⲿ���ã�ͬһ����·�����ظ�������չ�����ڶ����������ⲿ����";
			}

			return String.Format(
					"{0}��{1}��",
					GetType().FullName,
					description
				);
		}

		#endregion
	}
}