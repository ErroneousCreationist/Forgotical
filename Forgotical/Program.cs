namespace Forgotical.Application;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal class Program
{
    public const string VERSION = "0.0.3";

    public static void ExecuteCommand(string command)
    {
        Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = "/bin/bash";
        proc.StartInfo.Arguments = " -c \"" + command + " \"";
        proc.StartInfo.RedirectStandardOutput = true;
        //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        proc.Start();

        while (!proc.StandardOutput.EndOfStream)
        {
            Console.WriteLine(proc.StandardOutput.ReadLine());
        }
    }

    private static void Main(string[] args)
    {
        //if we are running an .fgt file with it, we get args, so we should use them
        if(args.Length>0)
        {
            RunGivenFile(args);
            return;
        }
        Console.Clear();
        Console.WriteLine("Welcome to the Forgotical interpreter (VERSION: "+VERSION+")");
        Console.WriteLine("The best new programming language that will... uh... I forgot.");
        Menu();

        void Menu()
        {
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Press 1 to use the SHELL, Press 2 to LOAD A CODE FILE AND RUN IT, Press 3 to BUILD A CODE FILE, Press 4 to exit");
            var input = Console.ReadLine();
            if (input == null || input == "")
            {
                Console.WriteLine("Please Enter Something");
                Menu();
                return;
            }
            switch (input.ToLower())
            {
                case "1":
                    InputAndRun();
                    break;
                case "2":
                    LoadFile();
                    break;
                case "3":
                    BuildGivenFile();
                    break;
                case "4":
                    Console.Clear();
                    return;
                default:
                    break;
            }
        }

        void BuildGivenFile()
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Clear();
                Console.WriteLine("Sorry, building isn't avaliable on Windows yet.");
                Console.WriteLine("--------------------------------------------------------------");
                Menu(); return;
            }

            Console.Clear();
            Console.WriteLine("Input the path to your file (.fgt), or -Q to cancel");
            var input = Console.ReadLine();
            if (input == null) { Console.Clear(); Menu(); return; }
            if(input=="-q") { Console.Clear(); Menu(); return; }
            if (!File.Exists(input))
            {
                Console.WriteLine("File doesn't exist!");
                Menu();
                return;
            }
            Console.WriteLine("BEGIN BUILD");
            var mydir = AppContext.BaseDirectory;
            Console.WriteLine("CREATING BUILT APP: "+ Directory.GetParent(input) + "/" + Path.GetFileName(input).Split('.')[0] + "_built.app");
            //checks if it already exists, deleting it if so
            if(Directory.Exists(Directory.GetParent(input) + "/" + Path.GetFileName(input).Split('.')[0] + "_built.app")) { Directory.Delete(Directory.GetParent(input) + "/" + Path.GetFileName(input).Split('.')[0] + "_built.app"); }
            //copies the default app
            CopyDirectory(mydir + "DefaultBuiltApp/Built.app", Directory.GetParent(input) + "/" + Path.GetFileName(input).Split('.')[0] + "_built.app", true);
            //copies over the interpreter library
            File.Copy(mydir + "Forgotical", Directory.GetParent(input) + "/" + Path.GetFileName(input).Split('.')[0] + "_built.app/Contents/MacOS/Forgotical");
            //copy over the source file
            File.Copy(input, Directory.GetParent(input) + "/" + Path.GetFileName(input).Split('.')[0] + "_built.app/Contents/MacOS/source.fgt");
            //make the runnable shell file in the app
            File.WriteAllText(Directory.GetParent(input) + "/" + Path.GetFileName(input).Split('.')[0] + "_built.app/Contents/MacOS/run.sh", $"#!/bin/bash\ncd \"$(dirname \"$0\")\"\nDIRECTORY=\"$(pwd)\"\nPATHTOAPP=\"$DIRECTORY/source.fgt\"\nCOMMAND=\"tell app \\\"Terminal\\\" to do script \\\"$DIRECTORY/Forgotical $PATHTOAPP -EXEC\\\"\"\nosascript \"-e $COMMAND\"");
            //ExecuteCommand($"chmod +x {Directory.GetParent(input) + "/" + Path.GetFileName(input).Split('.')[0] + "_built.app/Contents/MacOS/run"}");
            Console.WriteLine("BUILD COMPLETED");
            Console.WriteLine("Press any key to leave");
            Console.ReadKey();
            Menu();
        }

        void RunGivenFile(string[] args)
        {
            bool stepmode = args.Length == 2 && args[1] == "-step";
            bool executemode = args.Length == 2 && args[1] == "-EXEC";

            if (executemode)
            {
                ExecuteBuiltFile(args);
                return;
            }

            var input = args[0];
            Console.Clear();
            if (!File.Exists(input))
            {
                Console.WriteLine("File doesn't exist!");
                Console.WriteLine("--------------------------------------------------------------");
                Menu();
                return;
            }
            Console.WriteLine("Running file "+input);
            Console.WriteLine("--------------------------------------------------------------");
            var code = File.ReadAllText(input);
            var executor = new Interpreter(stepmode);
            bool success = executor.ExecuteCode(code, out string result);
            Console.WriteLine("\n--------------------------------------------------------------");
            var successstring = success ? "Successful" : "Failed with error";
            var rand = new Random();
            var stuff = new int[3];
            var rightindex = rand.Next(0, 3);
            stuff[0] = rightindex == 0 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
            stuff[1] = rightindex == 1 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
            stuff[2] = rightindex == 2 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
            var errorstring = success ? "" : $"The error occurred on... uh... line {stuff[0]}... or was it line {stuff[1]}? No no it was DEFINITELY line {stuff[2]}. I think.";
            Console.WriteLine($"Code finished executing. Result: \"{successstring}\". Output: \"{result}\". {errorstring}");
            //Console.WriteLine(executor.lines[executor.EXECUTION_LINE]);
            Console.WriteLine("Press any key to exit");
            return;
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        void ExecuteBuiltFile(string[] args)
        {
            Console.Clear();
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File doesn't exist!");
                Console.WriteLine("--------------------------------------------------------------");
                Menu();
                return;
            }

            var code = File.ReadAllText(args[0]);
            var executor = new Interpreter();
            bool success = executor.ExecuteCode(code, out string result);
            if (!success)
            {
                var rand = new Random();
                var stuff = new int[3];
                var rightindex = rand.Next(0, 3);
                stuff[0] = rightindex == 0 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
                stuff[1] = rightindex == 1 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
                stuff[2] = rightindex == 2 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
                var errorstring = $"The error occurred on... uh... line {stuff[0]}... or was it line {stuff[1]}? No no it was DEFINITELY line {stuff[2]}. I think.";
                Console.WriteLine($"Failed: \"{result}\" Line: \"{errorstring}\"");
            }
            else { Console.WriteLine(result); }
        }

        void LoadFile()
        {
            Console.Clear();
            Console.WriteLine("Input the path to your file (.fgt) or -Q to cancel");
            var input = Console.ReadLine();
            if (input == null) { Menu(); return; }
            if (input == "-q") { Console.Clear(); Menu(); return; }
            if (!File.Exists(input))
            {
                Console.WriteLine("File doesn't exist!");
                Menu();
                return;
            }
            Console.WriteLine("--------------------------------------------------------------");
            var code = File.ReadAllText(input);
            var executor = new Interpreter();
            bool success = executor.ExecuteCode(code, out string result);
            Console.WriteLine("\n--------------------------------------------------------------");
            var successstring = success ? "Successful" : "Failed with error";
            var rand = new Random();
            var stuff = new int[3];
            var rightindex = rand.Next(0, 3);
            stuff[0] = rightindex == 0 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
            stuff[1] = rightindex == 1 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
            stuff[2] = rightindex == 2 ? executor.EXECUTION_LINE + 1 : rand.Next(1, executor.lineslength + 1);
            var errorstring = success ? "" : $"The error occurred on... uh... line {stuff[0]}... or was it line {stuff[1]}? No no it was DEFINITELY line {stuff[2]}. I think.";
            Console.WriteLine($"Code finished executing. Result: \"{successstring}\". Output: \"{result}\". {errorstring}");
            //Console.WriteLine(executor.lines[executor.EXECUTION_LINE]);
            Console.WriteLine("Press any key to leave");
            Console.ReadKey();
            Menu();
        }

        void InputAndRun()
        {
            Console.Clear();
            Console.WriteLine("FORGOTICAL SHELL: "+VERSION+ " || Enter -Q to exit shell");
            Console.WriteLine("--------------------------------------------------------------");
            Dictionary<string, Interpreter.VariableStruct> VARIABLES = new Dictionary<string, Interpreter.VariableStruct>();
            Dictionary<string, Interpreter.PointerStruct> POINTERS = new Dictionary<string, Interpreter.PointerStruct>();
            string[] Lines = new string[0];
            int EXEC_LINE = 0;
            int MEMORY = 0;
            while (true)
            {
                var input = Console.ReadLine();
                if (input == null || input.ToLower() == "-q")
                {
                    Console.Clear();
                    Menu();
                    return;
                }
                var l = Lines.Length;
                Array.Resize(ref Lines, l + 1);
                Lines[l] = input;
                var executor = new Interpreter(VARIABLES, POINTERS, Lines, EXEC_LINE, MEMORY);
                bool success = executor.ShellExecuteLine(input, out string result);
                if (!success) { Console.WriteLine($"Error: \"{result}\""); continue; }
                VARIABLES = executor.VARIABLES;
                POINTERS = executor.POINTERS;
                EXEC_LINE += 1;
                MEMORY = executor.MEMORYAMOUNT;
                //foreach (var item in Lines)
                //{
                //    Console.WriteLine(item);
                //}
            }
        }
    }
}