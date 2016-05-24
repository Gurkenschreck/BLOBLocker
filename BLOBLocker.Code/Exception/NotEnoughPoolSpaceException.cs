using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Exception
{
    [Serializable]
    public class NotEnoughPoolSpaceException : ApplicationException
    {
        public NotEnoughPoolSpaceException() { }
        public NotEnoughPoolSpaceException(string message) : base(message) { }
        public NotEnoughPoolSpaceException(string message, System.Exception innerException) : base(message, innerException) { }
        public NotEnoughPoolSpaceException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
