using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    [Serializable]
    class ComparePatterns
    {
        public string Pattern { get; set; }

        public string Result { get; set; }

        public bool ActiveLocal { get; set; }

        public bool ActiveRemote { get; set; }

        public bool Case { get; set; }

       

    }
}
