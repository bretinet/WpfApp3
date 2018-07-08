using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3.Extensions
{
    public static class FileNameExtension
    {
        public static string CleanFileName(this string filename)
        {
            const string False = " (False)";
            const string True = " (True)";
            const string Warning = " (Warning)";
            const string Error = " (Error)";

            return filename
                .Replace(False, string.Empty)
                .Replace(True, string.Empty)
                .Replace(Warning, string.Empty)
                .Replace(Error, string.Empty);
        }
    }
}
