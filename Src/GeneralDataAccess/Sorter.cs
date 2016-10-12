#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����Sorter.cs
// �ļ�����������������������������ʽ���С�
//
//
// ������ʶ���α���billsoff@gmail.com�� 201008151033
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ������������������ʽ���С�
	/// </summary>
	[Serializable]
	public sealed class Sorter
	{
		#region ��̬��Ա

		/// <summary>
		/// �ϳ�������ʽ�����������Ϊ�ջ򲻺���Ч��������ʽ���򷵻� null��
		/// </summary>
		/// <param name="schema">�ܹ�����ͨ���ж�λ������ȡӳ����м��ϡ�</param>
		/// <param name="sorter">��������</param>
		/// <returns>�ϳɺõ�������ʽ��</returns>
		public static String ComposeSortExpression(IColumnLocatable schema, Sorter sorter)
		{
			if ((sorter == null) || (sorter.Expressions.Length == 0))
			{
				return null;
			}

			StringBuilder buffer = new StringBuilder();

			foreach (SortExpression expression in sorter.Expressions)
			{
				ColumnLocator colLocator = expression.ColumnLocator;

				foreach (Column col in schema[colLocator])
				{
					if (buffer.Length != 0)
					{
						buffer.Append(",");
					}

					String columnFullName = col.FullName ?? col.Alias;

					buffer.Append(columnFullName);

					if (expression.SortMethod == SortMethod.Descending)
					{
						buffer.Append(" DESC");
					}
				}
			}

			return buffer.ToString();
		}

		#endregion

		#region ˽���ֶ�

		private readonly List<SortExpression> m_expressions = new List<SortExpression>();

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public Sorter()
		{
		}

		/// <summary>
		/// ���캯����
		/// </summary>
		/// <param name="expressions">Ҫ�����������ʽ��</param>
		public Sorter(params SortExpression[] expressions)
		{
			if (expressions != null)
			{
				m_expressions.AddRange(expressions);
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ���а�����������ʽ��
		/// </summary>
		public SortExpression[] Expressions
		{
			get { return m_expressions.ToArray(); }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����������ʽ������������
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>��ǰ����������</returns>
		public Sorter Append(String propertyName)
		{
			return Append(null, propertyName, SortMethod.Ascending);
		}

		/// <summary>
		/// ����������ʽ��ָ�����򷽷���
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <param name="sortMethod">���򷽷���</param>
		/// <returns>��ǰ��������</returns>
		public Sorter Append(String propertyName, SortMethod sortMethod)
		{
			return Append(null, propertyName, sortMethod);
		}

		/// <summary>
		/// ����������ʽ������������
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <returns>��ǰ��������</returns>
		public Sorter Append(String entityPropertyName, String propertyName)
		{
			return Append(entityPropertyName, propertyName, SortMethod.Ascending);
		}

		/// <summary>
		/// ����������ʽ��ָ�����򷽷���
		/// </summary>
		/// <param name="entityPropertyName">ʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="sortMethod">���򷽷���</param>
		/// <returns></returns>
		public Sorter Append(String entityPropertyName, String propertyName, SortMethod sortMethod)
		{
			ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);
			SortExpression expression = new SortExpression(colLocator, sortMethod);

			m_expressions.Add(expression);

			return this;
		}

		/// <summary>
		/// ��ȡ���е��ж�λ����
		/// </summary>
		/// <returns>�ж�λ�����ϡ�</returns>
		public ColumnLocator[] GetAllColumnLocators()
		{
			ColumnLocator[] allColumnLocators = new ColumnLocator[Expressions.Length];

			for (Int32 i = 0; i < allColumnLocators.Length; i++)
			{
				allColumnLocators[i] = Expressions[i].ColumnLocator;
			}

			return allColumnLocators;
		}

		/// <summary>
		/// ��ȡ���е�����ѡ������ָʾ������ʵ�壩��
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>���е�����ѡ������</returns>
		public PropertySelector[] GetAllSelectors(Type entityType)
		{
			List<IPropertyChain> allForeignReferences = new List<IPropertyChain>();

			foreach (SortExpression expression in Expressions)
			{
				if (expression.ColumnLocator.PropertyPath.Length > 1)
				{
					PropertyChain chain = new PropertyChain(entityType, expression.ColumnLocator.PropertyPath);

					// ǰһ�����Խڵ����������´���������ᵼ������ǰ����
					IPropertyChain reference = chain.Previous.Copy();

					if (!allForeignReferences.Contains(reference))
					{
						allForeignReferences.Add(reference);
					}
				}
			}

			List<PropertySelector> allSelectors = allForeignReferences.ConvertAll<PropertySelector>(
					delegate(IPropertyChain chain)
					{
						return PropertySelector.Create(PropertySelectMode.LoadSchemaOnly, chain);
					}
				);

			return allSelectors.ToArray();
		}

		#endregion
	}
}