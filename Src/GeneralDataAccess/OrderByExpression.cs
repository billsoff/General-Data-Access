#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����OrderByExpression.cs
// �ļ��������������ڹ�����������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110328
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
	/// ���ڹ�����������
	/// </summary>
	[Serializable]
	public sealed class OrderByExpression
	{
		#region ˽���ֶ�

		private readonly List<SortExpression> m_sortExpressions = new List<SortExpression>();

		private readonly List<String> PropertyPath = new List<String>();
		private SortMethod m_sortMethod = SortMethod.Ascending;

		private ExpressionTokens m_currentToken = ExpressionTokens.Nothing;

		#endregion

		#region ����

		/// <summary>
		/// �ⲿ�����������ơ�
		/// </summary>
		/// <param name="entityPropertyName">�ⲿ�����������ơ�</param>
		/// <returns>��ǰʵ����</returns>
		public OrderByExpression ForeignRef(String entityPropertyName)
		{
			OrderByExpression expression = TryFlush();

			ExpressionTokens token = ExpressionTokens.ForeignRef;

			OnProcessing(token);

			PropertyPath.Add(entityPropertyName);
			m_sortMethod = SortMethod.Ascending;

			OnProcessed(token);

			return expression;
		}

		/// <summary>
		/// ֵ�������ơ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>��ǰʵ����</returns>
		public OrderByExpression Property(String propertyName)
		{
			OrderByExpression expression = TryFlush();

			ExpressionTokens token = ExpressionTokens.Property;

			OnProcessing(token);

			PropertyPath.Add(propertyName);
			m_sortMethod = SortMethod.Ascending;

			OnProcessed(token);

			return expression;
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>��ǰʵ����</returns>
		public OrderByExpression Property(IPropertyChainBuilder builder)
		{
			return Locator(builder);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>��ǰʵ����</returns>
		public OrderByExpression Property(IPropertyChain chain)
		{
			return Locator(chain);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>��ǰʵ����</returns>
		public OrderByExpression Property(IList<String> propertyPath)
		{
			return Locator(propertyPath);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>��ǰʵ����</returns>
		public OrderByExpression Locator(IPropertyChainBuilder builder)
		{
			return Locator(builder.Build());
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>��ǰʵ����</returns>
		public OrderByExpression Locator(IPropertyChain chain)
		{
			return Locator(chain.PropertyPath);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>��ǰʵ����</returns>
		public OrderByExpression Locator(IList<String> propertyPath)
		{
			OrderByExpression expression = TryFlush();

			ExpressionTokens token = ExpressionTokens.Locator;

			OnProcessing(token);

			this.PropertyPath.AddRange(propertyPath);
			m_sortMethod = SortMethod.Ascending;

			OnProcessed(token);

			return expression;
		}

		/// <summary>
		/// ������������Ĭ��ֵ��
		/// </summary>
		public OrderByExpression Ascending
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Ascending;

				OnProcessing(token);

				m_sortMethod = SortMethod.Ascending;

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		public OrderByExpression Descending
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Descending;

				OnProcessing(token);

				m_sortMethod = SortMethod.Descending;

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// ��һ��Ҫ��������ԡ�
		/// </summary>
		public OrderByExpression Then
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Then;

				OnProcessing(token);

				MakeSortExpression();

				OnProcessed(token);

				return this;
			}
		}

		#region �������Ե�һЩ��ݷ�ʽ

		/// <summary>
		/// ��Id�����ԡ�
		/// </summary>
		public OrderByExpression Id
		{
			get { return Property(WellKnown.Id); }
		}

		/// <summary>
		/// ��Name�����ԡ�
		/// </summary>
		public OrderByExpression Name
		{
			get { return Property(WellKnown.Name); }
		}

		/// <summary>
		/// ��FullName�����ԡ�
		/// </summary>
		public OrderByExpression FullName
		{
			get { return Property(WellKnown.FullName); }
		}

		/// <summary>
		/// ��DisplayName�����ԡ�
		/// </summary>
		public OrderByExpression DisplayName
		{
			get { return Property(WellKnown.DisplayName); }
		}

		/// <summary>
		/// ��DisplayOrder�����ԡ�
		/// </summary>
		public OrderByExpression DisplayOrder
		{
			get { return Property(WellKnown.DisplayOrder); }
		}

		/// <summary>
		/// ��Description�����ԡ�
		/// </summary>
		public OrderByExpression Description
		{
			get { return Property(WellKnown.Description); }
		}

		/// <summary>
		/// ��Active�����ԡ�
		/// </summary>
		public OrderByExpression Active
		{
			get { return Property(WellKnown.Active); }
		}

		/// <summary>
		/// ��Deactive�����ԡ�
		/// </summary>
		public OrderByExpression Deactive
		{
			get { return Property(WellKnown.Deactive); }
		}

		/// <summary>
		/// ��Female�����ԡ�
		/// </summary>
		public OrderByExpression Female
		{
			get { return Property(WellKnown.Female); }
		}

		/// <summary>
		/// ��Male�����ԡ�
		/// </summary>
		public OrderByExpression Male
		{
			get { return Property(WellKnown.Male); }
		}

		/// <summary>
		/// ��NavigateUrl�����ԡ�
		/// </summary>
		public OrderByExpression NavigateUrl
		{
			get { return Property(WellKnown.NavigateUrl); }
		}

		/// <summary>
		/// ��TimeCreated�����ԡ�
		/// </summary>
		public OrderByExpression TimeCreated
		{
			get { return Property(WellKnown.TimeCreated); }
		}

		/// <summary>
		/// ��TimeModified�����ԡ�
		/// </summary>
		public OrderByExpression TimeModified
		{
			get { return Property(WellKnown.TimeModified); }
		}

		/// <summary>
		/// �ⲿ���á�Category�����ԡ�
		/// </summary>
		public OrderByExpression Category
		{
			get { return ForeignRef(WellKnown.Category); }
		}

		/// <summary>
		/// �ⲿ���á�Organization�����ԡ�
		/// </summary>
		public OrderByExpression Organization
		{
			get { return ForeignRef(WellKnown.Organization); }
		}

		/// <summary>
		/// �ⲿ���á�Owner�����ԡ�
		/// </summary>
		public OrderByExpression Owner
		{
			get { return ForeignRef(WellKnown.Owner); }
		}

		/// <summary>
		/// �ⲿ���á�Parent�����ԡ�
		/// </summary>
		public OrderByExpression Parent
		{
			get { return ForeignRef(WellKnown.Parent); }
		}

		/// <summary>
		/// �ⲿ���á�Region�����ԡ�
		/// </summary>
		public OrderByExpression Region
		{
			get { return ForeignRef(WellKnown.Region); }
		}

		/// <summary>
		/// �ⲿ���á�Role�����ԡ�
		/// </summary>
		public OrderByExpression Role
		{
			get { return ForeignRef(WellKnown.Role); }
		}

		/// <summary>
		/// �ⲿ���á�User�����ԡ�
		/// </summary>
		public OrderByExpression User
		{
			get { return ForeignRef(WellKnown.User); }
		}

		#endregion

		/// <summary>
		/// ���ã��Ա�����ʹ�á�
		/// </summary>
		public void Reset()
		{
			m_currentToken = ExpressionTokens.Nothing;

			PropertyPath.Clear();
			m_sortMethod = SortMethod.Ascending;

			m_sortExpressions.Clear();
		}

		#endregion

		#region ����

		/// <summary>
		/// �����������������
		/// </summary>
		/// <returns>���ɵ���������</returns>
		public Sorter Resolve()
		{
			ExpressionTokens nextValidTokens = GetNextValidTokens();

			if ((nextValidTokens & ExpressionTokens.Finish) != 0)
			{
				// �������һ��������ʽ
				MakeSortExpression();

				Sorter result = null;

				if (m_sortExpressions.Count != 0)
				{
					result = new Sorter(m_sortExpressions.ToArray());
				}

				return result;
			}
			else
			{
				throw new InvalidOperationException("������ʽ����������ȷ�ĸ�ʽΪ��[ForeignRef(...).]Property(...)[.{Ascending|Descending}][.Then...]");
			}
		}

		#endregion

		#region ����ǿ��

		/// <summary>
		/// �� OrderByExpression �� Sorter ������ǿ��ת����
		/// </summary>
		/// <param name="expression">OrderByExpression ʵ����</param>
		/// <returns>OrderByExpression �����õ��Ľ����</returns>
		public static implicit operator Sorter(OrderByExpression expression)
		{
			return expression.Resolve();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����������ʽ��
		/// </summary>
		private void MakeSortExpression()
		{
			ExpressionTokens availableTokens = ExpressionTokens.Property
					| ExpressionTokens.Locator
					| ExpressionTokens.Ascending
					| ExpressionTokens.Descending;

			if (((m_currentToken & availableTokens) != 0) && (PropertyPath.Count != 0))
			{
				SortExpression expression = new SortExpression(PropertyPath.ToArray(), m_sortMethod);

				m_sortExpressions.Add(expression);

				// ����
				PropertyPath.Clear();
				m_sortMethod = SortMethod.Ascending;
			}
		}

		/// <summary>
		/// ����ʷ�Ԫ��ǰ���á�
		/// </summary>
		/// <param name="tokenAppending"></param>
		private void OnProcessing(ExpressionTokens tokenAppending)
		{
			SyntaxCheck(tokenAppending);
		}

		/// <summary>
		/// �����﷨��顣
		/// </summary>
		/// <param name="tokenAppending"></param>
		[Conditional("DEBUG")]
		private void SyntaxCheck(ExpressionTokens tokenAppending)
		{
			ExpressionTokens validTokens = GetNextValidTokens();

			if ((validTokens & tokenAppending) == 0)
			{
				throw new InvalidOperationException("������ʽ����������ȷ�ĸ�ʽΪ��[ForeignRef(...).]Property(...)[.{Ascending|Descending}][[.Then]...]");
			}
		}

		/// <summary>
		/// ��ɴʷ�Ԫ�ش������á�
		/// </summary>
		/// <param name="tokenAppended">���ӵĴʷ�Ԫ�ء�</param>
		private void OnProcessed(ExpressionTokens tokenAppended)
		{
			m_currentToken = tokenAppended;
		}

		/// <summary>
		/// ��ȡ��һ���Ϸ��Ĵʷ�Ԫ�ء�
		/// </summary>
		/// <returns>��һ���Ϸ��Ĵʷ�Ԫ�ء�</returns>
		private ExpressionTokens GetNextValidTokens()
		{
			ExpressionTokens nextValidToken = ExpressionTokens.Nothing;

			switch (m_currentToken)
			{
				case ExpressionTokens.Start:
					nextValidToken = ExpressionTokens.ForeignRef
						| ExpressionTokens.Property
						| ExpressionTokens.Locator
						| ExpressionTokens.Finish;
					break;

				case ExpressionTokens.ForeignRef:
					nextValidToken = ExpressionTokens.Property;
					break;

				case ExpressionTokens.Locator:
				case ExpressionTokens.Property:
					nextValidToken = ExpressionTokens.Ascending
						| ExpressionTokens.Descending
						| ExpressionTokens.Then
						| ExpressionTokens.Finish;
					break;

				case ExpressionTokens.Ascending:
				case ExpressionTokens.Descending:
					nextValidToken = ExpressionTokens.Then | ExpressionTokens.Finish;
					break;

				case ExpressionTokens.Then:
					nextValidToken = ExpressionTokens.ForeignRef | ExpressionTokens.Property | ExpressionTokens.Locator;
					break;

				default:
					break;
			}

			return nextValidToken;
		}

		/// <summary>
		/// ���Լ��㵱ǰ��������ʽ��
		/// </summary>
		/// <returns>��ǰʵ����</returns>
		private OrderByExpression TryFlush()
		{
			OrderByExpression expression = this;

			ExpressionTokens nextValidTokens = GetNextValidTokens();

			if ((nextValidTokens & ExpressionTokens.Then) != 0)
			{
				expression = Then;
			}

			return expression;
		}

		#endregion
	}
}