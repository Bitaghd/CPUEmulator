using CPUEmulator.Utils;

namespace CPUEmulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Processor processor = new Processor();

            List<string> program = new();
            List<int> data_new = new();




            //var command = Assembler.Assemble("LOOP:");
            //Console.WriteLine("{0:B32}", command);
            string filePath = @"TestData\TestProgram.txt";
            Parser.ParseFile(program, data_new, filePath);

            processor.LoadMemory(data_new, 0);
            processor.LoadProgram(program);
            processor.Run();
        }
    }
}
