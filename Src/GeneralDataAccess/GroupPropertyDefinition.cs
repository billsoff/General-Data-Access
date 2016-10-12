#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupPropertyDefinition.cs
// �ļ�������������ʾ���������������Զ��塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110628
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾ���������������Զ��塣
	/// </summary>
	internal sealed class GroupPropertyDefinition
	{
		#region ˽���ֶ�

		private readonly GroupDefinition m_group;
		private readonly PropertyInfo m_propertyInfo;

		private readonly AggregationAttribute m_aggregation;
		private readonly EntityPropertyDefinition m_definition;
		private readonly IPropertyChain m_propertyChain;

		private readonly Int32 m_level;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������÷������Ͷ��������ԡ�
		/// </summary>
		/// <param name="group">���������Ͷ��塣</param>
		/// <param name="propertyInfo">���ԡ�</param>
		public GroupPropertyDefinition(GroupDefinition group, PropertyInfo propertyInfo)
		{
			m_group = group;
			m_propertyInfo = propertyInfo;

			m_aggregation = (AggregationAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(AggregationAttribute));

			if (m_aggregation.PropertyPath != null)
			{
				m_propertyChain = new PropertyChain(group.Entity.Type, m_aggregation.PropertyPath);
				m_definition = m_group.Entity.Properties[m_propertyChain];
				m_level = m_aggregation.PropertyPath.Length;
			}
			else
			{
				m_level = 1;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡҪ���оۺϲ������������Զ��塣
		/// </summary>
		internal EntityPropertyDefinition Definition
		{
			get { return m_definition; }
		}

		/// <summary>
		/// ��ȡ���������ķ������Ͷ��塣
		/// </summary>
		internal GroupDefinition Group
		{
			get { return m_group; }
		}

		/// <summary>
		/// ��ȡ�������ǡ�
		/// </summary>
		public AggregationAttribute Aggregation
		{
			get { return m_aggregation; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ��ǰ�����Ƿ�Ϊ�����
		/// </summary>
		public Boolean IsGroupItem
		{
			get { return m_aggregation.IsGroupItem; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ�Ϊ�������ԣ���ֵ���ԣ���
		/// </summary>
		public Boolean IsPrimitive
		{
			get { return (m_definition == null) || m_definition.IsPrimitive; }
		}

		/// <summary>
		/// ��ȡҪ���оۺϼ�����������Եļ���
		/// </summary>
		public Int32 Level
		{
			get { return m_level; }
		}

		/// <summary>
		/// ��ȡ�������ơ�
		/// </summary>
		public String Name
		{
			get { return m_propertyInfo.Name; }
		}

		/// <summary>
		/// ��ȡҪ�ۺϵ���������
		/// </summary>
		public IPropertyChain PropertyChain
		{
			get { return m_propertyChain; }
		}

		/// <summary>
		/// ��ȡ���ԡ�
		/// </summary>
		public PropertyInfo PropertyInfo
		{
			get { return m_propertyInfo; }
		}

		/// <summary>
		/// ��ȡ�������͡�
		/// </summary>
		public Type Type
		{
			get { return m_propertyInfo.PropertyType; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// Ϊ���������ⲿ�������Դ���Ĭ�ϵ�ʵ��ܹ�������ɲ��ԡ�
		/// </summary>
		/// <returns>�����õ�ʵ��ܹ�������ɲ��ԡ�</returns>
		public CompositeBuilderStrategy CreateDefaultBuilderStrategy()
		{
			#region ǰ������

			Debug.Assert(!IsPrimitive, "ֻ�в��������ⲿ�������Բſ��Ե��÷��� GetBuilderStrategy��");

			#endregion

			LoadStrategyAttribute attr;

			attr = (LoadStrategyAttribute)Attribute.GetCustomAttribute(PropertyInfo, typeof(LoadStrategyAttribute));

			if (attr == null)
			{
				attr = Definition.Entity.LoadStrategy;
			}

			CompositeBuilderStrategy strategy = attr.Create();

			return strategy;
		}

		/// <summary>
		/// �ַ�����ʾ��
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			if (m_aggregation.PropertyPath != null)
			{
				return String.Format(
						"{0} -> {1}.{2}",
						m_aggregation.ToString(),
						m_group.Entity.Type.Name,
						String.Join(".", m_aggregation.PropertyPath)
					);
			}
			else
			{
				return String.Format(
						"{0} -> {1}",
						m_aggregation.ToString(),
						m_group.Entity.Type.Name
					);
			}
		}

		#endregion
	}
}