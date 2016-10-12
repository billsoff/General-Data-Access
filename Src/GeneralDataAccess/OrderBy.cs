#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����OrderBy.cs
// �ļ��������������������������ĸ����ࡣ
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���������������ĸ����ࡣ
	/// </summary>
	public abstract class OrderBy
	{
		#region ����

		/// <summary>
		/// �ⲿ�����������ơ�
		/// </summary>
		/// <param name="entityPropertyName">�ⲿ�����������ơ�</param>
		/// <returns>�� OrderByExpression ʵ����</returns>
		public static OrderByExpression ForeignRef(String entityPropertyName)
		{
			OrderByExpression expression = new OrderByExpression();

			expression.ForeignRef(entityPropertyName);

			return expression;
		}

		/// <summary>
		/// ֵ�������ơ�
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>�� OrderByExpression ʵ����</returns>
		public static OrderByExpression Property(String propertyName)
		{
			OrderByExpression expression = new OrderByExpression();

			expression.Property(propertyName);

			return expression;
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>�� OrderByExpression ʵ����</returns>
		public static OrderByExpression Property(IPropertyChainBuilder builder)
		{
			return Locator(builder);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>�� OrderByExpression ʵ����</returns>
		public static OrderByExpression Property(IPropertyChain chain)
		{
			return Locator(chain);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>�� OrderByExpression ʵ����</returns>
		public static OrderByExpression Property(IList<String> propertyPath)
		{
			return Locator(propertyPath);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>�� OrderByExpression ʵ����</returns>
		public static OrderByExpression Locator(IPropertyChainBuilder builder)
		{
			return Locator(builder.Build());
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>�� OrderByExpression ʵ����</returns>
		public static OrderByExpression Locator(IPropertyChain chain)
		{
			return Locator(chain.PropertyPath);
		}

		/// <summary>
		/// ��������·����
		/// </summary>
		/// <param name="propertyPath">����·����</param>
		/// <returns>�� OrderByExpression ʵ����</returns>
		public static OrderByExpression Locator(IList<String> propertyPath)
		{
			OrderByExpression expression = new OrderByExpression();

			expression.Locator(propertyPath);

			return expression;
		}

		#region �������Ե�һЩ��ݷ�ʽ

		/// <summary>
		/// ��Id�����ԡ�
		/// </summary>
		public static OrderByExpression Id
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Id;
			}
		}

		/// <summary>
		/// ��Name�����ԡ�
		/// </summary>
		public static OrderByExpression Name
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Name;
			}
		}

		/// <summary>
		/// ��FullName�����ԡ�
		/// </summary>
		public static OrderByExpression FullName
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.FullName;
			}
		}

		/// <summary>
		/// ��DisplayName�����ԡ�
		/// </summary>
		public static OrderByExpression DisplayName
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.DisplayName;
			}
		}

		/// <summary>
		/// ��DisplayOrder�����ԡ�
		/// </summary>
		public static OrderByExpression DisplayOrder
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.DisplayOrder;
			}
		}

		/// <summary>
		/// ��Description�����ԡ�
		/// </summary>
		public static OrderByExpression Description
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Description;
			}
		}

		/// <summary>
		/// ��Active�����ԡ�
		/// </summary>
		public static OrderByExpression Active
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Active;
			}
		}

		/// <summary>
		/// ��Deactive�����ԡ�
		/// </summary>
		public static OrderByExpression Deactive
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Deactive;
			}
		}

		/// <summary>
		/// ��Female�����ԡ�
		/// </summary>
		public static OrderByExpression Female
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Female;
			}
		}

		/// <summary>
		/// ��Male�����ԡ�
		/// </summary>
		public static OrderByExpression Male
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Male;
			}
		}

		/// <summary>
		/// ��NavigateUrl�����ԡ�
		/// </summary>
		public static OrderByExpression NavigateUrl
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.NavigateUrl;
			}
		}

		/// <summary>
		/// ��TimeCreated�����ԡ�
		/// </summary>
		public static OrderByExpression TimeCreated
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.TimeCreated;
			}
		}

		/// <summary>
		/// ��TimeModified�����ԡ�
		/// </summary>
		public static OrderByExpression TimeModified
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.TimeModified;
			}
		}

		/// <summary>
		/// �ⲿ���á�Category�����ԡ�
		/// </summary>
		public static OrderByExpression Category
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Category;
			}
		}

		/// <summary>
		/// �ⲿ���á�Organization�����ԡ�
		/// </summary>
		public static OrderByExpression Organization
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Organization;
			}
		}

		/// <summary>
		/// �ⲿ���á�Owner�����ԡ�
		/// </summary>
		public static OrderByExpression Owner
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Owner;
			}
		}

		/// <summary>
		/// �ⲿ���á�Parent�����ԡ�
		/// </summary>
		public static OrderByExpression Parent
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Parent;
			}
		}

		/// <summary>
		/// �ⲿ���á�Region�����ԡ�
		/// </summary>
		public static OrderByExpression Region
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Region;
			}
		}

		/// <summary>
		/// �ⲿ���á�Role�����ԡ�
		/// </summary>
		public static OrderByExpression Role
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.Role;
			}
		}

		/// <summary>
		/// �ⲿ���á�User�����ԡ�
		/// </summary>
		public static OrderByExpression User
		{
			get
			{
				OrderByExpression expression = new OrderByExpression();

				return expression.User;
			}
		}

		#endregion

		#endregion
	}
}