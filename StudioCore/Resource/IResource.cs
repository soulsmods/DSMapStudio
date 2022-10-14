using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Resource
{
    public interface IResource
    {
        public bool _Load(byte[] bytes, AccessLevel al, GameType type);
        public bool _Load(string file, AccessLevel al, GameType type);
    }
}
