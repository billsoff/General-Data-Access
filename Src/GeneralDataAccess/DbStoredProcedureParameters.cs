#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DbStoredProcedureParameters.cs
// 文件功能描述：存储过程参数集合。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110211
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 存储过程参数集合。
	/// </summary>
	[Serializable]
	public class DbStoredProcedureParameters
	{
		#region 私有字段

		private readonly String m_storedProcedureName;

		/// <summary>
		/// 这个存储供子类使用。
		/// </summary>
		private Dictionary<String, Object> m_parameterValues;

		/// <summary>
		/// 这个存储供直接使用。
		/// </summary>
		private Dictionary<String, QueryParameter> m_dbParameters;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数。
		/// </summary>
		public DbStoredProcedureParameters()
		{
		}

		/// <summary>
		/// 构造函数，初始化存储过程名称。
		/// </summary>
		/// <param name="storedProcedureName">存储过程名称。</param>
		public DbStoredProcedureParameters(String storedProcedureName)
		{
			if (storedProcedureName != null)
			{
				storedProcedureName = storedProcedureName.Trim();
			}

			if (String.IsNullOrEmpty(storedProcedureName))
			{
				throw new ArgumentNullException("storedProcedureName", "存储过程名称不能为空或空字符串。");
			}

			m_storedProcedureName = storedProcedureName;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取存储过程名称。
		/// </summary>
		public String StoredProcedureName
		{
			get { return m_storedProcedureName; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 清空参数集合，可再次使用当前实例。
		/// </summary>
		public void ClearDbParameters()
		{
			if (m_dbParameters != null)
			{
				m_dbParameters.Clear();
			}
		}

		/// <summary>
		/// 获取所有的数据库参数。
		/// </summary>
		/// <returns>所有的数据库参数的集合。</returns>
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
		/// 获取与指定参数名对应的属性信息。
		/// </summary>
		/// <param name="parameterName">参数名称。</param>
		/// <returns>与该参数名称相对应的属性信息，如果没有找到，则返回 null。</returns>
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
		/// 获取所有参数属性的枚举，键为参数名。
		/// </summary>
		/// <param name="notIncludingInput">如果为 true；则不包括 Input 参数，否则返回所有参数。</param>
		/// <returns>参数属性枚举。</returns>
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
		/// 获取指定参数名的参数值，该参数是通过调用 SetDbParameter 方法设置的参数。
		/// </summary>
		/// <typeparam name="TParameterValue">参数值的类型。</typeparam>
		/// <param name="parameterName">参数名称。</param>
		/// <returns>该参数的值，通过调用 Convert.ChangeType 进行类型转换。</returns>
		/// <exception cref="InvalidOperationException">参数不存在。</exception>
		/// <exception cref="FormatException">无法转换为指定的类型。</exception>
		public TParameterValue GetDbParameterValue<TParameterValue>(String parameterName)
		{
			if ((m_dbParameters == null) || !m_dbParameters.ContainsKey(parameterName))
			{
				throw new InvalidOperationException(String.Format("未设置参数 {0}。", parameterName));
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
		/// 设置存储过程参数，参数方向为 Input。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbType">数据库类型。</param>
		/// <param name="value">参数值。</param>
		public void SetDbParameter(String name, DbType dbType, Object value)
		{
			SetDbParameter(name, dbType, value, ParameterDirection.Input, 0, false, 0, 0);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbType">数据库类型。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		public void SetDbParameter(String name, DbType dbType, Object value, ParameterDirection direction)
		{
			SetDbParameter(name, dbType, value, direction, 0, false, 0, 0);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbType">数据库类型。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		/// <param name="size">参数大小。</param>
		public void SetDbParameter(String name, DbType dbType, Object value, ParameterDirection direction, Int32 size)
		{
			SetDbParameter(name, dbType, value, direction, size, false, 0, 0);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbType">数据库类型。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		/// <param name="size">参数大小。</param>
		/// <param name="nullable">指示参数是否可以为空。</param>
		public void SetDbParameter(String name, DbType dbType, Object value, ParameterDirection direction, Int32 size, Boolean nullable)
		{
			SetDbParameter(name, dbType, value, direction, size, nullable, 0, 0);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbType">数据库类型。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		/// <param name="size">参数大小。</param>
		/// <param name="nullable">指示参数是否可以为空。</param>
		/// <param name="precision">参数的精度（有效数字个数）。</param>
		/// <param name="scale">参数的小数位数。</param>
		public void SetDbParameter(String name, DbType dbType, Object value, ParameterDirection direction, Int32 size, Boolean nullable, Byte precision, Byte scale)
		{
			ValidateParameterNotDecoratedOnProperty(name);

			QueryParameter parameter = new QueryParameter(name, dbType, value, direction);

			parameter.Size = size;
			parameter.Nullable = nullable;
			parameter.Precision = precision;
			parameter.Scale = scale;

			EnsureDbParameterCollectionCreated();

			// 每个参数应只能设置一次，因此用 Add 方法
			m_dbParameters.Add(name, parameter);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbTypePropertyName">数据库类型属性名称。</param>
		/// <param name="dbTypePropertyValue">数据库类型值。</param>
		/// <param name="suppressRetrieveValue">该值指示是否阻止获取参数的返回值。</param>
		/// <param name="value">参数值。</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value)
		{
			SetDbParameter(name, dbTypePropertyName, dbTypePropertyValue, suppressRetrieveValue, value, ParameterDirection.Input, 0, false, 0, 0);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbTypePropertyName">数据库类型属性名称。</param>
		/// <param name="dbTypePropertyValue">数据库类型值。</param>
		/// <param name="suppressRetrieveValue">该值指示是否阻止获取参数的返回值。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value, ParameterDirection direction)
		{
			SetDbParameter(name, dbTypePropertyName, dbTypePropertyValue, suppressRetrieveValue, value, direction, 0, false, 0, 0);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbTypePropertyName">数据库类型属性名称。</param>
		/// <param name="dbTypePropertyValue">数据库类型值。</param>
		/// <param name="suppressRetrieveValue">该值指示是否阻止获取参数的返回值。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		/// <param name="size">参数大小。</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value, ParameterDirection direction, Int32 size)
		{
			SetDbParameter(name, dbTypePropertyName, dbTypePropertyValue, suppressRetrieveValue, value, direction, size, false, 0, 0);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbTypePropertyName">数据库类型属性名称。</param>
		/// <param name="dbTypePropertyValue">数据库类型值。</param>
		/// <param name="suppressRetrieveValue">该值指示是否阻止获取参数的返回值。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		/// <param name="size">参数大小。</param>
		/// <param name="nullable">指示参数是否可以为空。</param>
		public void SetDbParameter(String name, String dbTypePropertyName, Int32 dbTypePropertyValue, Boolean suppressRetrieveValue, Object value, ParameterDirection direction, Int32 size, Boolean nullable)
		{
			SetDbParameter(name, dbTypePropertyName, dbTypePropertyValue, suppressRetrieveValue, value, direction, size, nullable, 0, 0);
		}

		/// <summary>
		/// 设置存储过程参数。
		/// </summary>
		/// <param name="name">参数名称。</param>
		/// <param name="dbTypePropertyName">数据库类型属性名称。</param>
		/// <param name="dbTypePropertyValue">数据库类型值。</param>
		/// <param name="suppressRetrieveValue">该值指示是否阻止获取参数的返回值。</param>
		/// <param name="value">参数值。</param>
		/// <param name="direction">参数方向。</param>
		/// <param name="size">参数大小。</param>
		/// <param name="nullable">指示参数是否可以为空。</param>
		/// <param name="precision">参数的精度（有效数字个数）。</param>
		/// <param name="scale">参数的小数位数。</param>
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

			// 每个参数应只能设置一次，因此用 Add 方法
			m_dbParameters.Add(name, parameter);
		}

		/// <summary>
		/// 设置非输入参数的值。
		/// </summary>
		/// <param name="valueGotter"><see cref="StoredProcedureParameterValueGotter"/> 委托，获取参数的值。</param>
		public void SetNonInputParameterValues(StoredProcedureParameterValueGotter valueGotter)
		{
			if (valueGotter == null)
			{
				throw new ArgumentNullException("valueGotter", "参数值获取委托不能为空。");
			}

			SetNonInputParameterPropertyValues(valueGotter);
			SetNonInputBaseParameterValues(valueGotter);
		}

		#endregion

		#region 保护的索引器

		/// <summary>
		/// 获取或设置参数。
		/// </summary>
		/// <param name="parameterName">参数名称。</param>
		/// <returns>参数值。</returns>
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

				// 子类的属性可能会多次设置，因引采用索引器的方式设置
				m_parameterValues[parameterName] = value;
			}
		}

		#endregion

		#region 保护的方法

		/// <summary>
		/// 获取参数值，如果该参数已经设置值，则直接强制转换为指定的类型，否则返回该类型的默认值。
		/// </summary>
		/// <typeparam name="TParameterValue">参数属性类型。</typeparam>
		/// <param name="parameterName">参数名称。</param>
		/// <returns>参数值，如果未设置，则返回该类型的默认值。</returns>
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

		#region 辅助方法

		/// <summary>
		/// 确保参数集合被创建。
		/// </summary>
		private void EnsureDbParameterCollectionCreated()
		{
			if (m_dbParameters == null)
			{
				m_dbParameters = new Dictionary<String, QueryParameter>();
			}
		}

		/// <summary>
		/// 确保参数值集合被创建。
		/// </summary>
		private void EnsureParameterValueCollectionCreated()
		{
			if (m_parameterValues == null)
			{
				m_parameterValues = new Dictionary<String, Object>();
			}
		}

		/// <summary>
		/// 获取目标类型的基类型，即如果目标类型是可空类型，则返回其基础类型，其他类型原样返回。
		/// </summary>
		/// <param name="targetType">目标类型。</param>
		/// <returns>目标类型的基类型。</returns>
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
		/// 设置非输入参数属性的值。
		/// </summary>
		/// <param name="valueGotter"><see cref="StoredProcedureParameterValueGotter"/> 委托，获取参数的值。</param>
		private void SetNonInputParameterPropertyValues(StoredProcedureParameterValueGotter valueGotter)
		{
			// 遍历参数属性，取得其值并设置
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
		/// 设置非输入参数的数据库值。
		/// </summary>
		/// <param name="valueGotter"><see cref="StoredProcedureParameterValueGotter"/> 委托，获取参数的值。</param>
		private void SetNonInputBaseParameterValues(StoredProcedureParameterValueGotter valueGotter)
		{
			if (m_dbParameters == null)
			{
				return;
			}

			// 遍历内部参数存储，设置参数的值
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
		/// 验证参数没有标记到属性上。
		/// </summary>
		/// <param name="parameterNameToValidate">要验证的参数名称。</param>
		private void ValidateParameterNotDecoratedOnProperty(String parameterNameToValidate)
		{
			IEnumerable<KeyValuePair<String, PropertyInfo>> parameterProperties = GetParameterProperties(false);

			foreach (KeyValuePair<String, PropertyInfo> kvp in parameterProperties)
			{
				String parameterName = kvp.Key;
				PropertyInfo prop = kvp.Value;

				if (parameterName.Equals(parameterNameToValidate, StringComparison.Ordinal))
				{
					throw new InvalidOperationException(String.Format("参数 {0} 已标记到属性 {1}，无法直接设置该参数。", parameterName, prop.Name));
				}
			}
		}

		#endregion
	}
}