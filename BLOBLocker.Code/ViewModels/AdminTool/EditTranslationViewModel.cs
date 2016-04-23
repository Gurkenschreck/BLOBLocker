using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.AdminTool
{
    public class EditTranslationViewModel
    {
        public string Key { get; set; }
        public Translation Translation { get; set; }
    }
}
