#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntityDataBag.cs
// �ļ�����������ʵ�����ݰ������ڴ洢ʵ�����ݺͱ���㣨���Ժ�ع�����Ŀǰ��֧���ӳ����Ժͻع����ڽ����İ汾��ȫ��֧��������ʷ��¼��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110516
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
	/// ʵ�����ݰ������ڴ洢ʵ�����ݺͱ���㣨���Ժ�ع�����
	/// </summary>
	[Serializable]
	internal sealed class EntityDataBag
	{
		#region ˽���ֶ�

		private readonly Type m_type;
		private readonly EtyBusinessObject m_entity;

		// �����ӳټ��ص���������
		private Dictionary<String, Object> m_lazyLoads;

		[NonSerialized]
		private Boolean m_hasSavePoint;

		// �����Զ���˳��ɵ�ֵ�б�
		[NonSerialized]
		private Object[] m_savePoint;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ�塣
		/// </summary>
		/// <param name="entity">ʵ�塣</param>
		public EntityDataBag(EtyBusinessObject entity)
		{
			#region ǰ������

			Debug.Assert((entity != null), "ʵ����� entity ����Ϊ�ա�");

			#endregion

			m_type = entity.GetType();
			m_entity = entity;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡʵ�塣
		/// </summary>
		public EtyBusinessObject Entity
		{
			get { return m_entity; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ������˱���㡣
		/// </summary>
		public Boolean HasSavePoint
		{
			get { return m_hasSavePoint; }
		}

		/// <summary>
		/// ��ȡʵ�����͡�
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// �������㡣
		/// </summary>
		public void ClearSavePoint()
		{
			m_hasSavePoint = false;
		}

		/// <summary>
		/// ��ȡ���Ե�ֵ��������ʾ����ǿ��ת������
		/// </summary>
		/// <typeparam name="TValue">�������͡�</typeparam>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>����ֵ��</returns>
		public TValue GetLazyLoadValue<TValue>(String propertyName)
		{
			#region ǰ������

#if DEBUG

			EntityPropertyDefinition propertyDef = Definition.Properties[propertyName];

			Debug.Assert(propertyDef.LazyLoad, String.Format("���� {0} û�б��Ϊ�ӳټ��ء�", propertyName));

#endif

			#endregion

			if (IsEmpty(propertyName) && Entity.MayLazyLoad)
			{
				LoadLazyLoadValue(propertyName);
			}

			Object value = LazyLoads[propertyName];

			if ((value != null) && !DbConverter.IsDBEmpty(value))
			{
				return (TValue)value;
			}
			else
			{
				return default(TValue);
			}
		}

		/// <summary>
		/// �ж�ָ�������Ե�ֵ�Ƿ�Ϊ DBEmpty������δ��ʼ������
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>��������Ե�ֵ��δ��ʼ�����򷵻� true�����򷵻� false��</returns>
		public Boolean IsEmpty(String propertyName)
		{
			#region ǰ������

			Debug.Assert((propertyName != null), "�������Ʋ��� propertyName ����Ϊ�ա�");

			#endregion

			return LazyLoads.ContainsKey(propertyName) && DbConverter.IsDBEmpty(LazyLoads[propertyName]);
		}

		/// <summary>
		/// �ع��������һ������㡣
		/// </summary>
		/// <returns>����������ڣ���ع��ɹ������� true�����򷵻� false��</returns>
		public Boolean Rollback()
		{
			if (!HasSavePoint)
			{
				return false;
			}

			for (Int32 i = 0; i < SavePoint.Length; i++)
			{
				EntityPropertyDefinition propertyDef = Definition.Properties[i];

				if (!propertyDef.LazyLoad || !IsEmpty(propertyDef.Name))
				{
					propertyDef.PropertyInfo.SetValue(Entity, SavePoint[i], null);
				}
				else
				{
					Entity.SetEmpty(propertyDef.Name);
				}
			}

			m_hasSavePoint = false;

			return true;
		}

		/// <summary>
		/// ���ӳټ���������Ϊ DBEmpty��
		/// </summary>
		/// <param name="propertyName">�ӳټ����������ơ�</param>
		public void SetEmpty(String propertyName)
		{
			#region ǰ������

#if DEBUG

			EntityPropertyDefinition propertyDef = Definition.Properties[propertyName];

			Debug.Assert(propertyDef.LazyLoad, String.Format("���� {0} û�б��Ϊ�ӳټ��ء�", propertyName));

#endif

			#endregion

			LazyLoads[propertyName] = DBEmpty.Value;
		}

		/// <summary>
		/// �����ӳټ������Ե�ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="propertyValue">����ֵ��</param>
		public void SetLazyLoadValue(String propertyName, Object propertyValue)
		{
			#region ǰ������

#if DEBUG

			EntityPropertyDefinition propertyDef = Definition.Properties[propertyName];

			Debug.Assert(propertyDef.LazyLoad, String.Format("���� {0} û�б��Ϊ�ӳټ��ء�", propertyName));

#endif

			#endregion

			LazyLoads[propertyName] = propertyValue;
		}

		/// <summary>
		/// ���ñ���㡣
		/// </summary>
		public void SetSavePoint()
		{
			EntityDefinition definition = EntityDefinitionBuilder.Build(Entity.GetType());

			for (Int32 i = 0; i < SavePoint.Length; i++)
			{
				EntityPropertyDefinition propertyDef = Definition.Properties[i];

				if (!propertyDef.LazyLoad || !IsEmpty(propertyDef.Name))
				{
					SavePoint[i] = propertyDef.PropertyInfo.GetValue(Entity, null);
				}
				else
				{
					SavePoint[i] = DBEmpty.Value;
				}
			}

			m_hasSavePoint = true;
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡʵ�嶨�塣
		/// </summary>
		internal EntityDefinition Definition
		{
			get { return EntityDefinitionBuilder.Build(Type); }
		}

		#endregion

		#region ˽������

		/// <summary>
		/// ��ȡ�ӳټ���ֵ���ϣ������������ơ�
		/// </summary>
		private Dictionary<String, Object> LazyLoads
		{
			get
			{
				if (m_lazyLoads == null)
				{
					m_lazyLoads = new Dictionary<String, Object>();

					foreach (EntityPropertyDefinition propertyDef in Definition.Properties)
					{
						if (propertyDef.LazyLoad)
						{
							m_lazyLoads.Add(propertyDef.Name, DBEmpty.Value);
						}
					}
				}

				return m_lazyLoads;
			}
		}

		/// <summary>
		/// ��ȡ�洢�㡣
		/// </summary>
		private Object[] SavePoint
		{
			get
			{
				if (m_savePoint == null)
				{
					m_savePoint = new Object[Definition.Properties.Count];

					for (Int32 i = 0; i < m_savePoint.Length; i++)
					{
						m_savePoint[i] = DBEmpty.Value;
					}
				}

				return m_savePoint;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// �����ӳ�����ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns></returns>
		private void LoadLazyLoadValue(String propertyName)
		{
			PropertyChain chain = new PropertyChain(Type, new String[] { propertyName });

			Object partialEntity = Entity.DatabaseSession.LoadFirst(
					Type,
					Select.Properties(chain),
					Definition.ComposeLoadFilter(Entity)
				);

			EntityPropertyDefinition propertyDef = Definition.Properties[propertyName];

			LazyLoads[propertyName] = propertyDef.PropertyInfo.GetValue(partialEntity, null);
		}

		#endregion
	}
}