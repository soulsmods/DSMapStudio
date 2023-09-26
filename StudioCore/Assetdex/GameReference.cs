using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Assetdex
{
    /// <summary>
    /// Class <c>GameReference</c> is an entry contained within a <c>GameReference</c> list within a <c>AssetdexResource</c>.
    /// </summary>
    public class GameReference
    {
        public string gameType { get; set; }
        public List<AssetReference> chrList { get; set; }
        public List<AssetReference> objList { get; set; }
        public List<AssetReference> partList { get; set; }
        public List<AssetReference> mapPieceList { get; set; }
    }

}
