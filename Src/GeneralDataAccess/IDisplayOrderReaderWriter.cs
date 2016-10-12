#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IDisplayOrderReaderWriter.cs
// �ļ�������������ʾ��Ŷ�д���������� DisplayOrderAllocator��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110331
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
	/// ��ʾ��Ŷ�д���������� DisplayOrderAllocator��
	/// </summary>
	/// <typeparam name="TEntity">ʵ�����͡�</typeparam>
	public interface IDisplayOrderReaderWriter<TEntity> where TEntity : class
	{
		/// <summary>
		/// ��ȡ�����ʾ��š�
		/// </summary>
		/// <param name="item">�</param>
		/// <returns>��ʾ��š�</returns>
		Int32 GetDisplayOrder(TEntity item);

		/// <summary>
		/// ���������ʾ��š�
		/// </summary>
		/// <param name="item">�</param>
		/// <param name="displayOrder">Ҫ���õ���ʾ��š�</param>
		void SetDisplayOrder(TEntity item, Int32 displayOrder);
	}
}