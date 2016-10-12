#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����PropertyMappingAttribute.cs
// �ļ�����������ָʾ����ʵ���и�ʵ������ʽʵ��֮���ӳ���ϵ��
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110707
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
	/// ָʾ����ʵ���и�ʵ������ʽʵ��֮���ӳ���ϵ��
	/// </summary>
	[global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class PropertyMappingAttribute : Attribute
	{
		#region ˽���ֶ�

		private readonly String[] m_rootPropertyPath;
		private readonly String[] m_foreignReferencePropertyPath;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø��������ƺ��ⲿ�����������ơ�
		/// </summary>
		/// <param name="rootPropertyName">���������ơ�</param>
		/// <param name="foreignReferencePropertyName">�ⲿ�����������ơ�</param>
		public PropertyMappingAttribute(String rootPropertyName, String foreignReferencePropertyName)
			: this(new String[] { rootPropertyName }, new String[] { foreignReferencePropertyName })
		{
		}

		/// <summary>
		/// ���캯�������ø��������ƺ��ⲿ��������·����
		/// </summary>
		/// <param name="rootPropertyName">���������ơ�</param>
		/// <param name="foreignReferencePropertyPath">�ⲿ��������·����</param>
		public PropertyMappingAttribute(String rootPropertyName, String[] foreignReferencePropertyPath)
			: this(new String[] { rootPropertyName }, foreignReferencePropertyPath)
		{
		}

		/// <summary>
		/// ���캯�������ø�����·�����ⲿ�����������ơ�
		/// </summary>
		/// <param name="rootPropertyPath">������·����</param>
		/// <param name="foreignReferencePropertyName">�ⲿ�����������ơ�</param>
		public PropertyMappingAttribute(String[] rootPropertyPath, String foreignReferencePropertyName)
			: this(rootPropertyPath, new String[] { foreignReferencePropertyName })
		{
		}

		/// <summary>
		/// ���캯�������ø�����·�����ⲿ��������·����
		/// </summary>
		/// <param name="rootPropertyPath">������·����</param>
		/// <param name="foriegnReferencePropertyPath">�ⲿ��������·����</param>
		public PropertyMappingAttribute(String[] rootPropertyPath, String[] foriegnReferencePropertyPath)
		{
			m_rootPropertyPath = rootPropertyPath;
			m_foreignReferencePropertyPath = foriegnReferencePropertyPath;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ������·����
		/// </summary>
		public String[] RootPropertyPath
		{
			get { return m_rootPropertyPath; }
		}

		/// <summary>
		/// ��ȡ�ⲿ��������·����
		/// </summary>
		public String[] ForeignReferencePropertyPath
		{
			get { return m_foreignReferencePropertyPath; }
		}

		#endregion
	}
}