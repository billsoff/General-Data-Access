#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����ExpressionSchemaBuilderFactory.cs
// �ļ������������������ɱ��ʽ�ܹ���������
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110708
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
	/// �������ɱ��ʽ�ܹ���������
	/// </summary>
	internal static class ExpressionSchemaBuilderFactory
	{
		/// <summary>
		/// ����ָ�����͵ı��ʽ�ܹ���������
		/// </summary>
		/// <param name="propertyDef">����ʵ�����Զ��塣</param>
		/// <param name="parameterPrefix">��ѯ����ǰ׺��</param>
		/// <returns>���ʽ�ܹ���������</returns>
		public static ExpressionSchemaBuilder Create(CompositeForeignReferencePropertyDefinition propertyDef, String parameterPrefix)
		{
			ExpressionSchemaType type = GetExpressionSchemaType(propertyDef.Type);

			switch (type)
			{
				case ExpressionSchemaType.Entity:
					return new EntityExpressionSchemaBuilder(propertyDef, parameterPrefix);

				case ExpressionSchemaType.Group:
					return new GroupExpressionSchemaBuilder(propertyDef, parameterPrefix);

				case ExpressionSchemaType.Unknown:
				default:
					Debug.Fail(String.Format("Ŀ������ {0} �ı��ʽ�ܹ����Ͳ���ʶ��", propertyDef.Type.FullName));
					return null;
			}
		}

		/// <summary>
		/// ��ȡĿ�����͵ı��ʽ�ܹ����͡�
		/// </summary>
		/// <param name="targetType">Ŀ�����͡�</param>
		/// <returns>���ʽ�ܹ����͡�</returns>
		internal static ExpressionSchemaType GetExpressionSchemaType(Type targetType)
		{
			if (Attribute.IsDefined(targetType, typeof(TableAttribute)))
			{
				return ExpressionSchemaType.Entity;
			}
			else if (Attribute.IsDefined(targetType, typeof(GroupAttribute)))
			{
				return ExpressionSchemaType.Group;
			}
			else
			{
				return ExpressionSchemaType.Unknown;
			}
		}

		#region ��������

		#endregion
	}
}