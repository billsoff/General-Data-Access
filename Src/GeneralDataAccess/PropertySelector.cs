#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PropertySelector.cs
// �ļ���������������ѡ������ѡ��Ҫ���ص����ԡ�
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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ����ѡ������ѡ��Ҫ���ص����ԡ�
	/// </summary>
	[Serializable]
	public abstract class PropertySelector
	{
		#region ��������

		/// <summary>
		/// ����Ŀ��ʵ�����͵�ѡ������ֻ������ AllFromSchema��LoadSchemaOnly �� PrimaryKey��
		/// </summary>
		/// <param name="selectMode">ѡ��ģʽ��</param>
		/// <param name="entityType">Ŀ��ʵ�����͡�</param>
		/// <returns>����ѡ������</returns>
		public static PropertySelector Create(PropertySelectMode selectMode, Type entityType)
		{
			switch (selectMode)
			{
				case PropertySelectMode.AllFromSchema:
					return new AllFromSchemaPropertySelector(entityType);

				case PropertySelectMode.AllExceptLazyLoadFromSchema:
					return new AllExceptLazyLoadFromSchemaPropertySelector(entityType);

				case PropertySelectMode.LoadSchemaOnly:
					return new LoadSchemaOnlyPropertySelector(entityType);

				case PropertySelectMode.PrimaryKey:
					return new PrimaryKeyPropertySelector(entityType);

				case PropertySelectMode.Property:
					Debug.Fail("��֧�� Type ���͵Ĳ�����");
					break;

				default:
					Debug.Fail("����ʶ�� selectMode");
					break;
			}

			return null;
		}

		/// <summary>
		/// ��������ѡ������
		/// </summary>
		/// <param name="selectMode">����ģʽ��</param>
		/// <param name="chain">��������</param>
		/// <returns>����ѡ������</returns>
		public static PropertySelector Create(PropertySelectMode selectMode, IPropertyChain chain)
		{
			switch (selectMode)
			{
				case PropertySelectMode.AllFromSchema:
					return new AllFromSchemaPropertySelector(chain);

				case PropertySelectMode.AllExceptLazyLoadFromSchema:
					return new AllExceptLazyLoadFromSchemaPropertySelector(chain);

				case PropertySelectMode.LoadSchemaOnly:
					return new LoadSchemaOnlyPropertySelector(chain);

				case PropertySelectMode.Property:
					return new ActualPropertySelector(chain);

				case PropertySelectMode.PrimaryKey:
					return new PrimaryKeyPropertySelector(chain);

				default:
					Debug.Fail("����ʶ�� selectMode");
					break;
			}

			return null;
		}

		/// <summary>
		/// ��������ѡ������
		/// </summary>
		/// <param name="selectMode">����ģʽ��</param>
		/// <param name="builder">��������������</param>
		/// <returns>����ѡ������</returns>
		public static PropertySelector Create(PropertySelectMode selectMode, IPropertyChainBuilder builder)
		{
			switch (selectMode)
			{
				case PropertySelectMode.AllFromSchema:
					return new AllFromSchemaPropertySelector(builder);

				case PropertySelectMode.AllExceptLazyLoadFromSchema:
					return new AllExceptLazyLoadFromSchemaPropertySelector(builder);

				case PropertySelectMode.LoadSchemaOnly:
					return new LoadSchemaOnlyPropertySelector(builder);

				case PropertySelectMode.Property:
					return new ActualPropertySelector(builder);

				case PropertySelectMode.PrimaryKey:
					return new PrimaryKeyPropertySelector(builder);

				default:
					Debug.Fail("����ʶ�� selectMode");
					break;
			}

			return null;
		}

		/// <summary>
		/// ���� Property ģʽ��ѡ������
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>����ѡ������</returns>
		public static PropertySelector Create(IPropertyChain chain)
		{
			return Create(PropertySelectMode.Property, chain);
		}

		/// <summary>
		/// ���� Property ģʽ��ѡ������
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>����ѡ������</returns>
		public static PropertySelector Create(IPropertyChainBuilder builder)
		{
			return Create(PropertySelectMode.Property, builder);
		}

		#endregion

		#region ˽���ֶ�

		private readonly Type m_type;
		private readonly IPropertyChain m_propertyChain;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯��������ʵ�����͡�
		/// </summary>
		/// <param name="entityType">ʵ�����ͣ�����Ϊ�ա�</param>
		internal PropertySelector(Type entityType)
		{
			#region ǰ������

			Debug.Assert((entityType != null), "ʵ�����Ͳ��� entityType ����Ϊ�ա�");

			#endregion

			m_type = entityType;
		}

		/// <summary>
		/// ���캯����������������������
		/// </summary>
		/// <param name="builder">��������������</param>
		internal PropertySelector(IPropertyChainBuilder builder)
			: this(builder.Build())
		{
		}

		/// <summary>
		/// ���캯����������������
		/// </summary>
		/// <param name="chain">������������Ϊ�ա�</param>
		internal PropertySelector(IPropertyChain chain)
		{
			#region ǰ������

			Debug.Assert((chain != null), "���������� chain ������ա�");

			#endregion

			m_type = chain.Type;
			m_propertyChain = chain;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ��ʾ���ơ�
		/// </summary>
		public virtual String DisplayName
		{
			get { return Name; }
		}

		/// <summary>
		/// ��ȡѡ���������ơ�
		/// </summary>
		public String Name
		{
			get
			{
				if (PropertyChain != null)
				{
					return PropertyChain.FullName;
				}
				else
				{
					return m_type.Name;
				}
			}
		}

		/// <summary>
		/// ��ȡ��������
		/// </summary>
		public IPropertyChain PropertyChain
		{
			get { return m_propertyChain; }
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
		/// ָʾ�Ƿ����ָ����ʵ��ܹ������Ǽ�������λ���������ϵ�ʵ��ܹ���
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>�����Ҫ���ظ�ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		public Boolean LoadSchema(EntitySchema schema)
		{
			#region ǰ������

			Debug.Assert((schema != null), "Ҫ�жϵ�ʵ��ܹ����� schema ����Ϊ�ա�");

			#endregion

			return Contains(schema);
		}

		/// <summary>
		/// ָʾ�Ƿ񲻴�ָ����ʵ��ܹ���ѡ���κ����ԡ�
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>������Ӹ�ʵ��ܹ���ѡ���κ����ԣ��򷵻� true�����򷵻� false��</returns>
		public Boolean SelectNothingFrom(EntitySchema schema)
		{
			#region ǰ������

			Debug.Assert((schema != null), "Ҫ�жϵ�ʵ��ܹ����� schema ����Ϊ�ա�");

			#endregion

			return SelectNothingFromImpl(schema);
		}

		/// <summary>
		/// ָʾ�Ƿ�ѡ��ָ�������ԡ�
		/// </summary>
		/// <param name="property">���ԡ�</param>
		/// <returns>���Ҫѡ������ԣ��򷵻� true�����򷵻� false��</returns>
		public Boolean SelectProperty(EntityProperty property)
		{
			#region ǰ������

			Debug.Assert((property != null), "Ҫ�жϵ����Բ��� property ����Ϊ�ա�");

			#endregion

			return SelectPropertyImpl(property);
		}

		/// <summary>
		/// ��ʾ���ơ�
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return DisplayName;
		}

		#endregion

		#region �����Ա

		#region ��������

		/// <summary>
		/// ��������ѡ��ģʽ��
		/// </summary>
		public abstract PropertySelectMode SelectMode { get; }

		#endregion

		#region �����ķ���

		/// <summary>
		/// ָʾ�Ƿ񲻴�ָ����ʵ��ܹ���ѡ���κ����ԡ�
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>������Ӹ�ʵ��ܹ���ѡ���κ����ԣ��򷵻� true�����򷵻� false��</returns>
		protected abstract Boolean SelectNothingFromImpl(EntitySchema schema);

		/// <summary>
		/// ָʾ�Ƿ�ѡ��ָ�������ԡ�
		/// </summary>
		/// <param name="property">���ԡ�</param>
		/// <returns>���Ҫѡ������ԣ��򷵻� true�����򷵻� false��</returns>
		protected abstract Boolean SelectPropertyImpl(EntityProperty property);

		#endregion

		#endregion

		#region �����ķ���

		/// <summary>
		/// �ж��Ƿ����ʵ��ܹ���
		/// </summary>
		/// <param name="schema">Ҫ�жϵ�ʵ��ܹ���</param>
		/// <returns>�������ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		protected Boolean Contains(EntitySchema schema)
		{
			#region ǰ������

			Debug.Assert((Type == schema.Composite.Target.Type), "ʵ��ܹ���Ŀ��ʵ��������ѡ������Ŀ��ʵ�����Ͳ�ͬ��");

			#endregion

			return Name.StartsWith(schema.PropertyPath, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// �ж��Ƿ�������ԡ�
		/// </summary>
		/// <param name="property">���ԡ�</param>
		/// <returns>����������ԣ��򷵻� true�����򷵻� false��</returns>
		protected Boolean Contains(EntityProperty property)
		{
			#region ǰ������

			Debug.Assert((Type == property.Schema.Composite.Target.Type), "���Ե�Ŀ��ʵ��������ѡ������Ŀ��ʵ�����Ͳ�ͬ��");

			#endregion

			return Name.StartsWith(property.PropertyChain.FullName, CommonPolicies.PropertyNameComparison);
		}

		/// <summary>
		/// �ж��Ƿ�ӵ�����ԡ�
		/// </summary>
		/// <param name="property">���ԡ�</param>
		/// <returns>���ӵ�и����ԣ��򷵻� true�����򷵻� false��</returns>
		protected Boolean OwnProperty(EntityProperty property)
		{
			#region ǰ������

			Debug.Assert((Type == property.Schema.Composite.Target.Type), "���Ե�Ŀ��ʵ��������ѡ������Ŀ��ʵ�����Ͳ�ͬ��");

			#endregion

			return Name.Equals(property.Schema.PropertyPath, CommonPolicies.PropertyNameComparison);
		}

		#endregion
	}
}