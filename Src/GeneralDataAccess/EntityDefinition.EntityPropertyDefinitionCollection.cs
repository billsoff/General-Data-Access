#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityDefinition.EntityPropertyDefinitionCollection.cs
// �ļ�����������ʵ�����Լ��ϡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110413
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	partial class EntityDefinition
	{
		/// <summary>
		/// ʵ�����Լ��ϡ�
		/// </summary>
		internal sealed class EntityPropertyDefinitionCollection : ReadOnlyCollection<EntityPropertyDefinition>
		{
			#region ˽���ֶ�

			private readonly EntityDefinition m_entity;

			private readonly Dictionary<String, EntityPropertyDefinition> m_properties;
			private readonly Dictionary<String, Int32> m_propertyIndices;

			#endregion

			#region ���캯��

			/// <summary>
			/// ���캯��������Ҫ��װ��ʵ�������б�
			/// </summary>
			/// <param name="entity">������ʵ�嶨�塣</param>
			/// <param name="allEntityProperties">ʵ�������б�</param>
			internal EntityPropertyDefinitionCollection(EntityDefinition entity, IList<EntityPropertyDefinition> allEntityProperties)
				: base(allEntityProperties)
			{
				m_entity = entity;

				m_properties = new Dictionary<String, EntityPropertyDefinition>(allEntityProperties.Count);
				m_propertyIndices = new Dictionary<String, Int32>(allEntityProperties.Count);

				for (Int32 i = 0; i < allEntityProperties.Count; i++)
				{
					EntityPropertyDefinition propertyDef = allEntityProperties[i];

					m_properties.Add(propertyDef.Name, propertyDef);
					m_propertyIndices.Add(propertyDef.Name, i);
				}
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡָ�����Ƶ�ʵ�����ԣ��������Ա��������ִ�Сд��
			/// </summary>
			/// <param name="propertyName">�������ơ�</param>
			/// <returns>���и����Ƶ�ʵ�����ԡ�</returns>
			public EntityPropertyDefinition this[String propertyName]
			{
				get
				{
					#region ǰ������

					Debug.Assert(!String.IsNullOrEmpty(propertyName), "�������Ʋ���Ϊ�ա�");

					#endregion

					foreach (EntityPropertyDefinition property in Items)
					{
						if (property.Matches(propertyName))
						{
							return property;
						}
					}

					return null;
				}
			}

			/// <summary>
			/// �ɸ�������������λ�����Զ��壬���ִ�Сд�������Ǳ�����
			/// </summary>
			/// <param name="chain">��������</param>
			/// <returns>���Զ��塣</returns>
			public EntityPropertyDefinition this[IPropertyChain chain]
			{
				get
				{
					#region ǰ������

					Debug.Assert((chain != null), "���������� chain ����Ϊ�ա�");
					Debug.Assert((chain.Type == Entity.Type), "��������ʵ�����������Լ���������ʵ�����Ͳ�ͬ��");

					#endregion

					EntityPropertyDefinition propertyDef = null;

					EntityDefinition definition = this.Entity;
					chain = chain.Head;

					while (true)
					{
						propertyDef = definition.Properties[chain.Name];

						Debug.Assert((propertyDef != null), String.Format("{0} ���������� {1}", definition, chain.Name));

						chain = chain.Next;

						if (chain == null)
						{
							break;
						}

						Debug.Assert(
								!propertyDef.IsPrimitive,
								String.Format(
										"��ʹ�������� {0} ��λ����ʱ���м������ {1} Ϊ�������ԡ�",
										chain,
										propertyDef.Name
									)
							);

						definition = propertyDef.Relation.Parent;
					}

					return propertyDef;
				}
			}

			/// <summary>
			/// ��ȡ������ʵ�嶨�塣
			/// </summary>
			public EntityDefinition Entity
			{
				get { return m_entity; }
			}

			#endregion

			#region ��������

			/// <summary>
			/// �ж��Ƿ����ָ�����Ƶ����Զ��壬�������ִ�Сд�������Ǳ�����
			/// </summary>
			/// <param name="propertyName">�������ơ�</param>
			/// <returns>�������ָ�����Ƶ����ԣ��򷵻� true�����򷵻� false��</returns>
			public Boolean Contains(String propertyName)
			{
				Debug.Assert((propertyName != null), "���� propertyName ����Ϊ�ա�");

				return m_properties.ContainsKey(propertyName);
			}

			/// <summary>
			/// ��ȡ���е���ʵ�����ԡ�
			/// </summary>
			/// <returns>��ʵ�����ԡ�</returns>
			public EntityPropertyDefinition[] GetAllChildrenProperties()
			{
				List<EntityPropertyDefinition> results = new List<EntityPropertyDefinition>();

				foreach (EntityPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.IsChildren)
					{
						results.Add(propertyDef);
					}
				}

				return results.ToArray();
			}

			/// <summary>
			/// ��ȡָ�����Ƶ����Զ��壬�������ִ�Сд�������Ǳ�����
			/// </summary>
			/// <param name="propertyName">�������ơ�</param>
			/// <returns>���и����Ƶ����Զ��塣</returns>
			public EntityPropertyDefinition GetDefinitionByName(String propertyName)
			{
				return m_properties[propertyName];
			}

			/// <summary>
			/// ��ȡָ�����Ƶ����Զ����������������ִ�Сд�������Ǳ�����
			/// </summary>
			/// <param name="propertyName">�������ơ�</param>
			/// <returns>���Զ����������</returns>
			public Int32 GetIndexByName(String propertyName)
			{
				return m_propertyIndices[propertyName];
			}

			/// <summary>
			/// ��ȡ���е��������ԡ�
			/// </summary>
			/// <returns>�������Լ��ϡ�</returns>
			public EntityPropertyDefinition[] GetPrimaryKeys()
			{
				List<EntityPropertyDefinition> all = new List<EntityPropertyDefinition>();

				foreach (EntityPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.IsChildren)
					{
						continue;
					}

					if (propertyDef.IsPrimaryKey)
					{
						all.Add(propertyDef);
					}
				}

				return all.ToArray();
			}

			/// <summary>
			/// ��ȡ�����ݿ��������������ԡ�
			/// </summary>
			/// <returns>���ݿ��������������ԣ���������ڣ��򷵻� null��</returns>
			public EntityPropertyDefinition GetNativePrimaryKey()
			{
				EntityPropertyDefinition nativePrimaryKey = null;

				foreach (EntityPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.IsChildren || !propertyDef.IsPrimaryKey)
					{
						continue;
					}

					if (Attribute.IsDefined(propertyDef.PropertyInfo, typeof(NativeAttribute)))
					{
						nativePrimaryKey = propertyDef;

						break;
					}
				}

				Debug.Assert((nativePrimaryKey == null) || nativePrimaryKey.IsPrimitive, "NativeAttribute Ӧֻ�������Ϊ�����Ļ��������ϡ�");

				return nativePrimaryKey;
			}

			/// <summary>
			/// ��ȡ��ѡ�����ԡ�
			/// </summary>
			/// <returns>��ѡ�����Լ��ϡ�</returns>
			public EntityPropertyDefinition[] GetCandidateKeys()
			{
				List<EntityPropertyDefinition> all = new List<EntityPropertyDefinition>();

				foreach (EntityPropertyDefinition propertyDef in Items)
				{
					if (propertyDef.IsCandidateKey)
					{
						all.Add(propertyDef);
					}
				}

				return all.ToArray();
			}

			/// <summary>
			/// ��ȡ�Զ�����ֵ�ĺ�ѡ����
			/// </summary>
			/// <returns>�Զ�����ֵ�ĺ�ѡ����</returns>
			public EntityPropertyDefinition GetAutoGeneratedCandidateKey()
			{
				EntityPropertyDefinition[] candidates = GetCandidateKeys();

				if ((candidates.Length == 1) && candidates[0].AutoGenerateOnNew)
				{
					return candidates[0];
				}

				return null;
			}

			#endregion
		}
	}
}