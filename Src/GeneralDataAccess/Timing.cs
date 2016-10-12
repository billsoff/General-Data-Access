#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Timing.cs
// 文件功能描述：用于测试运行一个方法所消耗的时间。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110704
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
	/// <summary>
	/// 用于测试运行一个方法所消耗的时间。
	/// </summary>
	public static class Timing
	{
		#region 私有字段

		private static Dictionary<String, Watch> m_watches;

		#endregion

		/// <summary>
		/// 开始计数。
		/// </summary>
		/// <param name="operationName">操作名称。</param>
		/// <param name="id">唯一标识。</param>
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
		/// 结束计数。
		/// </summary>
		/// <param name="id">唯一标识。</param>
		[Conditional("DEBUG")]
		public static void Stop(String id)
		{
			Stop((String)null, id);
		}

		/// <summary>
		/// 结束计数。
		/// </summary>
		/// <param name="msg">附加消息。</param>
		/// <param name="id">唯一标识。</param>
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


		#region 类 Watch

		/// <summary>
		/// 用于性能计数。
		/// </summary>
		private sealed class Watch
		{
			#region 私有字段

			private Int32 m_count;
			private readonly String m_operationName;
			private readonly String m_id;
			private readonly Stopwatch m_stopwatch;

			#endregion

			#region 构造函数

			public Watch(String operationName, String id)
			{
				m_operationName = operationName;
				m_id = id;

				m_stopwatch = new Stopwatch();
			}

			#endregion

			#region 公共属性

			/// <summary>
			/// 获取调用次数。
			/// </summary>
			public Int32 Count
			{
				get { return m_count; }
			}

			/// <summary>
			/// 获取唯一标识。
			/// </summary>
			public String Id
			{
				get { return m_id; }
			}

			/// <summary>
			/// 获取操作名称。
			/// </summary>
			public String OperationName
			{
				get { return m_operationName; }
			}

			/// <summary>
			/// 获取计时秒表。
			/// </summary>
			public Stopwatch Stopwatch
			{
				get { return m_stopwatch; }
			}

			#endregion

			#region 公共方法

			/// <summary>
			/// 开始计数。
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
								"{0:yyyy-MM-dd HH:mm}: 性能计数 - {1} ... (第 {2:#,##0} 次调用) ID: {3}",
								DateTime.Now,
								m_operationName,
								++m_count,
								m_id
							)
					);

				m_stopwatch.Start();
			}

			/// <summary>
			/// 停止计数。
			/// </summary>
			public void Stop()
			{
				Stop(null);
			}

			/// <summary>
			/// 停止计数。
			/// </summary>
			/// <param name="msg">附加的消息，可以为空。</param>
			public void Stop(String msg)
			{
				m_stopwatch.Stop();

				if (!String.IsNullOrEmpty(msg))
				{
					Debug.WriteLine(
							String.Format(
									"{0:yyyy-MM-dd HH:mm}: 性能计数 - {1}完成, {2}, 用时 {3:#,##0} 毫秒 (第 {4:#,##0} 次调用) ID: {5}",
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
									"{0:yyyy-MM-dd HH:mm}: 性能计数 - {1}完成, 用时 {2:#,##0} 毫秒 (第 {3:#,##0} 次调用) ID: {4}",
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