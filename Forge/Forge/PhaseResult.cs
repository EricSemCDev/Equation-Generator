using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge
{
    internal class PhaseResult
    {
        public int R { get; set; }
        public List<Char> O { get; set; }
        public List<Block> B { get; set; }
        public int? RMe { get; set; }
        public int? RMa { get; set; }
        public string equacao { get; set; }
        public float peso_real { get; set; }
        public List<char> OperatorPool { get; set; }
    }
}
