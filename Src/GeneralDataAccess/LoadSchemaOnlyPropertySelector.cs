#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����LoadSchemaOnlyPropertySelector.cs
// �ļ�����������ָʾ������ʵ�塣
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
	/// ָʾ������ʵ�塣
	/// </summary>
	[Serializable]
	internal sealed class LoadSchemaOnlyPropertySelector : PropertySelector
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������Ҫ���ص�Ŀ��ʵ�塣
		/// </summary>
		/// <param name="entityType"></param>
		public LoadSchemaOnlyPropertySelector(Type entityType)
			: base(entityType)
		{
		}

		/// <summary>
		/// ���캯��������Ҫ���ص��ⲿ�������ԡ�
		/// </summary>
		/// <param name="chain">��������</param>
		public LoadSchemaOnlyPropertySelector(IPropertyChain chain)
			: base(chain)
		{
			#region ǰ������

			Debug.Assert(!chain.IsPrimitive, "������ LoadSchemaOnlyPropertySelector ʵ��ʱ�� ���������� chain ����ӳ��Ϊ�ⲿ�������ԡ�");

			#endregion
		}

		/// <summary>
		/// ���캯��������Ҫ���ص��ⲿ�������ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		public LoadSchemaOnlyPropertySelector(IPropertyChainBuilder builder)
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
				return Name + ".[Nothing]";
			}
		}

		/// <summary>
		/// Nothing��
		/// </summary>
		public override PropertySelectMode SelectMode
		{
			get { return PropertySelectMode.LoadSchemaOnly; }
		}

		/// <summary>
		/// ���Ƿ��� true��
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns></returns>
		protected override Boolean SelectNothingFromImpl(EntitySchema schema)
		{
			return true;
		}

		/// <summary>
		/// ���Ƿ��� false��
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean SelectPropertyImpl(EntityProperty property)
		{
			return false;
		}
	}
}