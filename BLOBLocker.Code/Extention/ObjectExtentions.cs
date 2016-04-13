using System;
using System.Collections.Generic;
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
            try
            {
                return (T)obj;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
