using System;
using System.Collections.Generic;

namespace Palindrome
{
    internal struct finder_match
    {
        public finder_match(int position, int length)
        {
            Position = position;
            Length = length;
        }

        public int Position;
        public int Length;
    }

    public class PalindromeFinder
    {
        public string SearchPalindrome(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException();
            }

            if (text == string.Empty)
            {
                throw new ArgumentOutOfRangeException();
            }

            // leftmost character is always good result
            finder_match bestMatch = new finder_match(0, 1);

            for (int startIndex = 0; startIndex < text.Length; startIndex += 1)
            {
                int leftPos = startIndex;
                int rightPos = text.Length;
                while (leftPos < rightPos - 1) // -1 for ignoring substrings lenght of 1
                {
                    var test = text.Substring(leftPos, rightPos - leftPos);
                    if (IsPalindrome(test))
                    {
                        if (bestMatch.Length < test.Length)
                        {
                            bestMatch = new finder_match(leftPos, test.Length);
                        }
                    }
                    rightPos -= 1;
                }
            }

            string result = text.Substring(0, 1); 
            if(bestMatch.Length > 0)
            {
                result = text.Substring(bestMatch.Position, bestMatch.Length);
            }

            return result;
        }

        public static bool IsPalindrome(string text)
        {
            var result = true;

            for (int i = 0; i < text.Length / 2; i += 1)
            {
                if (text[i] != text[text.Length - 1 - i])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}
