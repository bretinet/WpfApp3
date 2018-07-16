using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    [Serializable]
    internal class FolderLoadingConfiguration
    {
        internal FolderLoadingOptions FolderLoadingOptions { get; set; }

        internal List<string> FolderList { get; set; }

        public FolderLoadingConfiguration()
        {
            FolderLoadingOptions = FolderLoadingOptions.AllFolders;
        }

    }

    internal enum FolderLoadingOptions
    {
        AllFolders,
        OnlySelectedFolders,
        AllFolderExcept
    }
}
