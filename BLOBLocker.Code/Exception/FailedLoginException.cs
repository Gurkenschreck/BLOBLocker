using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Exception
{
    [Serializable]
    public class FailedLoginException : ApplicationException
    {
        public FailedLoginException() { }
        public FailedLoginException(string message) : base(message) { }
        public FailedLoginException(string message, System.Exception innerException) : base(message, innerException) { }
        public FailedLoginException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
