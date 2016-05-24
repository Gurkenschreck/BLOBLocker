using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Exception
{
    [Serializable]
    public class UnauthorizedPoolAccessException : ApplicationException
    {
        public UnauthorizedPoolAccessException() { }
        public UnauthorizedPoolAccessException(string message) : base(message) { }
        public UnauthorizedPoolAccessException(string message, System.Exception innerException) : base(message, innerException) { }
        public UnauthorizedPoolAccessException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
