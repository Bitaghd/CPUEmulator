namespace CPUEmulator
{
    public class Processor
    {
        private int[] cmem = new int[512]; // Память команд
        private int[] dmem = new int[512]; // Память данных
        private int[] reg = new int[8]; // Регистры 

        private int pc; // счетчик команд

        private bool flagZ = false;
        private bool flagS = false;

        public void LoadMemory(List<int> data, int startAddress)
        {
            for (int i = 0; i < data.Count; i++)
            {
                dmem[startAddress + i] = data[i];
            }
        }
        public void LoadProgram(List<string> program)
        {
            Assembler.LoadLabels(program);
            for (int i = 0; i < program.Count; i++)
            {
                var instruction = Assembler.Assemble(program[i], i);
                cmem[i] = instruction;
            }

        }

        public void Run()
        {
            pc = 0;
            while (pc < cmem.Length)
            {
                Execute(cmem[pc]);
            }
        }

        private void Execute(int cmd)
        {
            if(cmd == 0) // В памяти команд лейблы записываются нулевыми инструкциями
            {
                pc++;
                return;
            }
            int cmdtype = (cmd >> 28) & 0xF;
            int literal = (cmd >> 12) & 0xFFFF;
            int dest = (cmd >> 8) & 0xF;
            int op1 = (cmd >> 4) & 0xF;
            int op2 = cmd & 0xF;

            PrintState(cmd, cmdtype, dest, op1, op2, literal);

            switch (cmdtype)
            {
                case 0x1: // LOAD
                    if (literal > 0)
                        reg[dest] = literal;
                    else
                        reg[dest] = dmem[reg[op2]];
                    break;

                case 0x2: // ADD
                    if (op1 > 0 && op2 > 0)
                        reg[dest] = reg[op1] + reg[op2];
                    else if (literal > 0)            
                        reg[dest] = reg[op1] + literal;
                    UpdateFlags(reg[dest]);
                    break;

                case 0x3: // SUB
                    if (op1 > 0 && op2 > 0)
                        reg[dest] = reg[op1] - reg[op2];
                    else if (literal > 0)              
                        reg[dest] = reg[op1] - literal;
                    UpdateFlags(reg[dest]);
                    break;

                case 0x4: // JUMP
                    pc = literal;
                    return;

                case 0x5: //STORE
                    dmem[reg[dest]] = reg[op1];
                    break;

                case 0x6: //JUMPZ
                    if (flagZ) { pc = literal; return; }
                    break;

                case 0x7: //HALT
                    pc = cmem.Length;
                    return;

                case 0x8: //INC
                    reg[dest]++;
                    break;

                default:
                    Console.WriteLine($"Uknown command: {cmd}");
                    break;
            }
            pc++;
        }
        private void PrintState(int cmd, int opcode, int dest, int op1, int op2, int literal)
        {
            Console.WriteLine($"PC: {pc}");
            Console.WriteLine("Registers: " + string.Join(", ", reg));
            Console.WriteLine($"Flags: Z={flagZ}, S={flagS}");
            Console.WriteLine("Data: " + string.Join(", ", dmem[..10]));
            Console.WriteLine("Machine code: 0x{0:B4}_{1:B16}_{2:B4}_{3:B4}_{4:B4}", opcode, literal, dest, op1, op2);
            Console.WriteLine("Command: " + Assembler.GetCurrentInstruction(pc));
            Console.WriteLine($"Literal: {literal}, dest: {dest}, op1: {op1}, op2: {op2}");
            Console.WriteLine();
        }
        private void UpdateFlags(int result)
        {
            flagZ = result == 0; // Устанавливается, если результат равен 0
            flagS = result < 0;  // Устанавливается, если результат отрицательный
        }
    }
}
