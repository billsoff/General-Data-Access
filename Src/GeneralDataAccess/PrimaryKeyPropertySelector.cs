#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PrimaryKeyPropertySelector.cs
// �ļ�����������ֻѡ��������
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
	/// ֻѡ��������
	/// </summary>
	[Serializable]
	internal sealed class PrimaryKeyPropertySelector : PropertySelector
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ŀ��ʵ�����ͣ�ѡ��Ŀ��ʵ���е�������
		/// </summary>
		/// <param name="entityType"></param>
		public PrimaryKeyPropertySelector(Type entityType)
			: base(entityType)
		{
		}

		/// <summary>
		/// ���캯��������������������ѡ������������е�������
		/// </summary>
		/// <param name="chain"></param>
		public PrimaryKeyPropertySelector(IPropertyChain chain)
			: base(chain)
		{
			#region ǰ������

			Debug.Assert(!chain.IsPrimitive, "������ PrimaryKeyPropertySelector ʵ��ʱ������������ chain ����ӳ�䵽�������ԡ�");

			#endregion
		}

		/// <summary>
		/// ���캯����ͨ�������������������������ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		public PrimaryKeyPropertySelector(IPropertyChainBuilder builder)
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
				return Name + ".[Primary Key]";
			}
		}

		/// <summary>
		/// PrimaryKey��
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.PrimaryKey; }
		}

		/// <summary>
		/// ���Ǽ���λ���������е�ʵ��ܹ��Ĺ������ԡ�
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean SelectNothingFromImpl(EntitySchema schema)
		{
			return !Contains(schema);
		}

		/// <summary>
		/// �������й������Ժ͵�ǰ���������е��������ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return Contains(property) || (property.IsPrimaryKey && OwnProperty(property));
		}
	}
}