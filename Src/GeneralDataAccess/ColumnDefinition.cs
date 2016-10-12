#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ColumnDefinition.cs
// 文件功能描述：列定义。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 2011
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 列定义。
	/// </summary>
	internal sealed class ColumnDefinition : IEquatable<ColumnDefinition>
	{
		#region 私有字段

		private readonly String m_name;
		private readonly Boolean m_isPrimaryKey;

		private Type m_type;
		private DbType? m_dbType;

		private EntityPropertyDefinition m_property;
		private PropertyChain m_propertyChain;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数，设置列名，是否为主键。
		/// </summary>
		/// <param name="name">列名。</param>
		/// <param name="isPrimaryKey">指示列是否为主键。</param>
		internal ColumnDefinition(String name, Boolean isPrimaryKey)
		{
			m_name = name;
			m_isPrimaryKey = isPrimaryKey;
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取列的数据库类型。
		/// </summary>
		public DbType DbType
		{
			get
			{
#if DEBUG
				if (Property.IsPrimitive)
				{
					Debug.Assert((m_dbType != null), "基本属性映射列应设置其数据库类型。");
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
		/// 获取列的全名称，即其限定名称。
		/// </summary>
		public String FullName
		{
			get { return String.Format("{0}.{1}", Property.Entity.TableName, Name); }
		}

		/// <summary>
		/// 获取列的名称。
		/// </summary>
		public String Name
		{
			get { return m_name; }
		}

		/// <summary>
		/// 获取一个值，该值指示此属性是否是原生主键（即由数据库生成的主键）。
		/// </summary>
		public Boolean IsPrimaryKeyNative
		{
			get { return Property.IsPrimaryKeyNative; }
		}

		/// <summary>
		/// 获取一个值，该值指示列是否为主键。
		/// </summary>
		public Boolean IsPrimaryKey
		{
			get { return m_isPrimaryKey; }
		}

		/// <summary>
		/// 获取一个值，该值指示列是否延迟加载。
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
		/// 获取列的奇异值转换器。
		/// </summary>
		public OddValueConverterAttribute OddValueConverter
		{
			get { return Property.OddValueConverter; }
		}

		/// <summary>
		/// 获取列的属性。
		/// </summary>
		public EntityPropertyDefinition Property
		{
			get { return m_property; }
			set { m_property = value; }
		}

		/// <summary>
		/// 获取属性链。
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
		/// 获取列的类型。
		/// </summary>
		public Type Type
		{
			get
			{
#if DEBUG
				if (Property.IsPrimitive)
				{
					Debug.Assert((m_type != null), "基本属性映射列应设置其类型。");
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

		#region 公共方法

		/// <summary>
		/// 获取列的数据库值，即如果该的值为 null，将其转换为 DBNull（如果是 DBEmpty，则指示插入或更新操作忽略该列 ）。
		/// </summary>
		/// <param name="entity">要解析的实体。</param>
		/// <returns>实体中当前列的值。</returns>
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

						// 枚举类型且保存其文本
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
		/// 获取当前列的父映射列。
		/// </summary>
		/// <returns>当前列的父映射列。</returns>
		public ColumnDefinition GetParentColumn()
		{
			Debug.Assert(!Property.IsPrimitive, "当前列为基本列，没有映射父列。");

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
		/// 获取当前列的基本映射列（即其所属的属性为基本属性的映射列）。
		/// </summary>
		/// <returns>当前列的基本映射列，如果当前列所属的属性为基本属性，则返回当前列。</returns>
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
		/// 字符串表示，显示列的全名。
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return FullName;
		}

		#endregion

		#region 相等性

		/// <summary>
		/// 重写相等性。
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
		/// 重写。
		/// </summary>
		/// <returns></returns>
		public override Int32 GetHashCode()
		{
			return (Property.GetHashCode() ^ CommonPolicies.NormalizeColumnName(Name).GetHashCode());
		}

		/// <summary>
		/// 重写。
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
		/// 重写。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean operator !=(ColumnDefinition left, ColumnDefinition right)
		{
			return !(left == right);
		}

		#region IEquatable<ColumnDefinition> 成员

		/// <summary>
		/// 指示当前列定义对象是否等同于另一个列定义对象，如果属于同一个实体的同一个属性，列名不区分大小写相同，则等同。
		/// </summary>
		/// <param name="other">另一个列定义对象。</param>
		/// <returns>如果现当前列定义对象与另一个列定义对象等同，则返回 true；否则返回 false。</returns>
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