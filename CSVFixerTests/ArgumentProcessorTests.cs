using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSVFixer;
using System.Linq;

namespace CSVFixerTests
{
    [TestClass]
    public class ArgumentProcessorTests
    {
        private ArgumentProcessor processor;

        [TestInitialize]
        public void SetupTests()
        {
            this.processor = new ArgumentProcessor();
        }

        [TestMethod]
        public void TestFilenamesGivenForValidFiles()
        {
            var expectedFilenames = new string[]
            {
                @"Assets\test.txt",
                @"Assets\test.csv"
            };

            var fileNames = this.processor.GetFileNames(expectedFilenames);

            Assert.IsNotNull(fileNames, "Filenames was not able to be retrieved");
            Assert.AreEqual(expectedFilenames.Length, fileNames.Count());

            for (int i = 0; i < expectedFilenames.Length; i++)
            {
                var expected = expectedFilenames[i];
                var candidate = fileNames.ElementAt(i);

                Assert.AreEqual(expected, candidate, "Wrong file or order retrieved");
            }
        }

        [TestMethod]
        public void TestNullGivenForInvalidFiles()
        {
            var expectedFilenames = new string[]
            {
                @"Assets\test2.txt",
                @"Assets\test2.csv"
            };

            var fileNames = this.processor.GetFileNames(expectedFilenames);

            Assert.IsNull(fileNames, "Filenames was retrieved from invalid items");
        }

        [TestMethod]
        public void TestCorrectFileGivenWithinInvalidFiles()
        {
            var expectedFilenames = new string[]
            {
                @"Assets\test2.txt",
                @"Assets\test.csv",
                @"Assets\test3.pdf"
            };

            var fileNames = this.processor.GetFileNames(expectedFilenames);

            Assert.IsNotNull(fileNames, "Filename was not able to be retrieved");
            Assert.AreEqual(1, fileNames.Count());

            Assert.AreEqual(expectedFilenames[1], fileNames.ElementAt(0), "Wrong file or order retrieved");
        }
    }
}
