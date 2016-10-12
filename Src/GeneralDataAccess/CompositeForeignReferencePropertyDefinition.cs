#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeForeignReferencePropertyDefinition.cs
// �ļ���������������ʵ����ʽ�ܹ����Զ��塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110708
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����ʵ����ʽ�ܹ����Զ��塣
	/// </summary>
	internal sealed class CompositeForeignReferencePropertyDefinition : CompositePropertyDefinition
	{
		#region ˽���ֶ�

		private readonly PropertyJoinMode m_joinMode;
		private readonly List<String[]> m_rootPropertyPaths = new List<String[]>();
		private readonly List<String[]> m_mappedPropertyPaths = new List<String[]>();

		private IPropertyChain[] m_rootPropertyChains;
		private IPropertyChain[] m_mappedPropertyChains;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������������Ϣ��
		/// </summary>
		/// <param name="definition">����ʵ�嶨�塣</param>
		/// <param name="pf">������Ϣ��</param>
		public CompositeForeignReferencePropertyDefinition(CompositeDefinition definition, PropertyInfo pf)
			: base(definition, pf)
		{
			CompositeForeignReferenceAttribute refAttr = (CompositeForeignReferenceAttribute)Attribute.GetCustomAttribute(
					pf,
					typeof(CompositeForeignReferenceAttribute)
				);
			m_joinMode = refAttr.PropertyJoinMode;

			PropertyMappingAttribute[] allMappingAttrs = (PropertyMappingAttribute[])Attribute.GetCustomAttributes(
					pf,
					typeof(PropertyMappingAttribute)
				);

			foreach (PropertyMappingAttribute mappingAttr in allMappingAttrs)
			{
				m_rootPropertyPaths.Add(mappingAttr.RootPropertyPath);
				m_mappedPropertyPaths.Add(mappingAttr.ForeignReferencePropertyPath);
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�������ӷ�ʽ��
		/// </summary>
		public PropertyJoinMode JoinMode
		{
			get { return m_joinMode; }
		}

		/// <summary>
		/// ��ȡ���������б�
		/// </summary>
		public IPropertyChain[] RootPropertyChains
		{
			get
			{
				if (m_rootPropertyChains == null)
				{
					List<IPropertyChain> rootChains = m_rootPropertyPaths.ConvertAll<IPropertyChain>(
							delegate(String[] propertyPath)
							{
								return new PropertyChain(Composite.Root.Type, propertyPath);
							}
						);

					m_rootPropertyChains = rootChains.ToArray();
				}

				return m_rootPropertyChains;
			}
		}

		/// <summary>
		/// ��ȡӳ���������б�
		/// </summary>
		public IPropertyChain[] MappedPropertyChains
		{
			get
			{
				if (m_mappedPropertyChains == null)
				{
					List<IPropertyChain> mappedChains = m_mappedPropertyPaths.ConvertAll<IPropertyChain>(
							delegate(String[] propertyPath)
							{
								return new PropertyChain(Type, propertyPath);
							}
						);

					m_mappedPropertyChains = mappedChains.ToArray();
				}

				return m_mappedPropertyChains;
			}
		}

		#endregion
	}
}