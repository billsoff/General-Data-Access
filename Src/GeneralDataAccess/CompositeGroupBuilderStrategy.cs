#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeGroupBuilderStrategy.cs
// �ļ�����������������ʵ����ز��ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110630
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ������ʵ����ز��ԡ�
	/// </summary>
	[Serializable]
	internal sealed class CompositeGroupBuilderStrategy : CompositeBuilderStrategy
	{
		#region ˽���ֶ�

		private readonly Type m_groupResultType;

		[NonSerialized]
		private GroupDefinition m_group;

		/// <summary>
		/// ÿһ�����������ⲿ���õļ��ز��ԣ��������Ե����ơ�
		/// </summary>
		private readonly Dictionary<String, CompositeBuilderStrategy> m_strategies = new Dictionary<String, CompositeBuilderStrategy>();

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		/// <param name="groupResultType">������ʵ�����͡�</param>
		public CompositeGroupBuilderStrategy(Type groupResultType)
		{
			m_groupResultType = groupResultType;
			BuildForeignReferenceBuilderStrategies();
		}

		#endregion

		#region ����

		/// <summary>
		/// ָʾ�����Ƽ���
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return -1;
			}
		}

		/// <summary>
		/// ָʾҪ�������Թ��ˡ�
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// �������еľۺ�����������ʵ��ܹ����ϼ�ʵ��ܹ���
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>�����ʵ��ܹ�λ�ھۺ����Ե�·���ϻ����ɲ���Ҫ����أ��򷵻� true�����򷵻� false��</returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			foreach (GroupPropertyDefinition propertyDef in Group.Properties)
			{
				if (propertyDef.PropertyChain != null)
				{
					IPropertyChain chain = propertyDef.PropertyChain;

					if (chain.BelongsTo(schema))
					{
						return true;
					}

					if (!propertyDef.IsPrimitive && chain.Contains(schema))
					{
						CompositeBuilderStrategy strategy = GetForeignReferenceBuilderStrategy(propertyDef);

						if (strategy.LoadFromSchema(schema))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// ȷ���Ƿ�ѡ�������ʵ��ܹ��е��κ����ԡ�
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			foreach (GroupPropertyDefinition propertyDef in GroupItems)
			{
				if (schema.OwnProperty(propertyDef.PropertyChain))
				{
					if (propertyDef.IsPrimitive)
					{
						return false;
					}
				}
				else if (!propertyDef.IsPrimitive)
				{
					CompositeBuilderStrategy strategy = GetForeignReferenceBuilderStrategy(propertyDef);

					if (!strategy.SelectNothingFrom(schema))
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// ѡ�����ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			foreach (GroupPropertyDefinition groupPropertyDef in GroupItems)
			{
				if (groupPropertyDef.PropertyChain.Equals(property.PropertyChain))
				{
					if (groupPropertyDef.IsPrimitive)
					{
						return true;
					}
				}
				else if (!groupPropertyDef.IsPrimitive && groupPropertyDef.PropertyChain.Contains(property))
				{
					CompositeBuilderStrategy strategy = GetForeignReferenceBuilderStrategy(groupPropertyDef);

					if (strategy.SelectProperty(property))
					{
						return true;
					}
				}
			}

			return false;
		}

		#endregion

		#region ���Է���

		/// <summary>
		/// ���ɲ��Ե���ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns></returns>
		public override String Dump()
		{
			GroupPropertyDefinition[] allForeignRefs = Group.GetForeignReferenceProperties();

			if (allForeignRefs.Length == 0)
			{
				return base.Dump();
			}
			else
			{
				const String PADDING = "    "; // ���� 4 ���ո�
				StringBuilder buffer = new StringBuilder();

				buffer.AppendFormat("{0}�����ⲿ�������Ե����ɲ���Ϊ��", GetType().FullName);
				buffer.AppendLine();

				foreach (GroupPropertyDefinition propertyDef in allForeignRefs)
				{
					CompositeBuilderStrategy strategy = GetForeignReferenceBuilderStrategy(propertyDef);
					String info = strategy.Dump();
					info = Regex.Replace(info, "^", PADDING, RegexOptions.Multiline);
					buffer.AppendLine(info);
				}

				return buffer.ToString();
			}
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ������ʵ�嶨�塣
		/// </summary>
		internal GroupDefinition Group
		{
			get
			{
				if (m_group == null)
				{
					m_group = GroupDefinitionBuilder.Build(m_groupResultType);
				}

				return m_group;
			}
		}

		private GroupPropertyDefinition[] m_foreignReferences;

		/// <summary>
		/// ��ȡ���е��ⲿ�������ԡ�
		/// </summary>
		internal GroupPropertyDefinition[] ForeignReferences
		{
			get
			{
				if (m_foreignReferences == null)
				{
					m_foreignReferences = Group.GetForeignReferenceProperties();
				}

				return m_foreignReferences;
			}
		}

		private GroupPropertyDefinition[] m_groupItems;

		/// <summary>
		/// ��ȡ���еķ��������ԡ�
		/// </summary>
		internal GroupPropertyDefinition[] GroupItems
		{
			get
			{
				if (m_groupItems == null)
				{
					m_groupItems = Group.GetGroupItemProperties();
				}

				return m_groupItems;
			}
		}

		private GroupPropertyDefinition[] m_primitives;

		/// <summary>
		/// ��ȡ���еĻ������ԡ�
		/// </summary>
		internal GroupPropertyDefinition[] Primitives
		{
			get
			{
				if (m_primitives == null)
				{
					m_primitives = Group.GetPrimitiveProperties();
				}

				return m_primitives;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// �����ⲿ���õ����ɲ��ԡ�
		/// </summary>
		private void BuildForeignReferenceBuilderStrategies()
		{
			GroupPropertyDefinition[] foreignReferences = Group.GetForeignReferenceProperties();

			foreach (GroupPropertyDefinition propertyDef in foreignReferences)
			{
				CompositeBuilderStrategy strategy = propertyDef.CreateDefaultBuilderStrategy();
				strategy.InitialLevel = propertyDef.Level;

				m_strategies.Add(propertyDef.Name, strategy);
			}
		}

		/// <summary>
		/// ��ȡ���������ⲿ�������Ե�ʵ��ܹ�������ɲ��ԡ�
		/// </summary>
		/// <param name="propertyDef">�ⲿ�������ԡ�</param>
		/// <returns>�ⲿ�������Ե�ʵ��ܹ�������ɲ��ԡ�</returns>
		private CompositeBuilderStrategy GetForeignReferenceBuilderStrategy(GroupPropertyDefinition propertyDef)
		{
			return m_strategies[propertyDef.Name];
		}

		#endregion
	}
}