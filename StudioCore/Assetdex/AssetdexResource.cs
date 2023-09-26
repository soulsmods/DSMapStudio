using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudioCore.Assetdex
{
    /// <summary>
    /// Class <c>AssetdexResource</c> is the root object filled by the JSON deserialization.
    /// </summary>
    public class AssetdexResource
    {
        public List<GameReference> GameReference { get; set; }
    }
}
