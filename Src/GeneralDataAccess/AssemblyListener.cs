#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AssemblyListener.cs
// �ļ�������������ʵ��װ���������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110729
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
	/// ��ʵ��װ���������
	/// </summary>
	[Serializable]
	public abstract class AssemblyListener
	{
		#region ˽���ֶ�

		private readonly Type m_type;
		private readonly IPropertyChain m_propertyChain;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø����͡�
		/// </summary>
		/// <param name="type">�����͡�</param>
		protected AssemblyListener(Type type)
		{
			m_type = type;
		}

		/// <summary>
		/// ���캯����������ʵ�����ԡ�
		/// </summary>
		/// <param name="builder">��ʵ����������������</param>
		protected AssemblyListener(IPropertyChainBuilder builder)
			: this(builder.Build())
		{
		}

		/// <summary>
		/// ���캯����������ʵ�����ԡ�
		/// </summary>
		/// <param name="chain">��ʵ����������</param>
		protected AssemblyListener(IPropertyChain chain)
			: this(chain.Type)
		{
			m_propertyChain = chain;
		}

		#endregion

		#region ����������

		/// <summary>
		/// ��ȡ�����͡�
		/// </summary>
		protected Type Type
		{
			get { return m_type; }
		}

		/// <summary>
		/// ��ȡ��ʵ����������
		/// </summary>
		protected IPropertyChain PropertyChain
		{
			get { return m_propertyChain; }
		}

		#endregion

		/// <summary>
		/// ����ʼװ��ʱִ�д˷�����
		/// </summary>
		/// <param name="policy">װ�䷽�롣</param>
		/// <param name="parentEntities">��ʵ�弯�ϡ�</param>
		/// <param name="filter">��ʵ���������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
		public virtual void OnAssemblyStart(AssemblyPolicy policy, Object[] parentEntities, Filter filter, IDatabaseSession databaseSession)
		{
		}

		/// <summary>
		/// ����ʼװ����ʵ��ʱִ�д˷�����
		/// </summary>
		/// <param name="policy">װ�䷽�롣</param>
		/// <param name="chain">��ʵ�����������Ӹ�ʵ�忪ʼ����</param>
		/// <param name="parentEntities">��ʵ�弯�ϡ�</param>
		/// <param name="filter">��������</param>
		/// <param name="sorter">��������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
		public virtual void OnChildrenAssemblyStart(AssemblyPolicy policy, IPropertyChain chain, Object[] parentEntities, Filter filter, Sorter sorter, IDatabaseSession databaseSession)
		{
		}

		/// <summary>
		/// ����ʵ��װ�����ʱִ�д˷�����
		/// </summary>
		/// <param name="policy">װ�䷽�롣</param>
		/// <param name="chain">��ʵ�����������Ӹ�ʵ�忪ʼ����</param>
		/// <param name="parentEntities">��ʵ�弯�ϡ�</param>
		/// <param name="children">���ص���ʵ�弯�ϡ�</param>
		/// <param name="filter">���ڼ�����ʵ��Ĺ�������</param>
		/// <param name="sorter">���ڼ�����ʵ�����������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
		public virtual void OnChildrenAssemblyComplete(AssemblyPolicy policy, IPropertyChain chain, Object[] parentEntities, Object[] children, Filter filter, Sorter sorter, IDatabaseSession databaseSession)
		{
		}

		/// <summary>
		/// ��װ�����ʱִ�д˷�����
		/// </summary>
		/// <param name="policy">װ�䷽�롣</param>
		/// <param name="parentEnttities">��ʵ�弯�ϡ�</param>
		/// <param name="filter">��ʵ���������</param>
		/// <param name="databaseSession">���ݿ�Ự���档</param>
		public virtual void OnAssemblyComplete(AssemblyPolicy policy, Object[] parentEnttities, Filter filter, IDatabaseSession databaseSession)
		{
		}

		#region �����ķ���

		/// <summary>
		/// �������ʵ�����ʵ������ת��Ϊ������ʵ�������·����
		/// </summary>
		/// <param name="policy"></param>
		/// <param name="chain"></param>
		/// <returns></returns>
		protected static String[] Transform(AssemblyPolicy policy, IPropertyChain chain)
		{
			AssemblyChildrenEntry childrenEntry = policy.ChildrenEntries[chain.PropertyPath];
			List<String> propertyNames = new List<String>();
			AssemblyChildrenEntry current = childrenEntry;

			do
			{
				propertyNames.Add(current.ChildrenProperty.Name);
				current = current.Parent;
			} while (current != null);

			return propertyNames.ToArray();
		}

		#endregion
	}
}