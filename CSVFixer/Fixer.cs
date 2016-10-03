using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVFixer
{
    public class Fixer
    {
        private const string DEFAULT_SUFFIX = "_fixed";

        public Fixer()
        {
        }

        public bool TryFix(IEnumerable<string> files)
        {
            foreach (var filename in files)
            {
                bool success = TryFixFile(filename);
                if (!success)
                    return false;   // No need to continue if it fails.
            }

            return true;
        }

        private bool TryFixFile(string filename)
        {
            bool inPractice = true; // Assumes top is in practice.
            string saveFile = GenerateFilePath(filename);
            string line1 = null;

            using (var reader = new StreamReader(filename))
            using (var writer = new StreamWriter(saveFile))
            {
                string line = null;
                while (true)
                {
                    line = reader.ReadLine();
                    if (line == null)   // Read till there's nothing
                        break;

                    string fixedLine = TryFixLine(line);

                    if (line1 == null)
                        line1 = fixedLine;   // Store the header
                    else
                    {   // Do in practice check
                        if (fixedLine == line1)
                        {
                            inPractice = false; // No longer in practice.
                        }
                    }

                    bool writeLine = false;
                    if (inPractice && fixedLine == line1)
                    {
                        writeLine = true;
                    }
                    if (!inPractice && fixedLine != line1)
                    {
                        writeLine = true;
                    }

                    if (writeLine)
                    {   // Only saves if it's header or a valid not in practice line.
                        writer.WriteLine(fixedLine);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Try to fix the line by removing excessive double quotes. Assumption is that there's no empty quotes
        /// </summary>
        /// <param name="line">Raw line to fix</param>
        /// <returns>Line with no more double quote.</returns>
        public string TryFixLine(string line)
        {
            int pointer = 0;
            bool inQuote = false;
            StringBuilder build = new StringBuilder(line.Length);
            bool prevIsQuote = false;
            StringBuilder block = new StringBuilder();
            bool hasAComma = false;

            while (pointer < line.Length)
            {
                if (line[pointer] == '"')
                {
                    if (inQuote)
                    {
                        if (prevIsQuote)
                        {
                            // Continue as it is still in quote and previously was in quote
                        }
                        else
                        {
                            if (hasAComma)  // If has a comma, we need to encase in double quote
                                build.Append('"');
                            build.Append(block.ToString());
                            if (hasAComma)
                                build.Append('"');

                            inQuote = false;    // No longer in quote as there was something in between.
                            block.Clear();
                            hasAComma = false;
                        }
                    }
                    else
                    {   // Not in quote
                        if (prevIsQuote == false)
                        { // current pointer is a double quote. Also check for previous character. If previous character is a double quote, we just exited in quote
                            inQuote = true;
                            block.Clear();
                            hasAComma = false;
                        }
                        else
                        {
                            // Not doing anything here as quote is repeated
                        }
                    }
                }
                else
                {
                    if (inQuote)
                    {
                        if (line[pointer] == ',')
                            hasAComma = true;   // Found at least 1 comma in this block

                        block.Append(line[pointer]);
                    }
                    else
                        build.Append(line[pointer]);
                }

                prevIsQuote = line[pointer] == '"'; // Setup for next pointer
                pointer++;  // Move to the next character
            }

            return build.ToString();
        }

        public string GenerateFilePath(string filename)
        {
            var fileInfo = new FileInfo(filename);
            var directory = fileInfo.DirectoryName;

            var newFilename = fileInfo.Name.Insert(fileInfo.Name.Length - fileInfo.Extension.Length, DEFAULT_SUFFIX); // Insert suffix just before the extension.

            return Path.Combine(directory, newFilename);
        }
    }
}