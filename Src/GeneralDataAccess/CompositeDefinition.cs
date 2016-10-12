#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeDefinition.cs
// �ļ���������������ʵ�嶨�塣
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
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����ʵ�嶨�塣
	/// </summary>
	internal sealed partial class CompositeDefinition
	{
		#region ˽���ֶ�

		private readonly Type m_type;
		private readonly ConstructorInfo m_constructor;
		private readonly CompositeRootPropertyDefinition m_root;
		private readonly CompositeForeignReferencePropertyDefinitionCollection m_foreignReferences;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø���ʵ�����͡�
		/// </summary>
		/// <param name="compositeResultType">����ʵ�����͡�</param>
		public CompositeDefinition(Type compositeResultType)
		{
			#region ǰ������

			Debug.Assert(
					typeof(CompositeResult).IsAssignableFrom(compositeResultType),
					String.Format("���� {0} û�д� CompositeResult ������", compositeResultType.FullName)
				);

			Debug.Assert(
					Attribute.IsDefined(compositeResultType, typeof(CompositeAttribute)),
					String.Format("���� {0} ��û�б�� CompositeAttribute��", compositeResultType.FullName)
				);

			#endregion

			m_type = compositeResultType;

			CompositeAttribute compositeAttr = (CompositeAttribute)Attribute.GetCustomAttribute(compositeResultType, typeof(CompositeAttribute));

			PropertyInfo root = compositeResultType.GetProperty(compositeAttr.PropertyName, CommonPolicies.PropertyBindingFlags);
			m_root = new CompositeRootPropertyDefinition(this, root);

			m_constructor = compositeResultType.GetConstructor(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					(Binder)null,
					Type.EmptyTypes,
					(ParameterModifier[])null
				);

			#region ǰ������

			Debug.Assert(m_constructor != null, String.Format("���� {0} �в������޲εĹ��캯����", compositeResultType.FullName));

			#endregion

			List<CompositeForeignReferencePropertyDefinition> foreignRefs = new List<CompositeForeignReferencePropertyDefinition>();

			foreach (PropertyInfo pf in compositeResultType.GetProperties(CommonPolicies.PropertyBindingFlags))
			{
				if (Attribute.IsDefined(pf, typeof(CompositeForeignReferenceAttribute)))
				{
					foreignRefs.Add(new CompositeForeignReferencePropertyDefinition(this, pf));
				}
			}

			m_foreignReferences = new CompositeForeignReferencePropertyDefinitionCollection(foreignRefs);
		}

		#endregion

		#region ����

		/// <summary>
		/// ��ȡ�޲ι��캯����Ϣ��
		/// </summary>
		public ConstructorInfo Constructor
		{
			get { return m_constructor; }
		}

		/// <summary>
		/// ��ȡ�ⲿ�������Լ��ϡ�
		/// </summary>
		public CompositeForeignReferencePropertyDefinitionCollection ForeignReferences
		{
			get { return m_foreignReferences; }
		}

		/// <summary>
		/// ��ȡ�����ԡ�
		/// </summary>
		public CompositeRootPropertyDefinition Root
		{
			get { return m_root; }
		}

		/// <summary>
		/// ��ȡʵ�����͡�
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion
	}
}