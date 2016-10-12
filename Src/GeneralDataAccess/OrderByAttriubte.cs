#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����OrderByAttriubte.cs
// �ļ�����������ָʾ�����ֶΡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110407
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ָʾ�����ֶΡ�
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class OrderByAttribute : Attribute
	{
		#region ��̬��Ա

		/// <summary>
		/// �ϳ���������
		/// </summary>
		/// <param name="children">�����ԡ�</param>
		/// <returns>�ϳɺõ��������������������û�б�� OrderByAttribute���򷵻� null��</returns>
		public static Sorter ComposeSorter(PropertyInfo children)
		{
			OrderByAttribute[] orderByAttrs = (OrderByAttribute[])Attribute.GetCustomAttributes(children, typeof(OrderByAttribute));

			if (orderByAttrs.Length == 0)
			{
				return null;
			}

			Array.Sort<OrderByAttribute>(
					orderByAttrs,
					delegate(OrderByAttribute left, OrderByAttribute right)
					{
						return left.OrderNumber.CompareTo(right.OrderNumber);
					}
				);

			OrderByExpression expression = new OrderByExpression();

			for (Int32 i = 0; i < orderByAttrs.Length; i++)
			{
				OrderByAttribute attr = orderByAttrs[i];

				expression.Property(attr.PropertyPath);

				if (attr.SortMethod == SortMethod.Descending)
				{
					expression = expression.Descending;
				}
			}

			Sorter s = expression.Resolve();

			return s;
		}

		#endregion

		#region ˽���ֶ�

		private readonly String[] m_proertyPath;
		private readonly Int32 m_orderNumber;
		private SortMethod m_sortMethod;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯�����ڲ�ʹ�á�
		/// </summary>
		private OrderByAttribute()
		{
			m_sortMethod = SortMethod.Ascending;
		}

		/// <summary>
		/// ���캯��������Ҫ�����ֵ�������ƺ�����˳��
		/// </summary>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="orderNumber">����˳��</param>
		public OrderByAttribute(String propertyName, Int32 orderNumber)
			: this(new String[] { propertyName }, orderNumber)
		{
		}

		/// <summary>
		/// ���캯��������Ҫ�����ʵ���������ƺ�ֵ�������ơ�
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="orderNumber">����˳��</param>
		public OrderByAttribute(String entityPropertyName, String propertyName, Int32 orderNumber)
			: this(new String[] { entityPropertyName, propertyName }, orderNumber)
		{
		}

		/// <summary>
		/// ���캯������������·��������˳��
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="orderNumber">����˳��</param>
		public OrderByAttribute(String[] propertyPath, Int32 orderNumber)
			: this()
		{
			m_proertyPath = propertyPath;
			m_orderNumber = orderNumber;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����˳��
		/// </summary>
		public Int32 OrderNumber
		{
			get { return m_orderNumber; }
		}

		/// <summary>
		/// ��ȡ����·����
		/// </summary>
		public String[] PropertyPath { get { return m_proertyPath; } }

		/// <summary>
		/// ��ȡ����������ʽ��
		/// </summary>
		public SortMethod SortMethod
		{
			get { return m_sortMethod; }
			set { m_sortMethod = value; }
		}

		#endregion
	}
}