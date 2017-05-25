using Palindrome;
using System;
using System.Diagnostics;
using System.IO;

namespace PalindromeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = "input.txt";
            if(args.Length > 0)
            {
                inputFile = args[0];
            }

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Usage: PalindromeApp [InputFile]");
                Console.WriteLine("InputFile: input data file. Defaults to 'input.txt' if not specified.");
                Console.WriteLine("---");
                Console.WriteLine($"Error: file '{inputFile}' not found.");
            }
            else
            {
                string testStr;
                using(var file = File.OpenText(inputFile))
                {
                    testStr = file.ReadLine();
                    Console.WriteLine($"Input text: '{testStr}'");
                }
                var finder = new PalindromeFinder();
                try
                { 
                    var result = finder.SearchPalindrome(testStr);
                    Console.WriteLine($"Longest palindrome: '{result}'");
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit..");
                Console.ReadKey();
            }
        }
    }
}
