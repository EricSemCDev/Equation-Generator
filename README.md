# Forge — Gerador de Fase para Puzzle Numérico

## Contexto

Este problema é parte de uma plataforma de desafios de programação. Você está implementando o núcleo de geração de fases para um jogo de puzzle numérico em turnos.

Cada fase do jogo apresenta ao jogador um conjunto de **blocos**, cada bloco com múltiplas **faces** numéricas visíveis. O objetivo do jogador é selecionar faces de blocos e combiná-las com operadores disponíveis para atingir um **resultado-alvo R**, respeitando as restrições da equação.

Seu sistema — chamado de **Forge** — recebe uma configuração de fase e deve gerar automaticamente:
- Uma equação válida com resultado garantido
- Um conjunto de blocos com os valores da equação "plantados" nas faces
- Operadores disponíveis para o jogador usar
- Valores auxiliares de meta (RMe e RMa) que definem os extremos alcançáveis da fase

A equação segue avaliação **estritamente da esquerda para a direita**, sem precedência de operadores.

> Exemplo: `3 + 4 * 2 = 14`

Divisões que resultem em decimais seguem arredondamento padrão: `>= 0.5` arredonda para cima, `< 0.5` arredonda para baixo.

---

## Inputs

| Parâmetro | Descrição | Restrição |
|---|---|---|
| `N` | Número de blocos | `N >= 2` |
| `F` | Número de faces por bloco | `F >= 1` |
| `S` | Número de slots numéricos na equação | `S >= 2` |
| `OS` | Número de slots de operadores na equação | `OS >= 1` |
| `OC` | Número de operadores no pool disponível | `OC >= 1` |
| `D` | Nível de dificuldade da fase | `1 <= D <= 10` |
| `CS` | Lista de booleanos indicando quais slots são compostos | `len(CS) == S` |

### Sobre o parâmetro `CS` (Composite Slots)

Cada posição de `CS` indica se o slot numérico correspondente é **simples** (`false`) ou **composto** (`true`). Um slot composto ocupa dois blocos adjacentes, formando um número de dois dígitos (10–99). O número total de blocos `N` deve ser suficiente para cobrir todos os slots, incluindo os compostos.

> Restrição derivada: `N >= S + count(CS == true)`

---

## Estruturas

### Bloco
Contém `F` faces, cada uma com um valor inteiro de `0` a `9`. Valores não se repetem dentro do mesmo bloco. Valores podem se repetir entre blocos distintos.

### Slot Numérico
- **Simples** (`CS[i] = false`): ocupado por 1 bloco, produz valor de `0` a `9`
- **Composto** (`CS[i] = true`): ocupado por 2 blocos adjacentes, produz valor de `10` a `99`

### Equação Gerada
```
[slot_1] [op_1] [slot_2] [op_2] ... [slot_S] = R
```

---

## Impacto da Dificuldade

A dificuldade `D` controla globalmente o comportamento de geração:

| Aspecto | D <= 2 | D <= 5 | D <= 7 | D <= 10 |
|---|---|---|---|---|
| Faixa de valores dos slots simples | 1–4 | 5–9 | 0–9 | 0–9 |
| Rmax de referência | 10 | 50 | 100 | 500 |
| Chance de slot composto (D >= 6) | — | — | 20% + 15%*(D-6) | até 80%+ |
| Resultado negativo permitido | não | não | não | sim (D >= 8) |
| Chance de operadores pesados no pool | 20% | 50% | 50% | 80% |

---

## Processo

### 1. Geração do Pool de Operadores

Com base em `D` e `OC`, o algoritmo monta um pool de `OC` operadores mesclando operadores leves (`+`, `-`) e pesados (`*`, `/`) conforme a probabilidade da dificuldade. Nenhum operador pode aparecer mais vezes que `OS` no pool.

### 2. Seleção dos Operadores da Equação

Do pool gerado, o algoritmo sorteia `OS` operadores para compor a equação. A única restrição adicional é que não podem existir dois operadores `/` consecutivos na equação.

### 3. Geração dos Blocos e Valores

O algoritmo gera `S` valores de slot compatíveis com a dificuldade (simples ou compostos conforme `CS`). Avalia a equação da esquerda para a direita produzindo `R`. O resultado `R` deve estar dentro do `Rmax` de referência e, se `D < 8`, deve ser não-negativo. Caso contrário, descarta e tenta novamente.

Os dígitos dos valores da equação são **plantados** nas faces dos blocos correspondentes. As faces restantes de cada bloco são preenchidas aleatoriamente com valores de `0` a `9`, sem repetição no mesmo bloco.

### 4. Geração do RMe e RMa

O algoritmo varre todas as combinações possíveis de faces e operadores disponíveis:
- Encontra `Rmin` — menor resultado alcançável com os blocos e operadores gerados
- Encontra `Rmax_real` — maior resultado alcançável

Em seguida:
- Sorteia `RMe` no intervalo `[Rmin, R / 3]`
- Sorteia `RMa` no intervalo `[R + (R / 3), Rmax_real]`

Se o intervalo for vazio, o respectivo valor não é gerado (`null`).

---

## Outputs

| Saída | Descrição |
|---|---|
| `R` | Resultado da equação gerada |
| `O` | Lista de operadores usados na equação |
| `OperatorPool` | Pool completo de operadores disponíveis para o jogador |
| `B` | Lista de `N` blocos, cada um com `F` faces geradas |
| `RMe` | Valor sorteado abaixo de `R / 3`, com solução alcançável garantida (pode ser `null`) |
| `RMa` | Valor sorteado acima de `R + (R / 3)`, com solução alcançável garantida (pode ser `null`) |
| `equacao` | Solução garantida — uso interno, **não exposta ao jogador** |
| `peso_real` | Peso efetivo da equação gerada (reservado para uso futuro) |

---

## Restrições

- `N >= S + count(CS == true)` — blocos suficientes para todos os slots
- Slots compostos produzem valores de `10` a `99`, ocupando exatamente 2 blocos adjacentes
- Operadores `/` não podem aparecer consecutivamente na equação
- `RMe` nunca pode ser menor que `Rmin`
- `RMa` nunca pode ser maior que `Rmax_real`
- Para `D < 8`, o resultado `R` deve ser não-negativo

---

## Exemplo

**Configuração:**
```
N  = 12
F  = 4
S  = 5
OS = 4
OC = 6
D  = 10
CS = [ false, false, false, true, false ]
```

**Saída esperada (valores ilustrativos):**
```
R = 47
Pool de operadores = [ + | * | / | - | * | + ]
Operadores usados  = [ * | + | / | - ]
B (Lista de Blocos):
  Bloco 1:  [ 7, 3, 1, 5 ]
  Bloco 2:  [ 4, 9, 2, 6 ]
  Bloco 3:  [ 8, 0, 5, 2 ]
  Bloco 4:  [ 3, 6, 0, 9 ]  ← bloco composto (decena)
  Bloco 5:  [ 7, 1, 4, 8 ]  ← bloco composto (unidade)
  ...
equacao = 7 * 4 + 8 / 38 - 6 = 47
```

---

## Como Rodar

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) instalado (versão 6.0 ou superior)
- Visual Studio 2022+ **ou** qualquer terminal com a CLI do .NET

### Passo a passo

**1. Clone o repositório**
```bash
git clone https://github.com/EricSemCDev/Equation-Generator.git
cd Equation-Generator
```

**2. Acesse a pasta do projeto**
```bash
cd Forge/Forge
```

**3. Rode o projeto**
```bash
dotnet run
```

### Customizando os parâmetros

Abra o arquivo `Program.cs` e edite as variáveis no início do método `Main`:

```csharp
int blockCount    = 12;    // N — número de blocos
int facesPerBlock = 4;     // F — faces por bloco
int slotCount     = 5;     // S — slots numéricos
int operatorSlots = 4;     // OS — slots de operadores na equação
int operatorCount = 6;     // OC — operadores no pool
float difficulty  = 10f;   // D — dificuldade (1 a 10)

// CS — define quais slots são compostos (dois dígitos)
// O índice corresponde ao slot numérico na equação
List<bool> compositeSlots = new List<bool> { false, false, false, true, false };
```

Salve e rode novamente com `dotnet run`. A saída exibirá a configuração usada, o pool de operadores gerado, os blocos com suas faces e a equação garantida.

### Exemplo de saída no terminal

```
Configuração:
-----
blockCount (Número de blocos) = 12
facesPerBlock (Número de Faces) = 4
slotCount (Número de Slots Numéricos) = 5
operatorSlots (Número de Slots de Operadores) = 4
operatorCount (Número de Operadores disponíveis) = 6
difficulty (Dificuldade) = 10
compositeSlots = [ false, false, false, true, false ]
-----

Output
-----
R (Resultado da equação gerada) = 47
Pool de operadores gerados = [ + | * | / | - | * | + ]
Operadores usados na equação = [ * | + | / | - ]
B (Lista de Blocos) =
  Bloco 1: [ 7, 3, 1, 5 ]
  Bloco 2: [ 4, 9, 2, 6 ]
  ...
equacao (equação que garante resultado real) = 7 * 4 + 8 / 38 - 6 = 47
```

---

## Licença

MIT — veja o arquivo [LICENSE](./LICENSE) para detalhes.
