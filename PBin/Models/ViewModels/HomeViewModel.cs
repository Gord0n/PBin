﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PBin.Models.ViewModels
{
    public class HomeViewModel
    {

        public string sts { get; set; }

        public string SearchTerms { get; set; }

        public List<Post> Posts { get; set; }        

    }
}