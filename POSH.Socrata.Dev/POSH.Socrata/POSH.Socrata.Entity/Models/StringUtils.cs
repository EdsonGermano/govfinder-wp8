using System;
using System.Text;
using System.Text.RegularExpressions;

namespace POSH.Socrata.Entity.Models
{
    public class StringUtils
    {
        /// <summary>
        /// Removes leading and trailing whitespace
        /// and returns empty string if input is null.
        /// </summary>
        public static string trim(string s)
        {
            return safe(s).Trim();
        }

        public static string ElipsizeString(string input, int maxchars)
        {
            if (input.Length > maxchars)
            {
                return input.Substring(0, maxchars) + "…";
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Reduces multiple consecutive whitespace into single space
        /// and returns emptry string if input is null.
        /// </summary>
        public static string RemoveExcessWhitespace(string s)
        {
            string input = trim(s);
            string pattern = "\\s+";
            string replacement = " ";

            Regex rgx = new Regex(pattern);
            return rgx.Replace(input, replacement);
        }

        /// <summary>
        /// Removes all line breaks from string
        /// and returns emptry string if input is null.
        /// </summary>
        public static string RemoveAllLineBreak(string s)
        {
            string pattern = "\\n+";
            string replacement = " ";

            Regex rgx = new Regex(pattern);
            return rgx.Replace(safe(s), replacement);
        }

        /// <summary>
        /// Reduces multiple linebreaks into single line breaks
        /// and returns empty string if input is null.
        /// </summary>
        public static string RemoveMoreThanSingleLineBreak(string s)
        {
            string pattern = "\\n+";
            string replacement = "\n";

            Regex rgx = new Regex(pattern);
            return rgx.Replace(safe(s), replacement);
        }

        /// <summary>
        /// Reduces multiple linebreaks into single line breaks
        /// and returns empty string if input is null.
        /// </summary>
        public static string ConvertAllLinebreaksToDouble(string s)
        {
            string pattern = "\\n+";
            string replacement = "\n\n";

            Regex rgx = new Regex(pattern);
            return rgx.Replace(safe(s), replacement);
        }

        /// <summary>
        /// Returns empty string if input is null.
        /// </summary>
        public static string safe(string s)
        {
            if (s == null)
                return string.Empty;

            return s;
        }

        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        public static string SanitizeXmlString(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            StringBuilder buffer = new StringBuilder(xml.Length);

            foreach (char c in xml)
            {
                if (IsLegalXmlChar(c))
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        private static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }
    }
}