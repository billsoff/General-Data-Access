#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyUser.cs
// 文件功能描述：用户表。
//
//
// 创建标识：模板创建 2012/4/1 18:59:33
//
// 修改标识：
// 修改描述：
//
// --------------------------------------------------------------------------------------------------------*/
#endregion 版权及版本变化申明

using System;
using System.Data;

using Useease.GeneralDataAccess;

namespace Uedmp.Entity
{
    /// <summary>
    /// 用户表
    /// </summary>
    [Serializable, Table("T_User")]
    public sealed  partial class EtyUser : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyUser()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 用户ID
        /// </summary>
        [Column("User_Id", DbType.Int32, IsPrimaryKey = true), Native]
        public int UserId { get; private set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public const string PROP_USER_ID = "UserId";

		/// <summary>
        /// 登录帐号
        /// </summary>
        [Column("Login_Name", DbType.AnsiString)]
        public string LoginName { get; set; }

        /// <summary>
        /// 登录帐号
        /// </summary>
        public const string PROP_LOGIN_NAME = "LoginName";

		/// <summary>
        /// 登录密码,最好进行MD5加密
        /// </summary>
        [Column("Login_Pwd", DbType.AnsiString)]
        public string LoginPwd { get; set; }

        /// <summary>
        /// 登录密码,最好进行MD5加密
        /// </summary>
        public const string PROP_LOGIN_PWD = "LoginPwd";

		/// <summary>
        /// 创建时间
        /// </summary>
        [Column("Create_Time", DbType.DateTime)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public const string PROP_CREATE_TIME = "CreateTime";

		/// <summary>
        /// 用户姓名
        /// </summary>
        [Column("Name", DbType.AnsiString)]
        public string Name { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public const string PROP_NAME = "Name";

		/// <summary>
        /// 员工编号
        /// </summary>
        [Column("Serial_Num", DbType.AnsiString)]
        public string SerialNum { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public const string PROP_SERIAL_NUM = "SerialNum";

		/// <summary>
        /// 状态：0正常 1待审核  2暂停 3已注销
        /// </summary>
        [Column("Status", DbType.Int32)]
        public int Status { get; set; }

        /// <summary>
        /// 状态：0正常 1待审核  2暂停 3已注销
        /// </summary>
        public const string PROP_STATUS = "Status";

		/// <summary>
        /// 性别:0男 1女
        /// </summary>
        [Column("Sex", DbType.Int32)]
        public int Sex { get; set; }

        /// <summary>
        /// 性别:0男 1女
        /// </summary>
        public const string PROP_SEX = "Sex";

		/// <summary>
        /// 公司邮箱
        /// </summary>
        [Column("Email", DbType.AnsiString)]
        public string Email { get; set; }

        /// <summary>
        /// 公司邮箱
        /// </summary>
        public const string PROP_EMAIL = "Email";

		/// <summary>
        /// 住址
        /// </summary>
        [Column("Address", DbType.AnsiString)]
        public string Address { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        public const string PROP_ADDRESS = "Address";

		/// <summary>
        /// 备注
        /// </summary>
        [Column("Remark", DbType.AnsiString)]
        public string Remark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public const string PROP_REMARK = "Remark";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyUser))]
        public sealed  class UserPropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 用户ID
            /// </summary>
            public IPropertyChain UserId { get { return LinkPrimitiveProperty(PROP_USER_ID); } }
	
            /// <summary>
            /// 登录帐号
            /// </summary>
            public IPropertyChain LoginName { get { return LinkPrimitiveProperty(PROP_LOGIN_NAME); } }
	
            /// <summary>
            /// 登录密码,最好进行MD5加密
            /// </summary>
            public IPropertyChain LoginPwd { get { return LinkPrimitiveProperty(PROP_LOGIN_PWD); } }
	
            /// <summary>
            /// 创建时间
            /// </summary>
            public IPropertyChain CreateTime { get { return LinkPrimitiveProperty(PROP_CREATE_TIME); } }
	
            /// <summary>
            /// 用户姓名
            /// </summary>
            public IPropertyChain Name { get { return LinkPrimitiveProperty(PROP_NAME); } }
	
            /// <summary>
            /// 员工编号
            /// </summary>
            public IPropertyChain SerialNum { get { return LinkPrimitiveProperty(PROP_SERIAL_NUM); } }
	
            /// <summary>
            /// 状态：0正常 1待审核  2暂停 3已注销
            /// </summary>
            public IPropertyChain Status { get { return LinkPrimitiveProperty(PROP_STATUS); } }
	
            /// <summary>
            /// 性别:0男 1女
            /// </summary>
            public IPropertyChain Sex { get { return LinkPrimitiveProperty(PROP_SEX); } }
	
            /// <summary>
            /// 公司邮箱
            /// </summary>
            public IPropertyChain Email { get { return LinkPrimitiveProperty(PROP_EMAIL); } }
	
            /// <summary>
            /// 住址
            /// </summary>
            public IPropertyChain Address { get { return LinkPrimitiveProperty(PROP_ADDRESS); } }
	
            /// <summary>
            /// 备注
            /// </summary>
            public IPropertyChain Remark { get { return LinkPrimitiveProperty(PROP_REMARK); } }

            #endregion
        }

        #endregion
    }
    
    /// <summary>
    /// 实体
    /// </summary>
    partial class Entities
    {
		/// <summary>
        /// 用户表
        /// </summary>
        public static EtyUser.UserPropertyDescriptor User { get { return new EtyUser.UserPropertyDescriptor(); } }
    }
}