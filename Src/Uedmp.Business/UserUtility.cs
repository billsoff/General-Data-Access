#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：UserUtility.cs
// 文件功能描述： 用户工具类。
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
using System.ComponentModel;
using System.Linq;
using System.Text;

using Useease.GeneralDataAccess;
using Uedmp.Entity;

namespace Uedmp.Business
{
    /// <summary>
    /// 用户工具类。
    /// </summary>
    [DataObject]
    public static class UserUtility
    {
        public static LoginResult LoginUser(string account, string password)
        {
            EtyUser user = GetUserByAccount(account);

            if ((user == null) || (user.LoginPwd != password))
            {
                return new LoginResult("用户帐号或密码错误。");
            }
            else
            {
                return new LoginResult(user);
            }
        }

        /// <summary>
        /// 获取具有指定帐号用户。
        /// </summary>
        /// <param name="account">帐号。</param>
        /// <returns>具有该帐号的用户，如果未找到，则返回 null。</returns>
        public static EtyUser GetUserByAccount(string account)
        {
            return DbSessions.Default.LoadFirst<EtyUser>(Where.Property(Entities.User.LoginName).Is.EqualTo(account));
        }

        /// <summary>
        /// 获取具有指定唯一标识的用户。
        /// </summary>
        /// <param name="userId">用户的唯一标识。</param>
        /// <returns>具有该唯一标识的用户，如果未找到，则返回 null。</returns>
        public static EtyUser GetUserById(int userId)
        {
            return DbSessions.Default.LoadFirst<EtyUser>(
                    Where.Property(Entities.User.UserId).Is.EqualTo(userId)
                );
        }

        /// <summary>
        /// 获取所有的用户。
        /// </summary>
        /// <returns>所有的用户。</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static EtyUser[] GetAllUsers()
        {
            return DbSessions.Default.Load<EtyUser>(
                    OrderBy.Property(Entities.User.Name)
                );
        }
    }
}