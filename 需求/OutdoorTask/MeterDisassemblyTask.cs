using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaisonTech.CommonBLL;
using LaisonTech.STSDLLCaller;

namespace LaisonTech.LAPIS.ObjectDefine
{
    /// <summary>
    /// 2017/07/26 hwy
    /// 表计拆装任务，分为换表和销户任务，以TaskType区分
    /// </summary>
    [Serializable]
    public class MeterDisassemblyTaskBase : ObjectInfoBase
    {
        public MeterDisassemblyTaskBase()
        {
        }
        public void CopyFrom(MeterDisassemblyTaskBase src)
        {
            if (src == null)
            {
                return;
            }
            TaskID = src.TaskID;
            TechnicianID = src.TechnicianID;
            CustomerID = src.CustomerID;
            OperatorID = src.OperatorID;

            TaskType = src.TaskType;
            TaskState = src.TaskState;
            TaskResult = src.TaskResult;

            Feedback = src.Feedback;
            MeterStatue = src.MeterStatue;
            TotalComsumedValue = src.TotalComsumedValue;
            RemainValue = src.RemainValue;

            CreatTime = src.CreatTime;
            FinishTime = src.FinishTime;

            OldMeterTotalDialReading = src.OldMeterTotalDialReading;
        }

        /// <summary>
        /// 任务ID
        /// </summary>
        public Int32 TaskID = 0;

        /// <summary>
        /// 上门操作员ID
        /// </summary>
        public Int32 TechnicianID = 0;

        /// <summary>
        /// 用户ID
        /// </summary>
        public Int32 CustomerID = 0;

        /// <summary>
        /// LAPIS执行操作员ID
        /// </summary>
        public Int32 OperatorID = 0;

        /// <summary>
        /// 任务类型：这里只用到换表和销户
        /// </summary>
        public Int32 TaskType = (Int32)eTaskType.ReplaceMeter;

        /// <summary>
        /// 任务状态 eTaskState:Inprocessing=0,Finish=1
        /// </summary>
        public Int32 TaskState = (Int32)eTaskState.Inprocessing;

        /// <summary>
        /// 任务结果
        /// </summary>
        public Int32 TaskResult = (Int32)eTaskResult.Unknown;

        /// <summary>
        /// 工程师任务结束反馈信息
        /// </summary>
        public String Feedback = String.Empty;

        /// <summary>
        /// 表计状态，false：损坏，true：正常
        /// </summary>
        public Boolean MeterStatue = false;

        /// <summary>
        /// 表计总使用量/金额
        /// </summary>
        public Double TotalComsumedValue = 0;

        /// <summary>
        /// 表计剩余量/金额
        /// </summary>
        public Double RemainValue = 0;

        /// <summary>
        /// 任务创建日期
        /// </summary>
        public DateTime CreatTime = CommonCompute.Const_InvalidDate;

        /// <summary>
        /// 操作员审核日期
        /// </summary>
        public DateTime FinishTime = CommonCompute.Const_InvalidDate;

        /// <summary>
        /// 上个表计的机械总使用量
        /// 从换表中上个表计的电子读数00000010标识或总机械读数
        /// </summary>
        /// <returns></returns>
        public Double OldMeterTotalDialReading = 0;

    }

    /// <summary>
    /// 2017-07-27 lijia
    /// 换表/销户的任务文件的具体内容
    /// </summary>
    [Serializable]
    public class MeterDisassemblyTaskInfo : MeterDisassemblyTaskBase
    {
        public void CopyFrom(MeterDisassemblyTaskInfo src)
        {
            base.CopyFrom((MeterDisassemblyTaskBase)src);
            MeterAddress = src.MeterAddress;

            AreaCode = src.AreaCode;
            MeterProtocolType = src.MeterProtocolType;
            DecimalParameter = src.DecimalParameter;

            CustomerNumber = src.CustomerNumber;
            CustomerName = src.CustomerName;
            CustomerTelephone = src.CustomerTelephone;
            CustomerAddress = src.CustomerAddress;
            ServicemenName = src.ServicemenName;

            SetMeterKeyParam = src.SetMeterKeyParam;
        }

        /// <summary>
        /// 表地址
        /// </summary>
        public String MeterAddress = String.Empty;

        /// <summary>
        /// AreaCode
        /// </summary>
        public Int32 AreaCode = 0;

        /// <summary>
        /// 表计通信协议类型，eMeterProtocolType
        /// </summary>
        public eMeterProtocolType MeterProtocolType = eMeterProtocolType.Invalid;

        /// <summary>
        /// 精度参数
        /// 格式为：脉冲当量小数位数;计量模式;价格精度小数位数;结算精度小数位数;是否显示K
        /// </summary>
        public String DecimalParameter = String.Empty;

        /// <summary>
        /// 用户信息
        /// </summary>
        public String CustomerNumber = String.Empty;
        public String CustomerName = String.Empty;
        public String CustomerTelephone = String.Empty;
        public String CustomerAddress = String.Empty;

        /// <summary>
        /// 维修人员姓名
        /// </summary>
        public String ServicemenName = String.Empty;

        /// <summary>
        /// STS卡表使用的制用户卡密钥
        /// 仅STS卡表在退购时开启或关闭红外使用
        /// </summary>
        public String SetMeterKeyParam = String.Empty;
    }

    /// <summary>
    /// 2017-07-27 lijia
    /// 换表/销户的任务文件结果的具体内容
    /// </summary>
    public class MeterDisassemblyTaskResultInfo : MeterDisassemblyTaskBase
    {
        public void CopyFrom(MeterDisassemblyTaskResultInfo src)
        {
            base.CopyFrom((MeterDisassemblyTaskBase)src);
            MeterAddress = src.MeterAddress;
            OperateFinishTime = src.OperateFinishTime;
        }

        /// <summary>
        /// 表地址
        /// </summary>
        public String MeterAddress = String.Empty;

        /// <summary>
        /// 操作完成日期
        /// </summary>
        public DateTime OperateFinishTime = CommonCompute.Const_InvalidDate;

    }
}
