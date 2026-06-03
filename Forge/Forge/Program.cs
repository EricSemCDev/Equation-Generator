using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int blockCount = 12;
            int facesPerBlock = 4;
            int slotCount = 5;
            int operatorSlots = 4;
            int operatorCount = 6;
            float difficulty = 10f;

            List<bool> compositeSlots = new List<bool> { false, false, false, true, false };

            PhaseConfig config = new PhaseConfig(blockCount, facesPerBlock, slotCount, operatorSlots, operatorCount, difficulty, compositeSlots);
            Forge forge = new Forge();
            var data = forge.Generate(config);

            Console.WriteLine("Configuração:");
            Console.WriteLine("-----");
            Console.WriteLine("blockCount (Número de blocos) = " + blockCount);
            Console.WriteLine("facesPerBlock (Número de Faces) = " + facesPerBlock);
            Console.WriteLine("slotCount (Número de Slots Númericos) = " + slotCount);
            Console.WriteLine("operatorSlots (Número de Slots de Operadores) = " + operatorSlots);
            Console.WriteLine("operatorCount (Número de Operadores disponíveis) = " + operatorCount);
            Console.WriteLine("difficulty (Dificuldade) = " + difficulty);
            Console.WriteLine("compositeSlots = [ " + string.Join(", ", compositeSlots) + " ]");
            Console.WriteLine("-----");

            Console.WriteLine("");

            Console.WriteLine("Output");
            Console.WriteLine("-----");
            Console.WriteLine("R (Resultado da equação gerada) = " + data.R);
            Console.WriteLine("Pool de operadores gerados = [ " + string.Join(" | ", data.OperatorPool) + " ]");
            Console.WriteLine("Operadores usados na equação = [ " + string.Join(" | ", data.O) + " ]");
            Console.WriteLine("B (Lista de Blocos) =");
            for (int i = 0; i < data.B.Count; i++)
            {
                bool isComposite = i < config.compositeSlots.Count && config.compositeSlots[i];
                Console.WriteLine($"  Bloco {i + 1}: [ " + string.Join(", ", data.B[i].Faces) + " ]" + (isComposite ? " (composto)" : ""));
            }
            Console.WriteLine("equacao (equacao que garante resultado real) = " + data.equacao);
        }
    }
}