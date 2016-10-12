#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntitySuppressLazyLoadingTransient.cs
// 文件功能描述：暂时抑制延迟加载，当此对象被 Dispose 时立刻恢复实体 SuppressLazyLoad 原来的值。
//
//
// 创建标识：宋冰（billsoff@gmail.com） 20110413
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
    /// 暂时抑制延迟加载，当此对象被 Dispose 时立刻恢复实体 SuppressLazyLoad 原来的值。
    /// </summary>
    internal sealed class EntitySuppressLazyLoadingTransient : IDisposable
    {
        #region 私有字段

        private readonly EtyBusinessObject m_target;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，设置要抑制延迟加载的实体。
        /// </summary>
        /// <param name="entity">目标实体。</param>
        public EntitySuppressLazyLoadingTransient(Object entity)
            : this(entity as EtyBusinessObject)
        {
        }

        /// <summary>
        /// 构造函数，设置要抑制延迟加载的 EtyBusinessObject 对象。
        /// </summary>
        /// <param name="bo"></param>
        public EntitySuppressLazyLoadingTransient(EtyBusinessObject bo)
        {
            if ((bo != null) && !bo.SuppressLazyLoad)
            {
                bo.SuppressLazyLoad = true;

                m_target = bo;
            }
            else
            {
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 处置，取消抑制延迟加载。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 实施处置。
        /// </summary>
        /// <param name="disposing">指示是否为显式处置。</param>
        private void Dispose(Boolean disposing)
        {
            if (m_target != null)
            {
                m_target.SuppressLazyLoad = false;

                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        /// <summary>
        /// 终结器。
        /// </summary>
        ~EntitySuppressLazyLoadingTransient()
        {
            Dispose(false);
        }
    }
}