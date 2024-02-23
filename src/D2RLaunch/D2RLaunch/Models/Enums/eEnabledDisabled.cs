using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLaunch.Models.Enums
{
    public enum eEnabledDisabled
    {
        [Display(Name = "Disabled")]
        Disabled,
        [Display(Name = "Enabled")]
        Enabled,
    }
}
