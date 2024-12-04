namespace CPUEmulator.Utils
{
    internal class Parser
    {
        public static void ParseFile(List<string> program, List<int> data, string localFilePath)
        {
            var solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var filePath = Path.Combine(solutionDirectory, localFilePath);

            var file = File.ReadLines(filePath);

            bool isData = false;
            foreach (var line in file)
            {
                if (line == ".data")
                {
                    isData = true;
                    continue;
                }
                else if (line == ".text")
                {
                    isData = false;
                    continue;
                }

                if (isData)
                {
                    var number = int.Parse(line.Split(' ')[1]);
                    data.Add(number);
                }
                else
                {
                    
                    program.Add(line);
                }
            }
        }
    }
}
