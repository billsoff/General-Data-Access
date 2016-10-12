#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NativePrimaryKeyLoader.cs
// 文件功能描述：用于为实体加载数据库生成的主键。
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
using System.Diagnostics;
using System.Text;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 用于为实体加载数据库生成的主键。
    /// </summary>
    internal sealed class NativePrimaryKeyLoader
    {
        #region 私有字段

        private readonly Object m_entity;
        private readonly EntityDefinition m_definition;
        private readonly IDatabaseSession m_databaseSession;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，设置实体、定义和数据库会话引擎。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <param name="databaseSession">数据库会话引擎。</param>
        internal NativePrimaryKeyLoader(Object entity, IDatabaseSession databaseSession)
        {
            m_entity = entity;
            m_definition = EntityDefinitionBuilder.Build(entity.GetType()); ;
            m_databaseSession = databaseSession;

            // TODO: 取消自动生候选主键的值
            //m_definition.NativePrimaryKeyInfo.GenerateCandidateKeyValue(entity);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 加载主键值。
        /// </summary>
        public void Load()
        {
            m_definition.NativePrimaryKeyInfo.LoadPrimaryKeyValue(m_entity, m_databaseSession);
        }

        #endregion
    }
}