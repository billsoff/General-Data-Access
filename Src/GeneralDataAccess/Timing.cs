#region ��Ȩ���汾�仯����
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 ����������Ϣ�Ƽ����޹�˾
// ��Ȩ����
// 
//
// �ļ�����Timing.cs
// �ļ��������������ڲ�������һ�����������ĵ�ʱ�䡣
//
//
// ������ʶ���α���billsoff@gmail.com�� 20110704
//
// �޸ı�ʶ��
// �޸�������
//
// --------------------------------------------------------------------------------------------------------*/
#endregion ��Ȩ���汾�仯����

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// ���ڲ�������һ�����������ĵ�ʱ�䡣
	/// </summary>
	public static class Timing
	{
		#region ˽���ֶ�

		private static Dictionary<String, Watch> m_watches;

		#endregion

		/// <summary>
		/// ��ʼ������
		/// </summary>
		/// <param name="operationName">�������ơ�</param>
		/// <param name="id">Ψһ��ʶ��</param>
		[Conditional("DEBUG")]
		public static void Start(String operationName, String id)
		{
			if (!Watches.ContainsKey(id))
			{
				m_watches.Add(id, new Watch(operationName, id));
			}

			m_watches[id].Start();
		}

		/// <summary>
		/// ����������
		/// </summary>
		/// <param name="id">Ψһ��ʶ��</param>
		[Conditional("DEBUG")]
		public static void Stop(String id)
		{
			Stop((String)null, id);
		}

		/// <summary>
		/// ����������
		/// </summary>
		/// <param name="msg">������Ϣ��</param>
		/// <param name="id">Ψһ��ʶ��</param>
		[Conditional("DEBUG")]
		public static void Stop(String msg, String id)
		{
			Watch w;

			Watches.TryGetValue(id, out w);

			if (w != null)
			{
				w.Stop(msg);
			}
		}

		private static Dictionary<String, Watch> Watches
		{
			get
			{
				if (m_watches == null)
				{
					m_watches = new Dictionary<String, Watch>();
				}

				return m_watches;
			}
		}


		#region �� Watch

		/// <summary>
		/// �������ܼ�����
		/// </summary>
		private sealed class Watch
		{
			#region ˽���ֶ�

			private Int32 m_count;
			private readonly String m_operationName;
			private readonly String m_id;
			private readonly Stopwatch m_stopwatch;

			#endregion

			#region ���캯��

			public Watch(String operationName, String id)
			{
				m_operationName = operationName;
				m_id = id;

				m_stopwatch = new Stopwatch();
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ȡ���ô�����
			/// </summary>
			public Int32 Count
			{
				get { return m_count; }
			}

			/// <summary>
			/// ��ȡΨһ��ʶ��
			/// </summary>
			public String Id
			{
				get { return m_id; }
			}

			/// <summary>
			/// ��ȡ�������ơ�
			/// </summary>
			public String OperationName
			{
				get { return m_operationName; }
			}

			/// <summary>
			/// ��ȡ��ʱ���
			/// </summary>
			public Stopwatch Stopwatch
			{
				get { return m_stopwatch; }
			}

			#endregion

			#region ��������

			/// <summary>
			/// ��ʼ������
			/// </summary>
			public void Start()
			{
				m_stopwatch.Stop();
				m_stopwatch.Reset();

				if (m_count == Int32.MaxValue)
				{
					m_count = 0;
				}

				Debug.WriteLine(
						String.Format(
								"{0:yyyy-MM-dd HH:mm}: ���ܼ��� - {1} ... (�� {2:#,##0} �ε���) ID: {3}",
								DateTime.Now,
								m_operationName,
								++m_count,
								m_id
							)
					);

				m_stopwatch.Start();
			}

			/// <summary>
			/// ֹͣ������
			/// </summary>
			public void Stop()
			{
				Stop(null);
			}

			/// <summary>
			/// ֹͣ������
			/// </summary>
			/// <param name="msg">���ӵ���Ϣ������Ϊ�ա�</param>
			public void Stop(String msg)
			{
				m_stopwatch.Stop();

				if (!String.IsNullOrEmpty(msg))
				{
					Debug.WriteLine(
							String.Format(
									"{0:yyyy-MM-dd HH:mm}: ���ܼ��� - {1}���, {2}, ��ʱ {3:#,##0} ���� (�� {4:#,##0} �ε���) ID: {5}",
									DateTime.Now,
									m_operationName,
									msg,
									m_stopwatch.ElapsedMilliseconds,
									m_count,
									m_id
								)
						);
				}
				else
				{
					Debug.WriteLine(
							String.Format(
									"{0:yyyy-MM-dd HH:mm}: ���ܼ��� - {1}���, ��ʱ {2:#,##0} ���� (�� {3:#,##0} �ε���) ID: {4}",
									DateTime.Now,
									m_operationName,
									m_stopwatch.ElapsedMilliseconds,
									m_count,
									m_id
								)
						);
				}
			}

			#endregion
		}

		#endregion
	}
}