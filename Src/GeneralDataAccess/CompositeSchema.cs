#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeSchema.cs
// �ļ�������������ʾ����ʵ��ܹ���
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
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ʾ����ʵ��ܹ���
	/// </summary>
	public sealed class CompositeSchema : IDebugInfoProvider
	{
		#region ˽���ֶ�

		private readonly CompositeDefinition m_definition;
		private readonly Dictionary<String, ExpressionSchema> m_foreignReferences = new Dictionary<String, ExpressionSchema>();

		private String m_sqlExpression;
		private QueryParameter[] m_parameters;
		private EntitySchemaComposite m_root;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ø���ʵ�嶨�塣
		/// </summary>
		/// <param name="definition">����ʵ�嶨�塣</param>
		internal CompositeSchema(CompositeDefinition definition)
		{
			m_definition = definition;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ SQL ���ʽ��
		/// </summary>
		public String SqlExpression
		{
			get { return m_sqlExpression; }
			internal set { m_sqlExpression = value; }
		}

		/// <summary>
		/// ��ȡ��ѯ�������ϡ�
		/// </summary>
		public QueryParameter[] Parameters
		{
			get { return m_parameters; }
			internal set { m_parameters = value; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��������ʵ�塣
		/// </summary>
		/// <param name="record">��¼��</param>
		/// <returns>�����õĸ���ʵ�塣</returns>
		public CompositeResult Compose(IDataRecord record)
		{
			CompositeResult result = (CompositeResult)m_definition.Constructor.Invoke((Object[])null);

			Compose(result, record);

			return result;
		}

		/// <summary>
		/// ��������ʵ�塣
		/// </summary>
		/// <param name="result">Ҫ���ֵ�ĸ���ʵ�塣</param>
		/// <param name="record">��¼��</param>
		public void Compose(CompositeResult result, IDataRecord record)
		{
			Object[] dbValues = new Object[record.FieldCount];
			record.GetValues(dbValues);

			result[m_definition.Root.Name] = Root.Target.Compose(dbValues);

			foreach (CompositePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				String name = propertyDef.Name;
				ExpressionSchema schema = m_foreignReferences[name];
				result[name] = schema.Compose(dbValues);
			}
		}

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ�����ø�ʵ��ܹ���ϡ�
		/// </summary>
		internal EntitySchemaComposite Root
		{
			get { return m_root; }
			set { m_root = value; }
		}

		/// <summary>
		/// ��ȡ����ʵ���а����ı��ʽ�ܹ����ϡ�
		/// </summary>
		internal Dictionary<String, ExpressionSchema> ForeignReferences
		{
			get { return m_foreignReferences; }
		}

		#endregion

		#region IDebugInfoProvider ��Ա

		/// <summary>
		/// ��ȡ����ʵ��ܹ�����ϸ��Ϣ�����ڵ��ԡ�
		/// </summary>
		/// <returns></returns>
		public String Dump()
		{
			// ʵ���������
			String line = String.Empty.PadRight(120, '-');
			StringBuilder output = new StringBuilder();

			output.AppendLine();
			output.AppendLine(line);

			output.AppendFormat("����ʵ��ܹ���ϣ�{0}", m_definition.Type.FullName);

			output.AppendLine();
			output.AppendLine(line);

			// ��ʵ��ܹ�
			output.AppendFormat("��ʵ��ܹ���{0}\r\n{1}", Root.Dump(1), Root.BuilderStrategy.Dump(1));

			// �ⲿ����ʵ��ܹ�
			foreach (CompositeForeignReferencePropertyDefinition propertyDef in m_definition.ForeignReferences)
			{
				List<String> mappings = new List<String>();

				for (Int32 i = 0; i < propertyDef.RootPropertyChains.Length; i++)
				{
					mappings.Add(
							String.Format(
									"{0} -> {1}",
									propertyDef.RootPropertyChains[i].FullName,
									propertyDef.MappedPropertyChains[i].FullName
								)
						);
				}

				output.AppendLine();

				switch (propertyDef.JoinMode)
				{
					case PropertyJoinMode.Left:
						output.Append("LEFT JOIN ");
						break;

					case PropertyJoinMode.Right:
						output.Append("RIGHT JOIN ");
						break;

					case PropertyJoinMode.Inner:
						output.Append("INNER JOIN ");
						break;

					default:
						break;
				}

				output.AppendFormat("{1} ({2})", propertyDef.JoinMode.ToString(), propertyDef.Name, String.Join(" ", mappings.ToArray()));
				output.AppendLine();

				output.Append(ForeignReferences[propertyDef.Name].Dump(1));
			}

			// ����
			output.AppendLine();
			output.AppendLine(line);

			return output.ToString();
		}

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