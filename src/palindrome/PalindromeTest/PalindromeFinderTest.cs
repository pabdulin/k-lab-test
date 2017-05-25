using Microsoft.VisualStudio.TestTools.UnitTesting;
using Palindrome;

namespace PalindromeTest
{
    [TestClass]
    public class PalindromeFinderTest
    {
        private PalindromeFinder _target = new PalindromeFinder();

        private string[] _testPalindromes = new string[]
        {
            "c", "cbabc", "aczzca", "arozaupalanalapuazora", "arozaupalanalapuazoraarozaupalanalapuazora", "aaaaaaaaaaaaaaaaaaaaaaaaaaa"
        };

        private string[] _testNotPalindromes = new string[]
        {
           "ab", "abc", "arozaupaalanalapuazora"
        };

        [TestMethod]
        public void ShouldPassSampleTest()
        {
            var input = "sometextarozaupalanalapuazorasomeanothertext";
            var expected = "arozaupalanalapuazora";
            var result = _target.SearchPalindrome(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldPassLargeTest()
        {
            var input = "sometextarozaupalanalapuazorasomeanotherzzzzzzzzzzzzzzzzzzzzzzzzzzzzarozaupalanalapuazoraarozaupalanalapuazoratextsometextarozaupalanalapuazorasomeanotherzzzzzzzzzzzzzzzzzzzzzzzzzzzzarozaupalanalapuazfsdoraarozaupalanalapuazoratext";
            var expected = "arozaupalanalapuazoraarozaupalanalapuazora";
            var result = _target.SearchPalindrome(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldPassBarrierTest()
        {
            {
                var input = "aaaaaaaaaaaaaaaaaaaaaaaaabaaaa";
                var expected = "aaaaaaaaaaaaaaaaaaaaaaaaa";
                var result = _target.SearchPalindrome(input);
                Assert.AreEqual(expected, result);
            }

            {
                var input = "aabaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                var expected = "aaaaaaaaaaaaaaaaaaaaaaaaaaa";
                var result = _target.SearchPalindrome(input);
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void ShouldReturnLeftmostIfSameLength()
        {
            { 
                var input = "zraabbds";
                var expected = "aa";
                var result = _target.SearchPalindrome(input);
                Assert.AreEqual(expected, result);
            }

            {
                var input = "cba";
                var expected = "c";
                var result = _target.SearchPalindrome(input);
                Assert.AreEqual(expected, result);
            }

            {
                var input = "aaabbbccc";
                var expected = "aaa";
                var result = _target.SearchPalindrome(input);
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void ShouldReturnMaxLength()
        {
            var input = "zrcaacbabcds";
            var expected = "cbabc";
            var result = _target.SearchPalindrome(input);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ShouldWorkOnPolindromeOnly()
        {
            foreach (var input in _testPalindromes)
            {
                var expected = input;
                var result = _target.SearchPalindrome(input);
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void ShouldDetectPalindrome()
        {
            var expected = true;
            foreach (var input in _testPalindromes)
            {
                var result = PalindromeFinder.IsPalindrome(input, 0, input.Length);
                Assert.AreEqual(expected, result);
            }
        }

        [TestMethod]
        public void ShouldDetectNotPalindrome()
        {
            var expected = false;
            foreach (var input in _testNotPalindromes)
            {
                var result = PalindromeFinder.IsPalindrome(input, 0, input.Length);
                Assert.AreEqual(expected, result);
            }
        }
    }
}
