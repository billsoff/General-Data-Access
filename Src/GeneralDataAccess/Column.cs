#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����Column.cs
// �ļ������������С�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110413
//
// �޸ı�ʶ���α� 20110707
// �޸������������Ϊ�ɼ̳еģ����� Name��FullName��Type��DbType ��������Ϊ����ģ������� Alias��SqlExpression �����ԡ�
//
// �޸ı�ʶ���α� 20110712
// �޸������������Ϊ����ģ�������ص����Ը�Ϊ����ģ����� EntityColumn ����ԭ���Ĺ��ܡ�
//
// �޸ı�ʶ��
// �޸�������
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
	/// �У���ǰ��ʵ����һ��ʵ���С�
	/// </summary>
	public abstract class Column
	{
		#region ˽���ֶ�

		private Int32 m_index;
		private Int32 m_fieldIndexOffset;
		private Boolean m_selected;

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯�������������ࡣ
		/// </summary>
		protected Column()
		{
		}

		#endregion

		#region �����Ա

		#region ��������

		/// <summary>
		/// ��ȡ�е����ݿ����͡�
		/// </summary>
		public abstract DbType DbType { get; }

		/// <summary>
		/// ��ȡ�б��ʽ�����ھۺ��У�Ϊ�ۺϱ��ʽ��Ĭ��Ϊ�е�ȫ������
		/// </summary>
		public abstract String Expression { get; }

		/// <summary>
		/// ��ȡ�е�ȫ���ƣ������޶�����
		/// </summary>
		public abstract String FullName { get; }

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ���Ƿ�Ϊ������
		/// </summary>
		public abstract Boolean IsPrimaryKey { get; }

		/// <summary>
		/// ��ȡһ��ֵ��ָʾ�����Ƿ�Ϊ�����С�
		/// </summary>
		public abstract Boolean IsPrimitive { get; }

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ��ӳټ��ء�
		/// </summary>
		public abstract Boolean LazyLoad { get; }

		/// <summary>
		/// ��ȡ�����ơ�
		/// </summary>
		public abstract String Name { get; }

		/// <summary>
		/// ��ȡʵ�����ԡ�
		/// </summary>
		public abstract EntityProperty Property { get; }

		/// <summary>
		/// ��ȡ����ӳ������Ե����ơ�
		/// </summary>
		public abstract String PropertyName { get; }

		/// <summary>
		/// ��ȡ�����͡�
		/// </summary>
		public abstract Type Type { get; }

		#endregion

		#region �ڲ�����

		/// <summary>
		/// ��ȡ�ж��塣
		/// </summary>
		internal abstract ColumnDefinition Definition { get; }

		#endregion

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ�ۺ����ƣ�Ĭ��Ϊ null��
		/// </summary>
		public virtual String AggregationName
		{
			get { return null; }
		}

		/// <summary>
		/// ��ȡ��ǰ�еı�����
		/// <para>�����ǰ�б�ѡ���򷵻�����������򣬷�����ȫ����</para>
		/// </summary>
		public String Alias
		{
			get
			{
				if (Selected)
				{
					return CommonPolicies.GetColumnAlias(Index);
				}
				else
				{
					return FullName;
				}
			}
		}

		/// <summary>
		/// ��ȡ�ֶ�������
		/// </summary>
		public Int32 FieldIndex
		{
			get { return (m_index + m_fieldIndexOffset); }
		}

		/// <summary>
		/// ��ȡ�ֶ�����ƫ�ơ�
		/// </summary>
		public Int32 FieldIndexOffset
		{
			get { return m_fieldIndexOffset; }
			internal protected set { m_fieldIndexOffset = value; }
		}

		/// <summary>
		/// ��ȡ�У��ڼ�¼�еģ�������
		/// </summary>
		public Int32 Index
		{
			get { return m_index; }
			internal protected set { m_index = value; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�������Ƿ���ԭ���������������ݿ����ɵ���������
		/// </summary>
		public Boolean IsPrimaryKeyNative
		{
			get { return (Definition != null) ? Definition.IsPrimaryKeyNative : false; }
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ�ѡ��
		/// </summary>
		public Boolean Selected
		{
			get { return m_selected; }
			internal protected set { m_selected = value; }
		}

		#endregion

		#region ��������

		/// <summary>
		/// ��ȡ���С�
		/// </summary>
		/// <returns>���С�</returns>
		public Column GetParentColumn()
		{
			if (IsPrimitive || (Property == null) || !Property.Schema.HasRightRelations)
			{
				return null;
			}

			foreach (EntitySchemaRelation relation in Property.Schema.RightRelations)
			{
				if (relation.ChildProperty == Property)
				{
					Column parentColumn = relation.GetMappingParentColumn(this);

					return parentColumn;
				}
			}

			return null;
		}

		/// <summary>
		/// ��ȡ�е����ݿ�ֵ��������õ�ֵΪ null������ת��Ϊ DBNull������� DBEmpty����ָʾ�������²������Ը��� �������Ǵ���������ʵ�忪ʼ������
		/// </summary>
		/// <param name="entity">Ҫ������ʵ�塣</param>
		/// <returns>ʵ���е�ǰ�е�ֵ��</returns>
		public Object GetDbValue(Object entity)
		{
			return GetDbValue(entity, false);
		}

		/// <summary>
		/// ��ȡ�е����ݿ�ֵ��������õ�ֵΪ null������ת��Ϊ DBNull������� DBEmpty����ָʾ�������²������Ը��� ����
		/// </summary>
		/// <param name="entity">Ҫ������ʵ�塣</param>
		/// <param name="isProperty">
		/// <para>���Ϊ true��ָʾҪ������ʵ��ӳ���������������ԣ�ֻ���ⲿ�������������壩��</para>
		/// <para>�������Ǵ���������ʵ�嶨�忪ʼ������</para>
		/// </param>
		/// <returns>ʵ���е�ǰ�е�ֵ��</returns>
		public Object GetDbValue(Object entity, Boolean isProperty)
		{
			if (entity == null)
			{
				return DBNull.Value;
			}

			ColumnDefinition columnDef = Definition;

			if (!IsPrimitive && isProperty)
			{
				columnDef = columnDef.GetParentColumn();
			}

			#region ǰ�ö���

			Debug.Assert(
					columnDef.Property.Entity.Type.IsAssignableFrom(entity.GetType()),
					String.Format(
							"ʵ��ֵ�����ͣ�{0}������������ʵ�����ͣ�{1}�������ݡ�",
							entity.GetType(),
							columnDef.Property.Entity.Type
						)
				);

			#endregion

			return columnDef.GetDbValue(entity);
		}

		/// <summary>
		/// ��ʾ�е������ơ�
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return FullName;
		}

		#endregion
	}
}