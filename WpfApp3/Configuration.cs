using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    class Configuration
    {
        private static readonly Configuration instance = new Configuration();

        private Configuration()
        {
        }

        public static Configuration Instance
        {
            get
            {
                return instance;
            }
        }

        public List<string> SelectedFiles { get; set; }

        public List<string> UrlFiles { get; set; }

        public FilePatternConfiguration FileConfiguration { get; set; }

        public FilePatternConfiguration DefaultFileConfiguration { get; set; }

    }
}
