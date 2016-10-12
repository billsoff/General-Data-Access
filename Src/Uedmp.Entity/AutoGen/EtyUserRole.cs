#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//

// 文件名：EtyUserRole.cs
// 文件功能描述：。
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
    /// 
    /// </summary>
    [Serializable, Table("T_User_Role")]
    public sealed  partial class EtyUserRole : EtyBusinessObject
    {
        #region 构造函数
        /// <summary>
        /// 注意：用户不要直接调用此构造函数，而应通过调用工厂方法创建新实体。
        /// </summary>
        public EtyUserRole()
        {
        }

        #endregion

        #region 公共属性

		/// <summary>
        /// 用户
        /// </summary>
        [ForeignReference, ColumnMapping("User_Id", "User_Id"), PrimaryKey]
        public EtyUser User { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public const string PROP_USER = "User";

		/// <summary>
        /// 角色
        /// </summary>
        [ForeignReference, ColumnMapping("Role_Id", "Role_Id"), PrimaryKey]
        public EtyRole Role { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public const string PROP_ROLE = "Role";

        #endregion


        #region 属性描述符

        /// <summary>
        /// 视频文档属性描述符。
        /// </summary>
        [Serializable, PropertyDescriptor(typeof(EtyUserRole))]
        public sealed  class UserRolePropertyDescriptor : PropertyDescriptor
        {
            #region 公共属性
	
            /// <summary>
            /// 用户
            /// </summary>
			public EtyUser.UserPropertyDescriptor User { get { return LinkForeignReferenceProperty<EtyUser.UserPropertyDescriptor>(PROP_USER); } }
	
            /// <summary>
            /// 角色
            /// </summary>
			public EtyRole.RolePropertyDescriptor Role { get { return LinkForeignReferenceProperty<EtyRole.RolePropertyDescriptor>(PROP_ROLE); } }

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
        /// 
        /// </summary>
        public static EtyUserRole.UserRolePropertyDescriptor UserRole { get { return new EtyUserRole.UserRolePropertyDescriptor(); } }
    }
}