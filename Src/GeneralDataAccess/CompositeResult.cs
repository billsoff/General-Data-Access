#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeResult.cs
// �ļ�������������ʾ���Ͻ��������һ��ʵ��ܹ�����������ʽʵ��ܹ���ӳ��Ϊ�Ӳ�ѯ�������Ӷ��ɣ���
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
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾ���Ͻ��������һ��ʵ��ܹ�����������ʽʵ��ܹ���ӳ��Ϊ�Ӳ�ѯ�������Ӷ��ɣ���
	/// </summary>
	[Serializable]
	public abstract class CompositeResult : IDebugInfoProvider
	{
		#region ˽���ֶ�

		private readonly Dictionary<String, Object> m_values;
		private readonly CompositeDefinition m_definition;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����
		/// </summary>
		protected CompositeResult()
		{
			Type selfType = GetType();
			m_definition = CompositeDefinitionBuilder.Build(selfType);
			m_values = new Dictionary<String, Object>();

			m_values.Add(m_definition.Root.Name, null);

			foreach (CompositePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				m_values.Add(propertyDef.Name, null);
			}
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ���������Ե�ֵ��
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		internal Object this[String propertyName]
		{
			get
			{
				#region ǰ������

				Debug.Assert(
						m_values.ContainsKey(propertyName),
						String.Format("����ʵ�� {0} �в��������� {1}��", m_definition.Type.FullName, propertyName)
					);

				#endregion

				return m_values[propertyName];
			}

			set
			{
				#region ǰ������

				Debug.Assert(
						m_values.ContainsKey(propertyName),
						String.Format("����ʵ�� {0} �в��������� {1}��", m_definition.Type.FullName, propertyName)
					);

				#endregion

				m_values[propertyName] = value;
			}
		}

		/// <summary>
		/// ��ȡ����ʵ�嶨�塣
		/// </summary>
		internal CompositeDefinition Definition
		{
			get { return m_definition; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ����ʵ�����ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns>����ʵ�����ϸ��Ϣ��</returns>
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

			// Root
			DumpProperty(m_definition.Root, output, PADDING);

			// �ⲿ����
			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				DumpProperty(propertyDef, output, PADDING);
			}

			output.AppendLine(line);

			return output.ToString();
		}

		#endregion

		#region �����ķ���

		/// <summary>
		/// ��ȡ����ֵ��
		/// </summary>
		/// <typeparam name="TValue">����ֵ���͡�</typeparam>
		/// <param name="propertyName">�������ơ�</param>
		/// <returns>����ֵ��</returns>
		protected TValue GetValue<TValue>(String propertyName) where TValue : class
		{
			return (TValue)this[propertyName];
		}

		#endregion

		#region ��������

		/// <summary>
		/// �������Ե���ϸ��Ϣ��
		/// </summary>
		/// <param name="propertyDef">���Զ��塣</param>
		/// <param name="output">������档</param>
		/// <param name="indent">������</param>
		private void DumpProperty(CompositePropertyDefinition propertyDef, StringBuilder output, String indent)
		{
			output.AppendFormat("{0}:", propertyDef.Name);

			Object value = propertyDef.PropertyInfo.GetValue(this, null);

			if (value == null)
			{
				output.AppendLine(" (Null)");
			}
			else
			{
				ExpressionSchemaType schemaType = ExpressionSchemaBuilderFactory.GetExpressionSchemaType(propertyDef.Type);

				String info;

				switch (schemaType)
				{
					case ExpressionSchemaType.Entity:
						info = DbEntityDebugger.Dump(value);
						break;

					case ExpressionSchemaType.Group:
						info = ((GroupResult)value).Dump();
						break;

					case ExpressionSchemaType.Unknown:
					default:
						info = value.ToString();
						break;
				}

				info = Regex.Replace(info, "^", indent, RegexOptions.Multiline);

				output.AppendLine(info);
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