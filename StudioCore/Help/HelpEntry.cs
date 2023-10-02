using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Help
{
    public class HelpEntry
    {
        public string Title { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Contents { get; set; }
    }
}
