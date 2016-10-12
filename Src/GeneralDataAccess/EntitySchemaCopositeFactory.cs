#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����EntitySchemaCopositeFactory.cs
// �ļ�������������������ʵ��ܹ���ϡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110413
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��������ʵ��ܹ���ϡ�
	/// </summary>
	public class EntitySchemaCompositeFactory
	{
		#region ��̬��Ա

		#region ��������

		/// <summary>
		/// ����ָ��ʵ�����͵�ʵ��ܹ���ϣ�ʹ��Ĭ�ϵ����ɲ��ԡ�
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>��ʵ�����͵ļܹ���ϡ�</returns>
		public static EntitySchemaComposite Create(Type entityType)
		{
			return Create(entityType, CompositeBuilderStrategyFactory.Create(entityType));
		}

		/// <summary>
		/// ����ָ��ʵ�����͵�ʵ��ܹ���ϡ�
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <param name="builderStrategy">���ɲ��ԡ�</param>
		/// <returns>��ʵ�����͵ļܹ���ϡ�</returns>
		public static EntitySchemaComposite Create(Type entityType, CompositeBuilderStrategy builderStrategy)
		{
			EntitySchemaCompositeFactory factory = new EntitySchemaCompositeFactory(builderStrategy);

			return factory.DoCreate(entityType);
		}

		#endregion

		#endregion

		#region ˽���ֶ�

		private readonly CompositeBuilderStrategy m_builderStrategy;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����ʹ��Ĭ�����ɲ���
		/// </summary>
		private EntitySchemaCompositeFactory()
			: this(CompositeBuilderStrategyFactory.Default)
		{
		}

		/// <summary>
		/// ���캯����˽�л���֧�ֵ�����
		/// </summary>
		/// <param name="builderStrategy">���ɲ��ԡ�</param>
		private EntitySchemaCompositeFactory(CompositeBuilderStrategy builderStrategy)
		{
			m_builderStrategy = builderStrategy;
		}

		#endregion

		#region ��������

		/// <summary>
		/// �����ܹ���ϡ�
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>�����͵ļܹ���ϡ�</returns>
		public EntitySchemaComposite DoCreate(Type entityType)
		{
			EntitySchemaComposite composite = Build(entityType);

			return composite;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ���幹��ʵ��ܹ���ϡ�
		/// </summary>
		/// <param name="entityType">ʵ�����͡�</param>
		/// <returns>ʵ��ܹ���ϡ�</returns>
		private EntitySchemaComposite Build(Type entityType)
		{
			#region ���ܼ���

			Timing.Start("����ʵ��ܹ����", "EntitySchemaCompositeFactory.Build {DB558798-09FA-4cc5-8C73-8A1173A1ABEB}");

			#endregion

			EntityDefinition definition = EntityDefinitionBuilder.Build(entityType);
			EntitySchema target = new EntitySchema(definition);
			EntitySchemaComposite composite = new EntitySchemaComposite(m_builderStrategy, target);

			BuildForeignReferences(target);

			composite.Initialize();

			#region ���ܼ���

			Timing.Stop("EntitySchemaCompositeFactory.Build {DB558798-09FA-4cc5-8C73-8A1173A1ABEB}");

			#endregion


			return composite;
		}

		/// <summary>
		/// �����ⲿ���üܹ����˷������ݹ�ص����Խ���ȫ�����ⲿ���üܹ���
		/// </summary>
		/// <param name="target">Ŀ��ܹ���</param>
		private void BuildForeignReferences(EntitySchema target)
		{
			#region �ݹ���ֹ����

			if ((m_builderStrategy.MaxForeignReferenceLevel >= 0) && (target.Level >= m_builderStrategy.MaxForeignReferenceLevel))
			{
				return;
			}

			#endregion

			List<EntitySchemaRelation> rightRelations = new List<EntitySchemaRelation>();

			// ������ϵ
			foreach (EntityPropertyDefinition property in target.Entity.GetForeignReferenceProperties())
			{
				EntityDefinition parentDef = EntityDefinitionBuilder.Build(property.Type);
				EntitySchema parentSchema = new EntitySchema(target.Composite, parentDef);

				parentSchema.Level = (target.Level + 1);

				Column[] childColumns = target.Columns.GetColumnsByDefinitions(property.Relation.ChildColumns);
				Column[] parentColumns = parentSchema.Columns.GetColumnsByDefinitions(property.Relation.ParentColumns);

				EntitySchemaRelation relation = new EntitySchemaRelation(childColumns, parentColumns);

				// ֻҪ��һ������գ��򱾼�Ҳ����գ��������һ��Ϊ�����ӣ�����������ҲΪ������
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