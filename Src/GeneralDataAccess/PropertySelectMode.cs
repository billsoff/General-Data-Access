#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PropertySelectMode.cs
// �ļ���������������ѡ��ģʽ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110517
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
	/// ����ѡ��ģʽ��
	/// </summary>
	public enum PropertySelectMode
	{
		/// <summary>
		/// ѡ��ʵ���е��������ԡ�
		/// </summary>
		AllFromSchema,

		/// <summary>
		/// ѡ��ʵ���е����з��ӳټ������ԡ�
		/// </summary>
		AllExceptLazyLoadFromSchema,

		/// <summary>
		/// ��ѡ�����ԣ���ʵ����ӳ��ı�ֻ������ FROM �Ӿ��У�һ������Ϊ WHERE �� ORDER �Ӿ�Ҫ�õ����е��ֶΣ�����
		/// </summary>
		LoadSchemaOnly,

		/// <summary>
		/// ѡ�����ԡ�
		/// </summary>
		Property,

		/// <summary>
		/// ѡ��������
		/// </summary>
		PrimaryKey
	}
}