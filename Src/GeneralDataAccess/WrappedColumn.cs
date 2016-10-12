#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����WrappedColumn.cs
// �ļ�������������ʾ��һ���еİ�װ��
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
	/// ��ʾ��һ���еİ�װ��
	/// </summary>
	internal abstract class WrappedColumn : Column
	{
		#region ��̬����

		/// <summary>
		/// ������ײ�ı���װ�С�
		/// </summary>
		/// <param name="targentColumn">Ҫ���ҵ�Ŀ���С�</param>
		/// <returns>��ײ�ı���װ�У����Ŀ���в��� WrappedColumn���򷵻�������</returns>
		public static Column GetRootColumn(Column targentColumn)
		{
			if (!(targentColumn is WrappedColumn))
			{
				return targentColumn;
			}

			return GetRootColumn(((WrappedColumn)targentColumn).InnerColumn);
		}

		#endregion

		#region ˽���ֶ�

		private readonly Column m_innerColumn;

		#endregion

		#region ���캯��

		/// <summary>
		/// ���캯�������ñ���װ���С�
		/// </summary>
		/// <param name="innerColumn">����װ���С�</param>
		protected WrappedColumn(Column innerColumn)
		{
			#region ǰ������

			Debug.Assert(innerColumn != null, "����װ���в��� innerColumn ����Ϊ�ա�");

			#endregion

			m_innerColumn = innerColumn;

			Index = innerColumn.Index;
			Selected = innerColumn.Selected;
		}

		#endregion

		#region ��������

		/// <summary>
		/// ����װ���С�
		/// </summary>
		public Column InnerColumn
		{
			get { return m_innerColumn; }
		}

		/// <summary>
		/// ��ȡ�е����ݿ����͡�
		/// </summary>
		public override DbType DbType
		{
			get
			{
				return m_innerColumn.DbType;
			}
		}

		/// <summary>
		/// ��ȡ�б��ʽ�����ھۺ��У�Ϊ�ۺϱ��ʽ��Ĭ��Ϊ�е�ȫ������
		/// </summary>
		public override String Expression
		{
			get
			{
				return m_innerColumn.Expression;
			}
		}

		/// <summary>
		/// ��ȡ�е�ȫ���ƣ������޶�����
		/// </summary>
		public override String FullName
		{
			get
			{
				return m_innerColumn.FullName;
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ���Ƿ�Ϊ������
		/// </summary>
		public override Boolean IsPrimaryKey
		{
			get
			{
				return m_innerColumn.IsPrimaryKey;
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ��ָʾ�����Ƿ�Ϊ�����С�
		/// </summary>
		public override Boolean IsPrimitive
		{
			get
			{
				return m_innerColumn.IsPrimitive;
			}
		}

		/// <summary>
		/// ��ȡһ��ֵ����ֵָʾ�����Ƿ��ӳټ��ء�
		/// </summary>
		public override Boolean LazyLoad
		{
			get
			{
				return m_innerColumn.LazyLoad;
			}
		}

		/// <summary>
		/// ��ȡ�����ơ�
		/// </summary>
		public override String Name
		{
			get
			{
				return m_innerColumn.Name;
			}
		}

		/// <summary>
		/// ��ȡ�����������ԡ�
		/// </summary>
		public override EntityProperty Property
		{
			get
			{
				return m_innerColumn.Property;
			}
		}

		/// <summary>
		/// ��ȡ����ӳ������Ե����ơ�
		/// </summary>
		public override String PropertyName
		{
			get
			{
				return m_innerColumn.PropertyName;
			}
		}

		/// <summary>
		/// ��ȡ�����͡�
		/// </summary>
		public override Type Type
		{
			get
			{
				return m_innerColumn.Type;
			}
		}

		/// <summary>
		/// ��ȡ�ж��塣
		/// </summary>
		internal override ColumnDefinition Definition
		{
			get
			{
				return m_innerColumn.Definition;
			}
		}

		#endregion
	}
}