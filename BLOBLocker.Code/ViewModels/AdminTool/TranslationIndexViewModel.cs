using BLOBLocker.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.AdminTool
{
    public class TranslationIndexViewModel
    {
        public TranslationIndexViewModel()
        {
            IsModerator = false;
            IsTranslator = false;
            NewTranslation = new NewTranslationViewModel();
        }
        public bool IsModerator { get; set; }
        public bool IsTranslator { get; set; }
        public ICollection<Translation> Translations { get; set; }
        public NewTranslationViewModel NewTranslation { get; set; }
    }
}
