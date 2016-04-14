using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.Models.WebApp
{
    public enum TranslationType : byte
    {
        Global = 0,
        Error,
        Model,
        Nofitifaction
    }
}
