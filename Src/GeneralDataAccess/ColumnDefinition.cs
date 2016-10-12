#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ColumnDefinition.cs
// �ļ������������ж��塣
//
//
// ������ʶ���α���billsoff@gmail.com�� 2011
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// �ж��塣
	/// </summary>
	internal sealed class ColumnDefinition : IEquatable<ColumnDefinition>
	{
		#region ˽���ֶ�

		private readonly String m_name;
		private readonly Boolean m_isPrimaryKey;

		private Type m_type;
		private DbType? m_dbType;

		private EntityPropertyDefinition m_property;
		private PropertyChain m_propertyChain;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯���������������Ƿ�Ϊ������
		/// </summary>
		/// <param name="name">������</param>
		/// <param name="isPrimaryKey">ָʾ���Ƿ�Ϊ������</param>
		internal ColumnDefinition(String name, Boolean isPrimaryKey)
		{
			m_name = name;
			m_isPrimaryKey = isPrimaryKey;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�е����ݿ����͡�
		/// </summary>
		public DbType DbType
		{
			get
			{
#if DEBUG
				if (Property.IsPrimitive)
				{
					Debug.Assert((m_dbType != null), "��������ӳ����Ӧ���������ݿ����͡�");
				}
#endif

				if (m_dbType == null)
				{
					ColumnDefinition parentColumn = GetParentColumn();

					m_dbType = parentColumn.DbType;
				}

				return m_dbType.Value;
			}

			internal set { m_dbType = value; }
		}

		/// <summary>
		/// ��ȡ�е�ȫ���ƣ������޶����ơ�
		/// </summary>
		public String FullName
		{
			get { return String.Format("{0}.{1}", Property.Entity.TableName, Name); }
		}

		/// <summary>
		/// ��ȡ�е����ơ�
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�������Ƿ���ԭ���������������ݿ����ɵ���������
		/// </summary>
		public Boolean IsPrimaryKeyNative
		{
			get { return Property.IsPrimaryKeyNative; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ���Ƿ�Ϊ������
		/// </summary>
		public Boolean IsPrimaryKey
		{
			get { return m_isPrimaryKey; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ���Ƿ��ӳټ��ء�
		/// </summary>
		public Boolean LazyLoad
		{
			get
			{
				if (IsPrimaryKey)
				{
					return false;
				}

				return Property.LazyLoad;
			}
		}

		/// <summary>
		/// ��ȡ�е�����ֵת������
		/// </summary>
		public OddValueConverterAttribute OddValueConverter
		{
			get { return Property.OddValueConverter; }
		}

		/// <summary>
		/// ��ȡ�е����ԡ�
		/// </summary>
		public EntityPropertyDefinition Property
		{
			get { return m_property; }
			set { m_property = value; }
		}

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public IPropertyChain PropertyChain
		{
			get
			{
				if (m_propertyChain == null)
				{
					if (Property.IsPrimitive)
					{
						m_propertyChain = new PropertyChain(Property.Entity.Type, new String[] { Property.Name });
					}
					else
					{
						List<String> propertyPath = new List<String>();

						propertyPath.Add(Property.Name);

						ColumnDefinition parentColumnDef = GetParentColumn();

						propertyPath.AddRange(parentColumnDef.PropertyChain.PropertyPath);

						m_propertyChain = new PropertyChain(Property.Entity.Type, propertyPath.ToArray());
					}
				}

				return m_propertyChain;
			}
		}

		/// <summary>
		/// ��ȡ�е����͡�
		/// </summary>
		public Type Type
		{
			get
			{
#if DEBUG
				if (Property.IsPrimitive)
				{
					Debug.Assert((m_type != null), "��������ӳ����Ӧ���������͡�");
				}
#endif

				if (m_type == null)
				{
					ColumnDefinition parentColumn = GetParentColumn();

					m_type = parentColumn.Type;
				}

				return m_type;
			}

			internal set { m_type = value; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�е����ݿ�ֵ��������õ�ֵΪ null������ת��Ϊ DBNull������� DBEmpty����ָʾ�������²������Ը��� ����
		/// </summary>
		/// <param name="entity">Ҫ������ʵ�塣</param>
		/// <returns>ʵ���е�ǰ�е�ֵ��</returns>
		public Object GetDbValue(Object entity)
		{
			if (entity == null)
			{
				return DBNull.Value;
			}

			using (EtyBusinessObject.SuppressEntityLazyLoadTransient(entity))
			{
				Object value;

				if (Property.IsPrimitive)
				{
					if (EtyBusinessObject.IsEmpty(entity, Property.Name))
					{
						value = DBEmpty.Value;
					}
					else
					{
						value = Property.PropertyInfo.GetValue(entity, null);
						OddValueConverterAttribute converter = DbConverter.GetOddValueConverter(this);

						if (converter != null)
						{
							value = converter.GetDbValue(value);
						}
						else if (value == null)
						{
							value = DBNull.Value;
						}

						// ö�������ұ������ı�
						if (Type.IsEnum && (Property.UseEnumText != null))
						{
							switch (Property.UseEnumText.Text)
							{
								case EnumTextOption.Value:
									value = Convert.ChangeType(value, Enum.GetUnderlyingType(Type)).ToString();
									break;

								case EnumTextOption.Name:
								default:
									value = value.ToString();
									break;
							}
						}
					}
				}
				else
				{
					Object parentEntity = Property.PropertyInfo.GetValue(entity, null);
					ColumnDefinition parentColumnDef = GetParentColumn();

					value = parentColumnDef.GetDbValue(parentEntity);
				}

				return value;
			}
		}

		/// <summary>
		/// ��ȡ��ǰ�еĸ�ӳ���С�
		/// </summary>
		/// <returns>��ǰ�еĸ�ӳ���С�</returns>
		public ColumnDefinition GetParentColumn()
		{
			Debug.Assert(!Property.IsPrimitive, "��ǰ��Ϊ�����У�û��ӳ�丸�С�");

			if (Property.IsPrimitive)
			{
				return null;
			}

			EntityPropertyDefinitionRelation relation = Property.Relation;
			ColumnDefinition parentColumnDef;

			if (!Property.HasComproundColumns)
			{
				parentColumnDef = relation.ParentColumns[0];
			}
			else
			{
				Int32 index = Array.IndexOf<ColumnDefinition>(relation.ChildColumns, this);
				parentColumnDef = relation.ParentColumns[index];
			}

			return parentColumnDef;
		}

		/// <summary>
		/// ��ȡ��ǰ�еĻ���ӳ���У���������������Ϊ�������Ե�ӳ���У���
		/// </summary>
		/// <returns>��ǰ�еĻ���ӳ���У������ǰ������������Ϊ�������ԣ��򷵻ص�ǰ�С�</returns>
		public ColumnDefinition GetPrimitiveMappingColumn()
		{
			if (Property.IsPrimitive)
			{
				return this;
			}

			ColumnDefinition parentColumn = GetParentColumn();

			return parentColumn.GetPrimitiveMappingColumn();
		}

		/// <summary>
		/// �ַ�����ʾ����ʾ�е�ȫ����
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return FullName;
		}

		#endregion

		#region �����

		/// <summary>
		/// ��д����ԡ�
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override Boolean Equals(Object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (Object.ReferenceEquals(this, obj))
			{
				return true;
			}

			if (GetType() != obj.GetType())
			{
				return false;
			}

			EntityPropertyDefinition other = (EntityPropertyDefinition)obj;

			return Equals(other);
		}

		/// <summary>
		/// ��д��
		/// </summary>
		/// <returns></returns>
		public override Int32 GetHashCode()
		{
			return (Property.GetHashCode() ^ CommonPolicies.NormalizeColumnName(Name).GetHashCode());
		}

		/// <summary>
		/// ��д��
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean operator ==(ColumnDefinition left, ColumnDefinition right)
		{
			if (Object.ReferenceEquals(left, null))
			{
				return Object.ReferenceEquals(null, right);
			}

			return left.Equals(right);
		}

		/// <summary>
		/// ��д��
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean operator !=(ColumnDefinition left, ColumnDefinition right)
		{
			return !(left == right);
		}

		#region IEquatable<ColumnDefinition> ��Ա

		/// <summary>
		/// ָʾ��ǰ�ж�������Ƿ��ͬ����һ���ж�������������ͬһ��ʵ���ͬһ�����ԣ����������ִ�Сд��ͬ�����ͬ��
		/// </summary>
		/// <param name="other">��һ���ж������</param>
		/// <returns>����ֵ�ǰ�ж����������һ���ж�������ͬ���򷵻� true�����򷵻� false��</returns>
		public Boolean Equals(ColumnDefinition other)
		{
			if (Object.ReferenceEquals(other, null))
			{
				return false;
			}

			if (Object.ReferenceEquals(this, other))
			{
				return true;
			}

			return Property.Equals(other.Property) && Name.Equals(other.Name, CommonPolicies.ColumnNameComparison);
		}

		#endregion

		#endregion
	}
}