using System;
using System.Runtime.Serialization;

namespace MiniJob
{
    /// <summary>
    /// MiniJob 运行时异常
    /// </summary>
    public class MiniJobException : Exception
    {
        public MiniJobException()
        {

        }

        public MiniJobException(string message)
            : base(message)
        {

        }

        public MiniJobException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        public MiniJobException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}