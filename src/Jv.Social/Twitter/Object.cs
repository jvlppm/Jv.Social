﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jv.Social.Twitter
{
    public class Object : DynamicWrapper
    {
        internal Object(dynamic obj) : base((object)obj)
        {
        }
    }
}
