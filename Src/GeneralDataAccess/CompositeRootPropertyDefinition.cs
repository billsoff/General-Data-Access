#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeRootPropertyDefinition.cs
// �ļ���������������ʵ������Զ��塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110708
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����ʵ������Զ��塣
	/// </summary>
	internal sealed class CompositeRootPropertyDefinition : CompositePropertyDefinition
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������������Ϣ��
		/// </summary>
		/// <param name="definition">����ʵ�嶨�塣</param>
		/// <param name="pf">����������Ϣ��</param>
		public CompositeRootPropertyDefinition(CompositeDefinition definition, PropertyInfo pf)
			: base(definition, pf)
		{
		}

		#endregion
	}
}