using System.Collections.Generic;

namespace MiniJob.Enums
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 广义的运行状态
        /// </summary>
        /// <param name="instanceStatus"></param>
        /// <returns></returns>
        public static List<InstanceStatus> GeneralizedRuningStatus(this InstanceStatus instanceStatus)
        {
            return new List<InstanceStatus> { InstanceStatus.WaitingDispatch, InstanceStatus.WaitingWorkerReceive, InstanceStatus.Runing };
        }

        /// <summary>
        /// 结束状态
        /// </summary>
        /// <param name="instanceStatus"></param>
        /// <returns></returns>
        public static List<InstanceStatus> FinishedStatus(this InstanceStatus instanceStatus)
        {
            return new List<InstanceStatus> { InstanceStatus.Failed, InstanceStatus.Succeed, InstanceStatus.Canceled, InstanceStatus.Stoped };
        }
    }
}
