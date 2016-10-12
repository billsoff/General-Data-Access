#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：UpdateAction.cs
// 文件功能描述：实体更新动作。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110223
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Text;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 实体更新动作。
    /// </summary>
    internal enum UpdateAction
    {
        /// <summary>
        /// 不更新。
        /// </summary>
        None,

        /// <summary>
        /// 插入。
        /// </summary>
        Add,

        /// <summary>
        /// 修改。
        /// </summary>
        Modify,

        /// <summary>
        /// 删除。
        /// </summary>
        Delete
    }
}