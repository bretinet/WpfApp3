using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    class SourceCodeFile
    {
        public string Name { get; set; }

        public FileInfo FileInfo { get; set; }

        public bool IsFileImplementingPatterns { get; set; }
    }
}
 