using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Exception
{
    [Serializable]
    public class PoolFileNotFoundException : ApplicationException
    {
        public PoolFileNotFoundException() { }
        public PoolFileNotFoundException(string message) : base(message) { }
        public PoolFileNotFoundException(string message, System.Exception innerException) : base(message, innerException) { }
        public PoolFileNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
