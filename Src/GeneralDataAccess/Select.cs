#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����Select.cs
// �ļ���������������ѡ�����Եĸ����ࡣ
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110524
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
	/// ����ѡ�����Եĸ����ࡣ
	/// </summary>
	public static class Select
	{
		#region ��������

		/// <summary>
		/// ָʾ��ѡ���κ����ԡ�
		/// </summary>
		public static CompositeBuilderStrategy Nothing
		{
			get { return CompositeBuilderStrategyFactory.Nothing; }
		}

		/// <summary>
		/// ѡ��Ŀ��ʵ���е��������ԡ�
		/// </summary>
		/// <param name="entityType">Ŀ��ʵ�����͡�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression AllFrom(Type entityType)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllFrom(entityType);

			return expression;
		}

		/// <summary>
		/// ѡ���ⲿ�����б��е��������ԡ�
		/// </summary>
		/// <param name="allChains">�������б����е�Ԫ�ر������ⲿ���á�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression AllFrom(params IPropertyChain[] allChains)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllFrom(allChains);

			return expression;
		}

		/// <summary>
		/// ѡ���ⲿ�����б��е��������ԡ�
		/// </summary>
		/// <param name="allChainBuilders">�������������б����е�Ԫ�ر��������ⲿ������������</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression AllFrom(params IPropertyChainBuilder[] allChainBuilders)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllFrom(allChainBuilders);

			return expression;
		}

		/// <summary>
		/// ѡ��Ŀ��ʵ���е����з��ӳټ������ԡ�
		/// </summary>
		/// <param name="entityType">Ŀ��ʵ�����͡�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression AllExceptLazyLoadFrom(Type entityType)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllExceptLazyLoadFrom(entityType);

			return expression;
		}

		/// <summary>
		/// ѡ���ⲿ�����б��е����з��ӳټ������ԡ�
		/// </summary>
		/// <param name="allChains">�������б����е�Ԫ�ر������ⲿ���á�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression AllExceptLazyLoadFrom(params IPropertyChain[] allChains)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllExceptLazyLoadFrom(allChains);

			return expression;
		}

		/// <summary>
		/// ѡ���ⲿ�����б��е����з��ӳټ������ԡ�
		/// </summary>
		/// <param name="allChainBuilders">�������������б����е�Ԫ�ر������ⲿ���á�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression AllExceptLazyLoadFrom(params IPropertyChainBuilder[] allChainBuilders)
		{
			SelectExpression expression = new SelectExpression();

			expression.AllExceptLazyLoadFrom(allChainBuilders);

			return expression;
		}

		/// <summary>
		/// ѡ��һ�����ԡ�
		/// </summary>
		/// <param name="targetType">Ŀ�����͡�</param>
		/// <param name="propertyPath">����·����</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression Property(Type targetType, String[] propertyPath)
		{
			return Property(new PropertyChain(targetType, propertyPath));
		}

		/// <summary>
		/// ѡ��һ�����ԡ�
		/// </summary>
		/// <param name="builder">��������������</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression Property(IPropertyChainBuilder builder)
		{
			return Property(builder.Build());
		}

		/// <summary>
		/// ѡ��һ�����ԡ�
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression Property(IPropertyChain chain)
		{
			SelectExpression expression = new SelectExpression();

			expression.Property(chain);

			return expression;
		}

		/// <summary>
		/// ѡ�����ԡ�
		/// </summary>
		/// <param name="allChains">�������б����е�Ԫ�ر���Ϊֵ���ԡ�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression Properties(params IPropertyChain[] allChains)
		{
			SelectExpression expression = new SelectExpression();

			expression.Properties(allChains);

			return expression;
		}

		/// <summary>
		/// ѡ�����ԡ�
		/// </summary>
		/// <param name="allChainBuilders">�������������б�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression Properties(params IPropertyChainBuilder[] allChainBuilders)
		{
			SelectExpression expression = new SelectExpression();

			expression.Properties(allChainBuilders);

			return expression;
		}

		/// <summary>
		/// ѡ��Ŀ��ʵ���е��������ԡ�
		/// </summary>
		/// <param name="entityType">Ŀ��ʵ�����ͣ�����Ϊ�ա�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression PrimaryKeyOf(Type entityType)
		{
			SelectExpression expression = new SelectExpression();

			expression.PrimaryKeyOf(entityType);

			return expression;
		}

		/// <summary>
		/// ѡ�������ⲿ�����е��������ԡ�
		/// </summary>
		/// <param name="allChains">�������б����е�Ԫ�ر���ӳ��Ϊ�������ԡ�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression PrimaryKeysOf(params IPropertyChain[] allChains)
		{
			SelectExpression expression = new SelectExpression();

			expression.PrimaryKeysOf(allChains);

			return expression;
		}

		/// <summary>
		/// ѡ�������ⲿ�����е��������ԡ�
		/// </summary>
		/// <param name="allChainBuilders">�������������б����е�Ԫ�������������ԡ�</param>
		/// <returns>ѡ����ʽ��</returns>
		public static SelectExpression PrimaryKeysOf(params IPropertyChainBuilder[] allChainBuilders)
		{
			SelectExpression expression = new SelectExpression();

			expression.PrimaryKeysOf(allChainBuilders);

			return expression;
		}

		#endregion
	}
}