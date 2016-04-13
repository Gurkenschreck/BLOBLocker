using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BLOBLocker.Code.Extention
{
    public static class ObjectExtention
    {
        public static T As<T>(this object obj)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if(converter != null)
            {
                return (T)converter.ConvertFrom(obj);
            }
            return default(T);
        }
    }
}
