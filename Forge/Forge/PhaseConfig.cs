using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge
{
    internal class PhaseConfig
    {
        public int blockCount { get; set; }
        public int facesPerBlock { get; set; }
        public int slotCount { get; set; }
        public int operatorSlots { get; set; }
        public int operatorCount { get; set; }
        public float difficulty { get; set; }
        public List<bool> compositeSlots { get; set; }

        public PhaseConfig(int b, int f, int s, int oS, int oC, float d, List<bool> cs)
        {
            int totalBlocksNeeded = s + cs.Count(c => c == true);

            if (b < 2) throw new ArgumentException("O Número de blocos deve ser maior ou igual a 2");
            if (f < 1) throw new ArgumentException("O Número de faces em blocos deve ser maior ou igual a 1");
            if (s < 2) throw new ArgumentException("O Número de slots numéricos deve ser maior ou igual a 2");
            if (oS < 1) throw new ArgumentException("O Número de slots de operadores deve ser maior ou igual a 1");
            if (oC < 1) throw new ArgumentException("O Número de operadores deve ser maior ou igual a 1");
            if (d < 1 || d > 10) throw new ArgumentException("A dificuldade não pode ser menor que 1 ou maior que 10");
            if (cs.Count != s) throw new ArgumentException("compositeSlots deve ter o mesmo tamanho que slotCount");
            if (b < totalBlocksNeeded) throw new ArgumentException("Blocos insuficientes para os slots compostos");

            blockCount = b;
            facesPerBlock = f;
            slotCount = s;
            operatorSlots = oS;
            operatorCount = oC;
            difficulty = d;
            compositeSlots = cs;
        }
    }
}