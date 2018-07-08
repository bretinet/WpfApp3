using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    internal class Persistence<T> where T: class
    {
        internal T GetConfigurationValues(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                //throw new FileNotFoundException();
                return null;
            }

            T type;
            using (var file = File.Open(path, FileMode.Open))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                type = (T) formatter.Deserialize(file);
            }

            return type;
        }

        internal void SetConfigurationValues(string path, T value)
        {
            using (var file = File.Open(path, FileMode.OpenOrCreate))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                formatter.Serialize(file, value);
            }
        }
    }
}
