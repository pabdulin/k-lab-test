using System;

namespace Palindrome
{
    public class PalindromeFinder
    {
        public string SearchPalindrome(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text == string.Empty)
            {
                throw new ArgumentOutOfRangeException("text");
            }

            // performance monitor
            int testCount = 0;

            // leftmost character is always good result
            int bestMatchPos = 0;
            int bestMatchLength = 1;

            // walking left to right
            for (int leftPos = 0; leftPos < text.Length; leftPos += 1)
            {
                // while it's possible to get a better match
                for (int rightPos = text.Length; leftPos < rightPos - bestMatchLength; rightPos -= 1)
                {
                    // performance hint, do not check if border symbols don't match
                    if (text[leftPos] != text[rightPos - 1])
                    {
                        continue;
                    }

                    var length = rightPos - leftPos;
                    testCount += 1;
                    if (IsPalindrome(text, leftPos, length))
                    {
                        bestMatchPos = leftPos;
                        bestMatchLength = length;
                    }
                }
            }

            var result = text.Substring(bestMatchPos, bestMatchLength);
            return result;
        }

        public static bool IsPalindrome(string text, int startPos, int length)
        {
            var start = startPos;
            var middle = startPos + (length / 2);
            var end = startPos + length - 1;

            var result = true;

            for (int i = startPos; i < middle; i += 1)
            {
                var offset = i - startPos;
                if (text[start + offset] != text[end - offset])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}
