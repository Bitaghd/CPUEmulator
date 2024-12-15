namespace CPUEmulator
{
    class Assembler
    {
        private static List<string> instructions = new List<string>();
        private static Dictionary<string, int> labels = new Dictionary<string, int>();
        public static string GetCurrentInstruction(int index)
        {
            return instructions[index];
        }

        public static string GetLabelName(int value)
        {
            return labels.FirstOrDefault(x => x.Value == value).Key;
        }
        
        public static void LoadLabels(List<string> program)
        {

            for(int i = 0; i < program.Count; i++)
            {
                if (program[i].EndsWith(":"))
                {
                    var label = program[i].Trim(':');
                    labels.Add(label, i + 1);
                }
            }
        }

        private static int GetLabelIndex(string key)
        {
            if (labels.ContainsKey(key))
                return labels[key];
            return -1;
        }
        public static int GetInstructionSet(string instruction)
        {
            switch (instruction)
            {
                case "LOAD": return 0x1;
                case "ADD": return 0x2;
                case "SUB": return 0x3;
                case "JUMP": return 0x4;
                case "STORE": return 0x5;
                case "JUMPZ": return 0x6;
                case "HALT": return 0x7;
                case "INC": return 0x8;
                case "LOOP": return 0x9;
                default: return -1;
            }
        }

        public static int Assemble(string cmd, int index)
        {
            instructions.Add(cmd);
            string[] parts = cmd.Split(new[] { ' ', ','}, StringSplitOptions.RemoveEmptyEntries);

            // Пропускаем, если встречаем лейбл, возвращаем значение 0 в качестве инструкции
            if (parts.Length == 1 && parts[0].EndsWith(":"))
            {
                return 0;
            }

            if (parts.Length == 0 || GetInstructionSet(parts[0]) == -1)
            {
                throw new Exception($"Unknown command: {cmd}!");
            }

            int cmdtype = GetInstructionSet(parts[0]);

            int literal = 0, dest = 0, op1 = 0, op2 = 0;
            if (parts.Length > 1)
            {
                if (parts[1].StartsWith("[") || parts[1].StartsWith("#") || GetLabelIndex(parts[1]) != -1)
                    literal = ParseOperand(parts[1]);
                else
                    dest = ParseOperand(parts[1]);

            }
            if (parts.Length > 2)
            {
                if ((parts[2].StartsWith("#")))
                    literal = ParseOperand(parts[2]);
                else if (parts[2].StartsWith("[") && parts[2].EndsWith("]"))
                {
                    var inner = parts[2][1..^1];
                    if (inner.StartsWith("R"))
                        op2 = ParseOperand(inner);
                    else
                        op1 = ParseOperand(inner);
                }
                else
                    op1 = ParseOperand(parts[2]);

            }
            if (parts.Length > 3)
            {
                if ((parts[3].StartsWith("#")))
                    literal = ParseOperand(parts[3]);
                else
                    op2 = ParseOperand(parts[3]);
            }


            return (cmdtype << 28) | (literal << 12) | (dest << 8) | (op1 << 4) | op2;
        }

        private static int ParseOperand(string operand)
        {
            if (operand.StartsWith("#"))
            {
                // Непосредственное значение
                return int.Parse(operand.Substring(1)); // Убираем символ '#' и парсим как число
            }
            else if (operand.StartsWith("[") && operand.EndsWith("]"))
            {
                // Адрес в памяти (внутри может быть число или регистр)
                string inner = operand[1..^1]; // Убираем квадратные скобки
                if (inner.StartsWith("R"))
                {
                    var index = int.Parse(inner[1..]);
                    return index;
                }
                else
                {
                    // Это адрес в памяти: парсим как число
                    return int.Parse(inner);
                }
            }
            else if (operand.StartsWith("R"))
            {
                // Регистр
                return int.Parse(operand.Substring(1)); // Убираем символ 'R' и парсим индекс
            }
            else if (GetLabelIndex(operand) != -1) //Обрабатываем лейблы в инструкциях 
            {
                var label = GetLabelIndex(operand);
                return label;
            }
            else
            {
                throw new ArgumentException($"Uknown opcode: {operand}");
            }
        }
    }
}
