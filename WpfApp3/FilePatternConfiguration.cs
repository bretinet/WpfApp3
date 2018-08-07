using System;

namespace WpfApp3
{
    [Serializable]
    internal class FilePatternConfiguration 
    {
        public string RootFolder { get; set; }

        public string FilterPattern { get; set; }

        public string UrlBaseAddress { get; set; }

        public bool IncludeSubFolders { get; set; }
    }
}