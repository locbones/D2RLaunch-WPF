using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLaunch.Models.Enums
{
    public enum eUiThemes
    {
        [Display(Name = "Standard UI")]
        Standard,
        [Display(Name = "ReMoDDeD UI")]
        ReMoDDeD,
    }
}
