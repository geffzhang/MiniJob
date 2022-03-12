namespace MiniJob.Enums
{
    /// <summary>
    /// 任务实例状态
    /// </summary>
    public enum InstanceStatus
    {
        /// <summary>
        /// 等待派发
        /// </summary>
        WaitingDispatch,

        /// <summary>
        /// 等待Worker接收
        /// </summary>
        WaitingWorkerReceive,

        /// <summary>
        /// 运行中
        /// </summary>
        Runing,

        /// <summary>
        /// 失败
        /// </summary>
        Failed,

        /// <summary>
        /// 成功
        /// </summary>
        Succeed,

        /// <summary>
        /// 取消
        /// </summary>
        Canceled,

        /// <summary>
        /// 手动停止
        /// </summary>
        Stoped
    }
}
