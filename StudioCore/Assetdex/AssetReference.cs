using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Assetdex
{
    public class AssetReference
    {
        public string id { get; set; }
        public string referenceName { get; set; }
        public List<string> tagList { get; set; }
    }
}
