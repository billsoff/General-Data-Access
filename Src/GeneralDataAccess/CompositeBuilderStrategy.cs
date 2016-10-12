#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeBuilderStrategy.cs
// �ļ������������ṩʵ��ܹ�������ɲ��ԵĻ���ʵ�֡�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110505
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
	/// �ṩʵ��ܹ�������ɲ��ԵĻ���ʵ�֡�
	/// </summary>
	[Serializable]
	public abstract class CompositeBuilderStrategy : IDebugInfoProvider
	{
		#region ˽���ֶ�

		private Boolean m_alwaysSelectPrimaryKeyProperties = true;
		private Int32 m_initialLevel;

		private List<EntitySchema> m_loadSchemas;

		#endregion

		/// <summary>
		/// ��ȡ������һ��ֵ����ֵָʾ�Ƿ�����ѡ���������ԣ�Ĭ��Ϊ true��
		/// </summary>
		public Boolean AlwaysSelectPrimaryKeyProperties
		{
			get { return m_alwaysSelectPrimaryKeyProperties; }
			set { m_alwaysSelectPrimaryKeyProperties = value; }
		}

		/// <summary>
		/// ��ȡ��������ʼ�������ж�ʵ��ܹ�������ж϶�Ӧ�������ʼ�������ڷ�����ʵ��Ĳ��������ⲿ�������Եļ��أ���
		/// </summary>
		public Int32 InitialLevel
		{
			get { return m_initialLevel; }
			set { m_initialLevel = value; }
		}

		/// <summary>
		/// ����ⲿ���ü������С���㣬���ʾ�����ơ�
		/// </summary>
		public virtual Int32 MaxForeignReferenceLevel
		{
			get { return -1; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�Ƿ�ѡ������Ϊ�ӳټ��ص��������ԡ�
		/// </summary>
		public virtual Boolean SelectAllProperties
		{
			get { return true; }
		}

		/// <summary>
		/// ָʾ�Ƿ����ʵ��ܹ���
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>������ش�ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		public Boolean LoadFromSchema(EntitySchema schema)
		{
			Boolean loading = DoLoadFromSchema(schema);

			if (loading)
			{
				LoadSchemas.Add(schema);
			}

			return loading;
		}

		/// <summary>
		/// �ж��Ƿ��ָ����ʵ��ܹ��м������ԣ�Ĭ��ʵ�ַ��� false��
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>����������κ����ԣ��򷵻� true�����򷵻� false��</returns>
		public Boolean SelectNothingFrom(EntitySchema schema)
		{
			if (IsSchemaLoaded(schema))
			{
				return DoSelectNothingFrom(schema);
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// ָʾ�Ƿ�ѡ�����ԣ���� SelectAllProperties Ϊ true���򲻻���ô˷�������Ҫѡ���ʵ�����ԡ�
		/// </summary>
		/// <param name="property">ʵ�����ԡ�</param>
		/// <returns>���Ҫѡ������ԣ��򷵻� true�����򷵻� false��</returns>
		public Boolean SelectProperty(EntityProperty property)
		{
			if (IsSchemaLoaded(property.Schema))
			{
				return DoSelectProperty(property);
			}
			else
			{
				return false;
			}
		}

		#region �����ķ���

		#region �鷽��

		/// <summary>
		/// ָʾ�Ƿ����ʵ��ܹ���
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>������ش�ʵ��ܹ����򷵻� true�����򷵻� false��</returns>
		protected virtual Boolean DoLoadFromSchema(EntitySchema schema)
		{
			return true;
		}

		/// <summary>
		/// �ж��Ƿ��ָ����ʵ��ܹ��м������ԣ�Ĭ��ʵ�ַ��� false��
		/// </summary>
		/// <param name="schema">ʵ��ܹ���</param>
		/// <returns>����������κ����ԣ��򷵻� true�����򷵻� false��</returns>
		protected virtual Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			return false;
		}

		/// <summary>
		/// ָʾ�Ƿ�ѡ�����ԣ���� SelectAllProperties Ϊ true���򲻻���ô˷�������Ҫѡ���ʵ�����ԡ�
		/// </summary>
		/// <param name="property">ʵ�����ԡ�</param>
		/// <returns>���Ҫѡ������ԣ��򷵻� true�����򷵻� false��</returns>
		protected virtual Boolean DoSelectProperty(EntityProperty property)
		{
			return true;
		}

		#endregion

		/// <summary>
		/// �ж�Ҫ���ӵ�ʵ��ܹ��Ƿ��ظ����֣���ֹ�������޵ݹ飬׷�ݼ���Ϊ���� InitialLevel 2����������
		/// </summary>
		/// <param name="schemaToAttach">Ҫ���ӵ�ʵ��ܹ���</param>
		/// <returns>�����ʵ��ܹ��ظ����֣��򷵻� true�����򷵻� false��</returns>
		protected Boolean IsSchemaTypeRepetitive(EntitySchema schemaToAttach)
		{
			// ���ݵ��ڶ����ⲿ����
			const Int32 START_LEVEL = 2;

			return IsSchemaTypeRepetitive(schemaToAttach, START_LEVEL);
		}

		/// <summary>
		/// �ж�Ҫ���ӵ�ʵ��ܹ��Ƿ��ظ����֣���ֹ�������޵ݹ顣
		/// </summary>
		/// <param name="schemaToAttach">Ҫ���ӵ�ʵ��ܹ���</param>
		/// <param name="relativeStartLevel">��������� InitialLevel �ļ�����СΪ�㡣</param>
		/// <returns>�����ʵ��ܹ��ظ����֣��򷵻� true�����򷵻� false��</returns>
		protected Boolean IsSchemaTypeRepetitive(EntitySchema schemaToAttach, Int32 relativeStartLevel)
		{
			if (relativeStartLevel < 0)
			{
				relativeStartLevel = 0;
			}

			Int32 startLevel = m_initialLevel + relativeStartLevel;

			if (schemaToAttach.Level <= startLevel)
			{
				return false;
			}

			EntitySchemaRelation relation = schemaToAttach.LeftRelation;

			while (relation != null)
			{
				EntitySchema child = relation.ChildSchema;

				if (child.Type == schemaToAttach.Type)
				{
					return true;
				}

				// �Ѿ�����׷�ݵ�
				if (child.Level <= startLevel)
				{
					break;
				}

				relation = child.LeftRelation;
			}

			return false;
		}

		#endregion

		#region ˽������

		/// <summary>
		/// ��ȡ���ص�ʵ�弯�ϡ�
		/// </summary>
		public List<EntitySchema> LoadSchemas
		{
			get
			{
				if (m_loadSchemas == null)
				{
					m_loadSchemas = new List<EntitySchema>();
				}

				return m_loadSchemas;
			}
		}

		#endregion

		#region ��������

		/// <summary>
		/// �жϸ�����ʵ��ܹ��Ƿ��ѱ����ء�
		/// </summary>
		/// <param name="schema">Ҫ�жϵ�ʵ��ܹ���</param>
		/// <returns>�����ʵ��ܹ��ѱ����أ��򷵻� true�����򷵻� false��</returns>
		private Boolean IsSchemaLoaded(EntitySchema schema)
		{
			if (schema.Level == m_initialLevel)
			{
				return true;
			}

			if (m_loadSchemas == null)
			{
				return false;
			}

			return m_loadSchemas.Contains(schema);
		}

		#endregion

		#region IDebugInfoProvider ��Ա

		/// <summary>
		/// ��ȡ���ɲ��Ե���ϸ��Ϣ�����ڵ��ԣ�Ĭ�ϵ�ʵ��ֻ�Ǽ򵥵�������ɲ��Ե��������ơ�
		/// </summary>
		/// <returns>���ɲ��Ե���ϸ��Ϣ��</returns>
		public virtual String Dump()
		{
			return String.Format("{0}", GetType().FullName);
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