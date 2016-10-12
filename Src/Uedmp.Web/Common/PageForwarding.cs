#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：PageForwarding.cs
// 文件功能描述：指示进行页面跳转。
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

namespace Uedmp.Web.Common
{
    /// <summary>
    /// 指示进行页面跳转。
    /// </summary>
    public static class PageForwarding
    {
        /// <summary>
        /// 跳转到默认页面。
        /// </summary>
        public static void GoToAuthDefault()
        {
            HttpContext.Current.Response.Redirect("~/Auth/Default.aspx");
        }

        /// <summary>
        /// 转到项目管理页面。
        /// </summary>
        public static void GoToProjectManage()
        {
            HttpContext.Current.Response.Redirect("~/Auth/Project/ProjectManage.aspx");
        }

        /// <summary>
        /// 跳转到需求管理页面。
        /// </summary>
        /// <param name="projectId"></param>
        public static void GoToDemandManage(int projectId)
        {
            HttpContext.Current.Response.Redirect(
                    String.Format("~/Auth/Project/DemandManage.aspx?ProjectID={0}", projectId)
                );
        }

        /// <summary>
        /// 跳转到任务管理页面。
        /// </summary>
        public static void GoToTaskManage()
        {
            HttpContext.Current.Response.Redirect("~/Auth/Task/TaskManage.aspx");
        }
    }
}