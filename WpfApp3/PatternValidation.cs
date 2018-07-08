using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp3
{
    internal class PatternValidation
    {
        internal bool IsMatchingPattern(
                                string pattern, 
                                string text, 
                                RegexOptions options = RegexOptions.None,
                                TimeSpan timeSpan = default(TimeSpan))
        {
            Regex regex;

            if (timeSpan != default(TimeSpan))
            {
                regex = new Regex(pattern, options, timeSpan);
            }
            else
            {
                regex = new Regex(pattern, options);
            }

            var match = regex.Match(text);

            return match.Success;

        }
    }
}
