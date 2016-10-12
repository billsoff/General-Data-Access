#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����DbStoredProcedureParameters.cs
// �ļ������������洢���̲������ϡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110211
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// �洢���̲������ϡ�
	/// </summary>
	[Serializable]
	public class DbStoredProcedureParameters
	{
		#region ˽���ֶ�

		private readonly String m_storedProcedureName;

		/// <summary>
		/// ����洢������ʹ�á�
		/// </summary>
		private Dictionary<String, Object> m_parameterValues;

		/// <summary>
		/// ����洢��ֱ��ʹ�á�
		/// </summary>
		private Dictionary<String, QueryParameter> m_dbParameters;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public DbStoredProcedureParameters()
		{
		}

		/// <summary>
		/// ���캯������ʼ���洢�������ơ�
		/// </summary>
		/// <param name="storedProcedureName">�洢�������ơ�</param>
		public DbStoredProcedureParameters(String storedProcedureName)
		{
			if (storedProcedureName != null)
			{
				storedProcedureName = storedProcedureName.Trim();
			}

			if (String.IsNullOrEmpty(storedProcedureName))
			{
				throw new ArgumentNullException("storedProcedureName", "�洢�������Ʋ���Ϊ�ջ���ַ�����");
			}

			m_storedProcedureName = storedProcedureName;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�洢�������ơ�
		/// </summary>
		public String StoredProcedureName
		{
			get { return m_storedProcedureName; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ղ������ϣ����ٴ�ʹ�õ�ǰʵ����
		/// </summary>
		public void ClearDbParameters()
		{
			if (m_dbParameters != null)
			{
				m_dbParameters.Clear();
			}
		}

		/// <summary>
		/// ��ȡ���е����ݿ������
		/// </summary>
		/// <returns>���е����ݿ�����ļ��ϡ�</returns>
		public QueryParameter[] GetDbParameters()
		{
			List<QueryParameter> allParameters = new List<QueryParameter>();

			if (m_dbParameters != null)
			{
				allParameters.AddRange(m_dbParameters.Values);
			}

			Type actualType = GetType();

			foreach (PropertyInfo prop in actualType.GetProperties())
			{
				StoredProcedureParameterAttribute parameterAttr = (StoredProcedureParameterAttribute)Attribute.GetCustomAttribute(prop, typeof(StoredProcedureParameterAttribute));

				if ((parameterAttr != null) && prop.CanRead)
				{
					Object parameterValue = prop.GetValue(this, null);

					allParameters.Add(parameterAttr.CreateParameter(parameterValue));
				}
			}

			return allParameters.ToArray();
		}

		/// <summary>
		/// ��ȡ��ָ����������Ӧ��������Ϣ��
		/// </summary>
		/// <param name="parameterName">�������ơ�</param>
		/// <returns>��ò����������Ӧ��������Ϣ�����û���ҵ����򷵻� null��</returns>
		public PropertyInfo GetPropertyInfoByParameterName(String parameterName)
		{
			if (String.IsNullOrEmpty(parameterName))
			{
				return null;
			}

			Type actualType = GetType();

			foreach (PropertyInfo prop in actualType.GetProperties())
			{
				StoredProcedureParameterAttribute parameterAttr = (StoredProcedureParameterAttribute)Attribute.GetCustomAttribute(prop, typeof(StoredProcedureParameterAttribute));

				if ((parameterAttr != null) && parameterAttr.Name.Equals(parameterName, StringComparison.Ordinal))
				{
					return prop;
				}
			}

			return null;
		}

		/// <summary>
		/// ��ȡ���в������Ե�ö�٣���Ϊ��������
		/// </summary>
		/// <param name="notIncludingInput">���Ϊ true���򲻰��� Input ���������򷵻����в�����</param>
		/// <returns>��������ö�١�</returns>
		public IEnumerable<KeyValuePair<String, PropertyInfo>> GetParameterProperties(Boolean notIncludingInput)
		{
			Type actualType = GetType();

			foreach (PropertyInfo prop in actualType.GetProperties())
			{
				StoredProcedureParameterAttribute parameterAttr = (StoredProcedureParameterAttribute)Attribute.GetCustomAttribute(prop, typeof(StoredProcedureParameterAttribute));

				if ((parameterAttr != null) && (!notIncludingInput || (parameterAttr.Direction != ParameterDirection.Input)))
				{
					yield return new KeyValuePair<String, PropertyInfo>(parameterAttr.Name, prop);
				}
			}
		}

		/// <summary>
		/// ��ȡָ���������Ĳ���ֵ���ò�����ͨ������ SetDbParameter �������õĲ�����
		/// </summary>
		/// <typeparam name="TParameterValue">����ֵ�����͡�</typeparam>
		/// <param name="parameterName">�������ơ�</param>
		/// <returns>�ò�����ֵ��ͨ������ Convert.ChangeType ��������ת����</returns>
		/// <exception cref="InvalidOperationException">���������ڡ�</exception>
		/// <exception cref="FormatException">�޷�ת��Ϊָ�������͡�</exception>
		public TParameterValue GetDbParameterValue<TParameterValue>(String parameterName)
		{
			if ((m_dbParameters == null) || !m_dbParameters.ContainsKey(parameterName))
			{
				throw new InvalidOperationException(String.Format("δ���ò��� {0}��", parameterName));
			}

			QueryParameter parameter = m_dbParameters[parameterName];

			if ((parameter.Value == null) || Convert.IsDBNull(parameter.Value))
			{
				return default(TParameterValue);
			}
			else
			{
				Type targetType = GetBaseType(typeof(TParameterValue));

				return (TParameterValue)Convert.ChangeType(parameter.Value, targetType);
			}
		}

		/// <summary>
		/// ���ô洢���̲�������������Ϊ Input��
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbType">���ݿ����͡�</param>
		/// <param name="value">����ֵ��</param>
		public void SetDbParameter(String name, DbType dbType, Object value)
		{
			SetDbParameter(name, dbType, value, ParameterDirection.Input, 0, false, 0, 0);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbType">���ݿ����͡�</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		public void SetDbParameter(String name, DbType dbType, Object value, ParameterDirection direction)
		{
			SetDbParameter(name, dbType, value, direction, 0, false, 0, 0);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbType">���ݿ����͡�</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		/// <param name="size">������С��</param>
		public void SetDbParameter(String name, DbType dbType, Object value, ParameterDirection direction, Int32 size)
		{
			SetDbParameter(name, dbType, value, direction, size, false, 0, 0);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbType">���ݿ����͡�</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		/// <param name="size">������С��</param>
		/// <param name="nullable">ָʾ�����Ƿ����Ϊ�ա�</param>
		public void SetDbParameter(String name, DbType dbType, Object value, ParameterDirection direction, Int32 size, Boolean nullable)
		{
			SetDbParameter(name, dbType, value, direction, size, nullable, 0, 0);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbType">���ݿ����͡�</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		/// <param name="size">������С��</param>
		/// <param name="nullable">ָʾ�����Ƿ����Ϊ�ա�</param>
		/// <param name="precision">�����ľ��ȣ���Ч���ָ�������</param>
		/// <param name="scale">������С��λ����</param>
		public void SetDbParameter(String name, DbType dbType, Object value, ParameterDirection direction, Int32 size, Boolean nullable, Byte precision, Byte scale)
		{
			ValidateParameterNotDecoratedOnProperty(name);

			QueryParameter parameter = new QueryParameter(name, dbType, value, direction);

			parameter.Size = size;
			parameter.Nullable = nullable;
			parameter.Precision = precision;
			parameter.Scale = scale;

			EnsureDbParameterCollectionCreated();

			// ÿ������Ӧֻ������һ�Σ������ Add ����
			m_dbParameters.Add(name, parameter);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbTypePropertyName">���ݿ������������ơ�</param>
		/// <param name="dbTypePropertyValue">���ݿ�����ֵ��</param>
		/// <param name="suppressRetrieveValue">��ֵָʾ�Ƿ���ֹ��ȡ�����ķ���ֵ��</param>
		/// <param name="value">����ֵ��</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value)
		{
			SetDbParameter(name, dbTypePropertyName, dbTypePropertyValue, suppressRetrieveValue, value, ParameterDirection.Input, 0, false, 0, 0);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbTypePropertyName">���ݿ������������ơ�</param>
		/// <param name="dbTypePropertyValue">���ݿ�����ֵ��</param>
		/// <param name="suppressRetrieveValue">��ֵָʾ�Ƿ���ֹ��ȡ�����ķ���ֵ��</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value, ParameterDirection direction)
		{
			SetDbParameter(name, dbTypePropertyName, dbTypePropertyValue, suppressRetrieveValue, value, direction, 0, false, 0, 0);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbTypePropertyName">���ݿ������������ơ�</param>
		/// <param name="dbTypePropertyValue">���ݿ�����ֵ��</param>
		/// <param name="suppressRetrieveValue">��ֵָʾ�Ƿ���ֹ��ȡ�����ķ���ֵ��</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		/// <param name="size">������С��</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value, ParameterDirection direction, Int32 size)
		{
			SetDbParameter(name, dbTypePropertyName, dbTypePropertyValue, suppressRetrieveValue, value, direction, size, false, 0, 0);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbTypePropertyName">���ݿ������������ơ�</param>
		/// <param name="dbTypePropertyValue">���ݿ�����ֵ��</param>
		/// <param name="suppressRetrieveValue">��ֵָʾ�Ƿ���ֹ��ȡ�����ķ���ֵ��</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		/// <param name="size">������С��</param>
		/// <param name="nullable">ָʾ�����Ƿ����Ϊ�ա�</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value, ParameterDirection direction, Int32 size, Boolean nullable)
		{
			SetDbParameter(name, dbTypePropertyName, dbTypePropertyValue, suppressRetrieveValue, value, direction, size, nullable, 0, 0);
		}

		/// <summary>
		/// ���ô洢���̲�����
		/// </summary>
		/// <param name="name">�������ơ�</param>
		/// <param name="dbTypePropertyName">���ݿ������������ơ�</param>
		/// <param name="dbTypePropertyValue">���ݿ�����ֵ��</param>
		/// <param name="suppressRetrieveValue">��ֵָʾ�Ƿ���ֹ��ȡ�����ķ���ֵ��</param>
		/// <param name="value">����ֵ��</param>
		/// <param name="direction">��������</param>
		/// <param name="size">������С��</param>
		/// <param name="nullable">ָʾ�����Ƿ����Ϊ�ա�</param>
		/// <param name="precision">�����ľ��ȣ���Ч���ָ�������</param>
		/// <param name="scale">������С��λ����</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value, ParameterDirection direction, Int32 size, Boolean nullable, Byte precision, Byte scale)
		{
			ValidateParameterNotDecoratedOnProperty(name);

			QueryParameter parameter = new QueryParameter(name, DbType.Object, value, direction);

			parameter.DbTypePropertyName = dbTypePropertyName;
			parameter.DbTypePropertyValue = dbTypePropertyValue;
			parameter.SuppressRetrieveValue = suppressRetrieveValue;

			parameter.Size = size;
			parameter.Nullable = nullable;
			parameter.Precision = precision;
			parameter.Scale = scale;

			EnsureDbParameterCollectionCreated();

			// ÿ������Ӧֻ������һ�Σ������ Add ����
			m_dbParameters.Add(name, parameter);
		}

		/// <summary>
		/// ���÷����������ֵ��
		/// </summary>
		/// <param name="valueGotter"><see cref="StoredProcedureParameterValueGotter"/> ί�У���ȡ������ֵ��</param>
		public void SetNonInputParameterValues(StoredProcedureParameterValueGotter valueGotter)
		{
			if (valueGotter == null)
			{
				throw new ArgumentNullException("valueGotter", "����ֵ��ȡί�в���Ϊ�ա�");
			}

			SetNonInputParameterPropertyValues(valueGotter);
			SetNonInputBaseParameterValues(valueGotter);
		}

		#endregion

		#region ������������

		/// <summary>
		/// ��ȡ�����ò�����
		/// </summary>
		/// <param name="parameterName">�������ơ�</param>
		/// <returns>����ֵ��</returns>
		protected Object this[String parameterName]
		{
			get
			{
				if ((m_parameterValues != null) && m_parameterValues.ContainsKey(parameterName))
				{
					return m_parameterValues[parameterName];
				}
				else
				{
					return null;
				}
			}

			set
			{
				EnsureParameterValueCollectionCreated();

				// ��������Կ��ܻ������ã����������������ķ�ʽ����
				m_parameterValues[parameterName] = value;
			}
		}

		#endregion

		#region �����ķ���

		/// <summary>
		/// ��ȡ����ֵ������ò����Ѿ�����ֵ����ֱ��ǿ��ת��Ϊָ�������ͣ����򷵻ظ����͵�Ĭ��ֵ��
		/// </summary>
		/// <typeparam name="TParameterValue">�����������͡�</typeparam>
		/// <param name="parameterName">�������ơ�</param>
		/// <returns>����ֵ�����δ���ã��򷵻ظ����͵�Ĭ��ֵ��</returns>
		protected TParameterValue SafeCastParametValue<TParameterValue>(String parameterName)
		{
			Object parameterValue = this[parameterName];

			if (parameterValue != null)
			{
				return (TParameterValue)parameterValue;
			}
			else
			{
				return default(TParameterValue);
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ȷ���������ϱ�������
		/// </summary>
		private void EnsureDbParameterCollectionCreated()
		{
			if (m_dbParameters == null)
			{
				m_dbParameters = new Dictionary<String, QueryParameter>();
			}
		}

		/// <summary>
		/// ȷ������ֵ���ϱ�������
		/// </summary>
		private void EnsureParameterValueCollectionCreated()
		{
			if (m_parameterValues == null)
			{
				m_parameterValues = new Dictionary<String, Object>();
			}
		}

		/// <summary>
		/// ��ȡĿ�����͵Ļ����ͣ������Ŀ�������ǿɿ����ͣ��򷵻���������ͣ���������ԭ�����ء�
		/// </summary>
		/// <param name="targetType">Ŀ�����͡�</param>
		/// <returns>Ŀ�����͵Ļ����͡�</returns>
		private static Type GetBaseType(Type targetType)
		{
			if (targetType.IsGenericType && (targetType.GetGenericTypeDefinition() == typeof(Nullable<>)))
			{
				Type[] argumentTypes = targetType.GetGenericArguments();

				targetType = argumentTypes[0];
			}

			return targetType;
		}

		/// <summary>
		/// ���÷�����������Ե�ֵ��
		/// </summary>
		/// <param name="valueGotter"><see cref="StoredProcedureParameterValueGotter"/> ί�У���ȡ������ֵ��</param>
		private void SetNonInputParameterPropertyValues(StoredProcedureParameterValueGotter valueGotter)
		{
			// �����������ԣ�ȡ����ֵ������
			foreach (KeyValuePair<String, PropertyInfo> kvp in GetParameterProperties(true))
			{
				PropertyInfo prop = kvp.Value;

				StoredProcedureParameterAttribute storedProcAttr = (StoredProcedureParameterAttribute)Attribute.GetCustomAttribute(prop, typeof(StoredProcedureParameterAttribute));

				if (storedProcAttr.SuppressRetrieveValue)
				{
					continue;
				}

				String parameterName = kvp.Key;

				Object dbValue = valueGotter(parameterName);
				Object propValue;

				if (!Convert.IsDBNull(dbValue))
				{
					Type targetType = GetBaseType(prop.PropertyType);

					propValue = Convert.ChangeType(dbValue, targetType);
				}
				else
				{
					propValue = null;
				}

				prop.SetValue(this, propValue, null);
			}
		}

		/// <summary>
		/// ���÷�������������ݿ�ֵ��
		/// </summary>
		/// <param name="valueGotter"><see cref="StoredProcedureParameterValueGotter"/> ί�У���ȡ������ֵ��</param>
		private void SetNonInputBaseParameterValues(StoredProcedureParameterValueGotter valueGotter)
		{
			if (m_dbParameters == null)
			{
				return;
			}

			// �����ڲ������洢�����ò�����ֵ
			foreach (KeyValuePair<String, QueryParameter> kvp in m_dbParameters)
			{
				QueryParameter parameter = kvp.Value;

				if ((parameter.Direction != ParameterDirection.Input) && !parameter.SuppressRetrieveValue)
				{
					String parameterName = kvp.Key;
					Object dbValue = valueGotter(parameterName);

					parameter.Value = dbValue;
				}
			}
		}

		/// <summary>
		/// ��֤����û�б�ǵ������ϡ�
		/// </summary>
		/// <param name="parameterNameToValidate">Ҫ��֤�Ĳ������ơ�</param>
		private void ValidateParameterNotDecoratedOnProperty(String parameterNameToValidate)
		{
			IEnumerable<KeyValuePair<String, PropertyInfo>> parameterProperties = GetParameterProperties(false);

			foreach (KeyValuePair<String, PropertyInfo> kvp in parameterProperties)
			{
				String parameterName = kvp.Key;
				PropertyInfo prop = kvp.Value;

				if (parameterName.Equals(parameterNameToValidate, StringComparison.Ordinal))
				{
					throw new InvalidOperationException(String.Format("���� {0} �ѱ�ǵ����� {1}���޷�ֱ�����øò�����", parameterName, prop.Name));
				}
			}
		}

		#endregion
	}
}