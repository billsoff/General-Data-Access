#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NativeEmptyPrimaryKeyInfo.cs
// 文件功能描述：表示非原生主键。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110726
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
    /// 表示非原生主键。
    /// </summary>
    internal sealed class NativeEmptyPrimaryKeyInfo : NativePrimaryKeyInfo
    {
        #region 静态成员

        /// <summary>
        /// 单一实例。
        /// </summary>
        public static readonly NativeEmptyPrimaryKeyInfo Value = new NativeEmptyPrimaryKeyInfo();

        #endregion

        #region 构造函数

        /// <summary>
        /// 私有化，以维持一个单例。
        /// </summary>
        private NativeEmptyPrimaryKeyInfo()
        {
        }

        #endregion

        /// <summary>
        /// 总是返回 null。
        /// </summary>
        public override EntityDefinition Definition
        {
            get { return null; }
        }

        /// <summary>
        /// 总是返回 false。
        /// </summary>
        public override Boolean IsNative
        {
            get { return false; }
        }

        /// <summary>
        /// 总是返回 false。
        /// </summary>
        public override bool AutoIncrement
        {
            get { return false; }
        }

        /// <summary>
        /// 总是返回 null。
        /// </summary>
        public override string RetrieveIdentifierStatement
        {
            get { return null; }
        }

        /// <summary>
        /// 总是返回 null。
        /// </summary>
        public override EntityPropertyDefinition NativePrimaryKey
        {
            get { return null; }
        }

        /// <summary>
        /// 总是返回具有零个元素的空数组。
        /// </summary>
        public override EntityPropertyDefinition[] CandidateKeys
        {
            get { return new EntityPropertyDefinition[0]; }
        }
    }
}