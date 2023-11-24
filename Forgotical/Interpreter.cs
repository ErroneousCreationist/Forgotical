using System;
using Forgotical.InternalUtility;

namespace Forgotical
{
	public class Interpreter
	{
        public class VariableStruct
        {
            private string Value;
            private int CLARITYLINESLEFT, COMPLETEDECAYLINESLEFT, LinesExistedFor;

            public VariableStruct(string value, int line)
            {
                Random random = new Random(line);
                Value = value;
                LinesExistedFor = 0;
                CLARITYLINESLEFT = random.Next(7,10);
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
                LinesExistedFor = 0;
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
                LinesExistedFor = 0;
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

        private enum TypeOfOperatorEnum { None, AllocateMemory, Setvariable, Getvariable, AddToVariable, MinusFromVariable, DivideVariable, MultiplyVariable, DestroyVariable, RememberVariable, CreatePointer, GetPointer, DestroyPointer, GoToLine, OutputAndEnd, Print, PrintNoNewline, Input, AddString, GoToIf, IncrementPointer, DecrementPointer, GetPointerVariable }
        private enum ComparisionEnum { Invalid, Equal, Over, Under, OverEqual, UnderEqual, NotEqual }
		public Dictionary<string, VariableStruct> VARIABLES = new Dictionary<string, VariableStruct>();
        public Dictionary<string, PointerStruct> POINTERS = new Dictionary<string, PointerStruct>();
        public int MEMORYAMOUNT = 0;
        public int EXECUTION_LINE = 1;
        private bool ProgramEnded;
        private string Output = "";
        public int lineslength;
        public string[] lines;
        private bool STEP_MODE, SHELL_MODE;
        //private bool AlreadySucceeded;

		public Interpreter(bool step_mode = false)
		{
			VARIABLES = new Dictionary<string, VariableStruct>();
            MEMORYAMOUNT = 0;
            Output = "";
            ProgramEnded = false;
            EXECUTION_LINE = 0;
            lines = new string[0];
            STEP_MODE = step_mode;
		}

        /// <summary>
        /// This one is for shell mode, since the line, variables and pointers pass over.
        /// </summary>
        /// <param name="vars">the previously existing variables</param>
        /// <param name="pointers">the previously existing pointers</param>
        /// <param name="execline"></param>
        public Interpreter(Dictionary<string, VariableStruct> vars, Dictionary<string, PointerStruct> pointers, string[] prevlines, int execline, int mem)
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
        }

        public bool ShellExecuteLine(string code, out string Result)
        {
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

        public bool ExecuteCode(string code, out string Result)
        {
            //whitespace error because we hate whitespace here
            code = Utility.TranslateQuotedStrings(code); //sanitise all of the strings in quotation marks to make them safe and not throw errors
            var resultant = code.Split("\n");
            lineslength = resultant.Length;
            if (code.Contains(' ')) { Output = ""; Result = Errors.ERROR_MESSAGES["error_space"].GetChoice(); return false; }
            if(code.IndexOfAny("1234567890".ToCharArray()) != -1) { Output = ""; ProgramEnded = true; Result = Errors.ERROR_MESSAGES["error_numbers"].GetChoice(); return false; }

            //recreate to sanitise - to space so we can actually write stuff normally
            code = code.Replace("-", " ");
            resultant = code.Split("\n");
            lineslength = resultant.Length;
            lines = new string[lineslength];
            resultant.CopyTo(lines, 0);

            while (!ProgramEnded)
            {
                if (STEP_MODE)
                {
                    Console.WriteLine($"Current line: {EXECUTION_LINE + 1}. Line content: {resultant[EXECUTION_LINE]}");
                    foreach (var item in VARIABLES)
                    {
                        Console.WriteLine($"variable {item.Key} has a value of {item.Value.Get}");
                    }
                    Console.ReadKey();
                }
                if (EXECUTION_LINE == lineslength && !ProgramEnded) { Output = ""; ProgramEnded = true; break; }
                if (!ExecuteLine(resultant[EXECUTION_LINE], out string tempresult))
                {
                    Output = tempresult;
                    ProgramEnded = true;
                    Result = tempresult;
                    return false;
                }
                Random rand = new Random();
                var memoryupdateprobability = 1 - MathF.Pow(MathF.E, -(0.01f * MEMORYAMOUNT));
                if(rand.NextDouble() < 1-memoryupdateprobability)
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
                EXECUTION_LINE++;
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

            //sanitise but keep original so we can verify that we are protecting our pointers
            string origline = line;
            line = line.Replace("[", "");
            line = line.Replace("]", "");

            //split it
            var expressions = line.Split(":");

            //no expressions is wrong, exit and return error
            if (expressions.Length == 0) { RESULT = Errors.ERROR_MESSAGES["error_noexp"].GetChoice(); return false; }
            if (expressions.Length < 2) { RESULT = Errors.ERROR_MESSAGES["error_noargs"].GetChoice(); return false; }
            if (expressions.Length > 2) { RESULT = Errors.ERROR_MESSAGES["error_badexpstatement"].GetChoice(); return false; }

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
                default:
                    //if our operator does not exist then what happened
                    RESULT = Errors.ERROR_MESSAGES["error_badexp"].GetChoice();
                    return false;
            }

            //check for unprotected unsafe code
            if((op==TypeOfOperatorEnum.CreatePointer|| op == TypeOfOperatorEnum.IncrementPointer || op == TypeOfOperatorEnum.DecrementPointer || op ==TypeOfOperatorEnum.GetPointer||op==TypeOfOperatorEnum.DestroyPointer) && (origline[0] != '[' || origline[origline.Length-1] != ']'))
            {
                RESULT = Errors.ERROR_MESSAGES["error_unsafe"].GetChoice(); return false;
            }

            //get args (separated by commas
            string[] args = expressions[1].Split(",");
            if(args.Length == 0) { RESULT = Errors.ERROR_MESSAGES["error_noargs"].GetChoice(); return false; }

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
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (WordsToNumber.ConvertToNumbers(args[0])<=0)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return false;
                    }
                    Random rand = new Random(EXECUTION_LINE);
                    if(rand.NextDouble()>0.5f && WordsToNumber.ConvertToNumbers(args[0]) > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomuchmemory"].GetChoice(); return false;
                    }
                    MEMORYAMOUNT += (int)WordsToNumber.ConvertToNumbers(args[0]);
                    break;
                case TypeOfOperatorEnum.Setvariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (MEMORYAMOUNT<=0)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_nomemory"].GetChoice(); return false;
                    }
                    MEMORYAMOUNT -= 1;
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(args[1], EXECUTION_LINE);
                    }
                    else
                    {
                        VARIABLES.Add(args[0], new VariableStruct(args[1], EXECUTION_LINE));
                    }
                    break;
                case TypeOfOperatorEnum.Getvariable:
                    //IMPLEMENT THIS ELSEWHERE, IT DOESN'T REALLY DO ANYTHING HERE (if its in brackets it replaces itself with the appropriate value)
                    break;
                case TypeOfOperatorEnum.AddToVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return false;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    }
                    var num1 = WordsToNumber.ConvertToNumbers(VARIABLES[args[0]].Get);
                    var num2 = WordsToNumber.ConvertToNumbers(args[1]);
                    VARIABLES[args[0]] = new VariableStruct(NumberToWords.ConvertToWords(num1 + num2), EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.MinusFromVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return false;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    }
                    var subnum1 = WordsToNumber.ConvertToNumbers(VARIABLES[args[0]].Get);
                    var subnum2 = WordsToNumber.ConvertToNumbers(args[1]);

                    VARIABLES[args[0]] = new VariableStruct(NumberToWords.ConvertToWords(subnum1 - subnum2), EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.DivideVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return false;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    }
                    var divnum1 = WordsToNumber.ConvertToNumbers(VARIABLES[args[0]].Get);
                    var divnum2 = WordsToNumber.ConvertToNumbers(args[1]);
                    if(divnum2 == 0) { RESULT = Errors.ERROR_MESSAGES["error_divzero"].GetChoice(); return false; }
                    VARIABLES[args[0]] = new VariableStruct(NumberToWords.ConvertToWords(divnum1 / divnum2), EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.MultiplyVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return false;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    }
                    var mulnum1 = WordsToNumber.ConvertToNumbers(VARIABLES[args[0]].Get);
                    var mulnum2 = WordsToNumber.ConvertToNumbers(args[1]);

                    VARIABLES[args[0]] = new VariableStruct(NumberToWords.ConvertToWords(mulnum1 * mulnum2), EXECUTION_LINE);
                    break;
                case TypeOfOperatorEnum.DestroyVariable:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    }
                    MEMORYAMOUNT += 1;
                    VARIABLES.Remove(args[0]);
                    break;
                case TypeOfOperatorEnum.RememberVariable:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (!VARIABLES.ContainsKey(args[0]) && !POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_cantrefresh"].GetChoice(); return false;
                    }
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]].RefreshMemory();
                    }
                    else if (POINTERS.ContainsKey(args[0]))
                    {
                        POINTERS[args[0]].RefreshMemory();
                    }
                    break;
                case TypeOfOperatorEnum.CreatePointer:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    //if (!VARIABLES.ContainsKey(args[1]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    //}
                    var targetline = (int)WordsToNumber.ConvertToNumbers(args[1])-1;
                    if(targetline < 0 || targetline >= lineslength || targetline >= EXECUTION_LINE)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false;
                    }
                    var destlineexp = lines[targetline].Split(":");
                    if (destlineexp[0] != "Commit") { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false; }
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
                    //    RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    //}
                    //if (!POINTERS.ContainsKey(args[0]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return false;
                    //}
                    //VARIABLES[POINTERS[args[0]]];
                    break;
                case TypeOfOperatorEnum.GetPointerVariable:
                    //DO THIS IN SUB EXPRESSIONS
                    break;
                case TypeOfOperatorEnum.DestroyPointer:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return false;
                    }
                    POINTERS.Remove(args[0]);
                    break;
                case TypeOfOperatorEnum.IncrementPointer:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return false;
                    }
                    //if (!VARIABLES.ContainsKey(args[1]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    //}
                    var inctargetline = POINTERS[args[0]].GetTargetLine - (int)WordsToNumber.ConvertToNumbers(args[1]);
                    if (inctargetline < 0 || inctargetline >= lineslength)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false;
                    }
                    var incdestlineexp = lines[inctargetline].Split(":");
                    if (incdestlineexp[0] != "Commit") { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false; }
                    if(incdestlineexp.Length > 2) { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false; } //a line like this clearly hasn't been interpreted, meaning it has brackets, so we cannot point to it
                    var incpointertargetvariable = incdestlineexp[1].Split(",")[0];
                    if (!VARIABLES.ContainsKey(incpointertargetvariable)) { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false; } //if the variable described hasnt been created yet, we are looking into the future into what hasn't been interpreted yet, so we cannot point to this
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
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return false;
                    }
                    //if (!VARIABLES.ContainsKey(args[1]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    //}
                    var dectargetline = POINTERS[args[0]].GetTargetLine + (int)WordsToNumber.ConvertToNumbers(args[1]);
                    if (dectargetline < 0 || dectargetline >= lineslength)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false;
                    }
                    var decdestlineexp = lines[dectargetline].Split(":");
                    if (decdestlineexp[0] != "Commit") { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false; }
                    if (decdestlineexp.Length > 2) { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false; } //a line like this clearly hasn't been interpreted, meaning it has brackets, so we cannot point to it
                    var decpointertargetvariable = decdestlineexp[1].Split(",")[0];
                    if (!VARIABLES.ContainsKey(decpointertargetvariable)) { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return false; } //if the variable described hasnt been created yet, we are looking into the future into what hasn't been interpreted yet, so we cannot point to this
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
                        RESULT = Errors.ERROR_MESSAGES["error_shellgoto"].GetChoice(); return false;
                    }
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    var targetlinegoto = (int)WordsToNumber.ConvertToNumbers(args[1]) - 2;
                    if (targetlinegoto >= lineslength || targetlinegoto < 0)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return false;
                    }
                    EXECUTION_LINE = targetlinegoto;
                    RESULT = "";
                    return true;
                case TypeOfOperatorEnum.GoToIf:
                    if(SHELL_MODE)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_shellgoto"].GetChoice(); return false;
                    }
                    if (args.Length > 5)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if(args.Length<5)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return false;
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
                            return false;
                    }
                    dynamic value1 = type == ComparisionEnum.Equal || type == ComparisionEnum.NotEqual ? args[0] : WordsToNumber.ConvertToNumbers(args[0]);
                    dynamic value2 = type == ComparisionEnum.Equal || type == ComparisionEnum.NotEqual ? args[1] : WordsToNumber.ConvertToNumbers(args[1]);

                    //Console.WriteLine(value1 + " compared to " + value2); ;
                    var result = false;
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
                    if (targetlinegotoif >= lineslength || targetlinegotoif < 0)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidgoto"].GetChoice(); return false;
                    }
                    EXECUTION_LINE = targetlinegotoif;
                    RESULT = "";
                    return true;
                case TypeOfOperatorEnum.OutputAndEnd:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    //if (!VARIABLES.ContainsKey(args[0]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    //}
                    Output = args[0];
                    RESULT = args[0];
                    ProgramEnded = true;
                    return true;
                case TypeOfOperatorEnum.Print:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    Console.WriteLine(args[0]);
                    break;
                case TypeOfOperatorEnum.PrintNoNewline:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    Console.Write(args[0]);
                    break;
                case TypeOfOperatorEnum.Input:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    var read = Console.ReadLine();
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(read ?? "", EXECUTION_LINE);
                    }
                    break;
                case TypeOfOperatorEnum.AddString:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return false;
                    }
                    if (args.Length < 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_badargs"].GetChoice(); return false;
                    }
                    if (!VARIABLES.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    }
                    var val1 = VARIABLES[args[0]].Get;
                    var val2 = args[1];

                    VARIABLES[args[0]] = new VariableStruct(val1+val2, EXECUTION_LINE);
                    break;
            }
            RESULT = "";
            return true;
        }

        private (bool,string) ExecuteSubLine(string line, out string RESULT)
        {
            if (line[0] == '\\') { RESULT = ""; return (true,""); } //comments
            if (line == "") { RESULT = ""; return (true, ""); }
            TypeOfOperatorEnum op = TypeOfOperatorEnum.None;

            //sanitise the angle brackets but keep it around to check if we are protecting our pointers
            string origline = line;
            line = line.Replace("[", "");
            line = line.Replace("]", "");

            var expressions = line.Split(":");

            //no expressions is wrong, exit and return error
            if (expressions.Length == 0) { RESULT = Errors.ERROR_MESSAGES["error_noexp"].GetChoice(); return (false, ""); ; }
            if (expressions.Length > 2) { RESULT = Errors.ERROR_MESSAGES["error_badexpstatement"].GetChoice(); return (false, ""); ; }
            if (expressions.Length < 2) { RESULT = Errors.ERROR_MESSAGES["error_noargs"].GetChoice(); return (false, ""); ; }

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
                default:
                    //if our operator does not exist then what happened
                    RESULT = Errors.ERROR_MESSAGES["error_badexp"].GetChoice();
                    return (false,"");
            }

            //check for unprotected unsafe code
            if ((op == TypeOfOperatorEnum.CreatePointer || op == TypeOfOperatorEnum.GetPointer || op == TypeOfOperatorEnum.DestroyPointer) && (origline[0] != '[' || origline[origline.Length - 1] != ']'))
            {
                RESULT = Errors.ERROR_MESSAGES["error_unsafe"].GetChoice(); return (false, "") ;
            }

            //get args (separated by commas
            string[] args = expressions[1].Split(",");
            if (args.Length == 0) { RESULT = Errors.ERROR_MESSAGES["error_noargs"].GetChoice(); return (false, ""); }

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
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.Setvariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    if (MEMORYAMOUNT <= 0)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_nomemory"].GetChoice(); return (false, "");
                    }
                    MEMORYAMOUNT -= 1;
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(args[1], EXECUTION_LINE);
                    }
                    else
                    {
                        VARIABLES.Add(args[0], new VariableStruct(args[1], EXECUTION_LINE));
                    }
                    RESULT = "";
                    return (true, VARIABLES[args[0]].Get); //if we create or set a variable we just return its value there
                case TypeOfOperatorEnum.Getvariable:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    RESULT = "";
                    return (true, VARIABLES[args[0]].Get); //replace outselves with the variable value
                case TypeOfOperatorEnum.AddToVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    RESULT = "";
                    return (true, NumberToWords.ConvertToWords(WordsToNumber.ConvertToNumbers(args[0]) + WordsToNumber.ConvertToNumbers(args[1]))); //replace outselves with the variable value
                case TypeOfOperatorEnum.MinusFromVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    RESULT = "";
                    return (true, NumberToWords.ConvertToWords(WordsToNumber.ConvertToNumbers(args[0]) - WordsToNumber.ConvertToNumbers(args[1]))); //replace outselves with the variable value
                case TypeOfOperatorEnum.DivideVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    RESULT = "";
                    if(WordsToNumber.ConvertToNumbers(args[1]) == 0) { RESULT = Errors.ERROR_MESSAGES["error_divzero"].GetChoice(); return (false, ""); }
                    return (true, NumberToWords.ConvertToWords(WordsToNumber.ConvertToNumbers(args[0]) / WordsToNumber.ConvertToNumbers(args[1]))); //replace outselves with the variable value
                case TypeOfOperatorEnum.MultiplyVariable:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    RESULT = "";
                    return (true, NumberToWords.ConvertToWords(WordsToNumber.ConvertToNumbers(args[0]) * WordsToNumber.ConvertToNumbers(args[1]))); //replace outselves with the variable value
                case TypeOfOperatorEnum.DestroyVariable:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.RememberVariable:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.CreatePointer:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    //if (!VARIABLES.ContainsKey(args[1]))
                    //{
                    //    RESULT = Errors.ERROR_MESSAGES["error_varmissing"].GetChoice(); return false;
                    //}
                    var targetline = (int)WordsToNumber.ConvertToNumbers(args[1]) - 1;
                    if (targetline < 0 || targetline >= lineslength || targetline >= EXECUTION_LINE)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return (false, "");
                    }
                    var destlineexp = lines[targetline].Split(":");
                    if (destlineexp[0] != "Commit") { RESULT = Errors.ERROR_MESSAGES["error_invalidpointer"].GetChoice(); return (false, ""); }
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
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return (false, "");
                    }
                    RESULT = "";
                    return (true, POINTERS[args[0]].GetIsForgotten ? POINTERS[args[0]].GetStupidValue : VARIABLES[POINTERS[args[0]].Get].Get);
                case TypeOfOperatorEnum.DestroyPointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.IncrementPointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.DecrementPointer:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.GetPointerVariable:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    if (!POINTERS.ContainsKey(args[0]))
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_missingpointer"].GetChoice(); return (false, "");
                    }
                    RESULT = "";
                    return (true, POINTERS[args[0]].GetIsForgotten ? POINTERS[args[0]].GetStupidValue : POINTERS[args[0]].Get);
                case TypeOfOperatorEnum.GoToLine:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.GoToIf:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.OutputAndEnd:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.Print:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.PrintNoNewline:
                    RESULT = Errors.ERROR_MESSAGES["error_illegalexp"].GetChoice(); return (false, "");
                case TypeOfOperatorEnum.Input:
                    if (args.Length > 1)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    var read = Console.ReadLine();
                    if (VARIABLES.ContainsKey(args[0]))
                    {
                        VARIABLES[args[0]] = new VariableStruct(read ?? "", EXECUTION_LINE);
                    }
                    RESULT = "";
                    return (true, read??""); //return the input
                case TypeOfOperatorEnum.AddString:
                    if (args.Length > 2)
                    {
                        RESULT = Errors.ERROR_MESSAGES["error_toomanyargs"].GetChoice(); return (false, "");
                    }
                    RESULT = "";
                    return (true, args[0] + args[1]); //return the input
            }
            RESULT = "";
            return (false,"");
        }
    }
}

