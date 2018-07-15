using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3.Utilities
{
    internal static class IoUtilities
    {
        public static string ReadStringFromFile(string path)
        {
            try
            {
                var file = new StreamReader(path);
                var task = file.ReadToEndAsync();

                var text = task.ContinueWith((s) =>

                    {
                        file.Close();
                        return s.Result;
                    }
                );

                return text.Result;
            }
            catch
            {
                return null;
            }

        }


        public static void WriteStringToFile(string path, string content)
        {
            try
            {
                var file = new StreamWriter(path);
                file.Write(content);
                file.Flush();
                file.Close();
                file.Dispose();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
