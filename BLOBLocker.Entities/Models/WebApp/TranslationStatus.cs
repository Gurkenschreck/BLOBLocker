﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Entities.Models.WebApp
{
    public enum TranslationStatus : byte
    {
        New = 0,        // New string
        Translated,     // Finished with translating
        BaseModified,   // Base has been changed, translations have to be checked
        Live            // Currently displayed
    }
}
