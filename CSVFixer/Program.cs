using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file(s) from args
            var argProcessor = new ArgumentProcessor();
            IEnumerable<string> files = argProcessor.GetFileNames(args);
            if (files == null)
            {
                Console.WriteLine("There are no files to process");
                return;
            }

            // process file(s)
            // save file(s)
            var fixer = new Fixer();
            bool success = fixer.TryFix(files);
            if (success)
                Console.WriteLine("The provided files were fixed.");
            else
                Console.WriteLine("The provided files failed to be fixed.");
        }
    }
}
