#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupDefinition.cs
// �ļ�������������ʾ�����������Ͷ��塣
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
	/// ��ʾ�����������Ͷ��塣
	/// </summary>
	internal partial class GroupDefinition
	{
		#region ˽���ֶ�

		private readonly Type m_type;
		private readonly ConstructorInfo m_constructor;
		private readonly EntityDefinition m_entity;
		private readonly GroupPropertyDefinitionCollection m_properties;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������÷�����ʵ�����͡�
		/// </summary>
		/// <param name="groupType"></param>
		public GroupDefinition(Type groupType)
		{
			#region ǰ������

			Debug.Assert(groupType != null, "���������Ͳ��� groupType ����Ϊ�ա�");
			Debug.Assert(
					typeof(GroupResult).IsAssignableFrom(groupType),
					String.Format("���� {0} û�д� GroupResult ������", groupType.FullName)
				);
			Debug.Assert(
					Attribute.IsDefined(groupType, typeof(GroupAttribute)),
					String.Format("���� {0} ��û�б�� GroupAttribute��", groupType.FullName)
				);

			#endregion

			m_type = groupType;

			GroupAttribute groupAttr = (GroupAttribute)Attribute.GetCustomAttribute(groupType, typeof(GroupAttribute));

			#region ǰ������

			Debug.Assert(groupAttr.Type != null, String.Format("������ {0} �ϱ�ǵ� GroupAttribute ��û������Ҫ�����ʵ�����͡�", groupType.FullName));
			Debug.Assert(
					Attribute.IsDefined(groupAttr.Type, typeof(TableAttribute)),
					String.Format("Ҫ��������� {0} ����ʵ�����͡�", groupAttr.Type.FullName)
				);

			#endregion

			m_constructor = groupType.GetConstructor(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					(Binder)null,
					Type.EmptyTypes,
					(ParameterModifier[])null
				);

			#region ǰ������

			Debug.Assert(m_constructor != null, String.Format("���� {0} �в������޲εĹ��캯����", groupType.FullName));

			#endregion

			m_entity = EntityDefinitionBuilder.Build(groupAttr.Type);

			List<GroupPropertyDefinition> allProperties = new List<GroupPropertyDefinition>();

			foreach (PropertyInfo pf in groupType.GetProperties(CommonPolicies.PropertyBindingFlags))
			{
				if (Attribute.IsDefined(pf, typeof(AggregationAttribute)))
				{
					allProperties.Add(new GroupPropertyDefinition(this, pf));
				}
			}

			m_properties = new GroupPropertyDefinitionCollection(this, allProperties);
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ������ʵ�����͵��޲ι�������
		/// </summary>
		public ConstructorInfo Constructor
		{
			get { return m_constructor; }
		}

		/// <summary>
		/// ��ȡҪ�����ʵ�����͡�
		/// </summary>
		public EntityDefinition Entity
		{
			get { return m_entity; }
		}

		/// <summary>
		/// ��ȡ���м��ϡ�
		/// </summary>
		public GroupPropertyDefinitionCollection Properties
		{
			get { return m_properties; }
		}

		/// <summary>
		/// ��ȡ������ʵ�����͡�
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ���е��ⲿ�������ԡ�
		/// </summary>
		/// <returns>���е��ⲿ�������Լ��ϡ�</returns>
		public GroupPropertyDefinition[] GetForeignReferenceProperties()
		{
			List<GroupPropertyDefinition> results = new List<GroupPropertyDefinition>();

			foreach (GroupPropertyDefinition propertyDef in m_properties)
			{
				if (!propertyDef.IsPrimitive)
				{
					results.Add(propertyDef);
				}
			}

			return results.ToArray();
		}

		/// <summary>
		/// ��ȡ���еķ��������ԡ�
		/// </summary>
		/// <returns>�ּ������Եļ��ϡ�</returns>
		public GroupPropertyDefinition[] GetGroupItemProperties()
		{
			List<GroupPropertyDefinition> results = new List<GroupPropertyDefinition>();

			foreach (GroupPropertyDefinition propertyDef in Properties)
			{
				if (propertyDef.IsGroupItem)
				{
					results.Add(propertyDef);
				}
			}

			return results.ToArray();
		}

		/// <summary>
		/// ��ȡ���еĻ������ԣ���ֵ���ԣ���
		/// </summary>
		/// <returns>���еĻ������Եļ��ϡ�</returns>
		public GroupPropertyDefinition[] GetPrimitiveProperties()
		{
			List<GroupPropertyDefinition> results = new List<GroupPropertyDefinition>();

			foreach (GroupPropertyDefinition propertyDef in m_properties)
			{
				if (propertyDef.IsPrimitive)
				{
					results.Add(propertyDef);
				}
			}

			return results.ToArray();
		}

		/// <summary>
		/// �ַ�����ʾ��
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Format("GROUP OF {0}", m_entity.ToString());
		}

		#endregion
	}
}