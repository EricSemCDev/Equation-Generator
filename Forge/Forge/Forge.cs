using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge
{
    internal class Forge
    {
        private Random random = new Random();

        private (int, int) GetSlotRange(float difficulty)
        {
            if (difficulty <= 2) return (1, 4);
            if (difficulty <= 5) return (5, 9);
            return (0, 9);
        }

        private bool ShouldGenerateComposite(float difficulty)
        {
            if (difficulty < 6) return false;
            float chance = 0.20f + (difficulty - 6) * 0.15f;
            return random.NextDouble() < chance;
        }

        private int GetRmax(float difficulty)
        {
            if (difficulty <= 2) return 10;
            if (difficulty <= 5) return 50;
            if (difficulty <= 7) return 100;
            return 500;
        }

        private bool CanBeNegative(float difficulty) => difficulty >= 8;

        private List<char> GenerateOperatorPool(PhaseConfig config)
        {
            List<char> easyOps = new List<char> { '+', '-' };
            List<char> hardOps = new List<char> { '*', '/' };
            List<char> pool = new List<char>();

            float hardChance;
            if (config.difficulty <= 3) hardChance = 0.20f;
            else if (config.difficulty <= 7) hardChance = 0.50f;
            else hardChance = 0.80f;

            while (pool.Count < config.operatorCount)
            {
                char op;
                if (random.NextDouble() < hardChance)
                    op = hardOps[random.Next(0, hardOps.Count)];
                else
                    op = easyOps[random.Next(0, easyOps.Count)];

                int currentCount = pool.Count(o => o == op);
                if (currentCount < config.operatorSlots)
                    pool.Add(op);
            }

            return pool;
        }

        private List<char> SelectOperators(PhaseConfig config, List<char> pool)
        {
            List<char> selected = new List<char>();

            while (true)
            {
                selected.Clear();

                for (int i = 0; i < config.operatorSlots; i++)
                {
                    int index = random.Next(0, pool.Count);
                    selected.Add(pool[index]);
                }

                // valida que não tem / consecutivo
                bool invalid = false;
                for (int i = 0; i < selected.Count - 1; i++)
                {
                    if (selected[i] == '/' && selected[i + 1] == '/')
                    {
                        invalid = true;
                        break;
                    }
                }

                if (!invalid) return selected;
            }
        }

        private int GenerateSlotValue(PhaseConfig config)
        {
            if (ShouldGenerateComposite(config.difficulty))
                return random.Next(10, 100);

            var (min, max) = GetSlotRange(config.difficulty);
            return random.Next(min, max + 1);
        }

        private (List<Block>, string, int) GenerateBlocks(PhaseConfig config, List<char> operators)
        {
            List<Block> blocks = new List<Block>();
            for (int i = 0; i < config.blockCount; i++)
                blocks.Add(new Block());

            int Rmax = GetRmax(config.difficulty);
            bool canBeNegative = CanBeNegative(config.difficulty);

            while (true)
            {
                List<int> slotValues = new List<int>();
                List<int> slotDigits = new List<int>(); // dígitos individuais para plantar nos blocos

                // Gera valores para cada slot
                for (int i = 0; i < config.slotCount; i++)
                {
                    if (config.compositeSlots[i])
                    {
                        int d1 = random.Next(1, 10); // primeiro dígito (1-9, evita 0 na frente)
                        int d2 = random.Next(0, 10); // segundo dígito
                        int composite = d1 * 10 + d2;
                        slotValues.Add(composite);
                        slotDigits.Add(d1);
                        slotDigits.Add(d2);
                    }
                    else
                    {
                        var (min, max) = GetSlotRange(config.difficulty);
                        int val = random.Next(min, max + 1);
                        slotValues.Add(val);
                        slotDigits.Add(val);
                    }
                }

                // Avalia equação da esquerda pra direita
                float resultado = slotValues[0];
                bool invalid = false;

                for (int i = 0; i < operators.Count; i++)
                {
                    switch (operators[i])
                    {
                        case '+': resultado += slotValues[i + 1]; break;
                        case '-': resultado -= slotValues[i + 1]; break;
                        case '*': resultado *= slotValues[i + 1]; break;
                        case '/':
                            if (slotValues[i + 1] == 0) { invalid = true; break; }
                            resultado = (float)Math.Round(resultado / slotValues[i + 1]);
                            break;
                    }
                }

                if (invalid) continue;

                int R = (int)resultado;

                if (!canBeNegative && R < 0) continue;
                if (R > Rmax) continue;

                // Limpa blocos de tentativas anteriores
                foreach (Block b in blocks) b.Faces.Clear();

                // Planta dígitos nos blocos (1 dígito por bloco)
                for (int i = 0; i < slotDigits.Count; i++)
                    blocks[i].Faces.Add(slotDigits[i]);

                // Preenche faces restantes
                foreach (Block block in blocks)
                {
                    while (block.Faces.Count < config.facesPerBlock)
                    {
                        int randomFace = random.Next(0, 10);
                        if (!block.Faces.Contains(randomFace))
                            block.Faces.Add(randomFace);
                    }
                }

                // Embaralha faces
                foreach (Block block in blocks)
                {
                    for (int i = block.Faces.Count - 1; i > 0; i--)
                    {
                        int j = random.Next(0, i + 1);
                        int temp = block.Faces[i];
                        block.Faces[i] = block.Faces[j];
                        block.Faces[j] = temp;
                    }
                }

                // Monta string da equação
                string equacao = "";
                for (int i = 0; i < slotValues.Count; i++)
                {
                    equacao += slotValues[i];
                    if (i < operators.Count)
                        equacao += " " + operators[i] + " ";
                }
                equacao += " = " + R;

                return (blocks, equacao, R);
            }
        }

        public PhaseResult Generate(PhaseConfig config)
        {
            List<char> pool = GenerateOperatorPool(config);
            List<char> operators = SelectOperators(config, pool);
            var (blocks, equacao, R) = GenerateBlocks(config, operators);

            PhaseResult result = new PhaseResult();
            result.R = R;
            result.O = operators;
            result.B = blocks;
            result.equacao = equacao;
            result.OperatorPool = pool;
            return result;
        }
    }
}