﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RLauncher.Models.Enums
{
    public enum eRunewordSorting
    {
        [Display(Name = "By Name")]
        ByName,
        [Display(Name = "By Itemtype")]
        ByItemtype,
        [Display(Name = "By Req.Level")]
        ByReqLevel
    }
}
