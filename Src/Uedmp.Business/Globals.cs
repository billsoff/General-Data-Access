#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：Globals.cs
// 文件功能描述：提供工具方法。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120320
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using Uedmp.Entity;
using Useease.GeneralDataAccess;

namespace Uedmp.Business
{
    /// <summary>
    /// 提供工具方法。
    /// </summary>
    public static class Globals
    {
        private const string CURRENT_USER_KEY = "CurrentUser";

        /// <summary>
        /// 获取当前登录用户。
        /// </summary>
        public static EtyUser CurrentUser
        {
            get
            {
                HttpContext current = HttpContext.Current;

                if (current.User.Identity.IsAuthenticated)
                {
                    if (current.Items[CURRENT_USER_KEY] == null)
                    {
                        current.Items[CURRENT_USER_KEY] = UserUtility.GetUserByAccount(current.User.Identity.Name);
                    }
                }
                else
                {
                    current.Items[CURRENT_USER_KEY] = null;
                }

                return (EtyUser)current.Items[CURRENT_USER_KEY];
            }
        }

        /// <summary>
        /// 注销用户。
        /// </summary>
        public static void LoggedOut()
        {
            HttpContext current = HttpContext.Current;

            current.Session[CURRENT_USER_KEY] = null;
        }

        /// <summary>
        /// 显示消息提示。
        /// </summary>
        /// <param name="page">页面。</param>
        /// <param name="msg">消息。</param>
        public static void ShowMessage(Page page, string msg)
        {
            msg = EncodeMessage(msg);

            page.ClientScript.RegisterStartupScript(page.GetType(), "Message", String.Format("alert('{0}');", msg), true);
        }

        /// <summary>
        /// 显示消息提示。
        /// </summary>
        /// <param name="page">页面。</param>
        /// <param name="format">格式串。</param>
        /// <param name="args">参数列表。</param>
        public static void ShowMessage(Page page, string format, params object[] args)
        {
            string msg = String.Format(format, args);

            ShowMessage(page, msg);
        }

        /// <summary>
        /// 转义消息中的反斜杠、单引号和换行，以在消息框中呈现。
        /// </summary>
        /// <param name="msg">消息。</param>
        /// <returns>转义后的字符串。</returns>
        public static string EncodeMessage(string msg)
        {
            msg = msg.Replace(@"\", @"\\");
            msg = msg.Replace("'", @"\'");
            msg = msg.Replace("\r", @"\r");
            msg = msg.Replace("\n", @"\n");

            return msg;
        }

        public static void DownloadAttachment(Page page, string physicalFilePath)
        {
            string fileName = Path.GetFileName(physicalFilePath);

            DownloadAttachment(page, physicalFilePath, fileName);
        }

        public static void DownloadAttachment(Page page, string physicalFilePath, string originalFileName)
        {
            if (!File.Exists(physicalFilePath))
            {
                ShowMessage(page, "非常抱歉，您要下载的文件不存在。");

                return;
            }

            FileInfo info = new FileInfo(physicalFilePath);
            long fileLength = info.Length;

            HttpResponse response = page.Response;

            response.ContentType = "application/octet-stream";

            response.AppendHeader(
                    "Content-Disposition",
                    String.Format("attachment;filename={0}", HttpUtility.UrlEncode(originalFileName))
                );
            response.AppendHeader("Content-Length", fileLength.ToString());

            response.TransmitFile(physicalFilePath);
        }
    }
}