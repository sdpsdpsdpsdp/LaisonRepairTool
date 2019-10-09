using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LaisonTech.CommonBLL;
using LaisonTech.DLLCaller;

namespace LaisonTech.LAPIS.ObjectDefine
{
    /// <summary>
    /// 2017/07/26 hwy
    /// 退购任务类
    /// </summary>
    [Serializable]
    public class RefundTask : ObjectInfoBase
    {
        public RefundTask()
        {
        }
        public void CopyFrom(RefundTask src)
        {
            if (src==null)
            {
                return;
            }
            TaskID = src.TaskID;
            RefundTradeID = src.RefundTradeID;
            TechnicianID = src.TechnicianID;
            CustomerID = src.CustomerID;
            OperatorID = src.OperatorID;

            RefundType = src.RefundType;
            TaskState = src.TaskState;

            TIDTestingToken = src.TIDTestingToken;

            Feedback = src.Feedback;
            Remark = src.Remark;

            EnableRefund = src.EnableRefund;
            Auditing = src.Auditing;

            CreatTime = src.CreatTime;
            FinishTime = src.FinishTime;
        }

        /// <summary>
        /// 任务ID
        /// </summary>
        public Int32 TaskID = 0;

        /// <summary>
        /// 退购的交易记录ID
        /// </summary>
        public Int32 RefundTradeID = 0;

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
        /// 任务类型：eRefundTaskType：0：现场退购,1：线下退购
        /// </summary>
        public Int32 RefundType = (Int32)eRefundType.OnSite;

        /// <summary>
        /// 任务状态 eTaskState:Inprocessing=0,Finish=1
        /// </summary>
        public Int32 TaskState = (Int32)eTaskState.Inprocessing;

        /// <summary>
        /// 测试TID的TOKEN，值为0
        /// </summary>
        public String TIDTestingToken = String.Empty;

        /// <summary>
        /// 工程师任务结束反馈信息
        /// </summary>
        public String Feedback = String.Empty;

        /// <summary>
        /// 操作员任务备注，如有特殊情况，需要在这里记录
        /// </summary>
        public String Remark = String.Empty;

        /// <summary>
        /// 是否允许退购
        /// </summary>
        public Boolean EnableRefund = false;

        /// <summary>
        /// 操作员是否已审核
        /// </summary>
        public Boolean Auditing = false;

        /// <summary>
        /// 任务创建日期
        /// </summary>
        public DateTime CreatTime = CommonCompute.Const_InvalidDate;

        /// <summary>
        /// 操作员审核日期
        /// </summary>
        public DateTime FinishTime = CommonCompute.Const_InvalidDate;

    }

    /// <summary>
    /// 2017-08-01 lijia
    /// 退购的任务文件的具体内容
    /// </summary>
    [Serializable]
    public class RefundTaskInfo : RefundTask
    {
        public void CopyFrom(RefundTaskInfo src)
        {
            base.CopyFrom((RefundTask)src);
            MeterAddress = src.MeterAddress;

            RefundValue = src.RefundValue;
            PaymentAmount = src.PaymentAmount;
            PurchaseAmount = src.PurchaseAmount;
            PurchaseQuantity = src.PurchaseQuantity;

            CustomerNumber = src.CustomerNumber;
            CustomerName = src.CustomerName;
            CustomerTelephone = src.CustomerTelephone;
            CustomerAddress = src.CustomerAddress;
            ServicemenName = src.ServicemenName;

            IsCard = src.IsCard;
            ProtocolType = src.ProtocolType;
            DecimalParameter = src.DecimalParameter;
        }
        
        /// <summary>
        /// 表地址
        /// </summary>
        public String MeterAddress = String.Empty;
        /// <summary>
        /// 根据用户走量还是走金额，从交易记录中获取对应的值
        /// </summary>
        public Double RefundValue = 0;

        /// <summary>
        /// 交易记录内的支付金额，购买金额，购买量
        /// </summary>
        public Double PaymentAmount = 0;
        public Double PurchaseAmount = 0;
        public Double PurchaseQuantity = 0;
        
        /// <summary>
        /// 上次购买是否通过卡购买方式
        /// </summary>
        public Boolean IsCard = false;

        /// <summary>
        /// 表类型
        /// </summary>
        public Int32 ProtocolType = (Int32)eMeterProtocolType.Invalid;

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
    /// 2017-08-01 lijia
    /// 退购的任务文件结果的具体内容
    /// </summary>
    [Serializable]
    public class RefundTaskResultInfo : RefundTask
    {
        /// <summary>
        /// 表地址
        /// </summary>
        public String MeterAddress = String.Empty;

        /// <summary>
        /// 任务是否完成
        /// </summary>
        public Boolean TaskFinished = false;

        /// <summary>
        /// TaskProcess = FinishedStep;Step1Value;Step2Value;………. （以分号划分，第一个数据为总完成的步骤数，后面数据为每个步骤的值）
        /// </summary>
        public String TaskProcess = String.Empty;
    }
}
