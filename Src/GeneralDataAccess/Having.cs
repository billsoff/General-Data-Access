#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����Having.cs
// �ļ��������������ڹ����������ĸ����ࡣ
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110701
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
	/// ���ڹ����������ĸ����ࡣ
	/// </summary>
	public static class Having
	{
		#region ������

		/// <summary>
		/// "��"������
		/// </summary>
		public static FilterExpression Not
		{
			get
			{
				FilterExpression expression = (new FilterExpression()).Not;

				return expression;
			}
		}

		/// <summary>
		/// ���롱������
		/// </summary>
		public static FilterExpression And
		{
			get
			{
				FilterExpression expression = (new FilterExpression()).And;

				return expression;
			}
		}

		/// <summary>
		/// ���򡱲�����
		/// </summary>
		public static FilterExpression Or
		{
			get
			{
				FilterExpression expression = (new FilterExpression()).Or;

				return expression;
			}
		}

		/// <summary>
		/// ��ʼһ�����㵥Ԫ��
		/// </summary>
		public static FilterExpression With
		{
			get
			{
				FilterExpression expression = (new FilterExpression()).With;

				return expression;
			}
		}

		#endregion

		#region ����������


		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="f">��������</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Filter(Filter f)
		{
			FilterExpression expression = new FilterExpression();

			expression.Filter(f);

			return expression;
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="propertyName">ֵ�������ơ�</param>
		/// <param name="filterInfo">��������Ϣ��</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Filter(String propertyName, FilterInfo filterInfo)
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
		public static FilterExpression Filter(String entityPropertyName, String propertyName, FilterInfo filterInfo)
		{
			FilterExpression expression = new FilterExpression();

			expression.Filter(entityPropertyName, propertyName, filterInfo);

			return expression;
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="chain">��������</param>
		/// <param name="filterInfo">��������Ϣ��</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Filter(IPropertyChain chain, FilterInfo filterInfo)
		{
			return Filter(chain.PropertyPath, filterInfo);
		}

		/// <summary>
		/// �����������
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <param name="filterInfo">��������Ϣ��</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Filter(IList<String> propertyPath, FilterInfo filterInfo)
		{
			FilterExpression expression = new FilterExpression();

			expression.Filter(propertyPath, filterInfo);

			return expression;
		}

		#endregion

		#region ���ɹ�����

		/// <summary>
		/// �ⲿ�����������ơ�
		/// </summary>
		/// <param name="entityPropertyName">�ⲿ�����������ơ�</param>
		/// <returns>��ǰʵ����</returns>
		public static FilterExpression ForeignRef(String entityPropertyName)
		{
			FilterExpression expression = new FilterExpression();

			expression.ForeignRef(entityPropertyName);

			return expression;
		}

		/// <summary>
		/// ���ɹ�����������Ϣ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Property(String propertyName)
		{
			FilterExpression expression = new FilterExpression();

			expression.Property(propertyName);

			return expression;
		}

		/// <summary>
		/// ������������������
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Property(IPropertyChainBuilder builder)
		{
			return Locator(builder);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Property(IPropertyChain chain)
		{
			return Locator(chain);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Property(IList<String> propertyPath)
		{
			return Locator(propertyPath);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Locator(IPropertyChainBuilder builder)
		{
			return Locator(builder.Build());
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Locator(IPropertyChain chain)
		{
			return Locator(chain.PropertyPath);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>���������ʽ��</returns>
		public static FilterExpression Locator(IList<String> propertyPath)
		{
			FilterExpression expression = new FilterExpression();

			expression.Locator(propertyPath);

			return expression;
		}

		#region �������Ե�һЩ��ݷ�ʽ

		/// <summary>
		/// ��Id�����ԡ�
		/// </summary>
		public static FilterExpression Id
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Id;
			}
		}

		/// <summary>
		/// ��Name�����ԡ�
		/// </summary>
		public static FilterExpression Name
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Name;
			}
		}

		/// <summary>
		/// ��FullName�����ԡ�
		/// </summary>
		public static FilterExpression FullName
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.FullName;
			}
		}

		/// <summary>
		/// ��DisplayName�����ԡ�
		/// </summary>
		public static FilterExpression DisplayName
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.DisplayName;
			}
		}

		/// <summary>
		/// ��DisplayOrder�����ԡ�
		/// </summary>
		public static FilterExpression DisplayOrder
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.DisplayOrder;
			}
		}

		/// <summary>
		/// ��Description�����ԡ�
		/// </summary>
		public static FilterExpression Description
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Description;
			}
		}

		/// <summary>
		/// ��Active�����ԡ�
		/// </summary>
		public static FilterExpression Active
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Active;
			}
		}

		/// <summary>
		/// ��Deactive�����ԡ�
		/// </summary>
		public static FilterExpression Deactive
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Deactive;
			}
		}

		/// <summary>
		/// ��Female�����ԡ�
		/// </summary>
		public static FilterExpression Female
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Female;
			}
		}

		/// <summary>
		/// ��Male�����ԡ�
		/// </summary>
		public static FilterExpression Male
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Male;
			}
		}

		/// <summary>
		/// ��NavigateUrl�����ԡ�
		/// </summary>
		public static FilterExpression NavigateUrl
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.NavigateUrl;
			}
		}

		/// <summary>
		/// ��TimeCreated�����ԡ�
		/// </summary>
		public static FilterExpression TimeCreated
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.TimeCreated;
			}
		}

		/// <summary>
		/// ��TimeModified�����ԡ�
		/// </summary>
		public static FilterExpression TimeModified
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.TimeModified;
			}
		}

		/// <summary>
		/// �ⲿ���á�Category�����ԡ�
		/// </summary>
		public static FilterExpression Category
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Category;
			}
		}

		/// <summary>
		/// �ⲿ���á�Organization�����ԡ�
		/// </summary>
		public static FilterExpression Organization
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Organization;
			}
		}

		/// <summary>
		/// �ⲿ���á�Owner�����ԡ�
		/// </summary>
		public static FilterExpression Owner
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Owner;
			}
		}

		/// <summary>
		/// �ⲿ���á�Parent�����ԡ�
		/// </summary>
		public static FilterExpression Parent
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Parent;
			}
		}

		/// <summary>
		/// �ⲿ���á�Region�����ԡ�
		/// </summary>
		public static FilterExpression Region
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Region;
			}
		}

		/// <summary>
		/// �ⲿ���á�Role�����ԡ�
		/// </summary>
		public static FilterExpression Role
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.Role;
			}
		}

		/// <summary>
		/// �ⲿ���á�User�����ԡ�
		/// </summary>
		public static FilterExpression User
		{
			get
			{
				FilterExpression expression = new FilterExpression();

				return expression.User;
			}
		}

		#endregion

		#endregion
	}
}