using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge
{
    internal class Block
    {
        public List<int> Faces { get; private set; }

        public Block() 
        {
            Faces = new List<int>();
        }
    }
}
