using System;

namespace MiniJob.Model
{
    [Serializable]
    public class InstanceLogContent
    {
        public virtual Guid InstanceId { get; set; }

        public virtual DateTime LogTime { get; set; }

        public virtual int LogLevel { get; set; }

        public virtual string LogContent { get; set; }
    }
}
