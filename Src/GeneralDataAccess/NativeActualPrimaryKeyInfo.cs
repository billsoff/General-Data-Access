#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NativeActualPrimaryKeyInfo.cs
// 文件功能描述：真实原生主键信息。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110725
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
    internal sealed class NativeActualPrimaryKeyInfo : NativePrimaryKeyInfo
    {
        #region 私有字段

        private readonly EntityDefinition m_definition;
        private readonly EntityPropertyDefinition m_nativePrimaryKey;
        private readonly NativeAttribute m_nativeAttribute;
        private readonly EntityPropertyDefinition[] m_candidateKeys;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，设置实体定义、原生主键属性定义和候选键属性定义集合。
        /// </summary>
        /// <param name="definition">实体定义。</param>
        /// <param name="nativePrimaryKey">原生主键属性定义。</param>
        /// <param name="candidateKeys">候选键集合。</param>
        public NativeActualPrimaryKeyInfo(EntityDefinition definition, EntityPropertyDefinition nativePrimaryKey, EntityPropertyDefinition[] candidateKeys)
        {
            m_definition = definition;
            m_nativePrimaryKey = nativePrimaryKey;
            m_nativeAttribute = (NativeAttribute)Attribute.GetCustomAttribute(nativePrimaryKey.PropertyInfo, typeof(NativeAttribute));
            m_candidateKeys = candidateKeys;
        }

        #endregion

        /// <summary>
        /// 获取实体定义。
        /// </summary>
        public override EntityDefinition Definition
        {
            get { return m_definition; }
        }

        /// <summary>
        /// 总是返回 true。
        /// </summary>
        public override Boolean IsNative
        {
            get { return true; }
        }

        /// <summary>
        /// 返回一个值，该值指示主键是否为自增长（标识）字段。
        /// </summary>
        public override Boolean AutoIncrement
        {
            get { return m_nativeAttribute.AutoIncrement; }
        }

        /// <summary>
        /// 获取用于获取自增长（标识）字段的值的 SQL 指令。
        /// </summary>
        public override String RetrieveIdentifierStatement
        {
            get { return m_nativeAttribute.RetrieveIdentifierStatement; }
        }

        /// <summary>
        /// 获取原生主键属性定义。
        /// </summary>
        public override EntityPropertyDefinition NativePrimaryKey
        {
            get { return m_nativePrimaryKey; }
        }

        /// <summary>
        /// 获取候选主键属性定义集合。
        /// </summary>
        public override EntityPropertyDefinition[] CandidateKeys
        {
            get { return m_candidateKeys; }
        }
    }
}