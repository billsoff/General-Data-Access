#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����DbEntityPropertyInfoCache.cs
// �ļ������������������ݿ�ʵ���������Ϣ��
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
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// �������ݿ�ʵ���������Ϣ��
	/// </summary>
	internal class DbEntityPropertyInfoCache
	{
		#region ��̬��Ա

		#region ˽���ֶ�

		private static readonly DbEntityPropertyInfoCache m_instance;

		#endregion

		#region ���캯��

		/// <summary>
		/// ��̬���캯����
		/// </summary>
		static DbEntityPropertyInfoCache()
		{
			m_instance = new DbEntityPropertyInfoCache();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡʵ��������Ϣ��
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>�����͵�������Ϣ��</returns>
		public static DbEntityPropertyInfo GetProperty(Type entityType)
		{
			return m_instance.DoGetProperty(entityType);
		}

		#endregion

		#endregion

		#region ˽���ֶ�

		private readonly Object m_lock = new Object();
		private readonly Dictionary<Type, DbEntityPropertyInfo> m_propertyInfos = new Dictionary<Type, DbEntityPropertyInfo>();

		#endregion

		#region ���췽��

		/// <summary>
		/// ���캯����˽�л�����ʵ�ֵ�����
		/// </summary>
		private DbEntityPropertyInfoCache()
		{
		}

		#endregion

		#region �����ʵ�ַ���

		/// <summary>
		/// ��ȡʵ�����ԡ�
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>��ʵ�����͵����ԡ�</returns>
		public DbEntityPropertyInfo DoGetProperty(Type entityType)
		{
			DbEntityPropertyInfo info;

			m_propertyInfos.TryGetValue(entityType, out info);

			if (info == null)
			{
				info = DbEntityPropertyInfo.Create(entityType);

				lock (m_lock)
				{
					m_propertyInfos[entityType] = info;
				}

				info.Initialize();
			}

			return info;
		}

		#endregion
	}
}