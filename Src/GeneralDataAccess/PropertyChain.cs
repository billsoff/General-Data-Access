#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PropertyChain.cs
// 文件功能描述：属性链，定位属性。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110430
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;



namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 属性链，定位属性。
	/// </summary>
	[Serializable]
	public sealed class PropertyChain : IPropertyChain, IEquatable<PropertyChain>
	{
		#region 私有字段

		private readonly Type m_type;
		private readonly String[] m_propertyPath;

		private readonly String m_fullName;

		[NonSerialized]
		private IPropertyChain m_head;

		[NonSerialized]
		private IPropertyChain m_previous;

		[NonSerialized]
		private IPropertyChain m_next;

		#endregion

		#region 构造函数

		/// <summary>
		/// 构造函数。
		/// </summary>
		/// <param name="type">实体类型。</param>
		/// <param name="propertyPath">属性路径。</param>
		public PropertyChain(Type type, String[] propertyPath)
		{
			#region 前置条件

			Debug.Assert((type != null), "实体类型参数 type 不能为空。");
			Debug.Assert((propertyPath != null) && (propertyPath.Length != 0), "属性列表参数 propertyList 不能为空值或空数组。");

			#endregion

			m_type = type;
			m_propertyPath = propertyPath;

			m_fullName = String.Format("{0}.{1}", m_type.Name, String.Join(".", m_propertyPath));
		}

		#endregion

		#region 公共属性

		/// <summary>
		/// 获取属性列表的深度。
		/// </summary>
		public Int32 Depth
		{
			get { return m_propertyPath.Length; }
		}

		/// <summary>
		/// 获取属性全名称（从实体类型名开始以点分隔的属性列表）。
		/// </summary>
		public String FullName
		{
			get { return m_fullName; }
		}

		/// <summary>
		/// 获取头部。
		/// </summary>
		public IPropertyChain Head
		{
			get
			{
				if (m_head == null)
				{
					if (PropertyPath.Length == 1)
					{
						m_head = this;
					}
					else
					{
						m_head = this.Previous.Head;
					}
				}

				return m_head;
			}
		}

		/// <summary>
		/// 获取一个值，该值指示属性是否为目标实体的直属属性。
		/// </summary>
		public Boolean IsImmediateProperty
		{
			get { return (Depth == 1); }
		}

		/// <summary>
		/// 获取一个值，该值指示属性是否为基本属性。
		/// </summary>
		public Boolean IsPrimitive
		{
			get { return Property.IsPrimitive; }
		}

		/// <summary>
		/// 获取一个值，该值指示当前属性是否为子实体列表属性。
		/// </summary>
		public Boolean IsChildren
		{
			get { return Property.IsChildren; }
		}

		/// <summary>
		/// 获取属性名称。
		/// </summary>
		public String Name
		{
			get { return PropertyPath[PropertyPath.Length - 1]; }
		}

		/// <summary>
		/// 获取下一个属性节点。
		/// </summary>
		public IPropertyChain Next
		{
			get { return m_next; }
		}

		/// <summary>
		/// 获取前一个属性节点。
		/// </summary>
		public IPropertyChain Previous
		{
			get
			{
				if ((m_previous == null) && (PropertyPath.Length > 1))
				{
					String[] previousPropertyPath = new String[PropertyPath.Length - 1];
					Array.Copy(PropertyPath, previousPropertyPath, previousPropertyPath.Length);

					PropertyChain previous = new PropertyChain(Type, previousPropertyPath);
					previous.m_next = this;

					m_previous = previous;
				}

				return m_previous;
			}
		}

		/// <summary>
		/// 获取属性路径列表。
		/// </summary>
		public String[] PropertyPath
		{
			get { return m_propertyPath; }
		}

		/// <summary>
		/// 获取根属性名称。
		/// </summary>
		public String Root
		{
			get { return PropertyPath[0]; }
		}

		/// <summary>
		/// 获取目标实体类型。
		/// </summary>
		public Type Type
		{
			get { return m_type; }
		}

		/// <summary>
		/// 获取属性类型。
		/// </summary>
		public Type PropertyType
		{
			get { return Property.Type; }
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 判断是否属于实体架构。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果属于该实体架构，则返回 true；否则返回 false。</returns>
		public Boolean BelongsTo(EntitySchema schema)
		{
			#region 前置条件

			Debug.Assert(schema != null, "参数 schema 不能为空。");

			#endregion

			if (Type != schema.Composite.Target.Type)
			{
				return false;
			}

			return FullName.StartsWith(schema.PropertyPath, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// 判断是否属于属性。
		/// </summary>
		/// <param name="property">要判断的属性。</param>
		/// <returns>如果属于该属性，则返回 true；否则返回 false。</returns>
		public Boolean BelongsTo(EntityProperty property)
		{
			#region 前置条件

			Debug.Assert(property != null, "参数 property 不能为空。");

			#endregion

			if (Type != property.Schema.Composite.Target.Type)
			{
				return false;
			}

			return FullName.StartsWith(property.PropertyChain.FullName, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// 判断是否包含实体架构。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果包含该实体架构，则返回 true；否则返回 false。</returns>
		public Boolean Contains(EntitySchema schema)
		{
			#region 前置条件

			Debug.Assert(schema != null, "参数 schema 不能为空。");

			#endregion

			if (Type != schema.Composite.Target.Type)
			{
				return false;
			}

			return schema.PropertyPath.StartsWith(FullName, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// 获取当前实例的副本，新实例有独立的 Head 和 Previous 属性，而 Next 属性为空。
		/// </summary>
		/// <returns>当前实例的副本。</returns>
		public IPropertyChain Copy()
		{
			return new PropertyChain(Type, PropertyPath);
		}

		/// <summary>
		/// 判断是否包含属性。
		/// </summary>
		/// <param name="property">要判断的属性。</param>
		/// <returns>如果包含该属性，则返回 true；否则返回 false。</returns>
		public Boolean Contains(EntityProperty property)
		{
			#region 前置条件

			Debug.Assert(property != null, "参数 property 不能为空。");

			#endregion

			if (Type != property.Schema.Composite.Target.Type)
			{
				return false;
			}

			return property.PropertyChain.FullName.StartsWith(FullName, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// 获取实体中属性的值。
		/// </summary>
		/// <param name="entity">实体，类型必须与属性链的目标实体类型相同。</param>
		/// <returns>属性的值，如果属性尚未加载（DBEmpty），则返回 null。</returns>
		public Object GetPropertyValue(Object entity)
		{
			#region 前置条件

			Debug.Assert((entity != null), "实体参数 entity 不能为空。");
			Debug.Assert((entity.GetType() == Type), "实体的类型必须与实体定义的目标类型相同。");

			#endregion

			Object result = entity;
			IPropertyChain chain = Head;
			EntityDefinition definition = EntityDefinitionBuilder.Build(Type);

			using (EtyBusinessObject.SuppressEntityLazyLoadTransient(entity))
			{
				while (true)
				{
					if (EtyBusinessObject.IsEmpty(result, chain.Name))
					{
						result = null;

						break;
					}

					EntityPropertyDefinition propertyDef = definition.Properties[chain.Name];

					Debug.Assert((propertyDef != null), String.Format("实体 {0} 中不存在属性 {1}。", definition.Type.FullName, chain.FullName));

					result = propertyDef.PropertyInfo.GetValue(result, null);

					if (result == null)
					{
						break;
					}

					chain = chain.Next;

					if (chain == null)
					{
						break;
					}

					Debug.Assert(
							!propertyDef.IsChildren,
							String.Format("属性链 {0} 中上一级属性为子实体列表属性，无法继续求值。", chain.FullName)
						);

					definition = EntityDefinitionBuilder.Build(propertyDef.Type);
				}
			}

			return result;
		}

		/// <summary>
		/// 获取实体中属性的值。
		/// </summary>
		/// <typeparam name="TResult">属性类型。</typeparam>
		/// <param name="entity">实体，类型必须与属性链的目标实体类型相同。</param>
		/// <returns>属性的值，如果属性尚未加载（DBEmpty），则返回 null。</returns>
		public TResult GetPropertyValue<TResult>(Object entity)
		{
			Object result = GetPropertyValue(entity);
			Type resultType = typeof(TResult);

			if ((result == null) && resultType.IsArray)
			{
				Type elementType = resultType.GetElementType();
				result = Array.CreateInstance(elementType, 0);
			}

			if (result == null)
			{
				return default(TResult);
			}
			else if (result is IConvertible)
			{
				return (TResult)Convert.ChangeType(result, typeof(TResult));
			}
			else
			{
				return (TResult)result;
			}
		}

		/// <summary>
		/// 属性链的字符串表示。
		/// </summary>
		/// <returns>以点分隔的属性名称列表。</returns>
		public override String ToString()
		{
			return FullName;
		}

		/// <summary>
		/// 判断是否拥有属性。
		/// </summary>
		/// <param name="property">属性。</param>
		/// <returns>如果拥有该属性，则返回 true；否则返回 false。</returns>
		public Boolean OwnProperty(EntityProperty property)
		{
			#region 前置条件

			Debug.Assert(Type == property.Schema.Composite.Target.Type, "属性的目标实体类型与选择器的目标实体类型不同。");

			#endregion

			return FullName.Equals(property.Schema.PropertyPath, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// 向前延伸属性链，属性链的类型为目标属性链的类型。
		/// </summary>
		/// <param name="target">目标属性链。</param>
		/// <returns>延伸后新创建的属性链。</returns>
		public IPropertyChain Preppend(IPropertyChain target)
		{
			if ((target == null) || (target.Depth == 0))
			{
				return this;
			}

			String[] propertyPath = new String[target.Depth + Depth];

			Array.Copy(target.PropertyPath, propertyPath, target.Depth);
			Array.Copy(this.PropertyPath, 0, propertyPath, target.Depth, Depth);

			IPropertyChain result = new PropertyChain(target.Type, propertyPath);

			return result;
		}

		#endregion

		#region 相等性

		/// <summary>
		/// 等同性比较。
		/// </summary>
		/// <param name="obj">另一个对象。</param>
		/// <returns>如果二者等同，则返回 true；否则返回 false。</returns>
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

			return Equals((PropertyChain)obj);
		}

		/// <summary>
		/// 重写。
		/// </summary>
		/// <returns></returns>
		public override Int32 GetHashCode()
		{
			return Type.GetHashCode() ^ FullName.GetHashCode();
		}

		/// <summary>
		/// 重写。
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean operator ==(PropertyChain left, PropertyChain right)
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
		public static Boolean operator !=(PropertyChain left, PropertyChain right)
		{
			return !(left == right);
		}

		#region IEquatable<PropertyChain> 成员

		/// <summary>
		/// 指示当前属性链实例是否与另一个属性链实例等同，如果二者束定于相同的属性，则二者等同。
		/// </summary>
		/// <param name="other">另一个属性链实例。</param>
		/// <returns>如果两者等同，则返回 true；否则返回 false。</returns>
		public Boolean Equals(PropertyChain other)
		{
			if (Object.ReferenceEquals(other, null))
			{
				return false;
			}

			if (Object.ReferenceEquals(this, other))
			{
				return true;
			}

			return (Type == other.Type) && FullName.Equals(other.FullName, CommonPolicies.PropertyNameComparison);
		}

		#endregion

		#endregion

		#region 内部属性

		/// <summary>
		/// 获取目标实体定义。
		/// </summary>
		internal EntityDefinition Target
		{
			get
			{
				return EntityDefinitionBuilder.Build(m_type);
			}
		}

		/// <summary>
		/// 获取当前实体定义。
		/// </summary>
		internal EntityDefinition Current
		{
			get
			{
				EntityPropertyDefinition propertyDef = null;

				EntityDefinition definition = this.Target;
				IPropertyChain chain = this.Head;

				while (true)
				{
					propertyDef = definition.Properties[chain.Name];

					Debug.Assert((propertyDef != null), String.Format("{0} 不存在属性 {1}", definition, chain.Name));

					chain = chain.Next;

					if (chain == null)
					{
						break;
					}

					Debug.Assert(
							!propertyDef.IsPrimitive,
							String.Format(
									"当使用属性链 {0} 定位属性时，中间的属性 {1} 为基本属性。",
									chain,
									propertyDef.Name
								)
						);

					definition = propertyDef.Relation.Parent;
				}

				return definition;
			}
		}

		/// <summary>
		/// 获取属性定义。
		/// </summary>
		internal EntityPropertyDefinition Property
		{
			get
			{
				EntityPropertyDefinition propertyDef = null;

				EntityDefinition definition = this.Target;
				IPropertyChain chain = this.Head;

				while (true)
				{
					propertyDef = definition.Properties[chain.Name];

					Debug.Assert((propertyDef != null), String.Format("{0} 中不存在属性 {1}", definition, chain.Name));

					chain = chain.Next;

					if (chain == null)
					{
						break;
					}

					Debug.Assert(
							!propertyDef.IsPrimitive,
							String.Format(
									"当使用属性链 {0} 定位属性时，中间的属性 {1} 为基本属性。",
									chain,
									propertyDef.Name
								)
						);

					if (!propertyDef.IsChildren)
					{
						definition = propertyDef.Relation.Parent;
					}
					else
					{
						definition = propertyDef.ChildrenProperty.Entity;
					}
				}

				return propertyDef;
			}
		}

		#endregion
	}
}