using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Test.Merchandising
{
    using Blue.Cosacs.Merchandising;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Merchandising.Solr;
    using Blue.Events;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
        [TestCase("helloworld", "Helloworld", Description = "ToTitleCaseAllLowerCase")]
        [TestCase("hello world", "Hello World", Description = "ToTitleCaseAllLowerCaseWithSpace")]
        [TestCase("hello111 world", "Hello111 World", Description = "ToTitleCaseAllLowerCaseWithNumbers")]
        [TestCase("111hello world", "111Hello World", Description = "ToTitleCaseAllLowerCaseWithNumbersAtStart")]
        [TestCase("hello world111", "Hello World111", Description = "ToTitleCaseAllLowerCaseWithNumbersAtEnd")]
        [TestCase("hello$^%&world", "Hello$^%&World", Description = "ToTitleCaseAllLowerCaseWithSymbols")]
        [TestCase("hello$^%& world", "Hello$^%& World", Description = "ToTitleCaseAllLowerCaseWithSymbolsAndSpaces")]
        [TestCase("HELLOWORLD", "HELLOWORLD", Description = "ToTitleCaseAllUpperCase")]
        [TestCase("HELLO WORLD", "HELLO WORLD", Description = "ToTitleCaseAllUpperCaseWithSpace")]
        [TestCase("HELLO#$%^&*(WORLD", "HELLO#$%^&*(WORLD", Description = "ToTitleCaseAllUpperCaseWithSymbols")]
        [TestCase("HELLO#$% ^&*(WORLD", "HELLO#$% ^&*(WORLD", Description = "ToTitleCaseAllUpperCaseWithSymbolsAndSpaces")]
        [TestCase("124116", "124116", Description = "ToTitleCaseAllNumbers")]
        [TestCase("124 116", "124 116", Description = "ToTitleCaseAllNumbersWithSpaces")]
        [TestCase("The quick brown fox jumped over the lazy dog", "The Quick Brown Fox Jumped Over The Lazy Dog", Description = "ToTitleCaseMultipleWords")]
        [TestCase("", "", Description = "ToTitleCaseEmpty")]
        [TestCase(" ", " ", Description = "ToTitleCaseSpace")]
        [TestCase("  ", "  ", Description = "ToTitleCaseSpaces")]
        public void ToTitleCase(string original, string expected)
        {
            Assert.AreEqual(expected, original.ToTitleCase());
        }
    }
}
