namespace DigitClustering
{
    public class DataHandler
    {
        public static (int[][] inputs, int[] targets) GetAllData()
        {
            (var hwd1Inputs, var hwd1Targets) = ImportData("Data/HWD1");
            (var hwd2Inputs, var hwd2Targets) = ImportData("Data/HWD2");
            (var hwd3Inputs, var hwd3Targets) = ImportData("Data/HWD3");
            (var hwd4Inputs, var hwd4Targets) = ImportData("Data/HWD4");

            int[][] inputs = new int[hwd1Inputs.Length + hwd2Inputs.Length + hwd3Inputs.Length + hwd4Inputs.Length][];
            int[] targets = new int[hwd1Targets.Length + hwd2Targets.Length + hwd3Targets.Length + hwd4Targets.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = new int[32];
            }

            Array.Copy(hwd1Inputs, inputs, hwd1Inputs.Length);
            Array.Copy(hwd2Inputs, 0, inputs, hwd1Inputs.Length, hwd2Inputs.Length);
            Array.Copy(hwd3Inputs, 0, inputs, hwd1Inputs.Length + hwd2Inputs.Length, hwd3Inputs.Length);
            Array.Copy(hwd2Inputs, 0, inputs, hwd1Inputs.Length + hwd2Inputs.Length + hwd3Inputs.Length, hwd4Inputs.Length);

            Array.Copy(hwd1Targets, targets, hwd1Targets.Length);
            Array.Copy(hwd2Targets, 0, targets, hwd1Targets.Length, hwd2Targets.Length);
            Array.Copy(hwd3Targets, 0, targets, hwd1Targets.Length + hwd2Targets.Length, hwd3Targets.Length);
            Array.Copy(hwd4Targets, 0, targets, hwd1Targets.Length + hwd2Targets.Length + hwd3Targets.Length, hwd4Targets.Length);

            return (inputs, targets);
        }
        private static (int[][] inputs, int[] targets) ImportData(string path)
        {
            int fileLines = CountFileLines(path);
            int sampleCount = fileLines / 33;

            int[][,] inputs = new int[sampleCount][,];
            for (int i = 0; i < sampleCount; i++)
            {
                inputs[i] = new int[32, 32];
            }

            int[] targets = new int[sampleCount];

            using (StreamReader sr = new(path))
            {
                int lineIndex = 0;

                while (true)
                {
                    string? currentLine = sr.ReadLine()?.Trim();

                    if (currentLine == null)
                        break;

                    if (lineIndex % 33 != 32)
                    {
                        for (int col = 0; col < 32; col++)
                        {
                            inputs[lineIndex / 33][lineIndex % 33, col] = Convert.ToInt32(currentLine[col] - 48);
                        }
                    }
                    else
                    {
                        targets[lineIndex / 33] = Convert.ToInt32(currentLine[0] - 48);
                    }
                    lineIndex++;
                }
            }

            int[][] castedInputs = new int[inputs.Length][];
            for (int i = 0; i < castedInputs.Length; i++)
            {
                castedInputs[i] = inputs[i].Cast<int>().ToArray();
            }

            return (castedInputs, targets);
        }
        private static int CountFileLines(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }
    }
}
