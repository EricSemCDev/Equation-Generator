using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge
{
    internal class Operator
    {
        public char Symbol { get; set; }
        public int Weight { get; set; }

        public Operator(char symbol, int weight)
        {
            Symbol = symbol;
            Weight = weight;
        }

    }
}
