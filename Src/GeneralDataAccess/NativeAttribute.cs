#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：NativeAttribute.cs
// 文件功能描述：标记于主键属性上，指示该主键由数据库生成，只能应用于只有一个非引用的主键的实体。
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
using System.Text;

namespace Useease.GeneralDataAccess
{
    /// <summary>
    /// 标记于主键属性上，指示该主键由数据库生成，只能应用于只有一个非引用的主键的实体。
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class NativeAttribute : Attribute
    {
        #region 私有字段

        private Boolean m_autoIncrement = true;
        private String m_retrieveIdentifierStatement;

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取或设置一个值，该值指示主键是否为自增长字段，默认值为 true。
        /// </summary>
        public Boolean AutoIncrement
        {
            get { return m_autoIncrement; }
            set { m_autoIncrement = value; }
        }

        /// <summary>
        /// 获取或设置用于获取自增长（标识）字段值的 SQL 指令。
        /// </summary>
        public String RetrieveIdentifierStatement
        {
            get { return m_retrieveIdentifierStatement; }

            set
            {
                if (value != null)
                {
                    value = value.Trim();
                    value = value.TrimEnd(new Char[] { ';' });
                }

                m_retrieveIdentifierStatement = value;
            }
        }

        #endregion
    }
}