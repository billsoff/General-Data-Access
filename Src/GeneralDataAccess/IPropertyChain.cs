#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����IPropertyChain.cs
// �ļ�������������������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110527
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
	/// ��������
	/// </summary>
	public interface IPropertyChain
	{
		#region ����

		/// <summary>
		/// ��ȡ����������ȡ�
		/// </summary>
		Int32 Depth { get; }

		/// <summary>
		/// ��ȡ����ȫ���ƣ���ʵ����������ʼ�Ե�ָ��������б���
		/// </summary>
		String FullName { get; }

		/// <summary>
		/// ��ȡͷ����
		/// </summary>
		IPropertyChain Head { get; }

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ��ʵ���б����ԡ�
		/// </summary>
		Boolean IsChildren { get; }

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ�ΪĿ��ʵ���ֱ�����ԡ�
		/// </summary>
		Boolean IsImmediateProperty { get; }

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ�Ϊ�������ԡ�
		/// </summary>
		Boolean IsPrimitive { get; }

		/// <summary>
		/// ��ȡ�������ơ�
		/// </summary>
		String Name { get; }

		/// <summary>
		/// ��ȡ��һ�����Խڵ㡣
		/// </summary>
		IPropertyChain Next { get; }

		/// <summary>
		/// ��ȡǰһ�����Խڵ㡣
		/// </summary>
		IPropertyChain Previous { get; }

		/// <summary>
		/// ��ȡ����·���б�
		/// </summary>
		String[] PropertyPath { get; }

		/// <summary>
		/// ��ȡĿ��ʵ�����͡�
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// ��ȡ�������͡�
		/// </summary>
		Type PropertyType { get; }

		#endregion

		#region ����

		/// <summary>
		/// �ж��Ƿ�����ʵ��ܹ���
		/// </summary>
		/// <param name="schema">Ҫ�жϵ�ʵ��ܹ���</param>
		/// <returns>������ڸ�ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		Boolean BelongsTo(EntitySchema schema);

		/// <summary>
		/// �ж��Ƿ��������ԡ�
		/// </summary>
		/// <param name="property">Ҫ�жϵ����ԡ�</param>
		/// <returns>������ڸ����ԣ��򷵻� true�����򷵻� false��</returns>
		Boolean BelongsTo(EntityProperty property);

		/// <summary>
		/// �ж��Ƿ����ʵ��ܹ���
		/// </summary>
		/// <param name="schema">Ҫ�жϵ�ʵ��ܹ���</param>
		/// <returns>���������ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		Boolean Contains(EntitySchema schema);

		/// <summary>
		/// �ж��Ƿ�������ԡ�
		/// </summary>
		/// <param name="property">Ҫ�жϵ����ԡ�</param>
		/// <returns>������������ԣ��򷵻� true�����򷵻� false��</returns>
		Boolean Contains(EntityProperty property);

		/// <summary>
		/// ��ȡ��ǰʵ���ĸ�������ʵ���ж����� Head �� Previous ���ԣ��� Next ����Ϊ�ա�
		/// </summary>
		/// <returns>��ǰʵ���ĸ�����</returns>
		IPropertyChain Copy();

		/// <summary>
		/// ��ȡʵ�������Ե�ֵ��
		/// </summary>
		/// <param name="entity">ʵ�壬���ͱ�������������Ŀ��ʵ��������ͬ��</param>
		/// <returns>���Ե�ֵ�����������δ���أ�DBEmpty�����򷵻� null��</returns>
		Object GetPropertyValue(Object entity);

		/// <summary>
		/// ��ȡʵ�������Ե�ֵ��
		/// </summary>
		/// <typeparam name="TResult">�������͡�</typeparam>
		/// <param name="entity">ʵ�壬���ͱ�������������Ŀ��ʵ��������ͬ��</param>
		/// <returns>���Ե�ֵ�����������δ���أ�DBEmpty�����򷵻� null��</returns>
		TResult GetPropertyValue<TResult>(Object entity);

		/// <summary>
		/// �ж��Ƿ�ӵ�����ԡ�
		/// </summary>
		/// <param name="property">���ԡ�</param>
		/// <returns>���ӵ�и����ԣ��򷵻� true�����򷵻� false��</returns>
		Boolean OwnProperty(EntityProperty property);

		/// <summary>
		/// ��ǰ������������������������ΪĿ�������������͡�
		/// </summary>
		/// <param name="target">Ŀ����������</param>
		/// <returns>������´�������������</returns>
		IPropertyChain Preppend(IPropertyChain target);

		#endregion
	}
}