#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IEntityDefinitionProvider.cs
// �ļ������������ṩʵ�嶨�塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110413
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
	/// �ṩʵ�嶨�塣
	/// </summary>
	internal interface IEntityDefinitionProvider
	{
		/// <summary>
		/// ��ȡʵ�嶨�塣
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>ʵ�嶨�塣</returns>
		EntityDefinition GetDefinition(Type entityType);
	}
}