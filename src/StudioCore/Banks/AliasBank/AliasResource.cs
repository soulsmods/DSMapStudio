﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudioCore.Banks.AliasBank;

public class AliasResource 
{
    public List<AliasReference> list { get; set; }
}
