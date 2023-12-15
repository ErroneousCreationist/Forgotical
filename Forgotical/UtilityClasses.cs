using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

namespace Forgotical.InternalUtility
{
    class NumberToWords
    {
        public static string ConvertToWords(float v)
        {
            return DecimalToWords((decimal)v);
        }
        public static string ConvertToWords(int v)
        {
            return IntegerToWords(v);
        }

        private static string DecimalToWords(decimal number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + DecimalToWords(Math.Abs(number));

            string words = "";

            int intPortion = (int)number;
            decimal fraction = (number - intPortion) * 100;
            int decPortion = (int)fraction;

            words = IntegerToWords(intPortion);
            if (decPortion > 0)
            {
                words += " point ";
                words += IntegerToWords(decPortion);
            }
            return words;
        }

        private static string IntegerToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + IntegerToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += IntegerToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += IntegerToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += IntegerToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words;
        }
    }

    class WordsToNumber
    {
        private static Dictionary<string, long> numberTable = new Dictionary<string, long>{
        {"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},{"five",5},{"six",6},
        {"seven",7},{"eight",8},{"nine",9},{"ten",10},{"eleven",11},{"twelve",12},
        {"thirteen",13},{"fourteen",14},{"fifteen",15},{"sixteen",16},{"seventeen",17},
        {"eighteen",18},{"nineteen",19},{"twenty",20},{"thirty",30},{"forty",40},
        {"fifty",50},{"sixty",60},{"seventy",70},{"eighty",80},{"ninety",90},
        {"hundred",100},{"thousand",1000},{"lakh",100000},{"million",1000000},
        {"billion",1000000000},{"trillion",1000000000000},{"quadrillion",1000000000000000},
        {"quintillion",1000000000000000000}
        };

        public static float ConvertToNumbers(string numberString)
        {
            //decimal support
            if(numberString.Contains("point", StringComparison.InvariantCultureIgnoreCase))
            {
                //invalid number
                if(numberString.IndexOf("point", StringComparison.InvariantCultureIgnoreCase) != numberString.LastIndexOf("point", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 0;
                }

                var split = numberString.Split("point");
                var whole = ConvertToNumbers(split[0]);
                var dec = ConvertToNumbers(split[1]);
                while(dec>0)
                {
                    dec *= 0.1f;
                }
                return whole+dec;
            }

            var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                    .Select(m => m.Value.ToLowerInvariant())
                    .Where(v => numberTable.ContainsKey(v))
                    .Select(v => numberTable[v]);
            long acc = 0, total = 0L;
            foreach (var n in numbers)
            {
                if (n >= 1000)
                {
                    total += acc * n;
                    acc = 0;
                }
                else if (n >= 100)
                {
                    acc *= n;
                }
                else acc += n;
            }
            return (total + acc) * (numberString.StartsWith("minus",
                    StringComparison.InvariantCultureIgnoreCase) ? -1 : 1);
        }
    }

    public static class Utility
    {
        private static string[] Chars = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "!", "@", "#", "$", "%", "^", "&", "*" };
        public static string RandomCharacter
        {
            get
            {
                Random rand = new Random();
                return Chars[rand.Next(0, Chars.Length)];
            }
        }

        public static void ExecuteCommand(string command)
        {
            //unix
            if(Environment.OSVersion.Platform == PlatformID.Unix)
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
            //windows
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/C " + command
                    }
                };
                process.Start();
            }
        }

        public static List<string> GetStringsInParentheses(string input)
        {
            List<string> strings = new List<string>();
            int depth = 0;
            int start = -1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    if (depth == 0)
                    {
                        start = i + 1;
                    }
                    depth++;
                }
                else if (input[i] == ')')
                {
                    depth--;
                    if (depth == 0 && start != -1)
                    {
                        string sub = input.Substring(start, i - start);
                        strings.AddRange(GetStringsInParentheses(sub));
                        strings.Add(sub);
                        start = -1;
                    }
                }
            }
            return strings.OrderBy(s => s.Count(c => c == '(')).ToList();
        }

        public static List<string> GetStringsInQuotes(string input)
        {
            List<string> strings = new List<string>();
            int depth = 0;
            int start = -1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '"')
                {
                    if (depth == 0)
                    {
                        start = i + 1;
                    }
                    depth++;
                }
                else if (input[i] == '"')
                {
                    depth--;
                    if (depth == 0 && start != -1)
                    {
                        string sub = input.Substring(start, i - start);
                        strings.AddRange(GetStringsInQuotes(sub));
                        strings.Add(sub);
                        start = -1;
                    }
                }
            }
            return strings.OrderBy(s => s.Count(c => c == '(')).ToList();
        }

        static List<string> GetQuotedSubstrings(string input)
        {
            // Regular expression to match substrings inside quotation marks
            Regex regex = new Regex("(?<!~)\"([^\"]*?)\"");

            // Match all substrings enclosed in quotation marks (except those preceded by ~)
            MatchCollection matches = regex.Matches(input);

            // Extract substrings from matches without the quotation marks
            List<string> substrings = new List<string>();
            foreach (Match match in matches)
            {
                substrings.Add(match.Groups[1].Value);
            }

            return substrings;
        }

        private static Dictionary<string, string> TRANSLATIONS = new Dictionary<string, string>()
        {
            {" ","<<<<SPACE>>>>"},
            {"[","<<<<SQUAREBRACKETLEFT>>>>"},
            {"]","<<<<SQUAREBRACKETRIGHT>>>>"},
            {",","<<<<COMMA>>>>"},
            {":","<<<<COLON>>>>"},
            {"1","<<<<ONE>>>>"},
            {"2","<<<<TWO>>>>"},
            {"3","<<<<THREE>>>>"},
            {"4","<<<<FOUR>>>>"},
            {"5","<<<<FIVE>>>>"},
            {"6","<<<<SIX>>>>"},
            {"7","<<<<SEVEN>>>>"},
            {"8","<<<<EIGHT>>>>"},
            {"9","<<<<NINE>>>>"},
            {"0","<<<<TEN>>>>"},
            {"(","<<<<BRACKETLEFT>>>>"},
            {")","<<<<BRACKETRIGHT>>>>"},
            {"~'","<<<<SINGLEQUOTATIONMARK>>>>"},
            {"\"","<<<<QUOTATIONMARK>>>>"},
        };
        public static string Flip(this string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static string TranslateString(string input)
        {
            string returned = input; //make sure it is fully untranslated (in case someone writes a sn
            foreach (var item in TRANSLATIONS)
            {
                returned = returned.Replace(item.Key, item.Value);
            }
            return returned;
        }
        public static string UnTranslateString(string input)
        {
            string returned = input;
            foreach (var item in TRANSLATIONS)
            {
                returned = returned.Replace(item.Value, item.Key);
            }
            return returned;
        }

        public static string TranslateQuotedStrings(string input)
        {
            // Regular expression to match substrings inside single quotation marks (ignoring ~)
            Regex regex = new Regex("(?<!~)'[^']*?'");

            // Matches all substrings enclosed in single quotation marks (except those preceded by ~)
            string result = regex.Replace(input, match =>
            {
                string quotedString = match.Value;
                // Iterate through the translations and replace matching keys with values
                foreach (var item in TRANSLATIONS)
                {
                    quotedString = quotedString.Replace(item.Key, item.Value);
                }
                return quotedString;
            });

            return result;
        }
    }
}

