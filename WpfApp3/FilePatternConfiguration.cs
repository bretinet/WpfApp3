using System;

namespace WpfApp3
{
    [Serializable]
    internal class FilePatternConfiguration 
    {
        public string DefaultFolder { get; set; }

        public string DefaultFilterPattern { get; set; }

        public string DefaultUrl { get; set; }

        public bool IncludeSubFolders { get; set; }
    }
}