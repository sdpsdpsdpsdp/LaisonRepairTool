using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LaisonTech.CommonBLL;

namespace LaisonTech.LAPIS.ObjectDefine
{
    /// <summary>
    /// 2017-08-07 lijia
    /// 任务文件处理类
    /// 功能：
    /// 1.任务文件的加密盒解密。
    /// 2.任务结果文件的加密和解密
    /// 
    /// 使用方式：
    /// 1.Init初始化路径
    /// 2.加密、解密 任务文件
    /// 3.加密、解密 任务结果文件
    /// </summary>
    public class TaskFileManager
    {
        #region 内部常量

        /// <summary>
        /// 一次AES加密的长度
        /// </summary>
        private const Int32 Const_AESEncryptLen = 16;

        /// <summary>
        /// 任务文件加密密钥
        /// </summary>
        private const String EncryptKey_TaskFile = "LaisonTask123!@#";
        /// <summary>
        /// 任务结果文件加密密钥
        /// </summary>
        private const String EncryptKey_ResultFile = "TaskResult321#@!";

        /// <summary>
        /// 任务文件夹名
        /// </summary>
        private const String Const_TaskFileFolderName = "Task";
        /// <summary>
        /// 任务结果文件夹名
        /// </summary>
        private const String Const_ResultFileFolderName = "TaskResult";

        //基础
        private const String Const_TaskID = "TaskID";
        private const String Const_TaskType = "TaskType";
        private const String Const_TechnicianID = "TechnicianID";
        private const String Const_MeterAddress = "MeterAddress";
        private const String Const_IsCard = "IsCard";

        //--------------------------------
        //换表、销户 任务
        private const String Const_AreaCode = "AreaCode";
        private const String Const_MeterProtocolType = "MeterProtocolType";
        private const String Const_DecimalParameter = "DecimalParameter";
        //换表、销户 结果
        private const String Const_MeterStatue = "MeterStatue";
        private const String Const_UsedValue = "UsedValue";
        private const String Const_RemainValue = "RemainValue";
        private const String Const_Result = "Result";
        private const String Const_Feedback = "Feedback";
        private const String Const_Date = "Date";

        private const String Const_OldMeterTotalDialReading = "OldMeterTotalDialReading";

        //---------------
        //退购 任务
        private const String Const_RefundTradeID = "RefundTradeID";
        private const String Const_TIDTestingToken = "TIDTestingToken";
        private const String Const_RefundValue = "RefundValue";
        private const String Const_SetMeterKeyParam = "SetMeterKeyParam";
        //退购 结果
        private const String Const_AllowRefund = "AllowRefund";
        //任务是否已完成
        private const String Const_TaskFinished = "TaskFinished";
        //任务进度记录
        private const String Const_TaskProcess = "TaskProcess";


        /// <summary>
        /// 退购任务的文件名
        /// 前缀
        /// </summary>
        private const String Const_TaskFileName_RefundTask = "RefundTask";
        /// <summary>
        /// 退购任务结果文件名
        /// 前缀
        /// </summary>
        private const String Const_TaskFileResultName_RefundResult = "RefundTaskResult";

        /// <summary>
        /// 换表任务的文件名
        /// 前缀
        /// </summary>
        private const String Const_TaskFileName_ReplaceMeterTask = "ReplaceMeterTask";
        /// <summary>
        /// 换表任务结果文件名
        /// 前缀
        /// </summary>
        private const String Const_TaskFileResultName_ReplaceMeterResult = "ReplaceMeterTaskResult";

        /// <summary>
        /// 销户任务的文件名
        /// 前缀
        /// </summary>
        private const String Const_TaskFileName_CancelAccountTask = "CancelAccountTask";
        /// <summary>
        /// 销户任务结果文件名
        /// 前缀
        /// </summary>
        private const String Const_TaskFileResultName_CancelAccountResult = "CancelAccountTaskResult";

        #endregion

        #region 内部参数
        /// <summary>
        /// AES
        /// </summary>
        private static AESProcessor m_aes = AESProcessor.GetInstance();

        /// <summary>
        /// 任务结果文件夹名，带路径
        /// </summary>
        public static String ResultFileFolderPath = String.Empty;
        /// <summary>
        /// 任务文件夹名，带路径
        /// </summary>
        public static String TaskFileFolderPath = String.Empty;

        /// <summary>
        /// 软件的默认基础路径
        /// </summary>
        private static String m_AppPath = string.Empty;

        /// <summary>
        /// 构建dic<key-String, value-String>，key是等号左边，value是等号右边数据
        /// </summary>
        private static Dictionary<String, String> m_dicFileCon = null;
        #endregion

        #region 公开函数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="apppath"></param>
        /// <returns></returns>
        public static Boolean Init(String apppath)
        {
            if (apppath[apppath.Length - 1] != '\\')
            {
                apppath = apppath + "\\";
            }
            m_AppPath = apppath;
            ResultFileFolderPath = m_AppPath + Const_ResultFileFolderName + "\\";
            TaskFileFolderPath = m_AppPath + Const_TaskFileFolderName + "\\";
            return true;
        }

        /// <summary>
        /// 创建任务文件
        /// </summary>
        /// <param name="tasktype"></param>
        /// <param name="refundtaskinfo"></param>
        /// <param name="disassemblytaskinfo"></param>
        /// <returnes></returns>
        public static Boolean BuildTaskFile(eTaskType tasktype,
            RefundTaskInfo refundtaskinfo, MeterDisassemblyTaskInfo resultinfo, ref String filenamestr)
        {
            //首先，初始化解密任务文件的密钥
            InitKey(EncryptKey_TaskFile);

            //根据任务类型，获取任务文件名，带全路径
            String filename = GetFileName(tasktype, refundtaskinfo, resultinfo, false);
            if (String.IsNullOrEmpty(filename))
            {
                return false;
            }
            filename = TaskFileFolderPath + filename;
            filenamestr = filename;

            //构建文件具体的内容
            String filecontentstr = GetTaskFileContent(tasktype, refundtaskinfo, resultinfo);
            if (String.IsNullOrEmpty(filecontentstr))
            {
                return false;
            }

            //加密文件内容
            String encryptresfilestr = EncryptInfo(filecontentstr);

            //创建文件
            Boolean bl = FileProcessor.WriteFileString(filename, encryptresfilestr, false);
            return true;
        }
        /// <summary>
        /// 解析任务文件内容，获取任务具体内容
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tasktype">任务类型</param>
        /// <param name="refundtaskinfo">退购的任务信息</param>
        /// <param name="fileinfo">换表、销户的任务信息</param>
        /// <returns></returns>
        public static Boolean ParseTaskFile(String filename, ref eTaskType tasktype,
            ref RefundTaskInfo refundtaskinfo, ref MeterDisassemblyTaskInfo disassemblytaskinfo)
        {
            //首先，初始化解密任务文件的密钥
            InitKey(EncryptKey_TaskFile);

            //取出全部的文件内容
            String encryptstr = String.Empty;
            Boolean bl = FileProcessor.ReadFileString(filename, out encryptstr, false);
            if (!bl || String.IsNullOrEmpty(encryptstr))
            {
                return false;
            }

            //解析
         
            String filestr = DencryptTaskFileInfo(encryptstr);
            if (String.IsNullOrEmpty(filestr))
            {
                return false;
            }
            filestr = filestr.Trim();
            //获取任务类型
            bl = GetTaskType(filestr, ref tasktype);
            if (!bl)
            {
                return false;
            }

            //根据不同的任务类型获取任务信息类数据
            bl = GetTaskInfo(tasktype, ref refundtaskinfo, ref disassemblytaskinfo);
            return bl;
        }


        /// <summary>
        ///  创建任务结果文件
        /// </summary>
        /// <param name="tasktype"></param>
        /// <param name="refundtaskinfo"></param>
        /// <param name="resultinfo"></param>
        /// <param name="filenamestr"></param>
        /// <returns></returns>
        public static Boolean BuildTaskReslutFile(eTaskType tasktype,
            RefundTaskResultInfo refundtaskinfo, MeterDisassemblyTaskResultInfo resultinfo, ref String filenamestr)
        {
            //首先，初始化解密任务文件的密钥
            InitKey(EncryptKey_ResultFile);

            //先获取结果文件名
            String filename = GetFileName(tasktype, refundtaskinfo, resultinfo);
            if (String.IsNullOrEmpty(filename))
            {
                return false;
            }
            filename = ResultFileFolderPath + filename;
            filenamestr = filename;

            //构建文件具体的内容
            String filecontentstr = GetResultFileContent(tasktype, refundtaskinfo, resultinfo);
            if (String.IsNullOrEmpty(filecontentstr))
            {
                return false;
            }

            //加密文件内容
            String encryptresfilestr = EncryptInfo(filecontentstr);

            //创建文件
            Boolean bl = FileProcessor.WriteFileString(filename, encryptresfilestr, false);
            return true;
        }
        /// <summary>
        /// 解析任务结果文件，获取结果信息
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tasktype"></param>
        /// <param name="refundtaskinfo"></param>
        /// <param name="disassemblytaskinfo"></param>
        /// <returns></returns>
        public static Boolean ParseTaskReslutFile(String filename, ref eTaskType tasktype,
            ref RefundTaskResultInfo refundtaskinfo, ref MeterDisassemblyTaskResultInfo disassemblytaskinfo)
        {
            //首先，初始化解密任务文件的密钥
            InitKey(EncryptKey_ResultFile);

            //取出全部的文件内容
            String encryptstr = String.Empty;
            Boolean bl = FileProcessor.ReadFileString(filename, out encryptstr, false);
            if (!bl || String.IsNullOrEmpty(encryptstr))
            {
                return false;
            }

            //解析
            String filestr = DencryptTaskFileInfo(encryptstr);
            if (String.IsNullOrEmpty(filestr))
            {
                return false;
            }

            //获取任务类型
            bl = GetTaskType(filestr, ref tasktype);
            if (!bl)
            {
                return false;
            }

            //根据不同的任务类型获取任务信息类数据
            bl = GetTaskResultInfo(tasktype, ref refundtaskinfo, ref disassemblytaskinfo);
            return bl;
        }

        #endregion


        #region 私有函数
        /// <summary>
        /// 初始化密钥
        /// 在解密任务文件和加密任务结果文件之前一定需要执行一次初始化密钥
        /// </summary>
        private static void InitKey(String encryptionkey)
        {
            //计算密钥
            Byte[] keybytes = new Byte[16];
            Byte[] key = DataFormatProcessor.StringToBytes(encryptionkey);

            Array.Copy(key, 0, keybytes, 0, key.Length);

            //初始化AES密钥
            m_aes.InitKey(keybytes);
            return;
        }
        /// <summary>
        /// 解密任务文件内容
        /// </summary>
        /// <param name="encryptstr"></param>
        /// <returns></returns>
        private static String DencryptTaskFileInfo(String encryptstr)
        {
            //转化成字节数组，已加密
            Byte[] encryptBytes = DataFormatProcessor.HexStringToBytes(encryptstr);
            if (encryptBytes == null || encryptBytes.Length < 1)
            {
                return null;
            }

            //总长度
            Int32 allLen = encryptBytes.Length;

            //密文
            Byte[] rawbytes = new Byte[Const_AESEncryptLen];
            //原文
            Byte[] endata = null;
            String Ciphertext = String.Empty;

            String datastr = String.Empty;
            String value = String.Empty;
            Boolean bl = false;

            if (allLen <= Const_AESEncryptLen) //小于等于16字节
            {
                Array.Copy(encryptBytes, 0, rawbytes, 0, encryptBytes.Length);
                //AES加密
                m_aes.Decrypt(rawbytes, ref endata);

                //转换为HEX string 
                datastr = DataFormatProcessor.BytesToString(endata);
                bl = DataFormatProcessor.GetStringPair(datastr, '\0', ref Ciphertext, ref value);
                if (!bl || String.IsNullOrEmpty(Ciphertext))
                {
                    Ciphertext = datastr;
                }

                return Ciphertext;
            }
            //原始数据大于16字节，分段解密
            //已解密的长度
            Int32 sentDataTableLen = 0;
            //剩余需要解密的长度
            Int32 lastDataTableLen = allLen;
            rawbytes = new Byte[] { };
            List<String> CiphertextList = new List<string> { };
            //检查当前剩下还没解密的数据长度,是否超过16
            while (lastDataTableLen > Const_AESEncryptLen)
            {
                endata = null;
                rawbytes = new Byte[Const_AESEncryptLen];
                Array.Copy(encryptBytes, sentDataTableLen, rawbytes, 0, Const_AESEncryptLen);
                //随后 已发送长度增加16
                sentDataTableLen += Const_AESEncryptLen;
                lastDataTableLen -= Const_AESEncryptLen;

                //AES加密
                m_aes.Decrypt(rawbytes, ref endata);
                //转换为HEX string 
                datastr = DataFormatProcessor.BytesToString(endata);
                bl = DataFormatProcessor.GetStringPair(datastr, '\0', ref Ciphertext, ref value);
                if (!bl || String.IsNullOrEmpty(Ciphertext))
                {
                    Ciphertext = datastr;
                }
                CiphertextList.Add(Ciphertext);
            }
            endata = null;

            rawbytes = new Byte[Const_AESEncryptLen];
            Array.Copy(encryptBytes, sentDataTableLen, rawbytes, 0, lastDataTableLen);
            //AES加密
            m_aes.Decrypt(rawbytes, ref endata);

            //转换为HEX string 
            datastr = DataFormatProcessor.BytesToString(endata);
            bl = DataFormatProcessor.GetStringPair(datastr, '\0', ref Ciphertext, ref value);
            if (!bl || String.IsNullOrEmpty(Ciphertext))
            {
                Ciphertext = datastr;
            }

            CiphertextList.Add(Ciphertext);

            return DataFormatProcessor.ListToString(CiphertextList, "");

        }
        /// <summary>
        /// 加密任务结果内容
        /// </summary>
        /// <param name="filestr"></param>
        /// <returns></returns>
        private static String EncryptInfo(String filestr)
        {
            //使用 DataFormatProcessor获取 4字节
            Byte[] validdata = DataFormatProcessor.StringToBytes(filestr);
            if (validdata == null || validdata.Length < 1)
            {
                return null;
            }
            //总长度
            Int32 allLen = validdata.Length;

            //原文
            Byte[] rawbytes = new Byte[Const_AESEncryptLen];
            //密文
            Byte[] endata = null;
            String Ciphertext = String.Empty;

            if (allLen <= Const_AESEncryptLen) //小于等于16字节
            {
                Array.Copy(validdata, 0, rawbytes, 0, validdata.Length);
                //AES加密
                m_aes.Encrypt(rawbytes, ref endata);

                //转换为HEX string 
                Ciphertext = DataFormatProcessor.BytesToHexString(endata);

                return Ciphertext;
            }
            //原始数据大于16字节，分段传输
            //已发送的长度
            Int32 sentDataTableLen = 0;
            //剩余需要发送的长度
            Int32 lastDataTableLen = allLen;
            rawbytes = new Byte[] { };
            List<String> CiphertextList = new List<string> { };
            //检查当前剩下还没发送的数据长度,是否超过32K
            while (lastDataTableLen > Const_AESEncryptLen) //超过时，分段发送
            {
                endata = null;
                rawbytes = new Byte[Const_AESEncryptLen];
                Array.Copy(validdata, sentDataTableLen, rawbytes, 0, Const_AESEncryptLen);
                //随后 已发送长度增加32K
                sentDataTableLen += Const_AESEncryptLen;
                lastDataTableLen -= Const_AESEncryptLen;

                //AES加密
                m_aes.Encrypt(rawbytes, ref endata);
                //转换为HEX string 
                Ciphertext = DataFormatProcessor.BytesToHexString(endata);
                CiphertextList.Add(Ciphertext);
            }
            endata = null;
            //如果剩余不足32K, 直接全部发送,并且通知客户端已经发送完毕
            rawbytes = new Byte[Const_AESEncryptLen];
            Array.Copy(validdata, sentDataTableLen, rawbytes, 0, lastDataTableLen);
            //AES加密
            m_aes.Encrypt(rawbytes, ref endata);

            //转换为HEX string 
            Ciphertext = DataFormatProcessor.BytesToHexString(endata);
            CiphertextList.Add(Ciphertext);


            return DataFormatProcessor.ListToString(CiphertextList, "");
        }

        #region 解密

        /// <summary>
        /// 获取任务类型
        /// </summary>
        /// <param name="filestr"></param>
        /// <param name="tasktype"></param>
        /// <returns></returns>
        private static Boolean GetTaskType(String filestr, ref eTaskType tasktype)
        {
            if (String.IsNullOrEmpty(filestr))
            {
                return false;
            }
            //分割内容
            List<String> filelist = new List<string> { };
            Boolean bl = DataFormatProcessor.StringToList(filestr, Environment.NewLine, ref filelist);
            if (!bl || filelist.Count < 1)
            {
                return false;
            }

            //构建dic<key-String, value-String>，key是等号左边，value是等号右边数据
            m_dicFileCon = new Dictionary<string, string> { };
            foreach (String str in filelist)
            {
                List<String> list = new List<string> { };
                bl = DataFormatProcessor.StringToList(str, "=", ref list, false, true);
                if (!bl)
                {
                    return false;
                }
                if (list.Count == 1)
                {
                    list.Add("");
                }
                if (m_dicFileCon.Keys.Contains(list[0]))
                {
                    m_dicFileCon[list[0]] = list[1];
                }
                else
                {
                    m_dicFileCon.Add(list[0], list[1]);
                }
            }

            //获取任务类型
            if (!m_dicFileCon.ContainsKey(Const_TaskType))
            {
                return false;
            }
            String typestr = m_dicFileCon[Const_TaskType];
            tasktype = (eTaskType)Convert.ToInt32(typestr);
            return true;
        }

        #region 任务结果文件的解密
        /// <summary>
        /// 解密任务结果文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="refundtaskinfo"></param>
        /// <param name="disassemblytaskinfo"></param>
        /// <returns></returns>
        private static Boolean GetTaskResultInfo(eTaskType type, ref RefundTaskResultInfo refundtaskinfo, ref MeterDisassemblyTaskResultInfo disassemblytaskinfo)
        {
            refundtaskinfo = null;
            disassemblytaskinfo = null;
            switch (type)
            {
                case eTaskType.Refund:
                    return GetTaskResultInfo_Refund(type, ref refundtaskinfo);
                case eTaskType.CloseCustomer:
                case eTaskType.ReplaceMeter:
                    return GetTaskResultInfo_Replace(type, ref disassemblytaskinfo);
                default:
                    return false;
            }
        }
        /// <summary>
        /// 获取退购的任务结果信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="refundtaskinfo"></param>
        /// <returns></returns>
        private static Boolean GetTaskResultInfo_Refund(eTaskType type, ref RefundTaskResultInfo refundtaskinfo)
        {
            if (m_dicFileCon == null)
            {
                return false;
            }
            //判断任务类型，获取弹出的处理界面
            refundtaskinfo = new RefundTaskResultInfo { };
            //TaskID
            if (!m_dicFileCon.Keys.Contains(Const_TaskID))
            {
                return false;
            }
            refundtaskinfo.TaskID = Convert.ToInt32(m_dicFileCon[Const_TaskID]);

            //RefundTradeID
            if (!m_dicFileCon.Keys.Contains(Const_RefundTradeID))
            {
                return false;
            }
            refundtaskinfo.RefundTradeID = Convert.ToInt32(m_dicFileCon[Const_RefundTradeID]);

            //TechnicianID 
            if (!m_dicFileCon.Keys.Contains(Const_TechnicianID))
            {
                return false;
            }
            refundtaskinfo.TechnicianID = Convert.ToInt32(m_dicFileCon[Const_TechnicianID]);

            //MeterAddress
            if (!m_dicFileCon.Keys.Contains(Const_MeterAddress))
            {
                return false;
            }
            refundtaskinfo.MeterAddress = m_dicFileCon[Const_MeterAddress];

            //TaskFinished
            if (!m_dicFileCon.Keys.Contains(Const_TaskFinished))
            {
                return false;
            }
            refundtaskinfo.TaskFinished = (m_dicFileCon[Const_TaskFinished] == "1") ? true : false;

            //AllowRefund
            if (!m_dicFileCon.Keys.Contains(Const_AllowRefund))
            {
                return false;
            }
            refundtaskinfo.EnableRefund = (m_dicFileCon[Const_AllowRefund] == "1") ? true : false;

            //TaskProcess
            if (!m_dicFileCon.Keys.Contains(Const_TaskProcess))
            {
                return false;
            }
            refundtaskinfo.TaskProcess = m_dicFileCon[Const_TaskProcess];
            return true;
        }
        /// <summary>
        /// 获取换表、销户的任务结果信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="disassemblytaskinfo"></param>
        /// <returns></returns>
        private static Boolean GetTaskResultInfo_Replace(eTaskType type, ref MeterDisassemblyTaskResultInfo disassemblytaskinfo)
        {
            if (m_dicFileCon == null)
            {
                return false;
            }
            //判断任务类型，获取弹出的处理界面
            disassemblytaskinfo = new MeterDisassemblyTaskResultInfo { };
            //TaskID
            if (!m_dicFileCon.Keys.Contains(Const_TaskID))
            {
                return false;
            }
            disassemblytaskinfo.TaskID = Convert.ToInt32(m_dicFileCon[Const_TaskID]);

            //TaskType
            disassemblytaskinfo.TaskType = (Int32)type;

            //TechnicianID 
            if (!m_dicFileCon.Keys.Contains(Const_TechnicianID))
            {
                return false;
            }
            disassemblytaskinfo.TechnicianID = Convert.ToInt32(m_dicFileCon[Const_TechnicianID]);

            //MeterAddress
            if (!m_dicFileCon.Keys.Contains(Const_MeterAddress))
            {
                return false;
            }
            disassemblytaskinfo.MeterAddress = m_dicFileCon[Const_MeterAddress];

            //MeterStatue
            if (!m_dicFileCon.Keys.Contains(Const_MeterStatue))
            {
                return false;
            }
            disassemblytaskinfo.MeterStatue = (m_dicFileCon[Const_MeterStatue] == "1") ? true : false;

            //UsedValue
            if (!m_dicFileCon.Keys.Contains(Const_UsedValue))
            {
                return false;
            }
            disassemblytaskinfo.TotalComsumedValue = Convert.ToDouble(m_dicFileCon[Const_UsedValue]);

            //RemainValue
            if (!m_dicFileCon.Keys.Contains(Const_RemainValue))
            {
                return false;
            }
            disassemblytaskinfo.RemainValue = Convert.ToDouble(m_dicFileCon[Const_RemainValue]);

            //OldMeterTotalDialReading
            if (!m_dicFileCon.Keys.Contains(Const_OldMeterTotalDialReading))
            {
                return false;
            }
            disassemblytaskinfo.OldMeterTotalDialReading = Convert.ToDouble(m_dicFileCon[Const_OldMeterTotalDialReading]);
            
            //Result
            if (!m_dicFileCon.Keys.Contains(Const_Result))
            {
                return false;
            }
            disassemblytaskinfo.TaskResult = (m_dicFileCon[Const_Result] == "1") ? (Int32)eTaskResult.Success : (Int32)eTaskResult.Failed;

            //Feedback
            if (!m_dicFileCon.Keys.Contains(Const_Feedback))
            {
                return false;
            }
            disassemblytaskinfo.Feedback = m_dicFileCon[Const_Feedback];

            //Date
            if (!m_dicFileCon.Keys.Contains(Const_Date))
            {
                return false;
            }
            disassemblytaskinfo.OperateFinishTime = DateTime.Now;
            DateTime.TryParse(m_dicFileCon[Const_Date], out disassemblytaskinfo.OperateFinishTime);

            return true;
        }

        #endregion

        #region 任务文件的解密
        /// <summary>
        /// 根据任务类型，获取任务信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="refundtaskinfo"></param>
        /// <param name="disassemblytaskinfo"></param>
        /// <returns></returns>
        private static Boolean GetTaskInfo(eTaskType type, ref RefundTaskInfo refundtaskinfo, ref MeterDisassemblyTaskInfo disassemblytaskinfo)
        {
            refundtaskinfo = null;
            disassemblytaskinfo = null;
            switch (type)
            {
                case eTaskType.Refund:
                    return GetTaskInfo_Refund(type, ref refundtaskinfo);
                case eTaskType.CloseCustomer:
                case eTaskType.ReplaceMeter:
                    return GetTaskInfo_Replace(type, ref disassemblytaskinfo);
                default:
                    return false;
            }
        }
        /// <summary>
        /// 获取退购的任务信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="refundtaskinfo"></param>
        /// <returns></returns>
        private static Boolean GetTaskInfo_Refund(eTaskType type, ref RefundTaskInfo refundtaskinfo)
        {
            if (m_dicFileCon == null)
            {
                return false;
            }
            //判断任务类型，获取弹出的处理界面
            refundtaskinfo = new RefundTaskInfo { };
            //TaskID
            if (!m_dicFileCon.Keys.Contains(Const_TaskID))
            {
                return false;
            }
            refundtaskinfo.TaskID = Convert.ToInt32(m_dicFileCon[Const_TaskID]);

            //RefundTradeID
            if (!m_dicFileCon.Keys.Contains(Const_RefundTradeID))
            {
                return false;
            }
            refundtaskinfo.RefundTradeID = Convert.ToInt32(m_dicFileCon[Const_RefundTradeID]);
            
            //TechnicianID 
            if (!m_dicFileCon.Keys.Contains(Const_TechnicianID))
            {
                return false;
            }
            refundtaskinfo.TechnicianID = Convert.ToInt32(m_dicFileCon[Const_TechnicianID]);

            //MeterAddress
            if (!m_dicFileCon.Keys.Contains(Const_MeterAddress))
            {
                return false;
            }
            refundtaskinfo.MeterAddress = m_dicFileCon[Const_MeterAddress];

            //TIDTestingToken
            if (!m_dicFileCon.Keys.Contains(Const_TIDTestingToken))
            {
                return false;
            }
            refundtaskinfo.TIDTestingToken = m_dicFileCon[Const_TIDTestingToken];

            //RefundValue
            if (!m_dicFileCon.Keys.Contains(Const_RefundValue))
            {
                return false;
            }
            refundtaskinfo.RefundValue = Convert.ToDouble(m_dicFileCon[Const_RefundValue]);

            //MeterProtocolType
            if (!m_dicFileCon.Keys.Contains(Const_MeterProtocolType))
            {
                return false;
            }
            Int32 MeterProtocolType = Convert.ToInt32(m_dicFileCon[Const_MeterProtocolType]);
            refundtaskinfo.ProtocolType = MeterProtocolType;
            if (refundtaskinfo.ProtocolType == (Int32)eMeterProtocolType.STS_Card ||
                refundtaskinfo.ProtocolType == (Int32)eMeterProtocolType.STS_Card_LoRa ||
                refundtaskinfo.ProtocolType == (Int32)eMeterProtocolType.SoftwareSTS_Card ||
                refundtaskinfo.ProtocolType == (Int32)eMeterProtocolType.SoftwareSTS_Card_LoRa)
            {
                refundtaskinfo.IsCard = true;
            }

            //DecimalParameter
            if (!m_dicFileCon.Keys.Contains(Const_DecimalParameter))
            {
                return false;
            }
            refundtaskinfo.DecimalParameter = m_dicFileCon[Const_DecimalParameter];

            //SetMeterKeyParam
            if (!m_dicFileCon.Keys.Contains(Const_SetMeterKeyParam))
            {
                return false;
            }
            refundtaskinfo.SetMeterKeyParam = m_dicFileCon[Const_SetMeterKeyParam].Trim();

            return true;
        }
        /// <summary>
        /// 获取换表、销户的任务信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="disassemblytaskinfo"></param>
        /// <returns></returns>
        private static Boolean GetTaskInfo_Replace(eTaskType type, ref MeterDisassemblyTaskInfo disassemblytaskinfo)
        {
            if (m_dicFileCon == null)
            {
                return false;
            }
            //判断任务类型，获取弹出的处理界面
            disassemblytaskinfo = new MeterDisassemblyTaskInfo { };
            //TaskID
            if (!m_dicFileCon.Keys.Contains(Const_TaskID))
            {
                return false;
            }
            disassemblytaskinfo.TaskID = Convert.ToInt32(m_dicFileCon[Const_TaskID]);

            //TaskType
            disassemblytaskinfo.TaskType = (Int32)type;

            //TechnicianID 
            if (!m_dicFileCon.Keys.Contains(Const_TechnicianID))
            {
                return false;
            }
            disassemblytaskinfo.TechnicianID = Convert.ToInt32(m_dicFileCon[Const_TechnicianID]);

            //MeterAddress
            if (!m_dicFileCon.Keys.Contains(Const_MeterAddress))
            {
                return false;
            }
            disassemblytaskinfo.MeterAddress = m_dicFileCon[Const_MeterAddress];

            //AreaCode
            if (!m_dicFileCon.Keys.Contains(Const_AreaCode))
            {
                return false;
            }
            disassemblytaskinfo.AreaCode = Convert.ToInt32(m_dicFileCon[Const_AreaCode]);

            //MeterProtocolType
            if (!m_dicFileCon.Keys.Contains(Const_MeterProtocolType))
            {
                return false;
            }
            disassemblytaskinfo.MeterProtocolType = (eMeterProtocolType)Convert.ToInt32(m_dicFileCon[Const_MeterProtocolType]);

            //DecimalParameter
            if (!m_dicFileCon.Keys.Contains(Const_DecimalParameter))
            {
                return false;
            }
            disassemblytaskinfo.DecimalParameter = m_dicFileCon[Const_DecimalParameter];

            //SetMeterKeyParam 参数可选
            if (m_dicFileCon.Keys.Contains(Const_DecimalParameter))
            {
                disassemblytaskinfo.SetMeterKeyParam = m_dicFileCon[Const_SetMeterKeyParam];
            }

            return true;
        }
        #endregion

        #endregion

        #region 加密
        /// <summary>
        /// 获取文件名
        /// 根据任务类型和对应的参数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="refundtaskinfo"></param>
        /// <param name="resultinfo"></param>
        /// <param name="isResultFile">true-结果文件名，false-任务文件名</param>
        /// <returns></returns>
        private static String GetFileName(eTaskType type,
            RefundTask refundtaskinfo, MeterDisassemblyTaskBase resultinfo, Boolean isResultFile = true)
        {
            List<String> namelist = new List<string> { };

            String str = String.Empty;

            //先根据任务类型
            switch (type)
            {
                case eTaskType.CloseCustomer:
                    str = (isResultFile) ? Const_TaskFileResultName_CancelAccountResult : Const_TaskFileName_CancelAccountTask;
                    break;
                case eTaskType.Refund:
                    str = (isResultFile) ? Const_TaskFileResultName_RefundResult : Const_TaskFileName_RefundTask;
                    break;
                case eTaskType.ReplaceMeter:
                    str = (isResultFile) ? Const_TaskFileResultName_ReplaceMeterResult : Const_TaskFileName_ReplaceMeterTask;
                    break;
                default:
                    return null;
            }
            namelist.Add(str);

            if (type == eTaskType.Refund)
            {
                if (refundtaskinfo == null)
                {
                    return null;
                }
                //获取 TechnicianID
                namelist.Add(refundtaskinfo.TechnicianID.ToString());
                //获取 TaskID
                namelist.Add(refundtaskinfo.TaskID.ToString());
            }
            else
            {
                if (resultinfo == null)
                {
                    return null;
                }
                //获取 TechnicianID
                namelist.Add(resultinfo.TechnicianID.ToString());
                //获取 TaskID
                namelist.Add(resultinfo.TaskID.ToString());
            }
            String reslutfilename = DataFormatProcessor.ListToString(namelist, "_");
            reslutfilename = reslutfilename + ".bin";
            return reslutfilename;
        }

        /// <summary>
        /// 获取任务文件内容
        /// </summary>
        /// <param name="type"></param>
        /// <param name="refundtaskinfo"></param>
        /// <param name="resultinfo"></param>
        /// <returns></returns>
        private static String GetTaskFileContent(eTaskType type, RefundTaskInfo refundtaskinfo, MeterDisassemblyTaskInfo resultinfo)
        {
            //根据任务类型
            switch (type)
            {
                case eTaskType.CloseCustomer:
                case eTaskType.ReplaceMeter:
                    return GetTaskFileContent_Replase(resultinfo);
                case eTaskType.Refund:
                    return GetTaskFileContent_Refund(refundtaskinfo);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 根据RefundTaskInfo组合成任务文件内容
        /// 仅退购的任务
        /// </summary>
        /// <param name="refundtaskinfo"></param>
        /// <returns></returns>
        private static String GetTaskFileContent_Refund(RefundTaskInfo refundtaskinfo)
        {
            if (refundtaskinfo == null)
            {
                return null;
            }
            List<String> contentlist = new List<string> { };
            /*TaskID=XX（当前的任务ID）
             * TaskType=0:退购，1:换表，2:销户
             * TechnicianID =XX（上门工程师的ID）
             * MeterAddress=012017062001（表通信地址）
             * TIDTestingToken=XXXX XXXX XXXX XXXX XXXX（测试TID的TOKEN，值为0）
             * RefundToken=XXXX XXXX XXXX XXXX XXXX（撤销金额的TOKEN）
             * RefundValue=0.00（根据用户走量还是走金额，从交易记录中获取对应的值）
             * MeterProtocolType=XX (表通信协议类型，eMeterProtocolType)
             * DecimalParameter=脉冲当量小数位数;计量模式;价格精度小数位数;结算精度小数位数;是否显示K
             * SetMeterKeyParam=XXXXXXXXXXXXX (STS卡表在开启或关闭红外的时候需要这个设置密钥来制卡)
            */

            String filestr = Const_TaskID + "=" + refundtaskinfo.TaskID.ToString();
            contentlist.Add(filestr);
            filestr = Const_TaskType + "=0";
            contentlist.Add(filestr);

            filestr = Const_RefundTradeID + "=" + refundtaskinfo.RefundTradeID.ToString();
            contentlist.Add(filestr);

            filestr = Const_TechnicianID + "=" + refundtaskinfo.TechnicianID.ToString();
            contentlist.Add(filestr);

            filestr = Const_MeterAddress + "=" + refundtaskinfo.MeterAddress;
            contentlist.Add(filestr);

            filestr = Const_TIDTestingToken + "=" + refundtaskinfo.TIDTestingToken;
            contentlist.Add(filestr);
            filestr = Const_RefundValue + "=" + refundtaskinfo.RefundValue.ToString("F2");
            contentlist.Add(filestr);

            filestr = Const_MeterProtocolType + "=" + ((Int32)refundtaskinfo.ProtocolType).ToString();
            contentlist.Add(filestr);

            filestr = Const_DecimalParameter + "=" + refundtaskinfo.DecimalParameter;
            contentlist.Add(filestr);

            filestr = Const_SetMeterKeyParam + "=" + refundtaskinfo.SetMeterKeyParam;
            contentlist.Add(filestr);            

            String reslutfile = DataFormatProcessor.ListToString(contentlist, Environment.NewLine);
            return reslutfile;
        }
        /// <summary>
        /// 根据MeterDisassemblyTaskResultInfo组合成任务文件内容
        /// 换表和销户的任务
        /// </summary>
        /// <param name="resultinfo"></param>
        /// <returns></returns>
        private static String GetTaskFileContent_Replase(MeterDisassemblyTaskInfo resultinfo)
        {
            List<String> contentlist = new List<string> { };
            /*TaskID=XX（当前的任务ID）
             * TaskType=1（0-退购，1-换表，2-销户）
             * TechnicianID =XX（上门工程师的ID）
             * MeterAddress=012017062001（表通信地址，射频卡表是LAPIS中的meterbasicinfo里面的communicationaddress）
             * AreaCode=m_runparam.CurrentOperator.AreaCode
             * MeterProtocolType=XX (表通信协议类型，eMeterProtocolType)
             * DecimalParameter=脉冲当量小数位数;计量模式;价格精度小数位数;结算精度小数位数;货币符号;是否显示K*/

            String filestr = Const_TaskID + "=" + resultinfo.TaskID.ToString();
            contentlist.Add(filestr);
            filestr = Const_TaskType + "=" + ((Int32)resultinfo.TaskType).ToString();
            contentlist.Add(filestr);
            filestr = Const_TechnicianID + "=" + resultinfo.TechnicianID.ToString();
            contentlist.Add(filestr);

            filestr = Const_MeterAddress + "=" + resultinfo.MeterAddress;
            contentlist.Add(filestr);
            filestr = Const_AreaCode + "=" + resultinfo.AreaCode.ToString();
            contentlist.Add(filestr);

            filestr = Const_MeterProtocolType + "=" + ((Int32)resultinfo.MeterProtocolType).ToString();
            contentlist.Add(filestr);
            filestr = Const_DecimalParameter + "=" + resultinfo.DecimalParameter;
            contentlist.Add(filestr);

            //增加STS设置参数密钥
            filestr = Const_SetMeterKeyParam + "=" + resultinfo.SetMeterKeyParam;
            contentlist.Add(filestr);
            
            String reslutfile = DataFormatProcessor.ListToString(contentlist, Environment.NewLine);
            return reslutfile;
        }


        /// <summary>
        /// 组合成任务结果文件内容
        /// </summary>
        /// <param name="resultinfo"></param>
        /// <returns></returns>
        private static String GetResultFileContent(eTaskType type, RefundTaskResultInfo refundtaskinfo, MeterDisassemblyTaskResultInfo resultinfo)
        {
            //根据任务类型
            switch (type)
            {
                case eTaskType.CloseCustomer:
                case eTaskType.ReplaceMeter:
                    return GetResultFileContent_Replase(resultinfo);
                case eTaskType.Refund:
                    return GetResultFileContent_Refund(refundtaskinfo);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 根据RefundTaskInfo组合成任务结果文件内容
        /// 仅退购的任务结果
        /// </summary>
        /// <param name="resultinfo"></param>
        /// <returns></returns>
        private static String GetResultFileContent_Refund(RefundTaskResultInfo refundtaskinfo)
        {
            if (refundtaskinfo == null)
            {
                return null;
            }
            List<String> contentlist = new List<string> { };
            /*TaskID=XX（当前的任务ID）
              TaskType=0:退购，1:换表，2:销户
              TechnicianID =XX（上门工程师的ID）
              MeterAddress=012017062001（表通信地址）
              TaskFinished=0/1 （任务是否完成）
              AllowRefund=0/1 （是否允许退购，0：禁止，1：允许）
              TaskProcess=FinishedStep;Step1Value;Step2Value;………. （以分号划分，第一个数据为总完成的步骤数，后面数据为每个步骤的值）
            */
            String filestr = Const_TaskID + "=" + refundtaskinfo.TaskID.ToString();
            contentlist.Add(filestr);
            filestr = Const_TaskType + "=0";
            contentlist.Add(filestr);

            filestr = Const_RefundTradeID + "=" + refundtaskinfo.RefundTradeID.ToString();
            contentlist.Add(filestr);

            filestr = Const_TechnicianID + "=" + refundtaskinfo.TechnicianID.ToString();
            contentlist.Add(filestr);

            filestr = Const_MeterAddress + "=" + refundtaskinfo.MeterAddress;
            contentlist.Add(filestr);

            filestr = Const_TaskFinished + "=" + (refundtaskinfo.TaskFinished ? "1" : "0");
            contentlist.Add(filestr);

            filestr = Const_AllowRefund + "=" + (refundtaskinfo.EnableRefund ? "1" : "0");
            contentlist.Add(filestr);

            filestr = Const_TaskProcess + "=" + refundtaskinfo.TaskProcess;
            contentlist.Add(filestr);

            String reslutfile = DataFormatProcessor.ListToString(contentlist, Environment.NewLine);
            return reslutfile;
        }
        /// <summary>
        /// 根据MeterDisassemblyTaskResultInfo组合成任务结果文件内容
        /// 换表和销户的任务结果
        /// </summary>
        /// <param name="resultinfo"></param>
        /// <returns></returns>
        private static String GetResultFileContent_Replase(MeterDisassemblyTaskResultInfo resultinfo)
        {
            List<String> contentlist = new List<string> { };
            /*TaskID=XX（当前的任务ID）
             * TaskType=1（0-退购，1-换表，2-销户）
             * TechnicianID =XX（上门工程师的ID）
             * MeterAddress=012017062001（表通信地址，射频卡表是LAPIS中的meterbasicinfo里面的communicationaddress）
             * MeterStatue=0/1 (表计的状态，0-损坏，1-正常)
             * UsedValue=XX (配合MeterStatue，如果表计正常，则UsedValue指总使用金额/量。如果表计损坏，则UsedValue指当前机械读数)
             * RemainValue=XX(配合MeterStatue，如果表计正常，则RemainValue指剩余金额/量。如果表计损坏，保持为0，返回LAPIS由系统计算)
             * OldMeterTotalDialReading=XX(表计中的总使用量，如果表计正常，则表示标识00000010总使用量。如果表计损坏，则指当前机械读数)
             * Result=0/1 (任务是否成功，0-失败，1-成功)
             * Feedback= (工程师反馈的任务结果信息)
             * Date=20XX-XX-XX (操作日期)*/
            String filestr = Const_TaskID + "=" + resultinfo.TaskID.ToString();
            contentlist.Add(filestr);
            filestr = Const_TaskType + "=" + ((Int32)resultinfo.TaskType).ToString();
            contentlist.Add(filestr);
            filestr = Const_TechnicianID + "=" + resultinfo.TechnicianID.ToString();
            contentlist.Add(filestr);

            filestr = Const_MeterAddress + "=" + resultinfo.MeterAddress;
            contentlist.Add(filestr);
            String resstr = (resultinfo.MeterStatue) ? "1" : "0";
            filestr = Const_MeterStatue + "=" + resstr;
            contentlist.Add(filestr);

            filestr = Const_UsedValue + "=" + resultinfo.TotalComsumedValue.ToString("F2");
            contentlist.Add(filestr);
            filestr = Const_RemainValue + "=" + resultinfo.RemainValue.ToString("F2");
            contentlist.Add(filestr);

            filestr = Const_OldMeterTotalDialReading + "=" + resultinfo.OldMeterTotalDialReading.ToString("F2");
            contentlist.Add(filestr);
            

            resstr = (resultinfo.TaskResult == (Int32)eTaskResult.Failed) ? "0" : "1";
            filestr = Const_Result + "=" + resstr;
            contentlist.Add(filestr);

            filestr = Const_Feedback + "=" + resultinfo.Feedback;
            contentlist.Add(filestr);

            resstr = resultinfo.OperateFinishTime.ToString("yyyy-MM-dd");
            filestr = Const_Date + "=" + resstr;
            contentlist.Add(filestr);

            String reslutfile = DataFormatProcessor.ListToString(contentlist, Environment.NewLine);
            return reslutfile;
        }
        #endregion

        #endregion


    }
}
