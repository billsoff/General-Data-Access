#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����DBEmpty.cs
// �ļ�������������ʾ���ݿ��¼��һ���ֶε�ֵ��δ���ã���ͬ�� DBNull���������ݿ��ύ������޸�ָ��ʱ��ǰ����ʾ�������ֶΣ�ʹ�����ݿ��Ĭ��ֵ�����߽��ֶ�ֵ��Ϊ NULL����
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110415
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
	/// ��ʾ���ݿ��¼��һ���ֶε�ֵ��δ���ã���ͬ�� DBNull���������ݿ��ύ������޸�ָ��ʱ��ǰ����ʾ�������ֶΣ�ʹ�����ݿ��Ĭ��ֵ�����߽��ֶ�ֵ��Ϊ NULL����
	/// </summary>
	[Serializable]
	public sealed class DBEmpty : ISerializable
	{
		#region �����ֶ�

		/// <summary>
		/// ��ʾ <see cref="DBEmpty"/> ���Ψһʵ����
		/// </summary>
		public static readonly DBEmpty Value = new DBEmpty();

		#endregion

		#region ���캯��

		/// <summary>
		/// ˽�л���֧�ֵ�����
		/// </summary>
		private DBEmpty()
		{
		}

		/// <summary>
		/// ISerializable �Ĺ��캯����ʵ���ϴӲ�����á�
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private DBEmpty(SerializationInfo info, StreamingContext context)
		{
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
			info.SetType(typeof(DBEmptySerializationHelper));
		}

		#endregion

		#region ��������

		/// <summary>
		/// �ַ�����ʾ�����ؿմ���String.Empty����
		/// </summary>
		/// <returns></returns>
		public override String ToString()
		{
			return String.Empty;
		}

		/// <summary>
		/// �ַ�����ʾ�����ؿմ���String.Empty����
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public String ToString(IFormatProvider provider)
		{
			return String.Empty;
		}

		#endregion


		#region ������ DBEmptySerializationHelper

		/// <summary>
		/// ��֤���л���Ķ���ָ��Ӧ�ó�����ĵ�����
		/// </summary>
		[Serializable]
		private class DBEmptySerializationHelper : IObjectReference
		{
			#region IObjectReference ��Ա

			/// <summary>
			/// ��ȡ DBEmpty ��ʵ�������Ƿ�����Ψһʵ����
			/// </summary>
			/// <param name="context"></param>
			/// <returns></returns>
			public Object GetRealObject(StreamingContext context)
			{
				return DBEmpty.Value;
			}

			#endregion
		}

		#endregion
	}
}