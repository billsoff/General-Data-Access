#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：EntityAdditionalMembers.cs
// 文件功能描述：为实体类提供附加的属性。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120331
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
using Useease.GeneralDataAccess;

namespace Uedmp.Entity
{
    #region 项目需求实体

    partial class EtyDemand
    {
        #region 需求内容

        private EtyDemandVersion _DemandInfo;
        private bool _IsDemandInfoSet;

        /// <summary>
        /// 获取或设置项目内容。
        /// </summary>
        public EtyDemandVersion DemandInfo
        {
            get
            {
                if (!_IsDemandInfoSet)
                {
                    if (this.AllVersions.Length != 0)
                    {
                        _DemandInfo = this.AllVersions[0];
                    }

                    _IsDemandInfoSet = true;
                }

                return _DemandInfo;
            }

            set
            {
                _DemandInfo = value;
                _IsDemandInfoSet = true;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示需求内容是否已被设置。
        /// </summary>
        public bool IsDemandInfoSet
        {
            get { return _IsDemandInfoSet; }
        }

        /// <summary>
        /// 获取需求所有的版本，以版本号降序排列。
        /// </summary>
        [Children(EtyDemandVersion.PROP_DEMAND)]
        [OrderBy(EtyDemandVersion.PROP_DEMAND_VERSION, 1, SortMethod = SortMethod.Descending)]
        public EtyDemandVersion[] AllVersions
        {
            get { return GetChildren<EtyDemandVersion>(PROP_ALL_VERSIONS); }
            set { SetChildren(PROP_ALL_VERSIONS, value); }
        }

        public const string PROP_ALL_VERSIONS = "AllVersions";

        #endregion

        #region 需求内容

        /// <summary>
        /// 获取需求内容。
        /// </summary>
        public string DemandContent
        {
            get
            {
                return (DemandInfo != null) ? DemandInfo.DemandContent : null;
            }
        }

        /// <summary>
        /// 获取需求重要性。
        /// </summary>
        public EtyDemandImportance DemandImportance
        {
            get { return (DemandInfo != null) ? DemandInfo.DemandImportance : null; }
        }

        /// <summary>
        /// 获取重要性显示名称。
        /// </summary>
        public string ImportanceDisplayName
        {
            get { return (DemandImportance != null) ? DemandImportance.DemandImportanceDisplayName : null; }
        }

        /// <summary>
        /// 获取需求的优先极
        /// </summary>
        public EtyDemandPriority DemandPriority
        {
            get { return (DemandInfo != null) ? DemandInfo.DemandPriority : null; }
        }

        /// <summary>
        /// 获取需求优先级显示名称。
        /// </summary>
        public string PriorityDisplayName
        {
            get { return (DemandPriority != null) ? DemandPriority.DemandPriorityDisplayName : null; }
        }

        /// <summary>
        /// 获取需求方。
        /// </summary>
        public string DemandSide
        {
            get { return (DemandInfo != null) ? DemandInfo.DemandSide : null; }
        }

        /// <summary>
        /// 获取需求标题。
        /// </summary>
        public string DemandTitle
        {
            get { return (DemandInfo != null) ? DemandInfo.DemandTitle : null; }
        }

        /// <summary>
        /// 获取需求提交者。
        /// </summary>
        public string CreatorDisplayName
        {
            get { return (DemandInfo != null) ? DemandInfo.Creator.Name : null; }
        }

        #endregion
    }

    #endregion

    #region 工作任务实体

    partial class EtyTask
    {
        /// <summary>
        /// 获取分配者名称。
        /// </summary>
        public string AssignerName { get { return TaskAssigner.Name; } }

        /// <summary>
        /// 获取工作者名称。
        /// </summary>
        public string WorkerName { get { return TaskWorker.Name; } }

        /// <summary>
        /// 获取状态显示名称。
        /// </summary>
        public string StateDisplayName { get { return TaskState.TaskStateDisplayName; } }
    }

    #endregion
}