#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����SelectExpression.cs
// �ļ�����������ѡ����ʽ������ѡ��Ҫ���ص����ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110524
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
	/// ѡ����ʽ������ѡ��Ҫ���ص����ԡ�
	/// </summary>
	[Serializable]
	public class SelectExpression
	{
		#region ˽���ֶ�

		private readonly List<PropertySelector> m_selectors = new List<PropertySelector>();

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public SelectExpression()
		{
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ��������ѡ������
		/// </summary>
		public Boolean HasSelectors
		{
			get { return (m_selectors.Count != 0); }
		}

		/// <summary>
		/// ��ȡ���е�����ѡ������
		/// </summary>
		public PropertySelector[] Selectors
		{
			get { return m_selectors.ToArray(); }
		}

		/// <summary>
		/// ��ȡ����ѡ������
		/// </summary>
		public PropertySelector Selector
		{
			get { return m_selectors[0]; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ѡ��Ŀ��ʵ���е��������ԡ�
		/// </summary>
		/// <param name="entityType">Ŀ��ʵ�����͡�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression AllFrom(Type entityType)
		{
			#region ǰ������

			Debug.Assert((entityType != null), "Ŀ��ʵ�����Ͳ��� entityType ����Ϊ�ա�");

			#endregion

			m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllFromSchema, entityType));

			return this;
		}

		/// <summary>
		/// ѡ���ⲿ�����б��е��������ԡ�
		/// </summary>
		/// <param name="allChains">�������б����е�Ԫ�ر������ⲿ���á�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression AllFrom(params IPropertyChain[] allChains)
		{
			#region ǰ������

#if DEBUG

			Debug.Assert((allChains != null) && (allChains.Length != 0), "�������б���� allChains ����Ϊ�ջ���б�");

			foreach (IPropertyChain chain in allChains)
			{
				Debug.Assert(chain != null, "�������б��а��� null Ԫ�ء�");
				Debug.Assert(!chain.IsPrimitive && !chain.IsChildren, String.Format("������ {0} �����ⲿ�������Ի�Ŀ��ʵ�塣", chain.FullName));
			}

#endif

			#endregion

			foreach (IPropertyChain chain in allChains)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllFromSchema, chain));
			}

			return this;
		}


		/// <summary>
		/// ѡ���ⲿ�����б��е��������ԡ�
		/// </summary>
		/// <param name="allChainBuilders">�������������б����е�Ԫ�ر��������ⲿ������������</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression AllFrom(params IPropertyChainBuilder[] allChainBuilders)
		{
			#region ǰ������

#if DEBUG

			Debug.Assert((allChainBuilders != null) && (allChainBuilders.Length != 0), "�������б���� allChains ����Ϊ�ջ���б�");

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				Debug.Assert(builder != null, "�������б��а��� null Ԫ�ء�");

				IPropertyChain chain = builder.Build();

				Debug.Assert(!chain.IsPrimitive && !chain.IsChildren, String.Format("������ {0} �����ⲿ�������Ի�Ŀ��ʵ�塣", chain.FullName));
			}

#endif

			#endregion

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllFromSchema, builder));
			}

			return this;
		}

		/// <summary>
		/// ѡ��Ŀ��ʵ���е����з��ӳټ������ԡ�
		/// </summary>
		/// <param name="entityType">Ŀ��ʵ�����͡�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression AllExceptLazyLoadFrom(Type entityType)
		{
			#region ǰ������

			Debug.Assert((entityType != null), "Ŀ��ʵ�����Ͳ��� entityType ����Ϊ�ա�");

			#endregion

			m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllExceptLazyLoadFromSchema, entityType));

			return this;
		}

		/// <summary>
		/// ѡ���ⲿ�����б��е����з��ӳټ������ԡ�
		/// </summary>
		/// <param name="allChains">�������б����е�Ԫ�ر������ⲿ���á�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression AllExceptLazyLoadFrom(params IPropertyChain[] allChains)
		{
			#region ǰ������

#if DEBUG

			Debug.Assert((allChains != null) && (allChains.Length != 0), "�������б���� allChains ����Ϊ�ջ���б�");

			foreach (IPropertyChain chain in allChains)
			{
				Debug.Assert(chain != null, "�������б��а��� null Ԫ�ء�");
				Debug.Assert(!chain.IsPrimitive && !chain.IsChildren, String.Format("������ {0} �����ⲿ�������Ի�Ŀ��ʵ�塣", chain.FullName));
			}

#endif

			#endregion

			foreach (IPropertyChain chain in allChains)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllExceptLazyLoadFromSchema, chain));
			}

			return this;
		}

		/// <summary>
		/// ѡ���ⲿ�����б��е����з��ӳټ������ԡ�
		/// </summary>
		/// <param name="allChainBuilders">�������б����е�Ԫ�ر������ⲿ���á�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression AllExceptLazyLoadFrom(params IPropertyChainBuilder[] allChainBuilders)
		{
			#region ǰ������

#if DEBUG

			Debug.Assert((allChainBuilders != null) && (allChainBuilders.Length != 0), "�������б���� allChainBuilders ����Ϊ�ջ���б�");

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				Debug.Assert(builder != null, "�������������б��а��� null Ԫ�ء�");

				IPropertyChain chain = builder.Build();

				Debug.Assert(!chain.IsPrimitive && !chain.IsChildren, String.Format("������ {0} �����ⲿ�������Ի�Ŀ��ʵ�塣", chain.FullName));
			}

#endif

			#endregion

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.AllExceptLazyLoadFromSchema, builder));
			}

			return this;
		}

		/// <summary>
		/// ѡ��һ�����ԡ�
		/// </summary>
		/// <param name="targetType">Ŀ�����͡�</param>
		/// <param name="propertyPath">����·����</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression Property(Type targetType, String[] propertyPath)
		{
			#region ǰ������

			Debug.Assert(targetType != null, "Ŀ�����Ͳ��� targetType ����Ϊ�ա�");
			Debug.Assert((propertyPath != null) && (propertyPath.Length != 0), "����·������ propertyPath ����Ϊ�ջ���б�");

			#endregion

			return Property(new PropertyChain(targetType, propertyPath));
		}

		/// <summary>
		/// ѡ��һ�����ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression Property(IPropertyChainBuilder builder)
		{
			#region ǰ������

			Debug.Assert(builder != null, "���������������� builder ����Ϊ�ա�");

			#endregion

			return Property(builder.Build());
		}

		/// <summary>
		/// ѡ��һ�����ԡ�
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression Property(IPropertyChain chain)
		{
			#region ǰ������

			Debug.Assert(chain != null, "���������� chain ����Ϊ�ա�");

			#endregion

			m_selectors.Add(PropertySelector.Create(chain));

			return this;
		}

		/// <summary>
		/// ѡ�����ԡ�
		/// </summary>
		/// <param name="allChains">�������б�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression Properties(params IPropertyChain[] allChains)
		{
			#region ǰ������

			Debug.Assert(((allChains != null) && (allChains.Length != 0)), "�������б���� allChains ����Ϊ�ջ���б�");

			#endregion

			foreach (IPropertyChain chain in allChains)
			{
				m_selectors.Add(PropertySelector.Create(chain));
			}

			return this;
		}

		/// <summary>
		/// ѡ�����ԡ�
		/// </summary>
		/// <param name="allChainBuilders">�������������б�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression Properties(params IPropertyChainBuilder[] allChainBuilders)
		{
			#region ǰ������

			Debug.Assert(((allChainBuilders != null) && (allChainBuilders.Length != 0)), "�������б���� allChains ����Ϊ�ջ���б�");

			#endregion

			foreach (IPropertyChainBuilder builder in allChainBuilders)
			{
				m_selectors.Add(PropertySelector.Create(builder));
			}

			return this;
		}

		/// <summary>
		/// ѡ��Ŀ��ʵ���е��������ԡ�
		/// </summary>
		/// <param name="entityType">Ŀ��ʵ�����ͣ�����Ϊ�ա�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression PrimaryKeyOf(Type entityType)
		{
			#region ǰ������

			Debug.Assert((entityType != null), "Ŀ��ʵ�����Ͳ��� entityType ����Ϊ�ա�");

			#endregion

			m_selectors.Add(PropertySelector.Create(PropertySelectMode.PrimaryKey, entityType));

			return this;
		}

		/// <summary>
		/// ѡ�������ⲿ�����е��������ԡ�
		/// </summary>
		/// <param name="allChains">�������б����е�Ԫ�ر���ӳ��Ϊ�������ԡ�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression PrimaryKeysOf(params IPropertyChain[] allChains)
		{
			#region ǰ������

			Debug.Assert(((allChains != null) && (allChains.Length != 0)), "�������б���� allChains ����Ϊ�ջ���б�");

			#endregion

			foreach (IPropertyChain chain in allChains)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.PrimaryKey, chain));
			}

			return this;
		}

		/// <summary>
		/// ѡ�������ⲿ�����е��������ԡ�
		/// </summary>
		/// <param name="allChainBuilders">�������������б����е�Ԫ�������������ԡ�</param>
		/// <returns>��ǰʵ����</returns>
		public SelectExpression PrimaryKeysOf(params IPropertyChainBuilder[] allChainBuilders)
		{
			#region ǰ������

			Debug.Assert(((allChainBuilders != null) && (allChainBuilders.Length != 0)), "�������б���� allChains ����Ϊ�ջ���б�");

			#endregion

			foreach (IPropertyChain builder in allChainBuilders)
			{
				m_selectors.Add(PropertySelector.Create(PropertySelectMode.PrimaryKey, builder));
			}

			return this;
		}

		#endregion

		#region ����

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾѡ�����Ƿ���Խ��������Ƿ�ѡ�������ԣ���
		/// </summary>
		public Boolean IsResolvable
		{
			get { return (m_selectors.Count != 0); }
		}

		/// <summary>
		/// ��������ȡ����ѡ����ز��ԡ�
		/// </summary>
		/// <returns>CompositeBuilderStrategy ʵ��������ѡ����ز��ԡ�</returns>
		public CompositeBuilderStrategy Resolve()
		{
			#region ǰ������

			Debug.Assert(IsResolvable, "Ӧ����ѡ��һ�����ԡ�");

			#endregion

			return CompositeBuilderStrategyFactory.Create(m_selectors.ToArray());
		}

		/// <summary>
		/// ��ʽǿ��ת������ SelectExpression ת��Ϊ CompositeBuilderStrategy��
		/// </summary>
		/// <param name="expression">ѡ����ʽ��</param>
		/// <returns>ת�������� CompositeBuilderStrategy��</returns>
		public static implicit operator CompositeBuilderStrategy(SelectExpression expression)
		{
			return expression.Resolve();
		}

		/// <summary>
		/// ���ã��Թ��������á�
		/// </summary>
		public void Reset()
		{
			m_selectors.Clear();
		}

		#endregion
	}
}