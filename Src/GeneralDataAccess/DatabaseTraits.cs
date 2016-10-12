#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DatabaseTraits.cs
// 文件功能描述：表示数据库的特性。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120329
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

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 表示数据库的特性。
    /// </summary>
    public class DatabaseTraits
    {
        #region 公共属性

        /// <summary>
        /// 获取一个 SQL 指令，该指令用于获取标识字段的值。
        /// </summary>
        public virtual String RetrieveIdentifierStatement
        {
            get { return null; }
        }

        #endregion
    }
}