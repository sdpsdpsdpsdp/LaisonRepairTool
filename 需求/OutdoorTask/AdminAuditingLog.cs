using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaisonTech.CommonBLL;

namespace LaisonTech.LAPIS.ObjectDefine
{
    /// <summary>
    /// 2017/08/02 hwy
    /// 审批状态
    /// </summary>
    public enum eAuditingResult
    {
        /// <summary>
        /// 未审核
        /// </summary>
        NoAuditing = 0,
        /// <summary>
        /// 通过
        /// </summary>
        Pass = 1,
        /// <summary>
        /// 驳回
        /// </summary>
        Reject = 2,
    }

    /// <summary>
    /// 2017/08/02 hwy
    /// admin评审记录
    /// </summary>
    [Serializable]
    public class AdminAuditingLog : ObjectInfoBase
    {
        public AdminAuditingLog()
        {
        }

        /// <summary>
        /// 记录ID
        /// </summary>
        public Int32 LogID = 0;

        /// <summary>
        /// 评审类型--任务类型
        /// </summary>
        public Int32 AuditingType = (Int32)eTaskType.Refund;

        /// <summary>
        /// 需评审的任务ID
        /// </summary>
        public Int32 TaskID = 0;

        /// <summary>
        /// 请求admin审批的操作员ID
        /// </summary>
        public Int32 OperatorID = 0;

        /// <summary>
        /// 事件描述
        /// </summary>
        public String Description = String.Empty;

        /// <summary>
        /// admin审批状态
        /// </summary>
        public Int32 AuditingState = (Int32)eAuditingResult.NoAuditing;

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime AuditingTime = CommonCompute.Const_InvalidDate;

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime CreatTime = CommonCompute.Const_InvalidDate;
    }

    /// <summary>
    /// 2017/09/06 hwy
    /// 查询条件：admin审批记录
    /// </summary>
    [Serializable]
    public class QueryCondition_AdminAuditingLog : ObjectInfoBase
    {
        /// <summary>
        /// 是否查询全部任务类型
        /// </summary>
        public Boolean IsQueryAllTaskType = false;

        /// <summary>
        /// 是否根据操作员查询
        /// </summary>
        public Boolean IsQueryOperator = false;

        /// <summary>
        /// 待查询的任务类型
        /// </summary>
        public Int32 AuditingType = (Int32)eTaskType.Refund;

        /// <summary>
        /// 操作员ID
        /// </summary>
        public Int32 OperatorID = 0;

        /// <summary>
        /// 只查询未审核的记录
        /// </summary>
        public Boolean OnlyQueryUnAuditingLog = false;

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartTime = CommonCompute.Const_InvalidDate;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime = CommonCompute.Const_InvalidDate;
    }

}
