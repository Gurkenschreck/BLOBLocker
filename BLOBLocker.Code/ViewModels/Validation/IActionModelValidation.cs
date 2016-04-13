using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.ViewModels.Validation
{
    public interface IActionModelValidation
    {
        IEnumerable<ValidationResult> Validate(IDictionary<string, object> settings, IDictionary<string, object> actionParams);
    }
}
