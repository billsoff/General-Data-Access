#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupSchemaFilterCompilationContext.cs
// �ļ�����������������ʵ��ܹ����������뻷�������� HAVING ��������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110630
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ������ʵ��ܹ����������뻷�������� HAVING ��������
	/// </summary>
	internal class GroupSchemaFilterCompilationContext : FilterCompilationContext
	{
		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ��ܹ���
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		public GroupSchemaFilterCompilationContext(GroupSchema schema)
			: base(schema)
		{
		}

		/// <summary>
		/// ���캯��������ʵ��ܹ��Ͳ���ǰ׺��
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <param name="parameterPrifix">����ǰ׺��</param>
		public GroupSchemaFilterCompilationContext(GroupSchema schema, String parameterPrifix)
			: base(schema, parameterPrifix)
		{
		}

		#endregion

		/// <summary>
		/// ��������ǰ׺��ph__����
		/// </summary>
		protected override String ParameterNamePrefix
		{
			get { return CommonPolicies.HavingFilterParameterNamePrefix; }
		}
	}
}