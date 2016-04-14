using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Extention
{
    public static class EnumExtentions
    {
        public static ICollection<T> GetEnumCollection<T>(this Enum e)
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            return new List<T>(array);
        }
    }
}
