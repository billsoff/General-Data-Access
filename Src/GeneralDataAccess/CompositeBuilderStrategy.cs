#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CompositeBuilderStrategy.cs
// 文件功能描述：提供实体架构组合生成策略的基本实现。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110505
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 提供实体架构组合生成策略的基本实现。
	/// </summary>
	[Serializable]
	public abstract class CompositeBuilderStrategy : IDebugInfoProvider
	{
		#region 私有字段

		private Boolean m_alwaysSelectPrimaryKeyProperties = true;
		private Int32 m_initialLevel;

		private List<EntitySchema> m_loadSchemas;

		#endregion

		/// <summary>
		/// 获取或设置一个值，该值指示是否总是选择主键属性，默认为 true。
		/// </summary>
		public Boolean AlwaysSelectPrimaryKeyProperties
		{
			get { return m_alwaysSelectPrimaryKeyProperties; }
			set { m_alwaysSelectPrimaryKeyProperties = value; }
		}

		/// <summary>
		/// 获取或设置起始级别，所有对实体架构级别的判断都应相对于起始级别（用于分组结果实体的参与分组的外部引用属性的加载）。
		/// </summary>
		public Int32 InitialLevel
		{
			get { return m_initialLevel; }
			set { m_initialLevel = value; }
		}

		/// <summary>
		/// 最大外部引用级别，如果小于零，则表示不限制。
		/// </summary>
		public virtual Int32 MaxForeignReferenceLevel
		{
			get { return -1; }
		}

		/// <summary>
		/// 获取一个值，该值指示是否选择除标记为延迟加载的所有属性。
		/// </summary>
		public virtual Boolean SelectAllProperties
		{
			get { return true; }
		}

		/// <summary>
		/// 指示是否加载实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果加载此实体架构，则返回 true；否则返回 false。</returns>
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
		/// 判断是否从指定的实体架构中加载属性，默认实现返回 false。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果不加载任何属性，则返回 true；否则返回 false。</returns>
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
		/// 指示是否选择属性，如果 SelectAllProperties 为 true，则不会调用此方法过滤要选择的实体属性。
		/// </summary>
		/// <param name="property">实体属性。</param>
		/// <returns>如果要选择此属性，则返回 true；否则返回 false。</returns>
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

		#region 保护的方法

		#region 虚方法

		/// <summary>
		/// 指示是否加载实体架构。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果加载此实体架构，则返回 true；否则返回 false。</returns>
		protected virtual Boolean DoLoadFromSchema(EntitySchema schema)
		{
			return true;
		}

		/// <summary>
		/// 判断是否从指定的实体架构中加载属性，默认实现返回 false。
		/// </summary>
		/// <param name="schema">实体架构。</param>
		/// <returns>如果不加载任何属性，则返回 true；否则返回 false。</returns>
		protected virtual Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			return false;
		}

		/// <summary>
		/// 指示是否选择属性，如果 SelectAllProperties 为 true，则不会调用此方法过滤要选择的实体属性。
		/// </summary>
		/// <param name="property">实体属性。</param>
		/// <returns>如果要选择此属性，则返回 true；否则返回 false。</returns>
		protected virtual Boolean DoSelectProperty(EntityProperty property)
		{
			return true;
		}

		#endregion

		/// <summary>
		/// 判断要附加的实体架构是否重复出现，防止产生无限递归，追溯级别为高于 InitialLevel 2（包含）。
		/// </summary>
		/// <param name="schemaToAttach">要附加的实体架构。</param>
		/// <returns>如果该实体架构重复出现，则返回 true；否则返回 false。</returns>
		protected Boolean IsSchemaTypeRepetitive(EntitySchema schemaToAttach)
		{
			// 上溯到第二级外部引用
			const Int32 START_LEVEL = 2;

			return IsSchemaTypeRepetitive(schemaToAttach, START_LEVEL);
		}

		/// <summary>
		/// 判断要附加的实体架构是否重复出现，防止产生无限递归。
		/// </summary>
		/// <param name="schemaToAttach">要附加的实体架构。</param>
		/// <param name="relativeStartLevel">上溯相对于 InitialLevel 的级别，最小为零。</param>
		/// <returns>如果该实体架构重复出现，则返回 true；否则返回 false。</returns>
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

				// 已经到达追溯点
				if (child.Level <= startLevel)
				{
					break;
				}

				relation = child.LeftRelation;
			}

			return false;
		}

		#endregion

		#region 私有属性

		/// <summary>
		/// 获取加载的实体集合。
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

		#region 辅助方法

		/// <summary>
		/// 判断给定的实体架构是否已被加载。
		/// </summary>
		/// <param name="schema">要判断的实体架构。</param>
		/// <returns>如果该实体架构已被加载，则返回 true；否则返回 false。</returns>
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

		#region IDebugInfoProvider 成员

		/// <summary>
		/// 获取生成策略的详细信息，用于调试，默认的实现只是简单地输出生成策略的类型名称。
		/// </summary>
		/// <returns>生成策略的详细信息。</returns>
		public virtual String Dump()
		{
			return String.Format("{0}", GetType().FullName);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="indent"></param>
		/// <returns></returns>
		public String Dump(String indent)
		{
			return DbEntityDebugger.IndentText(Dump(), indent);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public String Dump(Int32 level)
		{
			return DbEntityDebugger.IndentText(Dump(), level);
		}

		/// <summary>
		/// 请参见 <see cref="IDebugInfoProvider"/> 的注释。
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