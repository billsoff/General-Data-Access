#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：QueryStringKeys.cs
// 文件功能描述：提供查询字符串的键名称。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120405
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using Uedmp.Business;
using Uedmp.Entity;

namespace Uedmp.Web.Common
{
    /// <summary>
    /// 提供查询字符串的键名称。
    /// </summary>
    public static class QueryStringKeys
    {
        /// <summary>
        /// 项目的唯一标识。
        /// </summary>
        public const string PROJECT_ID = "ProjectID";

        /// <summary>
        /// 获取项目的唯一标识。
        /// </summary>
        /// <param name="page">页面。</param>
        /// <returns>项目的唯一标识。</returns>
        public static int GetProjectId(Page page)
        {
            string projectIdStr = page.Request.QueryString[PROJECT_ID];
            int projectId;

            Int32.TryParse(projectIdStr, out projectId);

            return projectId;
        }

        /// <summary>
        /// 根据查询字符串获取项目。
        /// </summary>
        /// <param name="page">页面。</param>
        /// <returns>项目实体，如果不存在，则返回 null。</returns>
        public static EtyProject GetProject(Page page)
        {
            string projectIdStr = page.Request.QueryString[PROJECT_ID];
            int projectId;

            bool success = Int32.TryParse(projectIdStr, out projectId);

            if (success)
            {
                return ProjectUtility.GetProjectById(projectId);
            }
            else
            {
                return null;
            }
        }
    }
}