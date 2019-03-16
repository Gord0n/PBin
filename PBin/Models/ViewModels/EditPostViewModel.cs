using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PBin.Models.ViewModels
{
    public class EditPostViewModel
    {

        public string sts { get; set; }

        public string checkBoxValue { get; set; }

        public string postEnabled { get; set; }

        public Post Post { get; set; }

    }
}