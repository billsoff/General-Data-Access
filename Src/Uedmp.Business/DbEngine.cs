#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：DbEngine.cs
// 文件功能描述：数据库会话引擎。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120312
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

using Useease.GeneralDataAccess;

namespace Uedmp.Business
{
    #region 数据库会话引擎与实体工厂

    /// <summary>
    /// 数据库会话集合。
    /// </summary>
    public static class DbSessions
    {
        #region 公共属性

        /// <summary>
        /// 获取默认的数据库会话引擎。
        /// </summary>
        public static IDatabaseSession Default
        {
            get { return EntityManagers.Default; }
        }

        #endregion
    }

    /// <summary>
    /// 实体工厂集合。
    /// </summary>
    public static class EntityFactories
    {
        #region 公共属性

        /// <summary>
        /// 获取默认的数据库工厂。
        /// </summary>
        public static IEntityFactory Default
        {
            get { return EntityManagers.Default; }
        }

        #endregion
    }

    /// <summary>
    /// 实体管理器集合。
    /// </summary>
    internal static class EntityManagers
    {
        #region 私有字段

        private static readonly EntityManager m_default;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化管理器实例。
        /// </summary>
        static EntityManagers()
        {
            DatabaseSession dbSession = new DatabaseSession();

            m_default = new GlobalEntityManager(dbSession);
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取默认的实体管理。
        /// </summary>
        public static EntityManager Default
        {
            get { return EntityManagers.m_default; }
        }

        #endregion
    }

    /// <summary>
    /// 全局实体管理器。
    /// </summary>
    internal sealed class GlobalEntityManager : EntityManager
    {
        #region 构造函数

        /// <summary>
        /// 构造函数，设置底层数据库会话引擎。
        /// </summary>
        /// <param name="databaseSession">底层数据库会话引擎。</param>
        public GlobalEntityManager(IDatabaseSession databaseSession)
            : base(databaseSession)
        {
        }

        #endregion
    }

    #endregion
}