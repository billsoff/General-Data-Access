#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����CompositeNothingBuilderStrategy.cs
// �ļ�������������ѡ���κ����ԡ�
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110524
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ��ѡ���κ����ԡ�
	/// </summary>
	[Serializable]
	internal sealed class CompositeNothingBuilderStrategy : CompositeBuilderStrategy, ISerializable
	{
		#region ����

		public static readonly CompositeNothingBuilderStrategy Value = new CompositeNothingBuilderStrategy();

		#endregion

		#region ���캯��

		/// <summary>
		/// Ĭ�Ϲ��캯����˽�л�����ʵ�ֵ�����
		/// </summary>
		private CompositeNothingBuilderStrategy()
		{
		}

		/// <summary>
		/// ISerializable �Ĺ��캯����ʵ���ϴӲ�����á�
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private CompositeNothingBuilderStrategy(SerializationInfo info, StreamingContext context)
		{
		}

		#endregion

		#region ����

		/// <summary>
		/// ����Ϊ 0��
		/// </summary>
		public override Int32 MaxForeignReferenceLevel
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// ֻ���ص� 0 ��ʵ��ܹ���
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoLoadFromSchema(EntitySchema schema)
		{
			return (schema.Level == 0);
		}

		/// <summary>
		/// �������κ����ԡ�
		/// </summary>
		public override Boolean SelectAllProperties
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// �������κ����ԡ�
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override Boolean DoSelectNothingFrom(EntitySchema schema)
		{
			return true;
		}

		/// <summary>
		/// �������κ����ԡ�
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		protected override Boolean DoSelectProperty(EntityProperty property)
		{
			return false;
		}

		#endregion

		#region ������Ϣ

		/// <summary>
		/// ��ȡ���Ե���ϸ��Ϣ��
		/// </summary>
		/// <returns></returns>
		public override String Dump()
		{
			return String.Format("{0}����ѡ���κ�����", GetType().FullName);
		}

		#endregion

		#region ISerializable ��Ա

		/// <summary>
		/// ���л���ͨ������ IObjectReference ����ʹ�����л�����������ָ��Ӧ�ó������ͬһ��������
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof(CompositeNothingBuilderStrategySerializationHelper));
		}

		#endregion


		#region ������ CompositeNothingBuilderStrategySerializationHelper

		/// <summary>
		/// ��֤���л��Ķ���ָ��Ӧ�ó�����ĵ�����
		/// </summary>
		[Serializable]
		private class CompositeNothingBuilderStrategySerializationHelper : IObjectReference
		{
			#region IObjectReference ��Ա

			/// <summary>
			/// ��ȡ CompositeNothingBuilderStrategy�����Ƿ���Ψһʵ����
			/// </summary>
			/// <param name="context"></param>
			/// <returns></returns>
			public Object GetRealObject(StreamingContext context)
			{
				return CompositeNothingBuilderStrategy.Value;
			}

			#endregion
		}

		#endregion
	}
}