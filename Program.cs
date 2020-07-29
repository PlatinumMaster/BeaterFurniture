using System;
using System.Diagnostics;
using System.IO;

namespace BeaterFurniture
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                switch (args[0].ToLower())
                {
                    case "-d":
                        OverworldParser p = new OverworldParser(args[1], args[2]);
                        break;
                    case "-h":
                        Console.WriteLine(usage);
                        break;
                    case "-m":
                        string devkitPath;

                        // If this check fails, the user does not have devkitARM properly setup.
                        try
                        {
                            devkitPath = Path.Combine(Environment.GetEnvironmentVariable("DEVKITARM"), "bin");
                        }
                        catch (ArgumentNullException)
                        {
                            Console.WriteLine("Cannot detect a proper devkitARM setup. Exiting.");
                            return;
                        }

                        var filePath = Path.GetFileNameWithoutExtension(Path.GetFullPath(args[1]));
                        Process proc = Process.Start(Path.Combine(devkitPath, "arm-none-eabi-as"), $"-mthumb -c {args[1]} -o {filePath}.o");
                        proc.WaitForExit();
                        proc = Process.Start(Path.Combine(devkitPath, "arm-none-eabi-objcopy"), $"-O binary {filePath}.o {args[2]}");
                        proc.WaitForExit();
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine(usage);
            }
        }

        private static string usage = $"BeaterFurniture (currently for B2W2 only at the moment) --- Usage{Environment.NewLine}" +
            $"To decompile: BeaterFurniture -d <overworld> <output>{Environment.NewLine}" +
            $"To compile: BeaterFurniture -m <overworld> <output>";
    }
}
