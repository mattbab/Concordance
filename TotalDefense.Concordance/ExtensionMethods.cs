namespace TotalDefense.Concordance
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class ExtensionMethods
    {
        public static IEnumerable<char> Chars(this TextReader reader)
        {
            return Chars(reader, shouldLowercase: false);
        } 

        public static IEnumerable<char> Chars(this TextReader reader, bool shouldLowercase)
        {
            if (reader == null)
            {
                // C# 6: Replace "reader" constant with `nameof(reader)` so we don't have to change the string to keep 
                //       the values in sync.
                throw new ArgumentNullException("reader");
            }

            // NOTE: `reader.Peek` will return -1 when the file is empty
            // NOTE: `reader.Peek` may throw an IOException
            while (reader.Peek() >= 0)
            {
                char chr = (char)reader.Read();

                if (shouldLowercase)
                {
                    chr = Char.ToLower(chr);
                }

                yield return chr;
            }
        }
    }
}
