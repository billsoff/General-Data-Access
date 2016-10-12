#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AutoAssembling.cs
// �ļ��������������ڹ�����ʵ��װ�䷽�롣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110727
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
	/// ���ڹ�����ʵ��װ�䷽�롣
	/// </summary>
	public static class AutoAssembling
	{
		/// <summary>
		/// ָʾװ�����еģ�������ӵģ���ʵ�塣
		/// </summary>
		public static AssemblyPolicy AllChildren
		{
			get { return AssemblyPolicy.CreateLoadAllChildren(); }
		}

		/// <summary>
		/// ����Ҫ���ص������ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChainBuilder builder)
		{
			return Children(builder.Build(), (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <param name="strategyOption">����ѡ�</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, LoadStrategyOption strategyOption)
		{
			return Children(builder.Build(), strategyOption);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <param name="level">���ؼ���</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, Int32 level)
		{
			return Children(builder.Build(), level);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <param name="builderStrategy">���ز��ԡ�</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, CompositeBuilderStrategy builderStrategy)
		{
			return Children(builder.Build(), builderStrategy);
		}

		/// <summary>
		/// ����Ҫ���ص������ԡ�
		/// </summary>
		/// <param name="chain">�����ԡ�</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChain chain)
		{
			return Children(chain, (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="chain">�����ԡ�</param>
		/// <param name="strategyOption">����ѡ�</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChain chain, LoadStrategyOption strategyOption)
		{
			return Children(chain.PropertyPath, strategyOption);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="chain">�����ԡ�</param>
		/// <param name="level">���ؼ���</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChain chain, Int32 level)
		{
			return Children(chain.PropertyPath, level);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="chain">�����ԡ�</param>
		/// <param name="builderStrategy">���ز��ԡ�</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(IPropertyChain chain, CompositeBuilderStrategy builderStrategy)
		{
			return Children(chain.PropertyPath, builderStrategy);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="propertyPath">������·����</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(String[] propertyPath)
		{
			return Children(propertyPath, (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="propertyPath">������·����</param>
		/// <param name="strategyOption">����ѡ�</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(String[] propertyPath, LoadStrategyOption strategyOption)
		{
			AssemblyPolicyBuilder builder = new AssemblyPolicyBuilder();

			builder.Children(propertyPath, strategyOption);

			return builder;
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="propertyPath">������·����</param>
		/// <param name="level">���ؼ���</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(String[] propertyPath, Int32 level)
		{
			AssemblyPolicyBuilder builder = new AssemblyPolicyBuilder();

			builder.Children(propertyPath, level);

			return builder;
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="propertyPath">������·����</param>
		/// <param name="builderStrategy">���ز��ԡ�</param>
		/// <returns>װ�䷽����������</returns>
		public static AssemblyPolicyBuilder Children(String[] propertyPath, CompositeBuilderStrategy builderStrategy)
		{
			AssemblyPolicyBuilder builder = new AssemblyPolicyBuilder();

			builder.Children(propertyPath, builderStrategy);

			return builder;
		}
	}
}