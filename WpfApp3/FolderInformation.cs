using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    internal class FolderInformation
    {
        //internal IEnumerable<> GetFolders(DirectoryInfo directoryInfo)
        //{
        //    try
        //    {
        //        if (IncludeSubFolders)
        //        {
        //            var subdirectories = directoryInfo.GetDirectories();
        //            foreach (var folder in subdirectories)
        //            {
        //                var currentFolder = Path.Combine(directory, folder.Name);

        //                GetFoldersAndFiles(currentFolder);

        //            }
        //        }
        //    }
        //    catch { }


        //}

        internal bool IsFolderAccessible (string folder, FileSystemRights rights)
        {
            var accessControl = Directory.GetAccessControl(folder, AccessControlSections.Access);

            var folderAccessRules = accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            var currentUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //var ttt = ffff.Cast<AuthorizationRule>().ToList().Where(x=>x.IdentityReference.Value.Equals(sss));
            var userRules = folderAccessRules.Cast<FileSystemAccessRule>().ToList().Where(x => x.IdentityReference.Value.Equals(currentUserName)).ToList();

            if (userRules.Any() && (userRules.First().FileSystemRights & rights) > 0)
            {
                return true;
                //var subdirectories = directoryInfo.GetDirectories();
                //foreach (var folder in subdirectories)
                //{
                //    var currentFolder = Path.Combine(directory, folder.Name);

                //    GetFoldersAndFiles(currentFolder);

                //}
            }
            return false;
        }
    }
}
