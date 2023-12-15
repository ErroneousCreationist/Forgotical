using Forgotical.InternalUtility;

namespace Forgotical
{
	public class Interpreter
	{
        //classes for variables
        public class VariableStruct
        {
            private string Value;
            private int CLARITYLINESLEFT, COMPLETEDECAYLINESLEFT, LinesExistedFor;

            public VariableStruct(string value, int line)
            {
                Random random = new Random(line);
                Value = value;
                LinesExistedFor = 0;
                CLARITYLINESLEFT = random.Next(7,9);
                COMPLETEDECAYLINESLEFT = (int)(CLARITYLINESLEFT*((random.NextDouble()+1)*1.1));
            }

            public void LineChanged()
            {
                LinesExistedFor += 1;
                Random random = new Random();
                if (LinesExistedFor <COMPLETEDECAYLINESLEFT)
                {
                    if (LinesExistedFor > CLARITYLINESLEFT)
                    {
                        Value = Value.Insert(random.Next(0, Value.Length), Utility.RandomCharacter);
                    }
                }
                else
                {
                    Value = "";
                    for (int i = 0; i < random.Next(3, 8); i++)
                    {
                        Value += Utility.RandomCharacter;
                    }
                }
            }

            public void RefreshMemory()
            {
                LinesExistedFor = new Random().Next(-3,1);
            }

            public string Get
            {
                get
                {
                    return Value;
                }
            }
        }
        public class PointerStruct
        {
            private string Value, StupidMemoryValue;
            private int MEMORYTIME, LinesExistedFor, TARGET_LINE;

            public PointerStruct(string value, int line, int targetedline)
            {
                Random random = new Random(line);
                Value = value;
                StupidMemoryValue = "";
                LinesExistedFor = 0;
                MEMORYTIME = random.Next(8, 13);
                TARGET_LINE = targetedline;
            }

            public void LineChanged()
            {
                LinesExistedFor += 1;
                Random random = new Random();
                if (LinesExistedFor > MEMORYTIME)
                {
                    StupidMemoryValue = "";
                    for (int i = 0; i < random.Next(3, 8); i++)
                    {
                        StupidMemoryValue += Utility.RandomCharacter;
                    }
                }
            }

            public void RefreshMemory()
            {
                LinesExistedFor = new Random().Next(-3, 1);
            }
            public bool GetIsForgotten
            {
                get
                {
                    return LinesExistedFor > MEMORYTIME;
                }
            }
            public string Get
            {
                get
                {
                    return Value;
                }
            }
            public string GetStupidValue
            {
                get
                {
                    return Value;
                }
            }
            public int GetTargetLine
            {
                get
                {
                    return TARGET_LINE;
                }
            }

        }
        public class LinePointerStruct
        {
            private int STARTLINE, ENDLINE;
            private int MEMORYTIME, LinesExistedFor;
            private int LINESLENGTH;

            public LinePointerStruct(int start, int end, int line, int lineslength)
            {
                STARTLINE = start;
                ENDLINE = end;
                Random random = new Random(line);
                LinesExistedFor = 0;
                MEMORYTIME = random.Next(8, 13);
                LINESLENGTH = lineslength;
            }

            public void LineChanged()
            {
                LinesExistedFor += 1;
                if(LinesExistedFor > MEMORYTIME)
                {
                    Random rand = new Random();
                    if(LINESLENGTH==1)
                    {
                        STARTLINE = 1;
                        ENDLINE = 1;
                        return;
                    }
                    if(LINESLENGTH == 2)
                    {
                        STARTLINE = 1;
                        ENDLINE = 2;
                        return;
                    }
                    if (LINESLENGTH == 3)
                    {
                        STARTLINE = rand.Next(1, 3);
                        ENDLINE = 3;
                        return;
                    }
                    STARTLINE = rand.Next(1, LINESLENGTH / 2);
                    ENDLINE = rand.Next(LINESLENGTH / 2, LINESLENGTH + 1);
                }
            }

            public void RefreshMemory()
            {
                LinesExistedFor = new Random().Next(-3, 1);
            }
            public (int,int) Get
            {
                get
                {
                    return (STARTLINE, ENDLINE);
                }
            }
            
        }

        //enums
        private enum TypeOfOperatorEnum { None, AllocateMemory, Setvariable, Getvariable, AddToVariable, MinusFromVariable, DivideVariable,
            MultiplyVariable, DestroyVariable, RememberVariable, CreatePointer, GetPointer, DestroyPointer, GoToLine, OutputAndEnd, Print,
            PrintNoNewline, Input, AddString, GoToIf, IncrementPointer, DecrementPointer, GetPointerVariable, CreateConst, GetConst, DestroyConst,
            BranchGoto, StringOps, ReplaceCharacter, CharAtIndex, RunCommand, GetFilePath, GetPathToLocation, CreateLinePointer, ExecuteLinePointer,
            DestroyLinePointer, RemoveCharacter, ContainsCharacter, SanitiseNumber, IsNumber, IsTextNumber }
        private enum ComparisionEnum { Invalid, Equal, Over, Under, OverEqual, UnderEqual, NotEqual }
        private enum StringOpEnum { Invalid, Length, Lower, Upper, Reverse }

        //variable dictionaries
        public Dictionary<string, VariableStruct> VARIABLES = new Dictionary<string, VariableStruct>();
        public Dictionary<string, PointerStruct> POINTERS = new Dictionary<string, PointerStruct>();
        public Dictionary<string, string> CONSTS = new Dictionary<string, string>();
        public Dictionary<string, LinePointerStruct> LINEPTRS = new Dictionary<string, LinePointerStruct>();

        //values
        public int MEMORYAMOUNT = 0;
        public int EXECUTION_LINE = 1;
        private bool ProgramEnded;
        private bool wasrefreshline = false;
        private string Output = "";
        public int lineslength;
        public string[] lines;
        private bool STEP_MODE, SHELL_MODE;
        private string CodeFilePath;

        //switch control lines for line pointers
        private List<(int, int)> SWITCH_CONTROL_LINES = new List<(int, int)>();

        //default constructpr
		public Interpreter(bool step_mode = false, string codefilepath = "")
		{
			VARIABLES = new Dictionary<string, VariableStruct>();
            MEMORYAMOUNT = 0;
            Output = "";
            ProgramEnded = false;
            EXECUTION_LINE = 0;
            lines = new string[0];
            STEP_MODE = step_mode;
            CodeFilePath = codefilepath;
		}

        //constructor for shell mode
        public Interpreter(string codefilepath, Dictionary<string, VariableStruct> vars, Dictionary<string, PointerStruct> pointers, string[] prevlines, int execline, int mem)
        {
            VARIABLES = new Dictionary<string, VariableStruct>(vars);
            POINTERS = new Dictionary<string, PointerStruct>(pointers);
            MEMORYAMOUNT = mem;
            Output = "";
            ProgramEnded = false;
            EXECUTION_LINE = prevlines.Length-1;
            lines = prevlines;
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Utility.TranslateQuotedStrings(lines[i]);
            }
            STEP_MODE = false;
            SHELL_MODE = true;
            CodeFilePath = codefilepath;
        }

        //shell mode execute code
        public bool ShellExecuteLine(string code, out string Result)
        {
            code = Utility.TranslateQuotedStrings(code); //sanitise all of the strings in quotation marks to make them safe and not throw errors

            if (code.Contains(' ')) { Output = ""; Result = Errors.ERROR_MESSAGES["error_space"].GetChoice(); return false; }
            if (code.IndexOfAny("1234567890".ToCharArray()) != -1) { Output = ""; ProgramEnded = true; Result = Errors.ERROR_MESSAGES["error_numbers"].GetChoice(); return false; }
            //Utility.UnTranslateString(Utility.GetStringsInQuotes(code));
            if (!ExecuteLine(lines[EXECUTION_LINE], out string tempresult))
            {
                Output = tempresult;
                ProgramEnded = true;
                Result = tempresult;
                return false;
            }
            Result = Output;
            return true;
        }

        //execute code
        public bool ExecuteCode(string code, out string Result)
        {
            //whitespace error because we hate whitespace here
            code = Utility.TranslateQuotedStrings(code); //sanitise all of the strings in quotation marks to make them safe and not throw errors
            var resultant = code.Split("\n");
            lineslength = resultant.Length;
            if (code.Contains(' ')) { Output = ""; Result = Errors.ERROR_MESSAGES["error_space"].GetChoice(); return false; }
            if(code.IndexOfAny("1234567890".ToCharArray()) != -1) { Output = ""; ProgramEnded = true; Result = Errors.ERROR_MESSAGES["error_numbers"].GetChoice(); return false; }

            //recreate to sanitise - to space so we can actually write stuff normally
            resultant = code.Split("\n");
            lineslength = resultant.Length;
            lines = new string[lineslength];
            resultant.CopyTo(lines, 0);

            while (!ProgramEnded)
            {
                wasrefreshline = false;
                if (EXECUTION_LINE == lineslength && !ProgramEnded) { Output = ""; ProgramEnded = true; break; }
                int line = EXECUTION_LINE;
                if (!ExecuteLine(resultant[EXECUTION_LINE], out string tempresult))
                {
                    Output = tempresult;
                    ProgramEnded = true;
                    Result = tempresult;
                    return false;
                }
                Random rand = new Random();
                var memoryupdateprobability = 1 - MathF.Pow(MathF.E, -(0.01f * MEMORYAMOUNT));
                if (rand.NextDouble() < 1 - memoryupdateprobability && !wasrefreshline)
                {
                    foreach (var variable in VARIABLES)
                    {
                        variable.Value.LineChanged();
                    }
                    foreach (var pointer in POINTERS)
                    {
                        pointer.Value.LineChanged();
                    }
                }
                for (int i = 0; i < SWITCH_CONTROL_LINES.Count; i++)
                {
                    if (EXECUTION_LINE == SWITCH_CONTROL_LINES[i].Item1)
                    {
                        EXECUTION_LINE = SWITCH_CONTROL_LINES[i].Item2;
                        SWITCH_CONTROL_LINES.RemoveAt(i);
                        break;
                    }
                }
                EXECUTION_LINE++;
                if(STEP_MODE)
                {
                    Console.WriteLine($"Current line: {line + 1}. Line content: {resultant[line]}");
                    foreach (var item in VARIABLES)
                    {
                        Console.WriteLine($"variable {item.Key} has a value of {item.Value.Get}");
                    }
                    Console.Write("\n");
                    Console.ReadKey();
                }
            }
            Result = Output;
            return true;

            //if (AlreadySucceeded)
            //{
                
            //}
            //else
            //{
            //    Result = Output;
            //    return false;
            //}
        }

        //execute a single expression on a line
        private bool ExecuteLine(string thecode, out string RESULT)
        {
            string line = new string(thecode.ToCharArray());
            if (line == "") { RESULT = ""; return true; }
            if (line[0] == '\\') { RESULT = ""; return true; } //comments
            TypeOfOperatorEnum op = TypeOfOperatorEnum.None;
            //Console.WriteLine(line);
            //do the sub-expressions in the args place. for example, having Output:(GetVariable:name) would make it get that variable and replace the bracketed thing with that variable.
            while (true)
            {
                var submethods = Utility.GetStringsInParentheses(line);
                if (submethods.Count <= 0) { break; }
                var sublineresult = ExecuteSubLine(submethods[0], out string subresult);
                //Console.WriteLine("SUBLINE DEBUG: original " + "(" + submethods[0] + ")" + " new " + sublineresult.Item2);
                if (!sublineresult.Item1) { RESULT = subresult; return false; }
                else { line = line.Replace("(" + submethods[0] + ")", sublineresult.Item2); lines[EXECUTION_LINE] = line; }
            }

            //figure out if the line is wrapped in error handling
            bool errorhandled = line[0] == '{' && line[line.Length - 1] == '}';
            line = line.Replace("{", "");
            line = line.Replace("}", "");

            //sanitise but keep original so we can verify that we are protecting our pointers
            string origline = line;
            line = line.Replace("[", "");
            line = line.Replace("]", "");

            //split it
            var expressions = line.Split(":");

            //no expressions is wrong, exit and return error
            if (expressions.Length == 0) { RESULT = Errors.ERROR_MESSAGES["error_noexp"].GetChoice(); return errorhandled; }
            if (expressions.Length < 2) { RESULT = Errors.ERROR_MESSAGES["error_noargs"].GetChoice(); return errorhandled; }
            if (expressions.Length > 2) { RESULT = Errors.ERROR_MESSAGES["error_badexpstatement"].GetChoice(); return errorhandled; }

            //figure out which expression we need
            switch (expressions[0])
            {
                case "Commit":
                    op = TypeOfOperatorEnum.Setvariable;
                    break;
                case "Retrieve":
                    op = TypeOfOperatorEnum.Getvariable;
                    break;
                case "Remember":
                    op = TypeOfOperatorEnum.RememberVariable;
                    break;
                case "MakeRoomForTheMemory":
                    op = TypeOfOperatorEnum.AllocateMemory;
                    break;
                case "AddAVariableToTheFollowingVariable":
                    op = TypeOfOperatorEnum.AddToVariable;
                    break;
                case "SubtractAVariableFromTheFollowingVariable":
                    op = TypeOfOperatorEnum.MinusFromVariable;
                    break;
                case "DivideTheFollowingVariable":
                    op = TypeOfOperatorEnum.DivideVariable;
                    break;
                case "MultiplyTheFollowingVariable":
                    op = TypeOfOperatorEnum.MultiplyVariable;
                    break;
                case "PleaseDestroyAndForgetTheFollowingVariableForeverYesIAmSure":
                    op = TypeOfOperatorEnum.DestroyVariable;
                    break;
                case "CommitPointer":
                    op = TypeOfOperatorEnum.CreatePointer;
                    break;
                case "GetTheVariableDescribedByThePointerPlease":
                    op = TypeOfOperatorEnum.GetPointer;
                    break;
                case "DestroyThisThingThatPointsYesIAmSure":
                    op = TypeOfOperatorEnum.DestroyPointer;
                    break;
                case "PleaseGoBackOrForwardToReachTheFollowingLine":
                    op = TypeOfOperatorEnum.GoToLine;
                    break;
                case "EndTheProgramAndOutputTheFollowingYesIAmCompletelySure":
                    op = TypeOfOperatorEnum.OutputAndEnd;
                    break;
                case "PleaseAttemptToOutputTheFollowing":
                    op = TypeOfOperatorEnum.Print;
                    break;
                case "OutputTheFollowingWithNoNewline":
                    op = TypeOfOperatorEnum.PrintNoNewline;
                    break;
                case "PleaseGetInputFromTheUser":
                    op = TypeOfOperatorEnum.Input;
                    break;
                case "Concatnate":
                    op = TypeOfOperatorEnum.AddString;
                    break;
                case "ConditionalGoto":
                    op = TypeOfOperatorEnum.GoToIf;
                    break;
                case "PointerLineGoUp":
                    op = TypeOfOperatorEnum.IncrementPointer;
                    break;
                case "PointerLineGoDown":
                    op = TypeOfOperatorEnum.DecrementPointer;
                    break;
                case "GetThePointersVariable":
                    op = TypeOfOperatorEnum.GetPointerVariable;
                    break;
                case "MemoriseValueToLongTermMemory":
                    op = TypeOfOperatorEnum.CreateConst;
                    break;
                case "RetrieveFromLongTerm":
                    op = TypeOfOperatorEnum.GetConst;
                    break;
                case "ForgetValueFromLongTermMemory":
                    op = TypeOfOperatorEnum.DestroyConst;
                    break;
                case "BranchLikeATree":
                    op = TypeOfOperatorEnum.BranchGoto;
                    break;
                case "StrOp":
                    op = TypeOfOperatorEnum.StringOps;
                    break;
                case "Replace":
                    op = TypeOfOperatorEnum.ReplaceCharacter;
                    break;
                case "GetAtIndex":
                    op = TypeOfOperatorEnum.CharAtIndex;
                    break;
                case "ExecCommand":
                    op = TypeOfOperatorEnum.RunCommand;
                    break;
                case "GetThisPath":
                    op = TypeOfOperatorEnum.GetFilePath;
                    break;
                case "CommitLinePointer":
                    op = TypeOfOperatorEnum.CreateLinePointer;
                    break;
                case "DoLinePointer":
                    op = TypeOfOperatorEnum.ExecuteLinePointer;
                    break;
                case "ObliterateLinePointer":
                    op = TypeOfOperatorEnum.DestroyLinePointer;
                    break;
                case "RemoveCharacter":
                    op = TypeOfOperatorEnum.RemoveCharacter;
                    break;
                case "ContainsCharacter":
                    op = TypeOfOperatorEnum.ContainsCharacter;
                    break;
                case "NumToWord":
                    op = TypeOfOperatorEnum.SanitiseNumber;
                    break;
                case "IsANumber":
                    op = TypeOfOperatorEnum.IsNumber;
                    break;
                case "IsATextNumber":
                    op = TypeOfOperatorEnum.IsTextNumber;
                    break;
                default:
                    //if our operator does not exist then what happened
                    RESULT = Errors.ERROR_MESSAGES["error_badexp"].GetChoice();
                    return errorhandled;
            }

            //check for unprotected unsafe code
            if((op==TypeOfOperatorEnum.CreatePointer|| op == TypeOfOperatorEnum.IncrementPointer || op == TypeOfOperatorEnum.DecrementPointer || op ==TypeOfOperatorEnum.GetPointer||op==TypeOfOperatorEnum.DestroyPointer || op==TypeOfOperatorEnum.RunCommand || op==TypeOfOperatorEnum.CreateLinePointer || op==TypeOfOperatorEnum.ExecuteLinePointer || op==TypeOfOperatorEnum.DestroyLinePointer) && (origline[0] != '[' || origline[origline.Length-1] != ']'))
            {
                RESULT = Errors.ERROR_MESSAGES["error_unsafe"].GetChoice(); return errorhandled;
            }

            //get args (separated by commas
            string[] args = expressions[1].Split(",");
            if(args.Length == 0) { RESULT = Errors.ERROR_MESSAGES["error_noargs"].GetChoice(); return errorhandled; }

            //de-sanitise args
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].Contains("'")) { continue; }
                args[i] = args[i].Replace("'", "");
                args[i] = Utility.UnTranslateString(args[i]);
                args[i] = args[i].Replace("~'", "'");
            }

            //code for all of the expressions
            switch (op)
            {
                case TypeOfOperatorEnum.None:
                    RESULT = "";
                    return true;
                case TypeOfOperatorEnum.AllocateMemory:
                    if(args.Length>1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (WordsToNumber.ConvertToNumbers(args[0])<=0)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    Random rand = new Random(EXECUTION_LINE);
                    if(rand.NextDouble()>0.5f && WordsToNumber.ConvertToNumbers(args[0]) > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomuchmemory"].GetChoice(); return errorhandled;
                    }
                    MEMORYAMOUNT += (int)WordsToNumber.ConvertToNumbers(args[0]);
                    break;
                case TypeOfOperatorEnum.Setvariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(args[1], EXECUTION_LINE);
                    }
                    else
                    {
                        if (MEMORYAMOUNT <= 0)
                        {
                            RESULT = Errors.ERROR_MESSAGES["error_nomemory"].GetChoice(); return errorhandled;
                        }
                        MEMORYAMOUNT -= 1;
                        VARIABLES.Add(args[0], new VariableStruct(args[1], EXECUTION_LINE));
                    }
                    break;
                case TypeOfOperatorEnum.Getvariable:
                    //IMPLEMENT THIS ELSEWHERE, IT DOESN'T REALLY DO ANYTHING HERE (if its in brackets it replaces itself with the appropriate value)
                    break;
                case TypeOfOperatorEnum.AddToVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    }
                    var num1 = WordsToNumber.ConvertToNumbers(VARIABLES[args[0]].Get);
                    var num2 = WordsToNumber.ConvertToNumbers(args[1]);
                    VARIABLES[args[0]] = new VariableStruct(NumberToWords.ConvertToWords(num1 + num2), EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.MinusFromVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    }
                    var subnum1 = WordsToNumber.ConvertToNumbers(VARIABLES[args[0]].Get);
                    var subnum2 = WordsToNumber.ConvertToNumbers(args[1]);

                    VARIABLES[args[0]] = new VariableStruct(NumberToWords.ConvertToWords(subnum1 - subnum2), EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.DivideVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    }
                    var divnum1 = WordsToNumber.ConvertToNumbers(VARIABLES[args[0]].Get);
                    var divnum2 = WordsToNumber.ConvertToNumbers(args[1]);
                    if(divnum2 == 0) { RESULT = Errors.ERROR_MESSAGES["error_divzero"].GetChoice(); return errorhandled; }
                    VARIABLES[args[0]] = new VariableStruct(NumberToWords.ConvertToWords(divnum1 / divnum2), EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.MultiplyVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    }
                    var mulnum1 = WordsToNumber.ConvertToNumbers(VARIABLES[args[0]].Get);
                    var mulnum2 = WordsToNumber.ConvertToNumbers(args[1]);

                    VARIABLES[args[0]] = new VariableStruct(NumberToWords.ConvertToWords(mulnum1 * mulnum2), EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.DestroyVariable:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    }
                    MEMORYAMOUNT += 1;
                    VARIABLES.Remove(args[0]);
                    break;
                case TypeOfOperatorEnum.RememberVariable:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]) && !POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_cantrefresh"].GetChoice(); return errorhandled;
                    }
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]].RefreshMemory();
                    }
                    else if (POINTERS.ContainsKey(args[0]))
                    {
                        POINTERS[args[0]].RefreshMemory();
                    }
                    wasrefreshline = true;
                    break;
                case TypeOfOperatorEnum.CreatePointer:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    //if (!VARIABLES.ContainsKey(args[1]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    //}
                    var targetline = (int)WordsToNumber.ConvertToNumbers(args[1])-1;
                    if(targetline < 0 || targetline >= lineslength || targetline >= EXECUTION_LINE)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled;
                    }
                    var destlineexp = lines[targetline].Split(":");
                    if (destlineexp[0] != "Commit") { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled; }
                    var pointertargetvariable = destlineexp[1].Split(",")[0];
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        POINTERS.Add(args[0], new PointerStruct(pointertargetvariable, EXECUTION_LINE, targetline));
                    }
                    else
                    {
                        POINTERS[args[0]] = new PointerStruct(pointertargetvariable, EXECUTION_LINE, targetline);
                    }
                    break;
                case TypeOfOperatorEnum.GetPointer:
                    //THIS ONLY DOES SOEMTHING WHEN ITS A SUB EXPRESSION (in brackets)
                    //if (args.Length > 1)
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    //}
                    //if (!POINTERS.ContainsKey(args[0]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return errorhandled;
                    //}
                    //VARIABLES[POINTERS[args[0]]];
                    break;
                case TypeOfOperatorEnum.GetPointerVariable:
                    //DO THIS IN SUB EXPRESSIONS
                    break;
                case TypeOfOperatorEnum.DestroyPointer:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return errorhandled;
                    }
                    POINTERS.Remove(args[0]);
                    break;
                case TypeOfOperatorEnum.IncrementPointer:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return errorhandled;
                    }
                    //if (!VARIABLES.ContainsKey(args[1]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    //}
                    var inctargetline = POINTERS[args[0]].GetTargetLine - (int)WordsToNumber.ConvertToNumbers(args[1]);
                    if (inctargetline < 0 || inctargetline >= lineslength)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled;
                    }
                    var incdestlineexp = lines[inctargetline].Split(":");
                    if (incdestlineexp[0] != "Commit") { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled; }
                    if(incdestlineexp.Length > 2) { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled; } //a line like this clearly hasn't been interpreted, meaning it has brackets, so we cannot point to it
                    var incpointertargetvariable = incdestlineexp[1].Split(",")[0];
                    if (!VARIABLES.ContainsKey(incpointertargetvariable)) { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled; } //if the variable described hasnt been created yet, we are looking into the future into what hasn't been interpreted yet, so we cannot point to this
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        POINTERS.Add(args[0], new PointerStruct(incpointertargetvariable, EXECUTION_LINE, inctargetline));
                    }
                    else
                    {
                        POINTERS[args[0]] = new PointerStruct(incpointertargetvariable, EXECUTION_LINE, inctargetline);
                    }
                    break;
                case TypeOfOperatorEnum.DecrementPointer:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return errorhandled;
                    }
                    //if (!VARIABLES.ContainsKey(args[1]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    //}
                    var dectargetline = POINTERS[args[0]].GetTargetLine + (int)WordsToNumber.ConvertToNumbers(args[1]);
                    if (dectargetline < 0 || dectargetline >= lineslength)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled;
                    }
                    var decdestlineexp = lines[dectargetline].Split(":");
                    if (decdestlineexp[0] != "Commit") { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled; }
                    if (decdestlineexp.Length > 2) { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled; } //a line like this clearly hasn't been interpreted, meaning it has brackets, so we cannot point to it
                    var decpointertargetvariable = decdestlineexp[1].Split(",")[0];
                    if (!VARIABLES.ContainsKey(decpointertargetvariable)) { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return errorhandled; } //if the variable described hasnt been created yet, we are looking into the future into what hasn't been interpreted yet, so we cannot point to this
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        POINTERS.Add(args[0], new PointerStruct(decpointertargetvariable, EXECUTION_LINE, dectargetline));
                    }
                    else
                    {
                        POINTERS[args[0]] = new PointerStruct(decpointertargetvariable, EXECUTION_LINE, dectargetline);
                    }
                    break;
                case TypeOfOperatorEnum.GoToLine:
                    if (SHELL_MODE)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_shellgoto"].GetChoice(); return errorhandled;
                    }
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    var targetlinegoto = (int)WordsToNumber.ConvertToNumbers(args[0]) - 2;
                    if (targetlinegoto >= lineslength || targetlinegoto < -1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return errorhandled;
                    }
                    EXECUTION_LINE = targetlinegoto;
                    RESULT = "";
                    return true;
                case TypeOfOperatorEnum.GoToIf:
                    if(SHELL_MODE)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_shellgoto"].GetChoice(); return errorhandled;
                    }
                    if (args.Length > 5)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if(args.Length<5)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }

                    var type = ComparisionEnum.Invalid;
                    switch (args[2])
                    {
                        case "Equal":
                            type = ComparisionEnum.Equal;
                            break;
                        case "Over":
                            type = ComparisionEnum.Over;
                            break;
                        case "Under":
                            type = ComparisionEnum.Under;
                            break;
                        case "NotEqual":
                            type = ComparisionEnum.NotEqual;
                            break;
                        case "OverEqual":
                            type = ComparisionEnum.OverEqual;
                            break;
                        case "UnderEqual":
                            type = ComparisionEnum.UnderEqual;
                            break;
                        default:
                            type = ComparisionEnum.Invalid;
                            RESULT = Errors.ERROR_MESSAGES["error_badcomp"].GetChoice();
                            return errorhandled;
                    }
                    dynamic value1 = type == ComparisionEnum.Equal || type == ComparisionEnum.NotEqual ? args[0] : WordsToNumber.ConvertToNumbers(args[0]);
                    dynamic value2 = type == ComparisionEnum.Equal || type == ComparisionEnum.NotEqual ? args[1] : WordsToNumber.ConvertToNumbers(args[1]);

                    //Console.WriteLine(value1 + " compared to " + value2); ;
                    var result = errorhandled;
                    switch (type)
                    {
                        case ComparisionEnum.Equal:
                            result = value1 == value2;
                            break;
                        case ComparisionEnum.Over:
                            result = value1 > value2;
                            break;
                        case ComparisionEnum.Under:
                            result = value1 < value2;
                            break;
                        case ComparisionEnum.NotEqual:
                            result = value1 != value2;
                            break;
                        case ComparisionEnum.UnderEqual:
                            result = value1 <= value2;
                            break;
                        case ComparisionEnum.OverEqual:
                            result = value1 >= value2;
                            break;
                    }

                    var targetlinegotoif = result ? (int)WordsToNumber.ConvertToNumbers(args[3]) - 2 : (int)WordsToNumber.ConvertToNumbers(args[4]) - 2;
                    //Console.WriteLine(targetlinegotoif);
                    if (targetlinegotoif >= lineslength || targetlinegotoif < -1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return errorhandled;
                    }
                    EXECUTION_LINE = targetlinegotoif;
                    RESULT = "";
                    return true;
                case TypeOfOperatorEnum.OutputAndEnd:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    //if (!VARIABLES.ContainsKey(args[0]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    //}
                    Output = args[0];
                    RESULT = args[0];
                    ProgramEnded = true;
                    return true;
                case TypeOfOperatorEnum.Print:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    Console.WriteLine(args[0]);
                    break;
                case TypeOfOperatorEnum.PrintNoNewline:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    Console.Write(args[0]);
                    break;
                case TypeOfOperatorEnum.Input:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    var read = Console.ReadLine();
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(read ?? "", EXECUTION_LINE);
                    }
                    break;
                case TypeOfOperatorEnum.AddString:
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    }
                    string newstr = VARIABLES[args[0]].Get;
                    for (int i = 1; i < args.Length; i++)
                    {
                        newstr += args[i];
                    }
                    VARIABLES[args[0]] = new VariableStruct(newstr, EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.CreateConst:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (CONSTS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_alrconst"].GetChoice(); return errorhandled;
                    }
                    if (MEMORYAMOUNT < 3)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_constmemory"].GetChoice(); return errorhandled;
                    }
                    MEMORYAMOUNT -= 3;
                    CONSTS.Add(args[0], args[1]);
                    break;
                case TypeOfOperatorEnum.GetConst:
                    break;
                case TypeOfOperatorEnum.DestroyConst:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (!CONSTS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingconst"].GetChoice(); return errorhandled;
                    }
                    CONSTS.Remove(args[0]);
                    break;
                case TypeOfOperatorEnum.BranchGoto:
                    if (SHELL_MODE)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_shellgoto"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 4)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if ((args.Length-2) % 2 != 0)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badbranches"].GetChoice(); return errorhandled;
                    }
                    //get the branches since they are COMPARISIONVALUE, LINE, COMPARISIONVALUE, LINE, and so on
                    List<(string, string)> branches = new List<(string, string)>();
                    for (int i = 2; i < args.Length; i++)
                    {
                        if(i%2!=0)
                        {
                            branches.Add((args[i - 1], args[i]));
                        }
                    }
                    //loop through them and compare
                    foreach (var item in branches)
                    {
                        if (args[0]==item.Item1)
                        {
                            var targetlinegotobranch = (int)WordsToNumber.ConvertToNumbers(item.Item2) - 2;
                            if (targetlinegotobranch >= lineslength || targetlinegotobranch < -1)
                            {
                                RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return errorhandled;
                            }
                            EXECUTION_LINE = targetlinegotobranch;
                            RESULT = "";
                            return true;
                        }
                    }
                    //if we don't get any of them, do the default
                    var deftargetlinegotobranch = (int)WordsToNumber.ConvertToNumbers(args[1]) - 2;
                    if (deftargetlinegotobranch >= lineslength || deftargetlinegotobranch < -1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return errorhandled;
                    }
                    EXECUTION_LINE = deftargetlinegotobranch;
                    RESULT = "";
                    return true;
                case TypeOfOperatorEnum.StringOps:
                    if(args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if(args.Length>2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    //IMPLEMENT THIS FOR SUB-EXPRESSIONS
                    break;
                case TypeOfOperatorEnum.ReplaceCharacter:
                    if (args.Length < 3)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length > 3)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]) && !CONSTS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    }
                    if(!CONSTS.ContainsKey(args[0]))
                    {
                        var initialvalue = VARIABLES[args[0]].Get;
                        VARIABLES[args[0]] = new VariableStruct(initialvalue.Replace(args[1], args[2]), EXECUTION_LINE);
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        var initialvalue = CONSTS[args[0]];
                        CONSTS[args[0]] = initialvalue.Replace(args[1], args[2]);
                    }
                    break;
                case TypeOfOperatorEnum.CharAtIndex:
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    //IMPLEMENT IN SUB EXPRESSION
                    break;
                case TypeOfOperatorEnum.RunCommand:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    Utility.ExecuteCommand(args[0]);
                    break;
                case TypeOfOperatorEnum.GetFilePath:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    }
                    VARIABLES[args[0]] = new VariableStruct(CodeFilePath, EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.CreateLinePointer:
                    if (args.Length > 3)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (args.Length < 3)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return errorhandled;
                    }
                    var targ1 = (int)WordsToNumber.ConvertToNumbers(args[1])-2;
                    var targ2 = (int)WordsToNumber.ConvertToNumbers(args[2])-1;
                    if (targ1 >= lineslength || targ1 < -1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return errorhandled;
                    }
                    if (targ2 >= lineslength || targ2 < 0 || targ2<targ1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return errorhandled;
                    }
                    if (LINEPTRS.ContainsKey(args[0]))
                    {
                        LINEPTRS[args[0]] = new LinePointerStruct(targ1, targ2, EXECUTION_LINE, lineslength);
                    }
                    else
                    {
                        LINEPTRS.Add(args[0], new LinePointerStruct(targ1, targ2, EXECUTION_LINE, lineslength));
                    }
                    break;
                case TypeOfOperatorEnum.ExecuteLinePointer:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (!LINEPTRS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missinglineptr"].GetChoice(); return errorhandled;
                    }
                    var lineptr = LINEPTRS[args[0]].Get;
                    var target = lineptr.Item1;
                    //Console.WriteLine("first target " + target);
                    if (target >= lineslength || target < -1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return errorhandled;
                    }
                    var target2 = lineptr.Item2;
                    //Console.WriteLine("second target " + target2);
                    if (target2 >= lineslength || target2 < 0 || target2 < target)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return errorhandled;
                    }
                    SWITCH_CONTROL_LINES.Add((target2, EXECUTION_LINE));
                    EXECUTION_LINE = target;
                    RESULT = "";
                    return true;
                case TypeOfOperatorEnum.DestroyLinePointer:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    if (!LINEPTRS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missinglineptr"].GetChoice(); return errorhandled;
                    }
                    LINEPTRS.Remove(args[0]);
                    break;
                case TypeOfOperatorEnum.RemoveCharacter:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    //IMPLEMENT IN SUB EXPRESSION
                    break;
                case TypeOfOperatorEnum.ContainsCharacter:
                    if(args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    //IMPLEMENT IN SUB EXPRESSION
                    break;
                case TypeOfOperatorEnum.SanitiseNumber:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    //IMPLEMENT IN SUB EXPRESSION
                    break;
                case TypeOfOperatorEnum.IsNumber:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    //IMPLEMENT IN SUB EXPRESSION
                    break;
                case TypeOfOperatorEnum.IsTextNumber:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return errorhandled;
                    }
                    //IMPLEMENT IN SUB EXPRESSION
                    break;
            }
            RESULT = "";
            return true;
        }

        //execute a sub expression
        private (bool,string) ExecuteSubLine(string line, out string RESULT)
        {
            if (line[0] == '\\') { RESULT = ""; return (true,""); } //comments
            if (line == "") { RESULT = ""; return (true, ""); }
            TypeOfOperatorEnum op = TypeOfOperatorEnum.None;

            //figure out if the line is wrapped in error handling
            bool errorhandled = line[0] == '{' && line[line.Length - 1] == '}';
            line = line.Replace("{", "");
            line = line.Replace("}", "");

            //sanitise the angle brackets but keep it around to check if we are protecting our pointers
            string origline = line;
            line = line.Replace("[", "");
            line = line.Replace("]", "");

            var expressions = line.Split(":");

            //no expressions is wrong, exit and return error
            if (expressions.Length == 0) { RESULT = Errors.ERROR_MESSAGES["error_noexp"].GetChoice(); return (errorhandled, ""); ; }
            if (expressions.Length > 2) { RESULT = Errors.ERROR_MESSAGES["error_badexpstatement"].GetChoice(); return (errorhandled, ""); ; }
            if (expressions.Length < 2) { RESULT = Errors.ERROR_MESSAGES["error_noargs"].GetChoice(); return (errorhandled, ""); ; }

            //figure out which expression we need
            switch (expressions[0])
            {
                case "Commit":
                    op = TypeOfOperatorEnum.Setvariable;
                    break;
                case "Retrieve":
                    op = TypeOfOperatorEnum.Getvariable;
                    break;
                case "Remember":
                    op = TypeOfOperatorEnum.RememberVariable;
                    break;
                case "MakeRoomForTheMemory":
                    op = TypeOfOperatorEnum.AllocateMemory;
                    break;
                case "AddAVariableToTheFollowingVariable":
                    op = TypeOfOperatorEnum.AddToVariable;
                    break;
                case "SubtractAVariableFromTheFollowingVariable":
                    op = TypeOfOperatorEnum.MinusFromVariable;
                    break;
                case "DivideTheFollowingVariable":
                    op = TypeOfOperatorEnum.DivideVariable;
                    break;
                case "MultiplyTheFollowingVariable":
                    op = TypeOfOperatorEnum.MultiplyVariable;
                    break;
                case "PleaseDestroyAndForgetTheFollowingVariableForeverYesIAmSure":
                    op = TypeOfOperatorEnum.DestroyVariable;
                    break;
                case "CommitPointer":
                    op = TypeOfOperatorEnum.CreatePointer;
                    break;
                case "GetTheVariableDescribedByThePointerPlease":
                    op = TypeOfOperatorEnum.GetPointer;
                    break;
                case "DestroyThisThingThatPointsYesIAmSure":
                    op = TypeOfOperatorEnum.DestroyPointer;
                    break;
                case "PleaseGoBackOrForwardToReachTheFollowingLine":
                    op = TypeOfOperatorEnum.GoToLine;
                    break;
                case "EndTheProgramAndOutputTheFollowingYesIAmCompletelySure":
                    op = TypeOfOperatorEnum.OutputAndEnd;
                    break;
                case "PleaseAttemptToOutputTheFollowing":
                    op = TypeOfOperatorEnum.Print;
                    break;
                case "OutputTheFollowingWithNoNewline":
                    op = TypeOfOperatorEnum.PrintNoNewline;
                    break;
                case "PleaseGetInputFromTheUser":
                    op = TypeOfOperatorEnum.Input;
                    break;
                case "Concatnate":
                    op = TypeOfOperatorEnum.AddString;
                    break;
                case "ConditionalGoto":
                    op = TypeOfOperatorEnum.GoToIf;
                    break;
                case "PointerLineGoUp":
                    op = TypeOfOperatorEnum.IncrementPointer;
                    break;
                case "PointerLineGoDown":
                    op = TypeOfOperatorEnum.DecrementPointer;
                    break;
                case "GetThePointersVariable":
                    op = TypeOfOperatorEnum.GetPointerVariable;
                    break;
                case "MemoriseValueToLongTermMemory":
                    op = TypeOfOperatorEnum.CreateConst;
                    break;
                case "RetrieveFromLongTerm":
                    op = TypeOfOperatorEnum.GetConst;
                    break;
                case "ForgetValueFromLongTermMemory":
                    op = TypeOfOperatorEnum.DestroyConst;
                    break;
                case "BranchLikeATree":
                    op = TypeOfOperatorEnum.BranchGoto;
                    break;
                case "StrOp":
                    op = TypeOfOperatorEnum.StringOps;
                    break;
                case "Replace":
                    op = TypeOfOperatorEnum.ReplaceCharacter;
                    break;
                case "GetAtIndex":
                    op = TypeOfOperatorEnum.CharAtIndex;
                    break;
                case "ExecCommand":
                    op = TypeOfOperatorEnum.RunCommand;
                    break;
                case "GetThisPath":
                    op = TypeOfOperatorEnum.GetFilePath;
                    break;
                case "CommitLinePointer":
                    op = TypeOfOperatorEnum.CreateLinePointer;
                    break;
                case "DoLinePointer":
                    op = TypeOfOperatorEnum.ExecuteLinePointer;
                    break;
                case "ObliterateLinePointer":
                    op = TypeOfOperatorEnum.DestroyLinePointer;
                    break;
                case "RemoveCharacter":
                    op = TypeOfOperatorEnum.RemoveCharacter;
                    break;
                case "ContainsCharacter":
                    op = TypeOfOperatorEnum.ContainsCharacter;
                    break;
                case "NumToWord":
                    op = TypeOfOperatorEnum.SanitiseNumber;
                    break;
                case "IsANumber":
                    op = TypeOfOperatorEnum.IsNumber;
                    break;
                case "IsATextNumber":
                    op = TypeOfOperatorEnum.IsTextNumber;
                    break;
                default:
                    //if our operator does not exist then what happened
                    RESULT = Errors.ERROR_MESSAGES["error_badexp"].GetChoice();
                    return (errorhandled,"");
            }

            //check for unprotected unsafe code
            if ((op == TypeOfOperatorEnum.CreatePointer || op == TypeOfOperatorEnum.IncrementPointer || op == TypeOfOperatorEnum.DecrementPointer || op == TypeOfOperatorEnum.GetPointer || op == TypeOfOperatorEnum.DestroyPointer || op == TypeOfOperatorEnum.RunCommand || op == TypeOfOperatorEnum.CreateLinePointer || op == TypeOfOperatorEnum.ExecuteLinePointer || op == TypeOfOperatorEnum.DestroyLinePointer) && (origline[0] != '[' || origline[origline.Length - 1] != ']'))
            {
                RESULT = Errors.ERROR_MESSAGES["error_unsafe"].GetChoice(); return (errorhandled, "");
            }

            //get args (separated by commas
            string[] args = expressions[1].Split(",");
            if (args.Length == 0) { RESULT = Errors.ERROR_MESSAGES["error_noargs"].GetChoice(); return (errorhandled, ""); }

            //de-sanitise args
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].Contains("'")) { continue; }
                args[i] = args[i].Replace("'", "");
                args[i] = Utility.UnTranslateString(args[i]);
                args[i] = args[i].Replace("~'", "'");
            }

            //code for all of the expressions
            switch (op)
            {
                case TypeOfOperatorEnum.None:
                    RESULT = "";
                    return (true, "");
                case TypeOfOperatorEnum.AllocateMemory:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.Setvariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(args[1], EXECUTION_LINE);
                    }
                    else
                    {
                        if (MEMORYAMOUNT <= 0)
                        {
                            RESULT = Errors.ERROR_MESSAGES["error_nomemory"].GetChoice(); return (errorhandled, "");
                        }
                        MEMORYAMOUNT -= 1;
                        VARIABLES.Add(args[0], new VariableStruct(args[1], EXECUTION_LINE));
                    }
                    RESULT = "";
                    return (true, VARIABLES[args[0]].Get); //if we create or set a variable we just return its value there
                case TypeOfOperatorEnum.Getvariable:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, VARIABLES[args[0]].Get); //replace outselves with the variable value
                case TypeOfOperatorEnum.AddToVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, NumberToWords.ConvertToWords(WordsToNumber.ConvertToNumbers(args[0]) + WordsToNumber.ConvertToNumbers(args[1]))); //replace outselves with the variable value
                case TypeOfOperatorEnum.MinusFromVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, NumberToWords.ConvertToWords(WordsToNumber.ConvertToNumbers(args[0]) - WordsToNumber.ConvertToNumbers(args[1]))); //replace outselves with the variable value
                case TypeOfOperatorEnum.DivideVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    if(WordsToNumber.ConvertToNumbers(args[1]) == 0) { RESULT = Errors.ERROR_MESSAGES["error_divzero"].GetChoice(); return (errorhandled, ""); }
                    return (true, NumberToWords.ConvertToWords(WordsToNumber.ConvertToNumbers(args[0]) / WordsToNumber.ConvertToNumbers(args[1]))); //replace outselves with the variable value
                case TypeOfOperatorEnum.MultiplyVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, NumberToWords.ConvertToWords(WordsToNumber.ConvertToNumbers(args[0]) * WordsToNumber.ConvertToNumbers(args[1]))); //replace outselves with the variable value
                case TypeOfOperatorEnum.DestroyVariable:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.RememberVariable:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.CreatePointer:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    //if (!VARIABLES.ContainsKey(args[1]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return errorhandled;
                    //}
                    var targetline = (int)WordsToNumber.ConvertToNumbers(args[1]) - 1;
                    if (targetline < 0 || targetline >= lineslength || targetline >= EXECUTION_LINE)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return (errorhandled, "");
                    }
                    var destlineexp = lines[targetline].Split(":");
                    if (destlineexp[0] != "Commit") { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return (errorhandled, ""); }
                    var pointertargetvariable = destlineexp[1].Split(",")[0];
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        POINTERS.Add(args[0], new PointerStruct(pointertargetvariable, EXECUTION_LINE, targetline));
                    }
                    else
                    {
                        POINTERS[args[0]] = new PointerStruct(pointertargetvariable, EXECUTION_LINE, targetline);
                    }
                    RESULT = "";
                    return (true, VARIABLES[POINTERS[args[0]].Get].Get); //if we create or set a pointer we just return its value there
                case TypeOfOperatorEnum.GetPointer:
                    //THIS ONLY DOES SOEMTHING WHEN ITS A SUB EXPRESSION (in brackets)
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, POINTERS[args[0]].GetIsForgotten ? POINTERS[args[0]].GetStupidValue : VARIABLES[POINTERS[args[0]].Get].Get);
                case TypeOfOperatorEnum.DestroyPointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.IncrementPointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.DecrementPointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.GetPointerVariable:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, POINTERS[args[0]].GetIsForgotten ? POINTERS[args[0]].GetStupidValue : POINTERS[args[0]].Get);
                case TypeOfOperatorEnum.GoToLine:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.GoToIf:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.OutputAndEnd:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.Print:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.PrintNoNewline:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.Input:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    var read = Console.ReadLine();
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(read ?? "", EXECUTION_LINE);
                    }
                    RESULT = "";
                    return (true, read??""); //return the input
                case TypeOfOperatorEnum.AddString:
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    string newstr = args[0];
                    for (int i = 1; i < args.Length; i++)
                    {
                        newstr += args[i];
                    }
                    RESULT = "";
                    return (true, newstr); //return the input
                case TypeOfOperatorEnum.CreateConst:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (CONSTS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_alrconst"].GetChoice(); return (errorhandled, "");
                    }
                    if (MEMORYAMOUNT < 3)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_constmemory"].GetChoice(); return (errorhandled, "");
                    }
                    MEMORYAMOUNT -= 3;
                    CONSTS.Add(args[0], args[1]);
                    RESULT = "";
                    return (true, CONSTS[args[0]]);
                case TypeOfOperatorEnum.GetConst:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (!CONSTS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingconst"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, CONSTS[args[0]]);
                case TypeOfOperatorEnum.DestroyConst:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.BranchGoto:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.StringOps:
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    var strop = StringOpEnum.Invalid;
                    switch(args[1])
                    {
                        case "Lowercase":
                            strop = StringOpEnum.Lower;
                            break;
                        case "Uppercase":
                            strop = StringOpEnum.Upper;
                            break;
                        case "Reverse":
                            strop = StringOpEnum.Reverse;
                            break;
                        case "Length":
                            strop = StringOpEnum.Length;
                            break;
                        default:
                            RESULT = Errors.ERROR_MESSAGES["error_badstrop"].GetChoice(); return (errorhandled, "");
                    }
                    string result = "";
                    switch (strop)
                    {
                        case StringOpEnum.Length:
                            result = NumberToWords.ConvertToWords(args[0].Length);
                            break;
                        case StringOpEnum.Lower:
                            result = args[0].ToLower();
                            break;
                        case StringOpEnum.Upper:
                            result = args[0].ToUpper();
                            break;
                        case StringOpEnum.Reverse:
                            result = args[0].Flip();
                            break;
                    }
                    RESULT = "";
                    return (true, result);
                case TypeOfOperatorEnum.ReplaceCharacter:
                    if (args.Length < 3)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length > 3)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (!VARIABLES.ContainsKey(args[0]) && !CONSTS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return (errorhandled, "");
                    }
                    if (!CONSTS.ContainsKey(args[0]))
                    {
                        var initialvalue = VARIABLES[args[0]].Get;
                        VARIABLES[args[0]] = new VariableStruct(initialvalue.Replace(args[1], args[2]), EXECUTION_LINE);
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        var initialvalue = CONSTS[args[0]];
                        CONSTS[args[0]] = initialvalue.Replace(args[1], args[2]);
                    }
                    break;
                case TypeOfOperatorEnum.CharAtIndex:
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, new string(args[0][(int)WordsToNumber.ConvertToNumbers(args[1])], 1));
                case TypeOfOperatorEnum.RunCommand:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.GetFilePath:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(CodeFilePath, EXECUTION_LINE);
                    }
                    RESULT = "";
                    return (true, CodeFilePath);
                case TypeOfOperatorEnum.CreateLinePointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.ExecuteLinePointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.DestroyLinePointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (errorhandled, "");
                case TypeOfOperatorEnum.RemoveCharacter:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, args[0].Replace(args[1], ""));
                case TypeOfOperatorEnum.ContainsCharacter:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, args[0].Contains(args[1]) ? "true" : "false");
                case TypeOfOperatorEnum.SanitiseNumber:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    float.TryParse(args[0], out float res);
                    return (true, NumberToWords.ConvertToWords(res));
                case TypeOfOperatorEnum.IsNumber:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    return (true, float.TryParse(args[0], out float _) ? "true" : "false");
                case TypeOfOperatorEnum.IsTextNumber:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (errorhandled, "");
                    }
                    RESULT = "";
                    if (args[0] == "zero")
                    {
                        return (true, "true");
                    }
                    else
                    {
                        return (true, WordsToNumber.ConvertToNumbers(args[0]) == 0 ? "false" : "true");
                    }
            }
            RESULT = "";
            return (errorhandled,"");
        }
    }
}

