#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Util.cs
// 文件功能描述：提供公共服务。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120203
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Useease.Utilities;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 提供公共服务。
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// 供当前程序集使用的 Log 实例。
        /// </summary>
        public static readonly Log Log = new MyLog();


        #region 日志类

        /// <summary>
        /// 供当前程序集使用的日志类。
        /// </summary>
        private sealed class MyLog : Log
        {
            /// <summary>
            /// 跟踪源名称，为根命名空间。
            /// </summary>
            public override string TraceSourceName
            {
                get
                {
                    return "Useease.GeneralDataAccess";
                }
            }
        }

        #endregion
    }
}