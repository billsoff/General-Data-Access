#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����FilterExpression.cs
// �ļ��������������������ʽ�����ںϳɹ�������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110325
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���������ʽ�����ںϳɹ�������
	/// </summary>
	[Serializable]
	public class FilterExpression
	{
		#region ˽���ֶ�

		private readonly FilterBuilder m_builder = new FilterBuilder();
		private FilterFactory m_current;

		private List<String> m_propertyPath;

		private FilterInfoExpression m_filterInfoExpression;

		private readonly List<ExpressionTokens> m_appendedTokens = new List<ExpressionTokens>();

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		public FilterExpression()
		{
		}

		/// <summary>
		/// ���캯�������ó�ʼ��������
		/// </summary>
		/// <param name="initialFilter">��ʼ��������</param>
		public FilterExpression(Filter initialFilter)
		{
			m_current = new FilterFactory(initialFilter);
		}

		#endregion

		#region ��������

		/// <summary>
		/// �жϵ�ǰ���ʽ�Ƿ���Խ�����
		/// </summary>
		public Boolean IsResolvable
		{
			get
			{
				ExpressionTokens nextValidTokens = GetNextValidTokens();

				return ((nextValidTokens & ExpressionTokens.Finish) != 0);
			}
		}

		#region ������

		/// <summary>
		/// ���ǡ���������
		/// </summary>
		public FilterExpression Not
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Not;

				OnProcessing(token);

				if (!IsBuildingFilter)
				{
					m_builder.Push(new NotOperator(FilterFactories));
				}
				else
				{
					m_filterInfoExpression = FilterInfoExpression.Not;
				}

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// �����ֱ����һ�� With �� EndWith ��ֱ��ĩβ�Ĳ�����Ϊһ��������Ԫ���൱���������ʽ�е�Բ���ţ���
		/// </summary>
		public FilterExpression With
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.With;

				OnProcessing(token);

				m_builder.Push(new WithOperator(FilterFactories));

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// �������һ�� With ��Ԫ��
		/// </summary>
		public FilterExpression EndWith
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.EndWith;

				OnProcessing(token);

				m_builder.EndWith();

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// ���롱��������
		/// </summary>
		public FilterExpression And
		{
			get
			{
				if (CurrentToken == ExpressionTokens.Nothing)
				{
					Filter(Current.Filter);
				}

				ExpressionTokens token = ExpressionTokens.And;

				OnProcessing(token);

				m_builder.Push(new AndOperator(FilterFactories));

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// ���򡱲�������
		/// </summary>
		public FilterExpression Or
		{
			get
			{
				if (CurrentToken == ExpressionTokens.Nothing)
				{
					Filter(Current.Filter);
				}

				ExpressionTokens token = ExpressionTokens.Or;

				OnProcessing(token);

				m_builder.Push(new OrOperator(FilterFactories));

				OnProcessed(token);

				return this;
			}
		}

		/// <summary>
		/// ��ʼ���չ���������
		/// </summary>
		public FilterExpression Is
		{
			get
			{
				ExpressionTokens token = ExpressionTokens.Is;

				OnProcessing(token);

				OnProcessed(token);

				return this;
			}
		}

		#region FilterInfo

		/// <summary>
		/// IS NULL ����������
		/// </summary>
		public FilterExpression Null
		{
			get
			{
				OnFilterInfoProcessing();

				FilterInfo info = m_filterInfoExpression.Null;

				PushNewFilterFactoryOperator(info);

				OnFilterInfoProcessed();

				return this;
			}
		}

		/// <summary>
		/// ��Ϊ NULL ����ַ����Ĺ���������
		/// </summary>
		public FilterExpression NullOrEmpty
		{
			get
			{
				OnFilterInfoProcessing();

				FilterInfo info = m_filterInfoExpression.NullOrEmpty;

				PushNewFilterFactoryOperator(info);

				OnFilterInfoProcessed();

				return this;
			}
		}

		#endregion

		#endregion

		#region FilterInfo

		/// <summary>
		/// True ��������
		/// </summary>
		public FilterExpression True
		{
			get { return EqualTo(true); }
		}

		/// <summary>
		/// False ��������
		/// </summary>
		public FilterExpression False
		{
			get { return EqualTo(false); }
		}

		/// <summary>
		/// Empty �����������ڿ��ַ�������
		/// </summary>
		public FilterExpression Empty
		{
			get { return EqualTo(String.Empty); }
		}

		#endregion

		#endregion

		#region ��������

		#region ���� FilterFactory

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="f">��������</param>
		/// <returns>���������ʽ��</returns>
		public FilterExpression Filter(Filter f)
		{
			FilterFactory factory = new FilterFactory(f);

			return Filter(factory);
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="filterInfo">��������Ϣ��</param>
		/// <returns>���������ʽ��</returns>
		public FilterExpression Filter(String propertyName, FilterInfo filterInfo)
		{
			return Filter(null, propertyName, filterInfo);
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="entityPropertyName">�ⲿʵ���������ơ�</param>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="filterInfo">��������Ϣ��</param>
		/// <returns>���������ʽ��</returns>
		public FilterExpression Filter(String entityPropertyName, String propertyName, FilterInfo filterInfo)
		{
			ColumnLocator colLocator = new ColumnLocator(entityPropertyName, propertyName);

			FilterFactory factory = new FilterFactory(colLocator.PropertyPath, filterInfo);

			return Filter(factory);
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="chain">��������</param>
		/// <param name="filterInfo">��������Ϣ��</param>
		/// <returns>���������ʽ��</returns>
		public FilterExpression Filter(IPropertyChain chain, FilterInfo filterInfo)
		{
			return Filter(chain.PropertyPath, filterInfo);
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="filterInfo">��������Ϣ��</param>
		/// <returns>���������ʽ��</returns>
		public FilterExpression Filter(IList<String> propertyPath, FilterInfo filterInfo)
		{
			FilterFactory factory = new FilterFactory(propertyPath, filterInfo);

			return Filter(factory);
		}

		#endregion

		#region ���ɹ�����

		/// <summary>
		/// �ⲿ�����������ơ�
		/// </summary>
		/// <param name="entityPropertyName">�ⲿ�����������ơ�</param>
		/// <returns>��ǰʵ����</returns>
		public FilterExpression ForeignRef(String entityPropertyName)
		{
			ExpressionTokens token = ExpressionTokens.ForeignRef;

			OnProcessing(token);

			PropertyPath.Add(entityPropertyName);
			m_filterInfoExpression = FilterInfoExpression.Yes;

			OnProcessed(token);

			return this;
		}

		/// <summary>
		/// �������ơ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>��ǰʵ������</returns>
		public FilterExpression Property(String propertyName)
		{
			ExpressionTokens token = ExpressionTokens.Property;

			OnProcessing(token);

			PropertyPath.Add(propertyName);
			m_filterInfoExpression = FilterInfoExpression.Yes;

			OnProcessed(token);

			return this;
		}

		/// <summary>
		/// ������������������
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>��ǰʵ����</returns>
		public FilterExpression Property(IPropertyChainBuilder builder)
		{
			return Locator(builder);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>��ǰʵ����</returns>
		public FilterExpression Property(IPropertyChain chain)
		{
			return Locator(chain);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>��ǰʵ����</returns>
		public FilterExpression Property(IList<String> propertyPath)
		{
			return Locator(propertyPath);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>��ǰʵ����</returns>
		public FilterExpression Locator(IPropertyChainBuilder builder)
		{
			return Locator(builder.Build());
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>��ǰʵ����</returns>
		public FilterExpression Locator(IPropertyChain chain)
		{
			return Locator(chain.PropertyPath);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>��ǰʵ����</returns>
		public FilterExpression Locator(IList<String> propertyPath)
		{
			ExpressionTokens token = ExpressionTokens.Locator;

			OnProcessing(token);

			PropertyPath.AddRange(propertyPath);
			m_filterInfoExpression = FilterInfoExpression.Yes;

			OnProcessed(token);

			return this;
		}

		#region �������Ե�һЩ��ݷ�ʽ

		/// <summary>
		/// ��Id�����ԡ�
		/// </summary>
		public FilterExpression Id
		{
			get { return Property(WellKnown.Id); }
		}

		/// <summary>
		/// ��Name�����ԡ�
		/// </summary>
		public FilterExpression Name
		{
			get { return Property(WellKnown.Name); }
		}

		/// <summary>
		/// ��FullName�����ԡ�
		/// </summary>
		public FilterExpression FullName
		{
			get { return Property(WellKnown.FullName); }
		}

		/// <summary>
		/// ��DisplayName�����ԡ�
		/// </summary>
		public FilterExpression DisplayName
		{
			get { return Property(WellKnown.DisplayName); }
		}

		/// <summary>
		/// ��DisplayOrder�����ԡ�
		/// </summary>
		public FilterExpression DisplayOrder
		{
			get { return Property(WellKnown.DisplayOrder); }
		}

		/// <summary>
		/// ��Description�����ԡ�
		/// </summary>
		public FilterExpression Description
		{
			get { return Property(WellKnown.Description); }
		}

		/// <summary>
		/// ��Active�����ԡ�
		/// </summary>
		public FilterExpression Active
		{
			get { return Property(WellKnown.Active); }
		}

		/// <summary>
		/// ��Deactive�����ԡ�
		/// </summary>
		public FilterExpression Deactive
		{
			get { return Property(WellKnown.Deactive); }
		}

		/// <summary>
		/// ��Female�����ԡ�
		/// </summary>
		public FilterExpression Female
		{
			get { return Property(WellKnown.Female); }
		}

		/// <summary>
		/// ��Male�����ԡ�
		/// </summary>
		public FilterExpression Male
		{
			get { return Property(WellKnown.Male); }
		}

		/// <summary>
		/// ��NavigateUrl�����ԡ�
		/// </summary>
		public FilterExpression NavigateUrl
		{
			get { return Property(WellKnown.NavigateUrl); }
		}

		/// <summary>
		/// ��TimeCreated�����ԡ�
		/// </summary>
		public FilterExpression TimeCreated
		{
			get { return Property(WellKnown.TimeCreated); }
		}

		/// <summary>
		/// ��TimeModified�����ԡ�
		/// </summary>
		public FilterExpression TimeModified
		{
			get { return Property(WellKnown.TimeModified); }
		}

		/// <summary>
		/// �ⲿ���á�Category�����ԡ�
		/// </summary>
		public FilterExpression Category
		{
			get { return ForeignRef(WellKnown.Category); }
		}

		/// <summary>
		/// �ⲿ���á�Organization�����ԡ�
		/// </summary>
		public FilterExpression Organization
		{
			get { return ForeignRef(WellKnown.Organization); }
		}

		/// <summary>
		/// �ⲿ���á�Owner�����ԡ�
		/// </summary>
		public FilterExpression Owner
		{
			get { return ForeignRef(WellKnown.Owner); }
		}

		/// <summary>
		/// ��Parent�����ԡ�
		/// </summary>
		public FilterExpression Parent
		{
			get
			{
				if (!IsBuildingFilter)
				{
					return ForeignRef(WellKnown.Parent);
				}
				else
				{
					return Property(WellKnown.Parent);
				}
			}
		}

		/// <summary>
		/// �ⲿ���á�Region�����ԡ�
		/// </summary>
		public FilterExpression Region
		{
			get { return ForeignRef(WellKnown.Region); }
		}

		/// <summary>
		/// �ⲿ���á�Role�����ԡ�
		/// </summary>
		public FilterExpression Role
		{
			get { return ForeignRef(WellKnown.Role); }
		}

		/// <summary>
		/// �ⲿ���á�User�����ԡ�
		/// </summary>
		public FilterExpression User
		{
			get { return ForeignRef(WellKnown.User); }
		}

		#endregion

		#endregion

		#region FilterInfo

		/// <summary>
		/// ��ȹ���������
		/// </summary>
		/// <param name="propertyValue">ֵ��</param>
		/// <returns>���������ʽ��</returns>
		public FilterExpression EqualTo(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.EqualTo(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// ���ڹ�������
		/// </summary>
		/// <param name="propertyValue">����ֵ��</param>
		/// <returns>���ڹ�������</returns>
		public FilterExpression GreaterThan(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.GreaterThan(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#region GreaterThanOrEqualTo

		/// <summary>
		/// ���ڻ���ڹ�������
		/// </summary>
		/// <param name="propertyValue">Ҫ�Ƚϵ�ֵ��</param>
		/// <returns>��������Ϣ��</returns>
		public FilterExpression GreaterThanOrEqualTo(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.GreaterThanOrEqualTo(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// ���ڻ���ڹ��������� GreaterThanOrEqualTo ��ͬ��
		/// </summary>
		/// <param name="propertyValue">Ҫ�Ƚϵ�ֵ��</param>
		/// <returns>��������Ϣ��</returns>
		public FilterExpression AtLeast(Object propertyValue)
		{
			return GreaterThanOrEqualTo(propertyValue);
		}

		#endregion

		#region LessThan

		/// <summary>
		/// С�ڹ�������
		/// </summary>
		/// <param name="propertyValue">Ҫ�Ƚϵ�ֵ��</param>
		/// <returns>С�ڹ�������</returns>
		public FilterExpression LessThan(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.LessThan(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#endregion

		#region LessThanOrEqualTo

		/// <summary>
		/// С�ڻ���ڹ�������
		/// </summary>
		/// <param name="propertyValue">Ҫ�Ƚϵ�ֵ��</param>
		/// <returns>С�ڻ���ڹ�������</returns>
		public FilterExpression LessThanOrEqualTo(Object propertyValue)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.LessThanOrEqualTo(propertyValue);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// С�ڻ���ڹ��������� LessThanOrEqualTo ��ͬ��
		/// </summary>
		/// <param name="propertyValue">Ҫ�Ƚϵ�ֵ��</param>
		/// <returns>С�ڻ���ڹ�������</returns>
		public FilterExpression AtMost(Object propertyValue)
		{
			return LessThanOrEqualTo(propertyValue);
		}

		#endregion

		#region Between

		/// <summary>
		/// BETWEEN ��������
		/// </summary>
		/// <param name="from">��ʼ�㡣</param>
		/// <param name="to">�ս�㡣</param>
		/// <returns>BETWEEN ��������</returns>
		public FilterExpression Between(Object from, Object to)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.Between(from, to);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#endregion

		#region Like

		/// <summary>
		/// LIKE ��������
		/// </summary>
		/// <param name="patternText">ģʽ�ı���</param>
		/// <returns>LIKE ��������</returns>
		public FilterExpression Like(String patternText)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.Like(patternText);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// LIKE ��������
		/// </summary>
		/// <param name="patternText">ģʽ�ı���</param>
		/// <returns>LIKE ��������</returns>
		/// <param name="escapeChar">ת���ַ���</param>
		public FilterExpression Like(String patternText, Char escapeChar)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.Like(patternText, escapeChar);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// �����������ı���
		/// </summary>
		/// <param name="text">Ҫ�������ı���</param>
		/// <returns>��ǰ FilterExpression ʵ����</returns>
		public FilterExpression Containing(String text)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.Containing(text);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// �Ը������ı���ͷ��
		/// </summary>
		/// <param name="text">�������ı���</param>
		/// <returns>��ǰ FilterExpression ʵ����</returns>
		public FilterExpression StartingWith(String text)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.StartingWith(text);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// �Ը������ı���β��
		/// </summary>
		/// <param name="text">�������ı���</param>
		/// <returns>��ǰ FilterExpression ʵ����</returns>
		public FilterExpression EndingWith(String text)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.EndingWith(text);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#endregion

		#region IN

		/// <summary>
		/// IN ��������ʹ��ֵ�б�
		/// </summary>
		/// <param name="discreteValues">ֵ�б�</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(params Object[] discreteValues)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.InValues(discreteValues);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="select">ѡ����ʽ��</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(SelectExpression select)
		{
			return InValues(select, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="select">ѡ����ʽ��</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(SelectExpression select, Boolean distinct)
		{
			return InValues(select, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="select">ѡ����ʽ��</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(SelectExpression select, Filter whereFilter)
		{
			return InValues(select, whereFilter, (Filter)null, false);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="select">ѡ����ʽ��</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(SelectExpression select, Filter whereFilter, Boolean distinct)
		{
			return InValues(select, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="select">ѡ����ʽ��</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(SelectExpression select, Filter whereFilter, Filter havingFilter)
		{
			return InValues(select, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="select">ѡ����ʽ��</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(SelectExpression select, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			#region ǰ�ö���

			Debug.Assert(
					(select != null)
					&& select.HasSelectors
					&& (select.Selectors.Length == 1)
					&& (select.Selector.SelectMode == PropertySelectMode.Property),
					"ѡ����ʽ���� select ���벻��Ϊ�գ����ҽ���һ��ѡ��������ѡ��ģʽΪ Property��"
				);

			#endregion

			return InValues(select.Selector.PropertyChain, whereFilter, havingFilter, distinct);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(IPropertyChain property)
		{
			return InValues(property, (Filter)null, (Filter)null, false);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(IPropertyChain property, Boolean distinct)
		{
			return InValues(property, (Filter)null, (Filter)null, distinct);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(IPropertyChain property, Filter whereFilter)
		{
			return InValues(property, whereFilter, (Filter)null, false);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(IPropertyChain property, Filter whereFilter, Boolean distinct)
		{
			return InValues(property, whereFilter, (Filter)null, distinct);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(IPropertyChain property, Filter whereFilter, Filter havingFilter)
		{
			return InValues(property, whereFilter, havingFilter, false);
		}

		/// <summary>
		/// IN ��������ʹ���Ӳ�ѯ�б�
		/// </summary>
		/// <param name="property">Ҫѡ������ԣ�����ӳ��Ϊ�����С�</param>
		/// <param name="whereFilter">WHERE ��������</param>
		/// <param name="havingFilter">HAVING ��������</param>
		/// <param name="distinct">ָʾ SQL ָ�����Ƿ���� DISTINCT �ؼ��֡�</param>
		/// <returns>IN ��������</returns>
		public FilterExpression InValues(IPropertyChain property, Filter whereFilter, Filter havingFilter, Boolean distinct)
		{
			OnFilterInfoProcessing();

			FilterInfo info = m_filterInfoExpression.InValues(property, whereFilter, havingFilter, distinct);

			PushNewFilterFactoryOperator(info);

			OnFilterInfoProcessed();

			return this;
		}

		#endregion

		#endregion

		/// <summary>
		/// ���ã��Ա�����ʹ�á�
		/// </summary>
		public void Reset()
		{
			m_builder.Reset();
			m_current = null;

			if (m_propertyPath != null)
			{
				m_propertyPath.Clear();
			}

			m_appendedTokens.Clear();
		}

		/// <summary>
		/// �������ʽ����ȡ��������
		/// </summary>
		/// <returns>��������</returns>
		public Filter Resolve()
		{
			if (!IsResolvable)
			{
				throw new InvalidOperationException("���ʽ����ȫ���޷�������");
			}

			if (CurrentToken == ExpressionTokens.Start)
			{
				return Current.Filter;
			}

			m_current = m_builder.Resolve();

			return Current.Filter;
		}

		#endregion

		#region ǿ������ת��

		/// <summary>
		/// ��ʽǿ������ת���������������ʽת��Ϊ�������������� Resolve ������
		/// </summary>
		/// <param name="expression">���������ʽ��</param>
		/// <returns>��������</returns>
		public static implicit operator Filter(FilterExpression expression)
		{
			return expression.Resolve();
		}

		#endregion

		#region ˽������

		/// <summary>
		/// ��ȡ��ǰ�Ĺ�����������
		/// </summary>
		private FilterFactory Current
		{
			get
			{
				if (m_current == null)
				{
					m_current = FilterFactory.CreateDefault();
				}

				return m_current;
			}
		}

		/// <summary>
		/// ��ȡ��ǰ�Ĵʷ�Ԫ�ء�
		/// </summary>
		private ExpressionTokens CurrentToken
		{
			get
			{
				if (m_appendedTokens.Count != 0)
				{
					return m_appendedTokens[m_appendedTokens.Count - 1];
				}
				else
				{
					return ExpressionTokens.Nothing;
				}
			}
		}

		/// <summary>
		/// ��ȡ����������ջ��
		/// </summary>
		private IFilterFactoryOperands FilterFactories
		{
			get { return m_builder.FilterFactories; }
		}

		/// <summary>
		/// ��ȡһ��ֵ��ָʾ�Ƿ��� With ������ѹջ��
		/// </summary>
		private Boolean HasWithToken
		{
			get { return m_builder.HasWithToken; }
		}

		/// <summary>
		/// �жϵ�ǰ�Ƿ��ڹ����������Ľ׶Ρ�
		/// </summary>
		private Boolean IsBuildingFilter
		{
			get
			{
				if (CurrentToken == ExpressionTokens.Start)
				{
					return false;
				}

				ExpressionTokens buildingTokens = (ExpressionTokens.ForeignRef | ExpressionTokens.Property | ExpressionTokens.Is);

				if (CurrentToken != ExpressionTokens.Not)
				{
					return ((buildingTokens & CurrentToken) != 0);
				}
				else
				{
					return ((buildingTokens & PreviousToken) != 0);
				}
			}
		}

		/// <summary>
		/// ��ȡǰһ���ʷ�Ԫ�ء�
		/// </summary>
		private ExpressionTokens PreviousToken
		{
			get
			{
				if (m_appendedTokens.Count > 1)
				{
					return m_appendedTokens[m_appendedTokens.Count - 2];
				}
				else
				{
					return ExpressionTokens.Nothing;
				}
			}
		}

		/// <summary>
		/// ��ȡ����·����
		/// </summary>
		public List<String> PropertyPath
		{
			get
			{
				if (m_propertyPath == null)
				{
					m_propertyPath = new List<String>();
				}

				return m_propertyPath;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ѹջ������������
		/// </summary>
		/// <param name="factory">Ҫѹջ�Ĺ�����������</param>
		/// <returns>��ǰʵ����</returns>
		private FilterExpression Filter(FilterFactory factory)
		{
			ExpressionTokens token = ExpressionTokens.Filter;

			OnProcessing(token);

			m_builder.Push(factory);

			OnProcessed(token);

			return this;
		}

		/// <summary>
		/// ��ʵʩ��������ǰ���á�
		/// </summary>
		private void OnFilterInfoProcessing()
		{
			OnProcessing(ExpressionTokens.FilterInfo);
		}

		/// <summary>
		/// ��ʵʩ������������á�
		/// </summary>
		private void OnFilterInfoProcessed()
		{
			OnProcessed(ExpressionTokens.FilterInfo);
		}

		/// <summary>
		/// �ڴʷ�Ԫ�ظ���ǰ���á�
		/// </summary>
		/// <param name="tokenAppending">��Ҫ���ӵĴʷ�Ԫ�ء�</param>
		private void OnProcessing(ExpressionTokens tokenAppending)
		{
			SyntaxCheck(tokenAppending);
		}

		/// <summary>
		/// �ڴʷ�Ԫ�ظ��Ӻ���á�
		/// </summary>
		/// <param name="tokenAppended">�ѱ����ӵĴʷ�Ԫ�ء�</param>
		private void OnProcessed(ExpressionTokens tokenAppended)
		{
			m_appendedTokens.Add(tokenAppended);
		}

		/// <summary>
		/// ѹջ����������������������
		/// </summary>
		/// <param name="info">��������Ϣ��</param>
		private void PushNewFilterFactoryOperator(FilterInfo info)
		{
			ColumnLocator colLocator = new ColumnLocator(PropertyPath.ToArray());

			NewFilterFactoryOperator op = new NewFilterFactoryOperator(colLocator, info);

			m_builder.Push(op);

			// �����ж�λ��Ϣ
			PropertyPath.Clear();
		}

		#region �﷨���

		/// <summary>
		/// ���Ҫ���ӵĴʷ�Ԫ���Ƿ�Ϸ���
		/// </summary>
		/// <param name="tokenAppending"></param>
		[Conditional("DEBUG")]
		private void SyntaxCheck(ExpressionTokens tokenAppending)
		{
			ExpressionTokens validTokens = GetNextValidTokens();

			if ((validTokens & tokenAppending) == tokenAppending)
			{
				return;
			}

			// ���������ټ�
			List<String> currentTokens = m_appendedTokens.ConvertAll<String>(
					delegate(ExpressionTokens token)
					{
						return token.ToString();
					}
				);

			String traceString = String.Join(".", currentTokens.ToArray());

			traceString += String.Format(".[{0}]", tokenAppending.ToString());

			if (tokenAppending == ExpressionTokens.EndWith)
			{
				throw new InvalidOperationException(String.Format("û���� EndWith ��ƥ��� With �������������ټ�Ϊ��{0}", traceString));
			}

			// �ӺϷ��ı����ȥ�� Finish
			validTokens &= ~ExpressionTokens.Finish;

			if (!m_appendedTokens.Contains(ExpressionTokens.FilterInfo) && ((validTokens & ExpressionTokens.FilterInfo) == 0))
			{
				throw new InvalidOperationException(String.Format("�˴�����ʹ�� {0}����ʹ�õ�Ԫ��Ϊ {1}�������ټ�Ϊ��{2}", tokenAppending.ToString(), validTokens.ToString(), traceString));
			}
			else
			{
				throw new InvalidOperationException(String.Format("�˴�����ʹ�� {0}����ʹ�õ�Ԫ��Ϊ {1}�������ټ�Ϊ��{2}��ע��FilterInfo ָ���� Null��True��EqualTo �ȹ���������", tokenAppending.ToString(), validTokens.ToString(), traceString));
			}
		}

		/// <summary>
		/// �����һ���Ϸ��Ĵʷ�Ԫ�ؼ��ϡ�
		/// </summary>
		/// <returns>�Ϸ��Ĵʷ�Ԫ�ؼ��ϡ�</returns>
		private ExpressionTokens GetNextValidTokens()
		{
			ExpressionTokens nextTokens = ExpressionTokens.Nothing;

			switch (CurrentToken)
			{
				case ExpressionTokens.Start:
					nextTokens = ExpressionTokens.Filter
						| ExpressionTokens.ForeignRef
						| ExpressionTokens.Property
						| ExpressionTokens.Locator
						| ExpressionTokens.Not
						| ExpressionTokens.With
						| ExpressionTokens.Finish;
					break;

				case ExpressionTokens.Filter:
				case ExpressionTokens.FilterInfo:
					nextTokens = ExpressionTokens.And
						| ExpressionTokens.Or
						| ExpressionTokens.Finish;

					// ��Ҫ�ж��ܷ��ٸ��� EndWith
					if (HasWithToken)
					{
						nextTokens |= ExpressionTokens.EndWith;
					}

					break;

				case ExpressionTokens.ForeignRef:
					nextTokens = ExpressionTokens.Property
						| ExpressionTokens.Is
						| ExpressionTokens.Not
						| ExpressionTokens.FilterInfo;
					break;

				case ExpressionTokens.Locator:
				case ExpressionTokens.Property:
					nextTokens = ExpressionTokens.Is
						| ExpressionTokens.Not
						| ExpressionTokens.FilterInfo;
					break;

				case ExpressionTokens.Not:
					if (!IsBuildingFilter)
					{
						nextTokens = ExpressionTokens.Filter
							| ExpressionTokens.ForeignRef
							| ExpressionTokens.Property
							| ExpressionTokens.Locator
							| ExpressionTokens.With;
					}
					else
					{
						nextTokens = ExpressionTokens.FilterInfo;
					}

					break;

				case ExpressionTokens.With:
					nextTokens = ExpressionTokens.Filter
						| ExpressionTokens.ForeignRef
						| ExpressionTokens.Property
						| ExpressionTokens.Locator
						| ExpressionTokens.Not
						| ExpressionTokens.With;
					break;

				case ExpressionTokens.And:
				case ExpressionTokens.Or:
					nextTokens = ExpressionTokens.Filter
						| ExpressionTokens.ForeignRef
						| ExpressionTokens.Property
						| ExpressionTokens.Locator
						| ExpressionTokens.With;
					break;

				case ExpressionTokens.EndWith:
					nextTokens = ExpressionTokens.And
						| ExpressionTokens.Or
						| ExpressionTokens.Finish;

					if (HasWithToken)
					{
						nextTokens |= ExpressionTokens.EndWith;
					}

					break;

				case ExpressionTokens.Is:
					nextTokens = ExpressionTokens.Not | ExpressionTokens.FilterInfo;
					break;

				case ExpressionTokens.Finish:
				default:
					break;
			}

			return nextTokens;
		}

		#endregion

		#endregion
	}
}