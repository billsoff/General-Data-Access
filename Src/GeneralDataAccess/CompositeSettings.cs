#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeSettings.cs
// �ļ�������������Ը���ʵ��Ĺ��������ϡ�
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��Ը���ʵ��Ĺ��������ϡ�
	/// </summary>
	[Serializable]
	public sealed class CompositeSettings : IEnumerable<CompositeItemSettings>
	{
		#region ˽���ֶ�

		private readonly CompositeDefinition m_definition;
		private readonly Dictionary<String, CompositeItemSettings> m_allItemSettings;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø���ʵ������͡�
		/// </summary>
		/// <param name="type">����ʵ�����͡�</param>
		public CompositeSettings(Type type)
		{
			m_definition = CompositeDefinitionBuilder.Build(type);
			m_allItemSettings = new Dictionary<String, CompositeItemSettings>();

			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				m_allItemSettings.Add(propertyDef.Name, new CompositeItemSettings(this, propertyDef.Name));
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡָ���������ƵĹ�������
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>�����ԵĹ�������</returns>
		public CompositeItemSettings this[String propertyName]
		{
			get
			{
				#region ǰ������

				Debug.Assert(m_allItemSettings.ContainsKey(propertyName), String.Format("���������� {0}��", propertyName));

				#endregion

				return m_allItemSettings[propertyName];
			}
		}

		/// <summary>
		/// ��ȡָ�����ԵĹ�������
		/// </summary>
		/// <param name="chain">��������</param>
		/// <returns>�����ԵĹ�������</returns>
		public CompositeItemSettings this[IPropertyChain chain]
		{
			get { return this[chain.Name]; }
		}

		/// <summary>
		/// ��ȡָ�����ԵĹ�������
		/// </summary>
		/// <param name="chainBuilder">��������������</param>
		/// <returns>�����ԵĹ�������</returns>
		public CompositeItemSettings this[IPropertyChainBuilder chainBuilder]
		{
			get { return this[chainBuilder.Build()]; }
		}

		/// <summary>
		/// ��ȡ����ʵ�����͡�
		/// </summary>
		public Type Type
		{
			get { return Definition.Type; }
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ����ʵ�嶨�塣
		/// </summary>
		internal CompositeDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion

		#region IEnumerable<CompositeItemSettings> ��Ա

		/// <summary>
		/// ��ȡ CompositeResultFilter ö������
		/// </summary>
		/// <returns>CompositeResultFilter ö������</returns>
		public IEnumerator<CompositeItemSettings> GetEnumerator()
		{
			IEnumerable<CompositeItemSettings> e = GetCompositeItemSettings();

			return e.GetEnumerator();
		}

		#endregion

		#region IEnumerable ��Ա

		/// <summary>
		/// ��ȡö������
		/// </summary>
		/// <returns>ö������</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ʵ�������ö�١�
		/// </summary>
		/// <returns>������ö�١�</returns>
		private IEnumerable<CompositeItemSettings> GetCompositeItemSettings()
		{
			foreach (CompositeItemSettings itemSettings in m_allItemSettings.Values)
			{
				yield return itemSettings;
			}
		}

		#endregion
	}
}