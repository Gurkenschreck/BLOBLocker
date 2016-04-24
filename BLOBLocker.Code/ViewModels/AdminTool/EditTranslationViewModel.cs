using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.AdminTool
{
    public class EditResourceViewModel
    {
        public int ResourceID { get; set; }
        public bool IsModerator { get; set; }
        public StringResource StringResource { get; set; }
        public bool MajorBaseChange { get; set; }
    }
}
