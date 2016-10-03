using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVFixer
{
    public class ArgumentProcessor
    {
        public IEnumerable<string> GetFileNames(string[] args)
        {
            var filenames = args.Where(x => File.Exists(x));
            if (filenames.Count() == 0)
                return null;

            return filenames;
        }
    }
}
