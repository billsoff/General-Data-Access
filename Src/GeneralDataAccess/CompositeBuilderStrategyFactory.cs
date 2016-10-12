#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeBuilderStrategyFactory.cs
// �ļ��������������ɲ��Թ�����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110518
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ɲ��Թ�����
	/// </summary>
	public static class CompositeBuilderStrategyFactory
	{
		/// <summary>
		/// Ĭ�����ɲ��ԡ�
		/// </summary>
		public static CompositeBuilderStrategy Default
		{
			get { return AcceptAllExpandable; }
		}

		/// <summary>
		/// ���ɲ��ԣ�����������
		/// </summary>
		public static CompositeBuilderStrategy SelfOnly
		{
			get { return new CompositeSpecifyLevelBuilderStrategy(0); }
		}

		/// <summary>
		/// ���ɲ��ԣ����ص�һ���ⲿ���á�
		/// </summary>
		public static CompositeBuilderStrategy OneLevel
		{
			get { return new CompositeSpecifyLevelBuilderStrategy(1); }
		}

		/// <summary>
		/// չ�����м��𣬳����������ϱ���� SuppressExpand��
		/// </summary>
		public static CompositeBuilderStrategy AcceptAllExpandable
		{
			get { return new CompositeAcceptAllExpandableBuilderStrategy(); }
		}

		/// <summary>
		/// ���ɲ��ԣ������Ƽ���
		/// <para>ͬһ����·���������ظ������ü��ص��ڶ�����������Ϊֹ��</para>
		/// </summary>
		public static CompositeBuilderStrategy UnlimitedLevel
		{
			get { return new CompositeSpecifyLevelBuilderStrategy(-1); }
		}

		/// <summary>
		/// ��ѡ���κ����ԡ�
		/// </summary>
		public static CompositeBuilderStrategy Nothing
		{
			get { return CompositeNothingBuilderStrategy.Value; }
		}

		/// <summary>
		/// Ϊʵ��ϳ����ɲ��ԡ�
		/// <para>���û����ʽָ�����ز��ԣ���ʹ����ʵ���������ļ��ز��ԡ�</para>
		/// <para>�����δ��ʽָ��Ҳδ�������ز��ԣ���ʹ��Ĭ�ϲ��ԡ�</para>
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <param name="loadStrategy">���ز��ԡ�</param>
		/// <param name="additionalSelectors">���ӵ�ѡ�����б�����Ϊ�ա�</param>
		/// <returns>�ϳɵ����ɲ��ԡ�</returns>
		public static CompositeBuilderStrategy Compose(Type entityType, CompositeBuilderStrategy loadStrategy, IList<PropertySelector> additionalSelectors)
		{
			if (loadStrategy == null)
			{
				loadStrategy = Create(entityType);
			}

			if ((additionalSelectors != null) && (additionalSelectors.Count != 0))
			{
				CompositeBuilderStrategy additional = Create(additionalSelectors);

				CompositeBuilderStrategy result = Union(loadStrategy, additional);

				return result;
			}
			else
			{
				return loadStrategy;
			}
		}

		/// <summary>
		/// ���ü��ؼ���
		/// </summary>
		/// <para>���ָ���ļ���Ϊ���������޶����𣬴�ʱ��ͬһ����·���������ظ������ü��ص��ڶ�����������Ϊֹ��</para>
		/// <param name="level">���ؼ���</param>
		public static CompositeBuilderStrategy Create(Int32 level)
		{
			return new CompositeSpecifyLevelBuilderStrategy(level);
		}

		/// <summary>
		/// ��ʽѡ�����ԡ�
		/// </summary>
		/// <param name="allSelectors">����ѡ�����б�</param>
		/// <returns>���ԡ�</returns>
		public static CompositeBuilderStrategy Create(IList<PropertySelector> allSelectors)
		{
			if ((allSelectors == null) || (allSelectors.Count == 0))
			{
				return Nothing;
			}
			else
			{
				return new CompositeExplicitSelectionBulderStrategy(allSelectors);
			}
		}

		/// <summary>
		/// Ϊʵ�����ʹ������ɲ��ԡ�
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>�����õ����ɲ��ԡ�</returns>
		public static CompositeBuilderStrategy Create(Type entityType)
		{
			EntityDefinition definition = EntityDefinitionBuilder.Build(entityType);

			return definition.LoadStrategy.Create();
		}

		/// <summary>
		/// �������ڷ�������ɲ��ԡ�
		/// </summary>
		/// <param name="groupResultType">������ʵ�����͡�</param>
		/// <returns>�����õ����ɲ��ԡ�</returns>
		public static CompositeBuilderStrategy Create4Group(Type groupResultType)
		{
			return new CompositeGroupBuilderStrategy(groupResultType);
		}

		/// <summary>
		/// �����ɲ��Խ����޼���
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼������ɲ��ԡ�</param>
		/// <param name="trimmers">�޼������ϡ�</param>
		/// <returns>�޼�������ɲ��ԡ�</returns>
		public static CompositeBuilderStrategy TrimOff(CompositeBuilderStrategy builderStrategyToTrim, params PropertyTrimmer[] trimmers)
		{
			#region ǰ������

			Debug.Assert(builderStrategyToTrim != null, "���� builderStrategyToTrim ����Ϊ�ա�");
			Debug.Assert((trimmers != null) && (trimmers.Length != 0), "���� trimmers ����Ϊ�ջ�����顣");

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, trimmers);
		}

		/// <summary>
		/// �����ɲ��Խ����޼���
		/// </summary>
		/// <param name="builderStategyToTrim">Ҫ�޼������ɲ��ԡ�</param>
		/// <param name="trimmingProperties">Ҫ�޼����������б�</param>
		/// <returns>�޼�������ɲ��ԡ�</returns>
		public static CompositeBuilderStrategy TrimOff(CompositeBuilderStrategy builderStategyToTrim, params IPropertyChain[] trimmingProperties)
		{
			#region ǰ������

#if DEBUG

			Debug.Assert(builderStategyToTrim != null, "Ҫ�޼������ɲ��Բ��� builderStategyToTrim ����Ϊ�ա�");
			Debug.Assert(
					(trimmingProperties != null) && (trimmingProperties.Length != 0),
					"Ҫ�޼����������б���� trimmingProperties ����Ϊ�ջ���б�"
				);

			foreach (IPropertyChain chain in trimmingProperties)
			{
				if (chain.IsChildren)
				{
					Debug.Fail(String.Format("Ҫ�Ƴ����������б��е������� {0} ����ʵ���б����ԣ�û�����塣", chain.FullName));
				}
			}

#endif

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStategyToTrim, trimmingProperties);
		}

		/// <summary>
		/// �����ɲ��Խ����޼���
		/// </summary>
		/// <param name="builderStategyToTrim">Ҫ�޼������ɲ��ԡ�</param>
		/// <param name="trimmingProperties">Ҫ�޼����������б�</param>
		/// <returns>�޼�������ɲ��ԡ�</returns>
		public static CompositeBuilderStrategy TrimOff(CompositeBuilderStrategy builderStategyToTrim, params IPropertyChainBuilder[] trimmingProperties)
		{
			#region ǰ������

#if DEBUG

			Debug.Assert(builderStategyToTrim != null, "Ҫ�޼������ɲ��Բ��� builderStategyToTrim ����Ϊ�ա�");
			Debug.Assert(
					(trimmingProperties != null) && (trimmingProperties.Length != 0),
					"Ҫ�޼����������б���� trimmingProperties ����Ϊ�ջ���б�"
				);

			foreach (IPropertyChainBuilder builder in trimmingProperties)
			{
				IPropertyChain chain = builder.Build();

				if (chain.IsChildren)
				{
					Debug.Fail(String.Format("Ҫ�Ƴ����������б��е������� {0} ����ʵ���б����ԣ�û�����塣", chain.FullName));
				}
			}

#endif

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStategyToTrim, trimmingProperties);
		}

		/// <summary>
		/// �޼�ʵ����ֱ���ķ����������ԡ�
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼��Ĳ��ԡ�</param>
		/// <returns>�޼���Ĳ��ԡ�</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim)
		{
			#region ǰ������

			Debug.Assert(builderStrategyToTrim != null, "Ҫ�޼������ɲ��Բ��� builderStrategyToTrim ����Ϊ�ա�");

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, new NonPrimaryKeyPropertyTrimmer());
		}

		/// <summary>
		/// �޼��ⲿ�����еķ��������ԡ�
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼��Ĳ��ԡ�</param>
		/// <param name="from">Ҫ�����޼����ⲿ�������ԡ�</param>
		/// <returns>�޼���Ĳ��ԡ�</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim, IPropertyChain from)
		{
			#region ǰ������

			Debug.Assert(builderStrategyToTrim != null, "Ҫ�޼������ɲ��Բ��� builderStrategyToTrim ����Ϊ�ա�");
			Debug.Assert(from != null, "Ҫ�����޼����ⲿ�������Բ��� from ����Ϊ�ա�");

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, new NonPrimaryKeyPropertyTrimmer(from));
		}

		/// <summary>
		/// �޼��ⲿ�����б��з��������ԡ�
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼��Ĳ��ԡ�</param>
		/// <param name="fromList">�ⲿ�����б�</param>
		/// <returns>�޼���Ĳ��ԡ�</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim, IPropertyChain[] fromList)
		{
			#region ǰ������

			Debug.Assert(builderStrategyToTrim != null, "Ҫ�޼������ɲ��Բ��� builderStrategyToTrim ����Ϊ�ա�");
			Debug.Assert((fromList != null) && (fromList.Length != 0), "Ҫ�����޼����ⲿ�����б���� fromList ����Ϊ�ջ���б�");

			#endregion

			PropertyTrimmer[] allTrimmers = Array.ConvertAll<IPropertyChain, PropertyTrimmer>(
					fromList,
					delegate(IPropertyChain chain)
					{
						return new NonPrimaryKeyPropertyTrimmer(chain);
					}
				);

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, allTrimmers);
		}

		/// <summary>
		/// �޼��ⲿ���������еķ��������ԡ�
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼������ɲ��ԡ�</param>
		/// <param name="from">Ҫ�����޼����ⲿ�������ԡ�</param>
		/// <returns>�޼���Ĳ��ԡ�</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim, IPropertyChainBuilder from)
		{
			#region ǰ������

			Debug.Assert(builderStrategyToTrim != null, "Ҫ�޼������ɲ��Բ��� builderStrategyToTrim ����Ϊ�ա�");
			Debug.Assert(from != null, "Ҫ�����޼����ⲿ�������������������� from ����Ϊ�ա�");

			#endregion

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, new NonPrimaryKeyPropertyTrimmer(from));
		}

		/// <summary>
		/// �޼��ⲿ�����б��еķ��������ԡ�
		/// </summary>
		/// <param name="builderStrategyToTrim">Ҫ�޼������ɲ��ԡ�</param>
		/// <param name="fromList">�ⲿ�����б�</param>
		/// <returns>�޼�������ɲ��ԡ�</returns>
		public static CompositeBuilderStrategy TrimOffNonPrimaryKeys(CompositeBuilderStrategy builderStrategyToTrim, IPropertyChainBuilder[] fromList)
		{
			#region ǰ������

			Debug.Assert(builderStrategyToTrim != null, "Ҫ�޼������ɲ��Բ��� builderStrategyToTrim ����Ϊ�ա�");
			Debug.Assert((fromList != null) && (fromList.Length != 0), "Ҫ�����޼����ⲿ�����б���� fromList ����Ϊ�ջ���б�");

			#endregion

			PropertyTrimmer[] allTrimmers = Array.ConvertAll<IPropertyChainBuilder, PropertyTrimmer>(
					fromList,
					delegate(IPropertyChainBuilder builder)
					{
						return new NonPrimaryKeyPropertyTrimmer(builder);
					}
				);

			return new CompositeTrimmingBuilderStrategy(builderStrategyToTrim, allTrimmers);
		}

		/// <summary>
		/// �ϲ����ԣ�����ϲ��Ĳ����б�Ϊ�գ��򷵻� null��
		/// </summary>
		/// <param name="strategies">Ҫ�ϲ��Ĳ����б�</param>
		/// <returns>�ϲ������һ���µ� CompositeBuilderStrategy ʵ��������Ϊ null����������б�Ϊ�ա�</returns>
		public static CompositeBuilderStrategy Union(params CompositeBuilderStrategy[] strategies)
		{
			if ((strategies == null) || (strategies.Length == 0))
			{
				return null;
			}

			List<CompositeBuilderStrategy> results = new List<CompositeBuilderStrategy>();

			foreach (CompositeBuilderStrategy strategy in strategies)
			{
				if (strategy != null)
				{
					results.Add(strategy);
				}
			}

			if (results.Count != 0)
			{
				if (results.Count == 1)
				{
					return results[0];
				}
				else
				{
					return new CompositeUnionBuilderStrategy(results);
				}
			}
			else
			{
				return null;
			}
		}
	}
}