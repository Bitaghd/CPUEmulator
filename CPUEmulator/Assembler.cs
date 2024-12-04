
namespace CPUEmulator
{
    class Assembler
    {
        private static List<string> instructions = new List<string>();
        private static List<int> labels = new List<int>();
        public static string GetCurrentInstruction(int index)
        {
            return instructions[index];
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

        public static string GetInstructionSet(int instruction)
        {
            switch (instruction)
            {
                case 0x1: return "LOAD";
                case 0x2: return "ADD";
                case 0x3: return "SUB";
                case 0x4: return "JUMP";
                case 0x5: return "STORE";
                case 0x6: return "JUMPZ";
                case 0x7: return "HALT";
                case 0x8: return "INC";
                case 0x9: return "LOOP";
                default: return null;
            }
        }

        public static string Dissasemble(int cmd)
        {
            var result = GetInstructionSet(cmd);
            return result;
        }

        public static int Assemble(string cmd)
        {
            instructions.Add(cmd);
            string[] parts = cmd.Split(new[] { ' ', ','}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0 || GetInstructionSet(parts[0]) == -1)
            {
                throw new Exception($"Unknown command: {cmd}!");
            }

            // Обработать лейблы LOOP и END
            // НЕ В КАЧЕСТВЕ ИНСТРУКЦИЙ
            //if (parts.Length == 1 && parts[0].EndsWith(":"))
            //{
                
            //}
            int cmdtype = GetInstructionSet(parts[0]);
            Console.WriteLine(cmdtype);

            int literal = 0, dest = 0, op1 = 0, op2 = 0;
            if (parts.Length > 1)
            {
                if (parts[1].StartsWith("[") || parts[1].StartsWith("#"))
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
            else
            {
                throw new ArgumentException($"Uknown opcode: {operand}");
            }
        }
    }
}
