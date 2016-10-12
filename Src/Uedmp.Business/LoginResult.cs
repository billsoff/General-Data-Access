#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：LoginResult.cs
// 文件功能描述：登录结果。
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
using System.Linq;
using System.Text;

using Uedmp.Entity;

namespace Uedmp.Business
{
    /// <summary>
    /// 登录结果。
    /// </summary>
    [Serializable]
    public sealed class LoginResult
    {
        private readonly bool _Success;
        private readonly string _ErrorMessage;
        private readonly EtyUser _User;

        internal LoginResult(EtyUser user)
        {
            _Success = true;
            _User = user;
        }

        internal LoginResult(string errorMessage)
        {
            _Success = false;
            _ErrorMessage = errorMessage;
        }

        public bool Success
        {
            get { return _Success; }
        }

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
        }

        public EtyUser User
        {
            get { return _User; }
        }
    }
}