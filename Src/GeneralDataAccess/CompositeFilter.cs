#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeFilter.cs
// �ļ�������������ʾһ�����Ϲ�����������һ���ڲ��࣬�� Filter.Combine ����ʹ�á�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20100815
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
	/// ��ʾһ�����Ϲ�����������һ���ڲ��࣬�� Filter.Combine ����ʹ�á�
	/// </summary>
	[Serializable]
	internal class CompositeFilter : Filter
	{
		#region ���캯��

		/// <summary>
		/// ���캯���������߼����ӷ����ӹ��������ϡ�
		/// </summary>
		/// <param name="logicOperator">�߼����ӷ���AND �� OR��</param>
		/// <param name="filters">�ӹ��������ϡ�</param>
		public CompositeFilter(LogicOperator logicOperator, IEnumerable<Filter> filters)
			: base(logicOperator)
		{
			foreach (Filter f in filters)
			{
				this.Filters.Add(f);
			}
		}

		#endregion
	}
}