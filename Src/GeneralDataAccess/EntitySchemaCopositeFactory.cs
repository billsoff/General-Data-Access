#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntitySchemaCopositeFactory.cs
// 文件功能描述：用于生成实体架构组合。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110413
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于生成实体架构组合。
	/// </summary>
	public class EntitySchemaCompositeFactory
	{
		#region 静态成员

		#region 公共方法

		/// <summary>
		/// 创建指定实体类型的实体架构组合，使用默认的生成策略。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>该实体类型的架构组合。</returns>
		public static EntitySchemaComposite Create(Type entityType)
		{
			return Create(entityType, CompositeBuilderStrategyFactory.Create(entityType));
		}

		/// <summary>
		/// 创建指定实体类型的实体架构组合。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <param name="builderStrategy">生成策略。</param>
		/// <returns>该实体类型的架构组合。</returns>
		public static EntitySchemaComposite Create(Type entityType, CompositeBuilderStrategy builderStrategy)
		{
			EntitySchemaCompositeFactory factory = new EntitySchemaCompositeFactory(builderStrategy);

			return factory.DoCreate(entityType);
		}

		#endregion

		#endregion

		#region 私有字段

		private readonly CompositeBuilderStrategy m_builderStrategy;

		#endregion

		#region 构造函数

		/// <summary>
		/// 默认构造函数，使用默认生成策略
		/// </summary>
		private EntitySchemaCompositeFactory()
			: this(CompositeBuilderStrategyFactory.Default)
		{
		}

		/// <summary>
		/// 构造函数，私有化，支持单例。
		/// </summary>
		/// <param name="builderStrategy">生成策略。</param>
		private EntitySchemaCompositeFactory(CompositeBuilderStrategy builderStrategy)
		{
			m_builderStrategy = builderStrategy;
		}

		#endregion

		#region 公共方法

		/// <summary>
		/// 创建架构组合。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>该类型的架构组合。</returns>
		public EntitySchemaComposite DoCreate(Type entityType)
		{
			EntitySchemaComposite composite = Build(entityType);

			return composite;
		}

		#endregion

		#region 辅助方法

		/// <summary>
		/// 具体构建实体架构组合。
		/// </summary>
		/// <param name="entityType">实体类型。</param>
		/// <returns>实体架构组合。</returns>
		private EntitySchemaComposite Build(Type entityType)
		{
			#region 性能计数

			Timing.Start("生成实体架构组合", "EntitySchemaCompositeFactory.Build {DB558798-09FA-4cc5-8C73-8A1173A1ABEB}");

			#endregion

			EntityDefinition definition = EntityDefinitionBuilder.Build(entityType);
			EntitySchema target = new EntitySchema(definition);
			EntitySchemaComposite composite = new EntitySchemaComposite(m_builderStrategy, target);

			BuildForeignReferences(target);

			composite.Initialize();

			#region 性能计数

			Timing.Stop("EntitySchemaCompositeFactory.Build {DB558798-09FA-4cc5-8C73-8A1173A1ABEB}");

			#endregion


			return composite;
		}

		/// <summary>
		/// 创建外部引用架构，此方法被递归地调用以建立全部的外部引用架构。
		/// </summary>
		/// <param name="target">目标架构。</param>
		private void BuildForeignReferences(EntitySchema target)
		{
			#region 递归终止条件

			if ((m_builderStrategy.MaxForeignReferenceLevel >= 0) && (target.Level >= m_builderStrategy.MaxForeignReferenceLevel))
			{
				return;
			}

			#endregion

			List<EntitySchemaRelation> rightRelations = new List<EntitySchemaRelation>();

			// 创建关系
			foreach (EntityPropertyDefinition property in target.Entity.GetForeignReferenceProperties())
			{
				EntityDefinition parentDef = EntityDefinitionBuilder.Build(property.Type);
				EntitySchema parentSchema = new EntitySchema(target.Composite, parentDef);

				parentSchema.Level = (target.Level + 1);

				Column[] childColumns = target.Columns.GetColumnsByDefinitions(property.Relation.ChildColumns);
				Column[] parentColumns = parentSchema.Columns.GetColumnsByDefinitions(property.Relation.ParentColumns);

				EntitySchemaRelation relation = new EntitySchemaRelation(childColumns, parentColumns);

				// 只要上一级允许空，则本级也允许空，即如果上一级为左连接，则其后的连接也为左连接
				if (!relation.PermitNull && (target.LeftRelation != null) && target.LeftRelation.PermitNull)
				{
					relation.PermitNull = true;
				}

				parentSchema.LeftRelation = relation;

				if (m_builderStrategy.LoadFromSchema(parentSchema))
				{
					rightRelations.Add(relation);
				}
			}

			if (rightRelations.Count != 0)
			{
				target.RightRelations = rightRelations.ToArray();

				foreach (EntitySchemaRelation relation in target.RightRelations)
				{
					BuildForeignReferences(relation.ParentSchema);
				}
			}
		}

		#endregion
	}
}