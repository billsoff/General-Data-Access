#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GeneralFilterCompilationContext.cs
// �ļ�����������һ�����������뻷����ͨ��ʵ�֡�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110711
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
	/// һ�����������뻷����ͨ��ʵ�֡�
	/// </summary>
	internal class GeneralFilterCompilationContext : FilterCompilationContext
	{
		#region ���캯��

		/// <summary>
		/// ���캯�������üܹ���
		/// </summary>
		/// <param name="schema">�ܹ���</param>
		public GeneralFilterCompilationContext(IColumnLocatable schema)
			: base(schema)
		{
		}

		/// <summary>
		/// ���캯�������üܹ��Ͳ���ǰ׺��
		/// </summary>
		/// <param name="schema">�ܹ���</param>
		/// <param name="parameterPrefix">����ǰ׺��</param>
		public GeneralFilterCompilationContext(IColumnLocatable schema, String parameterPrefix)
			: base(schema, parameterPrefix)
		{
		}

		#endregion

		/// <summary>
		/// ����ǰ׺��pw__����
		/// </summary>
		protected override String ParameterNamePrefix
		{
			get { return CommonPolicies.WhereFilterParameterNamePrefix; }
		}
	}
}