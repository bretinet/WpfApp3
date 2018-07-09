using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    public class PattenFolderFactory
    {
        private const string PatterFolderName = "PatternFolder.bin";

        public IEnumerable<PatternFolder> ReadPatternFolders()
        {
            var patternFolders = new List<PatternFolder>();

            using (Stream stream = File.Open(PatterFolderName, FileMode.OpenOrCreate))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                if (stream.Length > 0)
                {
                    patternFolders = (List<PatternFolder>)formatter.Deserialize(stream);
                }
            }

            return patternFolders;
        }

        public void WritePatternFolders(IEnumerable<PatternFolder> patternFolders)
        {
            using (Stream stream = File.Open(PatterFolderName, FileMode.Create))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, patternFolders);
            }
        }
    }
}
