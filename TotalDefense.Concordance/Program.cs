namespace TotalDefense.Concordance
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                // We could extend this to accept the input text from stdin instead but for brevity we'll just tell
                // the user what they need to do.
                Console.Error.WriteLine(
                    "Please specify the path to a single file containing the text use to generate the concordance.");
                Environment.Exit(-1);
            }

            TextReader reader = null;
            bool hasError = false;

            // This could be handled with a standard `using(...){}` block, however, there's no point in the compiler 
            // generating a `try...finally` if we need to catch exceptions anyway.
            try
            {
                // Open the file and wrap it in a reader
                // NOTE: We could have just read the contents of the file with `File.ReadAllText` and avoided much of 
                //       the ceremony, however, if the file was too big to fit into our process' address space it would
                //       fail with an exception.
                reader = new StreamReader(args[0]);

                // Instantiate our concordance generator
                // NOTE: The use of an interface and separate class for this logic may be good practice but also feels 
                //       like overkill for this simple app.
                IConcordanceGenerator generator = new ConcordanceGenerator();

                // Generate the concordance based on the data in our stream
                var concordance = generator.Generate(reader);

                concordance.OrderBy(x => x.Key).Select((x, i) => $"{ToBase26(i+1)}. {x.Key} {{{x.Value.Count()}:{string.Join(",", x.Value)}}}").ToList().ForEach(Console.WriteLine);
            }
            catch (IOException)
            {
                // This catch will trip if exception is one of:
                //     * IOException - The path contains invalid syntax
                //     * FileNotFoundException - The path references a file which does not exist
                //     * DirectoryNotFoundException - The path references a file in a directory or drive which does
                //                                    not exist or is not available.
                Console.Error.WriteLine("The specified filename is invalid or does not exist.");

                // TODO: It may be worth logging the actual exception somewhere for diagnostic purposes

                hasError = true;
            }
            catch (ArgumentException)
            {
                // This catch will trip if exception is either a ArgumentException (file name was blank) or 
                // ArgumentNullException (file name was null)
                Console.Error.WriteLine("Please specify the path to a single file containing the text use to generate the concordance.");

                // TODO: It may be worth logging the actual exception somewhere for diagnostic purposes

                hasError = true;
            }
            finally
            {
                // C# 6: Replace the following conditional block with: reader?.Dispose();
                if (reader != null)
                {
                    reader.Dispose();
                }
            }

            if (hasError)
            {
                Environment.Exit(-1);
            }
        }

        private static string ToBase26(int myNumber)
        {
            var array = new LinkedList<int>();

            while (myNumber > 26)
            {
                int value = myNumber % 26;
                if (value == 0)
                {
                    myNumber = myNumber / 26 - 1;
                    array.AddFirst(26);
                }
                else
                {
                    myNumber /= 26;
                    array.AddFirst(value);
                }
            }

            if (myNumber > 0)
            {
                array.AddFirst(myNumber);
            }
            return new string(array.Select(s => (char)('a' + s - 1)).ToArray());
        }
    }
}
