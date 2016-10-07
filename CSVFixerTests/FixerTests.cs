using CSVFixer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVFixerTests
{
    [TestClass]
    public class FixerTests
    {
        private Fixer fixer;

        [TestInitialize]
        public void Setup()
        {
            this.fixer = new Fixer();
        }

        [TestMethod]
        public void TestGenerateFileNameDoesNotIncludeExtension()
        {
            var initialFileName = @"Assets\test.csv";

            var filename = this.fixer.GenerateFilePath(initialFileName);

            Assert.IsTrue(filename.Count(x => x == '.') == 1, "There were more than one extension in the string: " + filename);
            Assert.IsTrue(filename.EndsWith(@"Assets\test_fixed.csv"), "Filename failed to end with expected suffix. Filename: " + filename);
        }

        [TestMethod]
        public void TestCorrectLineNotChanged()
        {
            var expected = "Participant id,Technique,Granularity,Article count,Trial no,Stimuli,User Response,Trial Start Time,Trial End Time,Trial Time,Accuracy";

            var fixedLine = this.fixer.TryFixLine(expected);

            Assert.AreEqual(expected, fixedLine);
        }

        [TestMethod]
        public void TestIncorrectLineChanged()
        {
            var expected = @"p01,AUTOCOMPASTE,sentence,3,9,What is AC generator? Device used to transform mechanical energy into AC electrical power.,What is AC generator? Device used to transform mechanical energy into AC electrical power.,1474940000035,1474940008083,8048,1";
            var line = @"""p01"",""AUTOCOMPASTE"",""sentence"",""3"",""9"",""What is AC generator? Device used to transform mechanical energy into AC electrical power."",""What is AC generator? Device used to transform mechanical energy into AC electrical power."",""1474940000035"",""1474940008083"",""8048"",""1""";

            var fixedLine = this.fixer.TryFixLine(line);

            Assert.AreEqual(expected, fixedLine);
        }

        [TestMethod]
        public void TestIncorrectLineWithCommaInSentenceChanged()
        {
            var expected = "p01,TRADITIONAL,sentence,3,26,\"What is amplifier? A circuit that increases the voltage, current, or power of a signal.\",\"What is amplifier? A circuit that increases the voltage, current, or power of a signal.\",1474939764660,1474939776237,11577,1";
            var line = "\"\"\"p01\"\"\",\"\"\"TRADITIONAL\"\"\",\"\"\"sentence\"\"\",\"\"\"3\"\"\",\"\"\"26\"\"\",\"\"\"What is amplifier? A circuit that increases the voltage, current, or power of a signal.\"\"\",\"\"\"What is amplifier? A circuit that increases the voltage, current, or power of a signal.\"\"\",\"\"\"1474939764660\"\"\",\"\"\"1474939776237\"\"\",\"\"\"11577\"\"\",\"\"\"1\"\"\"";

            var fixedLine = this.fixer.TryFixLine(line);

            Assert.AreEqual(expected, fixedLine);
        }

        [TestMethod]
        public void TestEmptyLine()
        {
            var lines = new string[]
            {
                "\"\"\"p04\"\"\",\"\"\"AUTOCOMPASTE\"\"\",\"\"\"paragraph\"\"\",\"\"\"6\"\"\",\"\"\"35\"\"\",\"\"\"What is alligator clip? Spring clip on the end of a test lead used to make a temporary connection.\"\"\",\"\"\"\"\"\",\"\"\"1474939776492\"\"\",\"\"\"1474939776682\"\"\",\"\"\"190\"\"\",\"\"\"0\"\"\"",
                "\"\"\"p04\"\"\",\"\"\"AUTOCOMPASTE\"\"\",\"\"\"paragraph\"\"\",\"\"\"6\"\"\",\"\"\"36\"\"\",\"\"\"What is AC/DC? Equipment that will operate on either an AC or DC power source.\"\"\",\"\"\"What is AC/DC? Equipment that will operate on either an AC or DC power source.\"\"\",\"\"\"1474939776682\"\"\",\"\"\"1474939789467\"\"\",\"\"\"12785\"\"\",\"\"\"1\"\"\""
            };

            var fullString = lines.Select(x => this.fixer.TryFixLine(x)).Aggregate((x1, x2) => x1 + "\n" + x2);

            var expectedLine = "p04,AUTOCOMPASTE,paragraph,6,35,What is alligator clip? Spring clip on the end of a test lead used to make a temporary connection.,,1474939776492,1474939776682,190,0\np04,AUTOCOMPASTE,paragraph,6,36,What is AC/DC? Equipment that will operate on either an AC or DC power source.,What is AC/DC? Equipment that will operate on either an AC or DC power source.,1474939776682,1474939789467,12785,1";
            Assert.AreEqual(expectedLine, fullString);
        }

        [TestMethod]
        public void TestStartEmptyLine()
        {
            var lines = new string[]
            {
                "\"\"\"\"\"\",\"\"\"AUTOCOMPASTE\"\"\",\"\"\"paragraph\"\"\",\"\"\"6\"\"\",\"\"\"35\"\"\",\"\"\"What is alligator clip? Spring clip on the end of a test lead used to make a temporary connection.\"\"\",\"\"\"abc\"\"\",\"\"\"1474939776492\"\"\",\"\"\"1474939776682\"\"\",\"\"\"190\"\"\",\"\"\"0\"\"\"",
                "\"\"\"p04\"\"\",\"\"\"AUTOCOMPASTE\"\"\",\"\"\"paragraph\"\"\",\"\"\"6\"\"\",\"\"\"36\"\"\",\"\"\"What is AC/DC? Equipment that will operate on either an AC or DC power source.\"\"\",\"\"\"What is AC/DC? Equipment that will operate on either an AC or DC power source.\"\"\",\"\"\"1474939776682\"\"\",\"\"\"1474939789467\"\"\",\"\"\"12785\"\"\",\"\"\"1\"\"\""
            };

            var fullString = lines.Select(x => this.fixer.TryFixLine(x)).Aggregate((x1, x2) => x1 + "\n" + x2);

            var expectedLine = ",AUTOCOMPASTE,paragraph,6,35,What is alligator clip? Spring clip on the end of a test lead used to make a temporary connection.,abc,1474939776492,1474939776682,190,0\np04,AUTOCOMPASTE,paragraph,6,36,What is AC/DC? Equipment that will operate on either an AC or DC power source.,What is AC/DC? Equipment that will operate on either an AC or DC power source.,1474939776682,1474939789467,12785,1";
            Assert.AreEqual(expectedLine, fullString);
        }

        [TestMethod]
        public void TestEndEmptyLine()
        {
            var lines = new string[]
            {
                "\"\"\"p04\"\"\",\"\"\"AUTOCOMPASTE\"\"\",\"\"\"paragraph\"\"\",\"\"\"6\"\"\",\"\"\"35\"\"\",\"\"\"What is alligator clip? Spring clip on the end of a test lead used to make a temporary connection.\"\"\",\"\"\"abc\"\"\",\"\"\"1474939776492\"\"\",\"\"\"1474939776682\"\"\",\"\"\"190\"\"\",\"\"\"\"\"\"",
                "\"\"\"p04\"\"\",\"\"\"AUTOCOMPASTE\"\"\",\"\"\"paragraph\"\"\",\"\"\"6\"\"\",\"\"\"36\"\"\",\"\"\"What is AC/DC? Equipment that will operate on either an AC or DC power source.\"\"\",\"\"\"What is AC/DC? Equipment that will operate on either an AC or DC power source.\"\"\",\"\"\"1474939776682\"\"\",\"\"\"1474939789467\"\"\",\"\"\"12785\"\"\",\"\"\"1\"\"\""
            };

            var fullString = lines.Select(x => this.fixer.TryFixLine(x)).Aggregate((x1, x2) => x1 + "\n" + x2);

            var expectedLine = "p04,AUTOCOMPASTE,paragraph,6,35,What is alligator clip? Spring clip on the end of a test lead used to make a temporary connection.,abc,1474939776492,1474939776682,190,\np04,AUTOCOMPASTE,paragraph,6,36,What is AC/DC? Equipment that will operate on either an AC or DC power source.,What is AC/DC? Equipment that will operate on either an AC or DC power source.,1474939776682,1474939789467,12785,1";
            Assert.AreEqual(expectedLine, fullString);
        }

        [TestMethod]
        public void TestStartAndEndEmptyLine()
        {
            var lines = new string[]
            {
                "\"\"\"\"\"\",\"\"\"AUTOCOMPASTE\"\"\",\"\"\"paragraph\"\"\",\"\"\"6\"\"\",\"\"\"35\"\"\",\"\"\"What is alligator clip? Spring clip on the end of a test lead used to make a temporary connection.\"\"\",\"\"\"abc\"\"\",\"\"\"1474939776492\"\"\",\"\"\"1474939776682\"\"\",\"\"\"190\"\"\",\"\"\"\"\"\"",
                "\"\"\"p04\"\"\",\"\"\"AUTOCOMPASTE\"\"\",\"\"\"paragraph\"\"\",\"\"\"6\"\"\",\"\"\"36\"\"\",\"\"\"What is AC/DC? Equipment that will operate on either an AC or DC power source.\"\"\",\"\"\"What is AC/DC? Equipment that will operate on either an AC or DC power source.\"\"\",\"\"\"1474939776682\"\"\",\"\"\"1474939789467\"\"\",\"\"\"12785\"\"\",\"\"\"1\"\"\""
            };

            var fullString = lines.Select(x => this.fixer.TryFixLine(x)).Aggregate((x1, x2) => x1 + "\n" + x2);

            var expectedLine = ",AUTOCOMPASTE,paragraph,6,35,What is alligator clip? Spring clip on the end of a test lead used to make a temporary connection.,abc,1474939776492,1474939776682,190,\np04,AUTOCOMPASTE,paragraph,6,36,What is AC/DC? Equipment that will operate on either an AC or DC power source.,What is AC/DC? Equipment that will operate on either an AC or DC power source.,1474939776682,1474939789467,12785,1";
            Assert.AreEqual(expectedLine, fullString);
        }
    }
}
