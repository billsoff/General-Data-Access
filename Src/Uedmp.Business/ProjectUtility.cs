#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：ProjectUtility.cs
// 文件功能描述：项目工具类。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120401
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Uedmp.Entity;
using Useease.GeneralDataAccess;

namespace Uedmp.Business
{
    /// <summary>
    /// 项目工具类。
    /// </summary>
    [DataObject]
    public static class ProjectUtility
    {
        /// <summary>
        /// 获取指定唯一标识的项目。
        /// </summary>
        /// <param name="projectId">指定的项目唯一标识。</param>
        /// <returns>项目，如果没有找到，则返回 null。</returns>
        public static EtyProject GetProjectById(int projectId)
        {
            return DbSessions.Default.LoadFirst<EtyProject>(Where.Property(Entities.Project.ProjectId).Is.EqualTo(projectId));
        }

        /// <summary>
        /// 获取所有的项目。
        /// </summary>
        /// <returns>所有的项目。</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static EtyProject[] GetAllProjects()
        {
            return DbSessions.Default.Load<EtyProject>();
        }
    }
}