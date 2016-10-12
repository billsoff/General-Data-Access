#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����AssemblyPolicyBuilder.cs
// �ļ�����������װ�䷽����������
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
	/// װ�䷽����������
	/// </summary>
	public sealed class AssemblyPolicyBuilder
	{
		#region ˽���ֶ�

		private CompositeBuilderStrategy m_defaultBuilderStrategy;
		private readonly Dictionary<String, CompositeBuilderStrategy> m_children = new Dictionary<String, CompositeBuilderStrategy>();

		#endregion

		#region ��������

		#region ���빹��

		/// <summary>
		/// ����װ�䷽���Ĭ�ϼ��ز��ԡ�
		/// </summary>
		/// <param name="strategyOption">����ѡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Using(LoadStrategyOption strategyOption)
		{
			LoadStrategyAttribute attr = new LoadStrategyAttribute(strategyOption);
			CompositeBuilderStrategy strategy = attr.Create();

			return Using(strategy);
		}

		/// <summary>
		/// ����װ�䷽���Ĭ�ϼ��ز��ԡ�
		/// </summary>
		/// <param name="level">���ؼ���</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Using(Int32 level)
		{
			return Using(CompositeBuilderStrategyFactory.Create(level));
		}

		/// <summary>
		/// ����װ�䷽���Ĭ�ϼ��ز��ԣ�ע�⣬�˲��Բ��ܺ�������ء�
		/// </summary>
		/// <param name="builderStrategy">Ĭ�ϼ��ز��ԡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Using(CompositeBuilderStrategy builderStrategy)
		{
			m_defaultBuilderStrategy = builderStrategy;

			return this;
		}

		/// <summary>
		/// ����Ҫ���ص������ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(IPropertyChainBuilder builder)
		{
			return Children(builder.Build(), (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <param name="strategyOption">����ѡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, LoadStrategyOption strategyOption)
		{
			return Children(builder.Build(), strategyOption);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <param name="level">���ؼ���</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, Int32 level)
		{
			return Children(builder.Build(), level);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <param name="builderStrategy">���ز��ԡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(IPropertyChainBuilder builder, CompositeBuilderStrategy builderStrategy)
		{
			return Children(builder.Build(), builderStrategy);
		}

		/// <summary>
		/// ����Ҫ���ص������ԡ�
		/// </summary>
		/// <param name="chain">�����ԡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(IPropertyChain chain)
		{
			return Children(chain, (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="chain">�����ԡ�</param>
		/// <param name="strategyOption">����ѡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(IPropertyChain chain, LoadStrategyOption strategyOption)
		{
			return Children(chain.PropertyPath, strategyOption);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="chain">�����ԡ�</param>
		/// <param name="level">���ؼ���</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(IPropertyChain chain, Int32 level)
		{
			return Children(chain.PropertyPath, level);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="chain">�����ԡ�</param>
		/// <param name="builderStrategy">���ز��ԡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(IPropertyChain chain, CompositeBuilderStrategy builderStrategy)
		{
			return Children(chain.PropertyPath, builderStrategy);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="propertyPath">������·����</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(String[] propertyPath)
		{
			return Children(propertyPath, (CompositeBuilderStrategy)null);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="propertyPath">������·����</param>
		/// <param name="strategyOption">����ѡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(String[] propertyPath, LoadStrategyOption strategyOption)
		{
			LoadStrategyAttribute attr = new LoadStrategyAttribute(strategyOption);
			CompositeBuilderStrategy strategy = attr.Create();

			return Children(propertyPath, strategy);
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="propertyPath">������·����</param>
		/// <param name="level">���ؼ���</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(String[] propertyPath, Int32 level)
		{
			return Children(propertyPath, CompositeBuilderStrategyFactory.Create(level));
		}

		/// <summary>
		/// ����Ҫ���ص������Ժͼ��ز��ԡ�
		/// </summary>
		/// <param name="propertyPath">������·����</param>
		/// <param name="builderStrategy">���ز��ԡ�</param>
		/// <returns>��ǰʵ����</returns>
		public AssemblyPolicyBuilder Children(String[] propertyPath, CompositeBuilderStrategy builderStrategy)
		{
			for (Int32 i = 0; i < propertyPath.Length; i++)
			{
				String fullName = String.Join(CommonPolicies.DOT, propertyPath, 0, (i + 1));

				if (!m_children.ContainsKey(fullName))
				{
					m_children.Add(fullName, builderStrategy);
				}
				else if ((m_children[fullName] == null) && (builderStrategy != null))
				{
					m_children[fullName] = builderStrategy;
				}
			}

			return this;
		}

		#endregion

		/// <summary>
		/// ����װ����ԡ�
		/// </summary>
		/// <returns></returns>
		public AssemblyPolicy Build()
		{
			if (m_children.Count == 0)
			{
				return null;
			}

			List<String> allProperties = new List<String>(m_children.Keys);
			allProperties.Sort(
					delegate(String left, String right)
					{
						String[] leftPropertyPath = left.Split(CommonPolicies.DOT.ToCharArray());
						String[] rightPropertyPath = right.Split(CommonPolicies.DOT.ToCharArray());

						return leftPropertyPath.Length.CompareTo(rightPropertyPath.Length);
					}
				);

			AssemblyPolicy policy = AssemblyPolicy.CreateRoot();

			foreach (String fullName in allProperties)
			{
				AssemblyChildrenEntry parent = policy.GetParentEntry(fullName);
				String propertyName = CommonPolicies.GetPropertyName(fullName);
				CompositeBuilderStrategy builderStrategy = m_children[fullName] ?? m_defaultBuilderStrategy;

				AssemblyChildrenEntry current = new AssemblyChildrenEntry(policy, parent, propertyName, builderStrategy);

				if (parent == null)
				{
					policy.InnerList.Add(current);
				}
				else
				{
					parent.InnerList.Add(current);
				}
			}

			return policy;
		}

		#endregion

		#region ����ǿ��ת��

		/// <summary>
		/// ��ʽ����ǿ��ת��������װ�䷽�롣
		/// </summary>
		/// <param name="builder">��������</param>
		/// <returns>���ɺõ�װ�䷽�롣</returns>
		public static implicit operator AssemblyPolicy(AssemblyPolicyBuilder builder)
		{
			return builder.Build();
		}

		#endregion
	}
}