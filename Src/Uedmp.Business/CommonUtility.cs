#region 版权及版本变化申明
/*-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2012 广州优亿信息科技有限公司
// 版权所有
// 
//
// 文件名：CommonUtility.cs
// 文件功能描述：提供通用的业务功能。
//
//
// 创建标识：宋冰（13660481521@139.com） 20120401
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
    /// 提供通用的业务功能。
    /// </summary>
    [DataObject]
    public static class CommonUtility
    {
        /// <summary>
        /// 获取所有的需求优先级。
        /// </summary>
        /// <returns>所有的需求优先级。</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static EtyDemandPriority[] GetAllDemandPriorities()
        {
            return DbSessions.Default.Load<EtyDemandPriority>(OrderBy.Property(Entities.DemandPriority.DemandPriorityID));
        }

        /// <summary>
        /// 获取所有的需求重要性。
        /// </summary>
        /// <returns>所有的需求的重要性。</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static EtyDemandImportance[] GetAllDemandImportances()
        {
            return DbSessions.Default.Load<EtyDemandImportance>(OrderBy.Property(Entities.DemandImportance.DemandImportanceID));
        }

        /// <summary>
        /// 获取所有的任务状态。
        /// </summary>
        /// <returns>所有的任务状态。</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static EtyTaskState[] GetAllTaskStates()
        {
            return DbSessions.Default.Load<EtyTaskState>(OrderBy.Property(Entities.TaskState.TaskStateID));
        }

        /// <summary>
        /// 获取指定唯一标识的需求优先级。
        /// </summary>
        /// <param name="priorityId">需求优先级的唯一标识。</param>
        /// <returns>具有该标识的需求优先级，如果没有找到，则返回 null。</returns>
        public static EtyDemandPriority GetDemandPriorityById(int priorityId)
        {
            return DbSessions.Default.LoadFirst<EtyDemandPriority>(
                    Where.Property(Entities.DemandPriority.DemandPriorityID).Is.EqualTo(priorityId)
                );
        }

        /// <summary>
        /// 获取具有指定唯一标识的需求重要性。
        /// </summary>
        /// <param name="importanceId">需求重要性的唯一标识。</param>
        /// <returns>具有该唯一标识的需求重要性，如果没有找到，则返回 null。</returns>
        public static EtyDemandImportance GetDemandImportanceById(int importanceId)
        {
            return DbSessions.Default.LoadFirst<EtyDemandImportance>(
                    Where.Property(Entities.DemandImportance.DemandImportanceID).Is.EqualTo(importanceId)
                );
        }

        /// <summary>
        /// 获取具有指定唯一标识的任务状态。
        /// </summary>
        /// <param name="stateId">任务状态的唯一标识。</param>
        /// <returns>具有该唯一标识的任务状态，如果不存在，则返回 null。</returns>
        public static EtyTaskState GetTaskStateById(int stateId)
        {
            return DbSessions.Default.LoadFirst<EtyTaskState>(
                    Where.Property(Entities.TaskState.TaskStateID).Is.EqualTo(stateId)
                );
        }

        /// <summary>
        /// 获取小时数集合。
        /// </summary>
        /// <returns>小时数集合。</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static int[] GetHours()
        {
            int[] hours = new int[24];

            for (int i = 0; i < hours.Length; i++)
            {
                hours[i] = i;
            }

            return hours;
        }

        /// <summary>
        /// 获取分钟数。
        /// </summary>
        /// <returns>分钟数集合。</returns>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public static int[] GetMinutes()
        {
            int[] minutes = new int[60];

            for (int i = 0; i < minutes.Length; i++)
            {
                minutes[i] = i;
            }

            return minutes;
        }
    }
}