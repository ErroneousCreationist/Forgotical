namespace Forgotical.Application;
using Forgotical.InternalUtility;
internal class Program
{
    public const string VERSION = "0.0.2";

    private static void Main(string[] args)
    {
        //if we are running an .fgt file with it, we get args, so we should use them
        if(args.Length>0)
        {
            RunGivenFile(args);
            return;
        }

        Console.WriteLine("Welcome to the Forgotical interpreter (VERSION: "+VERSION+")");
        Console.WriteLine("The best new programming language that will... uh... I forgot.");
        Menu();

        void Menu()
        {
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Press 1 to use the SHELL, Press 2 to LOAD A CODE FILE AND RUN IT, Press 3 to EXIT");
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
                    Console.Clear();
                    return;
                default:
                    break;
            }
        }

        void RunGivenFile(string[] args)
        {
            bool stepmode = args.Length == 2 && args[1] == "-step";
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
            Console.WriteLine("--------------------------------------------------------------");
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

        void LoadFile()
        {
            Console.Clear();
            Console.WriteLine("Input the path to your file (has to be a text file, preferably .fgt)");
            var input = Console.ReadLine();
            if (input == null) { Menu(); return; }
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
            Console.WriteLine("--------------------------------------------------------------");
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