#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����GroupResult.cs
// �ļ�����������������������ʵ��Ļ��ࡣ
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110627
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ������������ʵ��Ļ��ࡣ
	/// </summary>
	[Serializable]
	public abstract class GroupResult : IDebugInfoProvider
	{
		#region ˽���ֶ�

		private readonly Dictionary<String, Object> m_values;
		private readonly GroupDefinition m_definition;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		protected GroupResult()
		{
			Type selfType = GetType();
			m_definition = GroupDefinitionBuilder.Build(selfType);
			m_values = new Dictionary<String, Object>(m_definition.Properties.Count);

			foreach (GroupPropertyDefinition propertyDef in m_definition.Properties)
			{
				m_values.Add(propertyDef.Name, null);
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡҪ�����ʵ�����͡�
		/// </summary>
		public Type Type
		{
			get { return m_definition.Entity.Type; }
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ����������ֵ��
		/// </summary>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>���и����Ƶ����Ե�ֵ��</returns>
		internal Object this[String propertyName]
		{
			get
			{
				#region ǰ������

				Debug.Assert(propertyName != null, "���� propertyName ����Ϊ�գ�null����");
				Debug.Assert(m_values.ContainsKey(propertyName), String.Format("�������ơ�{0}�������ڣ�����ۺϱ�ǡ�", propertyName));

				#endregion

				return m_values[propertyName];
			}

			set
			{
				#region ǰ������

				Debug.Assert(propertyName != null, "���� propertyName ����Ϊ�գ�null����");
				Debug.Assert(m_values.ContainsKey(propertyName), String.Format("�������ơ�{0}�������ڣ�����ۺϱ�ǡ�", propertyName));

				#endregion

				m_values[propertyName] = value;
			}
		}

		/// <summary>
		/// ��ȡ������ʵ�嶨�塣
		/// </summary>
		internal GroupDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ӡ����������ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns></returns>
		public String Dump()
		{
			#region ��ʽ

			/*
-----------------------------------------------------------------------------------------------------
GZPSS.Entity.MACL.EtyUser
-----------------------------------------------------------------------------------------------------
Id: xxx
UserName: xxx
UserAddress:
    Id: 23412325-afed-
    ParentAddress(EtyNewAddress): (Null)
-----------------------------------------------------------------------------------------------------
			 */

			#endregion

			const String PADDING = "    ";
			String line = String.Empty.PadRight(120, '-');

			StringBuilder output = new StringBuilder();

			output.AppendLine();
			output.AppendLine(line);

			output.AppendLine(GetType().FullName);

			output.AppendLine(line);

			foreach (GroupPropertyDefinition propertyDef in Definition.GetPrimitiveProperties())
			{
				output.AppendFormat("{0}: {1}{2}", propertyDef.Name, propertyDef.PropertyInfo.GetValue(this, null), Environment.NewLine);
			}

			foreach (GroupPropertyDefinition propertyDef in Definition.GetForeignReferenceProperties())
			{
				output.AppendFormat("{0}:", propertyDef.Name);

				Object value = propertyDef.PropertyInfo.GetValue(this, null);

				if (value == null)
				{
					output.AppendLine(" (Null)");
				}
				else
				{
					String info = DbEntityDebugger.Dump(value);
					info = Regex.Replace(info, "^", PADDING, RegexOptions.Multiline);

					output.AppendLine(info);
				}
			}

			output.AppendLine(line);

			return output.ToString();
		}

		#endregion

		#region �ܱ����ķ���

		/// <summary>
		/// ��ȡ����ֵ��
		/// </summary>
		/// <typeparam name="TValue">ֵ���͡�</typeparam>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>����ֵ��</returns>
		protected TValue GetValue<TValue>(String propertyName)
		{
			#region ǰ������

			Debug.Assert(
					m_definition.Properties.Contains(propertyName),
					String.Format("���������� {0} ���������� {1}��", m_definition.Type.FullName, propertyName)
				);

			Debug.Assert(
					m_definition.Properties[propertyName].Type == typeof(TValue),
					String.Format(
							"����  {0}  ֵ������ {1} ����������� {2} ��ͬ��",
							propertyName,
							m_definition.Properties[propertyName].Type.Name,
							typeof(TValue).Name
						)
				);

			#endregion

			Object value = this[propertyName];

			if (value != null)
			{
				return (TValue)value;
			}
			else
			{
				return default(TValue);
			}
		}

		#endregion

		#region IDebugInfoProvider ��Ա

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="indent"></param>
		/// <returns></returns>
		public String Dump(String indent)
		{
			return DbEntityDebugger.IndentText(Dump(), indent);
		}

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), level);
		}

		/// <summary>
		/// ��μ� <see cref="IDebugInfoProvider"/> ��ע�͡�
		/// </summary>
		/// <param name="indent"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(String indent, Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), indent, level);
		}

		#endregion
	}
}